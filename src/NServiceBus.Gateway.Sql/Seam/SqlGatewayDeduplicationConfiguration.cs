using NServiceBus.ObjectBuilder;
using NServiceBus.Settings;
using System;

namespace NServiceBus.Gateway.Sql
{
    class SqlGatewayDeduplicationConfiguration : GatewayDeduplicationConfiguration
    {
        public override void Setup(ReadOnlySettings settings)
        {
            base.Setup(settings);
        }

        public override IGatewayDeduplicationStorage CreateStorage(IBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
