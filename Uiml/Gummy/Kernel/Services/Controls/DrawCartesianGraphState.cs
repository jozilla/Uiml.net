using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    public class DrawCartesianGraphState : CartesianGraphState
    {
        System.Windows.Forms.MouseEventHandler m_mouseDownHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseUpHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseMoveHandler = null;
        System.Windows.Forms.MouseEventHandler m_doubleClickHandler = null;

        bool m_finished = false;
        Point m_lastMovePosition = Point.Empty;


        public DrawCartesianGraphState(CartesianGraph graph)
            : base(graph)
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
                m_doubleClickHandler = new System.Windows.Forms.MouseEventHandler(onDoubleClick);
                m_graph.MouseDoubleClick += m_doubleClickHandler;
            }
        }

        void onDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                m_finished = !m_finished;
                if (!m_finished)
                {
                    DomainObject dom = Selected.SelectedDomainObject.Instance.Selected;
                    Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                    ((Shape.Polygon)dom.Shape).AddPointSorted(pnt);
                    m_lastMovePosition = pnt;
                }
            }
        }

        void onMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selected.SelectedDomainObject.Instance.Selected != null && !m_finished)
            {
                DomainObject dom = Selected.SelectedDomainObject.Instance.Selected;
                if (m_lastMovePosition != Point.Empty)
                {
                    ((Shape.Polygon)dom.Shape).RemovePoint(m_lastMovePosition);
                }
                Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                m_lastMovePosition = pnt;
                ((Shape.Polygon)dom.Shape).AddPointSorted(m_lastMovePosition);
            }
        }

        void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {            
        }

        void onMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selected.SelectedDomainObject.Instance.Selected != null && !m_finished)
            {
                DomainObject dom = Selected.SelectedDomainObject.Instance.Selected;
                Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                ((Shape.Polygon)dom.Shape).AddPointSorted(pnt);
                m_lastMovePosition = Point.Empty;
            }
        }

        public override void DestroyEvents()
        {
            if (m_graph != null)
            {
                m_graph.MouseMove -= m_mouseMoveHandler;
                m_graph.MouseUp -= m_mouseUpHandler;
                m_graph.MouseDown -= m_mouseDownHandler;
                m_graph.MouseDoubleClick -= m_doubleClickHandler;
            }
        }

        ~DrawCartesianGraphState()
        {
            DestroyEvents();
        }
    }
}
