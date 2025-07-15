using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Fragments.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Accounting.Services
{
    public class AccountingService : AccountingInterface.AccountingInterfaceBase
    {
        private readonly ILogger<AccountingService> _logger;

        public AccountingService(ILogger<AccountingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<CreateJournalEntryResponse> CreateJournalEntry(CreateJournalEntryRequest request, ServerCallContext context)
        {
            return base.CreateJournalEntry(request, context);
        }

        public override Task<GetChartOfAccountsResponse> GetChartOfAccounts(GetChartOfAccountsRequest request, ServerCallContext context)
        {
            return base.GetChartOfAccounts(request, context);
        }

        public override Task<GetJournalEntriesResponse> GetJournalEntries(GetJournalEntriesRequest request, ServerCallContext context)
        {
            return base.GetJournalEntries(request, context);
        }

        public override Task<GetLedgerResponse> GetLedger(GetLedgerRequest request, ServerCallContext context)
        {
            return base.GetLedger(request, context);
        }
        public override Task<GetTransactionsResponse> GetTransactions(GetTransactionsRequest request, ServerCallContext context)
        {
            return base.GetTransactions(request, context);
        }

        public override Task<PostTransactionResponse> PostTransaction(PostTransactionRequest request, ServerCallContext context)
        {
            return base.PostTransaction(request, context);
        }
    }
}
