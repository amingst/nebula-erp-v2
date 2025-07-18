using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Services.Data.Postgres
{
    public static class UserEntityExtensions
    {
        public static UserPublicRecord ToPublicRecord(this UserEntity entity)
        {
            return new UserPublicRecord
            {
                UserId = entity.Id.ToString(),
                UserName = entity.UserName,
                DisplayName = entity.DisplayName,
                FirstName = entity.FirstName ?? string.Empty,
                LastName = entity.LastName ?? string.Empty,
                Identites = { entity.Identities ?? new List<string>() },
                CreatedUTC = DateTime.SpecifyKind(entity.CreatedUtc, DateTimeKind.Utc).ToTimestamp(),
                LastModifiedUTC = DateTime.SpecifyKind(entity.LastModifiedUtc, DateTimeKind.Utc).ToTimestamp(),
                LastLoginUTC = DateTime.SpecifyKind(entity.LastLoginUtc, DateTimeKind.Utc).ToTimestamp(),
                DisabledUtc = DateTime.SpecifyKind(entity.DisabledUtc, DateTimeKind.Utc).ToTimestamp(),
            };
        }

        public static UserPrivateRecord ToPrivateRecord(this UserEntity entity)
        {
            return new UserPrivateRecord
            {
                Email = entity.Email,
                Roles = { entity.Roles ?? new List<string>() },
            };
        }

        public static UserServerRecord ToServerRecord(this UserEntity entity)
        {
            return new UserServerRecord
            {
                PasswordHash = ByteString.CopyFrom(entity.PasswordHash),
                PasswordSalt = ByteString.CopyFrom(entity.Salt),
            };
        }

        public static UserFullRecord ToFullRecord(this UserEntity entity)
        {
            return new UserFullRecord
            {
                Public = entity.ToPublicRecord(),
                Private = entity.ToPrivateRecord(),
                Server = entity.ToServerRecord()
            };
        }

        public static UserRecord ToUserRecord(this UserEntity entity)
        {
            return new UserRecord
            {
                Public = entity.ToPublicRecord(),
                Private = entity.ToPrivateRecord(),
            };
        }
    }

    public static class UserRecordExtensions
    {
        public static UserEntity ToEntity(this UserFullRecord record)
        {
            return new UserEntity
            {
                Id = record.Public.UserId.ToGuid(),
                UserName = record.Public.UserName,
                Email = record.Private.Email,
                DisplayName = record.Public.DisplayName,
                FirstName = record.Public.FirstName ?? string.Empty,
                LastName = record.Public.LastName ?? string.Empty,
                CreatedUtc = record.Public.CreatedUTC.ToDateTime().ToUniversalTime(),
                LastModifiedUtc = record.Public.LastModifiedUTC.ToDateTime().ToUniversalTime(),
                LastLoginUtc = record.Public.LastLoginUTC.ToDateTime().ToUniversalTime(),
                DisabledUtc = record.Public.DisabledUtc.ToDateTime().ToUniversalTime(),
                Roles = record.Private.Roles.ToList(),
                Identities = record.Public.Identites.ToList(),
                PasswordHash = record.Server.PasswordHash.ToByteArray(),
                Salt = record.Server.PasswordSalt.ToByteArray()
            };
        }
    }
}
