using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public partial class MethodParameterView : UserControl
    {
        private MethodParameterModel model;

        public MethodParameterModel Model
        {
            get { return model; }
            set { model = value; }
        }

        public MethodParameterView (MethodParameterModel model)
        {
            InitializeComponent();
            Model = model;
            Model.Updated += new EventHandler(ModelUpdated);
            param.MouseDown += new MouseEventHandler(OnMouseDown);
            Draw();
        }

        void ModelUpdated(object sender, EventArgs e)
        {
            if (Model.Linked)
                linkIcon.Image = global::gummy.Properties.Resources.linked;
            else
                linkIcon.Image = global::gummy.Properties.Resources.not_linked;
        }

        void OnMouseDown(object sender, MouseEventArgs e) 
        {
            param.DoDragDrop(model, DragDropEffects.Link);
        }

        protected void Draw ()
        {
            // set text and color
            if (Model.ParameterType == MethodParameterType.Output)
            {
                param.Text = Model.Type.ToString();
                param.BackColor = Color.LightCoral;
                param.ForeColor = Color.White;
            }
            else if (Model.ParameterType == MethodParameterType.Input)
            {
                param.Text = string.Format("{0} [{1}]", Model.Type, Model.Name);
                param.BackColor = Color.LightGreen;
                param.ForeColor = Color.Black;
            }
            else if (Model.ParameterType == MethodParameterType.Invoke)
            {
                param.Text = Model.Name;
                param.BackColor = Color.DarkOrange;
                param.ForeColor = Color.White;
                // bold
                param.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
        }

        private void linkIcon_DoubleClick(object sender, EventArgs e)
        {
            if (Model.Linked)
            {
                Model.Link.UnlinkMethodParameter(Model);
                Model.Link = null;
            }
        }

        private void linkIcon_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }

        private void param_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }

        private void layout_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }

        private void MethodParameterView_MouseClick(object sender, MouseEventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.UpdateSelectedMethod(Model.Parent);
        }
    }
}
