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
using Uiml.Rendering;

namespace Uiml.LayoutManagement
{
	/// <summary>
	/// Keeps a Hashtable of all layout properties, in order
	/// to prevent different layouts which refer to the same property, 
	/// from adding duplicate properties, causing the solver to fail.
	/// Furthermore it handles initializing the properties.
	/// </summary>
	/// <remarks>
	/// Implements the Singleton pattern. 
	/// <seealso cref="http://www.yoda.arachsys.com/csharp/singleton.html"/>
	/// </remarks>
	public sealed class LayoutPropertyRepository
	{
		private static readonly LayoutPropertyRepository instance = new LayoutPropertyRepository();
		private Hashtable m_properties;

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static LayoutPropertyRepository()
		{
		}

		private LayoutPropertyRepository()
		{
			m_properties = new Hashtable();
		}

		public LayoutProperty Add(LayoutProperty p)
		{
			try
			{
				m_properties.Add(p.HashValue, p);
				return p;
			}
			catch(ArgumentException)
			{
				// duplicate entry: fail silently and return existing property
				return (LayoutProperty) m_properties[p.HashValue];
			}
		}

		public void InitializeProperties(UimlDocument doc, IRenderer r)
		{
			AddPropertiesToParts(doc);

			foreach (LayoutProperty lp in m_properties.Values)
			{
				int renderedValue = (int) lp.GetCurrentValue(r.PropertySetter, null);

				// will be converted to string automatically by LayoutProperty class!
				lp.InitializeVariable((double) renderedValue);
			}
		}

		private void AddPropertiesToParts(UimlDocument doc)
		{
			ArrayList styles = doc.UInterface.UStyle;
			// TODO: support multiple structures
			Structure structure = (Structure) doc.UInterface.UStructure[0];

			foreach (LayoutProperty lp in m_properties.Values)
			{
				Part part = structure.SearchPart(lp.PartName);
				Property prop;
				
				// remove existing, absolute layout properties

				// check for inline properties
				prop = part.GetProperty(lp.Name);
				if (prop != null)
					part.RemoveProperty(prop);

				// check for properties in style section
				foreach (Style s in styles)
				{
					prop = s.SearchProperty(lp.PartName, lp.Name);
					if (prop != null)
						s.RemoveProperty(prop);
				}

				part.AddProperty(lp);
			}
		}

		public void PrintProperties()
		{
			SortedList sortedProps = new SortedList(m_properties);

			foreach (LayoutProperty p in sortedProps.Values)
			{
				Console.WriteLine(p);
			}
		}

		public bool Contains(LayoutProperty p)
		{
			return m_properties.ContainsKey(p.HashValue);
		}

		public bool Contains(string key)
		{
			return m_properties.ContainsKey(key);
		}
		
		public LayoutProperty Get(string key)
		{
			if (Contains(key))
				return (LayoutProperty) m_properties[key];
			else
				return null;
		}
		
		public IEnumerator GetEnumerator()
		{
			return m_properties.Values.GetEnumerator();
		}

		public static LayoutPropertyRepository Instance
		{
			get { return instance; }
		}
	}
}
