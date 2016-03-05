namespace Visualizer
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.savePlanButton = new System.Windows.Forms.Button();
            this.savePlanFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openPlanButton = new System.Windows.Forms.Button();
            this.planSavedLabel = new System.Windows.Forms.Label();
            this.planLocationLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(0, 89);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(863, 437);
            this.webBrowser1.TabIndex = 0;
            // 
            // savePlanButton
            // 
            this.savePlanButton.Location = new System.Drawing.Point(3, 15);
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
            this.openPlanButton.Location = new System.Drawing.Point(157, 15);
            this.openPlanButton.Name = "openPlanButton";
            this.openPlanButton.Size = new System.Drawing.Size(67, 23);
            this.openPlanButton.TabIndex = 2;
            this.openPlanButton.Text = "Open Plan";
            this.openPlanButton.UseVisualStyleBackColor = true;
            this.openPlanButton.Click += new System.EventHandler(this.OpenPlanButtonClick);
            // 
            // planSavedLabel
            // 
            this.planSavedLabel.AutoSize = true;
            this.planSavedLabel.Location = new System.Drawing.Point(3, 55);
            this.planSavedLabel.Name = "planSavedLabel";
            this.planSavedLabel.Size = new System.Drawing.Size(80, 13);
            this.planSavedLabel.TabIndex = 3;
            this.planSavedLabel.Text = "Plan Saved to: ";
            this.planSavedLabel.Visible = false;
            // 
            // planLocationLinkLabel
            // 
            this.planLocationLinkLabel.AutoSize = true;
            this.planLocationLinkLabel.Location = new System.Drawing.Point(76, 55);
            this.planLocationLinkLabel.Name = "planLocationLinkLabel";
            this.planLocationLinkLabel.Size = new System.Drawing.Size(117, 13);
            this.planLocationLinkLabel.TabIndex = 4;
            this.planLocationLinkLabel.TabStop = true;
            this.planLocationLinkLabel.Text = "plan location goes here";
            this.planLocationLinkLabel.Visible = false;
            this.planLocationLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PlanLocationLinkLabelLinkClicked);
            // 
            // QueryPlanUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.planLocationLinkLabel);
            this.Controls.Add(this.planSavedLabel);
            this.Controls.Add(this.openPlanButton);
            this.Controls.Add(this.savePlanButton);
            this.Controls.Add(this.webBrowser1);
            this.Name = "QueryPlanUserControl";
            this.Size = new System.Drawing.Size(863, 529);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button savePlanButton;
        private System.Windows.Forms.SaveFileDialog savePlanFileDialog;
        private System.Windows.Forms.Button openPlanButton;
        private System.Windows.Forms.Label planSavedLabel;
        private System.Windows.Forms.LinkLabel planLocationLinkLabel;
    }
}
