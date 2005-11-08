/*
   Uiml.Net: a .Net UIML renderer (http://research.edm.uhasselt.be/kris/research/uiml.net)

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
*/


namespace Uiml
{

	using System;
	using System.Collections;
	using System.Reflection;
	using Uiml.Rendering;
	using Uiml.Executing;
	using Uiml.Executing.Binding;
	using System.Xml;
	
	///<summary>
	///
	///</summary>
	public class UimlComposite : IUimlComponent {
		private Hashtable leaves;
		private UimlContainerDocument container; 
		
	
		public UimlComposite(UimlDocument acontainer)
		{
			container = new UimlContainerDocument(acontainer);
			leaves = new Hashtable();
		}
		
		public Hashtable CompChildren 
		{ 
			get{ return leaves; } 
		}
		
		public void Add(string pattern, IUimlComponent component)
		{
			leaves.Add(pattern, component);
		}
		
		public void Remove(IUimlComponent component)
		{
		}
		
		public UimlComposite Composite 
		{ 
			get { return this; }
		}
		
		public UimlDocument Aggregate()
		{			
			IDictionaryEnumerator enumDocs = CompChildren.GetEnumerator();
			UimlDocument subDoc = null;
			while(enumDocs.MoveNext())
			{
			
				IUimlComponent component = (IUimlComponent)enumDocs.Value;
				UimlComposite composite = component.Composite;				
				if(component.Composite != null)
					subDoc = component.Composite.Aggregate();
				else
					subDoc = (UimlDocument)component;
				container.Insert((String)enumDocs.Key, subDoc);
			}
						
			return container.Document;
		} 
	}
	
	
	
}
	
