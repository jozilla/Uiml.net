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
	public abstract class Reorderer
	{
		private Part m_top;
		
		public Reorderer(Part top)
		{
			m_top = top;
			Process();
		}

		public void Process()
		{
			Process(Top);
			//PrintDepthList(Top);
		}
		
		public void Execute()
		{
			Execute(Top);
		}

		private void Process(Part top)
		{
			// construct a list of children, sorted by depth
			foreach (Part p in top.Children)
			{
				Process(p);

				Hashtable depths = top.ComponentsByDepth;
				LayoutProperty vlp = (LayoutProperty) LayoutPropertyRepository.Instance.Get(p.Identifier + "." + DEPTH);
                int depth = int.Parse((string) vlp.Value);
				
				if (!depths.ContainsKey(depth))
					depths[depth] = new ArrayList();
				
				ArrayList depthComponents = (ArrayList) depths[depth];
				if (!depthComponents.Contains(p))
					depthComponents.Add(p);
			}
		}

		/// <summary>
		/// Executes the reordering.
		/// </summary>
		public void Execute(Part top)
		{
			//Console.Write("Reordering " + top.Identifier + "... ");
			Hashtable depths = top.ComponentsByDepth;
			
			if (depths.Count > 1)
			{
				Console.WriteLine();
				// two or more depth levels -> let's split
				
				// get all parts that have to be put in tabs, sorted by depth
				ArrayList tabs = new ArrayList(); 
				ArrayList keys = new ArrayList(depths.Keys);
				keys.Sort();
				foreach (int k in keys)
				{
					Console.WriteLine("Processing depth: " + k);
					ArrayList components = (ArrayList) depths[k];
					Part p = (Part) components[0]; // TODO: support multiple parts!
					Console.WriteLine("Adding part [{0}]...", p.Identifier);
					tabs.Add(p);
				}

				// convert this ArrayList to a Part array
				Part[] tabArray = (Part[]) tabs.ToArray(typeof(Uiml.Part));
				
				// put them in tabs
				Reorder(top, tabArray);
			}
			else
			{
				//Console.WriteLine("Nothing to be done!");
			}

			// go deeper in the part tree
			foreach (Part p in top.Children)
			{
				Execute(p);
			}
		}

		protected abstract void Reorder(Part top, Part[] tabs);

		private void PrintDepthList(Part top)
		{
			Console.WriteLine("Part [{0}]:", top.Identifier);
			Hashtable h = top.ComponentsByDepth;
			PrintHashtable(h);
			
			foreach (Part p in top.Children)
			{
				PrintDepthList(p);
			}

			Console.WriteLine(" -------- ");
		}

		private void PrintHashtable(Hashtable h)
		{
			ArrayList keys = new ArrayList(h.Keys);
			keys.Sort();
			
			for (int i = 0; i < keys.Count; i++)  
			{
				Console.Write("Depth " + keys[i] + " ");
				ArrayList al = (ArrayList) h[keys[i]];
				Console.Write("[{0} items]", al.Count);
				Console.Write(": { ");

				foreach (Part p in al)
				{
					Console.Write(p.Identifier + " ");
				}

				Console.WriteLine("}");
      }
		}

		public Part Top
		{
			get { return m_top; }
		}

		public const string DEPTH = "depth";
	}
}
