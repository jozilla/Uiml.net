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
            this.layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layout.Controls.Add(this.param, 0, 0);
            this.layout.Location = new System.Drawing.Point(3, 3);
            this.layout.Name = "layout";
            this.layout.RowCount = 1;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layout.Size = new System.Drawing.Size(44, 15);
            this.layout.TabIndex = 1;
            // 
            // param
            // 
            this.param.AutoSize = true;
            this.param.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.param.Dock = System.Windows.Forms.DockStyle.Fill;
            this.param.Location = new System.Drawing.Point(3, 0);
            this.param.Name = "param";
            this.param.Size = new System.Drawing.Size(38, 15);
            this.param.TabIndex = 0;
            this.param.Text = "param";
            this.param.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MethodParameterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.layout);
            this.Name = "MethodParameterView";
            this.Size = new System.Drawing.Size(50, 21);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label param;

    }
}
