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

namespace Uiml.Executing
{
	using Uiml;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	using System.Reflection;
	#if !COMPACT
	using System.CodeDom.Compiler;
	#endif

	public class Script :  IExecutable, IUimlElement
	{

		private String m_scriptSource;
		private String m_type;
		private bool m_preCompiled = false;
		private Assembly m_compiledAssembly;
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
			Source = scriptSource;
			Type   = scriptType;
		}

		public Script(XmlNode n) : this()
		{
			//TODO this should be replaced by
			//"waiting for the parent Part" when the Script
			//element is more complete
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;

			XmlAttributeCollection attr = n.Attributes;
			if(attr.GetNamedItem(TYPE) != null)
				Type = attr.GetNamedItem(TYPE).Value;
			else
				Console.WriteLine("Warning: " + IAM + " should have \"" + TYPE + "\" attribute!");
			Source = n.InnerText;
		}

		public void GetEvents(ArrayList al)
		{
		}

		///<summary>
		///Loads a CodeProvider for the language used for inline scripting
		///</summary>
		///<todo>
		///Load CodeProviders dynamically...
		///</todo>
		protected void PreCompile()
		{
			#if !COMPACT
			CodeDomProvider theProvider = null;
			switch(Type)
			{
				case "CSharp":
				case "C#":
				case "csharp":
				case "C Sharp":
				case "C sharp":
					theProvider = new Microsoft.CSharp.CSharpCodeProvider();
					break;
				case "JScript":
					// FIXME
					Console.WriteLine("JScript does not yet work with Mono.");
					return;
					//theProvider = new Microsoft.JScript.JScriptCodeProvider();
					//break;
        case "Visual Basic":
				case "VB":
				case "VB.Net":
				case "vb":
				case "vb.net":
				case "VB.NET":
					theProvider = new Microsoft.VisualBasic.VBCodeProvider();
					break;
				case "Nemerle":
				case "nemerle":
					theProvider = new Nemerle.Compiler.NemerleCodeProvider();
					break;
				case "Boo":
				case "boo":
					theProvider = new Boo.Lang.CodeDom.BooCodeProvider();
					break;
				default:
					theProvider = new Microsoft.CSharp.CSharpCodeProvider();
					break;
			}
			
			ICodeCompiler theCompiler = theProvider.CreateCompiler();
			CompilerParameters compParams = new CompilerParameters();
			IEnumerator enumLibs = ExternalLibraries.Instance.LoadedAssemblies;
			while(enumLibs.MoveNext())
			{
				Console.WriteLine("[Inline Script precompile linking]: " + enumLibs.Current);
				compParams.ReferencedAssemblies.Add(((Assembly)enumLibs.Current).Location);
			}
			compParams.GenerateExecutable = true;
			compParams.GenerateInMemory = true;
			CompilerResults crs = theCompiler.CompileAssemblyFromSource(compParams,Source);
			if(crs.Errors.HasErrors)
				for(int i=0; i< crs.Errors.Count; i++)
					Console.WriteLine("[Inline script precompile error]:" + crs.Errors[i]);
		   m_compiledAssembly = crs.CompiledAssembly;
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
		  
			// FIXME: remove this test when JScript works
			if (m_compiledAssembly == null)
				return null;
			
			// let's execute m_compiledAssembly here	
			Type[] types = m_compiledAssembly.GetTypes();

			// Boo and Nemerle have non-public Main methods
			BindingFlags flags = ( BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic ); 

			foreach(Type t in types)
      {
        MethodInfo[] mi = t.GetMethods(flags);
				foreach(MethodInfo m in mi)
				{
					if(m.Name == "Main")
					{
						try
						{
							m_retValue = m.Invoke(null, null);
						}
						catch (TargetParameterCountException)
						{
							// parameter to main method is string[] instead of void
							m_retValue = m.Invoke(null, new object[] { new string[] {} });
						}
					}
				}
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

		public String Source
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

		
		public const string IAM  = "script";
		public const string TYPE  = "type";
	}
}
