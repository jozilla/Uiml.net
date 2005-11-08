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


namespace Uiml{

	using System;

	public class TemplateAlreadyProcessedException : Exception
	{

		private String m_identifier;
		private Uri m_location = null;

		public TemplateAlreadyProcessedException(string id) : base(id)
		{ 
			Identifier = m_identifier;
		}

		public TemplateAlreadyProcessedException(string id, Uri location) : this(id)
		{ 
			m_location = location;
		}


		public override String ToString()
		{		
			String resultStr = "Template id \""+ Identifier +"\" already processed";
			if(m_location!=null)
				resultStr  += "\n\tlocation=\""+ m_location +"\"";
			return resultStr;
		}

		public String Identifier
		{
			get { return m_identifier;  }
			set { m_identifier = value; }
		}

		public Uri Location
		{
			get { return m_location;  }
			set { m_location = value; }
		}
	}
}
