using System;
using System.Data.Common;
using System.Linq;
using LINQPad;

namespace ExecutionPlanVisualizer.Helpers
{
    internal class LinqToSqlDatabaseHelper : DatabaseHelper
    {
        public LinqToSqlDatabaseHelper(DbConnection connection)
        {
            Connection = connection;
        }

        protected override DbCommand CreateCommand(IQueryable queryable)
        {
            return Util.CurrentDataContext.GetCommand(queryable);
        }
    }
}