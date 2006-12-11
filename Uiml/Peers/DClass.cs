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

namespace Uiml.Peers
{
	/// <summary>
	/// This class represents a &lt;d-class&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-class (d-method*, d-property*, event*, listener*)&gt;
	/// &lt;!ATTLIST d-class
	///	          id NMTOKEN #REQUIRED
	///	          source CDATA #IMPLIED
	///	          how (append|cascade|replace) "replace"
	///	          export (hidden|optional|required) "optional"
	///	          maps-to CDATA #REQUIRED
	///	          maps-type CDATA #REQUIRED
	///	          used-in-tag (event|listener|part) #REQUIRED&gt;
	/// </summary>
	public class DClass : UimlAttributes, IUimlElement
	{
		protected ArrayList m_children;

		protected string m_mapsTo;
		protected string m_mapsType;
		protected string m_usedInTag;

		public enum USED_IN_TAG_VALS { Event, Listener, Part };
		public const string EVENT = "event";
		public const string LISTENER = "listener";
		public const string PART = "part";

		public DClass()
		{
            m_children = new ArrayList();
        }

		public DClass(XmlNode n) : this()
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;

			base.ReadAttributes(n);
			XmlAttributeCollection attr = n.Attributes;
			if(attr.GetNamedItem(MAPS_TO) != null)
				MapsTo = attr.GetNamedItem(MAPS_TO).Value;

			if(attr.GetNamedItem(MAPS_TYPE) != null)
				MapsType = attr.GetNamedItem(MAPS_TYPE).Value;

			if(attr.GetNamedItem(USED_IN_TAG) != null)
				UsedInTag = attr.GetNamedItem(USED_IN_TAG).Value;
			
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
					case DProperty.IAM:
						AddChild(new DProperty(c));
						break;
					case Event.IAM:
						AddChild(new Event(c));
						break;
						// TODO: implemenent listener
					case "listener": 
						break;
				}
			}
		}

		public bool HasChildren
		{
			get { return m_children.Count > 0; }
		}

		public void AddChild(object o)
		{
			m_children.Add(o);
		}

		public IEnumerator GetEnumerator()
		{
			return m_children.GetEnumerator();
		}

		public ArrayList Search(Type t)
		{
			ArrayList l = new ArrayList();

            IEnumerator e = GetEnumerator();

            while(e.MoveNext())
            {
                if(e.Current.GetType().Equals(t))
                {
                    l.Add(e.Current);
                }
            }

			return l; 						
		}

		public ArrayList Children
		{
			get	{ return m_children; }
		}		

		public string MapsTo
		{
			get { return m_mapsTo; }
			set { m_mapsTo = value;	}
		}

		public string MapsType
		{
			get { return m_mapsType; }
			set { m_mapsType = value; }
		}

		public string UsedInTag
		{
			get { return m_usedInTag; }
			set { m_usedInTag = value; }
		}

		public const string MAPS_TO         = "maps-to";
		public const string MAPS_TYPE		= "maps-type";
		public const string USED_IN_TAG		= "used-in-tag";
		
		public const string IAM				= "d-class";
	}
}
