using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ExecutionPlanVisualizer
{
    public partial class QueryPlanUserControl : UserControl
    {
        public QueryPlanUserControl()
        {
            InitializeComponent();

            var assocQueryString = NativeMethods.AssocQueryString(NativeMethods.AssocStr.Executable, ".sqlplan");

            if (string.IsNullOrEmpty(assocQueryString))
            {
                openPlanButton.Visible = false;
            }
            else
            {
                var fileDescription = FileVersionInfo.GetVersionInfo(assocQueryString).FileDescription;
                openPlanButton.Text = $"Open with {fileDescription}";
            }
        }

        public string PlanHtml { get; set; }

        public string PlanXml { get; set; }

        public List<MissingIndexDetails> Indexes { get; set; }

        private void SavePlanButtonClick(object sender, EventArgs e)
        {
            if (savePlanFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(savePlanFileDialog.FileName, PlanXml);

                planLocationLinkLabel.Text = savePlanFileDialog.FileName;
                planSavedLabel.Visible = planLocationLinkLabel.Visible = true;
            }
        }

        private void OpenPlanButtonClick(object sender, EventArgs e)
        {
            var tempFile = Path.ChangeExtension(Path.GetTempFileName(), "sqlplan");
            File.WriteAllText(tempFile, PlanXml);

            try
            {
                Process.Start(tempFile);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Cannot open execution plan. {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlanLocationLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", planLocationLinkLabel.Text));
        }

        private void QueryPlanUserControlVisibleChanged(object sender, EventArgs e)
        {
            webBrowser.DocumentText = PlanHtml;

            if (Indexes.Any())
            {
                missingIndexesButton.Visible = true;
                missingIndexesButton.Text = string.Format(missingIndexesButton.Text, Indexes.Count, Indexes.Count > 1 ? "es" : "");
            }
        }
    }
}
