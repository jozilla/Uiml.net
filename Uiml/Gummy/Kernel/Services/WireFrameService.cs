using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Kernel.Services.Controls;

namespace Uiml.Gummy.Kernel.Services
{
    public class WireFrameService : IService
    {
        WireFrameController m_controller = null;

        public void Init()
        {
            m_controller = new WireFrameController();
            m_controller.Enabled = false;
        }

        public bool Open()
        {
            return true;
        }

        public bool Close()
        {
            return true;
        }

        public string ServiceName
        {
            get
            {
                return "gummy-wireframes";
            }
        }

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return m_controller;
            }
        }

        public IServiceConfiguration ServiceConfiguration
        {
            get
            {
                return null;
            }
        }

        public void NotifyConfigurationChanged()
        {
        }
    }
}
