/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/


namespace Uiml{
	
	using System;
	using System.Xml;
	using System.Collections;

	///<summary>
	///Implementation for the restructure element. This allows to change
	/// the structure of the User Interface at runtime
	/// &lt;!ELEMENT restructure (template)?&gt; 
	/// &lt;!ATTLIST restructure at-part NMTOKEN #IMPLIED 
	///              how (union|cascade|replace|delete) "replace" 
	///              where (first|last|before|after) "last" 
	///              where-part NMTOKEN #IMPLIED source CDATA #IMPLIED&gt;
	///</summary>
	public class Restructure : IUimlElement{
	
		public Restructure()
		{
		}
		
		public Restructure(XmlNode n) : this()
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;
		}

		public ArrayList Children
		{
			get { return null; }//for now
		}

		public const String IAM     = "restructure";
	}
}
