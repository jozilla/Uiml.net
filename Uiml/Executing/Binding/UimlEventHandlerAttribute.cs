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

		public UimlEventHandlerAttribute(string eventName)
		{
			m_event = eventName;
		}
		
		public UimlEventHandlerAttribute(string eventName, params object[] args)
		{
			m_event = eventName;
			m_args = new ArrayList(args);
		}

		public UimlEventHandlerAttribute(params object[] args)
		{
			m_args = new ArrayList(args);
		}

		public bool HasParams
		{
			get { return m_args != null; }
		}
		
		public object[] Params 
		{
			get 
			{
				if(m_args != null)
					return m_args.ToArray(); 
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
