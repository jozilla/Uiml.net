using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class BringDomainObjectForward : ACommand
    {
        DomainObject m_dom = null;

        public BringDomainObjectForward(DomainObject dom)
            : base()
        {
            m_dom = dom;
            Label = "bring forward";
        }

        public DomainObject DomainObject
        {
            get
            {
                return m_dom;
            }
            set
            {
                m_dom = value;
            }
        }

        public override void Execute()
        {
            //CanvasService canvasService = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");
            DesignerKernel.Instance.CurrentDocument.DomainObjects.MoveUp(m_dom);
        }

        public override void Undo()
        {            
        }
    }
}
