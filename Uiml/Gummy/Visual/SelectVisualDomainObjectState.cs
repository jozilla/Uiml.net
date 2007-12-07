using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Visual
{
    public class SelectVisualDomainObjectState : VisualDomainObjectState
    {
        int m_borderSize = 4;
        List<Rectangle> m_rectangles = new List<Rectangle>();
//        SelectedDomainObject.DomainObjectSelectedHandler m_domObjectSelectedHandler = null;
        MouseEventHandler m_mouseDownHandler = null;

        public SelectVisualDomainObjectState()
            : base()
        {
        }

        ~SelectVisualDomainObjectState()
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

        protected void onDomainObjectSelected(DomainObject dom, EventArgs e)
        {/*
            if (dom == m_visDom.DomainObject)
            {
                m_visDom.DomainObject.Selected = true;
            }
            else
            {
                m_visDom.DomainObject.Selected = false;                
            }
            m_visDom.Refresh();*/
        }

        protected override void onMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {            
        }

        protected override void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {            
        }

        protected override void onMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {            
        }

        protected override void onPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {           
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

        private void initializeRectangles()
        {
            m_rectangles.Clear();
            m_rectangles.Add(new Rectangle(0, 0, m_visDom.Width, BorderSize));
            m_rectangles.Add(new Rectangle(0, 0, BorderSize, m_visDom.Height));
            m_rectangles.Add(new Rectangle(0, m_visDom.Height - BorderSize -1, m_visDom.Width, m_visDom.Height));
            m_rectangles.Add(new Rectangle(m_visDom.Width - BorderSize - 1, 0, m_visDom.Width, m_visDom.Height));
        }

        protected void paintSelected(Graphics g)
        {            
            if (m_visDom.DomainObject.Selected)
            {
                initializeRectangles();
                for (int i = 0; i < m_rectangles.Count; i++)
                {
                    g.FillRectangle(Brushes.LightSeaGreen, m_rectangles[i]);
                }                
            }
        }
    }
}
