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
    public class EmployeeService : EmployeeInterface.EmployeeInterfaceBase
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly IEmployeeRepository _employees;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employees)
        {
            _logger = logger;
            _employees = employees;
        }

        // ...existing EmployeeService methods from HRService...
    }
}
