/*
  	 Uiml.Net: a Uiml.Net renderer (http://lumumba.luc.ac.be/kris/research/uiml.net/)
   
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

namespace Uiml.Executing
{
	using Uiml;
	using Uiml.Rendering;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	
	public class Event : UimlAttributes, IExecutable, IUimlElement
	{
		private string m_executeType;
		private System.Object m_ExecuteObject;
		private string m_name;
		private string m_class;
		private string m_partName;
		private string m_partClass;

		private IExecutable m_parent;
		

		public Event()
		{
		}

		public Event(XmlNode xmlNode)
		{
			Process(xmlNode);
		}

		public void Process(XmlNode n)
		{
			if(n.Name == EVENT)
			{
		   	XmlAttributeCollection attr = n.Attributes;
				if(attr.GetNamedItem(NAME) != null)
				 Name = attr.GetNamedItem(NAME).Value;
				if(attr.GetNamedItem(CLASS) != null)
				 Class = attr.GetNamedItem(CLASS).Value;
				if(attr.GetNamedItem(PARTNAME) != null)
				 PartName = attr.GetNamedItem(PARTNAME).Value;
				if(attr.GetNamedItem(PARTCLASS) != null)
				 PartClass = attr.GetNamedItem(PARTCLASS).Value;
			}
		}


		public System.Object Execute()
		{
			return null;
		}

		public System.Object Execute(IRenderer renderer)
		{
			return null;
		}

		public string ExecuteType
		{
			get { return m_executeType; }
		}

		public string Name
		{
			get { return m_name;  }
			set { m_name = value; }
		}

		public string PartName
		{
			get { return m_partName;  }
			set { m_partName = value; }
		}

		public string PartClass
		{
			get { return m_partClass;  }
			set { m_partClass = value; }
		}

		public string Class
		{
			get { return m_class;  }
			set { m_class = value; }
		}

		public ArrayList Children
		{
			get { return null; }
		}

		public const string EVENT = "event";
		public const string NAME  = "name";
		public const string CLASS = "class";
		public const string PARTNAME  = "part-name";
		public const string PARTCLASS = "part-class";

	}

}
