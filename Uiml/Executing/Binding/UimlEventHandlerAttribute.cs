/*
 	Uiml.Net: a Uiml.Net renderer (http://lumumba.luc.ac.be/kris/research/uiml.net/)
   
	Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                    Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

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

	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/

namespace Uiml.Executing.Binding
{
	using System;
	using System.Collections;
	
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public class UimlEventHandlerAttribute : Attribute
	{
		private string m_event;
		private ArrayList m_args = null;			

		///  <param name="eventName">
		///   The event class we're interested in.
		///  </param>
		public UimlEventHandlerAttribute(string eventName)
		{
			m_event = eventName;
		}
	
		/*
		 * This constructor's signature used to be:
		 * 
		 * public UimlEventHandlerAttribute(string eventName, params object[] args)
		 * 
		 * There was a bug in Mono which caused the runtime to abort
		 * when I used the params object[] argument. When I changed it
		 * to params string[], everything worked fine. 
		 *
		 * Later it occured that params string[] was much more intuitive. 
		 * After all, part identifiers _are_ strings!
		 */
		
		/// <param name="eventName">
		///  The event class we're interested in.
		/// </param>
		/// <param name="args">
		///  A number of part identifiers. These represent the parts which
		///  we want to examine.
		/// </param>
		public UimlEventHandlerAttribute(string eventName, params string[] args)
		{
			m_event = eventName;
			m_args = new ArrayList(args);
		}

		public bool HasParams
		{
			get { return m_args != null; }
		}
		
		public string[] Params 
		{
			get 
			{
				if(HasParams)
					return (string[])m_args.ToArray(typeof(string)); 
				else
					return null;
			}
		}

		public string Event
		{
			get { return m_event; }
		}
	}
}

