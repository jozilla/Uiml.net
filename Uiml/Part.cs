/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU General Public License
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

		private ArrayList m_children;
		private ArrayList m_properties;
		private string    m_class;

		///<summary>
		/// The (final) concrete interface object representing this part
		///</summary>
		private System.Object m_uiObject;

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
			if(SourceAvailable)
				switch(How)
				{ 
				  case REPLACE:
					 {
					 }
					 break;
				  case CASCADE:
					 {
					 }
					 break;
				  case UNION:
					 {
					 }
					 break;
				  default:
					 { //TODO: isolate this body in a separate private method 
						ProcessSubTree(n);
					 }
					 break;
				}
			else
				ProcessSubTree(n);
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
							m_children.Add(new Part(xnl[i]));
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

		public ArrayList Children
		{
			get { return m_children; }
		}

		///<summary>
		///Libglade style autoconnect funtion. Makes binding a declarative UI specification
		///with the object oriented approach much more feasible.
		///</summary>
		public void AutoConnect(Object o)
		{
			//get all On<Name><Event>() functions
		}

		
		 public event EventHandler Eventor;
		///<summary>
		///Java Listeners style support is added here by a delegate. The Object o
		///is interested in the events generated by this part and the events of its subtree
		///</summary>
		public void Connect(Object o)
		{
			//get a list of all the methods that are available on Object o;
			Type t = o.GetType();
			System.Reflection.MemberInfo[] mInfos = t.GetMembers();
			foreach(System.Reflection.MemberInfo mi in mInfos)
			{
				Console.WriteLine("Trying to connecting {0} to {1}", mi.Name, Identifier);
			}
			
			

			//invoke the same method on the children of this part, with the same object
			IEnumerator enumerator = Children.GetEnumerator();
			while(enumerator.MoveNext())
			{
				try { ((Part)(enumerator.Current)).Connect(o); }catch(Exception e){}
			}
			
			//each part should connect separately (and allow the 
			//redundancy). Otherwise restructure could change the event notification
			//behavior and we don't want that to happen!
		}

		protected virtual void OnEventor(EventArgs e)
		{
			if(Eventor!=null)
			 Eventor(this,e); 
		}



		public const string IAM = "part";
		public const string PART = "part";
		public const string CLASS = "class";
		public const string STYLE = "style";
		public const string CONTENT = "content";
		public const string BEHAVIOUR = "behaviour";
	}

}
