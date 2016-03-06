using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ExecutionPlanVisualizer.Properties;
using LINQPad;

namespace ExecutionPlanVisualizer
{
    public static class QueryPlanVisualizer
    {
        private const string ExecutionPlanPanelTitle = "Query Execution Plan";
        private static bool shouldExtract = true;

        public static void DumpPlan<T>(this IQueryable<T> queryable)
        {
            var sqlConnection = Util.CurrentDataContext.Connection as SqlConnection;

            if (sqlConnection == null)
            {
                var control = new Label { Text = "Query Plan Visualizer supports only Sql Server" };
                PanelManager.DisplayControl(control, ExecutionPlanPanelTitle);
                return;
            }

            try
            {
                Util.CurrentDataContext.Connection.Open();

                using (var command = new SqlCommand("SET STATISTICS XML ON", sqlConnection))
                {
                    command.ExecuteNonQuery();
                }

                using (var reader = Util.CurrentDataContext.GetCommand(queryable).ExecuteReader())
                {
                    while (reader.NextResult())
                    {
                        if (reader.GetName(0) == "Microsoft SQL Server 2005 XML Showplan")
                        {
                            reader.Read();

                            var planXml = reader.GetString(0);

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
                                Indexes = indexes
                            };

                            PanelManager.DisplayControl(queryPlanUserControl, ExecutionPlanPanelTitle);

                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var control = new Label { Text = exception.ToString() };
                PanelManager.DisplayControl(control, ExecutionPlanPanelTitle);
            }
            finally
            {
                Util.CurrentDataContext.Connection.Close();
            }
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