namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    partial class MethodView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.methodName = new System.Windows.Forms.Label();
            this.inputs = new System.Windows.Forms.TableLayoutPanel();
            this.outputs = new System.Windows.Forms.TableLayoutPanel();
            this.layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // layout
            // 
            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layout.BackColor = System.Drawing.Color.Transparent;
            this.layout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.layout.ColumnCount = 3;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layout.Controls.Add(this.methodName, 1, 0);
            this.layout.Controls.Add(this.inputs, 0, 0);
            this.layout.Controls.Add(this.outputs, 2, 0);
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.RowCount = 1;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Size = new System.Drawing.Size(104, 15);
            this.layout.TabIndex = 0;
            // 
            // methodName
            // 
            this.methodName.AutoSize = true;
            this.methodName.BackColor = System.Drawing.Color.DarkOrange;
            this.methodName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.methodName.ForeColor = System.Drawing.Color.White;
            this.methodName.Location = new System.Drawing.Point(11, 1);
            this.methodName.Name = "methodName";
            this.methodName.Size = new System.Drawing.Size(82, 13);
            this.methodName.TabIndex = 4;
            this.methodName.Text = "method name";
            this.methodName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // inputs
            // 
            this.inputs.AutoSize = true;
            this.inputs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.inputs.BackColor = System.Drawing.Color.Transparent;
            this.inputs.ColumnCount = 1;
            this.inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.inputs.Location = new System.Drawing.Point(4, 4);
            this.inputs.Name = "inputs";
            this.inputs.RowCount = 1;
            this.inputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.inputs.Size = new System.Drawing.Size(0, 0);
            this.inputs.TabIndex = 6;
            // 
            // outputs
            // 
            this.outputs.AutoSize = true;
            this.outputs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outputs.ColumnCount = 1;
            this.outputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.outputs.Location = new System.Drawing.Point(100, 4);
            this.outputs.Name = "outputs";
            this.outputs.RowCount = 1;
            this.outputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.outputs.Size = new System.Drawing.Size(0, 0);
            this.outputs.TabIndex = 7;
            // 
            // MethodView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.layout);
            this.Name = "MethodView";
            this.Size = new System.Drawing.Size(107, 18);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label methodName;
        private System.Windows.Forms.TableLayoutPanel inputs;
        private System.Windows.Forms.TableLayoutPanel outputs;

    }
}
