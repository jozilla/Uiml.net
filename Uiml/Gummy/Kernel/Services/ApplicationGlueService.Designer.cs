namespace Uiml.Gummy.Kernel.Services {
    partial class ApplicationGlueService {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.layout = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Size = new System.Drawing.Size(284, 264);
            this.layout.TabIndex = 0;
            // 
            // ApplicationGlueService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.layout);
            this.Name = "ApplicationGlueService";
            this.Text = "Application Glue";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel layout;

    }
}