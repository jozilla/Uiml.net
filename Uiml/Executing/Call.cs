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
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.Reflection;

	using Uiml.Rendering; //this is BAD, should make a separate Type Resolving subpackage?
	
	public class Call : IExecutable, IUimlElement
	{
		private ArrayList m_params;
		private string m_methodName;
		private string m_objectName;
		//private ITypeDecoder m_td;
		//private Vocabulary m_voc;
		//private IPropertySetter m_propertySetter;
		private IRenderer m_renderer;
		private Part m_partTree;

		private ArrayList m_logicDescriptions = null;


		public Call()
		{
			m_params = new ArrayList();
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

		public void AttachLogic(ArrayList logicDocs)
		{
			IEnumerator enumChildren = Children.GetEnumerator();
			while(enumChildren.MoveNext())
			{
				try{
				((Param)enumChildren.Current).AttachLogic(logicDocs);
				}catch(Exception e) { /* Never mind if this fails; it could be scripts */ }
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
			if(attr.GetNamedItem(NAME) != null)
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

		public Object ExecuteMethod(string concreteMethodName, Type objectType, Logic l)
		{
			Uiml.Param[] parameters;
			if(l == null)
				parameters = Renderer.Voc.GetMethodParams(ObjectName, MethodName);
			else
				parameters = l.GetMethodParams(ObjectName, MethodName);

			//convert the params to types
			Type[] tparamTypes = new Type[parameters.Length];
			int i=0;
			try
			{
				for(i=0; i<parameters.Length; i++)
				{
					tparamTypes[i] = Type.GetType(parameters[i].Type);
					int j = 0;
					while(tparamTypes[i] == null)	
						tparamTypes[i] = ((Assembly)ExternalLibraries.Instance.Assemblies[j++]).GetType(parameters[i].Type);
				}
			}
				catch(ArgumentOutOfRangeException aore)
				{
					Console.WriteLine("Can not resolve type {0} of parameter {1} while calling method {2}",parameters[i].Type ,i , Name);
					Console.WriteLine("Trying to continue without executing {0}...", Name);
					return null;
				}
				
			MethodInfo m = objectType.GetMethod(concreteMethodName, tparamTypes);
			System.Object[] args = new System.Object[tparamTypes.Length];

			for(int k=0; k<args.Length; k++)
			{
				String propValue = (string)((Uiml.Executing.Param)m_params[k]).Value(Renderer);
				args[k] = Renderer.Decoder.GetArg(propValue, tparamTypes[k]);
			}
			
			return m.Invoke(null, args); //static method
		}		

		public Object ExecuteProperty(string concreteMethodName, Type objectType)
		{
			PropertyInfo pi = objectType.GetProperty(concreteMethodName);
			return pi.GetValue(null, null);
		}

		public Object ExecuteField(string concreteMethodName, Type objectType)
		{
			FieldInfo fi = objectType.GetField(concreteMethodName);
			return fi.GetValue(null);
		}

		///<summary>
		/// This Execute function needs serious rewriting and debugging :-(, allthough... it seems to work
		/// It loads a method and its params from this call and executes it at runtime
		///</summary>
		public Object Execute()
		{
			if(Name == null || Name == "" || Name == ".")
			{ //it is an "internal script"
				ArrayList l = Children;
				Object script2;
				foreach(Object script in l)
				{
					script2 = script;
					((IExecutable)script).Execute();
				}
				//only return the return value of the last script
				return ((Script)script2).ReturnValue;
			}
			
			Type type = null;				
		
			try
			{
				//is the function available in the vocabulary or in one of the logic components?
				string concreteObjectName = null; 
				string concreteMethodName = null;
				
				Logic logic = null;
				bool foundMapping = false;
				if(m_logicDescriptions != null)
				{
					IEnumerator enumDocs = m_logicDescriptions.GetEnumerator();				
					while(enumDocs.MoveNext() && (!foundMapping))
					{
						try
						{
							logic = (Logic)enumDocs.Current;
							concreteObjectName = logic.MapsOnComponent(ObjectName);
							concreteMethodName = logic.GetMethodComponent(MethodName, ObjectName);
							foundMapping = true;
						}
							catch(MappingNotFoundException mnfe)
							{
								//Console.WriteLine("Mapping not found in Logic document \"{0}->{1}\"...", ObjectName, MethodName);
							}
					}
				}
			
				if(!foundMapping)
				{
					logic = null;
					concreteObjectName = Renderer.Voc.MapsOnComponent(ObjectName);				
					concreteMethodName = Renderer.Voc.GetMethodComponent(MethodName, ObjectName);
					foundMapping = true;
				}

				IEnumerator enumAssemblies = ExternalLibraries.Instance.LoadedAssemblies;
				while((enumAssemblies.MoveNext()) && (type==null))
				{
					Assembly a = (Assembly)enumAssemblies.Current;
					type = a.GetType(concreteObjectName, false, true);
				}

				if(type == null)
				{
					Console.WriteLine("Could not find type {0} ({1}) -- aborting call execution", concreteObjectName, ObjectName);
					return null;
				}

				try { return ExecuteMethod(concreteMethodName, type, logic); } 
				    catch(NullReferenceException nre1)//method failed, try property
				    { try { return ExecuteProperty(concreteMethodName, type); }
					        //property failed, try field
					       catch(NullReferenceException nre2)	{ return ExecuteField(concreteMethodName, type); 	}
				    }
			   }
				catch(MappingNotFoundException mnfe) 
				{
					Console.WriteLine("Vocabulary error: {0}", mnfe);
					return null;
				}
				catch(TargetInvocationException tie)
				{
					Console.WriteLine("Unable to execute call {0} on type {1}", Name, type);
					Console.WriteLine("Reason:{0}", tie);
					return null; //replace by exception???
				}
				catch(Exception e)
				{
					Console.WriteLine("Unable to execute call {0}", Name);
					Console.WriteLine("Reason:{0}", e);
					return null; //replace by exception???
				}
		}

		public Object Execute(IRenderer renderer)
		{
			Renderer = renderer;
			return Execute();
		}

		public void AddParam(Param p)
		{
			m_params.Add(p);
		}

		public string Name
		{
			get { return ObjectName + "." + MethodName; }
			set 
			{
				ObjectName = value.Substring(0,value.LastIndexOf('.'));
				MethodName = value.Substring(value.LastIndexOf('.')+1);
			}
		}
		
		public string MethodName
		{
			get { return m_methodName;  }
			set { m_methodName = value; }
		}

		public string ObjectName
		{
			get { return m_objectName;  }
			set { m_objectName = value; }
		}
/*
		public ITypeDecoder Decoder
		{
			get { return m_td;  }
			set { m_td = value; }
		}

		public Vocabulary Voc
		{
			get { return m_voc;  }
			set { m_voc = value; }
		}
*/
		public IRenderer Renderer
		{
			get { return m_renderer;  }
			set { m_renderer = value; }
		}
/*
		public IPropertySetter PropertySetter
		{
			get { return m_propertySetter;  }
			set { m_propertySetter = value; }
		}
*/
 
		public ArrayList Children
		{
			get { return m_params; }
		}

		public const string CALL  = "call";
		public const string PARAM = "param";
		public const string NAME  = "name";
		public const string SCRIPT  = "script";
	}
}

