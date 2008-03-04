using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Visual
{
    public class ToolboxVisualDomainObjectState : VisualDomainObjectState
    {
        int m_borderSize = 4;
        List<Rectangle> m_rectangles = new List<Rectangle>();
        MouseEventHandler m_mouseDownHandler = null;

        public ToolboxVisualDomainObjectState()
            : base()
        {
        }

        ~ToolboxVisualDomainObjectState()
        {
            finalize();
        }

        private void finalize()
        {
            if(m_mouseDownHandler != null && m_visDom != null)
                m_visDom.MouseDown -= m_mouseDownHandler;
        }

        public override void Detach()
        {
            base.Detach();
            finalize();
        }

        public override void Attach(VisualDomainObject visDom)
        {
            base.Attach(visDom);            
            m_mouseDownHandler = new MouseEventHandler(onMouseDown);
            m_visDom.MouseDown += m_mouseDownHandler;
        }

        void onMouseDown(object sender, MouseEventArgs e)
        {
            DragDropEffects effect = m_visDom.DoDragDrop(m_visDom.DomainObject, DragDropEffects.Move);            
        }

        public int BorderSize
        {
            get
            {
                return m_borderSize;
            }
            set
            {
                m_borderSize = value;
            }
        }      
        
    }
}
