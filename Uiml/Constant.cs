/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.uhasselt.be/kris/research/uiml.net)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@uhasselt.be)
	                     Expertise Centre for Digital Media (http://edm.uhasselt.be)
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

namespace Uiml
{

	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Generic;

	using Uiml.Rendering;

	///<summary>
	///Represents the constant element:
	///  &lt;!ELEMENT constant (constant*)&gt; 
	///  &lt;!ATTLIST constant id NMTOKEN #IMPLIED 
	///               source CDATA #IMPLIED 
	///               how (union|cascade|replace) "replace" 
	///               export (hidden|optional|required) "optional" 
	///               model CDATA #IMPLIED 
	///               value CDATA #IMPLIED&gt;
	///
	/// Valid model attributes are : list | tree
	///</summary>
	public class Constant : UimlAttributes, IUimlElement,	ICloneable{

		private string m_model = "";
		//Generics would be nice here :-)
		private Object m_data = null;
		private ArrayList m_children = new ArrayList();
		private Constant parent;

		public object Clone()
		{
			Constant clone = new Constant();
            clone.CopyAttributesFrom(this);

            /*if(m_data != null)
            {
                if(m_data is ICloneable)
        			clone.m_data = ((ICloneable)m_data).Clone();
                else
                    clone.m_data = m_data;
            }*/
            Console.WriteLine("Debug data = "+m_data);
            clone.m_data = m_data;
            clone.m_model = m_model;

			if(m_children != null)
			{                
				for(int i = 0; i<m_children.Count; i++){
				    Constant cons = (Constant)((Constant)m_children[i]).Clone();
                    cons.parent = this;
                    clone.Add(cons);
                }
			}
            clone.parent = parent;
			return clone;
		}

		public Constant()
		{
		}

		public Constant(XmlNode n) : this()
		{
			Process(n);

		}

		public void Process(XmlNode n)
		{
			if(n.Name == IAM)
			{
				base.ReadAttributes(n);
				XmlAttributeCollection attr = n.Attributes;
				if(attr.GetNamedItem(MODEL) != null) //it is a model == other sub constants are there!!
					m_model = attr.GetNamedItem(MODEL).Value;
				if(attr.GetNamedItem(VALUE) != null)
					Value = attr.GetNamedItem(VALUE).Value; //it is always a string
				if(n.HasChildNodes)
				{
					XmlNodeList xnl = n.ChildNodes;
					for(int i=0; i<xnl.Count; i++)
						Add(new Constant(xnl[i]));
				}
			}
		}

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(IAM);
            List<XmlAttribute> attributes = CreateAttributes(doc);
            if (m_model.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(MODEL);
                attr.Value = Model;
                attributes.Add(attr);
            }
            if (Value != null)
            {
                XmlAttribute attr = doc.CreateAttribute(VALUE);
                attr.Value = Value.ToString();
                attributes.Add(attr);
            }

            foreach (XmlAttribute attr in attributes)
            {
                node.Attributes.Append(attr);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                IUimlElement element = (IUimlElement)Children[i];
                node.AppendChild(element.Serialize(doc));
            }

            return node;
        }

        public void Add(Constant c)
		{
			if(Children == null)
				Children = new ArrayList();
			Children.Add(c);
		}

		public bool HasChildren
		{
			get
			{
                return Children.Count > 0;
			}			
		}

		public int ChildCount
		{
			get
			{
				return Children.Count;
			}			
		}

		public string Model
		{
			get { return m_model;  }
			set { m_model = value; }
		}

		public System.Object Value
		{
			get { return m_data; }
			set { m_data = value; }
		}


		public override String ToString()
		{
				String bufferedString;
				if(Value != null)
				  bufferedString = "[ Constant " + Model + "\"" + Value.ToString() + "\" ]";
				else
					bufferedString = "[ Unnamed Constant " + Model + " ]";
				
				if(Children != null)
				{
					bufferedString += " -> \n";
					IEnumerator enumValues = Children.GetEnumerator();
					while(enumValues.MoveNext())
						bufferedString += "{ " + ((Constant)enumValues.Current).ToString() + " }\n";
				}
				return bufferedString;
		}

		public ArrayList Children
		{
			get { return m_children;  }
			set { m_children = value; }
		}

		public const string IAM = "constant";
		public const string VALUE = "value";
		public const string MODEL = "model";
	
		public const string CONTENT = "content";

		//data models:
		public const string LIST = "list";
		public const string TREE = "tree";

	}
}
