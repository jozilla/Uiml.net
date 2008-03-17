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


/*
TODO:
	- add support for property placeholders in scripts. E.g. like asp.net allows
	C#/vb.net code inside html
	- add support for result capturing, so results from scripts can be used inside the user interface
*/

namespace Uiml.Peers
{
	using Uiml;
	using Uiml.Executing;
	
	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Generic;
	using System.IO;

	using System.Reflection;
	#if !COMPACT
	using System.CodeDom.Compiler;
	#endif

	/// <summary>
	/// This class represents a &lt;script&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-param (#PCDATA)&gt;
	/// &lt;ATTLIST d-param
	///             id NMTOKEN #IMPLIED
	///             type CDATA #IMPLIED&gt;
	/// </summary>
	public class Script : UimlAttributes, IUimlElement
	{
		private String m_scriptSource;
		private String m_type;
		private bool m_preCompiled = false;
		private Assembly m_compiledAssemly;
		private Object m_retValue = null;

		public Script()
		{
		}

		public Script(XmlNode xmlNode, Part partTop) : this()
		{
			Process(xmlNode);
		}

		public Script(String scriptType, String scriptSource) :this()
		{
			ScriptSource = scriptSource;
			Type   = scriptType;
		}

		public Script(XmlNode n) : this()
		{
			//TODO this should be replaced by
			//"waiting for the parent Part" when the Script
			//element is more complete
			Process(n);
		}

        public virtual object Clone()
        {
            Script script = new Script();
            script.CopyAttributesFrom(this);
            script.m_scriptSource = m_scriptSource;
            script.m_type = m_type;
            script.m_preCompiled = m_preCompiled;
            script.m_compiledAssemly = m_compiledAssemly;
            script.m_retValue = m_retValue;

            return script;
        }

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;

            base.ReadAttributes(n);
			XmlAttributeCollection attr = n.Attributes;
			
			if(attr.GetNamedItem(TYPE) != null)
				Type = attr.GetNamedItem(TYPE).Value;
			else
				Console.WriteLine("Warning: " + IAM + " should have \"" + TYPE + "\" attribute!");
			ScriptSource = n.InnerText;
		}

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(IAM);
            List<XmlAttribute> attributes = CreateAttributes(doc);

            if (Type.Length > 0)
            {
                XmlAttribute attr = doc.CreateAttribute(TYPE);
                attr.Value = Type;
            }

            foreach (XmlAttribute attr in attributes)
            {
                node.Attributes.Append(attr);
            }

            node.InnerText = ScriptSource;

            return node;
        }

		public void GetEvents(ArrayList al)
		{
		}

		protected void PreCompile()
		{
			#if !COMPACT
			CodeDomProvider theProvider;
			switch(Type)
			{
				case "CSharp":
				case "C#":
				case "csharp":
				case "C Sharp":
				case "C sharp":
					theProvider = new Microsoft.CSharp.CSharpCodeProvider();
					break;
					//   case "JScript":
					//     theProvider = new Microsoft.JScript.JScriptCodeProvider();
					//     break;
				case "Visual Basic":
				case "VB":
				case "VB.Net":
				case "vb":
				case "vb.net":
				case "VB.NET":
					theProvider = new Microsoft.VisualBasic.VBCodeProvider();
					break;
				default:
					theProvider = new Microsoft.CSharp.CSharpCodeProvider();
					break;
			}
			ICodeCompiler theCompiler = theProvider.CreateCompiler();
			CompilerParameters compParams = new CompilerParameters();
			IEnumerator enumLibs = ExternalLibraries.Instance.LoadedAssemblies;
			while(enumLibs.MoveNext())
				compParams.ReferencedAssemblies.Add(((Assembly)enumLibs.Current).GetName().Name);
			compParams.GenerateExecutable = true;
			compParams.GenerateInMemory = true;
			CompilerResults crs = theCompiler.CompileAssemblyFromSource(compParams, ScriptSource);
			if(crs.Errors.HasErrors)
				for(int i=0; i< crs.Errors.Count; i++)
					Console.WriteLine(crs.Errors[i]);
			m_compiledAssemly = crs.CompiledAssembly;
			//if fails: wrap source in main method + class -> recompile
			#endif
		}


		public Object Execute(String name, Object[] parameters)
		{
			return Execute();
		}


		public Object Execute(Object[] parameters)
		{
			return Execute();
		}

		public Object Execute()
		{
			if(!m_preCompiled)
				PreCompile();
			//add code to execute m_compiledAssemly here	
			Type[] types = m_compiledAssemly.GetTypes();
			BindingFlags flags = ( BindingFlags.Public | BindingFlags.Static ); 
			foreach(Type t in types)
			{
				MethodInfo[] mi = t.GetMethods(flags);
				foreach(MethodInfo m in mi)
					if(m.Name == "Main")
						m_retValue = m.Invoke(null, null);
			}
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

		public String ScriptSource
		{
			get { return m_scriptSource;  }
			set { m_scriptSource = value; }
		}

		public String Type
		{
			get { return m_type;  }
			set { m_type = value; }
		}

		public Object ReturnValue
		{
			get { return m_retValue; }
		}

		
		public const string TYPE	= "type";

		public const string IAM		= "script";
	}
	
}
