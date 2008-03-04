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

    public class CosineInterpolationAlgorithm : InterpolationAlgorithm
    {
        ExamplePickingAlgorithm m_picking = null;

        public CosineInterpolationAlgorithm(DomainObject dom)
            : base(dom)
        {
            m_picking = new ExamplePickingAlgorithm(dom);
        }

        private void oldUpdate(Size size)
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

        public double linearInterpolate(double x1, double y1, double x2, double y2, double x3)
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

        public double planarInterpolate(Vec3d A, Vec3d B, Vec3d C, double width, double height)
        {
            Vec3d AB = B.Minus(A);
            Vec3d AC = C.Minus(A);
            Vec3d N = AB.Cross(AC);

            //The coordinates of the plane _x*X + _y*Y + _z*Z = _A
            double _X = N.X;
            double _Y = N.Y;
            double _Z = N.Z;
            double _A = -((N.X * (-A.X)) + (N.Y * (-A.Y)) + (N.Z * (-A.Z)));

            double x = _X * width;
            double y = _Y * height;

            if (_Z == 0.0d)
                _Z = 0.1d;
            double z = (_A - x - y) / _Z;
            //Console.WriteLine("width for {0},{1} = {2}", size.Width, size.Height, z);
            if (z <= 0)
                z = 1.0d;

            return z;
        }

        public override void Update(System.Drawing.Size size)
        {
            oldUpdate(size);
            return;
            //Get the three shortest sizes
            Size[] shortestSizes = ExampleRepository.Instance.GetShortestSizes(size, DomainObject, 3);

            if (shortestSizes != null)
            {
                Dictionary<Size, DomainObject> examples = ExampleRepository.Instance.GetDomainObjectExamples(DomainObject.Identifier);
                double width = planarInterpolate(new Vec3d(shortestSizes[0].Width, shortestSizes[0].Height, examples[shortestSizes[0]].Size.Width), 
                    new Vec3d(shortestSizes[1].Width, shortestSizes[1].Height, examples[shortestSizes[1]].Size.Width), 
                     new Vec3d(shortestSizes[2].Width, shortestSizes[2].Height, examples[shortestSizes[2]].Size.Width),
                     (double)size.Width, (double)size.Height);
                double height = planarInterpolate(new Vec3d(shortestSizes[0].Width, shortestSizes[0].Height, examples[shortestSizes[0]].Size.Height), 
                    new Vec3d(shortestSizes[1].Width, shortestSizes[1].Height, examples[shortestSizes[1]].Size.Height), 
                     new Vec3d(shortestSizes[2].Width, shortestSizes[2].Height, examples[shortestSizes[2]].Size.Height),
                     (double)size.Width, (double)size.Height);
                if (width <= 0.0d)
                    width = 1.0d;
                if (height <= 0.0d)
                    height = 1.0d;
                DomainObject.Size = new Size(Convert.ToInt32(width), Convert.ToInt32(height));
                /*
                
                Vec3d A = new Vec3d(shortestSizes[0].Width, shortestSizes[0].Height, examples[shortestSizes[0]].Size.Width);
                Vec3d B = new Vec3d(shortestSizes[1].Width, shortestSizes[1].Height, examples[shortestSizes[1]].Size.Width);
                Vec3d C = new Vec3d(shortestSizes[2].Width, shortestSizes[2].Height, examples[shortestSizes[2]].Size.Width);

                Vec3d AB = B.Minus(A);
                Vec3d AC = C.Minus(A);
                Vec3d N = AB.Cross(AC);

                //The coordinates of the plane _x*X + _y*Y + _z*Z = _A
                double _X = N.X;
                double _Y = N.Y;
                double _Z = N.Z;
                double _A = -((N.X * (-A.X)) + (N.Y * (-A.Y)) + (N.Z * (-A.Z)));

                double x = _X * (double)size.Width;
                double y = _Y * (double)size.Height;
                if (_Z == 0.0d)
                    _Z = 0.1d;
                double z = planarInterpolate(A, B, C, (double)size.Width, (double)size.Height);                
                if (z <= 0)
                    z = 1.0d;
                DomainObject.Size = new Size(Convert.ToInt32(z), DomainObject.Size.Height);*/

                /*

                Vec3d a = N.PerpendicularToNormalA();
                Vec3d b = N.PerpendicularToNormalB();
                Vec3d c = new Vec3d(0.0d, 0.0d, _A / _Z);

                //The plane is parametrized : value = s*a + t*b + c
                double s = (double)size.Width / 1500.0d;
                double t = (double)size.Height / 1500.0d;
                Vec3d value = new Vec3d(s*a.X + t * b.X + c.X, s*a.Y + t * b.Y + c.Y, s * a.Z + t * b.Z + c.Z);                
                Console.WriteLine("Value on parameters s={0}, t= {1} is [{2},{3},{4}]",s,t,value.X,value.Y,value.Z);
                if (value.Z <= 0)
                    value.Z = 1.0d;
                DomainObject.Size = new Size(Convert.ToInt32(value.Z), DomainObject.Size.Height);*/
            }
            else
            {
                //oldUpdate(size);                
            }
        }
        /*
        public double linearInterpolate(double y1, double y2, double mu)
        {
            return (y1 * (1.0d-mu) + y2*mu );
        }*/

        public double cosineInterpolate(double y1, double y2, double mu)
        {
            double mu2 = (1 - Math.Cos(mu * Math.PI)) / 2.0d;
            return (y1 * (1.0d - mu2) + y2 * mu2);
        }

        public double distance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt( ( (x1 - x2) * (x1 - x2) ) + ( (y1 - y2) * (y1 - y2) ) );
            return distance;
        }

    }
}
