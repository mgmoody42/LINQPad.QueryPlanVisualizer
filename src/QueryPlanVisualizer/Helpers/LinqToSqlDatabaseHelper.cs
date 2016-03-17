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

        public override string GetSqlServerQueryExecutionPlan<T>(IQueryable<T> queryable)
        {
            try
            {
                Connection.Open();

                using (var command = Connection.CreateCommand())
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
                Connection.Close();
            }

            return null;
        }



    }
}