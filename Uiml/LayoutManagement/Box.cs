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
using Uiml;
using Cassowary;

namespace Uiml.LayoutManagement
{
	public class Box
	{
		public Box(string property, string beginProp, string endProp, string oppositeProp, string oppositeBegin, string oppositeEnd, Layout layout, uint spacing, bool homogeneous)
		{
			m_spacing = spacing;
			m_homogeneous = homogeneous;
			m_layout = layout;
			m_property = property;
			m_beginProp = beginProp;
			m_endProp = endProp;
			m_oppositeProp = oppositeProp;
			m_oppositeBegin = oppositeBegin;
			m_oppositeEnd = oppositeEnd;

			m_constraints = new ArrayList();
		}

		public Box(string property, string beginProp, string endProp, string oppositeProp, string oppositeBegin, string oppositeEnd, Layout layout) : 
		  this(property, beginProp, endProp, oppositeProp, oppositeBegin, oppositeEnd, layout, 0, false)
		{}

		public void BuildConstraints()
		{
			ClLinearExpression e = new ClLinearExpression();
			
			// child1.property + spacing + child2.property + spacing + ... = container.property
			foreach (Part child in m_layout.Container.Children)
			{
				ClVariable child_dimension = ((LayoutProperty) m_layout.Properties[child.Identifier + "." + m_property]).Variable;
				ClLinearExpression e_plus_spacing = Cl.Plus(e, new ClLinearExpression(Spacing));
				e = Cl.Plus(e_plus_spacing, child_dimension);
			}

			ClVariable container_dimension = ((LayoutProperty) m_layout.Properties[m_layout.Container.Identifier + "." + m_property]).Variable;
			m_constraints.Add(new ClLinearEquation(e, container_dimension));

			// children in order of dimension of each other
			Part oldChild = null;
			foreach (Part child in m_layout.Container.Children)
			{
				if (oldChild != null)
				{
					ClVariable oldChild_end = ((LayoutProperty) m_layout.Properties[oldChild.Identifier + "." + m_endProp]).Variable;
					ClVariable child_begin = ((LayoutProperty) m_layout.Properties[child.Identifier + "." + m_beginProp]).Variable;
					m_constraints.Add(new ClLinearInequality(Cl.Plus(oldChild_end, Spacing), Cl.LEQ, new ClLinearExpression(child_begin)));
				}
				oldChild = child;
			}

			if (Homogeneous)
			{
				oldChild = null;
				// all child widgets must have equal dimensions
				foreach (Part child in m_layout.Container.Children)
				{
					if (oldChild != null)
					{
						ClVariable oldChild_dimension = ((LayoutProperty) m_layout.Properties[oldChild.Identifier + "." + m_property]).Variable;
						ClVariable child_dimension = ((LayoutProperty) m_layout.Properties[child.Identifier + "." + m_property]).Variable;
						m_constraints.Add(new ClLinearEquation(oldChild_dimension, new ClLinearExpression(child_dimension)));
					}
					oldChild = child;
				}
			}
		}

		public uint Spacing
		{
			get { return m_spacing; }
			set { m_spacing = value; }
		}

		public bool Homogeneous
		{
			get { return m_homogeneous; }
			set { m_homogeneous = value; }
		}

		public ArrayList Constraints
		{
			get { return m_constraints; }
		}

		private uint m_spacing;
		private bool m_homogeneous;
		private Layout m_layout;
		private string m_property;
		private string m_beginProp;
		private string m_endProp;
		private string m_oppositeProp;
		private string m_oppositeBegin;
		private string m_oppositeEnd;
		private ArrayList m_constraints;
	}
}
