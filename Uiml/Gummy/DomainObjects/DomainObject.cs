using System;
using System.IO;
using Uiml;
using Uiml.Gummy.Serialize;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Uiml.Gummy.Domain
{

	public class DomainObject : ICloneable
	{
        private List<Property> m_properties = new List<Property>();        
        private DomainObject m_parent = null;
        private Part m_part = null;

        private bool m_selected = false;

        private PositionManipulator m_positionManipulator = null;
        private SizeManipulator m_sizeManipulator = null;
        
        public delegate void DomainObjectUpdateHandler (object sender, EventArgs e);

        public event DomainObjectUpdateHandler DomainObjectUpdated;
        		
		public DomainObject()
		{
		}

        public object Clone()
        {
            DomainObject clone = new DomainObject();
            for (int i = 0; i < m_properties.Count; i++)
            {
                Property prop = m_properties[i];
                clone.m_properties.Add((Property)prop.Clone());
            }
            clone.m_part = (Part)m_part.Clone();
            clone.m_selected = m_selected;
            if (m_positionManipulator != null)
            {
                clone.m_positionManipulator = (PositionManipulator)m_positionManipulator.Clone();
                clone.m_positionManipulator.DomainObject = clone;
            }
            if (m_sizeManipulator != null)
            {
                clone.m_sizeManipulator = (SizeManipulator)m_sizeManipulator.Clone();
                clone.m_sizeManipulator.DomainObject = clone;
            }

            return clone;
        }

        public Part Part
        {
            get
            {
                return m_part;
            }
            set
            {
                m_part = value;
            }
        }

        public Size Size
        {
            get
            {
                return m_sizeManipulator.Size;
            }
            set
            {
                m_sizeManipulator.Size = value;
                Updated();
            }
        }

        public Point Location
        {
            get
            {
                return m_positionManipulator.Position;
            }
            set
            {
                m_positionManipulator.Position = value;
                Updated();
            }
        }

        public SizeManipulator SizeManipulator
        {
            get { return m_sizeManipulator;  }
            set { m_sizeManipulator = value; }
        }

        public PositionManipulator PositionManipulator
        {
            get { return m_positionManipulator; }
            set { m_positionManipulator = value;  }
        }

        public void AddAttribute(Property attribute)
        {
            m_properties.Add(attribute);
        }

        public List<Property> Properties
        {
            get
            {
                return m_properties;
            }
        }

        public DomainObject Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        public virtual DomainObject MoveNext()
        {
            return null;
        }

        public void Updated()
        {
            if(DomainObjectUpdated != null)
                DomainObjectUpdated(this, new EventArgs());
        }

        public Property FindProperty(string propertyName)
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                Property prop = Properties[i];
                if (prop.Name == propertyName)
                {
                    return prop;                    
                }
            }
            return null;
        }

        public bool Selected
        {
            get
            {
                return m_selected;
            }
            set
            {
                m_selected = value;
            }
        }
       
	}
}
