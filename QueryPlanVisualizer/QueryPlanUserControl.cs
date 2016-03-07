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

        public List<MissingIndexDetails> Indexes { get; set; } = new List<MissingIndexDetails>();

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
            Process.Start("explorer.exe", $"/select,\"{planLocationLinkLabel.Text}\"");
        }

        private void QueryPlanUserControlVisibleChanged(object sender, EventArgs e)
        {
            webBrowser.DocumentText = PlanHtml;

            if (Indexes.Count > 0 && tabControl.TabPages.Count == 1)
            {
                tabControl.TabPages.Add(indexesTabPage);
            }

            if (Indexes.Count == 0 && tabControl.TabPages.Count > 1)
            {
                tabControl.TabPages.Remove(indexesTabPage);
            }

            indexesTabPage.Text = $"{Indexes.Count} Missing Index{(Indexes.Count > 1 ? "es" : "")}";

            indexesDataGridView.DataSource = Indexes;
            indexesDataGridView.ResetBindings();
        }

        private void IndexesDataGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //http://stackoverflow.com/a/10049887/239438
            for (int i = 0; i < indexesDataGridView.Columns.Count - 1; i++)
            {
                indexesDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            indexesDataGridView.Columns[indexesDataGridView.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            indexesDataGridView.Columns[indexesDataGridView.Columns.Count - 2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            for (int i = 0; i < indexesDataGridView.Columns.Count; i++)
            {
                int width = indexesDataGridView.Columns[i].Width;
                indexesDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                indexesDataGridView.Columns[i].Width = width;
            }
        }
    }
}
