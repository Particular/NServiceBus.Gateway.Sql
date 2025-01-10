# NServiceBus.Gateway.Sql

NServiceBus.Gateway.Sql is the official [NServiceBus Gateway](https://github.com/Particular/NServiceBus.Gateway) persistence implementation for [Microsoft SQL Server](http://www.microsoft.com/sqlserver).

It is part of the [Particular Service Platform](https://particular.net/service-platform), which includes [NServiceBus](https://particular.net/nservicebus) and tools to build, monitor, and debug distributed systems.

See the [SQL Gateway Storage documentation](https://docs.particular.net/nservicebus/gateway/sql) for more details on how to use it.

## Running tests locally

Running the tests requires Microsoft SQL Server, which can be hosted from a Docker container. The connection string is retrieved from the `SQLServerConnectionString` environment variable. An arbitrary database name can be used for the tests, as the test will first rewrite the connection string to use the `master` database and create the database name specified in the connection string if it does not already exist.
