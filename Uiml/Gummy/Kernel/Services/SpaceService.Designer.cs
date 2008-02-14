namespace Uiml.Gummy.Kernel.Services
{
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
            this.graph1 = new Uiml.Gummy.Kernel.Services.Graph();
            this.SuspendLayout();
            // 
            // graph1
            // 
            this.graph1.Cursor = System.Windows.Forms.Cursors.Default;
            this.graph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph1.Location = new System.Drawing.Point(0, 0);
            this.graph1.Name = "graph1";
            this.graph1.Size = new System.Drawing.Size(516, 433);
            this.graph1.TabIndex = 0;
            // 
            // SpaceService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 433);
            this.Controls.Add(this.graph1);
            this.Name = "SpaceService";
            this.Text = "2D Example Space";
            this.ResumeLayout(false);

        }

        #endregion

        public Graph graph1;

    }
}

