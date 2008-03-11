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
        Uiml.Gummy.Kernel.Selected.SelectedDomainObject.DomainObjectSelectedHandler m_domSelectHandler = null;

        private bool m_draw = false;
       // private bool m_first = true;

        //A substate of this state is the manipulation...
        ManipulateCartesianGraphState m_manipulateState = null;

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
                m_domSelectHandler = new Uiml.Gummy.Kernel.Selected.SelectedDomainObject.DomainObjectSelectedHandler(onDomainObjectSelected);
                Selected.SelectedDomainObject.Instance.DomainObjectSelected += m_domSelectHandler;
                m_manipulateState = new ManipulateCartesianGraphState(value);
            }
        }

        void onDomainObjectSelected(DomainObject dom, EventArgs e)
        {           
        }

        void onDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                DomainObject dom = Selected.SelectedDomainObject.Instance.Selected;
                Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                dom.Polygon.TmpPoint = pnt;
                if (m_draw)
                {                    
                    dom.Polygon.AddTmpPoint();
                }
                m_draw = !m_draw;
            }
        }

        void onMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selected.SelectedDomainObject.Instance.Selected != null && m_draw)
            {
                DomainObject dom = Selected.SelectedDomainObject.Instance.Selected;
                Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                dom.Polygon.TmpPoint = pnt;                
            }
        }

        void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {            
        }

        void onMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                DomainObject dom = Selected.SelectedDomainObject.Instance.Selected;
                if (Selected.SelectedDomainObject.Instance.Selected.Polygon.Points.Count == 0)
                {
                    Point pnt = new Point(e.Location.X - m_graph.Origin.X, e.Location.Y - m_graph.Origin.Y);
                    dom.Polygon.TmpPoint = pnt;
                    m_draw = true;
                    //m_first = false;
                }                
                if (m_draw)
                {
                    dom.Polygon.AddTmpPoint();
                    //((Shape.Polygon)dom.Shape).AddPointSorted();
                }
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
                Selected.SelectedDomainObject.Instance.DomainObjectSelected -= m_domSelectHandler;
            }
            if (m_manipulateState != null)
                m_manipulateState.DestroyEvents();
        }

        ~DrawCartesianGraphState()
        {
            DestroyEvents();
        }
    }
}
