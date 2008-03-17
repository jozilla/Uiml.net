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
using Uiml.Peers;

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

        private MethodParameterDomainObjectInvokeBinding m_invokeBinding = null;
        private List<MethodParameterDomainObjectOutputBinding> m_outputBindings = new List<MethodParameterDomainObjectOutputBinding>();
        private List<MethodParameterDomainObjectInputBinding> m_inputBindings = new List<MethodParameterDomainObjectInputBinding>();

        private DomainObjectGroup m_domObjectGroup = null;
              
		public DomainObject()
		{
            //The default interpolation algorithm
            m_interpolationAlgorithm = new LinearInterpolationAlgorithm(this);
		}

        protected void copyInto(DomainObject dom)
        {
            for (int i = 0; i < dom.m_properties.Count; i++)
            {
                Property prop = dom.m_properties[i];
                m_properties.Add((Property)prop.Clone());
            }
            if (dom.m_part != null)
                m_part = (Part)dom.m_part.Clone();
            m_selected = dom.m_selected;
            m_color = dom.m_color;
            if (dom.m_positionManipulator != null)
            {
                m_positionManipulator = (PositionManipulator)dom.m_positionManipulator.Clone();
                m_positionManipulator.DomainObject = this;
            }
            if (dom.m_sizeManipulator != null)
            {
                m_sizeManipulator = (SizeManipulator)dom.m_sizeManipulator.Clone();
                m_sizeManipulator.DomainObject = this;
            }
            if (dom.m_shape != null)
                m_shape = (Polygon)dom.m_shape.Clone();

        }

        public virtual object Clone()
        {
            DomainObject clone = new DomainObject();
            clone.copyInto(this);
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

        public MethodParameterDomainObjectInvokeBinding InvokeBinding 
        {
            get { return m_invokeBinding; }
        }

        public List<MethodParameterDomainObjectOutputBinding> OutputBindings 
        {
            get { return m_outputBindings; } 
        }

        public List<MethodParameterDomainObjectInputBinding> InputBindings
        {
            get { return m_inputBindings; }
        }

        public bool Bound 
        {
            get { return m_inputBindings.Count > 0 || m_outputBindings.Count > 0 || m_invokeBinding != null; }
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

        public virtual Size Size
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

        public virtual Polygon Polygon
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

        public virtual Point Location
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

        public virtual Rectangle Bounds
        {
            get
            {
                Rectangle rect = new Rectangle(Location, Size);
                return rect;
            }
            set
            {
                Size = value.Size;
                Location = value.Location;
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

        public void BindMethodParameter(MethodParameterModel mpm)
        {
            if (mpm.ParameterType == MethodParameterType.Output)
            {
                Property p = this.FindProperty("text");
                MethodParameterDomainObjectOutputBinding binding = new MethodParameterDomainObjectOutputBinding(mpm, this, p);
                m_outputBindings.Add(binding);
                mpm.Binding = binding;
                DesignerKernel.Instance.CurrentDocument.Methods.AddMethod(mpm.Parent);
            }
            else if (mpm.ParameterType == MethodParameterType.Input)
            {
                Property p = this.FindProperty("text");
                MethodParameterDomainObjectInputBinding binding = new MethodParameterDomainObjectInputBinding(mpm, this, p);
                m_inputBindings.Add(binding);
                mpm.Binding = binding;
                DesignerKernel.Instance.CurrentDocument.Methods.AddMethod(mpm.Parent);
            }
            else if (mpm.ParameterType == MethodParameterType.Invoke)
            {
                m_invokeBinding = new MethodParameterDomainObjectInvokeBinding(mpm, this, "ButtonPressed");
                mpm.Binding = m_invokeBinding;
                DesignerKernel.Instance.CurrentDocument.Methods.AddMethod(mpm.Parent);
            }

            Updated();
        }

        public void BreakBinding(MethodParameterDomainObjectBinding binding) 
        {
            if (binding is MethodParameterDomainObjectInputBinding)
                m_inputBindings.Remove((MethodParameterDomainObjectInputBinding)binding);
            else if (binding is MethodParameterDomainObjectOutputBinding)
                m_outputBindings.Remove((MethodParameterDomainObjectOutputBinding)binding);
            else if (binding is MethodParameterDomainObjectInvokeBinding)
                m_invokeBinding = null;
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

        public DomainObjectGroup DomainObjectGroup
        {
            get
            {
                return m_domObjectGroup;
            }
            set
            {
                m_domObjectGroup = value;
            }
        }

        public static Color DEFAULT_COLOR = Color.Black;
        public static Color SELECTED_COLOR = Color.Blue;
        public static Color UNSELECTED_COLOR = Color.LightGoldenrodYellow;
       
	}
}
