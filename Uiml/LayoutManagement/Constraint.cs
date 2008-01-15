/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.uhasselt.be/kris/research/uiml.net)

	 Copyright (C) 2005  Kris Luyten (kris.luyten@uhasselt.be)
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

	Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>
*/

using System;
using System.Collections;
using System.Xml;

using Cassowary;
using Cassowary.Parsing;

namespace Uiml.LayoutManagement
{
	public class Constraint : IUimlElement
	{
		private string m_strength;
		private string m_rule;

		private ClConstraint m_clConstraint;
		private ClStrength m_clStrength;

		private Layout m_layout;

        private Constraint()
        {
        }

		public Constraint(Layout layout)
		{
			m_layout = layout;
		}

		public Constraint(XmlNode n, Layout layout) : this(layout)
		{
			Process(n);
		}

		public Constraint(Layout layout, string rule) : this(layout)
		{
			m_rule = rule;
			m_strength = DEFAULT_STRENGTH;
			ProcessStrength();
		}

        public XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(IAM);
            //FIXME
            return node;
        }

        public virtual object Clone()
        {
            Constraint clone = new Constraint();
            clone.m_strength = m_strength;
            clone.m_rule = m_rule;
            clone.m_clConstraint = m_clConstraint;
            clone.m_clStrength = m_clStrength;

            if(m_layout != null)
            {
                clone.m_layout = (Layout)m_layout.Clone();
            }

            return clone;
        }

        public virtual object Clone()
        {
            Constraint clone = new Constraint();
            clone.m_strength = m_strength;
            clone.m_rule = m_rule;
            clone.m_clConstraint = m_clConstraint;
            clone.m_clStrength = m_clStrength;

            if(m_layout != null)
            {
                clone.m_layout = (Layout)m_layout.Clone();
            }

            return clone;
        }

		public void Process(XmlNode n)
		{
			if (n.Name != IAM)
				return;

			XmlAttributeCollection attr = n.Attributes;
				
			if (attr.GetNamedItem(STRENGTH) != null)
				m_strength = attr.GetNamedItem(STRENGTH).Value;
			else
				m_strength = DEFAULT_STRENGTH;

			ProcessStrength();
				
			if (n.HasChildNodes)
			{
				foreach (XmlNode child in n.ChildNodes)
				{
					switch(child.Name)
					{
						case RULE:
							ProcessRule(child);
							break;
						case ALIAS:
							ProcessAlias(child);
							break;
					}
				}
			}
		}

		protected void ProcessRule(XmlNode n)
		{
			m_rule = n.InnerText;
		}

		protected void ProcessAlias(XmlNode n)
		{
			string name;
			XmlAttributeCollection attr = n.Attributes;
			
			if (attr.GetNamedItem(NAME) != null)
			{
				name = attr.GetNamedItem(NAME).Value;
				
				ConstraintAlias cs = 
				  new ConstraintAlias(name, n.InnerText);
			
				// get rule from semantics (dynamically created)
				m_rule = cs.Rule;
			}
			else
			{
				Console.WriteLine("Constraint aliases must have a name attribute!");
			}
		}

		protected void ProcessStrength()
		{
			switch (m_strength)
			{
				case "required":
					m_clStrength = ClStrength.Required;
					break;
				case "strong":
					m_clStrength = ClStrength.Strong;
					break;
				case "medium":
					m_clStrength = ClStrength.Medium;
					break;
				case "weak":
					m_clStrength = ClStrength.Weak;
					break;
				default:
					m_clStrength = ClStrength.Strong;
					break;
			}
		}
		
		public void InitializeConstraint()
		{
			ParseRule();
		}

		protected void ParseRule()
		{
			Hashtable context = new Hashtable();
			
			foreach (string id in m_layout.Properties.Keys)
				context.Add(id, ((LayoutProperty) m_layout.Properties[id]).Variable);
			
			try
			{
				ClParser p = new ClParser();
				p.AddContext(context);
				p.Parse(m_rule);
				m_clConstraint = p.Result;
				m_clConstraint.Strength = m_clStrength;
			}
			catch (ExClParseError ecpe)
			{
				Console.WriteLine(ecpe);
			}
		}

		public ArrayList Children
		{
			get	{ return null; }
		}

		/// <summary>
		/// The actual constraint to be passed to the solver.
		/// </summary>
		public ClConstraint Value
		{
			get { return m_clConstraint; }
		}

		public const string IAM = "constraint";
		public const string STRENGTH = "strength";
		public const string DEFAULT_STRENGTH = "strong";
		
		public const string RULE = "rule";
		public const string ALIAS = "alias";
		public const string NAME = "name";
	}
}
