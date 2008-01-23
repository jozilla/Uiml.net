/*
    Uiml.Net: a .Net UIML renderer (http://research.edm.uhasselt.be/kris/research/uiml.net)

	 Copyright (C) 2005  Kris Luyten (kris.luyten@uhasselt.be)
	                     Expertise Centre for Digital Media (http://www.edm.uhasselt.be)
								Hasselt University

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

namespace Uiml{

	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Generic;
	using System.IO;

	using Uiml.Executing;
	using Uiml.Rendering;

	///<summary>
	/// Implementation for the property element
	/// &lt;!ELEMENT property (#PCDATA| constant| property| reference| call| op| event| iterator)*&gt;
	/// &lt;!ATTLIST property name NMTOKEN #IMPLIED 
	///              source CDATA #IMPLIED 
	///              how (union|cascade|replace) "replace" 
	///              export (hidden|optional|required) "optional" 
	///              part-name NMTOKEN #IMPLIED 
	///              part-class NMTOKEN #IMPLIED 
	///              event-name NMTOKEN #IMPLIED 
	///              event-class NMTOKEN #IMPLIED 
	///              call-name NMTOKEN #IMPLIED 
	///              call-class NMTOKEN #IMPLIED&gt;
	///</summary>
	public class Property : UimlAttributes, IUimlElement, ICloneable {

		// Properties
		private string m_name = null;
		private System.Object m_value = null;

		//member vars
		private bool m_setter = true;
		private bool m_hasToBeSet;
		private string m_class_name = "";
		private string m_part_name  = "";
		private System.Object m_subprop = null;

		//To resolve conflicting
		//properties the last property of the canonical form has to be applied over the rest.
		//This number will take care of counting which Property was created before/after another.
		private static int counter = 0;
		private int m_nr = -1;

		public object Clone()
		{
			Property prop = new Property();
            prop.CopyAttributesFrom(this);

            prop.m_name = m_name;
			prop.m_setter = m_setter;
			prop.m_hasToBeSet = m_hasToBeSet;
			prop.m_class_name = m_class_name;
			prop.m_part_name = m_part_name;

            if(m_subprop != null)
			prop.m_subprop = ((IUimlElement)m_subprop).Clone();

            prop.m_value = m_value;
			//if(m_subprop != null  && m_subprop is Constant)
			//	prop.SubProp = ((Constant)m_subprop).Clone();
			//else
			//	prop.SubProp = SubProp;

			return prop;
		}

		public Property()
		{
			m_nr = counter++;
		}

		public Property(string partName, string name, string val) : this()
		{
			m_part_name = partName;
			m_name = name;
			m_value = val;
		}

        public Property(string name, string val): this("", name, val) 
        { }

		public Property(XmlNode n) : this()
		{
			Process(n);
		}

		public Property(XmlNode n, bool setter) : this(n)
		{
			m_setter = setter;
		}


		///<summary>
		/// Processes a property node.
		///</summary>
		public void Process(XmlNode n)
		{
			if(n.Name != PROPERTY)
				return;

			base.ReadAttributes(n);
				
			XmlAttributeCollection attr = n.Attributes;
			if(attr.GetNamedItem(NAME) != null)
				 Name = attr.GetNamedItem(NAME).Value;

			if(attr.GetNamedItem(PART_NAME) != null)
				 m_part_name = attr.GetNamedItem(PART_NAME).Value;

			if(attr.GetNamedItem(PART_CLASS) != null)
				m_class_name = attr.GetNamedItem(PART_CLASS).Value;

	
			if(n.HasChildNodes)	//is this property "set" by a sub-property?
			{
				if(n.ChildNodes[0].NodeType == XmlNodeType.Text)
					Value = n.InnerXml;					
				else
				{
					m_setter = false;
					XmlNodeList xnl = n.ChildNodes;
					for(int i=0; i<xnl.Count; i++)
					{
						switch(xnl[i].Name)
						{
							case PROPERTY:
								m_subprop = new Property(xnl[i],true);
								Lazy = true;
								break;
							case CONSTANT:
								m_subprop = new Constant(xnl[i]);
								Lazy = true;
								break;						
							case REFERENCE:
								m_subprop = new Reference(xnl[i]);
								break;
							case CALL:
								Lazy = true;
								m_subprop = new Call(xnl[i]);
								break;
						}
					}
				}
			}
			else//this property has a direct value
				Value = n.InnerText;
		}

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(IAM);
            //Construct attributes
            List<XmlAttribute> attributes = CreateAttributes(doc);
            if (Name != null)
            {
                XmlAttribute attr = doc.CreateAttribute(NAME);
                attr.Value = Name;
                attributes.Add(attr);
            }
            if (PartClass.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(PART_CLASS);
                attr.Value = PartClass;
                attributes.Add(attr);
            }
            if (PartName.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(PART_NAME);
                attr.Value = PartName;
                attributes.Add(attr);
            }

            foreach (XmlAttribute attr in attributes)
            {
                node.Attributes.Append(attr);
            }

            //Add children
            if (m_subprop != null)
                node.AppendChild(((IUimlElement)m_subprop).Serialize(doc));
            else if (m_value != null)
                node.InnerText = m_value.ToString();

            return node;
        }

		public string Name
		{
			get { return m_name;  }
			set {	m_name = value; }
		}

		public virtual System.Object Value
		{
			get {	return m_value;  }
			set {	m_value = value; }
		}

		public string PartName
		{
			get { return m_part_name;  }
			set {	m_part_name = value;	}
		}

		public string PartClass
		{
			get { return m_class_name;  }
			set {	m_class_name = value; }
		}

		public bool Setter
		{
			get { return m_setter;  }
			set {	m_setter = value;	}
		}

		public bool Lazy
		{
			get { return m_hasToBeSet;  }
			set { m_hasToBeSet = value; }
		}

		///<summary>
		///Gets the <b>current</b> value of this property
		///</summary>
		public virtual System.Object GetCurrentValue(IPropertySetter iPropSetter, Part p)
		{
			return iPropSetter.GetValue(p, this);
		}

		///<summary>
		///A property can connect to external objects if it is set by a call:
		/// &lt;property&gt;&lt;call&gt;...&lt;/call&gt;&lt;/property&gt;
		///</summary>
		public void Connect(object o)
		{
			if(m_subprop is Uiml.Executing.Call)
				((Uiml.Executing.Call)m_subprop).Connect(o);
		}

		public void Disconnect(object o)
		{
			if(m_subprop is Uiml.Executing.Call)
				((Uiml.Executing.Call)m_subprop).Disconnect(o);
		}

		///<summary>
		/// Resolves the sub properties to get the value for this property
		///</summary>
		public void Resolve(IRenderer renderer)
		{
			try
			{
				//the property has to be executed:
				if(m_subprop is Uiml.Executing.IExecutable)
                    // don't convert to a string, because we might need
                    // complex types coming from the app logic!
					Value = ((IExecutable)m_subprop).Execute(renderer);//.ToString();
				//the property is a constant and has to be converted into the correct data structure:
				else if(m_subprop is Uiml.Constant)//TODO: check the following line for consistent execution
					Value = ((Constant)m_subprop); 
				//the property has to get its value from another property:
				else
					Value = ((Property)m_subprop).GetCurrentValue(renderer.PropertySetter, null);
			}
				catch(Exception e)
				{
					Console.WriteLine("Could not resolve property {0}", Name);
					Console.WriteLine("Caught exception {0}", e);
				}
			
		}

		public void AttachLogic(ArrayList logicDocs)
		{
			if(m_subprop is Uiml.Executing.Call)
				((Uiml.Executing.Call)m_subprop).AttachLogic(logicDocs);
			else if(m_subprop is Uiml.Property)
				((Uiml.Property)m_subprop).AttachLogic(logicDocs);

		}

		public override String ToString()
		{
			return "[Property]:" + Name + " for " + PartName  + "="  + Value;
		}

		public ArrayList Children
		{
			get { return null; }
		}

		public virtual bool IsVirtual
		{
			get { return false; }
		}
		public const string IAM         = "property";
		public const string NAME        = "name";
		public const string PART_NAME   = "part-name";
		public const string PART_CLASS  = "part-class";
		public const string EVENT_NAME  = "event-name";
		public const string EVENT_CLASS = "event-class";
		public const string CALL_NAME   = "call-name";
		public const string CALL_CLASS  = "call-class";
		public const string CALL        = "call";
		public const string PROPERTY    = "property";
		public const string CONSTANT    = "constant";
		public const string REFERENCE   = "reference";
		//private enum m_how  { Union, Cascade, Replace };
		//private enum m_export { Hidden, Optional, Required }; 
	}
}
