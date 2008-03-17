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



namespace Uiml{

	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Generic;

    using System.Xml.Serialization;
    using System.Xml;

	///<summary>
	/// Implements the structure element of Uiml. Instead of allowing several parts as "top"-elements
	/// this implementation only allows one part as a top element. When the structure element
	/// has several part elements it embeds these children as the children of one part element "Top".
	/// Top will be the only child of structure then.
	///
	///&lt;!ELEMENT structure (part*)&gt; 
	///&lt;!ATTLIST structure id NMTOKEN #IMPLIED 
	///                       source CDATA #IMPLIED 
	///                       how (append|cascade|replace) "replace" 
	///                       export (hidden|optional|required) "optional"&gt;
	///</summary>
	public class Structure : UimlAttributes, IUimlElement{
		private Part m_top;
		
		public Structure() 
		{
		}

		public Structure(XmlNode n)
		{
			Process(n);
		}

        public virtual object Clone()
        {
            Structure structure = new Structure();
            if(m_top != null)
            {
                structure.m_top = (Part)m_top.Clone();
            }

            return structure;
        }

		///<summary>
		///Processes the structure subtree
		///</summary>
		///<remarks>
		///precondition: n has children
		///</remarks>
		public void Process(XmlNode n)
		{
			if(n.Name !=  IAM)
				return;
			
			base.ReadAttributes(n);
			if(n.ChildNodes.Count == 1)
			{
				m_top = new Part(n.ChildNodes[0]);
			}
			else
			{
				m_top = new Part("top-container");
				m_top.Class = "Frame";
				XmlNodeList xnl = n.ChildNodes;
				for(int i=0; i<xnl.Count; i++)
					m_top.AddChild(new Part(xnl[i]));
			}
		}

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(IAM);
            List<XmlAttribute> list = CreateAttributes(doc);

            foreach (XmlAttribute attr in list)
            {
                node.Attributes.Append(attr);
            }

            if (m_top != null)
            {
                XmlNode xmlTop = m_top.Serialize(doc);
                node.AppendChild(xmlTop);
            }

            return node;
        }

		public Part Top
		{
			get
			{
				if(m_top == null)
					Console.WriteLine("warning: no ui structure!");
				return m_top;
			}

			set
			{
				m_top = value;
			}
		}

		public Part SearchPart(string partName)
		{
			return Top.SearchPart(partName);
		}

        [XmlIgnore]
		public ArrayList Children
		{
			get { return null; }
		}

		public string PartTree() 
		{
			return Top.ToString();
		}

		public const string IAM = "structure";
	}
}
