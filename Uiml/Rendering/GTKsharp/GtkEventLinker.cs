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

namespace Uiml.Rendering.GTKsharp
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Gtk;
	using GtkSharp;

	using Uiml;
	using Uiml.Executing;
	using Uiml.Rendering;
	

	///<summary>
	///Links the events from the concrete widget set with the behavior specified in a UIML document
	///</summary>
	public class GtkEventLinker
	{
		private Structure m_uiStruct;
		private Behavior  m_uiBehavior;
		private IRenderer m_renderer;
		

		public GtkEventLinker(IRenderer renderer)
		{
			m_renderer = renderer;
		}


		///<summary>
		///Links the different rules in the behavior with the parts of the structure. 
		///This method uses link(Rule, Part) for each rule that is available in the behavior section.
		///</summary>
		public void Link(Structure uiStruct, Behavior uiBehavior)
		{
			if((uiBehavior == null)||(uiStruct == null))
				return;

			m_uiStruct   = uiStruct;
			m_uiBehavior = uiBehavior;

			Part topPart = uiStruct.Top;

			IEnumerator enumRules = m_uiBehavior.Rules;
			while(enumRules.MoveNext())
			{
				Rule r = (Rule)enumRules.Current;
				link(r, topPart);
			}
		}

		protected IRenderer Renderer
		{
			get { return m_renderer; }
		}


		virtual protected void link(Rule r, Part p)
		{
	   	link(r,p,new GtkEventLink(r.Condition, m_renderer));
		}

		///<summary>
		///Searches the parts that are used in the condition of Rule r. Events of a certain type emitted
		///by thow parts are connected to the condition of Rule r so it can be evaluated if such an event
		///occurs
		///</summary>
		///<param name="r">The rule with a condition and action</param>
		///<param name="p">A part where p itself or one of it subparts will emit events that are of interest
		///for the condition of Rule r</param>
		///<param name="gel">Links the renderer with the condition so appropriate queries can be executed by the condition.
		///It also allows the action to use the renderer afterwards to change properties in the user interface at runtime</param>

		protected void link(Rule r, Part p, GtkEventLink gel)
		{
			IEnumerator eventsEnum = r.Condition.GetEvents().GetEnumerator();			
			while(eventsEnum.MoveNext())
			{
				Event e = (Event)eventsEnum.Current;
				Part thePart = p.SearchPart(e.PartName);
				if(thePart == null)
				{
					if(e.PartName == "")
						Console.WriteLine("Error in behavior specification: no part name given for {0}", e.Class);
					else
						Console.WriteLine("Error in behavior specification: {0} does not exist for event {0}", e.PartName, e.Class);
					return;
				}
				Widget w = (Widget)thePart.UiObject;

				string concreteEventName = m_renderer.Voc.MapsOnCls(e.Class);
				/* TODO: eventhandling is done with an ugly hack: while the uiml vocabulary
				 * describes all sorts of Event delegates, only EventHandler is used internally!
				 * The Types declared in the vocabulary ar used to facilitate searching the correct mappings!
				//get the event out of the vocabulary!
				Console.WriteLine("Event links with: {0}", concreteEventName);				
				Type delegateType = null;//load the concreteEventName delegate dynamically
				ArrayList loadedAssemblies = m_renderer.LoadedAssemblies;
				IEnumerator enumAssemblies = loadedAssemblies.GetEnumerator();
				
				while((enumAssemblies.MoveNext()) && (delegateType==null))
				{
					Assembly a = (Assembly)enumAssemblies.Current;
					delegateType = a.GetType(concreteEventName, false, true);
				}
				*/
				
					try
					{
						Delegate handler = Delegate.CreateDelegate(typeof(EventHandler), gel, EXECUTE_METHOD);
						string eventId = m_renderer.Voc.GetEventFor(thePart.Class,concreteEventName);

						//Sometimes eventId is a composed event:
						//it is an event of a property of widget w
						//so first load the property of w, and then link with
						//the event of this property
						//<property>.<event>
						
						Type theType = w.GetType();
						int j = eventId.IndexOf('.');
						System.Object targetObject = thePart.UiObject;
						while(j!=-1)
						{
							String parentType = eventId.Substring(0,j);
							eventId=eventId.Substring(j+1,eventId.Length-j-1);
							PropertyInfo pInfo = theType.GetProperty(parentType);
							theType = pInfo.PropertyType;
							targetObject = pInfo.GetValue(targetObject, null);
							j = eventId.IndexOf('.');
						}
						EventInfo eInfo = theType.GetEvent(eventId);
						//load the event info as provided by the mappings of Widget w
						eInfo.AddEventHandler(targetObject, handler);
					}
						catch(ArgumentException ae)
						{
							Console.WriteLine("Invalid argument: {0}", ae);
							Console.WriteLine("Caused by: {0}", ae.ParamName);
						}
						catch(Exception be)
						{
							Console.WriteLine("Unexpected failure:");
							Console.WriteLine(be);
							Console.WriteLine("Please contact the uiml.net maintainer with the above output.");
						}
			}
		}

		private const string EXECUTE_METHOD = "Execute";
	}
}

