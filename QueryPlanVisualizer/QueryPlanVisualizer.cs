using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;

using LINQPad;

using Visualizer.Properties;

namespace Visualizer
{
    public static class QueryPlanVisualizer
    {
        public static void DumpPlan<T>(this IQueryable<T> queryable)
        {
            var sqlConnection = Util.CurrentDataContext.Connection as SqlConnection;

            if (sqlConnection == null)
            {
                var control = new Label { Text = "Query Plan Visualizer supports only Sql Server" };
                PanelManager.DisplayControl(control);
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

                            var schema = new XmlSchemaSet();
                            using (var planSchemaReader = XmlReader.Create(new StringReader(Resources.showplanxml)))
                            {
                                schema.Add("http://schemas.microsoft.com/sqlserver/2004/07/showplan", planSchemaReader);
                            }

                            var settings = new XmlReaderSettings
                            {
                                ValidationType = ValidationType.Schema,
                                Schemas = schema,
                            };


                            var transform = new XslCompiledTransform(true);

                            using (var xsltReader = XmlReader.Create(new StringReader(Resources.qpXslt)))
                            {
                                transform.Load(xsltReader);
                            }

                            var planHtml = new StringBuilder();

                            using (var queryPlanReader = XmlReader.Create(new StringReader(reader.GetString(0)), settings))
                            {
                                using (var writer = XmlWriter.Create(planHtml, transform.OutputSettings))
                                {
                                    transform.Transform(queryPlanReader, writer);
                                }
                            }

                            var html = string.Format(Resources.template, planHtml);
                            var webBrowser = new WebBrowser { DocumentText = html };

                            PanelManager.DisplayControl(webBrowser, "Query plan");
                            
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var control = new Label { Text = exception.ToString() };
                PanelManager.DisplayControl(control);
            }
            finally
            {
                Util.CurrentDataContext.Connection.Close();
            }
        }
    }
}