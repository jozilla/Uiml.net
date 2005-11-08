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
	using System.Collections; 

	///<summary>
	///</summary>
	public class TemplateRepository{
		private static TemplateRepository instance = null;
		private Hashtable templates;
		
		private TemplateRepository()
		{	
			templates = new Hashtable();
		}

		public static TemplateRepository Instance
		{
			get
			{
				if(instance==null)
					instance = new TemplateRepository();
				return instance;
			}
		}

		public void Register(Template t)
		{
			try{
//				if(templates.Contains(t.Identifier))
//					Console.WriteLine("Ignoring duplicate entry of template {1}.", t.Identifier);
//				else
					templates.Add(t.Identifier, t);
			}catch(ArgumentNullException ane){
				Console.WriteLine("Template with no identifier!");
			}catch(ArgumentException ae){
				Console.WriteLine("Ignoring duplicate entry of template '{0}' in template repository", t.Identifier);
			}		
		}


		public Template Query(string id)
		{
			if (id.StartsWith("#") == false)
				return (Template) templates[id];
			else
				return (Template) templates[id.Substring(1)];
		}
		
		
	}
}
