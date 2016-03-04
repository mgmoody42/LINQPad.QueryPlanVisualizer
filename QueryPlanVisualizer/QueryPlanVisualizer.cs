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
                            schema.Add("http://schemas.microsoft.com/sqlserver/2004/07/showplan",
                                @"showplanxml.xsd");

                            var settings = new XmlReaderSettings()
                            {
                                ValidationType = ValidationType.Schema,
                                Schemas = schema,
                            };

                            using (var xmlReader = XmlReader.Create(new StringReader(reader.GetString(0)), settings))
                            {
                                var transform = new XslCompiledTransform(true);
                                transform.Load(@"qp.xslt");

                                var returnValue = new StringBuilder();
                                using (var writer = XmlWriter.Create(returnValue, transform.OutputSettings))
                                {
                                    transform.Transform(xmlReader, writer);
                                }

                                var template = File.ReadAllText(@"template.html");
                                var html = string.Format(template, returnValue);
                                var webBrowser = new WebBrowser {DocumentText = html};
                                

                                PanelManager.DisplayControl(webBrowser, "Query plan");
                            }

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