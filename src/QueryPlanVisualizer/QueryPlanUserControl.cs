using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using LINQPad;

namespace ExecutionPlanVisualizer
{
    public partial class QueryPlanUserControl : UserControl
    {
        private string planXml;
        private List<MissingIndexDetails> indexes;

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

        private void SavePlanButtonClick(object sender, EventArgs e)
        {
            if (savePlanFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(savePlanFileDialog.FileName, planXml);

                planLocationLinkLabel.Text = savePlanFileDialog.FileName;
                planSavedLabel.Visible = planLocationLinkLabel.Visible = true;
            }
        }

        private void OpenPlanButtonClick(object sender, EventArgs e)
        {
            var tempFile = Path.ChangeExtension(Path.GetTempFileName(), "sqlplan");
            File.WriteAllText(tempFile, planXml);

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

        private void IndexesDataGridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //http://stackoverflow.com/a/10049887/239438
            for (int i = 0; i < indexesDataGridView.Columns.Count - 1; i++)
            {
                indexesDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            indexesDataGridView.Columns[indexesDataGridView.Columns.Count - 2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            for (int i = 0; i < indexesDataGridView.Columns.Count; i++)
            {
                int width = indexesDataGridView.Columns[i].Width;
                indexesDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                indexesDataGridView.Columns[i].Width = width;
            }
        }

        private async void IndexesDataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //http://stackoverflow.com/a/13687844/239438
            if (!(indexesDataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn) || e.RowIndex < 0)
            {
                return;
            }

            if (MessageBox.Show("Do you really want to create this index?", "Confirm", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            var script = indexes[e.RowIndex].Script;

            try
            {
                indexesDataGridView.Enabled = false;
                progressBar.Visible = indexLabel.Visible = true;

                await DatabaseHelper.CreateIndexAsync(Util.CurrentDataContext.Connection, script);

                IndexCreated?.Invoke(sender, e);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Cannot create index. {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            indexesDataGridView.Enabled = true;
            progressBar.Visible = indexLabel.Visible = false;
        }

        public event EventHandler IndexCreated;

        public void DisplayExecutionPlanDetails(string planXml, string planHtml, List<MissingIndexDetails> indexes)
        {
            this.planXml = planXml;
            this.indexes = indexes;

            webBrowser.DocumentText = planHtml;

            if (this.indexes.Count > 0 && tabControl.TabPages.Count == 1)
            {
                tabControl.TabPages.Add(indexesTabPage);
            }

            if (this.indexes.Count == 0 && tabControl.TabPages.Count > 1)
            {
                tabControl.TabPages.Remove(indexesTabPage);
            }

            indexesTabPage.Text = $"{this.indexes.Count} Missing Index{(this.indexes.Count > 1 ? "es" : "")}";

            indexesDataGridView.DataSource = this.indexes;
            indexesDataGridView.ResetBindings();
        }
    }
}
