using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pb = global::Google.Protobuf;

namespace Nebula.Services.Fragments.Authentication
{
    public sealed partial class UserRecord : pb::IMessage<UserRecord>
    {
        public Guid UserIdGuid
        {
            get => Guid.Parse(Public.UserId);
            set => Public.UserId = value.ToString();
        }
    }
}
