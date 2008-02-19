using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel.Services.Controls;

namespace Uiml.Gummy.Kernel.Services
{
    public partial class SpaceService : Form
    {
        CanvasService m_canvas = null;
        bool m_customResize = false;

        public SpaceService()
        {
            InitializeComponent();                        
        }

        void m_canvas_Resize(object sender, EventArgs e)
        {
            if (!m_customResize)
            {
                Size sizeToSet = new Size(m_canvas.Size.Width, m_canvas.Size.Height);
                if (m_canvas.Size.Width % graph1.XStep != 0)
                {
                    sizeToSet.Width = m_canvas.Size.Width - (m_canvas.Size.Width % graph1.XStep);
                }
                if (m_canvas.Size.Height % graph1.YStep != 0)
                {
                    sizeToSet.Height = m_canvas.Size.Height - (m_canvas.Size.Height % graph1.YStep);
                }
                graph1.FocussedSize = sizeToSet;
            }
        }

        void graph1_DesignSpaceCursorChanged(object sender, Size size)
        {
            m_customResize = true;
                m_canvas.Size = size;
            m_customResize = false;
            m_canvas.UpdateToNewSize();
        }


        public void Init()
        {
            m_canvas = (CanvasService)DesignerKernel.Instance.GetService("gummy-canvas");
            graph1.DesignSpaceCursorChanged += new DesignSpaceSizeChangeHandler(graph1_DesignSpaceCursorChanged);            
        }

        public bool Open()
        {
            this.Visible = true;
            graph1.InitGraph();            
            m_canvas.Resize += new EventHandler(m_canvas_Resize);
            m_canvas.Size = new Size(200,200);
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