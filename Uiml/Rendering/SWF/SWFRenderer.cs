/*
 	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/projects/uiml.net/)

	 Copyright (C) 2004  Kris Luyten (kris.luyten@uhasselt.be)
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

	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/

namespace Uiml.Rendering.SWF
{
	using System;
	using System.Collections;
	using System.Reflection;

	using System.Windows.Forms;

	using Uiml;
	using Uiml.Utils.Reflection;
	using Uiml.Rendering;
    using Uiml.Rendering.TypeDecoding;
	using Uiml.LayoutManagement;

	public class SWFRenderer : Renderer, IPropertySetter
	{
		private SWFRenderedInstance m_topWindow;

		private string m_addProperty = "Controls";
		private string m_addMethod = "Add";

		public SWFRenderer()
		{
			ExternalLibraries.Instance.Add(SYSTEM_ASSEMBLY, AssemblyLoader.LoadFromGacOrAppDir(SYSTEM_ASSEMBLY));
			ExternalLibraries.Instance.Add(DRAWING_ASSEMBLY, AssemblyLoader.LoadFromGacOrAppDir(DRAWING_ASSEMBLY));

			GuiAssembly = AssemblyLoader.LoadFromGacOrAppDir(SWF_ASSEMBLY);
			ExternalLibraries.Instance.Add(SWF_ASSEMBLY, GuiAssembly);
			
			// register type decoders
			TypeDecoder.Instance.Register(typeof(SWFTypeDecoders));
		}

		public IRenderedInstance TopWindow
		{
			get { return m_topWindow; }
		}
		
		public override IPropertySetter PropertySetter
		{
			get { return this; }
		}

		
		public override IRenderedInstance PreRender(UimlDocument uimlDoc)
		{
			try
			{	
				m_topWindow = new SWFRenderedInstance();		
				m_topWindow.Title = uimlDoc.Title;
				Structure uiStruct   = (Structure)uimlDoc.UInterface.UStructure[0];
				Style     uiStyle    = (Style)uimlDoc.UInterface.UStyle[0];
				Behavior  uiBehavior = null;
				try{
					uiBehavior = (Behavior)uimlDoc.UInterface.UBehavior[0];
				}catch(Exception e){ /* no behavior specified */ }

				Top = uiStruct.Top;
				Voc = uimlDoc.SearchPeers(NAME).GetVocabulary();
				Control c = Render(uiStruct.Top, uiStyle);
				//Render has filled the part-tree with the concrete object references
				//to the individual widgets, now attach the behavior			
				SWFEventLinker sel = new SWFEventLinker(this);
				sel.Link(uiStruct, uiBehavior);
				m_topWindow.Add(c);
				return m_topWindow;
			}
			catch(NullReferenceException nrfe)
			{
				Console.WriteLine("The Rendering Engine says: Check the input documents, they seem to be invalid");
				throw nrfe;
			}
			
			catch(Exception e)
			{
				Console.WriteLine("Unexpected failure ({0}) while processing {1}:", e.GetType(), uimlDoc);
				Console.WriteLine(e.ToString());
				Console.WriteLine("Please contact the uiml.net maintainer with the above output.");
				throw e;
			}
		}
		
		public override IRenderedInstance Render(UimlDocument uimlDoc)
		{
			IRenderedInstance instance = PreRender(uimlDoc);
			uimlDoc.SolveLayoutProperties(this);
			
			// render again
			instance = PreRender(uimlDoc);
			
			// of course only if there's a layout specified, it's possible to reorder
			if (uimlDoc.HasLayout) 
			{
				// now do reordering
				Structure uiStruct   = (Structure)uimlDoc.UInterface.UStructure[0];
				Console.Write("Trying to reorder the interface... ");
				SWFReorderer reorderer = new SWFReorderer(uiStruct.Top);
				reorderer.Execute();
				Console.WriteLine("Done!");

				// we need to render any possible added widgets
				instance = PreRender(uimlDoc);
				
				// then the constraints must be solved
				uimlDoc.SolveLayoutProperties(this);

				// and we must render once again to apply these
				instance = PreRender(uimlDoc);
			}
			
			return instance;
		}


		///<summary>
		/// This is the ``core'' rendering method. It will recursively descend into the Part hierarchy
		/// and using the .net reflection mechanisms to create the appropriate widgets
		///</summary>
		private Control Render(Part uiPart, Style uiStyle) //throws WrongNestingException, MappingNotFoundException
		{
			string className = Voc.MapsOnCls(uiPart.Class);				
			Type classType = GuiAssembly.GetType(className);
			Type containerType = GuiAssembly.GetType(CONTAINER);

			object swfObject = (Activator.CreateInstance(classType));
			  			  
			//attach it to the Part for later manipulation
			uiPart.UiObject = swfObject;

			if(uiPart.HasSubParts())
			{	
				if(classType.IsSubclassOf(containerType))
				{
					IEnumerator enumParts = uiPart.GetSubParts();
					while(enumParts.MoveNext())
					{
						Part subPart = (Part)enumParts.Current;
						Control b = Render(subPart, uiStyle);

						// add b to swfObject
						PropertyInfo p = classType.GetProperty(m_addProperty);						
						Control.ControlCollection controls = (Control.ControlCollection)p.GetValue(swfObject, null);
						MethodInfo m = controls.GetType().GetMethod(m_addMethod, new Type[] { b.GetType() } );
						m.Invoke(controls, new System.Object[] { b });						
					}
					return (Control)ApplyProperties((Control)swfObject, uiPart, uiStyle);
					
				}
				else
				{
					throw new WrongNestingException(className, CONTAINER);
				}
			}
			else
			{
				try
				{						
					return (Control) ApplyProperties((Control)swfObject, uiPart, uiStyle);
				}
				catch(Exception e)
				{
					Console.WriteLine("Waaaaahhhgggrrr!:{0}",e);
					Console.WriteLine(swfObject.GetType().ToString());
					return null;
				}
			}
		}

		///<summary>
		/// This is the implementation of the method specified in the IPropertySetter Interface
		/// For now it is implemented in the rendering engine itself, but when it becomes
		/// too complex the IPropertySetter implementation will be isolated from
		/// this rendering class.
		///</summary>
		///<remarks>
		/// If part==null, the property will be retrieved fro the first matching part
		/// found starting from the top-part in first-order.
		///</remarks>
		public System.Object GetValue(Part part, Property prop)
		{
			if(part == null)
				part = Top;
			//search for the part, and get the widget
			Part p = part.SearchPart(prop.PartName);
			string className  = Voc.MapsOnCls(p.Class);
			Type classType = GuiAssembly.GetType(className);
			string getter = Voc.GetPropertyGetter(prop.Name, p.Class);

			System.Object targetObject = p.UiObject;
			MemberInfo mInfo = null;
			int j = getter.IndexOf('.');
			while(j!=-1)
			{
				String parentType = getter.Substring(0,j);
				getter=getter.Substring(j+1,getter.Length-j-1);			
				mInfo = ResolveProperty(classType, parentType, out classType, ref targetObject);
				//targetObject = ((PropertyInfo)mInfo).GetValue(targetObject, null);
				j = getter.IndexOf('.');
			}
			
			try{ mInfo = ResolveProperty(classType, getter, out classType, ref targetObject); }
				catch(Exception e)
				{
					Console.WriteLine("Unable to query property \"{0}\" of part \"{1}\";", prop.Name, part.Identifier);
					Console.WriteLine("Please check your UIML document or the {0} vocabulary", Voc.VocabularyName);
				}
				
			return targetObject;
		}

		/* Moved to super class
		///<summary>
		/// Dissects the method information for a specific property
		///</summary>
		///<param name="baseT"></param>
		///<param name="newT"></param>
		///<param name="retType"></param>
		///<param name="nextValue"></param>
		private MemberInfo ResolveProperty(Type baseT, String newT, out Type retType, ref System.Object nextValue)
		{			
			MemberInfo m = baseT.GetProperty(newT);
			if(m == null)
			{
				m = baseT.GetMethod(newT, new Type[0]);
				retType = ((MethodInfo)m).ReturnType;
				nextValue = ((MethodInfo)m).Invoke(nextValue, null);
			}
			else
			{
				retType = ((PropertyInfo)m).PropertyType;
				nextValue = ((PropertyInfo)m).GetValue(nextValue, null);
			}
			return m;
		}*/

		///<summary>
		/// Applies several properties to an individual concrete widget instance   
		/// relying on hard-coded knowledge about the widgets
		///</summary>
		///<todo>
		// Change to the .custom format, like used for the gtk# bindings
		///</todo>
		protected override System.Object LoadAdHocProperties(ref System.Object uiObject, Part part, Style s)
		{
			// TODO -> use this method to automatically set properties for specific classes (such as multiline for Text) 
			return uiObject;
		}
	
		public const int SPACE = 3;
		public const string SYSTEM_ASSEMBLY		= "mscorlib"; 
		public const string SWF_ASSEMBLY		= "System.Windows.Forms";
		public const string DRAWING_ASSEMBLY	= "System.Drawing";
		
		public const int MAX_ASSSEMBLIES = 2;

		// a Control can have children
		public const string CONTAINER = "System.Windows.Forms.Control";

		public const string NAME = "swf-1.1";
		public const string CONSTRUCTOR = "constructor";		
	}
}
