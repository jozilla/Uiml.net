using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    public partial class Arrow : UserControl
    {
        public Arrow()
        {
            InitializeComponent();
            Paint += new PaintEventHandler(onPaint);
            Refresh();
        }

        void onPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Transparent, 0, 0, Width, Height);
            PointF pnt1 = new PointF( 0.5f * (float)Width, 0  );
            PointF pnt2 = new PointF(0.5f * (float)Width, Height);
            PointF pnt3 = new PointF(0, 0.5f * (float)Height);
            PointF[] points = new PointF[] { pnt1, pnt2, pnt3};
            g.DrawPolygon(Pens.Black, points);
            g.FillPolygon(Brushes.Black, points);
            PointF pnta = new PointF(0.5f * (float)Width, 0.333f * (float)Height);
            PointF pntb = new PointF(0.5f * (float)Width, 0.666f * (float)Height);
            PointF pntc = new PointF((float)Width, 0.333f * (float)Height);
            PointF pntd = new PointF((float)Width, 0.666f * (float)Height);
            PointF[] pnts = new PointF[] {pnta,pntc,pntd,pntb};
            g.DrawPolygon(Pens.Black, pnts);
            g.FillPolygon(Brushes.Black, pnts);
        }
    }
}
