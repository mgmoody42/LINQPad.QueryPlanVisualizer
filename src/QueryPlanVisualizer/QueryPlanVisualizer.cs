using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ExecutionPlanVisualizer.Helpers;
using ExecutionPlanVisualizer.Properties;
using LINQPad;

namespace ExecutionPlanVisualizer
{
    public static class QueryPlanVisualizer
    {
        private const string ExecutionPlanPanelTitle = "Query Execution Plan";
        private static bool shouldExtract = true;

        public static IQueryable<T> DumpPlan<T>(this IQueryable<T> queryable, bool dumpData = false)
        {
            var databaseHelper = DatabaseHelper.Create(Util.CurrentDataContext);
            
            var sqlConnection = Util.CurrentDataContext?.Connection as SqlConnection;

            if (databaseHelper is LinqToSqlDatabaseHelper && sqlConnection == null)
            {
                var control = new Label {Text = "Query Plan Visualizer supports only Sql Server"};
                PanelManager.DisplayControl(control, ExecutionPlanPanelTitle);
                return queryable;
            }

            if (dumpData)
            {
                queryable.Dump();
            }

            try
            {
                var planXml = databaseHelper.GetSqlServerQueryExecutionPlan(queryable);

                var queryPlanProcessor = new QueryPlanProcessor(planXml);

                var indexes = queryPlanProcessor.GetMissingIndexes();
                var planHtml = queryPlanProcessor.ConvertPlanToHtml();


                var files = ExtractFiles();
                files.Add(planHtml);

                var html = string.Format(Resources.template, files.ToArray());
                var queryPlanUserControl = new QueryPlanUserControl
                {
                    PlanXml = planXml,
                    PlanHtml = html,
                    Indexes = indexes,
                    DatabaseHelper = databaseHelper
                };

                queryPlanUserControl.IndexCreated += (sender, args) =>
                {
                    if (MessageBox.Show("Index created. Refresh query plan?", "", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DumpPlan(queryable);
                    }
                };

                var panel = PanelManager.GetOutputPanel(ExecutionPlanPanelTitle);

                panel?.Close();

                PanelManager.DisplayControl(queryPlanUserControl, ExecutionPlanPanelTitle);
            }
            catch (Exception exception)
            {
                var control = new Label {Text = exception.ToString()};
                PanelManager.DisplayControl(control, ExecutionPlanPanelTitle);
            }
            return queryable;
        }

        private static List<string> ExtractFiles()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LINQPadQueryVisualizer");
            var imagesFolder = Path.Combine(folder, "images");

            if (!Directory.Exists(folder))
            {
                shouldExtract = true;
                Directory.CreateDirectory(folder);
            }

            if (!Directory.Exists(imagesFolder))
            {
                shouldExtract = true;
                Directory.CreateDirectory(imagesFolder);
            }

            var qpJavascript = Path.Combine(folder, "qp.js");
            var qpStyleSheet = Path.Combine(folder, "qp.css");
            var jquery = Path.Combine(folder, "jquery.js");

            if (shouldExtract)
            {
                File.WriteAllText(qpJavascript, Resources.jquery);
                File.WriteAllText(qpStyleSheet, Resources.qpStyleSheet);
                File.WriteAllText(jquery, Resources.qpJavascript);

                var assembly = Assembly.GetExecutingAssembly();
                var resourceNames = assembly.GetManifestResourceNames();

                foreach (var name in resourceNames.Where(name => name.EndsWith(".gif")))
                {
                    using (var stream = assembly.GetManifestResourceStream(name))
                    {
                        using (var file = new FileStream(Path.Combine(imagesFolder, Path.GetFileNameWithoutExtension(name).Split('.').Last() + ".gif"), FileMode.Create, FileAccess.Write))
                        {
                            stream.CopyTo(file);
                        }
                    }
                }

                shouldExtract = false;
            }

            return new List<string> { qpStyleSheet, qpJavascript, jquery };
        }
    }
}