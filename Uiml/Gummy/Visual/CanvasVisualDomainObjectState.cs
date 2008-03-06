using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel;
using Uiml.Gummy.Kernel.Services;
using Uiml.Gummy.Kernel.Services.Commands;
using Uiml.Gummy.Kernel.Selected;

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

    public enum MoveState
    {
        None,
        Resize,
        Move
    }

    public class CanvasVisualDomainObjectState : VisualDomainObjectState
    {        
        int m_boxSize = 8;
        Dictionary<BoxID, Rectangle> m_rectangles = new Dictionary<BoxID, Rectangle>();
        MoveState m_moveState = MoveState.None;

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
        MouseEventHandler m_mouseMoveHandler = null;
        MouseEventHandler m_mouseUpHandler = null;
        PaintEventHandler m_paintEventHandler = null;

        List<ICommand> m_commands = new List<ICommand>();
       
        public CanvasVisualDomainObjectState()
            : base()
        {
        }

        ~CanvasVisualDomainObjectState()
        {
            finalize();
        }

        private void finalize()
        {
            if (m_selectedHandler != null)
                SelectedDomainObject.Instance.DomainObjectSelected -= m_selectedHandler;
            if (m_mouseDownHandler != null)
                m_visDom.MouseDown -= m_mouseDownHandler;
            if (m_mouseMoveHandler != null)
                m_visDom.MouseMove -= m_mouseMoveHandler;
            if (m_mouseUpHandler != null)
                m_visDom.MouseUp -= m_mouseUpHandler;
            if (m_paintEventHandler != null)
                m_visDom.Paint -= m_paintEventHandler;
        }

        public override void Detach()
        {
            finalize();
            base.Detach();
        }

        public override void Attach(VisualDomainObject visDom)
        {
            base.Attach(visDom);
            visDom.IconMode = false;
            visDom.FixSize();
            m_selectedHandler = new SelectedDomainObject.DomainObjectSelectedHandler(onDomainObjectSelected);
            SelectedDomainObject.Instance.DomainObjectSelected += m_selectedHandler;
            m_mouseDownHandler = new MouseEventHandler(mouseDownEvent);
            m_visDom.MouseDown += m_mouseDownHandler;
            m_mouseMoveHandler = new System.Windows.Forms.MouseEventHandler(mouseMoveEvent);
            visDom.MouseMove += m_mouseMoveHandler;
            m_mouseUpHandler = new System.Windows.Forms.MouseEventHandler(mouseUpEvent);
            visDom.MouseUp += m_mouseUpHandler;
            m_paintEventHandler += new PaintEventHandler(paintEvent);
            visDom.Paint += m_paintEventHandler;
            m_commands.Add(new CopyDomainObject(visDom.DomainObject));
            m_commands.Add(new PasteDomainObject());
            m_commands.Add(new DeleteDomainObject(visDom.DomainObject));
            m_commands.Add(new CutDomainObject(visDom.DomainObject));
        }    

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

        void mouseDownEvent(object sender, MouseEventArgs e)
        {
            Console.Out.WriteLine("mousDown");            

            if (!m_visDom.DomainObject.Selected)
            {
                SelectedDomainObject.Instance.Selected = m_visDom.DomainObject;
            }

            if (e.Button == MouseButtons.Left)
            {
                clicked = true;

                m_lastClicked = clickedBox(e.X, e.Y);
                m_delta = new Point(e.X, e.Y);
                m_oldWidth = m_visDom.Width;
                m_oldHeight = m_visDom.Height;
                m_oldX = e.X + m_visDom.Location.X;
                m_oldY = e.Y + m_visDom.Location.Y;
                m_oldBounds = m_visDom.Bounds;

                m_visDom.Refresh();
                m_moveState = MoveState.None;
            }
            else
            {
                ContextMenu cMenu = new ContextMenu();
                Menu menu = (Menu)cMenu;
                MenuFactory.CreateMenu(m_commands, ref menu);
                cMenu.Show(m_visDom, e.Location);
            }

        }        

        protected void mouseMoveEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!clicked)
            {
                m_moveState = MoveState.None;
                return;
            }
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
                    m_moveState = MoveState.None;
                    break;
                case BoxID.Center:
                    m_visDom.DomainObject.Location = new Point(m_visDom.Location.X + e.X - m_delta.X,
                        m_visDom.Location.Y + e.Y - m_delta.Y
                        );
                    m_moveState = MoveState.Move;
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
                    m_moveState = MoveState.Resize;
                    break;
                case BoxID.BottomMiddle:
                    height = m_oldBounds.Height + dY;
                    m_visDom.DomainObject.Location = new Point(m_oldBounds.X, m_oldBounds.Y);
                    m_visDom.DomainObject.Size = new Size(m_oldBounds.Width, height <= (2 * BoxSize) ? (2 * BoxSize) : height);
                    m_moveState = MoveState.Resize;
                    break;
                case BoxID.BottomRight:
                    height = m_oldBounds.Height + dY;
					width =	m_oldBounds.Width + dX ;
					m_visDom.DomainObject.Location = new Point(m_oldBounds.X, m_oldBounds.Y);
                    m_visDom.DomainObject.Size = new Size(width <= (2 * BoxSize) ? (2 * BoxSize) : width, height <= (2 * BoxSize) ? (2 * BoxSize) : height);
                    m_moveState = MoveState.Resize;
                    break;
                case BoxID.MiddleLeft:
                    width = m_oldBounds.Width - dX;
                    loc = new Point(m_oldBounds.X + dX, m_oldBounds.Y);
                    if (m_visDom.Location.X + m_visDom.DomainObject.Size.Width - loc.X > (2 * BoxSize))
                    {
                        m_visDom.DomainObject.Location = loc;
                        m_visDom.DomainObject.Size = new Size(width <= (2 * BoxSize) ? (2 * BoxSize) : width, m_oldBounds.Height);
                    }
                    m_moveState = MoveState.Resize;
                    break;
                case BoxID.MiddleRight:
                    width = m_oldBounds.Width + dX;
                    m_visDom.DomainObject.Location = new Point(m_oldBounds.X, m_oldBounds.Y);
                    m_visDom.DomainObject.Size = new Size(width <=( 2*BoxSize) ?( 2*BoxSize) : width, m_oldBounds.Height);
                    m_moveState = MoveState.Resize;
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
                    m_moveState = MoveState.Resize;
                    break;
                case BoxID.TopMiddle:
                    height = m_oldBounds.Height - dY;
                    loc = new Point(m_oldBounds.X, m_oldBounds.Y + dY);
                    if (m_visDom.Location.Y + m_visDom.DomainObject.Size.Height - loc.Y > (2 * BoxSize))
                    {
                        m_visDom.DomainObject.Location = loc;
                        m_visDom.DomainObject.Size = new Size(m_oldBounds.Width, height <= (2 * BoxSize) ? (2 * BoxSize) : height);
                    }
                    m_moveState = MoveState.Resize;
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
                    m_moveState = MoveState.Resize;
                    break;                    
            }
        }

        void mouseUpEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("mouseUp");
            switch (m_moveState)
            {
                case MoveState.Move:
                    if (m_oldBounds.Location.X != m_visDom.Location.X ||
                        m_oldBounds.Location.Y != m_visDom.Location.Y)
                    {
                        addSnapshot();
                    }
                    break;
                case MoveState.Resize:
                    if (m_oldBounds.Size.Width != m_visDom.Width ||
                        m_oldBounds.Size.Height != m_visDom.Height)
                    {
                        addSnapshot();
                    }
                    m_visDom.FixSize();
                    break;
            }
            m_moveState = MoveState.None;
            clicked = false;
        }

        private void addSnapshot()
        {
            ExampleRepository.Instance.AddExampleDomainObject(
                        ((CanvasService)DesignerKernel.Instance.GetService("gummy-canvas")).CanvasSize,
                        (DomainObject)m_visDom.DomainObject.Clone()
                    );
        }

        void paintEvent(object sender, System.Windows.Forms.PaintEventArgs e)
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
                g.FillRectangle(Brushes.Black, enumerator.Current.Value);
            }
        }
    }
}
