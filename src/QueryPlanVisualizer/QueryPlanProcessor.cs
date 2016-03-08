using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;
using ExecutionPlanVisualizer.Properties;

namespace ExecutionPlanVisualizer
{
    class QueryPlanProcessor
    {
        private readonly string planXml;
        private static readonly XNamespace PlanXmlNamespace = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";

        public QueryPlanProcessor(string planXml)
        {
            this.planXml = planXml;
        }

        public List<MissingIndexDetails> GetMissingIndexes()
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

        public string ConvertPlanToHtml()
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
    }
}