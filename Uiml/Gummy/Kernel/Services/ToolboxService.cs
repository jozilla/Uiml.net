using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Uiml.Peers;
using Uiml.Rendering;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Visual;
using Uiml.Gummy.Serialize;

using System.Windows.Forms;
using System.Drawing;

namespace Uiml.Gummy.Kernel.Services
{
    public class ToolboxService : Form, IService
    {
        private List<DomainObject> m_domainObjects = new List<DomainObject>();

        public ToolboxService() : base()
        {
            Size = new Size(500, 500);
            BackColor = Color.DarkGray;
        }

        public void Init()
        {
            Text = "Toolbox";
            List<VisualDomainObject> visualDomainObjects = new List<VisualDomainObject>();
            Hashtable dclasses = ActiveSerializer.Instance.Serializer.Voc.DClasses;
            IDictionaryEnumerator enumerator = dclasses.GetEnumerator();

            Size size = new Size(100, 40);
            int x = 0;
            int y = 0;
            int counter = 1;
            int height = 40;
            
            while (enumerator.MoveNext())
            {
                DClass dclass = (DClass)enumerator.Entry.Value;
                if (ActiveSerializer.Instance.Serializer.Accept(dclass) && dclass.UsedInTag == "part")
                {
                    DomainObject domObject = DomainObjectFactory.Instance.Create(dclass);                    
                    VisualDomainObject visDomObject = new VisualDomainObject(domObject);
                    domObject.Size = size;
                    domObject.Location = new Point(x, y);
                    visualDomainObjects.Add(visDomObject);
                    Controls.Add(visDomObject);

                    if (counter % 2 == 0)
                    {
                        y += size.Height + 5;
                        height += size.Height + 5;
                        x = 0;
                    }
                    else
                    {
                        x += size.Width + 5;
                    }
                    counter++;
                }
            }
            int k = visualDomainObjects.Count;
            Size = new Size(size.Width * 2 + 15, height + size.Height);
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
                return "gummy-toolbox";
            }
        }
    }
}
