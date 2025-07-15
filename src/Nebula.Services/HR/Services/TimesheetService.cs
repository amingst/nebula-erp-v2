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
    public class TimesheetService : TimesheetInterface.TimesheetInterfaceBase
    {
        private readonly ILogger<TimesheetService> _logger;
        private readonly ITimesheetRepository _timesheets;

        public TimesheetService(ILogger<TimesheetService> logger, ITimesheetRepository timesheets)
        {
            _logger = logger;
            _timesheets = timesheets;
        }

        // ...existing TimesheetService methods from HRService...
    }
}
