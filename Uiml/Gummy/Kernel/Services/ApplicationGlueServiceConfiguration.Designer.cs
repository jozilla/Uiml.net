namespace Uiml.Gummy.Kernel.Services
{
    partial class ApplicationGlueServiceConfiguration
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
            this.browseButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.loadStatus = new System.Windows.Forms.ProgressBar();
            this.serviceLabel = new System.Windows.Forms.Label();
            this.service = new System.Windows.Forms.TextBox();
            this.loadLabel = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.resolutionTable = new System.Windows.Forms.TableLayoutPanel();
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.resolutionTable.SuspendLayout();
            this.mainTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(203, 3);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Browse ...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(203, 32);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 3;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // loadStatus
            // 
            this.loadStatus.Location = new System.Drawing.Point(74, 32);
            this.loadStatus.Name = "loadStatus";
            this.loadStatus.Size = new System.Drawing.Size(123, 23);
            this.loadStatus.TabIndex = 4;
            // 
            // serviceLabel
            // 
            this.serviceLabel.AutoSize = true;
            this.serviceLabel.Location = new System.Drawing.Point(3, 0);
            this.serviceLabel.Name = "serviceLabel";
            this.serviceLabel.Size = new System.Drawing.Size(46, 13);
            this.serviceLabel.TabIndex = 1;
            this.serviceLabel.Text = "Service:";
            // 
            // service
            // 
            this.service.Location = new System.Drawing.Point(74, 3);
            this.service.Name = "service";
            this.service.Size = new System.Drawing.Size(122, 20);
            this.service.TabIndex = 3;
            // 
            // loadLabel
            // 
            this.loadLabel.AutoSize = true;
            this.loadLabel.Location = new System.Drawing.Point(3, 29);
            this.loadLabel.Name = "loadLabel";
            this.loadLabel.Size = new System.Drawing.Size(65, 13);
            this.loadLabel.TabIndex = 2;
            this.loadLabel.Text = "Load status:";
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(3, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(282, 13);
            this.title.TabIndex = 1;
            this.title.Text = "Select the service to design a user interface for.";
            // 
            // resolutionTable
            // 
            this.resolutionTable.AutoSize = true;
            this.resolutionTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolutionTable.ColumnCount = 3;
            this.resolutionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.resolutionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.resolutionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.resolutionTable.Controls.Add(this.serviceLabel, 0, 0);
            this.resolutionTable.Controls.Add(this.loadButton, 2, 1);
            this.resolutionTable.Controls.Add(this.loadStatus, 1, 1);
            this.resolutionTable.Controls.Add(this.loadLabel, 0, 1);
            this.resolutionTable.Controls.Add(this.service, 1, 0);
            this.resolutionTable.Controls.Add(this.browseButton, 2, 0);
            this.resolutionTable.Location = new System.Drawing.Point(3, 67);
            this.resolutionTable.Name = "resolutionTable";
            this.resolutionTable.RowCount = 2;
            this.resolutionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.resolutionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.resolutionTable.Size = new System.Drawing.Size(281, 58);
            this.resolutionTable.TabIndex = 0;
            // 
            // mainTable
            // 
            this.mainTable.AutoSize = true;
            this.mainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTable.ColumnCount = 1;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.Controls.Add(this.resolutionTable, 0, 1);
            this.mainTable.Controls.Add(this.title, 0, 0);
            this.mainTable.Location = new System.Drawing.Point(3, 3);
            this.mainTable.Name = "mainTable";
            this.mainTable.RowCount = 2;
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.Size = new System.Drawing.Size(288, 128);
            this.mainTable.TabIndex = 6;
            // 
            // ApplicationGlueServiceConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.mainTable);
            this.Name = "ApplicationGlueServiceConfiguration";
            this.Size = new System.Drawing.Size(315, 134);
            this.resolutionTable.ResumeLayout(false);
            this.resolutionTable.PerformLayout();
            this.mainTable.ResumeLayout(false);
            this.mainTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.ProgressBar loadStatus;
        private System.Windows.Forms.Label serviceLabel;
        private System.Windows.Forms.TextBox service;
        private System.Windows.Forms.Label loadLabel;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.TableLayoutPanel resolutionTable;
        private System.Windows.Forms.TableLayoutPanel mainTable;
    }
}
