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

	public class Logic : UimlAttributes, IUimlElement
	{
		private XmlNode m_tag;

		public Logic()
		{
		}

		public Logic(XmlNode n) : this() 
		{
			m_tag = n;
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name == IAM)
				base.ReadAttributes(n);					
		}

		public ArrayList Children
		{
			get { return null; }
		}

		public XmlNode Tag
		{
			get { return m_tag; }
		}

		public const string IAM = "logic";
	}
}

