using System;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LINQPad;

namespace ExecutionPlanVisualizer.Helpers
{
    internal abstract class DatabaseHelper
    {
        private DbConnection _dbConnection;

        public static DatabaseHelper Create<T>(DataContextBase dataContextBase, IQueryable<T> queryable)
        {
            if (dataContextBase != null)
            {
                return new LinqToSqlDatabaseHelper(dataContextBase);
            }

            var table = queryable as ITable;

            if (table != null)
            {
                return new LinqToSqlDatabaseHelper(table.Context);
            }

            var dataQueryType = typeof(DataContext).Assembly.GetType("System.Data.Linq.DataQuery`1");

            if (queryable.GetType().GetGenericTypeDefinition() == dataQueryType)
            {
                var contextField = queryable.GetType().GetField("context", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                var context = contextField?.GetValue(queryable) as DataContext;

                if (context != null)
                {
                    return new LinqToSqlDatabaseHelper(context);
                }
            }

            return new EntityFrameworkDatabaseHelper();
        }

        public DbConnection Connection
        {
            get
            {
                if (_dbConnection == null)
                {
                    throw new InvalidOperationException("Connection has not been set.");
                }
                return _dbConnection;
            }
            set { _dbConnection = value; }
        }

        public virtual string GetSqlServerQueryExecutionPlan<T>(IQueryable<T> queryable)
        {
            using (var command = CreateCommand(queryable))
            {
                try
                {
                    Connection.Open();

                    using (var setStatisticsCommand = Connection.CreateCommand())
                    {
                        setStatisticsCommand.CommandText = "SET STATISTICS XML ON";
                        setStatisticsCommand.ExecuteNonQuery();
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

                    return null;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public virtual async Task CreateIndexAsync(string script)
        {
            try
            {
                await Connection.OpenAsync();

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = script;
                    var result = await command.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                Connection.Close();
            }
        }

        protected abstract DbCommand CreateCommand(IQueryable queryable);
    }
}