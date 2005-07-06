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
	GNU General Public License for more details.

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
	///Helper class that allows to insert a document in a container UIML document that 
	///contains placeholders
	///a placeholder is defined in the part tree as follows:
	///  &lt;part class="HBox" name="container" &gt;
	///  &lt;/part&gt; 
	///</summary>
	public class UimlContainerDocument 
	{
		private UimlDocument topDoc;
		private UimlDocument workingDoc;
	
		public UimlContainerDocument(UimlDocument doc)
		{
			topDoc = doc;
			//TODO: workingDoc = (UimlDocument)topDoc.Clone();
			workingDoc = topDoc;
		}
		
		public void Insert(string pattern, UimlDocument doc)
		{
			//TODO: maintain order of pattern while inserting
			//TODO: replace "<!-- pattern -->" with template
			 
			Uiml.Part parent = workingDoc.SearchPart(pattern);
			parent.AddChild( ((Structure)doc.UInterface.UStructure[0]).Top);
			ArrayList properties = ((Style)doc.UInterface.UStyle[0]).Children;
			IEnumerator enumProps = properties.GetEnumerator();
			while(enumProps.MoveNext())
			{
				((Style)workingDoc.UInterface.UStyle[0]).Children.Add((Property)enumProps.Current);
			}
		}
		
		public void Reset()
		{
			workingDoc = (UimlDocument)topDoc.Clone();		
		}
		
		public UimlDocument Document
		{
			get { return workingDoc; }		
		}
	} 
			
}