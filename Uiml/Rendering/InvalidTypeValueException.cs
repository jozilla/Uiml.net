/*
	 Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)
    
	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU General Public License
	as published by the Free Software Foundation; either version 2
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/


namespace Uiml.Rendering{

	using System;

	public class InvalidTypeValueException : Exception
	{

		private string m_type, m_value;
		
		public InvalidTypeValueException(string type, string value) : base(value + " is not a valid value for the type " + type)
		{ 
			m_value = value;
			m_type = type;
		}


		public string Type
		{
			get{ return m_type; }
			set{ m_type = value; }
		}

		public string Value
		{
			get{ return m_value; }
			set{ m_value = value; }
		}
	}
}
