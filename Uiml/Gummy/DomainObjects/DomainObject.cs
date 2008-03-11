using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel;
using Uiml.Gummy.Serialize;
using Uiml;
using Uiml.Gummy.Interpolation;
using Shape;
using Uiml.Gummy.Kernel.Services.ApplicationGlue;

namespace Uiml.Gummy.Domain
{

	public class DomainObject : ICloneable
	{
        private List<Property> m_properties = new List<Property>();        
        private DomainObject m_parent = null;
        private Part m_part = null;

        private bool m_selected = false;

        private InterpolationAlgorithm m_interpolationAlgorithm = null;

        private PositionManipulator m_positionManipulator = null;
        private SizeManipulator m_sizeManipulator = null;

        private Color m_color = DEFAULT_COLOR;

        public delegate void DomainObjectUpdateHandler (object sender, EventArgs e);

        public event DomainObjectUpdateHandler DomainObjectUpdated;

        private Polygon m_shape = new Polygon();

        private MethodModel m_methodLink = null;
        private List<MethodParameterModel> m_methodOutParamLinks = new List<MethodParameterModel>();
        private List<MethodParameterModel> m_methodInParamLinks = new List<MethodParameterModel>();
        		
		public DomainObject()
		{
            //The default interpolation algorithm
            m_interpolationAlgorithm = new LinearInterpolationAlgorithm(this);
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
            clone.m_color = m_color;
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
            //Tmp
            clone.m_shape = m_shape;

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

        public MethodModel MethodLink 
        {
            get { return m_methodLink; }
        }

        public List<MethodParameterModel> MethodOutputParameterLinks 
        {
            get { return m_methodOutParamLinks; } 
        }

        public List<MethodParameterModel> MethodInputParameterLinks
        {
            get { return m_methodInParamLinks; }
        }

        public bool Linked 
        {
            get { return m_methodInParamLinks.Count > 0 || m_methodOutParamLinks.Count > 0 || m_methodLink != null; }
        }

        public String Identifier
        {
            get
            {
                if(m_part != null)
                    return m_part.Identifier;
                return "none";
            }
            set
            {
                m_part.Identifier = value;
                for (int i = 0; i < m_properties.Count; i++)
                {
                    m_properties[i].PartName = value;
                }
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

        public Polygon Polygon
        {
            get
            {
                return m_shape;
            }
            set
            {
                m_shape = value;
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

        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
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
            set
            {
                m_properties = value;
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

        public void LinkMethodParameter(MethodParameterModel mpm)
        {
            if (mpm.IsOutput)
            {
                m_methodOutParamLinks.Add(mpm);
                DesignerKernel.Instance.CurrentDocument.Behavior.RegisterOutput(mpm, this);
            }
            else
            {
                m_methodInParamLinks.Add(mpm);
                DesignerKernel.Instance.CurrentDocument.Behavior.RegisterInput(mpm, this);
            }

            Updated();
        }

        public void LinkMethod(MethodModel mm) 
        {
            m_methodLink = mm;
            DesignerKernel.Instance.CurrentDocument.Behavior.RegisterInvoke(mm, this);
            Updated();
        }

        public void UnlinkMethod() 
        {
            m_methodLink = null;
            Updated();
        }

        public void UnlinkMethodParameter(MethodParameterModel mpm) 
        {
            if (mpm.IsOutput)
                m_methodOutParamLinks.Remove(mpm);
            else
                m_methodInParamLinks.Remove(mpm);

            Updated();
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

        public InterpolationAlgorithm InterpolationAlgorithm
        {
            get
            {
                return m_interpolationAlgorithm;
            }
            set
            {
                m_interpolationAlgorithm = value;
            }
        }

        //Copies the properties and part of the parameter domain object to this domain object
        public void CopyUIMLFrom(DomainObject dom)
        {
            string id = Identifier;
            Properties.Clear();
            for (int i = 0; i < dom.Properties.Count; i++)
            {
                Property prop = (Property)dom.Properties[i].Clone();
                prop.PartName = id;
                Properties.Add(prop);
            }

            Part = (Part)dom.Part.Clone();
            Part.Identifier = id;
        }

        public void UpdateToNewSize(Size size)
        {
            Console.Out.WriteLine("Update [{0}] to the new size [{1}]",Identifier,size);
            m_interpolationAlgorithm.Update(size);
        }

        public static Color DEFAULT_COLOR = Color.Black;
        public static Color SELECTED_COLOR = Color.Blue;
        public static Color UNSELECTED_COLOR = Color.LightGoldenrodYellow;
       
	}
}
