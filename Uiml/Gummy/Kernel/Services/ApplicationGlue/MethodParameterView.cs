using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public partial class MethodParameterView : UserControl
    {
        private MethodParameterModel model;

        public MethodParameterModel Model
        {
            get { return model; }
            set { model = value; }
        }

        public MethodParameterView (MethodParameterModel model)
        {
            InitializeComponent();
            Model = model;
            Draw();
        }

        protected void Draw ()
        {
            // set text and color
            if (Model.IsOutput)
            {
                param.Text = Model.Type.ToString();
                param.BackColor = Color.LightCoral;
                param.ForeColor = Color.White;
            }
            else
            {
                param.Text = string.Format("{0} [{1}]", Model.Type, Model.Name);
                param.BackColor = Color.LightGreen;
                param.ForeColor = Color.Black;
            }
        }
    }
}
