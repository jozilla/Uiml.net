/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)

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

using System;

namespace Uiml.net.Peers
{
	/// <summary>
	/// This class represents a &lt;d-property&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-property (d-method*,d-param*)&gt;
	/// &lt;!ATTLIST d-property
	///           id NMTOKEN #REQUIRED
	///           maps-type (attribute|getMethod|setMethod|method) #REQUIRED
	///           maps-to CDATA #REQUIRED
	///           return-type CDATA #IMPLIED&gt;
	/// </summary>
	public class DProperty
	{
		protected string m_identifier;
		protected string m_mapsType;
		protected string m_mapsTo;
		protected string m_returnType;

		public enum MAPS_TYPE_VALS { Attribute, GetMethod, SetMethod, Method };

		public string Identifier
		{
			get { return m_identifier;}
			set { m_identifier = value; }
		}

		public string MapsType
		{
			get { return m_mapsType; }
			set { m_mapsType = value; }
		}

		public string MapsTo
		{
			get { return m_mapsTo; }
			set { m_mapsTo = value;	}
		}

		public string ReturnType
		{
			get { return m_returnType;	}
			set { m_returnType = value;	}
		}
	}
}
