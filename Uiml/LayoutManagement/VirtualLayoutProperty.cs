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
using Uiml.Rendering;
using Cassowary;

namespace Uiml.LayoutManagement
{
	/// <summary>
	/// This is a layout property whose value is <strong>not</strong> determined by the renderer. Can be used
	/// to implement properties that don't exist in widget sets, such as z-order.
	/// </summary>
	public class VirtualLayoutProperty : LayoutProperty
	{
		public VirtualLayoutProperty() : base()
		{}

		public VirtualLayoutProperty(XmlNode n) : base(n)
		{}
		
		public VirtualLayoutProperty(string partName, string name) : base(partName, name)
		{}

		///<summary>
		///Gets the <b>current</b> value of this property
		///</summary>
		public override object GetCurrentValue(IPropertySetter iPropSetter, Part p)
		{
			// ignore iPropSetter, just use it's current value
			return int.Parse((string) Value); // return int (this should in fact be determined dynamically)
		}

		public override bool IsVirtual
		{
			get { return true; }
		}
	}
}
