using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services
{
    public partial class SpaceService : Form
    {
        CanvasService m_canvas = null;
        Dictionary<Size, List<DomainObject>> m_examples = new Dictionary<Size, List<DomainObject>>();
        bool m_customResize = false;

        public SpaceService()
        {
            InitializeComponent();                        
        }

        void m_canvas_Resize(object sender, EventArgs e)
        {
            if (!m_customResize)
            {
                graph1.FocussedSize = m_canvas.Size;
            }
        }

        void graph1_DesignSpaceExampleSelected(object sender, Size size)
        {
            m_customResize = true;
            m_canvas.Size = size;
            m_canvas.DomainObjects = m_examples[size];
            m_customResize = false;
        }

        void graph1_DesignSpaceCursorChanged(object sender, Size size)
        {
            m_customResize = true;
            m_canvas.Size = size;
            m_customResize = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            graph1.CreateSnapshot();
            m_examples.Add(graph1.FocussedSize, m_canvas.DomainObjects);
        }

        public void Init()
        {
            m_canvas = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");
            graph1.DesignSpaceCursorChanged += new DesignSpaceSizeChangeHandler(graph1_DesignSpaceCursorChanged);
            graph1.DesignSpaceExampleSelected += new DesignSpaceSizeChangeHandler(graph1_DesignSpaceExampleSelected);
        }

        public bool Open()
        {
            this.Visible = true;
            m_canvas.Size = graph1.FocussedSize;
            m_canvas.Resize += new EventHandler(m_canvas_Resize);
            return true;
        }

        public bool Close()
        {
            m_canvas.Close();
            this.Visible = false;
            return true;
        }

        public string ServiceName
        {
            get
            {
                return "gummy-designspace";
            }
        }

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return this;
            }
        }
    }
}