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

	using Uiml.Rendering;
	
	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Generic;
	using System.IO;

	
	public class Rule : UimlAttributes, IExecutable, IUimlElement
	{
		private Condition m_condition;
		private Action    m_action;
		private Part      m_partTree;

        private XmlNode   m_waitingNode;

        private bool      m_empty;

		public Rule()
        {
            IsEmpty = true;
		}

		public Rule(XmlNode xmlNode, Part partTop) : this()
		{
			m_partTree = partTop;
			Process(xmlNode);
		}

		///<summary>
		///This constructor is for rules that are contained in a Template element:
		///ar parse-time there is no logical parent for this node
		///</summary>
		public Rule(XmlNode xmlNode) : this()
		{
			m_waitingNode = xmlNode;
		}

        public virtual object Clone()
        {
            Rule clone = new Rule();            
            if(m_waitingNode != null)
                clone.m_waitingNode = (XmlNode)m_waitingNode.Clone();

            clone.m_action = (Action)m_action.Clone();
            clone.m_empty = m_empty;

            clone.m_condition = (Condition)m_condition.Clone();
            clone.m_condition.Attach(clone.m_action);
            clone.PartTree = PartTree;
            return clone;
        }

		///<summary>
		///Factory method that resolves this Rule for a given Part 
		///</summary>
		public Rule Resolve(Part partTop)
		{
			if(m_waitingNode != null)
				return new Rule(m_waitingNode, partTop);
			else
				return null;//this should never happen!
		}

		public void Process(XmlNode n)
		{
			ReadAttributes(n);
			if(n.Name == IAM)
			{
				if(n.HasChildNodes)
                {
                    IsEmpty = false;
					XmlNodeList xnl = n.ChildNodes;
					m_condition = new Condition(xnl[0], m_partTree);
					m_action    = new Action(xnl[1], m_partTree);
					m_condition.Attach(m_action);
				}
			}
		}

        public override XmlNode Serialize( XmlDocument doc )
        {
            XmlElement element = doc.CreateElement(IAM);
            
            List<XmlAttribute> attributes = CreateAttributes(doc);
            foreach( XmlAttribute attr in attributes )
            {
                element.Attributes.Append( attr );
            }

            ArrayList list = new ArrayList();
            list.Add(m_condition);
            list.Add(m_action);            
            for (int i = 0; i < list.Count; i++)
            {
                IUimlElement uimlElement = (IUimlElement)list[i];
                element.AppendChild(uimlElement.Serialize(doc));
            }

            return element;
        }

        public void Connect(object o)
		{
			m_action.Connect(o);
		}

		public void Disconnect(object o)
		{
			m_action.Disconnect(o);
		}
			
		public System.Object Execute()
		{
			//using this function would mean "pushing" of actions
			//by the ``conditions''
			//
			//check condition
			//execute action
			return null;
		}

		public System.Object Execute(IRenderer renderer)
		{
			return null;
		}

		public Action Action
		{
			get { return m_action; }
		}

		public Condition Condition
		{
			get { return m_condition; }
		}

		public ArrayList Children
		{
			get 
            {
                return null;
            }
        }

        public bool IsEmpty
        {
            set { m_empty = value; }
            get { return m_empty; }
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
                if(m_action != null)
                {
                    m_action.PartTree = value;
                }
                if(m_condition != null)
                {
                    m_condition.PartTree = value;
                }
            }
        }

		public const string IAM      = "rule";

	}

}
