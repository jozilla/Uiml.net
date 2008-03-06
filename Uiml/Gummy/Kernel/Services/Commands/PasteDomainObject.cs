using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel.Services;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class PasteDomainObject : ACommand
    {
        private Point m_defaultLocation = Point.Empty;

        public PasteDomainObject()
            : base()
        {            
            Label = "paste";
        }

        public PasteDomainObject(Point defaultLocation)
            : this()
        {
            DefaultLocation = defaultLocation;
        }

        public Point DefaultLocation
        {
            get
            {
                return m_defaultLocation;
            }
            set
            {
                m_defaultLocation = value;
            }
        }

        public override void Execute()
        {
            if (Enabled)
            {                
                DomainObject pasted = (DomainObject)Selected.SelectedDomainObject.Instance.ClipBoardDomainObject.Clone();
                pasted.Identifier = DomainObjectFactory.Instance.AutoID();
                if (DefaultLocation == Point.Empty)
                    pasted.Location = ((CanvasService)DesignerKernel.Instance.GetService("gummy-canvas")).MouseLocation;
                else
                    pasted.Location = DefaultLocation;
                DesignerKernel.Instance.CurrentDocument.DomainObjects.Add(pasted);
                (DomainObject)Selected.SelectedDomainObject.Instance.Selected = pasted;
            }
        }

        public override void Undo()
        {            
        }

        public override bool Enabled
        {
            get
            {
                return Selected.SelectedDomainObject.Instance.ClipBoardDomainObject != null;
            }
            set
            {               
            }
        }
    }
}
