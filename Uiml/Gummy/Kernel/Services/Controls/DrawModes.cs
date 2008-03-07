using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.Controls
{   

    public partial class DrawModes : UserControl
    {
        public DrawModes()
        {
            InitializeComponent();
        }

        private void cursor_Click(object sender, EventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.Mode = Mode.Navigate;
        }

        private void m_paintButton_Click(object sender, EventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.Mode = Mode.Draw;
        }

        public void SpaceModeChanged(object sender, Mode mode)
        {
            Button btn = new Button();
            m_cursorButton.BackColor = btn.BackColor;
            m_paintButton.BackColor = btn.BackColor;

            switch (mode)
            {
                case Mode.Draw:
                    m_paintButton.BackColor = Color.Yellow;
                    break;
                case Mode.Navigate:
                    m_cursorButton.BackColor = Color.Yellow;
                    break;
            }
        }
        
    }
}
