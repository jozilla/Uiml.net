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
	/// This class represents a &lt;d-param&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-param (#PCDATA)&gt;
	/// &lt;!ATTLIST d-param
	///           id NMTOKEN #IMPLIED
	///           type CDATA #IMPLIED&gt;
	/// </summary>
	//public class DParam : IUimlElement
	public class DParam : Param 
	{
		protected ArrayList m_children = null;

		public DParam()
		{}

		public DParam(XmlNode n)
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			base.Process(n);
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

		public ArrayList Children
		{
			get { return m_children; }
		}

		public const string IAM				= "d-param";
	}
}