/*
  	 Uiml.Net: a Uiml.Net renderer (http://lumumba.luc.ac.be/kris/research/uiml.net/)
   
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

namespace Uiml.Executing
{
	using Uiml;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	public class Equal :  IExecutable, IUimlElement
	{

		public Equal()
		{
		}

		public Equal(XmlNode xmlNode, Part partTop) : this()
		{
			Process(xmlNode);
		}

		public void Process(XmlNode n)
		{
		}

		public void GetEvents(ArrayList al)
		{
		}

		public Object Execute()
		{
			return null;
		}

		public Object Execute(Uiml.Rendering.IRenderer renderer)
		{
			return Execute();
		}

		public ArrayList Children
		{
			get { return null; }
		}


	}
	
}
