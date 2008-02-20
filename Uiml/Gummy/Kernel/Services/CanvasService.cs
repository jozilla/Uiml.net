using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Visual;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services
{
    public class CanvasService : Form, IService
    {
        DomainObjectCollection m_domainObjects = new DomainObjectCollection();          

        public CanvasService()
            : base()
        {
        }

        public void Init()
        {
            Text = "Canvas";
            Size = new Size(400, 400);
            AllowDrop = true;           
            DragDrop += new DragEventHandler(onDragDrop);
            DragEnter += new DragEventHandler(onDragEnter);
            BackColor = Color.DarkGray;
            Resize += new EventHandler(onResize);
            m_domainObjects.DomainObjectCollectionUpdated += new DomainObjectCollection.DomainObjectCollectionUpdatedHandler(onDomainObjectCollectionUpdated);
        }

        void onResize(object sender, EventArgs e)
        {
            //Update every domainobject to its new properties...
            DomainObjectCollection.Enumerator domEnum = m_domainObjects.GetEnumerator();
            while (domEnum.MoveNext())
            {
                domEnum.Current.UpdateToNewSize(Size);
            }
        }

        void onDomainObjectCollectionUpdated(object sender, DomainObjectCollectionEventArgs e)
        {
            switch (e.State)
            {
                case DomainObjectCollectionEventArgs.STATE.MOREADDED:
                case DomainObjectCollectionEventArgs.STATE.MOREREMOVED:
                    Controls.Clear();
                    for (int i = 0; i < m_domainObjects.Count; i++)
                    {
                        VisualDomainObject visDom = new VisualDomainObject(m_domainObjects[i]);
                        visDom.State = new ResizeVisualDomainObjectState();
                        Controls.Add(visDom);
                    }
                    break;
                case DomainObjectCollectionEventArgs.STATE.ONEADDED:
                    VisualDomainObject vDom = new VisualDomainObject(e.DomainObject);
                    vDom.State = new ResizeVisualDomainObjectState();                    
                    Controls.Add(vDom);
                    vDom.BringToFront();
                    break;
                case DomainObjectCollectionEventArgs.STATE.ONEREMOVED:
                    //FixMe: Implement removing domain objects
                    break;
            }
           
        }

        public bool Open()
        {
            this.Visible = true;
            return true;
        }

        public bool Close()
        {
            this.Visible = false;
            return true;
        }

        public string ServiceName
        {
            get
            {
                return "gummy-canvas";
            }
        }

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return this;
            }
        }       

        void onDragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("onDragEnter");
            e.Effect = DragDropEffects.Move;
        }

        void onDragDrop(object sender, DragEventArgs e)
        {
            //Fixme: isn't there a better way to visualize the drag and drop?
            DomainObject tmp = new DomainObject();            
            DomainObject dom = (DomainObject)e.Data.GetData(tmp.GetType());
            DomainObject domCloned = (DomainObject)dom.Clone();
            domCloned.Location = this.PointToClient(new Point(e.X, e.Y));
            domCloned.Identifier = DomainObjectFactory.Instance.AutoID();
            m_domainObjects.Add(domCloned);
            ExampleRepository.Instance.AddExampleDomainObject(Size, domCloned);
          
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CanvasService
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "CanvasService";
            this.Load += new System.EventHandler(this.CanvasService_Load);
            this.ResumeLayout(false);
        }

        private void CanvasService_Load(object sender, EventArgs e)
        {

        }

        //Deprecated -> may not be used anymore
        public List<DomainObject> DomainObjects
        {
            get
            {
                List<DomainObject> domainObjects = new List<DomainObject>();
                for (int i = 0; i < m_domainObjects.Count; i++)
                {
                    domainObjects.Add((DomainObject)m_domainObjects[i].Clone());
                }
                return domainObjects;
            }
            set
            {
                m_domainObjects.Clear();
                m_domainObjects.AddRange(value);
            }
        }

        public System.Windows.Forms.Control ServiceConfigurationControl 
        {
            get 
            {
                return null; // no configuration
            }
        }
    }
}
