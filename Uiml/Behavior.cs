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

	using Uiml.Executing;

	///<summary>
	///Represents the behavior element:
	/// &lt;!ELEMENT behavior (rule*)&gt; 
	/// &lt;!ATTLIST behavior id NMTOKEN #IMPLIED 
	///              source CDATA #IMPLIED 
	///              how (union|cascade|replace) "replace" 
	///              export (hidden|optional|required) "optional"&gt;
	///</summary>
	public class Behavior : UimlAttributes, IUimlElement{
		private ArrayList m_rules;
		private Part      m_partTree;

		private XmlNode m_waitingNode;

		public Behavior()
		{
			m_rules = new ArrayList();
		}

		public Behavior(XmlNode n, Part topPart) : this()
		{
			m_partTree = topPart;
			Process(n);
		}

		public Behavior(XmlNode n) : this()
		{
			m_waitingNode = n;
		}

        public virtual object Clone()
        {
            Behavior clone = new Behavior();
            clone.CopyAttributesFrom(this);

            if(m_rules != null)
            {
                clone.m_rules = new ArrayList();
                for(int i = 0; i < m_rules.Count; i++)
                {
                    Rule rule = (Rule)m_rules[i];                    
                    clone.m_rules.Add(rule.Clone());
                }
            }
            clone.PartTree = PartTree;

            return clone;
        }

		///<summary>
		///Factory method that resolves this Behavior for a given Part 
		///</summary>
		public Behavior Resolve(Part partTop)
		{
			if(m_waitingNode != null)
				return new Behavior(m_waitingNode, partTop);
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
					XmlNodeList xnl = n.ChildNodes;
					for(int i=0; i<xnl.Count; i++)
						m_rules.Add(new Rule(xnl[i], m_partTree));
				}

			}
		}

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(IAM);
            List<XmlAttribute> attributes = this.CreateAttributes(doc);

            foreach (XmlAttribute attr in attributes)
            {
                node.Attributes.Append(attr);
            }

            for (int i = 0; i < m_rules.Count; i++)
            {
                IUimlElement element = (IUimlElement)m_rules[i];
                node.AppendChild(element.Serialize(doc));
            }

            return node;
        }

        public void AttachPeers(ArrayList logics)
		{
			
			IEnumerator enumRules = Rules;
			while(enumRules.MoveNext())
			{
				AttachPeers((Rule)enumRules.Current, logics);
			}
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
                if(m_rules != null)
                {
                    for(int i = 0; i < m_rules.Count; i++)
                    {
                        Rule rule = (Rule)m_rules[i];
                        rule.PartTree = PartTree;
                    }
                }
            }
        }

		private void AttachPeers(IUimlElement iue, ArrayList logics)
		{
			if(iue is Uiml.Executing.Rule)
			{									
				((Rule)iue).Action.AttachLogic(logics);
			}

			if(iue.Children != null)
			{
				IEnumerator enumChildren = iue.Children.GetEnumerator();
				while(enumChildren.MoveNext())
				{
					AttachPeers((IUimlElement)enumChildren.Current, logics);
				}				
			}
		}

		public IEnumerator Rules
		{
			get {	return m_rules.GetEnumerator(); }
		}

		public override string ToString()
		{
			string str= "<behavior>";
			
			IEnumerator er = Rules;
			while(er.MoveNext())
					str += er.Current + "\n";
			str += "</behavior>";
			return str;
			
		}

		public ArrayList Children
		{
			get { return m_rules; }
		}

		public const string IAM = "behavior";
	}
}

