using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nebula.Services.Authentication.Services.Data;
using Nebula.Services.Authentication.Shared;
using Nebula.Services.Authentication.Shared.Extensions;
using Nebula.Services.Authentication.Shared.Helpers;
using Nebula.Services.Fragments.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Services
{
    [Authorize]
    public class UserService : UserInterface.UserInterfaceBase
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _users;
        private readonly SigningCredentials _creds;
        private static readonly HashAlgorithm hasher = SHA256.Create();
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public UserService(ILogger<UserService> logger, IUserRepository users)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _creds = new SigningCredentials(JwtExtensions.GetPrivateKey(), SecurityAlgorithms.EcdsaSha256);
        }

        [AllowAnonymous]
        public override async Task<AuthenticateUserResponse> AuthenticateUser(AutheticateUserRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                return new AuthenticateUserResponse() { Error = "Username or Password Undefined"};

            var user = await _users.GetByUserName(request.UserName);
            if (user == null)
            {
                user = await _users.GetByEmail(request.UserName);
                if (user == null)
                    return new AuthenticateUserResponse() { Error = "User not found" };
            }

            bool isCorrect = await IsPasswordCorrect(request.Password, user);
            if (!isCorrect) {                 
                _logger.LogWarning("Failed login attempt for user {UserName}", request.UserName);
                return new AuthenticateUserResponse() { Error = "Invalid password" };
            }

            return new AuthenticateUserResponse()
            {
                Error = "No Error",
                Token = GenerateToken(user)
            };
        }

        [AllowAnonymous]
        public override async Task<AuthenticateUserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
        {
            var emailTaken = await _users.EmailTaken(request.Email);
            if (emailTaken)
                return new AuthenticateUserResponse { Error = "Email already taken" };
            var userameTaken = await _users.UserNameTaken(request.UserName);
            if (userameTaken)
                return new AuthenticateUserResponse { Error = "Username already taken" };

            var newUser = new UserFullRecord
            {
                Public = new UserPublicRecord
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = request.UserName,
                    DisplayName = request.DisplayName ?? request.UserName,
                    FirstName = request.FirstName ?? string.Empty,
                    LastName = request.LastName ?? string.Empty,
                    CreatedUTC = DateTime.UtcNow.ToTimestamp(),
                    LastModifiedUTC = DateTime.UtcNow.ToTimestamp(),
                    LastLoginUTC = DateTime.UtcNow.ToTimestamp(),
                    DisabledUtc = new Timestamp() // Default empty timestamp
                },
                Private = new UserPrivateRecord
                {
                    Email = request.Email,
                },
                Server = new UserServerRecord
                {
                }
            };

            // Add default identity and role for all users
            newUser.Public.Identites.Add("user");
            newUser.Private.Roles.Add(NebulaUser.ROLE_ORG_USER);


            byte[] salt = RandomNumberGenerator.GetBytes(16);
            newUser.Server.PasswordSalt = ByteString.CopyFrom(salt);
            newUser.Server.PasswordHash = ByteString.CopyFrom(ComputeSaltedHash(request.Password, salt));

            var created = await _users.Create(newUser);
            if (!created)
            {
                _logger.LogError("Failed to create user {UserName} with email {Email}", request.UserName, request.Email);
                return new AuthenticateUserResponse { Error = "Failed to create user" };
            }

            var token = GenerateToken(newUser);

            return new AuthenticateUserResponse()
            {
                Error = "No Error",
                Token = token
            };
        }

        public override async Task<GetOwnUserResponse> GetOwnUser(GetOwnUserRequest request, ServerCallContext context)
        {
            var res = new GetOwnUserResponse();

            var ownUser = NebulaUserHelper.ParseUser(context.GetHttpContext());
            if (ownUser == null)
            {
                res.Error = "User not authenticated";
                return res;
            }

            var user = await _users.GetById(ownUser.Id);
            if (user == null)
            {
                res.Error = "User not found";
                return res;
            }

            res.Record = user;
            res.Error = "No Error";
            return res;
        }

        private string GenerateToken(UserFullRecord user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            var nebulaUser = new NebulaUser
            {
                Id = Guid.Parse(user.Public.UserId),
                UserName = user.Public.UserName,
                DisplayName = user.Public.DisplayName,
                FirstName = user.Public.FirstName,
                LastName = user.Public.LastName,
            };

            nebulaUser.Idents.AddRange(user.Public.Identites.ToList());
            nebulaUser.Roles.AddRange(user.Private.Roles.ToList());

            //if (otherClaims != null)
            //{
            //    nebulaUser.ExtraClaims.AddRange(otherClaims.Select(c => new Claim(c.Name, c.Value)));
            //    nebulaUser.ExtraClaims.AddRange(otherClaims.Select(c => new Claim(c.Name + "Exp", c.ExpiresOnUTC.Seconds.ToString())));
            //}

            return GenerateToken(nebulaUser);
        }

        private string GenerateToken(NebulaUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenExpiration = DateTime.UtcNow.AddDays(7);
            var claims = user.ToClaims().ToArray();
            var subject = new ClaimsIdentity(claims);
            var token = tokenHandler.CreateJwtSecurityToken(null, null, subject, null, tokenExpiration, DateTime.UtcNow, _creds);

            return tokenHandler.WriteToken(token);
        }

        private byte[] ComputeSaltedHash(string plainText, ReadOnlySpan<byte> salt)
        {
            return ComputeSaltedHash(Encoding.UTF8.GetBytes(plainText), salt);
        }

        private byte[] ComputeSaltedHash(ReadOnlySpan<byte> plainText, ReadOnlySpan<byte> salt)
        {
            byte[] plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            plainText.CopyTo(plainTextWithSaltBytes.AsSpan());
            salt.CopyTo(plainTextWithSaltBytes.AsSpan(plainText.Length));

            return hasher.ComputeHash(plainTextWithSaltBytes);
        }

        private async Task<bool> IsPasswordCorrect(string password, UserFullRecord user)
        {
            var hash = ComputeSaltedHash(password, user.Server.PasswordSalt.Span);
            if (CryptographicOperations.FixedTimeEquals(user.Server.PasswordHash.Span, hash))
                return true;

            return false;
        }
    }
}
