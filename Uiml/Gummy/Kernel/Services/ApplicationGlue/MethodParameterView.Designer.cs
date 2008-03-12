namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    partial class MethodParameterView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
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
        private void InitializeComponent ()
        {
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.param = new System.Windows.Forms.Label();
            this.linkIcon = new System.Windows.Forms.PictureBox();
            this.layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.linkIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layout.BackColor = System.Drawing.Color.Transparent;
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.Controls.Add(this.param, 0, 0);
            this.layout.Controls.Add(this.linkIcon, 0, 1);
            this.layout.Location = new System.Drawing.Point(3, 3);
            this.layout.Name = "layout";
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.Size = new System.Drawing.Size(47, 34);
            this.layout.TabIndex = 1;
            this.layout.MouseClick += new System.Windows.Forms.MouseEventHandler(this.layout_MouseClick);
            // 
            // param
            // 
            this.param.AutoSize = true;
            this.param.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.param.Dock = System.Windows.Forms.DockStyle.Fill;
            this.param.Location = new System.Drawing.Point(3, 0);
            this.param.Name = "param";
            this.param.Size = new System.Drawing.Size(41, 15);
            this.param.TabIndex = 0;
            this.param.Text = "param";
            this.param.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.param.MouseClick += new System.Windows.Forms.MouseEventHandler(this.param_MouseClick);
            // 
            // linkIcon
            // 
            this.linkIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkIcon.Image = global::gummy.Properties.Resources.not_linked;
            this.linkIcon.Location = new System.Drawing.Point(3, 18);
            this.linkIcon.Name = "linkIcon";
            this.linkIcon.Size = new System.Drawing.Size(41, 13);
            this.linkIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.linkIcon.TabIndex = 1;
            this.linkIcon.TabStop = false;
            this.linkIcon.DoubleClick += new System.EventHandler(this.linkIcon_DoubleClick);
            this.linkIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.linkIcon_MouseClick);
            // 
            // MethodParameterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.layout);
            this.Name = "MethodParameterView";
            this.Size = new System.Drawing.Size(53, 40);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MethodParameterView_MouseClick);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.linkIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label param;
        private System.Windows.Forms.PictureBox linkIcon;

    }
}
