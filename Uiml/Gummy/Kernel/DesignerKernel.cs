using System;
using System.Collections.Generic;
using System.Text;
using Uiml.Gummy.Serialize;
using Uiml.Gummy.Kernel.Services;
using Uiml.Gummy.Serialize.SWF;

using System.Windows.Forms;

namespace Uiml.Gummy.Kernel
{
    public class DesignerKernel : IService
    {
        ToolboxService m_tbService = null;
        CanvasService m_cService = null;
        PropertiesService m_pService = null;
        ApplicationGlueService m_aService = null;

        public DesignerKernel(string vocabulary)
        {
            //For this moment only SWF is accepted
            ActiveSerializer.Instance.Serializer = new SWFUimlSerializer();
            m_tbService = new ToolboxService();
            m_cService = new CanvasService();
            m_pService = new PropertiesService();
            m_aService = new ApplicationGlueService();
        }

        public void Init()
        {
            m_tbService.Init();
            m_tbService.Visible = true;
            m_cService.Init();            
            m_cService.Visible = true;
            m_pService.Init();
            m_pService.Visible = true;
            m_aService.Init();
            m_aService.Visible = true;

            Application.Run(m_tbService);
        }
    }
}
