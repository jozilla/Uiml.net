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

        //Get all the example sizes where the designer specifies some things 
        public List<Size> GetExampleSizes()
        {
            List<Size> sizes = new List<Size>();
            sizes.AddRange(m_examples.Keys);
            return sizes;
        }
    }
}
