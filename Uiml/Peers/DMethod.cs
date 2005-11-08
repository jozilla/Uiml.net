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
	/// This class represents a &lt;d-method&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-method (d-param*, script?)&gt;
	/// &lt;!ATTLIST d-method
	///           id NMTOKEN #REQUIRED
	///           source CDATA #IMPLIED
	///           how (append|cascade|replace) "replace"
	///           export (hidden|optional|required) "optional"
	///           maps-to CDATA #REQUIRED
	///           return-type CDATA #IMPLIED&gt;
	/// </summary>
	public class DMethod : UimlAttributes, IUimlElement
	{
		protected ArrayList m_children = null;

		protected string m_mapsTo;
		protected string m_returnType;

		public DMethod()
		{}

		public DMethod(XmlNode n)
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

			if(attr.GetNamedItem(RETURN_TYPE) != null)
				ReturnType = attr.GetNamedItem(RETURN_TYPE).Value;
			
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
					case DParam.IAM:
						AddChild(new DParam(c));
						break;
					case Script.IAM:
						AddChild(new Script(c));
						break;
				}
			}
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
			get { return m_mapsTo; }
			set { m_mapsTo = value;	}
		}

		public string ReturnType
		{
			get { return m_returnType;	}
			set { m_returnType = value;	}
		}

		public const string MAPS_TO         = "maps-to";
		public const string RETURN_TYPE		= "return-type";

		public const string IAM				= "d-method";
	}
}
