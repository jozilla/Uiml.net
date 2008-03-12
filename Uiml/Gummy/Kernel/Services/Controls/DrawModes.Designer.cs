namespace Uiml.Gummy.Kernel.Services.Controls
{
    partial class DrawModes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrawModes));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_erase = new System.Windows.Forms.Button();
            this.m_paintButton = new System.Windows.Forms.Button();
            this.m_cursorButton = new System.Windows.Forms.Button();
            this.m_groupZones = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.m_erase, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_paintButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_cursorButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_groupZones, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(220, 50);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // m_erase
            // 
            this.m_erase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_erase.Image = ((System.Drawing.Image)(resources.GetObject("m_erase.Image")));
            this.m_erase.Location = new System.Drawing.Point(103, 3);
            this.m_erase.Name = "m_erase";
            this.m_erase.Size = new System.Drawing.Size(44, 44);
            this.m_erase.TabIndex = 2;
            this.m_erase.UseVisualStyleBackColor = true;
            this.m_erase.Click += new System.EventHandler(this.m_erase_Click);
            // 
            // m_paintButton
            // 
            this.m_paintButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_paintButton.Enabled = false;
            this.m_paintButton.Image = ((System.Drawing.Image)(resources.GetObject("m_paintButton.Image")));
            this.m_paintButton.Location = new System.Drawing.Point(53, 3);
            this.m_paintButton.Name = "m_paintButton";
            this.m_paintButton.Size = new System.Drawing.Size(44, 44);
            this.m_paintButton.TabIndex = 1;
            this.m_paintButton.UseVisualStyleBackColor = true;
            this.m_paintButton.Click += new System.EventHandler(this.m_paintButton_Click);
            // 
            // m_cursorButton
            // 
            this.m_cursorButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_cursorButton.Image = ((System.Drawing.Image)(resources.GetObject("m_cursorButton.Image")));
            this.m_cursorButton.Location = new System.Drawing.Point(3, 3);
            this.m_cursorButton.Name = "m_cursorButton";
            this.m_cursorButton.Size = new System.Drawing.Size(44, 44);
            this.m_cursorButton.TabIndex = 0;
            this.m_cursorButton.UseVisualStyleBackColor = true;
            this.m_cursorButton.Click += new System.EventHandler(this.cursor_Click);
            // 
            // m_groupZones
            // 
            this.m_groupZones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_groupZones.Image = ((System.Drawing.Image)(resources.GetObject("m_groupZones.Image")));
            this.m_groupZones.Location = new System.Drawing.Point(153, 3);
            this.m_groupZones.Name = "m_groupZones";
            this.m_groupZones.Size = new System.Drawing.Size(44, 44);
            this.m_groupZones.TabIndex = 3;
            this.m_groupZones.UseVisualStyleBackColor = true;
            // 
            // DrawModes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DrawModes";
            this.Size = new System.Drawing.Size(213, 48);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button m_cursorButton;
        private System.Windows.Forms.Button m_erase;
        private System.Windows.Forms.Button m_paintButton;
        private System.Windows.Forms.Button m_groupZones;
    }
}
