using NServiceBus.Extensibility;
using System;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlGatewayDeduplicationStorage : IGatewayDeduplicationStorage
    {
        readonly SqlSettings settings;

        public SqlGatewayDeduplicationStorage(SqlSettings settings)
        {
            this.settings = settings;
        }

        public bool SupportsDistributedTransactions => true;

        public Task<IDeduplicationSession> CheckForDuplicate(string messageId, ContextBag context)
        {
            return Task.FromResult<IDeduplicationSession>(
                new SqlDeduplicationSession(messageId, settings));
        }
    }
}
