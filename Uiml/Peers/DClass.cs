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

namespace Uiml.Peers
{
	/// <summary>
	/// This class represents a &lt;d-class&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-class (d-method*, d-property*, event*, listener*)&gt;
	/// &lt;!ATTLIST d-class
	///	          id NMTOKEN #REQUIRED
	///	          source CDATA #IMPLIED
	///	          how (append|cascade|replace) "replace"
	///	          export (hidden|optional|required) "optional"
	///	          maps-to CDATA #REQUIRED
	///	          maps-type CDATA #REQUIRED
	///	          used-in-tag (event|listener|part) #REQUIRED&gt;
	/// </summary>
	public class DClass : UimlAttributes
	{
		protected string m_mapsTo;
		protected string m_mapsType;
		protected string m_usedInTag;

		public enum USED_IN_TAG_VALS { Event, Listener, Part };

		public string MapsTo
		{
			get { return m_mapsTo; }
			set { m_mapsTo = value;	}
		}

		public string MapsType
		{
			get { return m_mapsType; }
			set { m_mapsType = value; }
		}

		public string UsedInTag
		{
			get { return m_usedInTag; }
			set { m_usedInTag = value; }
		}
	}
}
