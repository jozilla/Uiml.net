/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.uhasselt.be/kris/research/uiml.net)

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

using System;

namespace Uiml{


	/// <summary>
	/// This class resolves a template using the ``cascade'' method
	/// </summary>
	public class CascadeTemplateResolver : ITemplateResolver {

		public CascadeTemplateResolver()
		{}

		public virtual IUimlElement Resolve(Template t, IUimlElement placeholder)
		{
			// add all unique children of the template's top element to placeholder's children
			Console.Write("Trying to unite element '{0}' with template '{1}'... ", ((UimlAttributes) placeholder).Identifier, t.Identifier);

			try
			{
				// check if types are compatible
				if (t.Top.GetType().Equals(placeholder.GetType()))
				{
					// TODO: only add unique children !!!
					placeholder.Children.AddRange(t.Top.Children);
					Console.WriteLine("OK!");
				}
				else
					Console.WriteLine("Failed! -> incompatible types, no action taken");
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed!");
			}
		
			return placeholder; // always return placeholder, regardless of modifications
		}
	}
}
