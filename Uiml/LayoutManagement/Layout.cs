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

namespace Uiml.LayoutManagement
{
	public class Layout : IUimlElement
	{
		private ArrayList m_constraints;
		private ArrayList m_clConstraints = null;
		private ArrayList m_implicitConstraints;
		private Hashtable m_properties;
		private Structure m_structure;
		private ConstraintSystem m_constraintSystem;

		private string[] m_layoutProperties		= { "top", "bottom", "left", "right", "height", "width" };
		private string[] m_virtualLayoutProperties	= { "depth" };

		private Part m_container;

		private Box m_box;

		public Layout()
		{
			m_constraints = new ArrayList();
			m_implicitConstraints = new ArrayList();
			m_properties = new Hashtable();
			m_container = null;
			m_structure = null;
		}

		public Layout(Part container) : this()
		{
			m_container = container;
			AddAllLayoutProperties();
		}

		public Layout(XmlNode n, Structure structure) : this()
		{
			m_structure = structure;
			Process(n);
		}

		public Layout(XmlNode n, Part container) : this()
		{
			m_container = container;
			Process(n);
		}

        public virtual object Clone()
        {
            //FIXME:Fix this clone function
            Layout clone = new Layout();
            clone.m_constraints = m_constraints;
            clone.m_clConstraints = m_clConstraints;
            clone.m_implicitConstraints = m_implicitConstraints;
            clone.m_properties = m_properties;
            clone.m_structure = m_structure;
            clone.m_constraintSystem = m_constraintSystem;
            clone.m_container = m_container;
            clone.m_box = m_box;

            return clone;
        }

		public void Process(System.Xml.XmlNode n)
		{
			if (n.Name != IAM)
				return;
			
			XmlAttributeCollection attr = n.Attributes;
			if (attr.GetNamedItem(PART_NAME) != null)
			{
				string part_name = attr.GetNamedItem(PART_NAME).Value;
				try 
				{
					m_container = m_structure.SearchPart(part_name);
				}
				catch (NullReferenceException nre)
				{
					// structure not defined
					throw new LayoutException("Structure must be given when part-name is used");
				}
			}
			else
			{
				if (m_structure != null)
				{
					// use top part
					m_container = m_structure.Top;
				}
				else
				{
					// m_container should be specified
					if (m_container == null)
					{
						throw new LayoutException("Inline layouts must have Container property filled in");
					}
				}
			}

			AddAllLayoutProperties();

			if (n.HasChildNodes)
			{
				XmlNodeList xnl = n.ChildNodes;
				for (int i=0; i<xnl.Count; i++)
					if (xnl[i].NodeType != XmlNodeType.Comment)
					{
						if (xnl[i].Name == Constraint.IAM)
							AddConstraint(new Constraint(xnl[i], this));
						else
							ProcessBox(xnl[i]);
					}
			}
		}

		protected void ProcessBox(System.Xml.XmlNode n)
		{
			XmlAttributeCollection attr = n.Attributes;
			uint spacing = 0;
			bool homogeneous = false;

			switch(n.Name)
			{
				case "hbox":
					if (attr.GetNamedItem("spacing") != null)
						spacing = uint.Parse(attr.GetNamedItem("spacing").Value);
					if (attr.GetNamedItem("homogeneous") != null)
						homogeneous = bool.Parse(attr.GetNamedItem("homogeneous").Value);
					m_box = new Box("width", "left", "right", "height", "top", "bottom", this, spacing, homogeneous);
					break;
				case "vbox":
					if (attr.GetNamedItem("spacing") != null)
						spacing = uint.Parse(attr.GetNamedItem("spacing").Value);
					if (attr.GetNamedItem("homogeneous") != null)
						homogeneous = bool.Parse(attr.GetNamedItem("homogeneous").Value);
					m_box = new Box("height", "top", "bottom", "width", "left", "right", this, spacing, homogeneous);
					break;
			}
		}

		public void AddConstraint(Constraint c)
		{
			m_constraints.Add(c);
		}

		protected void AddAllLayoutProperties(Part p)
		{
			// layout properties
			foreach (string propName in m_layoutProperties)
			{
				LayoutProperty lp = new LayoutProperty(p.Identifier, propName);
				m_properties.Add(lp.HashValue, LayoutPropertyRepository.Instance.Add(lp));
			}

			// virtual layout properties
			foreach (string propName in m_virtualLayoutProperties)
			{
				VirtualLayoutProperty vlp = new VirtualLayoutProperty(p.Identifier, propName);
				m_properties.Add(vlp.HashValue, LayoutPropertyRepository.Instance.Add(vlp));
			}
		}

