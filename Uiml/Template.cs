/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU General Public License
	as published by the Free Software Foundation; either version 2
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/


namespace Uiml{
	
	using System;
	using System.Xml;
	using System.Collections;

	///<summary>
	///Implementation for the template element
	///    &lt;!ELEMENT template 
	///                   (behavior|constant|content|d-class|d-component| 
	///                    interface |logic|part|peers|presentation|property| 
	///                    restructure|rule |script|structure|style)&gt;
	///    &lt;!ATTLIST template id NMTOKEN #IMPLIED&gt;
	///</summary>
	public class Template : IUimlElement{
		public String m_identifier;
		public IUimlElement m_top;

		public Template()
		{
		}

		public Template(XmlNode n) : this()
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name != TEMPLATE)
				return;


		}

		public string Identifier
		{
			get { return m_identifier;  }
			set { m_identifier = value; }
		}

		public IUimlElement Top
		{
			get { return m_top; }
		}

		public ArrayList Children
		{
			get 
			{ 
				ArrayList a = new ArrayList(); 
				a.Add(Top); 
				return a; 
			}
		}


		public const String TEMPLATE = "template";

	}
}
