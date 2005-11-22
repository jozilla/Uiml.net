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
using Cassowary;
using Uiml;

namespace Uiml.LayoutManagement
{
	public class SWFReorderer : Reorderer
	{
		public SWFReorderer(Part top) : base(top)
		{}

		protected void RemoveRedundantConstraints(Part top, Part[] parts)
		{
			ArrayList alParts = new ArrayList(parts);
			alParts.Add(top);
			Part[] newPartArray = (Part[]) alParts.ToArray(Type.GetType("Uiml.Part"));
			// remove all constraints between parts in different tabs
			// and between parts and top
			foreach (Layout l in top.ULayout)
			{
				ClConstraint[] toBeRemoved = l.GetConstraintsConcerning(newPartArray);

				foreach (ClConstraint c in toBeRemoved)
				{
					l.RemoveConstraint(c);
				}
			}
		}

		protected void AddLayoutProperties(Part top, Part p, Part child)
		{
			foreach (Layout l in top.ULayout)
			{
				l.AddAndInitializeReorderedLayoutProperties(p, child);
			}
		}

		protected override void Reorder(Part top, Part[] tabs)
		{
			RemoveRedundantConstraints(top, tabs);

			// create tabControl
			Part tabControl = new Part(top.Identifier + "-reordered");
			tabControl.Class = TABCONTROL_CLASS;

			foreach (Part child in tabs)
			{
				// remove from top
				top.RemoveChild(child);

				// add to new tabPage
				Part tabPage = new Part(child.Identifier + "-tab");
				tabPage.Class = TABPAGE_CLASS;
				tabPage.AddChild(child);

				// set tabpage's text
				tabPage.AddProperty(new Property(tabPage.Identifier, TEXT_PROPERTY, child.Identifier));
				
				// add tabPage to tabControl
				tabControl.AddChild(tabPage);

				AddLayoutProperties(top, tabPage, child);

				// extra layout for tabPage:
				// the tab switcher at the top isn't counted in to the total height of the widget,
				// as well as the surrounding frame isn't counted in to the total width.
				Layout tabPageLayout = new Layout(tabPage);
				
				int heightPixs = 26;
				string heightCompensatorRule = string.Format("{0}.bottom >= {1}.bottom + {2}", tabPage.Identifier, child.Identifier, heightPixs);
				Constraint heightCompensator = new Constraint(tabPageLayout, heightCompensatorRule);
				tabPageLayout.AddConstraint(heightCompensator);

				int widthPixs = 8;
				string widthCompensatorRule = string.Format("{0}.right >= {1}.right + {2}", tabPage.Identifier, child.Identifier, widthPixs);
				Constraint widthCompensator = new Constraint(tabPageLayout, widthCompensatorRule);
				tabPageLayout.AddConstraint(widthCompensator);

				tabPage.AddLayout(tabPageLayout);
			}

			// add layout to tabControl
			tabControl.AddLayout(new Layout(tabControl));

			// add tabControl to top
			top.AddChild(tabControl);

			AddLayoutProperties(top, tabControl, top);
		}

		public const string TABCONTROL_CLASS  = "Tabs";
		public const string TABPAGE_CLASS     = "TabPage";
		public const string TEXT_PROPERTY     = "label";
	}
}