		/// <summary>
		/// Used for reordering.
		/// </summary>
		/// <param name="p">the reordered part</param>
		/// <param name="child">the original part</param>
		public void AddAndInitializeReorderedLayoutProperties(Part p, Part child)
		{
			AddAllLayoutProperties(p);
			
			// now initialize the properties of this part to the ones of the child

			foreach (string propName in m_layoutProperties)
			{
				string sChildVal = (string) LayoutPropertyRepository.Instance.Get(p.Identifier + "." + propName).Value;
				double dChildVal = double.Parse(sChildVal);
				
				LayoutPropertyRepository.Instance.Get(p.Identifier + "." + propName).InitializeVariable(dChildVal);
			}
		}

		public void ResetVirtualProperties(Part[] parts)
		{
			foreach (Part p in parts)
			{
				foreach (string propName in m_layoutProperties)
				{
					LayoutPropertyRepository.Instance.Get(p.Identifier + "." + propName).InitializeVariable(0);
				}
			}
		}

		/// <summary>
		/// Add all layout properties for the corresponding part and its subparts.
		/// </summary>
		protected void AddAllLayoutProperties()
		{
			AddAllLayoutProperties(Container);
			
			// now the subparts (one level deep)!
			foreach (Part ch in Container.Children)
			{
				AddAllLayoutProperties(ch);
			}
		}

		protected void AddStayConstraints(Part p)
		{
			ClVariable height, width, top, left;

			height = ((LayoutProperty) m_properties[p.Identifier + ".height"]).Variable;
			width = ((LayoutProperty) m_properties[p.Identifier + ".width"]).Variable;
			top = ((LayoutProperty) m_properties[p.Identifier + ".top"]).Variable;
			left = ((LayoutProperty) m_properties[p.Identifier + ".left"]).Variable;
				
			m_implicitConstraints.Add(new ClStayConstraint(height));
			m_implicitConstraints.Add(new ClStayConstraint(width));
		}

		protected void AddImplicitConstraints()
		{
			// stay constraints must be added first!
			AddStayConstraints();
			AddSemanticConstraints();
		}

		protected void AddStayConstraints()
		{
			foreach(Part ch in Container.Children)
			{
				// only for non-containers
				if (ch.Children.Count == 0)
					AddStayConstraints(ch);
			}
		}

		protected void AddSemanticConstraints()
		{
			AddSemanticConstraints(Container);

			foreach(Part ch in Container.Children)
			{
				// for non-containers
				if (ch.Children.Count == 0)
					AddSemanticConstraints(ch);
				else // for containers
					AddSemanticConstraintsForChildContainer(ch);
			}
		}

		protected void AddSemanticConstraintsForChildContainer(Part p)
		{
			ClVariable container_width, container_height, bottom, right;
			ClConstraint inside_bounds;

			container_width = ((LayoutProperty) m_properties[PartName + ".width"]).Variable;
			container_height = ((LayoutProperty) m_properties[PartName + ".height"]).Variable;
			bottom = ((LayoutProperty) m_properties[p.Identifier + ".bottom"]).Variable;
			right = ((LayoutProperty) m_properties[p.Identifier + ".right"]).Variable;

			// in boundaries of parent! -> not for container itself ofcourse
			if (PartName != p.Identifier)
			{
				inside_bounds = new ClLinearInequality(bottom, Cl.LEQ, container_height, ClStrength.Required);
				m_implicitConstraints.Add(inside_bounds);

				inside_bounds = new ClLinearInequality(right, Cl.LEQ, container_width, ClStrength.Required);
				m_implicitConstraints.Add(inside_bounds);
			}
		}

