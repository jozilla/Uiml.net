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

	public class Equal :  IExecutable, IUimlElement
	{	
        private Part m_partTree;
        private Event m_event;
        private object m_childObject;
        private string m_childType = null;

		public Equal()
		{
            m_childObject = null;
            m_childType = null;
		}

		public Equal(XmlNode xmlNode, Part partTop) : this()
		{
            m_partTree = partTop;
			Process(xmlNode);  
		}

        //TODO: Check cloning
        public virtual object Clone()
        {
            Equal clone = new Equal();
            if (m_childObject != null && m_childObject is ICloneable)
                clone.m_childObject = ((ICloneable)m_childObject).Clone();
            if (m_event != null)
                clone.m_event = (Event)m_event.Clone();
            clone.m_childType = m_childType;
            clone.PartTree = PartTree;

            return clone;
		}

		public void Process(XmlNode n)
		{
            if (n.Name != IAM)
                return;

            // cannot have attributes

            if (n.HasChildNodes)
            {
                XmlNodeList xnl = n.ChildNodes;
                if (xnl.Count != 2)
                {
                    // parameter mismatch
                    throw new XmlElementMismatchException("Your input document is not in the correct format. <equal> should have only 2 elements.");
                }
                else
                {
                    for (int i = 0; i < xnl.Count; i++)
                    {
                        switch (xnl[i].Name)
                        {
                            case EVENT:
                                m_event = new Event(xnl[0]);//Possible bug....
                                break;
                            case CONSTANT:
                                m_childType = CONSTANT;
                                m_childObject = new Constant(xnl[0]);
                                break;
                            case PROPERTY:
                                m_childType = PROPERTY;
                                m_childObject = new Property(xnl[0]);
                                break;
                            case REFERENCE:
                                m_childType = REFERENCE;
                                m_childObject = new Reference(xnl[0]);
                                break;
                            case OP:
                                m_childType = OP;
                                m_childObject = new Op(xnl[0], m_partTree);
                                break;
                        }
                    }

                    // error handling
                    // event must be <> null
                    if (m_event == null)
                    {
                        throw new NotInitializedException("Your input document is not in the correct format. <equal> must have 1 <event>.");
                    }
                    // at least one of (constant, property, reference, op) must be init
                    if (m_childObject == null)
                    {
                        throw new NotInitializedException("Your input document is not in the correct format.  Check your syntax near <equal>.");
                    }
                }
            }
        }

        public XmlNode Serialize(XmlDocument doc)
        {
            XmlNode nod = doc.CreateElement(IAM);
            //FIXME
            return nod;
        }

        public void GetEvents(ArrayList al)
        {
            al.Add(m_event);
            if (m_childObject is Op)
                ((Op)m_childObject).GetEvents(al);
            else
                al.Add(m_childObject);
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
			get { return null; }
		}  

        public Part PartTree
        {
            get
            {
                return m_partTree;
            }
            set
            {
                m_partTree = value;
                if (m_childObject is Op)
                    ((Op)m_childObject).PartTree = value;
            }
        }

        public const string IAM       = "equal";
        public const string EVENT     = "event";
        public const string CONSTANT  = "constant";
        public const string PROPERTY  = "property";
        public const string REFERENCE = "reference";
        public const string OP        = "op";
    }
	
}
