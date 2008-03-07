using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class SendDomainObjectBackward : ACommand
    {
        DomainObject m_domObject = null;

        public SendDomainObjectBackward(DomainObject dom) : base()
        {
            Label = "send backward";
            DomainObject = dom;
        }

        public DomainObject DomainObject
        {
            get
            {
                return m_domObject;
            }
            set
            {
                m_domObject = value;
            }
        }

        public override void Execute()
        {
            //CanvasService service = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");
            DesignerKernel.Instance.CurrentDocument.DomainObjects.SendBackward(DomainObject);
        }

        public override void Undo()
        {            
        }
    }
}
