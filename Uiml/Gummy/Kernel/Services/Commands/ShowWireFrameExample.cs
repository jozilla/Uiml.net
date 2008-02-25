using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class ShowWireFrameExample : ACommand
    {
        //New state
        Size m_size = Size.Empty;
        //Old state
        Size m_oldSize = Size.Empty;
        bool m_wireFramed = false;

        public ShowWireFrameExample(Size size)
        {
            m_size = size;
        }
        
        public override void Execute()
        { 
            CanvasService canvasService = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");
            
            //Get the old state
            m_oldSize = canvasService.WireFrameSize;
            m_wireFramed = canvasService.WireFramed;
            //Set the new state
            canvasService.WireFrameSize = m_size;            
        }

        public override void Undo()
        {
            CanvasService canvasService = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");

            canvasService.WireFramed = m_wireFramed;
            canvasService.WireFrameSize = m_size;
        }

        public override string Label
        {
            get
            {
                return "Show wire-frames";
            }
        }
    }
}
