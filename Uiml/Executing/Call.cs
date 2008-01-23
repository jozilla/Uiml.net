/*
  	 Uiml.Net: a Uiml .Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)
   
	 Copyright (C) 2005  Kris Luyten (kris.luyten@uhasselt.be)
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
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.Reflection;

	using Uiml.Rendering; 
	using Uiml.Peers; // needed for determining which type of caller is needed 
	using Uiml.Executing.Callers;
	
	/// <summary>
	/// This class represents a &lt;call&gt; tag.
	/// </summary>
	public class Call : IExecutable, IUimlElement
	{
		private string m_name = "";
		private string m_objectName = "";
		private string m_methodName = "";

		private ArrayList m_params;
		private IRenderer m_renderer;
		private Part m_partTree;
		private Hashtable m_outputParams;
		
		private ArrayList m_connObjects = null;
		private ArrayList m_logicDescriptions = null;

		private CallerFactory m_callerFac;
		private Caller m_caller = null;

		public Call()
		{
			m_params = new ArrayList();
			m_callerFac = new CallerFactory(this);
		}

		public Call(XmlNode n) : this()
		{
			Process(n);
		}

		public Call(XmlNode n, Part topPart) : this()
		{
			m_partTree = topPart;
			Process(n);
		}

        public virtual object Clone()
        {
            Call call = new Call();
            call.m_name = m_name;
            call.m_objectName = m_objectName;
            call.m_methodName = m_methodName;

            //Not cloneable objects
            call.m_renderer = m_renderer;
            call.m_callerFac = m_callerFac;
            call.m_caller = m_caller;
            call.m_outputParams = m_outputParams;

            //clone the logic descriptions
            if (m_logicDescriptions != null)
            {
                call.m_logicDescriptions = new ArrayList();
                for (int i = 0; i < m_logicDescriptions.Count; i++)
                {
                    Logic logic = (Logic)((Logic)m_logicDescriptions[i]).Clone();
                    call.m_logicDescriptions.Add(logic);
                }
            }            

            if(m_params != null)
            {
                call.m_params = new ArrayList();
                for(int i = 0; i < m_params.Count; i++)
                {
                    IUimlElement element = (IUimlElement) m_params[i];
                    call.m_params.Add(element.Clone());
                }
            }
            //we do not clone this (its external application logic)
            if(m_connObjects != null)
            {
                call.m_connObjects = new ArrayList();
                for(int i = 0; i < m_connObjects.Count; i++)
                {
                    call.m_connObjects.Add(m_connObjects[i]);
                }
            }

            call.PartTree = m_partTree;

            return call;

        }

		public void AttachLogic(ArrayList logicDocs)
		{
			IEnumerator enumChildren = Children.GetEnumerator();
			while(enumChildren.MoveNext())
			{
                //FIXME: Refactor this
				try{
				((Param)enumChildren.Current).AttachLogic(logicDocs);
				}catch(Exception) { /* Never mind if this fails; it could be scripts */ }
			}
			m_logicDescriptions = logicDocs;
		}

		//TODO: resolve concrete object name and concrete method name when this object
		//is being constructed from its UIML specification. This makes the "Execute" method faster!
		public void Process(XmlNode n)
		{
			if(n.Name != CALL)
				return;
		   
			XmlAttributeCollection attr = n.Attributes;
			
			if (attr.GetNamedItem(NAME) != null)
				 Name = attr.GetNamedItem(NAME).Value;

			if(n.HasChildNodes)	//is this property "set" by a sub-property?
			{
				XmlNodeList xnl = n.ChildNodes;
				for(int i=0; i<xnl.Count; i++)
				{
					if(xnl[i].Name == PARAM)
					{
						m_params.Add(new Executing.Param(xnl[i], m_partTree));
					}
					if(xnl[i].Name == SCRIPT) //this is not correct wrt to the UIML specification !!! But it is very cool to have it: inline code
					{
						m_params.Add(new Executing.Script(xnl[i], m_partTree));
					}
				}
			}
		}

        public XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(CALL);

            if (Name.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(NAME);
                attr.Value = Name;
                node.Attributes.Append(attr);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                IUimlElement element = (IUimlElement)Children[i];
                node.AppendChild(element.Serialize(doc));
            }

            return node;
        }

        ///<summary>
        ///Used by the Rule to indicate this call will use object o when part of a rule
        ///</summary>
        public void Connect(object o)
		{
			if(!Connected)
				m_connObjects = new ArrayList();
				
			m_connObjects.Add(o);
		}

		///<summary>
		///Used by the Rule to indicate the possibility to use object o to
		///execute this call is removed
		///</summary>
		public void Disconnect(object o)
		{
			if(Connected)
			{
				m_connObjects.Remove(o);
			}
		}

		public object Execute() 
		{	
			InitializeCaller();
			return Caller.Execute(out m_outputParams);
		}

		public object Execute(IRenderer renderer)
		{
			Renderer = renderer;
			return Execute();
		}

		public void AddParam(Param p)
		{
			m_params.Add(p);
		}

		protected void InitializeCaller()
		{
			if (Caller != null)
				return;

			Caller = m_callerFac.CreateCaller();
		}
		
		public bool Connected
		{
			get { return m_connObjects != null; }
		}
		
		public ArrayList ConnectedObjects
		{
			get { return m_connObjects; }
		}

		public Hashtable OutputParameters
		{
			get { return m_outputParams; }
		}
		
		public IRenderer Renderer
		{
			get { return m_renderer;  }
			set { m_renderer = value; }
		}

		public ArrayList Children
		{
			get { return m_params; }
		}

		public ArrayList Params
		{
			get { return m_params; }
		}

		public string Name
		{
			get { return m_name; }
			set 
			{ 
				m_name = value;
				ObjectName = Name.Substring(0, Name.LastIndexOf('.'));
				MethodName = Name.Substring(Name.LastIndexOf('.')+1);
			} 
		}
		
		public string MethodName
		{
			get { return m_methodName; }
			set { m_methodName = value; }
		}

		public string ObjectName
		{
			get { return m_objectName; }
			set { m_objectName = value; }
		}

		public Caller Caller
		{
			get { return m_caller; }
			set { m_caller = value; }
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
                if(m_params != null)
                {
                    for(int i = 0; i < m_params.Count; i++)
                    {
                        if(m_params[i] is Param)
                        {
                            Param param = (Param)m_params[i];
                            param.PartTree = value;
                        }
                    }
                }
            }
        }

		public const string CALL  = "call";
		public const string PARAM = "param";
		public const string NAME  = "name";
		public const string SCRIPT  = "script";
	}
}
