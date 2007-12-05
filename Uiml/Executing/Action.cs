/*
  	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)
   
	 Copyright (C) 2003  Kris Luyten (kris.luyten@uhasselt.be)
	                     Expertise Centre for Digital Media (http://www.edm.uhasselt.be)
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
	using Uiml.Rendering;

	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	
	public class Action : IExecutable, IUimlElement
	{
		private Part            m_partTree;
		private ArrayList       m_subActions;
		private Event           m_event = null;
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

        public virtual object Clone()
        {
            Action clone = new Action();
            
            if(m_subActions != null)
            {
                clone.m_subActions = new ArrayList();
                for(int i = 0; i< m_subActions.Count; i++)
                {
                    IUimlElement tmp = (IUimlElement)m_subActions[i];
                    clone.m_subActions.Add(tmp.Clone());
                }
            }
            clone.m_renderer = m_renderer;
            clone.m_propSetter = m_propSetter;

            clone.PartTree = m_partTree;


            return clone;
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
						if (xnl[i].Name == CALL)
							m_subActions.Add(new Call(xnl[i], m_partTree));
						else if (xnl[i].Name == PROPERTY)
							m_subActions.Add(new Uiml.Property(xnl[i]));
                        else if (xnl[i].Name == EVENT)
                            m_subActions.Add(new Event(xnl[i]));
					}
				}
			}
		}

		public System.Object Execute()
		{
			foreach(System.Object o in m_subActions)
			{
				if (o is Uiml.Executing.Call)
					((Call)o).Execute(m_renderer);
				else if (o is Uiml.Property)					
					m_propSetter.ApplyProperty(m_partTree, (Property)o);
                else if (o is Event)
                    ((Event)o).Execute(m_renderer);
			}
			return null;
		}

		public System.Object Execute(IRenderer renderer)
		{
			m_renderer = renderer;
			m_propSetter = renderer.PropertySetter;
			return Execute(); 
		}

		public void AttachLogic(ArrayList logicDocs)
		{
			foreach(System.Object o in m_subActions)
			{
				if (o is Uiml.Executing.Call)
					((Call)o).AttachLogic(logicDocs);
				else if (o is Uiml.Property)
					((Property)o).AttachLogic(logicDocs);
			}
		}
		
		public void Connect(object o)
		{
			foreach(object c in m_subActions)
			{
				if(c is Uiml.Executing.Call)
					((Call)c).Connect(o);
				if(c is Uiml.Property)
				{
					Property p = (Property)c;
					if(p.Lazy)
						p.Connect(o);
				}
			}
		}

		public void Disconnect(object o)
		{
			foreach(object c in m_subActions)
			{
				if(c is Uiml.Executing.Call)
					((Call)c).Disconnect(o);
				if(c is Uiml.Property)
				{
					Property p = (Property)c;
					if(p.Lazy)
						p.Disconnect(o);
				}
			}

		}

		public ArrayList Children
		{
			get { return m_subActions; }
		}

        public Part PartTree
        {
            get
            {
                return m_partTree;
            }
            set
            {
                m_partTree = value;
                if(m_subActions != null)
                {
                    for(int i = 0; i <m_subActions.Count; i++)
                    {
                        if(m_subActions[i] is Call)
                        {
                            Call tmp = (Call)m_subActions[i];
                            tmp.PartTree = value;
                        }
                    }
                }
            }
        }

		public const string ACTION   = "action";
		public const string PROPERTY = "property";
		public const string CALL     = "call";
        public const string EVENT    = "event";


	}

}
