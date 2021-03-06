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

namespace Uiml.Peers
{
	using Uiml;
	using Uiml.Executing;
	using Uiml.Rendering;
	
	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// This class represents a &lt;event&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT event EMPTY&gt;
	/// &lt;!ATTLIST event
	///           class NMTOKEN #IMPLIED
	///           part-name NMTOKEN #IMPLIED
	///           part-class NMTOKEN #IMPLIED&gt;
	/// </summary>
	public class Event : UimlAttributes, IUimlElement
	{
		private string m_executeType = "";
		private System.Object m_ExecuteObject;
		private string m_name = ""; // FIXME: this is not part of the specification!
		private string m_class = "";
		private string m_partName = "";
		private string m_partClass = "";

		private IExecutable m_parent; // wordt dit gebruikt ??
		

		public Event()
		{
		}

		public Event(XmlNode xmlNode)
		{
			Process(xmlNode);
		}

        public virtual object Clone()
        {
            Event clone = new Event();
            clone.CopyAttributesFrom(this);
            clone.m_executeType = m_executeType;
            clone.m_name = m_name;
            clone.m_class = m_class;
            clone.m_partName = m_partName;
            clone.m_partClass = m_partClass;
            clone.m_parent = m_parent;
            clone.m_ExecuteObject = m_ExecuteObject; //FIXME: Clone this (if possible)

            return clone;
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

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("event");
            List<XmlAttribute> attributes = base.CreateAttributes(doc);

            if (Name.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(NAME);
                attr.Value = Name;
                attributes.Add(attr);
            }
            if (Class.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(CLASS);
                attr.Value = Class;
                attributes.Add(attr);
            }
            if (PartName.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(PARTNAME);
                attr.Value = PartName;
                attributes.Add(attr);
            }
            if (PartClass.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(PARTCLASS);
                attr.Value = PartClass;
                attributes.Add(attr);
            }

            foreach (XmlAttribute attr in attributes)
            {
                node.Attributes.Append(attr);
            }

            return node;
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

		public const string EVENT			= "event";
		public const string NAME			= "name";
		public const string CLASS			= "class";
		public const string PARTNAME		= "part-name";
		public const string PARTCLASS		= "part-class";
		
		public const string IAM				= "event";
	}

}
