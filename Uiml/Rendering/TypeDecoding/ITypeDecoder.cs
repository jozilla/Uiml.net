/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.uhasselt.be/kris/research/uiml.net)
                                                                                                
    Copyright (C) 2003  Kris Luyten (kris.luyten@uhasselt.be)
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
                                       	                                                         
namespace Uiml.Rendering
{
	using System;
	
	using Uiml;
	

	public interface ITypeDecoder
	{
		/// <summary>
	    /// Returns the value of a property converted to the correct type.
	    /// This function is used when a property maps onto a .NET property.
	    /// <returns>The property's value</returns>
	    /// </summary>
		object GetArg(object o, Type t);

	    /// <summary>
	    /// Returns the value of a property converted to the correct types. 
	    /// This function is used when a property maps onto a method.
	    /// <returns>The property's value</returns>
	    /// </summary>
		object[] GetArgs(Property p, Type[] types);
		
		/// <summary>
		/// Convenience function to get the converted values of multiple 
		/// properties at once. 
		/// <returns>An array with the values of each property</returns>
		/// </summary>
		object[] GetMultipleArgs(Property[] p, Type[] types);
	}
}
