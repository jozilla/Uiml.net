namespace Uiml.Gummy.Kernel.Services
{
    partial class ToolboxServiceConfiguration
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
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.resolutionTable = new System.Windows.Forms.TableLayoutPanel();
            this.title = new System.Windows.Forms.Label();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.platformWidgetsetTable = new System.Windows.Forms.TableLayoutPanel();
            this.platform = new System.Windows.Forms.ComboBox();
            this.widgetset = new System.Windows.Forms.ComboBox();
            this.mainTable.SuspendLayout();
            this.mainTableLayout.SuspendLayout();
            this.platformWidgetsetTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTable
            // 
            this.mainTable.AutoSize = true;
            this.mainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTable.ColumnCount = 1;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainTable.Controls.Add(this.resolutionTable, 0, 1);
            this.mainTable.Location = new System.Drawing.Point(0, 0);
            this.mainTable.Name = "mainTable";
            this.mainTable.RowCount = 2;
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTable.Size = new System.Drawing.Size(200, 100);
            this.mainTable.TabIndex = 0;
            // 
            // resolutionTable
            // 
            this.resolutionTable.AutoSize = true;
            this.resolutionTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolutionTable.ColumnCount = 2;
            this.resolutionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.resolutionTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.resolutionTable.Location = new System.Drawing.Point(3, 23);
            this.resolutionTable.Name = "resolutionTable";
            this.resolutionTable.RowCount = 2;
            this.resolutionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.resolutionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.resolutionTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.resolutionTable.Size = new System.Drawing.Size(0, 0);
            this.resolutionTable.TabIndex = 0;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(3, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(213, 13);
            this.title.TabIndex = 1;
            this.title.Text = "Specify the target screen resolution.";
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.AutoSize = true;
            this.mainTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayout.Controls.Add(this.titleLabel, 0, 0);
            this.mainTableLayout.Controls.Add(this.platformWidgetsetTable, 0, 1);
            this.mainTableLayout.Location = new System.Drawing.Point(3, 3);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 2;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.Size = new System.Drawing.Size(293, 68);
            this.mainTableLayout.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(3, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(194, 13);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Select a platform and widget set.";
            // 
            // platformWidgetsetTable
            // 
            this.platformWidgetsetTable.ColumnCount = 2;
            this.platformWidgetsetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.platformWidgetsetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.platformWidgetsetTable.Controls.Add(this.platform, 0, 0);
            this.platformWidgetsetTable.Controls.Add(this.widgetset, 1, 0);
            this.platformWidgetsetTable.Location = new System.Drawing.Point(3, 16);
            this.platformWidgetsetTable.Name = "platformWidgetsetTable";
            this.platformWidgetsetTable.RowCount = 1;
            this.platformWidgetsetTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.platformWidgetsetTable.Size = new System.Drawing.Size(287, 49);
            this.platformWidgetsetTable.TabIndex = 1;
            // 
            // platform
            // 
            this.platform.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.platform.FormattingEnabled = true;
            this.platform.Location = new System.Drawing.Point(3, 25);
            this.platform.Name = "platform";
            this.platform.Size = new System.Drawing.Size(121, 21);
            this.platform.TabIndex = 0;
            this.platform.Text = "Platform";
            this.platform.SelectedIndexChanged += new System.EventHandler(this.platform_SelectedIndexChanged);
            // 
            // widgetset
            // 
            this.widgetset.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.widgetset.FormattingEnabled = true;
            this.widgetset.Location = new System.Drawing.Point(130, 25);
            this.widgetset.Name = "widgetset";
            this.widgetset.Size = new System.Drawing.Size(154, 21);
            this.widgetset.TabIndex = 1;
            this.widgetset.Text = "Widget set";
            this.widgetset.SelectedIndexChanged += new System.EventHandler(this.widgetset_SelectedIndexChanged);
            // 
            // ToolboxServiceConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.mainTableLayout);
            this.Name = "ToolboxServiceConfiguration";
            this.Size = new System.Drawing.Size(299, 74);
            this.mainTable.ResumeLayout(false);
            this.mainTable.PerformLayout();
            this.mainTableLayout.ResumeLayout(false);
            this.mainTableLayout.PerformLayout();
            this.platformWidgetsetTable.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTable;
        private System.Windows.Forms.TableLayoutPanel resolutionTable;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.ComboBox platform;
        private System.Windows.Forms.TableLayoutPanel platformWidgetsetTable;
        private System.Windows.Forms.ComboBox widgetset;
    }
}
