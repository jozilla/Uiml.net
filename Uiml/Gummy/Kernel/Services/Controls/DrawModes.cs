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
            //DesignerKernel.Instance.CurrentDocument.SpaceModeChanged += new Document.SpaceModeChangeHandler(spaceModeChanged);
        }

        private void cursor_Click(object sender, EventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.Mode = Mode.Navigate;
        }

        private void m_paintButton_Click(object sender, EventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.Mode = Mode.Draw;
        }

        private void m_erase_Click(object sender, EventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.Mode = Mode.Erase;
        }

        public void SpaceModeChanged(object sender, Mode mode)
        {
            Button btn = new Button();
            m_cursorButton.BackColor = btn.BackColor;
            m_paintButton.BackColor = btn.BackColor;
            m_erase.BackColor = btn.BackColor;

            switch (mode)
            {
                case Mode.Draw:
                    m_paintButton.BackColor = Color.Yellow;
                    break;
                case Mode.Navigate:
                    m_cursorButton.BackColor = Color.Yellow;
                    break;
                case Mode.Erase:
                    m_erase.BackColor = Color.Yellow;
                    break;
                default:
                    break;
            }
        }

        public bool DrawModeEnabled
        {
            get
            {
                return m_paintButton.Enabled;
            }
            set
            {
                m_paintButton.Enabled = value;
            }
        }

        public bool NavigateModeEnabled
        {
            get
            {
                return m_cursorButton.Enabled;
            }
            set
            {
                m_cursorButton.Enabled = value;
            }
        }        
    }
}
