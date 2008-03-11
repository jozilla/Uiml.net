using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    public class EraseCartesianGraphState : CartesianGraphState
    {
        System.Windows.Forms.MouseEventHandler m_mouseDownHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseUpHandler = null;
        System.Windows.Forms.MouseEventHandler m_mouseMoveHandler = null;
        PaintEventHandler m_paintEventHandler = null;

        bool m_moved = false;
        Rectangle m_eraser = new Rectangle(0,0,20,20);

        public EraseCartesianGraphState(CartesianGraph graph)
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
                m_paintEventHandler = new PaintEventHandler(onPaint);
                m_graph.CartesianGraphPaint += m_paintEventHandler;
            }
        }

        void onPaint(object sender, PaintEventArgs e)
        {
            if (m_moved)
            {                
                e.Graphics.FillRectangle(Brushes.DarkGray, m_eraser);
                e.Graphics.DrawRectangle(Pens.PeachPuff, m_eraser);
            }
        }

        void onMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            m_moved = true;
            m_eraser.Location = new Point(e.Location.X - m_eraser.Width/2, e.Location.Y - m_eraser.Height/2);
            m_graph.Refresh();
        }

        void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {           
        }

        void onMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(Selected.SelectedDomainObject.Instance.Selected != null)
            {
                Rectangle positionnedRectangle = new Rectangle(m_eraser.X - m_graph.Origin.X, m_eraser.Y - m_graph.Origin.Y, m_eraser.Width, m_eraser.Height);
                List<Point> pointsToDelete = Selected.SelectedDomainObject.Instance.Selected.Polygon.PointsInRectangle(positionnedRectangle);
                foreach (Point pnt in pointsToDelete)
                {
                    Selected.SelectedDomainObject.Instance.Selected.Polygon.RemovePoint(pnt);
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
                m_graph.CartesianGraphPaint -= m_paintEventHandler;
            }
        }
    }
}
