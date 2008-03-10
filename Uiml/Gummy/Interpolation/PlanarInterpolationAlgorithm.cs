using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Interpolation
{
    public class PlanarInterpolationAlgorithm : InterpolationAlgorithm
    {
        public PlanarInterpolationAlgorithm(DomainObject dom)
            : base(dom)
        {
        }

        public override void Update(System.Drawing.Size size)
        {
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
              
            }
        }

        private double planarInterpolate(Vec3d A, Vec3d B, Vec3d C, double width, double height)
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
            if (z <= 0)
                z = 1.0d;

            return z;
        }


    }
}
