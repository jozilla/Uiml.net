using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Uiml.Gummy.Visual
{
    public abstract class VisualDomainObjectState
    {
        protected MouseEventHandler m_mouseClickHandler = null;
        protected MouseEventHandler m_mouseMoveHandler = null;
        protected MouseEventHandler m_mouseUpHandler = null;
        protected PaintEventHandler m_paintEventHandler = null;

        protected VisualDomainObject m_visDom = null;

        public VisualDomainObjectState()
        {            
        }

        ~VisualDomainObjectState()
        {
            finalize();
        }

        private void finalize()
        {
            if (m_visDom == null)
                return;
            if(m_mouseClickHandler != null)
                m_visDom.MouseClick -= m_mouseClickHandler;
            if(m_mouseMoveHandler != null)
                m_visDom.MouseMove -= m_mouseMoveHandler;
            if(m_mouseUpHandler != null)
                m_visDom.MouseUp -= m_mouseUpHandler;
            if(m_paintEventHandler != null)
                m_visDom.Paint -= m_paintEventHandler;
        }

        public virtual void Attach(VisualDomainObject visDom)
        {
            Detach();
            m_visDom = visDom;
            m_mouseClickHandler = new System.Windows.Forms.MouseEventHandler(onMouseClick);
            visDom.MouseClick += m_mouseClickHandler;
            m_mouseMoveHandler = new System.Windows.Forms.MouseEventHandler(onMouseMove);
            visDom.MouseMove += m_mouseMoveHandler;
            m_mouseUpHandler = new System.Windows.Forms.MouseEventHandler(onMouseUp);
            visDom.MouseUp += m_mouseUpHandler;
            m_paintEventHandler += new PaintEventHandler(onPaint);
            visDom.Paint += m_paintEventHandler;
        }

        public virtual void Detach()
        {
            finalize();
        }

        protected abstract void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e);
        protected abstract void onMouseMove(object sender, System.Windows.Forms.MouseEventArgs e);
        protected abstract void onMouseClick(object sender, System.Windows.Forms.MouseEventArgs e);
        protected abstract void onPaint(object sender, PaintEventArgs e);
    }
}
