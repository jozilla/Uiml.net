/*
    Uiml.Net: a .Net UIML renderer (http://reseaech.edm.luc.ac.be/kris/research/uiml.net)

	 Copyright (C) 2004  Kris Luyten (kris.luyten@luc.ac.be)
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


namespace Uiml {

	using System;
	using System.Xml;
	using System.Reflection;
	using System.Collections; 
	
	using Uiml.Executing.Binding;
	
	///<summary>
	/// Part, p.34 of UIML 3.0 spec
	/// Represents one instance of a widget or nothing.
	/// Can be nested to obtain hierarchical relationship.
	///
	/// &lt;!ELEMENT part (style?, content?, behavior?, part*, repeat*)&gt;
	/// &lt;!ATTLIST part 
	///                 id NMTOKEN #IMPLIED 
	///                 class NMTOKEN #IMPLIED 
	///                 source CDATA #IMPLIED 
	///                 where (first|last|before|after) "last" 
	///                 where-part NMTOKEN #IMPLIED 
	///                 how (union|cascade|replace) "replace" 
	///                 export (hidden|optional|required) "optional"&gt;
	///</summary>
	public class Part : UimlAttributes, IUimlElement
	{
		#if COMPACT
		public class Connection
		{
			private object m_obj;
			private MethodInfo m_method;
			
			public Connection()
			{}

			public Connection(object o, MethodInfo mi)
			{
				m_obj = o;
				m_method = mi;
			}

			public MethodInfo Method
			{
				get { return m_method; }
				set { m_method = value; }
			}

			public object Object
			{
				get { return m_obj; }
				set { m_obj = value; }
			}
		}
		#endif

		private ArrayList m_children;
		private ArrayList m_properties;
		private string    m_class;
		private Part		parent = null;

		///<summary>
		/// The (final) concrete interface object representing this part
		///</summary>
		private System.Object m_uiObject;
		
		/// <summary>
		///  This member is used to signal an event to the application logic. It is
		///  initialized in the Connect method. 
		/// </summary>
		private UimlEventArgs m_eventArgs = null;

		#if COMPACT
		private ArrayList m_connections = null;
		#endif

		public Part()
		{
			m_children   = new ArrayList();
			m_properties = new ArrayList();
		}

		public Part(string identifier) : this()
		{
			Identifier = identifier;
		}

		public Part(XmlNode n) : this()
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;

			ReadAttributes(n);
			ProcessSubTree(n);

			if(SourceAvailable)
			{
				ITemplateResolver templateResolver = Template.GetResolver(How);
				Template t = TemplateRepository.Instance.Query(Source);				
				templateResolver.Resolve(t,this);
			}
		}

		private void ProcessSubTree(XmlNode n)
		{
			XmlAttributeCollection attr = n.Attributes;
			if(attr.GetNamedItem(ID) != null)
				Identifier = attr.GetNamedItem(ID).Value;
			if(attr.GetNamedItem(CLASS)!=null)
				Class = attr.GetNamedItem(CLASS).Value;
			if(n.HasChildNodes)
			{
				XmlNodeList xnl = n.ChildNodes;
				for(int i=0; i<xnl.Count; i++)
				{						
					switch(xnl[i].Name)
					{
						case STYLE:
							ProcessDirectProperties(xnl[i]);
							break;
						case BEHAVIOUR:
							m_children.Add(new Behavior(xnl[i], this));
							break;
						case CONTENT:
							m_children.Add(new Content(xnl[i]));
							break;
						case IAM:
							Part p = new Part(xnl[i]);
							p.Parent = this;
							m_children.Add(p);
							break;
					}
				}
			}
		}

		private void ProcessDirectProperties(XmlNode n)
		{
			if(n.HasChildNodes)
			{
					XmlNodeList xnl = n.ChildNodes;
					for(int i=0; i<xnl.Count; i++)
					{
						if(xnl[i].Name == Property.IAM)
							m_properties.Add(new Property(xnl[i]));
					}
			}
		}

		public void AddChild(Part p)
		{
			if(m_children == null)
				m_children = new ArrayList();
			m_children.Add(p);
		}

		public Part Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public bool Top
		{
			get { return (parent==null); }
		}

		public bool HasSubParts()
		{
			return (m_children.Count != 0);
		}

		public IEnumerator GetSubParts()
		{
			return m_children.GetEnumerator();
		}


		///<value>
		/// Sets and gets the class of this part. This class identifier still
		/// has to be mapped onto a specific class using a vocabulary
		///</value>
		///<remarks>
		///
		///</remarks>
		public String Class
		{
			get { return m_class; }
			set { m_class = value; }
		}


		public System.Object UiObject
		{
			get { return m_uiObject; }
			set { m_uiObject = value; }
		}

		public IEnumerator Properties
		{
			get { return m_properties.GetEnumerator(); }
		}

		public Part SearchPart(string checkIdentifier)
		{
			if(checkIdentifier == Identifier)
				return this;
			else if(m_children.Count == 0)
					return null;
			else
			{
				IEnumerator enumParts = GetSubParts();
				while(enumParts.MoveNext())
				{
					Part p = ((Part)enumParts.Current).SearchPart(checkIdentifier);
					if(p!=null)
						return p;
				}
			}
			//not found...
			return null;
		}
		
		public Property GetProperty(string name)
		{
			IEnumerator enumProps = Properties;
			while(enumProps.MoveNext())
			{
				Property prop = (Property)enumProps.Current;
				if(prop.Name == name)
					return prop;
			}

			// not found
			return null;
		}

		#if COMPACT
		public bool Connected
		{
			get	{ return m_connections != null; }
		}
		#endif
		
		public ArrayList Children
		{
			get { return m_children; }
		}


		
		public delegate void UimlEventHandler(Part sender, UimlEventArgs e);
		public event UimlEventHandler Signal;

		///<summary>
		///Java Listeners style support is added here by a delegate. The Object o
		///is interested in the events generated by this part and the events of its subtree
		///</summary>
		public void Connect(object o, UimlDocument doc)
		{
			Peers.Vocabulary voc = doc.SearchPeers(doc.Vocabulary).GetVocabulary();
			// get a list of all the methods that are available on object o.
			Type t = o.GetType();
			MethodInfo[] mtds = t.GetMethods();
			foreach(MethodInfo mi in mtds)
			{
				UimlEventHandlerAttribute eHandler = GetEventHandler(mi);
				if(eHandler != null)
				{
					Console.WriteLine("Found UimlEventHandler [{0}]", mi.Name);			
					Console.WriteLine("\tConnecting to \"{0}\" of type <{1}> ", Identifier, Class);
					
					initEventArgs(eHandler);
					
					#if !COMPACT
					Delegate handler = Delegate.CreateDelegate(typeof(EventHandler), this, "OnWidgetEvent");
					#else
					Delegate handler = new EventHandler(OnWidgetEvent);
					#endif

					// connect the Signal event to o's event handler
					#if !COMPACT
					UimlEventHandler ObjectHandler = (UimlEventHandler)Delegate.CreateDelegate(typeof(UimlEventHandler), o, mi.Name);
					Signal += ObjectHandler;
					#else
					/* This is done with a ugly hack in the .NET Compact framework:
					 * - we keep a list of MethodInfo objects
					 * - when the event should be signalled, we invoke these methods
					 */
					if(!Connected)
						m_connections = new ArrayList();

					m_connections.Add(new Connection(o, mi));
					#endif		
					
					try
					{
						// only handle events of the class eHandler.Event
						string concreteEventName = voc.MapsOnHandler(eHandler.Event);
						string eventId = voc.GetEventFor(Class, concreteEventName);
					
						//Sometimes eventId is a composed event:
						//it is an event of a property of widget w
						//so first load the property of w, and then link with
						//the event of this property
						//<property>.<event>
					
						Type theType = UiObject.GetType();
						int j = eventId.IndexOf('.');
						object targetObject = UiObject;
						while(j!=-1)
						{
							String parentType = eventId.Substring(0,j);
							eventId = eventId.Substring(j+1,eventId.Length-j-1);
							PropertyInfo pInfo = theType.GetProperty(parentType);
							theType = pInfo.PropertyType;
							targetObject = pInfo.GetValue(targetObject, null);
							j = eventId.IndexOf('.');
						}

						EventInfo eInfo = theType.GetEvent(eventId);
						//load the event info as provided by the mappings of Widget w
						eInfo.AddEventHandler(targetObject, handler);
					}
					catch(MappingNotFoundException e)
					{
						Console.WriteLine("\tFAILED");
					}
					catch(NullReferenceException nre)
					{
						Console.WriteLine(nre);
						Console.WriteLine("Make sure to connect the objects after calling Render!");
					}
				}
			}
			
			//invoke the same method on the children of this part, with the same object
			IEnumerator enumerator = Children.GetEnumerator();
			while(enumerator.MoveNext())
			{
				try 
				{ 
					Part child = ((Part)(enumerator.Current));
					child.m_eventArgs = m_eventArgs;
					child.Connect(o, doc); 
				}
				catch(MappingNotFoundException e)
				{
					//TODO
					//Console.WriteLine("Warning: could not connect to child \"{0}\"", ((Part)enumerator.Current).Identifier);
				}
			}

			//each part should connect separately (and allow the 
			//redundancy). Otherwise restructure could change the event notification
			//behavior and we don't want that to happen!
		}
		
		protected UimlEventHandlerAttribute GetEventHandler(MethodInfo mi)
		{
			object[] attrs = mi.GetCustomAttributes(true);
			ArrayList l = new ArrayList(); 
			
			foreach(Attribute a in attrs)
			{
				if(a is UimlEventHandlerAttribute)
				{
					// there can be only one event handler per method, so return it
					return (UimlEventHandlerAttribute)a;
				}
			}
			
			// none found
			return null;
		}

		protected void OnWidgetEvent(object sender, EventArgs e)
		{
			#if COMPACT
			/* we are always connected when this method is called
			 * this test could thus be eliminated
			 */
			if(Connected) 
			{
				IEnumerator ce = m_connections.GetEnumerator();
				while(ce.MoveNext())
				{
					Connection conn = (Connection)ce.Current;
					conn.Method.Invoke(conn.Object, new object[] {this, m_eventArgs});
				}
			}
			#else
			if(Signal!=null)
			 Signal(this, m_eventArgs); 
			#endif
			
		}

		protected void initEventArgs(UimlEventHandlerAttribute a)
		{
			if(m_eventArgs != null)
				return; // already set by parent

			if(a.Params == null)
				return;

			m_eventArgs = new UimlEventArgs();
			
			for(int i = 0; i < a.Params.Length; i++)
			{
				Part p = SearchPart(a.Params[i]);
				if(p != null)
					m_eventArgs.AddPart(p);
				else
					Console.WriteLine("Could not add Part [{0}] to the UimlEventArgs; part not found", a.Params[i]);
			}
		}

		public const string IAM = "part";
		public const string PART = "part";
		public const string CLASS = "class";
		public const string STYLE = "style";
		public const string CONTENT = "content";
		public const string BEHAVIOUR = "behaviour";
	}

}
