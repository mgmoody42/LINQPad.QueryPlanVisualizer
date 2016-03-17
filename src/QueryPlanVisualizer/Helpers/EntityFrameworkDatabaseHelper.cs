using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Threading.Tasks;

namespace ExecutionPlanVisualizer.Helpers
{
    internal class EntityFrameworkDatabaseHelper : DatabaseHelper
    {
        public override string GetSqlServerQueryExecutionPlan<T>(IQueryable<T> queryable)
        {
            var interceptor = new CommandCapturingInterceptor();
            try
            {

                DbInterception.Add(interceptor);

                try
                {
                    var result = queryable.Provider.Execute(queryable.Expression);
                }
                catch (Exception ex)
                {
                    if (ex is CommandCapturedException || ex.InnerException is CommandCapturedException)
                    {
                        // ignore
                    }
                    else
                    {
                        throw;
                    }
                }

                if (interceptor.Command == null)
                {
                    throw new InvalidOperationException("DbInterception failed to capture DbCommand.");
                }

                Connection = interceptor.Command.Connection;

                Connection.Open();

                try
                {
                    using (var command = Connection.CreateCommand())
                    {
                        command.CommandText = "SET STATISTICS XML ON";
                        command.ExecuteNonQuery();
                    }

                    using (var command = Connection.CreateCommand())
                    {
                        command.CommandText = interceptor.Command.CommandText;
                        foreach (DbParameter parameter in interceptor.Command.Parameters)
                        {
                            var parameterCopy = command.CreateParameter();
                            parameterCopy.ParameterName = parameter.ParameterName;
                            parameterCopy.DbType = parameter.DbType;
                            parameterCopy.Value = parameter.Value;

                            command.Parameters.Add(parameterCopy);
                        }

                        using (var reader = command.ExecuteReader())
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

                }
                finally
                {
                    Connection.Close();
                }
            }
            finally
            {
                DbInterception.Remove(interceptor);
            }

            return null;
        }
        private class CommandCapturedException : Exception { }

        private sealed class CommandCapturingInterceptor : IDbCommandInterceptor
        {
            public DbCommand Command { get; set; }

            public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
            {
            }

            public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
            {
            }

            public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
            {
                Command = command;
                throw new CommandCapturedException();
            }

            public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
            {
            }

            public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
            {
            }

            public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
            {
            }
        }

        public Task CreateIndexAsync(DbConnection connection, string script)
        {

            return null;
        }
    }
}