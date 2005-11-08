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

namespace Uiml.Peers
{

	using System;
	using System.Xml;
	using System.IO;

	public class CustomVocabulary : Vocabulary {

		public CustomVocabulary()
		{}

		public CustomVocabulary(string idName, XmlNode subDoc )
		{
			Load(idName, subDoc);
		}

		private void Load(string idName, XmlNode subDoc)
		{
			//TODO			
		}

		public string QueryMapsTo(string name)
		{
			//TODO
			return "dummy";
		}

		public const string MAPSTO = "maps-to";
			
	}

}
