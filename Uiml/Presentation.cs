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
*/


namespace Uiml{

	using System;
	using System.Xml;
	using System.Collections;


	public class Presentation : UimlAttributes, IUimlElement{

		private Vocabulary m_voc;
		private string m_base;

		public Presentation()
		{ }


		public Presentation(XmlNode nTopPeer)
		{
			Process(nTopPeer);
		}

		public void Process(XmlNode n){//Exception toevoegen
			if(n.Name == IAM){
				base.ReadAttributes(n);
				XmlAttributeCollection attr = n.Attributes;
				if(attr.GetNamedItem(BASE) != null){
					//the presentation is loaded from an URI
					m_voc = new BaseVocabulary(attr.GetNamedItem(BASE).Value);
					m_base = attr.GetNamedItem(BASE).Value;
				}else if(attr.GetNamedItem(ID) != null){
					m_identifier = attr.GetNamedItem(ID).Value;
					//make a custom vocabulary for this presentation
					m_voc = new CustomVocabulary(m_identifier, n);
				}else if(attr.GetNamedItem(SOURCE) != null){
					m_source = attr.GetNamedItem(SOURCE).Value;
				}
			}
		}

		public bool Provides(string pattern)
		{
			return ( m_base.ToLower().IndexOf(pattern.ToLower()) > -1 );
		}

		///<value>
		///Gets and sets  the vocabulary attached in this presentation
		///</value>
		public Vocabulary UimlVocabulary
		{
			get { return m_voc; }
			set { m_voc = value; }
		}

		public ArrayList Children
		{
			get { return null; }
		}

		public const string IAM = "presentation";
		public const string BASE         = "base";
		public const string DEFCLASS		= "d=class";

	}

/*
	public class DefClass : IUimlProcessor{

		public const string DEFMETHOD   = "d-method";
		public const string DEFPROPERTY = "d-property";
		public const string DEFEVENT    = "event";
		public const string DEFLISTENER = "listener";
		public const string MAPSTO      = "maps-to";
		public const string MAPSTYPE	  = "maps-type"; 



		public DefClass(XmlNode n){
			Process(n);
		}


		protected void Process(XmlNode n){
			XmlAttributeCollection attr = n.Attributes;
	
		}


		private string method;
		public String Method{
			get
			{
				return method;
			}

			set
			{
				method=value;
			}
		}

		private string property;
		public String Property{
			get
			{
				return property;
			}

			set
			{
				property=value;
			}
		}
	}*/
}
