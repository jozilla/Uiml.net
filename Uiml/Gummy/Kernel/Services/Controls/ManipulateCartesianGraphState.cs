using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    public class ManipulateCartesianGraphState : CartesianGraphState
    {
        System.Windows.Forms.MouseEventHandler m_mouseDownHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseUpHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseMoveHandler = null;

        int m_clicked = -1;

        public ManipulateCartesianGraphState(CartesianGraph cart)
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
            if (Selected.SelectedDomainObject.Instance.Selected != null && m_clicked != -1)
            {
                Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                Selected.SelectedDomainObject.Instance.Selected.Polygon.ReplacePoint(m_clicked, pnt);
            }
        }

        void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_clicked = -1;
        }

        void onMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                m_clicked = Selected.SelectedDomainObject.Instance.Selected.Polygon.ClickedPoint(pnt);
            }
        }

        ~ManipulateCartesianGraphState()
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
