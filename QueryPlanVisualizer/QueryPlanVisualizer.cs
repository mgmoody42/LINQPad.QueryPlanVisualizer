using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;
using ExecutionPlanVisualizer.Properties;
using LINQPad;
using Visualizer;

namespace ExecutionPlanVisualizer
{
    public static class QueryPlanVisualizer
    {
        private const string ExecutionPlanPanelTitle = "Query Execution Plan";
        private static readonly XNamespace PlanXmlNamespace = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";
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
                            var planHtml = ConvertPlanToHtml(planXml);

                            var files = ExtractFiles();
                            files.Add(planHtml);

                            GetMissingIndexes(planXml);

                            var html = string.Format(Resources.template, files.ToArray());
                            var queryPlanUserControl = new QueryPlanUserControl
                            {
                                PlanXml = planXml,
                                PlanHtml = html
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

        private static string ConvertPlanToHtml(string planXml)
        {
            var schema = new XmlSchemaSet();
            using (var planSchemaReader = XmlReader.Create(new StringReader(Resources.showplanxml)))
            {
                schema.Add(PlanXmlNamespace.NamespaceName, planSchemaReader);
            }

            var transform = new XslCompiledTransform(true);

            using (var xsltReader = XmlReader.Create(new StringReader(Resources.qpXslt)))
            {
                transform.Load(xsltReader);
            }

            var planHtml = new StringBuilder();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schema,
            };

            using (var queryPlanReader = XmlReader.Create(new StringReader(planXml), settings))
            {
                using (var writer = XmlWriter.Create(planHtml, transform.OutputSettings))
                {
                    transform.Transform(queryPlanReader, writer);
                }
            }
            return planHtml.ToString();
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

        private static List<MissingIndexDetails> GetMissingIndexes(string planXml)
        {
            var document = XDocument.Parse(planXml);

            var missingIndexGroups = document.Descendants(PlanXmlNamespace.WithName("MissingIndexGroup"));

            var result = from missingIndexGroup in missingIndexGroups

                         let missingIndexes = missingIndexGroup.Descendants(PlanXmlNamespace.WithName("MissingIndex"))

                         let indexes = from missingIndex in missingIndexes
                                       let columnGroups = missingIndex.Descendants(PlanXmlNamespace.WithName("ColumnGroup"))

                                       let equalityColumns = (from columnGroup in columnGroups
                                                              where columnGroup.AttributeValue("Usage") == "EQUALITY"
                                                              from column in columnGroup.Descendants()
                                                              select column.AttributeValue("Name"))

                                       let inequalityColumns = (from columnGroup in columnGroups
                                                                where columnGroup.AttributeValue("Usage") == "INEQUALITY"
                                                                from column in columnGroup.Descendants()
                                                                select column.AttributeValue("Name"))

                                       let includeColumns = (from columnGroup in columnGroups
                                                             where columnGroup.AttributeValue("Usage") == "INCLUDE"
                                                             from column in columnGroup.Descendants()
                                                             select column.AttributeValue("Name"))

                                       select new MissingIndexDetails
                                       {
                                           Impact = Convert.ToDouble(missingIndexGroup.AttributeValue("Impact")),

                                           Database = missingIndex.AttributeValue("Database"),
                                           Table = missingIndex.AttributeValue("Table"),
                                           Schema = missingIndex.AttributeValue("Schema"),

                                           EqualityColumns = new List<string>(equalityColumns),
                                           InequalityColumns = new List<string>(inequalityColumns),

                                           IncludeColumns = new List<string>(includeColumns)
                                       }

                         from index in indexes
                         select index;

            return result.ToList();
        }
    }
}