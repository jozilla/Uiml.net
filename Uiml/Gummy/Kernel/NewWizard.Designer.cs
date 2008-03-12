namespace Uiml.Gummy.Kernel {
    partial class NewWizard {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewWizard));
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.buttonsTable = new System.Windows.Forms.TableLayoutPanel();
            this.back = new System.Windows.Forms.Button();
            this.next = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.Label();
            this.logoTable = new System.Windows.Forms.TableLayoutPanel();
            this.logo = new System.Windows.Forms.PictureBox();
            this.mainTable.SuspendLayout();
            this.buttonsTable.SuspendLayout();
            this.logoTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTable
            // 
            this.mainTable.AutoSize = true;
            this.mainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTable.ColumnCount = 2;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTable.Controls.Add(this.buttonsTable, 1, 2);
            this.mainTable.Controls.Add(this.title, 1, 0);
            this.mainTable.Controls.Add(this.logoTable, 0, 0);
            this.mainTable.Location = new System.Drawing.Point(0, 3);
            this.mainTable.Name = "mainTable";
            this.mainTable.RowCount = 3;
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTable.Size = new System.Drawing.Size(425, 212);
            this.mainTable.TabIndex = 0;
            // 
            // buttonsTable
            // 
            this.buttonsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonsTable.AutoSize = true;
            this.buttonsTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonsTable.ColumnCount = 3;
            this.buttonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTable.Controls.Add(this.back, 0, 0);
            this.buttonsTable.Controls.Add(this.next, 1, 0);
            this.buttonsTable.Controls.Add(this.cancel, 2, 0);
            this.buttonsTable.Location = new System.Drawing.Point(179, 180);
            this.buttonsTable.Name = "buttonsTable";
            this.buttonsTable.RowCount = 1;
            this.buttonsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonsTable.Size = new System.Drawing.Size(243, 29);
            this.buttonsTable.TabIndex = 0;
            // 
            // back
            // 
            this.back.Enabled = false;
            this.back.Location = new System.Drawing.Point(3, 3);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(75, 23);
            this.back.TabIndex = 0;
            this.back.Text = "Back";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // next
            // 
            this.next.Location = new System.Drawing.Point(84, 3);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 23);
            this.next.TabIndex = 1;
            this.next.Text = "Next";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(165, 3);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.ForeColor = System.Drawing.Color.Firebrick;
            this.title.Location = new System.Drawing.Point(179, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(72, 18);
            this.title.TabIndex = 1;
            this.title.Text = "Step 1 of 4";
            // 
            // logoTable
            // 
            this.logoTable.AutoSize = true;
            this.logoTable.BackColor = System.Drawing.Color.White;
            this.logoTable.ColumnCount = 1;
            this.logoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.logoTable.Controls.Add(this.logo, 0, 0);
            this.logoTable.Location = new System.Drawing.Point(3, 3);
            this.logoTable.Name = "logoTable";
            this.logoTable.RowCount = 1;
            this.mainTable.SetRowSpan(this.logoTable, 3);
            this.logoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.logoTable.Size = new System.Drawing.Size(170, 206);
            this.logoTable.TabIndex = 2;
            // 
            // logo
            // 
            this.logo.BackColor = System.Drawing.Color.White;
            this.logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.logo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logo.Image = ((System.Drawing.Image)(resources.GetObject("logo.Image")));
            this.logo.Location = new System.Drawing.Point(3, 3);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(164, 200);
            this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logo.TabIndex = 2;
            this.logo.TabStop = false;
            // 
            // NewWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(429, 222);
            this.Controls.Add(this.mainTable);
            this.Name = "NewWizard";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New design";
            this.mainTable.ResumeLayout(false);
            this.mainTable.PerformLayout();
            this.buttonsTable.ResumeLayout(false);
            this.logoTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTable;
        private System.Windows.Forms.TableLayoutPanel buttonsTable;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.TableLayoutPanel logoTable;
    }
}
