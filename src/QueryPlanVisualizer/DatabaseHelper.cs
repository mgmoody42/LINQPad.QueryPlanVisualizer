using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using LINQPad;

namespace ExecutionPlanVisualizer
{
    internal static class DatabaseHelper
    {
        internal static string GetSqlServerQueryExecutionPlan<T>(DbConnection connection, IQueryable<T> queryable)
        {
            try
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SET STATISTICS XML ON";
                    command.ExecuteNonQuery();
                }

                using (var reader = Util.CurrentDataContext.GetCommand(queryable).ExecuteReader())
                {
                    while (reader.NextResult())
                    {
                        if (reader.GetName(0) == "Microsoft SQL Server 2005 XML Showplan")
                        {
                            reader.Read();
                            return reader.GetString(0);
                        }
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return null;
        }

        internal static async Task CreateIndexAsync(DbConnection connection, string script)
        {
            try
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = script;
                    var result = await command.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}