		protected void AddSemanticConstraints(Part p)
		{
			ClVariable top, bottom, left, right, height, width, container_width, container_height;
			ClConstraint height_semantics, width_semantics, positive, inside_bounds;

			top = ((LayoutProperty) m_properties[p.Identifier + ".top"]).Variable;
			bottom = ((LayoutProperty) m_properties[p.Identifier + ".bottom"]).Variable;
			left = ((LayoutProperty) m_properties[p.Identifier + ".left"]).Variable;
			right = ((LayoutProperty) m_properties[p.Identifier + ".right"]).Variable;
			height = ((LayoutProperty) m_properties[p.Identifier + ".height"]).Variable;
			width = ((LayoutProperty) m_properties[p.Identifier + ".width"]).Variable;

			container_width = ((LayoutProperty) m_properties[PartName + ".width"]).Variable;
			container_height = ((LayoutProperty) m_properties[PartName + ".height"]).Variable;

			// bottom = top + height
			height_semantics = new ClLinearEquation(bottom, Cl.Plus(top, new ClLinearExpression(height)), ClStrength.Required);
			// right = left + width
			width_semantics = new ClLinearEquation(right, Cl.Plus(left, new ClLinearExpression(width)), ClStrength.Required);
				
			m_implicitConstraints.Add(height_semantics);
			m_implicitConstraints.Add(width_semantics);

			// all positive integers (including zero)
			positive = new ClLinearInequality(top, Cl.GEQ, 0.0, ClStrength.Required);
			m_implicitConstraints.Add(positive);

			positive = new ClLinearInequality(bottom, Cl.GEQ, 0.0, ClStrength.Required);
			m_implicitConstraints.Add(positive);

			positive = new ClLinearInequality(left, Cl.GEQ, 0.0, ClStrength.Required);
			m_implicitConstraints.Add(positive);

			positive = new ClLinearInequality(right, Cl.GEQ, 0.0, ClStrength.Required);
			m_implicitConstraints.Add(positive);

			// in boundaries of parent! -> not for container itself ofcourse
			if (PartName != p.Identifier)
			{
				inside_bounds = new ClLinearInequality(bottom, Cl.LEQ, container_height, ClStrength.Required);
				m_implicitConstraints.Add(inside_bounds);

				inside_bounds = new ClLinearInequality(right, Cl.LEQ, container_width, ClStrength.Required);
				m_implicitConstraints.Add(inside_bounds);
			}
		}

		public LayoutProperty AddProperty(LayoutProperty lp)
		{
			try
			{
				m_properties.Add(lp.HashValue, LayoutPropertyRepository.Instance.Add(lp));
				return lp;
			}
			catch(ArgumentException)
			{
				// duplicate entry: fail silently
				return (LayoutProperty) m_properties[lp.HashValue];
			}
		}
		
		protected void InitializeConstraints()
		{
			foreach (Constraint c in m_constraints)
			{
				c.InitializeConstraint();
			}
		}

		public void RemoveConstraint(Constraint c)
		{
			m_constraints.Remove(c);
			Constraints.Remove(c.Value);
		}

		public void RemoveConstraint(ClConstraint c)
		{
			Constraints.Remove(c);
		}
		
		public void SolveLayoutProperties()
		{
			AddImplicitConstraints();
			InitializeConstraints();

			if (m_box != null)
			{
				m_box.BuildConstraints();
			}

			BuildConstraints();
		}

		protected void BuildConstraints()
		{
			m_clConstraints = new ArrayList();
			m_clConstraints.AddRange(m_implicitConstraints);
					
			foreach (Constraint c in m_constraints)
			{
				m_clConstraints.Add(c.Value);
			}

			if (m_box != null)
			{
				m_clConstraints.AddRange(m_box.Constraints);
			}
		}

		public ClConstraint[] GetConstraintsConcerning(Part[] parts)
		{
			ArrayList constraints = new ArrayList();

			foreach (ClConstraint c in Constraints)
			{
				bool match = false;
				string matched = "";

				foreach (ClVariable v in c.Expression.Terms.Keys)
				{
					foreach (Part p in parts)
					{
						string vPartName = v.Name.Substring(0, v.Name.IndexOf("."));
						if (p.Identifier == vPartName)
						{
							if (!match)
							{
								matched = vPartName;
								match = true;
							}
							else
							{
								if (matched != vPartName)
								{
									// second, different match, these are interconnecting constraints
									constraints.Add(c);
								}
							}
						}
					}
				}
			}

			return (ClConstraint[]) constraints.ToArray(typeof(Cassowary.ClConstraint));
		}

		public string PartName
		{
			get { return Container.Identifier; }
		}

		public Part Container
		{
			get { return m_container; }
		}

		public Hashtable Properties
		{
			get { return m_properties; }
		}

		public ArrayList Constraints 
		{
			get 
			{ 
				if (m_clConstraints == null)
				{
					BuildConstraints();
				}
				
				return m_clConstraints;
			}
		}			

		public ArrayList Children
		{
			get { return m_constraints; }
		}

		public const string IAM		= "layout";
		public const string PART_NAME	= "part-name";
	}
}
