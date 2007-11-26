/*
    Uiml.Net: a .Net UIML renderer (http://research.edm.uhasselt.be/kris/research/uiml.net)

	 Copyright (C) 2004  Kris Luyten (kris.luyten@uhasselt.be)
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

namespace Uiml{

	using System.Collections;
	using System.Xml;

	abstract class UimlElement :  IUimlElement
	{
		public abstract void Process(XmlNode node);

		public abstract ArrayList Children { get; }
        public abstract object Clone();

		public void ApplyTemplate()
		{
		}

		protected void PostProcess(string source, string export)
		{
		}

		public IUimlElement GetChild(int i)
		{
			if(Children.Count > i)
			{
				return (IUimlElement)Children[i];
			}
			else
				return null; //not found
		}

		public void SetChild(int i, IUimlElement child)
		{
			if(Children.Count > i)
			{
				Children[i] = child;
			}	
		}
	}
}

