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
            this.hScrollBar3 = new System.Windows.Forms.HScrollBar();
            this.m_lblNewDesign = new System.Windows.Forms.Label();
            this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.m_lblWireFrame = new System.Windows.Forms.Label();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.m_lblSelected = new System.Windows.Forms.Label();
            this.m_btnEnableDisable = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hScrollBar3
            // 
            this.hScrollBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar3.Location = new System.Drawing.Point(6, 137);
            this.hScrollBar3.Name = "hScrollBar3";
            this.hScrollBar3.Size = new System.Drawing.Size(256, 20);
            this.hScrollBar3.TabIndex = 20;
            // 
            // m_lblNewDesign
            // 
            this.m_lblNewDesign.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblNewDesign.AutoSize = true;
            this.m_lblNewDesign.Location = new System.Drawing.Point(3, 114);
            this.m_lblNewDesign.Name = "m_lblNewDesign";
            this.m_lblNewDesign.Size = new System.Drawing.Size(113, 13);
            this.m_lblNewDesign.TabIndex = 23;
            this.m_lblNewDesign.Text = "Opacity user interface:";
            // 
            // hScrollBar2
            // 
            this.hScrollBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar2.Location = new System.Drawing.Point(6, 84);
            this.hScrollBar2.Name = "hScrollBar2";
            this.hScrollBar2.Size = new System.Drawing.Size(256, 20);
            this.hScrollBar2.TabIndex = 19;
            // 
            // m_lblWireFrame
            // 
            this.m_lblWireFrame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblWireFrame.AutoSize = true;
            this.m_lblWireFrame.Location = new System.Drawing.Point(3, 9);
            this.m_lblWireFrame.Name = "m_lblWireFrame";
            this.m_lblWireFrame.Size = new System.Drawing.Size(102, 13);
            this.m_lblWireFrame.TabIndex = 21;
            this.m_lblWireFrame.Text = "Opacity wire-frames:";
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar1.Location = new System.Drawing.Point(6, 32);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(256, 19);
            this.hScrollBar1.TabIndex = 18;
            // 
            // m_lblSelected
            // 
            this.m_lblSelected.AutoSize = true;
            this.m_lblSelected.Location = new System.Drawing.Point(3, 61);
            this.m_lblSelected.Name = "m_lblSelected";
            this.m_lblSelected.Size = new System.Drawing.Size(140, 13);
            this.m_lblSelected.TabIndex = 22;
            this.m_lblSelected.Text = "Opacity selected wire-frame:";
            // 
            // m_btnEnableDisable
            // 
            this.m_btnEnableDisable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnEnableDisable.Location = new System.Drawing.Point(133, 165);
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
            this.Controls.Add(this.hScrollBar3);
            this.Controls.Add(this.m_lblNewDesign);
            this.Controls.Add(this.hScrollBar2);
            this.Controls.Add(this.m_lblWireFrame);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.m_lblSelected);
            this.Name = "WireFrameController";
            this.Size = new System.Drawing.Size(276, 188);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.HScrollBar hScrollBar3;
        private System.Windows.Forms.Label m_lblNewDesign;
        private System.Windows.Forms.HScrollBar hScrollBar2;
        private System.Windows.Forms.Label m_lblWireFrame;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Label m_lblSelected;
        private System.Windows.Forms.Button m_btnEnableDisable;



    }
}
