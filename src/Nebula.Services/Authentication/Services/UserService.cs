using Grpc.Core;
using Microsoft.Extensions.Logging;
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

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<AuthenticateUserResponse> AuthenticateUser(AutheticateUserRequest request, ServerCallContext context)
        {
            return base.AuthenticateUser(request, context);
        }
    }
}
