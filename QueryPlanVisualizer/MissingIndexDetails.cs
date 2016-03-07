using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutionPlanVisualizer
{
    public class MissingIndexDetails
    {
        private const string CreateIndexTemplate = "CREATE NONCLUSTERED INDEX [IX_{0}_{1:yyyyMMdd_HHmmss_fff}] ON {2}.{3}.{4} ({5})";
        private readonly Lazy<string> scriptGenerator;

        public MissingIndexDetails()
        {
            scriptGenerator = new Lazy<string>(CreateScript);
        }

        public double Impact { get; set; }

        public string Database { get; set; }
        public string Schema { get; set; }
        public string Table { get; set; }

        public List<string> EqualityColumns { get; set; }
        public List<string> InequalityColumns { get; set; }

        public List<string> IncludeColumns { get; set; }

        public string Script => scriptGenerator.Value;

        private string CreateScript()
        {
            var indexColumns = string.Join(",", EqualityColumns.Concat(InequalityColumns));

            var script = string.Format(CreateIndexTemplate, Table.Trim('[', ']'), DateTime.UtcNow, Database, Schema, Table,
                indexColumns);

            if (IncludeColumns?.Count > 0)
            {
                var includeColumns = string.Join(",", IncludeColumns);
                script += $"{Environment.NewLine}INCLUDE ({includeColumns})";
            }

            return script;
        }
    }
}