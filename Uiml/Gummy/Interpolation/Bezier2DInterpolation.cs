using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Interpolation
{
    public class Bezier2DInterpolation : InterpolationAlgorithm
    {
        float m_a = 0.0f;
        float m_b = 0.0f;
        float m_c = 0.0f;
        float m_d = 0.0f;

        public Bezier2DInterpolation(DomainObject dom)
            : base(dom)
        {
        }

        private void calculateABCD(Dictionary<int,int> values)
        {
            //Dictionary<float,int>
            float y0 = 0.0f;
            float x0 = 0.0f;

            float y1 = 0.0f;
            float x1 = 0.0f;

            float y2 = 0.0f;
            float x2 = 0.0f;

            float y3 = 0.0f;
            float x3 = 0.0f;
            Dictionary<int, int>.Enumerator enumerator = values.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }
        }

        public override void Update(System.Drawing.Size size)
        {
            Dictionary<Size, DomainObject> domDict = ExampleRepository.Instance.GetDomainObjectExamples(DomainObject.Identifier);
            if (domDict.Count >= 4)
            {
                Dictionary<int, int> values = new Dictionary<int, int>();
                Dictionary<Size, DomainObject>.Enumerator enumerator = domDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    values.Add(enumerator.Current.Key.Width, enumerator.Current.Value.Size.Width);
                }
                calculateABCD(values);
            }
        }
    }
}
