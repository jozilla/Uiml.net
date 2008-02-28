using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Interpolation
{
    public class CosineInterpolationAlgorithm : InterpolationAlgorithm
    {
        ExamplePickingAlgorithm m_picking = null;

        public CosineInterpolationAlgorithm(DomainObject dom)
            : base(dom)
        {
            m_picking = new ExamplePickingAlgorithm(dom);
        }

        public override void Update(System.Drawing.Size size)
        {
            //Get the 2 nearest examples of this domainobject
            Dictionary<Size,DomainObject> examples = ExampleRepository.Instance.GetDomainObjectExamples(DomainObject.Identifier);
            Dictionary<Size, DomainObject>.Enumerator enumerator = examples.GetEnumerator();
            SortedDictionary<double, Size> distances = new SortedDictionary<double, Size>();

            while (enumerator.MoveNext())
            {
                Size exampleSize = enumerator.Current.Key;
                //if (exampleSize.Width != size.Width && exampleSize.Height != size.Height)
                //{
                double x1 = (double)size.Width;
                double y1 = (double)size.Height;
                double x2 = (double)exampleSize.Width;
                double y2 = (double)exampleSize.Height;

                double dist = distance(x1, y1, x2, y2);

                if (!distances.ContainsKey(dist))
                {
                    distances.Add(dist, exampleSize);
                }
                //}
            }

            if (examples.Count >= 2)
            {
                SortedDictionary<double, Size>.Enumerator distEnumerator = distances.GetEnumerator();
                int counter = 0;
                Size consideredExample1 = Size.Empty;
                Size consideredExample2 = Size.Empty;
                //Debug
                //Console.Out.WriteLine("The shortest calculated distances from " + size + ":");
                while (distEnumerator.MoveNext())
                {
                    //DEBUG
                    //Console.Out.WriteLine("distance ("+counter+") for point "+distEnumerator.Current.Value+" is "+distEnumerator.Current.Key);
                    if (counter == 0)
                        consideredExample1 = distEnumerator.Current.Value;
                    else if (counter == 1)
                        consideredExample2 = distEnumerator.Current.Value;
                    else
                        break;
                    counter++;
                }
                if (consideredExample2 == Size.Empty)
                    consideredExample2 = consideredExample1;

                //We only consider componentwidth and totalwidth for now
                double minWidth = Math.Min((double)consideredExample1.Width, (double)consideredExample2.Width);
                double maxWidth = Math.Max((double)consideredExample1.Width, (double)consideredExample2.Width);
                Size tmp;
                if(minWidth != consideredExample1.Width)
                {
                    tmp = consideredExample1;
                    consideredExample1 = consideredExample2;
                    consideredExample2 = tmp;
                }

                double m1 = (double)examples[consideredExample1].Size.Width / (double)consideredExample1.Width;
                double m2 = (double)examples[consideredExample2].Size.Width / (double)consideredExample2.Width;

                double delta = Math.Abs((double)consideredExample1.Width - (double)consideredExample2.Width);
                double m = ((double)size.Width - (double)minWidth)/ delta;
                double m3 = linearInterpolate(m1, m2, m);
                int width = Convert.ToInt32(m3 * (double)size.Width );
                if (width == 0)
                    width = 1;

                //DomainObject sizeDom = ExampleRepository.Instance.GetDomainObjectExamples(DomainObject.Identifier)[size];
                //DomainObject.CopyUIMLFrom(sizeDom);
                DomainObject.Size = new Size(width,DomainObject.Size.Height);
                Console.Out.WriteLine("Calculating point for mu = {0}",m);
                //double munknown = 
            }
            else
            {
                m_picking.Update(size);
            }
        }

        public double linearInterpolate(double y1, double y2, double mu)
        {
            return (y1 * (1.0d-mu) + y2*mu );
        }

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
