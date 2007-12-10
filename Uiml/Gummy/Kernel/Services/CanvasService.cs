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
            DragLeave += new EventHandler(onDragLeave);
            DragOver += new DragEventHandler(onDragOver);            
        }

        void onDragLeave(object sender, EventArgs e)
        {
            Console.WriteLine("onDragLeave");
        }

        void onDragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine("onDragOver");
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

            VisualDomainObject visDom = new VisualDomainObject(domCloned);
            visDom.State = new ResizeVisualDomainObjectState();
            Controls.Add(visDom);
            
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

    }
}
