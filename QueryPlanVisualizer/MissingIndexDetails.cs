using System.Collections.Generic;

namespace ExecutionPlanVisualizer
{
    class MissingIndexDetails
    {
        public double Impact { get; set; }

        public string Database { get; set; }
        public string Schema { get; set; }
        public string Table { get; set; }

        public List<string> EqualityColumns { get; set; }
        public List<string> InequalityColumns { get; set; }

        public List<string> IncludeColumns { get; set; }
    }
}