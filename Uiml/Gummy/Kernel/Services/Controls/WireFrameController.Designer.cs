namespace Uiml.Gummy.Kernel.Services.Controls
{
    partial class WireFrameController
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
            this.m_btnEnableDisable = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_btnEnableDisable
            // 
            this.m_btnEnableDisable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnEnableDisable.Location = new System.Drawing.Point(144, 3);
            this.m_btnEnableDisable.Name = "m_btnEnableDisable";
            this.m_btnEnableDisable.Size = new System.Drawing.Size(129, 23);
            this.m_btnEnableDisable.TabIndex = 24;
            this.m_btnEnableDisable.Text = "disable wire-frames";
            this.m_btnEnableDisable.UseVisualStyleBackColor = true;
            this.m_btnEnableDisable.Click += new System.EventHandler(this.btnEnableDisable_Click);
            // 
            // WireFrameController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_btnEnableDisable);
            this.Name = "WireFrameController";
            this.Size = new System.Drawing.Size(276, 34);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnEnableDisable;



    }
}
