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

	using Uiml.Rendering;

	
	public class Condition : IExecutable, IUimlElement
	{
		private string m_conditionType;
		private System.Object m_conditionObject;
		private ArrayList m_actions;
		private Part m_partTree;
		private IRenderer m_renderer;
		
		public Condition()
		{
			m_actions = new ArrayList();
		}

		public Condition(XmlNode xmlNode, Part partTop) : this()
		{
			m_partTree = partTop;
			Process(xmlNode);
		}

		//for events:
		public delegate System.Object EventNotifier();

		public void Process(XmlNode n)
		{
			if(n.Name == CONDITION)
			{
				if(n.HasChildNodes)
				{
					XmlNodeList xnl = n.ChildNodes;
					switch(xnl[0].Name)
					{
						case EVENT:
							ConditionType = EVENT;
							m_conditionObject = new Event(xnl[0]);
							break;
						case OPERATOR:
							ConditionType = OPERATOR;
							m_conditionObject = new Op(xnl[0], m_partTree);
							break;
						case EQUAL:
							ConditionType = EQUAL;
							m_conditionObject = new Equal(xnl[0], m_partTree);
							break;
					}
				}
			}
		}


		public System.Object Execute()
		{
			if(CheckCondition())
			{
				IEnumerator enumActions = m_actions.GetEnumerator();
				while(enumActions.MoveNext())
				{
					Action a = (Action)enumActions.Current;
					a.Execute(m_renderer);
				}
			}
			return null;
		}

		///<summary>
		///TODO TODO TODO
		///</summary>
		private bool CheckCondition()
		{
			return true;
		}

		public System.Object Execute(IRenderer renderer)
		{
			m_renderer = renderer;
			return Execute();
		}

		public void Execute(System.Object o, EventArgs a)
		{
		}

		public void Attach(Action a)
		{
			m_actions.Add(a);
		}

		public string ConditionType
		{
			get { return m_conditionType;  }
			set { m_conditionType = value; }
		}

		///<summary>
		///  Returns all the events this condition specifies.
		///</summary>
		public IEnumerator Events
		{
			get 			
			{
				return GetEvents().GetEnumerator();
			}
		}

		public ArrayList GetEvents()
		{
			ArrayList l = new ArrayList();
			GetEvents(ref l);
			return l;
		}

		private void GetEvents(ref ArrayList l)
		{
			if(m_conditionObject is Event)
				l.Add(m_conditionObject);
			else if(m_conditionObject is Op)
				((Op)m_conditionObject).GetEvents(l);
			else if(m_conditionObject is Equal)
				((Equal)m_conditionObject).GetEvents(l);
		}

		public ArrayList Children
		{
			get { return null; }
		}

		public const string CONDITION = "condition";
		public const string EQUAL     = "equal";
		public const string OPERATOR  = "op";
		public const string EVENT     = "event";
	}
}
