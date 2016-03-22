using System.Data.Common;
using System.Data.Linq;
using System.Linq;

namespace ExecutionPlanVisualizer.Helpers
{
    internal class LinqToSqlDatabaseHelper : DatabaseHelper
    {
        private readonly DataContext dataContext;

        public LinqToSqlDatabaseHelper(DataContext dataContext)
        {
            this.dataContext = dataContext;
            Connection = dataContext.Connection;
        }

        protected override DbCommand CreateCommand(IQueryable queryable)
        {
            return dataContext.GetCommand(queryable);
        }
    }
}