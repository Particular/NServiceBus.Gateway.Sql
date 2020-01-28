using NServiceBus.Extensibility;
using System;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlGatewayDeduplicationStorage : IGatewayDeduplicationStorage
    {
        public bool SupportsDistributedTransactions => true;

        public Task<IDeduplicationSession> CheckForDuplicate(string messageId, ContextBag context)
        {
            throw new NotImplementedException();
        }
    }
}
