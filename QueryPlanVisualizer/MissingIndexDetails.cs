using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutionPlanVisualizer
{
    public class MissingIndexDetails
    {
        private const string CreateIndexTemplate = "CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>] ON {0}.{1}.{2} ({3})";

        public double Impact { get; set; }

        public string Database { get; set; }
        public string Schema { get; set; }
        public string Table { get; set; }

        public List<string> EqualityColumns { get; set; }
        public List<string> InequalityColumns { get; set; }

        public List<string> IncludeColumns { get; set; }

        public string Script
        {
            get
            {
                var indexColumns = string.Join(",", EqualityColumns.Concat(InequalityColumns));

                var script = string.Format(CreateIndexTemplate, Database, Schema, Table, indexColumns);

                if (IncludeColumns?.Count > 0)
                {
                    var includeColumns = string.Join(",", IncludeColumns);
                    script += $"{Environment.NewLine}INCLUDE ({includeColumns})";
                }

                return script;
            }
        }
    }
}