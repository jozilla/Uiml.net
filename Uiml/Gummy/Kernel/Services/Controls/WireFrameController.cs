using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Uiml.Gummy.Kernel.Services.Commands;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    public partial class WireFrameController : UserControl
    {
        public WireFrameController()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnEnableDisable_Click(object sender, EventArgs e)
        {
            DisableWireFrameExample disableCommand = new DisableWireFrameExample();
            disableCommand.Execute();
        }
    }
}
