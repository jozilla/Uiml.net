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
	using System.IO;

	public class Param :  IUimlElement {
		private string m_type;
		private string m_value;
		private string m_identifier;
		private bool m_isout;

		public Param()
		{
		}

		public Param(XmlNode n) : this()
		{
			Console.WriteLine("PARAM!");
			Process(n);
		}
		
		/// <summary>
		/// This function is used by the DParam class.
		/// </summary>
		/// <param name="n">the XmlNode to process</param>
		/// <param name="additionalName">the name which must be checked instead of IAM</param>
		public void Process(XmlNode n, string additionalName)
		{
			if(n.Name == additionalName)
			{
				Value = n.Value;
				XmlAttributeCollection attr = n.Attributes;
				if(attr.GetNamedItem(ID) != null)
					Identifier = attr.GetNamedItem(ID).Value;
				if(attr.GetNamedItem(TYPE) != null)
					Type = attr.GetNamedItem(TYPE).Value;
				if(attr.GetNamedItem(DIRECTION) != null)
				{
					switch(attr.GetNamedItem(DIRECTION).Value)
					{
						case IN : IsOut = false;
							break;
						case OUT: IsOut = true;
							break;
						case INOUT: IsOut = false;
							break;
					}
				}
			}
		}

		public void Process(XmlNode n)
		{
			if(n.Name == IAM)
			{
				Value = n.Value;
			   XmlAttributeCollection attr = n.Attributes;
				if(attr.GetNamedItem(ID) != null)
					Identifier = attr.GetNamedItem(ID).Value;
				if(attr.GetNamedItem(TYPE) != null)
					Type = attr.GetNamedItem(TYPE).Value;
				if(attr.GetNamedItem(DIRECTION) != null)
				{
					switch(attr.GetNamedItem(DIRECTION).Value)
					{
						case IN : IsOut = false;
									 break;
						case OUT: IsOut = true;
									 break;
						case INOUT: IsOut = false;
										break;
					}
				}					
			}
		}

		public string Type
		{
			get { return m_type;  }
			set { m_type = value; }
		}

		public string Identifier
		{
			get { return m_identifier;  }
		   set { m_identifier = value; }
		}

		public string Value
		{
			get { return m_value;  }
			set { m_value = value; }
		}

		public bool IsOut
		{
			get { return m_isout;  }
			set { m_isout = value; }
		}


		public override string ToString()
		{
			return "{" + Identifier + "}:" + Value + ":" + Type;
		}

		public ArrayList Children
		{
			get { return null; }
		}


		public const string TYPE = "type";
		public const string ID = "id";
		public const string IAM = "param";
		public const string DIRECTION = "direction";
		public const string IN = "in";
		public const string OUT = "out";
		public const string INOUT = "inout";
		
	}
}
