/*
  	 Uiml.Net: a Uiml.Net renderer (http://lumumba.uhasselt.be/kris/research/uiml.net/)
   
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

namespace Uiml.Executing
{
	using Uiml;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	
	public class Op :  IExecutable, IUimlElement
    {
        private ArrayList m_children;
        private string m_type; // type of the OP => && || == != < >
        private Part m_top;

        public Op()
        {
            m_children = new ArrayList();
        }

        public Op(XmlNode xmlNode, Part partTop)
            : this()
        {
            m_top = partTop;
            Process(xmlNode);
        }	 

        public virtual object Clone()
        {
            Op clone = new Op();
            if(m_children != null)
            {
                clone.m_children = new ArrayList();
                for(int i = 0; i < m_children.Count; i++)
                {
                    IUimlElement element = (IUimlElement)m_children[i];
                    clone.m_children.Add(element.Clone());
                }
            }
            clone.PartTree = m_top;

            return clone;
        }

        public void Process(XmlNode n)
        {
            if (n.Name == OP)
            {
                ReadAttributes(n);

                if (n.HasChildNodes)
                {
                    XmlNodeList xnl = n.ChildNodes;
                    for (int i = 0; i < xnl.Count; i++)
                    {
                        switch (xnl[i].Name)
                        {
                            case CONSTANT:
                                m_children.Add(new Constant(xnl[i]));
                                break;
                            case PROPERTY:
                                m_children.Add(new Property(xnl[i]));
                                break;
                            case REFERENCE:
                                m_children.Add(new Reference(xnl[i]));
                                break;
                            case CALL:
                                m_children.Add(new Call(xnl[i]));
                                break;
                            case OP:
                                m_children.Add(new Op(xnl[i], m_top));
                                break;
                            case EVENT:
                                m_children.Add(new Event(xnl[i]));
                                break;
                        }
                    }
                }
            }
        }

        public XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(OP);

            if (Type.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(NAME);
                attr.Value = Type;
                node.Attributes.Append(attr);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                IUimlElement element = (IUimlElement)Children[i];
                node.AppendChild(element.Serialize(doc));
            }

            return node;
        }

        public void ReadAttributes(XmlNode node)
        {
            XmlAttributeCollection xac = node.Attributes;

            if (xac.GetNamedItem(NAME) != null)
                Type = xac.GetNamedItem(NAME).Value;
            else
                throw new AttributeException("An <op> needs an attribute called 'name'");
        }

        public void GetEvents(ArrayList al)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is Op)
                    ((Op)Children[i]).GetEvents(al);
                else
                    al.Add(Children[i]);
            }
        }

        public Object Execute()
        {
            return null;
        }

        public Object Execute(Uiml.Rendering.IRenderer renderer)
        {
            return Execute();
        }

        public ArrayList Children
        {
            get { return m_children; }
        }

        public string Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public bool CheckCondition()
        {
            return true;
            //throw new Exception("The method or operation is not implemented.");
        }		

        public Part PartTree
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
                for(int i = 0; i < m_children.Count; i++)
                {
                    if(m_children[i] is Op)
                    {
                        Op tmp = (Op)m_children[i];
                        tmp.PartTree = m_top;
                    }
                }
            }
        }


        public const string ACTION      = "action";
        public const string PROPERTY    = "property";
        public const string CALL        = "call";
        public const string OP          = "op";
        public const string EVENT       = "event";
        public const string REFERENCE   = "reference";
        public const string CONSTANT    = "constant";
        public const string AND         = "and";            // &&
        public const string OR          = "or";             // ||
        public const string EQUAL       = "equal";          // ==
        public const string NOTEQUAL    = "notequal";       // !=
        public const string LESSTHAN    = "lessthan";       // <
        public const string GREATERTHAN = "greatherthan";   // >
        public const string NAME        = "name";
    }
	
}
