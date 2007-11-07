/*
  	 Uiml.Net: a Uiml.Net renderer (http://lumumba.uhasselt.be/kris/research/uiml.net/)
   
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

namespace Uiml.Executing
{
	using Uiml;

	using Uiml.Rendering;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	
	public class Param : IExecutable, IUimlElement
	{
		private System.Object m_value;
		private string m_identifier;
		private string m_name;
		private string m_type;
		private bool m_lazy;
		private Part m_partTree;
		private System.Object m_subprop;


		public Param()
		{
		}

		public Param(XmlNode xmlNode, Part topPart) : this()
		{
			m_partTree = topPart;
			Process(xmlNode);
		}

        public virtual object Clone()
        {
            Param param = new Param();
            
            param.m_identifier = m_identifier;
            param.m_name = m_name;
            param.m_type = m_type;
            param.m_lazy = m_lazy;

            if(m_value != null)
            {
                if(m_value is IUimlElement)
                {
                    param.m_value = ((IUimlElement)m_value).Clone();
                }
                else
                {
                    param.m_value = m_value;
                }
            }
            if(m_subprop != null)
            {
                //FIXME: Whats this ?
            }

            param.PartTree = m_partTree;

            return param;

        }

		public void Process(XmlNode n)
		{
			if(n.Name == IAM)
			{
				m_value = n.InnerText;
			   XmlAttributeCollection attr = n.Attributes;
				if(attr.GetNamedItem(NAME) != null)
					Identifier = attr.GetNamedItem(NAME).Value;
				if(attr.GetNamedItem(TYPE) != null)
					Type = attr.GetNamedItem(TYPE).Value;

				if(n.HasChildNodes)	//is this property "set" by a sub-property?
				{
					if(n.ChildNodes[0].NodeType == XmlNodeType.Text)
						m_value = n.InnerXml;
					else
					{
						XmlNodeList xnl = n.ChildNodes;
						switch(xnl[0].Name)
						{
							case CALL:
								m_value = new Call(xnl[0], m_partTree);
								Lazy = true;
								break;
							case PROPERTY:
								m_value = new Uiml.Property(xnl[0]);
								Lazy = true;
								break;
						}
					}
				}
			}
			
		}

		public void AttachLogic(ArrayList logicDocs)
		{
			if(m_value is Call)
				((Call)m_value).AttachLogic(logicDocs);
			//should Property be supported here. I don't think so....
		}


		public System.Object Execute()
		{
			return null;
		}

		public System.Object Execute(IRenderer renderer)
		{
			return Value(renderer);
		}


		public System.Object Value()
		{
			if(Lazy)
				throw new ValueNotEvaluatedException("This value has not been resolved yet: try Value(IPropertySetter)");
			else
				return m_value;
		}
		
		public System.Object Value(IRenderer renderer)
		{		
			if(Lazy)
			{
				if(m_value is Call)
					return ((Call)m_value).Execute(renderer).ToString();
				if(m_value is Uiml.Property)
				{
					Property prop = (Property)m_value;
					if(prop.Lazy)
						prop.Resolve(renderer);
					return prop.GetCurrentValue(renderer.PropertySetter, m_partTree).ToString();
				}
			}
				
			return (string)m_value;
		}

		public bool Lazy
		{
			get { return m_lazy;  }
			set { m_lazy = value; }
		}

		public string Identifier
		{
			get { return m_identifier;  }
			set { m_identifier = value; }
		}

		public string Name
		{
			get { return m_name;  }
			set { m_name = value; }
		}

		public string Type
		{
			get { return m_type;  }
			set { m_type = value; }
		}

		public ArrayList Children
		{
			get 
			{
				ArrayList al = new ArrayList();
				al.Add(m_value);
				return al; 
			}
		}

        public Part PartTree
        {
            get
            {
                return m_partTree;
            }
            set
            {
                m_partTree = value;
                if(m_value is Call)
                    ((Call)m_value).PartTree = value;
            }
        }


		public const string PARAM    = "param";
		public const string NAME     = "name";
		public const string TYPE     = "type";
		public const string IAM      = "param";
		public const string CALL     = "call";
		public const string PROPERTY = "property";
	}

}
