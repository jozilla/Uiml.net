using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public partial class DefaultValueDialog : Form
    {
        protected MethodParameterModel m_parameter;

        public MethodParameterModel Parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        public string Value
        {
            get { return value.Text; }
        }

        public DefaultValueDialog(MethodParameterModel parameter)
        {
            m_parameter = parameter;
            InitializeComponent();
            
            // initialize dialog
            param.Text = parameter.Name + ":";
        }

        private void value_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = value.Text.Trim() != string.Empty;
        }
    }
}
