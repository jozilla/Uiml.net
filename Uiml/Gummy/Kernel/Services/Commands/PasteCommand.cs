using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel.Services;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class PasteCommand : ACommand
    {       
        public PasteCommand()
            : base()
        {            
            Label = "paste";
        }

        public override void Execute()
        {
            if (Selected.SelectedDomainObject.Instance.ClipBoardDomainObject != null)
            {                
                DomainObject pasted = (DomainObject)Selected.SelectedDomainObject.Instance.ClipBoardDomainObject.Clone();
                pasted.Identifier = DomainObjectFactory.Instance.AutoID();               
                pasted.Location = ((CanvasService)DesignerKernel.Instance.GetService("gummy-canvas")).MouseLocation;
                ((CanvasService)DesignerKernel.Instance.GetService("gummy-canvas")).DomainObjects.Add(pasted);
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
