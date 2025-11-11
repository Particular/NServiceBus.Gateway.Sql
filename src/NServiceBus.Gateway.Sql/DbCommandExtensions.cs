namespace NServiceBus.Gateway.Sql;

using System.Data.Common;

static class DbCommandExtensions
{
    public static void AddParameter(this DbCommand cmd, string name, object value)
    {
        var parameter = cmd.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        cmd.Parameters.Add(parameter);
    }
}