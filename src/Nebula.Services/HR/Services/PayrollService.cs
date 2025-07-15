using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.HR;
using Nebula.Services.HR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nebula.Services.HR.Services
{
    public class PayrollService : PayrollInterface.PayrollInterfaceBase
    {
        private readonly ILogger<PayrollService> _logger;
        private readonly IPayrollRepository _payrolls;

        public PayrollService(ILogger<PayrollService> logger, IPayrollRepository payrolls)
        {
            _logger = logger;
            _payrolls = payrolls;
        }

        // ...existing PayrollService methods from HRService...
    }
}
