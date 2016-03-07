namespace ExecutionPlanVisualizer
{
    partial class QueryPlanUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.savePlanButton = new System.Windows.Forms.Button();
            this.savePlanFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openPlanButton = new System.Windows.Forms.Button();
            this.planSavedLabel = new System.Windows.Forms.Label();
            this.planLocationLinkLabel = new System.Windows.Forms.LinkLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.planTabPage = new System.Windows.Forms.TabPage();
            this.indexesTabPage = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.planTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(3, 3);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(849, 449);
            this.webBrowser.TabIndex = 0;
            // 
            // savePlanButton
            // 
            this.savePlanButton.Location = new System.Drawing.Point(245, 8);
            this.savePlanButton.Name = "savePlanButton";
            this.savePlanButton.Size = new System.Drawing.Size(114, 23);
            this.savePlanButton.TabIndex = 1;
            this.savePlanButton.Text = "Save Plan XML";
            this.savePlanButton.UseVisualStyleBackColor = true;
            this.savePlanButton.Click += new System.EventHandler(this.SavePlanButtonClick);
            // 
            // savePlanFileDialog
            // 
            this.savePlanFileDialog.Filter = "Execution Plan Files|*.sqlplan";
            this.savePlanFileDialog.RestoreDirectory = true;
            // 
            // openPlanButton
            // 
            this.openPlanButton.AutoSize = true;
            this.openPlanButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.openPlanButton.Location = new System.Drawing.Point(7, 8);
            this.openPlanButton.Name = "openPlanButton";
            this.openPlanButton.Size = new System.Drawing.Size(215, 23);
            this.openPlanButton.TabIndex = 2;
            this.openPlanButton.Text = "Open with Sql Server Management Studio";
            this.openPlanButton.UseVisualStyleBackColor = true;
            this.openPlanButton.Click += new System.EventHandler(this.OpenPlanButtonClick);
            // 
            // planSavedLabel
            // 
            this.planSavedLabel.AutoSize = true;
            this.planSavedLabel.Location = new System.Drawing.Point(365, 13);
            this.planSavedLabel.Name = "planSavedLabel";
            this.planSavedLabel.Size = new System.Drawing.Size(80, 13);
            this.planSavedLabel.TabIndex = 3;
            this.planSavedLabel.Text = "Plan Saved to: ";
            this.planSavedLabel.Visible = false;
            // 
            // planLocationLinkLabel
            // 
            this.planLocationLinkLabel.AutoSize = true;
            this.planLocationLinkLabel.Location = new System.Drawing.Point(438, 13);
            this.planLocationLinkLabel.Name = "planLocationLinkLabel";
            this.planLocationLinkLabel.Size = new System.Drawing.Size(117, 13);
            this.planLocationLinkLabel.TabIndex = 4;
            this.planLocationLinkLabel.TabStop = true;
            this.planLocationLinkLabel.Text = "plan location goes here";
            this.planLocationLinkLabel.Visible = false;
            this.planLocationLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PlanLocationLinkLabelLinkClicked);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.planTabPage);
            this.tabControl.Controls.Add(this.indexesTabPage);
            this.tabControl.Location = new System.Drawing.Point(0, 45);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(863, 481);
            this.tabControl.TabIndex = 6;
            // 
            // planTabPage
            // 
            this.planTabPage.Controls.Add(this.webBrowser);
            this.planTabPage.Location = new System.Drawing.Point(4, 22);
            this.planTabPage.Name = "planTabPage";
            this.planTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.planTabPage.Size = new System.Drawing.Size(855, 455);
            this.planTabPage.TabIndex = 0;
            this.planTabPage.Text = "Query Execution Plan";
            this.planTabPage.UseVisualStyleBackColor = true;
            // 
            // indexesTabPage
            // 
            this.indexesTabPage.Location = new System.Drawing.Point(4, 22);
            this.indexesTabPage.Name = "indexesTabPage";
            this.indexesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.indexesTabPage.Size = new System.Drawing.Size(855, 455);
            this.indexesTabPage.TabIndex = 1;
            this.indexesTabPage.Text = "Missing Indexes";
            this.indexesTabPage.UseVisualStyleBackColor = true;
            // 
            // QueryPlanUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.planLocationLinkLabel);
            this.Controls.Add(this.planSavedLabel);
            this.Controls.Add(this.openPlanButton);
            this.Controls.Add(this.savePlanButton);
            this.Name = "QueryPlanUserControl";
            this.Size = new System.Drawing.Size(863, 529);
            this.VisibleChanged += new System.EventHandler(this.QueryPlanUserControlVisibleChanged);
            this.tabControl.ResumeLayout(false);
            this.planTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button savePlanButton;
        private System.Windows.Forms.SaveFileDialog savePlanFileDialog;
        private System.Windows.Forms.Button openPlanButton;
        private System.Windows.Forms.Label planSavedLabel;
        private System.Windows.Forms.LinkLabel planLocationLinkLabel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage planTabPage;
        private System.Windows.Forms.TabPage indexesTabPage;
    }
}
