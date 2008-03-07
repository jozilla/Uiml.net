using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Shape
{
    public delegate void ShapUpdateHandler(IShape shape);

    public interface IShape
    {
        void Paint(Graphics g, Point offset);
        bool PointInShape(Point p);

        event ShapUpdateHandler ShapeUpdated;
    }
}
