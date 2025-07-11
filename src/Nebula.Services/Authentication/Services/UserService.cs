using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Authentication.Services.Data;
using Nebula.Services.Fragments.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Services
{
    public class UserService : UserInterface.UserInterfaceBase
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _users;

        public UserService(ILogger<UserService> logger, IUserRepository users)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public override Task<AuthenticateUserResponse> AuthenticateUser(AutheticateUserRequest request, ServerCallContext context)
        {
            return base.AuthenticateUser(request, context);
        }

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
                },
                Private = new UserPrivateRecord
                {
                    Email = request.Email,
                },
                Server = new UserServerRecord
                {
                }
            };

            var created = await _users.Create(newUser);
            if (!created)
            {
                _logger.LogError("Failed to create user {UserName} with email {Email}", request.UserName, request.Email);
                return new AuthenticateUserResponse { Error = "Failed to create user" };
            }

            return new AuthenticateUserResponse()
            {
                Error = "No Error",
                Token = "TODO: GENERATE TOKEN"
            };
        }
    }
}
