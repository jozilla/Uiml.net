using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Kernel.Services;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class DisableWireFrameExample : ACommand
    {

        public DisableWireFrameExample()
            : base()
        {
            Label = "disable wireframes";
        }

        public override void Execute()
        {
            CanvasService canvasService = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");
            WireFrameService wireFrameService = (WireFrameService)DesignerKernel.Instance.GetService("gummy-wireframes");

            canvasService.WireFramed = false;
            wireFrameService.ServiceControl.Enabled = false;
        }

        public override void Undo()
        {            
        }
  
    }
}
