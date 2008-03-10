using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public partial class MethodView : UserControl
    {
        private MethodModel model;

        public MethodModel Model
        {
            get { return model; }
            set { model = value; }
        }

        public MethodView (MethodModel m)
        {
            InitializeComponent();
            Model = m;
            Draw();
        }

        protected void Draw ()
        {
            // remove existing controls
            inputs.Controls.Clear();
            outputs.Controls.Clear();

            // set method name text
            methodName.Text = Model.Name;

            // set inputs
            int row = 0;
            foreach (MethodParameterModel input in Model.Inputs)
            {
                MethodParameterView inView = new MethodParameterView(input);
                outputs.Controls.Add(inView, 0, row);
                inputs.RowCount = inputs.RowCount + 1;
                row++;
            }

            // set outputs
            row = 0;
            foreach (MethodParameterModel output in Model.Outputs)
            {
                MethodParameterView outView = new MethodParameterView(output);
                outputs.Controls.Add(outView, 0, row);
                outputs.RowCount = outputs.RowCount + 1;
                row++;
            }
        }
    }
}
