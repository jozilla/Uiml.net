/*
  	 Uiml.Net: a Uiml.Net renderer (http://lumumba.uhasselt.be/kris/research/uiml.net/)
   
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

namespace Uiml.Executing
{
	using Uiml;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	public class Equal :  IExecutable, IUimlElement
	{
        Part m_top = null;

		public Equal()
		{
		}

		public Equal(XmlNode xmlNode, Part partTop) : this()
		{
			Process(xmlNode);
            m_top = partTop;
		}

        public virtual object Clone()
        {
            Equal clone = new Equal();
            clone.m_top = m_top;

            return clone;
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

        public Part PartTree
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
            }
        }


	}
	
}
