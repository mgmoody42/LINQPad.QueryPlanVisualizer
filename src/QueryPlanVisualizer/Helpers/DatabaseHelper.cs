using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using LINQPad;

namespace ExecutionPlanVisualizer.Helpers
{
    internal abstract class DatabaseHelper
    {
        private DbConnection _dbConnection;

        public static DatabaseHelper Create(DataContextBase dataContextBase)
        {
            if (dataContextBase == null)
            {
                return new EntityFrameworkDatabaseHelper();
            } 

            return new LinqToSqlDatabaseHelper(dataContextBase.Connection);
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

        public abstract string GetSqlServerQueryExecutionPlan<T>(IQueryable<T> queryable);

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
    }
}