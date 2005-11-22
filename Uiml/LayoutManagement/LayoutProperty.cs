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
using System.Xml;
using Cassowary;

namespace Uiml.LayoutManagement
{
	/// <summary>
	/// This is a property used as a variable in the spatial constraints, like "width" for example.
	/// </summary>
	public class LayoutProperty : Property
	{
		private ClVariable m_var = null;
		private double m_varValue = 0;

		public LayoutProperty() : base()
		{}

		public LayoutProperty(XmlNode n) : base(n)
		{}
		
		public LayoutProperty(string partName, string name) : base(partName, name, "")
		{}

		public override bool Equals(object obj)
		{
			if (obj is LayoutProperty)
			{
				LayoutProperty lp = (LayoutProperty) obj;
				return PartName == lp.PartName && Name == lp.Name;
			}

			return false;
		}

		public void InitializeVariable(double val)
		{
			m_var = new ClVariable(HashValue, val);
		}

		public override string ToString()
		{
			return string.Format("{0} = {1}", HashValue, Value);
		}

		public override System.Object Value
		{
			get 
			{
				if (m_var != null)
				{
					base.Value = String.Format("{0}", m_var.Value);
					return base.Value;  
				}
				else
				{
					return "0";
				}
			}
		}

		public ClVariable Variable 
		{
			get { return m_var; }
		}

		public string HashValue
		{
			get { return PartName + "." + Name; }
		}
	}
}
