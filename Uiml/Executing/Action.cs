/*
  	 Uiml.Net: a Uiml.Net renderer (http://lumumba.luc.ac.be/kris/research/uiml.net/)
   
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

namespace Uiml.Executing
{
	using Uiml;
	using Uiml.Rendering;

	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	
	public class Action : IExecutable, IUimlElement
	{
		private Part m_partTree;
		private ArrayList m_subActions;
		private Event m_event = null;
		private IRenderer       m_renderer;
		private IPropertySetter m_propSetter;
		
		public Action()
		{
			m_subActions = new ArrayList();
		}

		public Action(XmlNode xmlNode, Part partTop) : this()
		{
			m_partTree = partTop;
			Process(xmlNode);
		}

		public void Process(XmlNode n)
		{
			if(n.Name == ACTION)
			{
				if(n.HasChildNodes)
				{
					XmlNodeList xnl = n.ChildNodes;
					for(int i=0; i<xnl.Count; i++)
					{
						if(xnl[i].Name == CALL)
							m_subActions.Add(new Call(xnl[i], m_partTree));
						else if(xnl[i].Name == PROPERTY)
							m_subActions.Add(new Uiml.Property(xnl[i]));
					}
				}
			}
		}

		public System.Object Execute()
		{
			foreach(System.Object o in m_subActions)
			{
				if(o is Uiml.Executing.Call)
					((Call)o).Execute(m_renderer);
				else if(o is Uiml.Property)					
					m_propSetter.ApplyProperty(m_partTree, (Property)o);
			}
			return null;
		}

		public System.Object Execute(IRenderer renderer)
		{
			m_renderer   = renderer;
			m_propSetter = renderer.PropertySetter;
			return Execute(); 
		}

		public void AttachLogic(ArrayList logicDocs)
		{
			foreach(System.Object o in m_subActions)
			{
				if(o is Uiml.Executing.Call)
					((Call)o).AttachLogic(logicDocs);
				else if(o is Uiml.Property)
					((Property)o).AttachLogic(logicDocs);
			}
		}
		
		public void Connect(object o)
		{
			foreach(object c in m_subActions)
			{
				if(c is Uiml.Executing.Call)
					((Call)c).Connect(o);
			}
		}

		public ArrayList Children
		{
			get { return null; }
		}

		public const string ACTION   = "action";
		public const string PROPERTY = "property";
		public const string CALL     = "call";


	}

}
