using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public partial class MethodsView : UserControl
    {
        private MethodsModel model;

        public MethodsModel Model
        {
            get { return model; }
            set { model = value; }
        }

        public MethodsView (MethodsModel model)
        {
            InitializeComponent();
            Model = model;
            Draw();
        }

        protected void Draw ()
        {
            // create headers
            Label linputs = new Label();
            linputs.Text = "Inputs";
            linputs.AutoSize = true;
            linputs.Dock = System.Windows.Forms.DockStyle.Fill;
            linputs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            Label lname = new Label();
            lname.Text = "Method name";
            lname.AutoSize = true;
            lname.Dock = System.Windows.Forms.DockStyle.Fill;
            lname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            Label loutputs = new Label();
            loutputs.Text = "Outputs";
            loutputs.AutoSize = true;
            loutputs.Dock = System.Windows.Forms.DockStyle.Fill;
            loutputs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // add them
            layout.Controls.Add(linputs, 0, 0);
            layout.Controls.Add(lname, 1, 0);
            layout.Controls.Add(loutputs, 2, 0);

            // add the methods, starting from the second row
            int row = 1; 
            foreach (MethodModel method in Model.Methods)
            {
                // create input container
                TableLayoutPanel inputs = new TableLayoutPanel();
                inputs.AutoSize = true;
                inputs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                inputs.BackColor = System.Drawing.Color.Transparent;
                inputs.ColumnCount = 1;
                inputs.RowCount = 1;
                inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                inputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
                inputs.Name = "inputs";
                layout.Controls.Add(inputs, 0, row);

                // create method name
                Label methodName = new Label();
                methodName.AutoSize = true;
                methodName.BackColor = System.Drawing.Color.DarkOrange;
                methodName.Dock = System.Windows.Forms.DockStyle.Fill;
                methodName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                methodName.ForeColor = System.Drawing.Color.White;
                methodName.Name = "methodName";
                methodName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                
                methodName.Text = method.Name;
                layout.Controls.Add(methodName, 1, row);

                // create output container
                TableLayoutPanel outputs = new TableLayoutPanel();
                outputs.AutoSize = true;
                outputs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                outputs.BackColor = System.Drawing.Color.Transparent;
                outputs.ColumnCount = 1;
                outputs.RowCount = 1;
                outputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                outputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
                outputs.Name = "outputs";
                layout.Controls.Add(outputs, 2, row);

                // populate inputs
                int inRow = 0;
                foreach (MethodParameterModel input in method.Inputs)
                {
                    MethodParameterView inView = new MethodParameterView(input);
                    inputs.Controls.Add(inView, 0, inRow);
                    inputs.RowCount = inputs.RowCount + 1;
                    inRow++;
                }

                // populate outputs
                int outRow = 0;
                foreach (MethodParameterModel output in method.Outputs)
                {
                    MethodParameterView outView = new MethodParameterView(output);
                    outputs.Controls.Add(outView, 0, outRow);
                    outputs.RowCount = outputs.RowCount + 1;
                    outRow++;
                }

                row++;
            }
        }
    }
}
