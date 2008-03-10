using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public partial class MethodNameView : UserControl
    {
        private MethodModel model;

        public MethodModel Model
        {
            get { return model; }
            set { model = value; }
        }

        public MethodNameView(MethodModel model)
        {
            InitializeComponent();
            Model = model;
            Draw();
            name.MouseDown += new MouseEventHandler(OnMouseDown);
        }

        void OnMouseDown(object sender, MouseEventArgs e) 
        {
            name.DoDragDrop(model, DragDropEffects.Link);
        }

        protected void Draw ()
        {
            // set text
            name.Text = Model.Name;
        }
    }
}
