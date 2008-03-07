using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Interpolation
{
    public class Vec3d
    {
        public double X = 0.0d;
        public double Y = 0.0d;
        public double Z = 0.0d;

        public Vec3d()
        { }

        public Vec3d(double x, double y, double z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        //This - vec
        public Vec3d Minus(Vec3d vec)
        {
            return new Vec3d(X - vec.X, Y - vec.Y, Z - vec.Z);
        }

        //Crosproduct This x vec
        public Vec3d Cross(Vec3d vec)
        {
            Vec3d cross = new Vec3d();
            cross.X = (Y * vec.Z) - (Z * vec.Y);
            cross.Y = -((X * vec.Z) - (Z * vec.X));
            cross.Z = (X * vec.Y) - (Y * vec.X);

            return cross;
        }

        //Perpendicular A
        public Vec3d PerpendicularToNormalA()
        {
            Vec3d perpendicular = new Vec3d();
            perpendicular.X = 1.0d;
            perpendicular.Y = 0.0d;
            perpendicular.Z = -X / Z;
            return perpendicular;
        }

        //Perpendicular B
        public Vec3d PerpendicularToNormalB()
        {
            Vec3d perpendicular = new Vec3d();
            perpendicular.X = 0.0d;
            perpendicular.Y = 1.0d;
            perpendicular.Z = -Y / Z;
            return perpendicular;
        }

    }

    public class LinearInterpolationAlgorithm : InterpolationAlgorithm
    {
        ExamplePickingAlgorithm m_picking = null;

        public LinearInterpolationAlgorithm(DomainObject dom)
            : base(dom)
        {
            m_picking = new ExamplePickingAlgorithm(dom);
        }

        private double linearInterpolate(double x1, double y1, double x2, double y2, double x3)
        {
            if (x2 == x1)
            {
                //Exceptional case
                return y2;
            }
            else
            {
                //Regular case
                double a = (y2 - y1) / (x2 - x1);
                double b = y1 - a * x1;

                return x3 * a + b;
            }
        }

        
        public override void Update(System.Drawing.Size size)
        {
            //Get the two shortest examples
            Size[] shortestSizesHeight = ExampleRepository.Instance.GetShortestSizes(size, DomainObject, 2);
            Size[] shortestSizesWidth = ExampleRepository.Instance.GetShortestSizes(size, DomainObject, 2);

            if (shortestSizesHeight != null || shortestSizesWidth != null)
            {
                Dictionary<Size, DomainObject> examples = ExampleRepository.Instance.GetDomainObjectExamples(DomainObject.Identifier);

                Size consideredExampleH1 = shortestSizesHeight[0];
                Size consideredExampleH2 = shortestSizesHeight[1];
                Size consideredExampleW1 = shortestSizesWidth[0];
                Size consideredExampleW2 = shortestSizesWidth[1];

                double width = linearInterpolate((double)consideredExampleW1.Width, (double)examples[consideredExampleW1].Size.Width, (double)consideredExampleW2.Width, (double)examples[consideredExampleW2].Size.Width, (double)size.Width);
                if (width <= 0.0f)
                    width = 1.0f;
                double height = linearInterpolate((double)consideredExampleH1.Height, (double)examples[consideredExampleH1].Size.Height, (double)consideredExampleH2.Height, (double)examples[consideredExampleH2].Size.Height, (double)size.Height);
                if (height <= 0.0f)
                    height = 1.0f;
                double x = linearInterpolate((double)consideredExampleW1.Width, (double)examples[consideredExampleW1].Location.X, (double)consideredExampleW2.Width, (double)examples[consideredExampleW2].Location.X, (double)size.Width);
                if (x <= 0.0f)
                    x = 1.0f;
                double y = linearInterpolate((double)consideredExampleH1.Height, (double)examples[consideredExampleH1].Location.Y, (double)consideredExampleH2.Height, (double)examples[consideredExampleH2].Location.Y, (double)size.Height);
                if (y <= 0.0f)
                    y = 1.0f;

                DomainObject.Size = new Size(Convert.ToInt32(width), Convert.ToInt32(height));
                DomainObject.Location = new Point(Convert.ToInt32(x), Convert.ToInt32(y));

            }
            else
            {
                m_picking.Update(size);
            }
        }

    }
}
