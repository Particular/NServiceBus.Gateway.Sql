﻿using NUnit.Framework;

namespace NServiceBus.Gateway.AcceptanceTests
{
    [SetUpFixture]
    public class SetupFixture
    {
        [OneTimeSetUp]
        public void Setup()
        {
#if NET461
            // Weird bug about deserialization of objects across AppDomains
            // Otherwise it wants test classes to be marked as serializable in net461
            // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/mitigation-deserialization-of-objects-across-app-domains
            System.Configuration.ConfigurationManager.GetSection("dummy");
#endif
        }
    }
}
