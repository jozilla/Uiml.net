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
            DesignerKernel kernel = new DesignerKernel("");
            kernel.Init();
            kernel.Open();
        }
    }
}