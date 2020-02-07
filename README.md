# NServiceBus.Gateway.Sql

The official [NServiceBus Gateway](https://github.com/Particular/NServiceBus.Gateway) persistence implementation for Microsoft SQL Server, supporting both the [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient) and [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient) packages.

Learn more through our [documentation](https://docs.particular.net/nservicebus/gateway/sql/).

## How to test locally

Running the tests requires Microsoft SQL Server, which can be hosted from a Docker container. The connection string is retrieved from the `SQLServerConnectionString` environment variable. An arbitrary database name can be used for the tests, as the test will first rewrite the connection string to use the `master` database and create the database name specified in the connection string if it does not already exist.
