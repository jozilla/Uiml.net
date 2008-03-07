namespace Uiml.Gummy.Kernel.Services
{
    using Uiml.Gummy.Kernel.Services.Controls;

    partial class SpaceService : IService
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_cartesianGraphControl = new Uiml.Gummy.Kernel.Services.Controls.CartesianGraph();
            this.SuspendLayout();
            // 
            // m_cartesianGraphControl
            // 
            this.m_cartesianGraphControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.m_cartesianGraphControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_cartesianGraphControl.Location = new System.Drawing.Point(0, 0);
            this.m_cartesianGraphControl.Mode = Uiml.Gummy.Kernel.Services.Controls.Mode.CURSOR;
            this.m_cartesianGraphControl.Name = "m_cartesianGraphControl";
            this.m_cartesianGraphControl.Size = new System.Drawing.Size(377, 314);
            this.m_cartesianGraphControl.TabIndex = 0;
            // 
            // SpaceService
            // 
            this.Controls.Add(this.m_cartesianGraphControl);
            this.Size = new System.Drawing.Size(377, 314);
            this.Text = "2D Example Space";
            this.ResumeLayout(false);

        }

        #endregion

        public Uiml.Gummy.Kernel.Services.Controls.CartesianGraph m_cartesianGraphControl;

    }
}

