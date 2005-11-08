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
	using System.Text;
	using System.Collections;

	///<summary>
	///Implementation for the head element
	///   &lt;!ELEMENT head (meta*)&gt; 
	///   &lt;!ELEMENT meta EMPTY&gt;
	///   &lt;!ATTLIST meta 
	///                name NMTOKEN #REQUIRED 
	///                content CDATA #REQUIRED&gt;
	///</summary>
	public class Head : IUimlElement, ICloneable{
		private Hashtable metaChildren;

		public Head()
		{
			metaChildren = new Hashtable();
		}

		public Head(XmlNode n) : this()
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;

			if(n.HasChildNodes)
			{
				XmlNodeList xnl = n.ChildNodes;
				for(int i=0; i<xnl.Count; i++)
					if(xnl[i].Name == META)
					{
						XmlAttributeCollection attr = xnl[i].Attributes;
						metaChildren.Add(attr.GetNamedItem(NAME).Value, attr.GetNamedItem(CONTENT).Value);
					}
			}
		}

		public override String ToString()
		{
			StringBuilder strBuffer = new StringBuilder();
			strBuffer.Append("\n");
			foreach(DictionaryEntry entry in metaChildren)
			{
				strBuffer.Append(entry.Key).Append(":").Append(entry.Value);
				strBuffer.Append("\n");
			}
			strBuffer.Append("\n");
			return strBuffer.ToString();
		}

		public ArrayList Children
		{
			get { return null; }
		}
		
		public Object Clone()
		{
			Head iamclone = new Head();
			iamclone.MetaChildren = MetaChildren;
			return iamclone;			
		}
		
		public Hashtable MetaChildren
		{
			get { return metaChildren; }
			set { metaChildren= value; } 
		}

		public const String IAM     = "head";
		public const String META    = "meta"; 
		public const String NAME    = "name";
		public const String CONTENT = "content";
	}
}
