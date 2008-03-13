using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Drawing;

namespace Uiml.Gummy.Domain
{
    //A datatructure that contains all the example user interface designs that are created
    public class ExampleRepository
    {
        //Delegates
        public delegate void ExampleDesignAddedHandler(object sender, Size s);

        //Event handlers
        public event ExampleDesignAddedHandler ExampleDesignAdded;

        Dictionary<Size, Dictionary<string, DomainObject>> m_examples = new Dictionary<Size, Dictionary<string, DomainObject>>();

        static ExampleRepository m_repository = null;

        private ExampleRepository()
        { }

        public static ExampleRepository Instance
        {
            get
            {
                if (m_repository == null)
                    m_repository = new ExampleRepository();
                return m_repository;
            }
        }

        //Fire the example design added event
        private void fireExampleDesignAdded(Size s)
        {
            if (ExampleDesignAdded != null)
                ExampleDesignAdded(this, s);
        }

        //Add a domainobject that is changed on a certain size 'size'
        public void AddExampleDomainObject(Size size, DomainObject obj)
        {
            if (m_examples.ContainsKey(size))
            {
                if (m_examples[size].ContainsKey(obj.Identifier))
                {
                    m_examples[size].Remove(obj.Identifier);
                    m_examples[size].Add(obj.Identifier, obj);
                }
                else
                {
                    m_examples[size].Add(obj.Identifier, obj);
                }
            }
            else
            {
                Dictionary<string, DomainObject> m_nameDict = new Dictionary<string, DomainObject>();
                m_nameDict.Add(obj.Identifier, obj);
                m_examples.Add(size, m_nameDict);
                fireExampleDesignAdded(size);
            }
            PrintRepositoryContent();
        }

        //Get the various instances of the domain objects at certain sizes
        public Dictionary<Size, DomainObject> GetDomainObjectExamples(string label)
        {
            Dictionary<Size, DomainObject> list = new Dictionary<Size, DomainObject>();
            Dictionary<Size, Dictionary<string, DomainObject>>.Enumerator dictEnum = m_examples.GetEnumerator();
            while (dictEnum.MoveNext())
            {
                if (dictEnum.Current.Value.ContainsKey(label))
                {
                    list.Add(dictEnum.Current.Key, dictEnum.Current.Value[label]);
                }
            }
            return list;
        }

        public DomainObject GetDomainObjectExample(string label, Size size)
        {
            return GetDomainObjectExamples(label)[size];
        }

        //Get all the example sizes where the designer specifies some things 
        public List<Size> GetExampleSizes()
        {
            List<Size> sizes = new List<Size>();
            sizes.AddRange(m_examples.Keys);
            return sizes;
        }

        public Dictionary<string,DomainObject> GetExample(Size size)
        {
            if (m_examples.ContainsKey(size))
            {
                return m_examples[size];
            }
            return null;
        }

        public void PrintRepositoryContent()
        {
            //Console.Out.WriteLine("[RepositoryContent]");
            Dictionary<Size, Dictionary<string, DomainObject>>.Enumerator dictEnum = m_examples.GetEnumerator();
            while (dictEnum.MoveNext())
            {
                Dictionary<string,DomainObject>.Enumerator enumerator = dictEnum.Current.Value.GetEnumerator();
                //Console.Out.WriteLine("Examples given for size {0}",dictEnum.Current.Key);                
            }
            //Console.Out.WriteLine("[/RepositoryContent]");
        }

        public Size[] GetShortestSizes(Size size, DomainObject dom, int number_of_examples)
        {
            //Get all the examples of this domain object
            Dictionary<Size, DomainObject> examples = GetDomainObjectExamples(dom.Identifier);
            //Check if there are enough examples
            if (examples.Count < number_of_examples)
            {
                return null;
            }
            Dictionary<Size, DomainObject>.Enumerator enumerator = examples.GetEnumerator();

            //Sort the distances...
            SortedDictionary<double, List<Size>> distances = new SortedDictionary<double, List<Size>>();
            while (enumerator.MoveNext())
            {
                Size exampleSize = enumerator.Current.Key;
                
                double x1 = (double)size.Width;
                double y1 = (double)size.Height;
                double x2 = (double)exampleSize.Width;
                double y2 = (double)exampleSize.Height;

                double dist = distance(x1, y1, x2, y2);

                if (!distances.ContainsKey(dist))
                {
                    List<Size> sizeList = new List<Size>();
                    sizeList.Add(exampleSize);
                    distances.Add(dist, sizeList);
                }
                else
                {
                    distances[dist].Add(exampleSize);
                }
            }
            //Pick the right distances from the temporal datastructure
            Size[] shortest_examples = new Size[number_of_examples];

            SortedDictionary<double, List<Size>>.Enumerator sortedDictEnumerator = distances.GetEnumerator();
            int counter = 0;
            while (sortedDictEnumerator.MoveNext() && counter < number_of_examples)
            {
                List<Size> tmpSizes = sortedDictEnumerator.Current.Value;
                for (int i = 0; i < tmpSizes.Count && counter < number_of_examples; i++)
                {
                    shortest_examples[counter] = tmpSizes[i];
                    counter++;
                }                
            }

            return shortest_examples;
        }

        private double distance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
            return distance;
        }        

        public List<PointF> GetZones(DomainObject dom)
        {
            Dictionary<Size, DomainObject> examples = GetDomainObjectExamples(dom.Identifier);
            Dictionary<Size, DomainObject>.Enumerator enumerator = examples.GetEnumerator();
            List<PointF> zones = new List<PointF>();            
            if (examples != null && examples.Count > 2)
            {
                List<PointF> keyPoints = new List<PointF>();                
                while (enumerator.MoveNext())
                {
                    keyPoints.Add(new PointF((float)enumerator.Current.Key.Width,(float)enumerator.Current.Key.Height));                    
                }
                PointF old1 = keyPoints[0];
                PointF old2 = keyPoints[1];                
                for (int i = 2; i < keyPoints.Count; i++)
                {
                    zones.Add(intersection(old1, keyPoints[i], old2, keyPoints[i]));
                    old1 = old2;
                    old2 = keyPoints[i];
                }

                return zones;
            }
            else
                return zones;
        }

        private PointF intersection(PointF pnt1, PointF pnt2, PointF pnt3, PointF pnt4)
        {
            float ua_teller = (((pnt4.X - pnt3.X) * (pnt1.Y - pnt3.Y)) - ((pnt4.Y - pnt3.Y) * (pnt1.X - pnt3.X)));
            float ua_noemer = (((pnt4.Y - pnt3.Y) * (pnt2.X - pnt1.X)) - ((pnt4.X - pnt3.X) * (pnt2.Y - pnt1.Y)));
            float ua = ua_teller / ua_noemer;
            float ub_teller = ((pnt2.X - pnt1.X)* (pnt1.Y - pnt3.Y)) - ((pnt2.Y - pnt1.Y)*(pnt1.X - pnt3.X)); 
            float ub_noemer = ((pnt4.Y - pnt3.Y)* (pnt2.X - pnt1.X)) - ((pnt4.X - pnt3.X) * (pnt2.Y - pnt1.Y));
            float ub = ub_teller / ub_noemer;

            return new PointF(pnt1.X + (ua * (pnt2.X - pnt1.X)), pnt1.X + (ub * (pnt2.Y - pnt1.Y)));
        }
    }

    public class Zone
    {
        public List<PointF> Points = new List<PointF>();

        public Zone()
        {
        }
    }
}
