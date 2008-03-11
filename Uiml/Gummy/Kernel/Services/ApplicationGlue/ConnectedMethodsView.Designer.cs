namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    partial class ConnectedMethodsView
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
            this.connectionIcon = new System.Windows.Forms.PictureBox();
            this.layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.layout.ColumnCount = 4;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.Controls.Add(this.connectionIcon, 3, 1);
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.Size = new System.Drawing.Size(43, 41);
            this.layout.TabIndex = 0;
            // 
            // connectionIcon
            // 
            this.connectionIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionIcon.Image = global::gummy.Properties.Resources.connection_not_ok;
            this.connectionIcon.InitialImage = null;
            this.connectionIcon.Location = new System.Drawing.Point(7, 5);
            this.connectionIcon.Name = "connectionIcon";
            this.connectionIcon.Size = new System.Drawing.Size(32, 32);
            this.connectionIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.connectionIcon.TabIndex = 0;
            this.connectionIcon.TabStop = false;
            // 
            // ConnectedMethodsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.layout);
            this.Name = "ConnectedMethodsView";
            this.Size = new System.Drawing.Size(46, 44);
            this.layout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.connectionIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.PictureBox connectionIcon;
    }
}
