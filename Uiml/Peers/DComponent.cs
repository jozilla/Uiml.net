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

	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/

using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Uiml.Peers
{
	/// <summary>
	/// This class represents a &lt;d-component&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-component (d-method)*&gt;
	/// &lt;!ATTLIST d-component
	///           id NMTOKEN #REQUIRED
	///           source CDATA #IMPLIED
	///           how (append|cascade|replace) "replace"
	///           export (hidden|optional|required) "optional"
	///           maps-to CDATA #IMPLIED
	///           location CDATA #IMPLIED&gt;
	/// </summary>
	public class DComponent : UimlAttributes, IUimlElement
	{
		protected ArrayList m_children = null;

		protected string m_mapsTo = "";
		protected Location m_location = null;		

		public DComponent()
		{}

		public DComponent(XmlNode n)
		{
			Process(n);
		}

        public virtual object Clone()
        {
            DComponent clone = new DComponent();
            clone.CopyAttributesFrom(this);
            clone.m_mapsTo = m_mapsTo;
            if(m_location != null)
            {
                clone.m_location = (Location)m_location.Clone();
            }

            if(m_children != null)
            {
                clone.m_children = new ArrayList();
                for(int i = 0; i < m_children.Count; i++)
                {
                    IUimlElement element = (IUimlElement)m_children[i];
                    clone.m_children.Add(element.Clone());
                }
            }

            return clone;
        }

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;
			
			base.ReadAttributes(n);
			XmlAttributeCollection attr = n.Attributes;
			if(attr.GetNamedItem(MAPS_TO) != null)
				MapsTo = attr.GetNamedItem(MAPS_TO).Value;

			if(attr.GetNamedItem(LOCATION) != null)
				Location = new Location(attr.GetNamedItem(LOCATION).Value);
			
			ProcessChildren(n.ChildNodes);
		}

		protected void ProcessChildren(XmlNodeList l)
		{
			IEnumerator enumChildren = l.GetEnumerator();

			while(enumChildren.MoveNext())
			{
				XmlNode c = (XmlNode) enumChildren.Current;
				switch(c.Name)
				{
					case DMethod.IAM:
						AddChild(new DMethod(c));
						break;					
				}
			}
		}

        public override XmlNode Serialize(XmlDocument doc)
        {
            //Construct the node
            XmlNode node = doc.CreateElement(IAM);

            //Add attributes
            List<XmlAttribute> attributes = CreateAttributes(doc);

            if (MapsTo.Length > 0) {
                XmlAttribute attr = doc.CreateAttribute(MAPS_TO);
                attr.Value = MapsTo;
                attributes.Add(attr);
            }
            if (Location != null) {
                XmlAttribute attr = doc.CreateAttribute(LOCATION);
                attr.Value = Location.Value;
                attributes.Add(attr);
            }
            
            foreach (XmlAttribute attr in attributes)
            {
                node.Attributes.Append(attr);
            }

            //Add children
            for (int i = 0; i < Children.Count; i++)
            {
                IUimlElement element = (IUimlElement)Children[i];
                node.AppendChild(element.Serialize(doc));
            }

            //Return the constructed node
            return node;
        }

		public bool HasChildren
		{
			get { return m_children != null; }
		}

		public void AddChild(object o)
		{
			if(m_children == null)
				m_children = new ArrayList();
			m_children.Add(o);
		}

		public IEnumerator GetEnumerator()
		{
			return m_children.GetEnumerator();
		}

		public ArrayList Search(Type t)
		{
			ArrayList l = new ArrayList();

			if(HasChildren)
			{
				IEnumerator e = GetEnumerator();

				while(e.MoveNext())
				{
					if(e.Current.GetType().Equals(t))
					{
						l.Add(e.Current);
					}
				}
			}

			return l; 						
		}

		public ArrayList Children
		{
			get { return m_children; }
		}

		public string MapsTo
		{
			get { return m_mapsTo;	}
			set { m_mapsTo = value;	}
		}

		public Location Location
		{
			get	{ return m_location; }
			set	{ m_location = value; }
		}

		public const string MAPS_TO         = "maps-to";
		public const string LOCATION		= "location";

		public const string IAM				= "d-component";
	}
}
