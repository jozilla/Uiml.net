/*
 	 Uiml.Net: a Uiml.Net renderer (http://resaerch.edm.luc.ac.be/kris/project/uiml.net/)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://www.edm.luc.ac.be)
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
namespace Uiml.Rendering.WXnet
{
	using System;
	using System.Collections;
	using System.Reflection;

	using wx;

	using Uiml;
	using Uiml.Rendering;
	

	public class WxRenderer : Renderer, IPropertySetter
	{
		private WxRenderedInstance m_topWindow;
		private String m_adder = ADD;
		
		public WxRenderer()
		{
			Console.WriteLine("creating WxRenderer");
			Decoder = new WxTypeDecoder();
			ExternalLibraries.Instance.Add(SYSTEM_ASSEMBLY, Assembly.Load(SYSTEM_ASSEMBLY));
			GuiAssembly = Assembly.Load(WX_ASSEMBLY);
			ExternalLibraries.Instance.Add(WX_ASSEMBLY, GuiAssembly);
			ExternalLibraries.Instance.Add(DRAWING_ASSEMBLY, Assembly.Load(DRAWING_ASSEMBLY));
			//Gtk.Application.Init();
			//wx.App.Initialize();
			Console.WriteLine("WxRenderer created");
		}

		public IRenderedInstance TopWindow
		{
			get { return m_topWindow; }
		}

		public override IPropertySetter PropertySetter
		{
			get { return this; }
		}
		
		public override IRenderedInstance Render(UimlDocument uimlDoc)
		{
			try
			{
				m_topWindow = new WxRenderedInstance("Uiml container");

				Structure uiStruct   = (Structure)uimlDoc.UInterface.UStructure[0];
				Style     uiStyle    = (Style)uimlDoc.UInterface.UStyle[0];
				Behavior  uiBehavior = null;
				try{
					uiBehavior = (Behavior)uimlDoc.UInterface.UBehavior[0];
				}catch(Exception e){ /* no behavior specified */ }

				Top = uiStruct.Top;
				Voc = uimlDoc.SearchPeers(NAME).GetVocabulary();
				Console.WriteLine("Calling TopFrame...");
				Console.WriteLine("In Render; TopFrame is {0}", m_topWindow.TopFrame);
				Window c = Render(uiStruct.Top, uiStyle, m_topWindow.TopFrame);
				//Render has filled the part-tree with the concrete object references
				//to the individual widgets, now attach the behavior
				//c.Reparent((Window)m_topWindow.TopFrame);
				return m_topWindow;
			}
				catch(NullReferenceException nrfe)
				{
					Console.WriteLine("The Rendering Engine says: Check the input documents, they seem to be invalid");
					Console.WriteLine(nrfe);
					throw nrfe;
				}
		}

		///<summary>
		/// This is the ``core'' rendering method. It will recursively descend into the Part hierarchy
		/// and using the .net reflection mechanisms to create the appropriate widgets
		///</summary>
		private Window Render(Part uiPart, Style uiStyle, Window parent) //throws WrongNestingException, MappingNotFoundException
		{
		     string className = Voc.MapsOnCls(uiPart.Class);
			  Type classType = GuiAssembly.GetType(className);
			  Type containerType = GuiAssembly.GetType(CONTAINER);

			  System.Object wxObject;
			  
//			  try{ 			  
//				  Console.WriteLine("Try to create instance for {0}", classType);
//				  wxObject = Activator.CreateInstance(classType); 			  
//				  Console.WriteLine("Created object {0}", wxObject);
//			  }
//				  catch(MissingMethodException mme)
				  {
					  	if(classType.Equals(containerType))
							wxObject = CreateLayoutWithParams(classType, uiPart, uiStyle); 
						else
							wxObject = CreateWithParams(classType, uiPart, uiStyle, parent); 
				  }
			  
			  //attach it to the Part for later manipulation
			  uiPart.UiObject = wxObject;


				if(uiPart.HasSubParts())
				{
					if(classType.Equals(containerType))
					{
						m_adder = ADD;
						IEnumerator enumParts = uiPart.GetSubParts();
						Window wxContainer = new Panel(parent);
						while(enumParts.MoveNext())
						{
							Part subPart = (Part)enumParts.Current;
							Window b = Render(subPart, uiStyle, wxContainer);
							MethodInfo m = classType.GetMethod(m_adder, new Type[] { b.GetType() } );
							m.Invoke(wxObject, new System.Object[] { b });
						}
						PropertyInfo addLayout = typeof(Window).GetProperty(APPLY_LAYOUT /*, new Type[] { classType }*/ );
						addLayout.SetValue(wxContainer, wxObject, null );
						wxContainer.Fit();
						return (Window)ApplyProperties((Window)wxContainer, uiPart, uiStyle);
					}
					else
					{
						throw new WrongNestingException(className, CONTAINER);
					}
				}
				else
				{
					//wxObject = AttachEvents((Widget)wxObject, uiBehavior);
					return (Window)ApplyProperties((Window)wxObject, uiPart, uiStyle);
				}
		}


		///<summary>
		/// This method is used when the classType does not contain a parameterless
		/// constructor. It searches for the first constructor that ``fits'' and
		/// according to the defined Style in uiStyle for the uiPart.
		///</summary>
		private System.Object CreateLayoutWithParams(Type classType, Part uiPart, Style uiStyle)
		{ 
		  Param[] parameters = Voc.GetParams(CONSTRUCTOR, uiPart.Class);
		  Type[] tparamTypes = new Type[parameters.Length];
		  int i = 0;
		  foreach(Param p in parameters)
		  {
				 int j = 0;
				 while(tparamTypes[i] == null)
				 {
					 try
					 {
						tparamTypes[i] = ((Assembly)ExternalLibraries.Instance.Assemblies[j++]).GetType(p.Type, true, true);
					 }
					 	catch(TypeLoadException tle)
						{
						}
						catch(Exception aiobe)//replace by ArrayIndexOutOfBounds
						{
							Console.WriteLine("Error in vocabulary: parameter {3} of type {0} for constructor of {1} is specified incorrect for part {2}", p.Type, uiPart.Class, uiPart.Identifier, p.Identifier);
							Environment.Exit(-1);
						}
				 }
				 i++;
		  }

		  ConstructorInfo construct = classType.GetConstructor(tparamTypes);
		  if(construct == null)
		  {
			  Console.WriteLine("Error in vocabulary: constructor for {0} is not correctly defined.", uiPart.Class);
			  Environment.Exit(-1);
		  }

		  Property[] props = null;
		  if(uiStyle != null)
			  props = uiStyle.GetProperties(uiPart.Identifier, parameters);
			  
		  if(props.Length != parameters.Length)
		  {
		     String paramsString = "";
			  for(int j=0; j< parameters.Length; j++)
				  paramsString +=  parameters[j].Identifier + ":" + parameters[j].Type + ", ";
			  Console.WriteLine("Error in uiml document: required properties \"{0}\" are not available for {1} ({2})", paramsString, uiPart.Identifier, uiPart.Class);
			  Environment.Exit(-1);
			  return null; //useless statement
			 }
			 else
			 {
			  //create the arguments for the constructor from the property array
			  //and pass it to the constuctor immediately
			  try{ return construct.Invoke(Decoder.GetMultipleArgs(props,tparamTypes));}
			  catch
			  { //TODO: remove redundant statements!!
				  String paramsString = "";
				  for(int j=0; j< parameters.Length; j++)
					  paramsString +=  parameters[j].Identifier + ":" + parameters[j].Type + ", ";
				  Console.WriteLine("Error in uiml document: required properties \"{0}\" are not available for {1} ({2})", paramsString, uiPart.Identifier, uiPart.Class);
				  Environment.Exit(-1);
				  return null; //useless statement
			  }
		  }
		}


		///<summary>
		/// This method is used when the classType does not contain a parameterless
		/// constructor. It searches for the first constructor that ``fits'' and
		/// according to the defined Style in uiStyle for the uiPart.
		///</summary>
		private Window CreateWithParams(Type classType, Part uiPart, Style uiStyle, Window parent)
		{
		  System.Object wxObject = null;
		  Param[] parameters = Voc.GetParams(CONSTRUCTOR, uiPart.Class);		 
		  Type[] tparamTypes = new Type[parameters.Length + 1];
		  tparamTypes[0] = typeof(Window);
		  int i = 1;
		  foreach(Param p in parameters)
		  {
				 int j = 0;
				 while(tparamTypes[i] == null)
				 {
					 try
					 {
						tparamTypes[i] = ((Assembly)ExternalLibraries.Instance.Assemblies[j++]).GetType(p.Type, true, true);
					 }
					 	catch(TypeLoadException tle)
						{
						}
						catch
						{
							Console.WriteLine("Error in vocabulary: parameter {3} of type {0} for constructor of {1} is specified incorrect for part {2}", p.Type, uiPart.Class, uiPart.Identifier, p.Identifier);
							Environment.Exit(-1);
						}
				 }
				 i++;
		  }
		  ConstructorInfo construct = classType.GetConstructor(tparamTypes);

		  if(construct == null)
		  {
			  Console.WriteLine("Error in vocabulary: constructor for {0} is not correctly defined.", uiPart.Class);
			  Environment.Exit(-1);
		  }

		  Property[] tempProps = uiStyle.GetProperties(uiPart.Identifier, parameters);
		  Property[] props = new Property[tempProps.Length + 1];
		  
		  for(int teller=1; teller< props.Length; teller++)
			  props[teller] = tempProps[teller-1];
		  
		  if(props.Length-1 != parameters.Length)
		  {
			  String paramsString = "";
			  for(int j=0; j< parameters.Length; j++)
				  paramsString +=  parameters[j].Identifier + ":" + parameters[j].Type + ", ";
			  Console.WriteLine("Error in uiml document: required properties \"{0}\" are not available for {1} ({2})", paramsString, uiPart.Identifier, uiPart.Class);
			  Environment.Exit(-1);
			  return null; //useless statement
		  }
		  else
		  { 
			  //if the first type is Window, insert "parent" in props[0]
			  if( tparamTypes[0].Equals(typeof(Window)))
			  {
				  props[0] = new Property();
				  props[0].Name = "window";
				  props[0].PartName = "wx.Window";
				  props[0].PartName = "wx.Window";
				  props[0].Value = parent;
			  }

			  //create the arguments for the constructor from the property array
			  //and pass it to the constuctor immediately
			  try{ return (Window)construct.Invoke(Decoder.GetMultipleArgs(props,tparamTypes));}
			  catch 
			  { //TODO: remove redundant statements!!
				  String paramsString = "";
				  for(int j=0; j< parameters.Length; j++)
					  paramsString +=  parameters[j].Identifier + ":" + parameters[j].Type + ", ";
				  Console.WriteLine("Error in uiml document: required properties \"{0}\" are not available for {1} ({2})", paramsString, uiPart.Identifier, uiPart.Class);
				  Environment.Exit(-1);
				  return null; //useless statement

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
			PropertyInfo pInfo = null;
			int j = getter.IndexOf('.');
			while(j!=-1)
			{
				String parentType = getter.Substring(0,j);
				getter=getter.Substring(j+1,getter.Length-j-1);
				pInfo = classType.GetProperty(parentType);
				classType = pInfo.PropertyType;
				targetObject = pInfo.GetValue(targetObject, null);
				j = getter.IndexOf('.');
			}
			pInfo = classType.GetProperty(getter);

			//PropertyInfo pInfo = classType.GetProperty(getter);
			try
			{
				return pInfo.GetValue(targetObject, null);
			}
				catch(Exception e)
				{
					Console.WriteLine(e);
					//TODO TODO TODO
					return null;
				}
			
		}


		///<summary>
		/// Applies several properties to an individual concrete widget instance   
		/// relying on hard-coded knowledge about the widgets
		///</summary>
		protected override System.Object LoadAdHocProperties(ref System.Object uiObject, Part part, Style s)
		{
			// Still not necessary :-)
			return uiObject;
		}


		public const int SPACE = 3;
		public const string WX_ASSEMBLY    = "wx.NET";
		public const string SYSTEM_ASSEMBLY = "mscorlib";
		public const string DRAWING_ASSEMBLY = "System.Drawing";
		public const int MAX_ASSSEMBLIES = 3;

		public const string NAME = "wx-net-1.0";

		public const string CONTAINER = "wx.BoxSizer";
		public const string ADD = "Add";
		public const string APPLY_LAYOUT = "Sizer";
		public const string CONSTRUCTOR = "constructor";
	}
}
