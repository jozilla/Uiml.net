using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    public class NavigateCartesianGraphState : CartesianGraphState
    {
        bool m_cursorClicked = false;
        System.Windows.Forms.MouseEventHandler m_mouseDownHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseUpHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseMoveHandler = null;

        public NavigateCartesianGraphState(CartesianGraph cart)
            : base(cart)
        {            
        }

        public override CartesianGraph CartesianGraph
        {
            get
            {
                return base.CartesianGraph;
            }
            set
            {
                DestroyEvents();
                base.CartesianGraph = value;
                m_mouseDownHandler = new System.Windows.Forms.MouseEventHandler(onMouseDown);
                m_graph.MouseDown += m_mouseDownHandler;
                m_mouseMoveHandler = new System.Windows.Forms.MouseEventHandler(onMouseMove);
                m_graph.MouseMove += m_mouseMoveHandler;
                m_mouseUpHandler = new System.Windows.Forms.MouseEventHandler(onMouseUp);
                m_graph.MouseUp += m_mouseUpHandler;                
            }
        }

        void onMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_cursorClicked)
            {
                m_graph.CursorPosition = e.Location;
                m_graph.Refresh();
            }
        }

        void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_cursorClicked = false;
        }

        void onMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_cursorClicked = true;
                m_graph.CursorPosition = e.Location;
                m_graph.Refresh();
            }
        }

        ~NavigateCartesianGraphState()
        {
            DestroyEvents();
        }

        public override void DestroyEvents()
        {
            if (m_graph != null)
            {
                m_graph.MouseMove -= m_mouseMoveHandler;
                m_graph.MouseUp -= m_mouseUpHandler;
                m_graph.MouseDown -= m_mouseDownHandler;
            }
        }
    }
}
