using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Authentication.Shared;
using Nebula.Services.Authentication.Shared.Helpers;
using Nebula.Services.Fragments.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nebula.Authentication.Services
{
    public class ClaimsClient
    {
        public const string CLAIMS_SERVICE_LIST = "CLAIMS_SERVICE_LIST";
        public const string CHANNEL_NAME_CHAT = "chat";
        public const string CHANNEL_NAME_PAYMENT = "payment";

        private readonly ILogger logger;
        public readonly NebulaUser User;

        public ClaimsClient(NebulaUserHelper userHelper, ILogger<ClaimsClient> logger)
        {
            this.logger = logger;

            User = userHelper.MyUser;
        }

        public async Task<IEnumerable<ClaimRecord>> GetOtherClaims(Guid userId)
        {
            List<Channel> channels = GetChannels();

            var tasks = channels.Select(c => GetOtherClaims(userId, c));

            await Task.WhenAll(tasks);

            Dictionary<string, ClaimRecord> dict = new Dictionary<string, ClaimRecord>();

            foreach (var t in tasks)
            {
                foreach (var claim in await t)
                {
                    if (!dict.ContainsKey(claim.Name))
                    {
                        dict[claim.Name] = claim;
                        continue;
                    }

                    if (dict[claim.Name].ExpiresOnUTC < claim.ExpiresOnUTC)
                    {
                        dict[claim.Name] = claim;
                    }
                }
            }

            return dict.Values;
        }

        private async Task<IEnumerable<ClaimRecord>> GetOtherClaims(Guid userId, Channel channel)
        {
            if (channel == null)
                return new ClaimRecord[0];

            try
            {
                var client = new ClaimsInterface.ClaimsInterfaceClient(channel);
                var reply = await client.GetClaimsAsync(new GetClaimsRequest()
                {
                    UserID = userId.ToString()
                });

                return reply.Claims;
            }
            catch (Exception)
            {

            }

            return new ClaimRecord[0];
        }

        private List<Channel> GetChannels()
        {
            List<Channel> channels = new List<Channel>();

            var channelNames = GetStringFromEnvVar(CLAIMS_SERVICE_LIST)?.Split(',') ?? new string[0];
            foreach (var name in channelNames)
            {
                switch (name)
                {
                    case CHANNEL_NAME_CHAT:
                        //channels.Add(settingsService.ChatServiceChannel);
                        break;
                    case CHANNEL_NAME_PAYMENT:
                        //channels.Add(settingsService.PaymentServiceChannel);
                        break;
                }
            }

            return channels;
        }

        private string GetStringFromEnvVar(string varName)
        {
            var envVar = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.Process);

            return envVar;
        }
    }
}
