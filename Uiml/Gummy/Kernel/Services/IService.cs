using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services
{
    public interface IService
    {
        void Init();
        bool Open();
        bool Close();

        string ServiceName
        {
            get;
        }

        System.Windows.Forms.Control ServiceControl
        {
            get;
        }

        System.Windows.Forms.Control ServiceConfigurationControl 
        {
            get;
        }
    }
}
