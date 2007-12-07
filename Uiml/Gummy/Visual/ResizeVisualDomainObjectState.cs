using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Visual
{
    public enum BoxID
    {
        TopLeft,
        TopMiddle,
        TopRight,
        MiddleLeft,
        MiddleRight,
        BottomLeft,
        BottomRight,
        BottomMiddle,
        Center,
        None
    }

    public class ResizeVisualDomainObjectState : VisualDomainObjectState
    {        
        int m_boxSize = 8;
        Dictionary<BoxID, Rectangle> m_rectangles = new Dictionary<BoxID, Rectangle>();        

        BoxID m_lastClicked = BoxID.Center;
        Point m_delta;
        int m_oldWidth = 0;
        int m_oldHeight = 0;
        int m_oldX = 0;
        int m_oldY = 0;
        Rectangle m_oldBounds;
        bool clicked = false;

        SelectedDomainObject.DomainObjectSelectedHandler m_selectedHandler = null;
        MouseEventHandler m_mouseDownHandler = null;
        //MouseEventHandler m_mouseUpHandler = null;
       
        public ResizeVisualDomainObjectState()
            : base()
        {
        }

        ~ResizeVisualDomainObjectState()
        {
            finalize();
        }

        private void finalize()
        {
            if (m_selectedHandler != null)
                SelectedDomainObject.Instance.DomainObjectSelected -= m_selectedHandler;
            //if (m_mouseUpHandler != null)
            //    m_visDom.MouseUp -= m_mouseUpHandler;
            if (m_mouseDownHandler != null)
                m_visDom.MouseDown -= m_mouseDownHandler;
        }

        public override void Detach()
        {
            finalize();
            base.Detach();
        }

        public override void Attach(VisualDomainObject visDom)
        {
            base.Attach(visDom);
            m_selectedHandler = new SelectedDomainObject.DomainObjectSelectedHandler(onDomainObjectSelected);
            SelectedDomainObject.Instance.DomainObjectSelected += m_selectedHandler;
            m_mouseDownHandler = new MouseEventHandler(onMouseDown);
            m_visDom.MouseDown += m_mouseDownHandler;
            //m_mouseUpHandler = new MouseEventHandler(onMouseUp);
            //m_visDom.MouseUp += m_mouseUpHandler;
        }

        /*void onMouseUp(object sender, MouseEventArgs e)
        {
            Console.Out.WriteLine("mouseUp");
        }*/
        

        void onDomainObjectSelected(DomainObject dom, EventArgs e)
        {
            if (m_visDom.DomainObject == dom)
            {
                m_visDom.DomainObject.Selected = true;
            }
            else
            {
                m_visDom.DomainObject.Selected = false;
            }
            m_visDom.Refresh();
        }

        public int BoxSize
        {
            get
            {
                return m_boxSize;
            }
            set
            {
                m_boxSize = value;
            }
        }

        void onMouseDown(object sender, MouseEventArgs e)
        {
            Console.Out.WriteLine("mousDown");
            clicked = true;
            
            m_lastClicked = clickedBox(e.X, e.Y);            
            m_delta = new Point(e.X, e.Y);            
            m_oldWidth = m_visDom.Width;
            m_oldHeight = m_visDom.Height;
            m_oldX = e.X + m_visDom.Location.X;
            m_oldY = e.Y + m_visDom.Location.Y;
            m_oldBounds = m_visDom.Bounds;

            if (!m_visDom.DomainObject.Selected)
            {
                SelectedDomainObject.Instance.Selected = m_visDom.DomainObject;
            }

            m_visDom.Refresh();

        }
        
        protected override void onMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {           
        }

        protected override void onMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!clicked)
                return;
            int position_x = e.X + m_visDom.Location.X;
            int position_y = e.Y + m_visDom.Location.Y;

            int dX = position_x - m_oldX;
            int dY = position_y - m_oldY;

            int width = 0;
            int height = 0;
            
            Point loc = new Point(m_oldBounds.X, m_oldBounds.Y);

            switch (m_lastClicked)
            {
                case BoxID.None :
                    break;
                case BoxID.Center:
                    m_visDom.DomainObject.Location = new Point(m_visDom.Location.X + e.X - m_delta.X,
                        m_visDom.Location.Y + e.Y - m_delta.Y
                        );
                    break;
                case BoxID.BottomLeft:
                    width =	m_oldBounds.Width - dX;
					height = m_oldBounds.Height + dY;
                    loc = new Point(m_oldBounds.X + dX, m_oldBounds.Y);
                    if (m_visDom.Location.X + m_visDom.DomainObject.Size.Width - loc.X > (2 * BoxSize))
                    {
                        m_visDom.DomainObject.Location = loc;
                        m_visDom.DomainObject.Size = new Size(width <= (2 * BoxSize) ? (2 * BoxSize) : width, height <= (2 * BoxSize) ? (2 * BoxSize) : height);
                    }
                    break;
                case BoxID.BottomMiddle:
                    height = m_oldBounds.Height + dY;
                    m_visDom.DomainObject.Location = new Point(m_oldBounds.X, m_oldBounds.Y);
                    m_visDom.DomainObject.Size = new Size(m_oldBounds.Width, height <= (2 * BoxSize) ? (2 * BoxSize) : height); 
                    break;
                case BoxID.BottomRight:
                    height = m_oldBounds.Height + dY;
					width =	m_oldBounds.Width + dX ;
					m_visDom.DomainObject.Location = new Point(m_oldBounds.X, m_oldBounds.Y);
                    m_visDom.DomainObject.Size = new Size(width <= (2 * BoxSize) ? (2 * BoxSize) : width, height <= (2 * BoxSize) ? (2 * BoxSize) : height); 
                    break;
                case BoxID.MiddleLeft:
                    width = m_oldBounds.Width - dX;
                    loc = new Point(m_oldBounds.X + dX, m_oldBounds.Y);
                    if (m_visDom.Location.X + m_visDom.DomainObject.Size.Width - loc.X > (2 * BoxSize))
                    {
                        m_visDom.DomainObject.Location = loc;
                        m_visDom.DomainObject.Size = new Size(width <= (2 * BoxSize) ? (2 * BoxSize) : width, m_oldBounds.Height);
                    }
                    break;
                case BoxID.MiddleRight:
                    width = m_oldBounds.Width + dX;
                    m_visDom.DomainObject.Location = new Point(m_oldBounds.X, m_oldBounds.Y);
                    m_visDom.DomainObject.Size = new Size(width <=( 2*BoxSize) ?( 2*BoxSize) : width, m_oldBounds.Height); 
                    break;
                case BoxID.TopLeft:
                    width = m_oldBounds.Width - dX;
                    height = m_oldBounds.Height - dY;
                    loc = new Point(m_oldBounds.X + dX, m_oldBounds.Y + dY);
                    if (m_visDom.Location.X + m_visDom.DomainObject.Size.Width - loc.X > (2 * BoxSize) &&
                        m_visDom.Location.Y + m_visDom.DomainObject.Size.Height - loc.Y > (2 * BoxSize))
                    {
                        m_visDom.DomainObject.Location = loc;
                        m_visDom.DomainObject.Size = new Size(width <= (2 * BoxSize) ? (2 * BoxSize) : width, height <= (2 * BoxSize) ? (2 * BoxSize) : height);
                    }
                    break;
                case BoxID.TopMiddle:
                    height = m_oldBounds.Height - dY;
                    loc = new Point(m_oldBounds.X, m_oldBounds.Y + dY);
                    if (m_visDom.Location.Y + m_visDom.DomainObject.Size.Height - loc.Y > (2 * BoxSize))
                    {
                        m_visDom.DomainObject.Location = loc;
                        m_visDom.DomainObject.Size = new Size(m_oldBounds.Width, height <= (2 * BoxSize) ? (2 * BoxSize) : height);
                    }
                    //loc = new Point(m_oldBounds.X, m_oldBounds.Y + dY);
                    break;
                case BoxID.TopRight:
                    height = m_oldBounds.Height - dY;
                    width = m_oldBounds.Width + dX;
                    loc = new Point(m_oldBounds.X, m_oldBounds.Y + dY);
                    if (m_visDom.Location.Y + m_visDom.DomainObject.Size.Height - loc.Y > (2 * BoxSize))
                    {
                        m_visDom.DomainObject.Location = loc;
                        m_visDom.DomainObject.Size = new Size(width <= (2 * BoxSize) ? (2 * BoxSize) : width, height <= (2 * BoxSize) ? (2 * BoxSize) : height);
                    }
                    break;                    
            }
        }

        protected override void onMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("mouseUp");
            clicked = false;
        }

        protected override void onPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (m_visDom.DomainObject.Selected)
            {
                PaintBoundary(e.Graphics);
            }
        }

        private void initializeBoxes()
        {
            m_rectangles.Clear();
            m_rectangles.Add(BoxID.TopLeft, new Rectangle(0, 0, BoxSize, BoxSize));
            m_rectangles.Add(BoxID.TopMiddle, new Rectangle(m_visDom.Width / 2 - BoxSize / 2, 0, BoxSize, BoxSize));
            m_rectangles.Add(BoxID.TopRight, new Rectangle(m_visDom.Width - BoxSize, 0, BoxSize, BoxSize));
            m_rectangles.Add(BoxID.BottomLeft, new Rectangle(0, m_visDom.Height - BoxSize, BoxSize, BoxSize));
            m_rectangles.Add(BoxID.BottomMiddle, new Rectangle(m_visDom.Width / 2 - BoxSize / 2, m_visDom.Height - BoxSize, BoxSize, BoxSize));
            m_rectangles.Add(BoxID.BottomRight, new Rectangle(m_visDom.Width - BoxSize, m_visDom.Height - BoxSize, BoxSize, BoxSize));
            m_rectangles.Add(BoxID.MiddleLeft, new Rectangle(0, m_visDom.Height / 2 - BoxSize / 2, BoxSize, BoxSize));
            m_rectangles.Add(BoxID.MiddleRight, new Rectangle(m_visDom.Width - BoxSize, m_visDom.Height / 2 - BoxSize / 2, BoxSize, BoxSize));
        }

        private BoxID clickedBox(int x, int y)
        {
            Dictionary<BoxID, Rectangle>.Enumerator enumerator = m_rectangles.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.Contains(x, y))
                    return enumerator.Current.Key;
            }
            return BoxID.Center;
        }

        protected void PaintBoundary(Graphics g)
        {
            initializeBoxes();
            Dictionary<BoxID,Rectangle>.Enumerator enumerator = m_rectangles.GetEnumerator();
            while (enumerator.MoveNext())
            {
                g.FillRectangle(Brushes.RosyBrown, enumerator.Current.Value);
            }
        }
    }
}
