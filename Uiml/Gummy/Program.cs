using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Uiml.Gummy.Kernel;

namespace Uiml.Gummy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {           
            DesignerKernel.Instance.Init();
            DesignerKernel.Instance.Open();

            /*
            //Testing purposes.. 
            Form frm = new Form();
            Uiml.Gummy.Kernel.Services.Controls.Arrow m_arrow = new Uiml.Gummy.Kernel.Services.Controls.Arrow();
            m_arrow.Location = new System.Drawing.Point(0, 0);
            m_arrow.Size = new System.Drawing.Size(100, 100);
            frm.Size = new System.Drawing.Size(500, 500);
            frm.Controls.Add(m_arrow);
            frm.Show();
            Application.Run(frm);*/
        }
    }
}