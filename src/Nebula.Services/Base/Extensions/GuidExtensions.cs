using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Base.Extensions
{
    public static class GuidExtensions
    {
        public static Guid ToGuid(this string id)
        {
            Guid guid = Guid.Empty;
            Guid.TryParse(id, out guid);
            return guid;
        }

    }
}
