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
        Point m_location = Point.Empty;

        public PasteCommand()
            : base()
        {
            DesignerKernel.Instance.GetService("gummy-canvas").ServiceControl.MouseMove += new System.Windows.Forms.MouseEventHandler(canvasMouseMove);
            Label = "paste";
        }

        void canvasMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_location = e.Location;
        }

        public override void Execute()
        {
            if (Selected.SelectedDomainObject.Instance.ClipBoardDomainObject != null)
            {                
                DomainObject pasted = (DomainObject)Selected.SelectedDomainObject.Instance.ClipBoardDomainObject.Clone();
                pasted.Identifier = DomainObjectFactory.Instance.AutoID();
                pasted.Location = m_location;
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
