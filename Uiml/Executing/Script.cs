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
					theProvider = DynamicallyLoadLanguage("Nemerle", NEMERLE_LIBS, NEMERLE_CODE_PROVIDER, NEMERLE_CODE_PROVIDER_LIB_INDEX);
					if (theProvider == null)
						return; // an error has occured, just quit ...
					
					break;
				case "Boo":
				case "boo":
					theProvider = DynamicallyLoadLanguage("Boo", BOO_LIBS, BOO_CODE_PROVIDER, BOO_CODE_PROVIDER_LIB_INDEX);
					if (theProvider == null)
						return; // an error has occured, just quit ...
					
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

		private CodeDomProvider DynamicallyLoadLanguage(string lang, string[] libNames, string codeProvider, int codeProviderLibIndex)
		{		
			ArrayList assemblies = new ArrayList();
			
			try
			{
				// dynamically load language libraries
				foreach (string libName in libNames)
				{
					// FIXME: we should use Assembly.Load, but this is not flexible
					// since we have to know exactly which version to use. However, using
					// LoadWithPartialName could lead to problems with mixing 1.0 and 2.0 
					// assemblies (as is the case with Nemerle).
					Console.Write("[Loading {0} library]: {1} ... ", lang, libName);
					Assembly lib = Assembly.LoadWithPartialName(libName);
			
					if (lib != null)
					{
						assemblies.Add(lib);
						Console.WriteLine("OK!");
					       	Console.WriteLine("\t--> {0}", lib.FullName);
					}
					else
					{
						Console.WriteLine("Failed :-(");
						throw new FileNotFoundException("Couldn't load assembly {0}", libName);
					}
				}
				
				Assembly codeProviderlib = (Assembly) assemblies[codeProviderLibIndex];
				Type t = codeProviderlib.GetType(codeProvider);
				return (CodeDomProvider) Activator.CreateInstance(t);
			}
			catch (Exception err)
			{
				Console.WriteLine("Error while loading Boo libraries: {0}", err);
				return null;
			}
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
		
		public const string NEMERLE_CODE_PROVIDER = "Nemerle.Compiler.NemerleCodeProvider";
		public const int NEMERLE_CODE_PROVIDER_LIB_INDEX = 1; // the second lib 
		public static string[] NEMERLE_LIBS = {
							"Nemerle",
							"Nemerle.Compiler",
							"Nemerle.Macros",
				                      };
		
		public const string BOO_CODE_PROVIDER = "Boo.Lang.CodeDom.BooCodeProvider";
		public const int BOO_CODE_PROVIDER_LIB_INDEX = 1; // the second lib
		public static string[] BOO_LIBS = { 
						    "Boo.Lang", 
						    "Boo.Lang.CodeDom", 
						    "Boo.Lang.Compiler", 
						    "Boo.Lang.Interpreter",
						    "Boo.Lang.Parser", 
						    "Boo.Lang.Useful" 
						 };
	}
}
