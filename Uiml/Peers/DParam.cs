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
	/// This class represents a &lt;d-param&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-param (#PCDATA)&gt;
	/// &lt;!ATTLIST d-param
	///           id NMTOKEN #IMPLIED
	///           type CDATA #IMPLIED&gt;
	/// </summary>
	//public class DParam : IUimlElement
	public class DParam : Param 
	{
		protected ArrayList m_children;
        protected string m_defValue = null;

		public DParam()
		{
            m_children = new ArrayList();
        }

		public DParam(XmlNode n) : this()
		{
			Process(n);
		}
        
        protected override Param PreClone()
        {
            return new DParam();
        }

        public override object Clone()
        {
            DParam clone = (DParam)base.Clone();
            clone.m_defValue = m_defValue;
            if (m_children != null)
            {
                clone.m_children = new ArrayList();
                for (int i = 0; i < m_children.Count; i++)
                {
                    if (m_children[i] is ICloneable)
                        clone.m_children.Add(((ICloneable)m_children[i]).Clone());
                    else
                        clone.m_children.Add(m_children[i]);
                }
            }
            return clone;
        }

		public new void Process(XmlNode n)
		{
			base.Process(n, IAM);

            // check for default values
            if (n.FirstChild != null) 
            {
                m_defValue = n.FirstChild.Value;
            }
		}

        public bool HasDefaultValue 
        {
            get { return DefaultValue != null; } 
        }

        public string DefaultValue 
        {
            get { return m_defValue; }
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

		public new ArrayList Children
		{
			get { return m_children; }
		}

		public new const string IAM				= "d-param";
	}
}
