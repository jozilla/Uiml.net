/*
	 uiml.net: a uiml.ney renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)
    
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
	using System.Xml;
	using System.Xml.XPath;
	using System.IO;

	public class BaseVocabulary : Vocabulary{

		public BaseVocabulary()
		{}

		public BaseVocabulary(string vocName) : base(vocName)
		{
			Load(vocName);
		}

		private void Load(string vocName)
		{
			//first try to load the string "as is". If this does not work
			//try to load the online vocabulary provided this exists
			try
			{
	   	   XPathDocument xp = new XPathDocument(vocName);
				UimlVocabulary =  xp.CreateNavigator();
			}
				catch(XmlException xe)
				{
					Console.WriteLine("Could not load {0} because of: ", vocName);
					Console.WriteLine(xe);
					Console.WriteLine("Trying other possible vocabulary locations...");
					try
					{
						UimlVocabulary = new XPathDocument(VOCABULARY_BASE + vocName + VOCABULARY_EXT).CreateNavigator();
					}
						catch(Exception e)
						{
							UimlVocabulary = new XPathDocument(VOCABULARY_BASE2 + vocName + VOCABULARY_EXT).CreateNavigator();
						}
				}
				catch(Exception e)
				{						
					Console.WriteLine(e);
				}
				
		}		


		public XPathNavigator UimlVocabulary
		{
			get { return m_vocabulary; }
			set { m_vocabulary = value; }
		}
			
	}

}

