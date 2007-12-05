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

	///<summary>
	///Implementation for the reference element
	///   &lt;!ELEMENT reference EMPTY&gt; 
	///   &lt;!ATTLIST reference constant-name NMTOKEN #IMPLIED 
	///                          url-name NMTOKEN #IMPLIED&gt;
	///</summary>
	public class Reference : IUimlElement{
		private String m_constantName;
		private String m_urlName;

		public Reference()
		{
		}

		public Reference(XmlNode n) : this()
		{
			Process(n);
		}

        public virtual object Clone()
        {
            Reference clone = new Reference();
            clone.m_constantName = m_constantName;
            clone.m_urlName = m_urlName;

            return clone;
        }

		public void Process(XmlNode n)
		{
			if(n.Name != REFERENCE)
				return;
			
			XmlAttributeCollection attr = n.Attributes;
			if(attr.GetNamedItem(CONSTANT_NAME) != null) //it is a model == other sub constants are there!!
				ConstantName = attr.GetNamedItem(CONSTANT_NAME).Value;
			if(attr.GetNamedItem(URL_NAME) != null)
			   UrlName = attr.GetNamedItem(URL_NAME).Value; //it is always a string

		}

		public String ConstantName
		{
			get { return m_constantName;  }
			set { m_constantName = value; }
		}

		public String UrlName
		{
			get { return m_urlName;  }
			set { m_urlName = value; }
		}

		public ArrayList Children
		{
			get { return null; }
		}

		///<summary>
		///Retrieves the value for this constant from 
		///a given content section
		///</summary>
		public Constant Resolve(Content content)
		{
			return content.Query(ConstantName);
		}

		public const String REFERENCE = "reference";
		public const String CONSTANT_NAME = "constant-name";
		public const String URL_NAME = "UrlName";

	}
}
