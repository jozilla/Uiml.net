/*
  	 Uiml.Net: a Uiml.Net renderer (http://research.edm.luc.ac.be/kris/research/uiml.net/)
   
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

	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/


/*
TODO:
	- add support for property placeholders in scripts. E.g. like asp.net allows
	C#/vb.net code inside html
	- add support for result capturing, so results from scripts can be used inside the user interface
*/

namespace Uiml.net.Peers
{
	using Uiml;
	using Uiml.Executing;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.IO;

	using System.Reflection;
	using System.CodeDom.Compiler;

	/// <summary>
	/// This class represents a &lt;script&gt; element in the vocabulary, specified by the following DTD:
	/// &lt;!ELEMENT d-param (#PCDATA)&gt;
	/// &lt;ATTLIST d-param
	///             id NMTOKEN #IMPLIED
	///             type CDATA #IMPLIED&gt;
	/// </summary>
	public class Script :  IExecutable, IUimlElement
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

		protected void PreCompile()
		{
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
			CompilerResults crs = theCompiler.CompileAssemblyFromSource(compParams,Source);
			if(crs.Errors.HasErrors)
				for(int i=0; i< crs.Errors.Count; i++)
					Console.WriteLine(crs.Errors[i]);
			m_compiledAssemly = crs.CompiledAssembly;
			//if fails: wrap source in main method + class -> recompile
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
