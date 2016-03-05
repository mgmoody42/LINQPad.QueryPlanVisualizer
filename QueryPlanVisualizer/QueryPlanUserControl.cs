using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visualizer
{
    public partial class QueryPlanUserControl : UserControl
    {
        public QueryPlanUserControl()
        {
            InitializeComponent();

            var assocQueryString = NativeMethods.AssocQueryString(NativeMethods.AssocStr.Executable, ".sqlplan");

            var fileDescription = FileVersionInfo.GetVersionInfo(assocQueryString).FileDescription;
            openPlanButton.Text = $"Open with {fileDescription}";
        }

        public string PlanHtml
        {
            set { webBrowser1.DocumentText = value; }
        }

        public string PlanXml { get; set; }

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
    }
}
