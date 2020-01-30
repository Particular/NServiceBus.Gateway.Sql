using NServiceBus.Extensibility;
using NServiceBus.ObjectBuilder;
using System;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlGatewayDeduplicationStorage : IGatewayDeduplicationStorage
    {
        readonly SqlSettings settings;
        readonly IBuilder builder;

        public SqlGatewayDeduplicationStorage(IBuilder builder, SqlSettings settings)
        {
            this.builder = builder;
            this.settings = settings;
        }

        public bool SupportsDistributedTransactions => true;

        public Task<IDeduplicationSession> CheckForDuplicate(string messageId, ContextBag context)
        {
            return Task.FromResult<IDeduplicationSession>(
                new SqlDeduplicationSession(builder, messageId, settings));
        }
    }
}
