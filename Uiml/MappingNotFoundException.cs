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


namespace Uiml{

	using System;

	public class MappingNotFoundException : Exception
	{

		private String m_from, m_to, m_searchingFor;

		public MappingNotFoundException(string query) : base(query)
		{ 
			Query = query;
		}

		public MappingNotFoundException(string query, string from, string to) : base(query)
		{
			From = from;
			To   = to;
			Query = query;
		}

		public override String ToString()
		{
			String resultStr = Query;
			if((To!=null)&&(From!=null))
				resultStr += "[" + From + " => " + To + "]";
			else
				resultStr += "[ incomplete mapping in vocabulary (To=\""+To+"\",From=\""+From+"\")]" ;
			return resultStr;
		}

		public String From
		{
			get { return m_from;  }
			set { m_from = value; }
		}

		public String To
		{
			get { return m_to;  }
			set { m_to = value; }
		}

		public String Query
		{
			get { return m_searchingFor;  }
			set { m_searchingFor = value; }
		}

	}
}
