/*
 	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)

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

namespace Uiml.Rendering.GTKsharp
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Gtk;
	using GtkSharp;

	using Uiml;
	using Uiml.Utils.Reflection;
	using Uiml.Rendering;
    using Uiml.Rendering.TypeDecoding;

	using Style = Uiml.Style;
	

	public class GtkRenderer : Renderer, IPropertySetter
	{
		private GtkRenderedInstance m_topWindow;

		private string m_adder = "Add";

		public GtkRenderer()
		{
			ExternalLibraries.Instance.Add(SYSTEM_ASSEMBLY, AssemblyLoader.LoadFromGacOrAppDir(SYSTEM_ASSEMBLY));
			GuiAssembly = AssemblyLoader.LoadFromGacOrAppDir(GTK_ASSEMBLY);
			ExternalLibraries.Instance.Add(GTK_ASSEMBLY, GuiAssembly);
			ExternalLibraries.Instance.Add(GDK_ASSEMBLY, AssemblyLoader.LoadFromGacOrAppDir(GDK_ASSEMBLY));
			ExternalLibraries.Instance.Add(PANGO_ASSEMBLY, AssemblyLoader.LoadFromGacOrAppDir(PANGO_ASSEMBLY));
			ExternalLibraries.Instance.Add(GLIB_ASSEMBLY, AssemblyLoader.LoadFromGacOrAppDir(GLIB_ASSEMBLY));
			
			// register type decoders
			TypeDecoder.Instance.Register(typeof(GtkTypeDecoders));
		}

		public override IRenderedInstance TopWindow
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
				Application.Init();
				m_topWindow = new GtkRenderedInstance();
				m_topWindow.Title = uimlDoc.Title;
				Structure uiStruct   = (Structure)uimlDoc.UInterface.UStructure[0];
				Style     uiStyle    = (Style)uimlDoc.UInterface.UStyle[0];
				Behavior  uiBehavior = null;
				try{
					uiBehavior = (Behavior)uimlDoc.UInterface.UBehavior[0];
				}catch(Exception e){ /* no behavior specified */ }

				Top = uiStruct.Top;
				Voc = uimlDoc.SearchPeers(NAME).GetVocabulary();
				Widget c = Render(uiStruct.Top, uiStyle);
				//Render has filled the part-tree with the concrete object references
				//to the individual widgets, now attach the behavior			
				//GtkEventLinker gll = new GtkEventLinker(this);
                EventLinker gll = new EventLinker(this);
				gll.Link(uiStruct, uiBehavior);
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

		///<summary>
		/// This is the ``core'' rendering method. It will recursively descend into the Part hierarchy
		/// and using the .net reflection mechanisms to create the appropriate widgets
		///</summary>
		private Widget Render(Part uiPart, Style uiStyle) //throws WrongNestingException, MappingNotFoundException
		{
		     string className = Voc.MapsOnCls(uiPart.Class);
			  Type classType = GuiAssembly.GetType(className);
			  Type containerType = GuiAssembly.GetType(CONTAINER);

			  bool layoutWidget = DoesLayout(classType);			  
			  System.Object gtkObject;
			  
			  try
			  {
				  if(layoutWidget)
					  gtkObject = Activator.CreateInstance(classType, new System.Object[] { HOMOGENEOUS, SPACING} );
				  else
					  gtkObject = Activator.CreateInstance(classType);				  
			  }
			  catch(MissingMethodException mme)
			  {
				  gtkObject = CreateWithParams(classType, uiPart, uiStyle);
			  }
			  
			  //attach it to the Part for later manipulation
			  uiPart.UiObject = gtkObject;


				if(uiPart.HasSubParts())
				{	
					if(classType.IsSubclassOf(containerType))
					{
						//if(layoutWidget)//this is layout code
						//	m_adder = PACKSTART;
						//else
							m_adder = ADD;
							
						IEnumerator enumParts = uiPart.GetSubParts();
						while(enumParts.MoveNext())
						{
							Part subPart = (Part)enumParts.Current;
                            if (subPart.Identifier == "textfield2")
                            {
                                int zz = 0;
                            }
							Widget b = Render(subPart, uiStyle);
							//((Container)gtkObject).Add(b); replaced by:
							MethodInfo m = classType.GetMethod(m_adder, new Type[] { b.GetType() } );
							m.Invoke(gtkObject, new System.Object[] { b });
						}
						return (Container)ApplyProperties((Container)gtkObject, uiPart, uiStyle);
						
					}
					else
					{
						throw new WrongNestingException(className, CONTAINER);
					}
				}
				else
				{
					//gtkObject = AttachEvents((Widget)gtkObject, uiBehavior);
					return (Widget)ApplyProperties((Widget)gtkObject, uiPart, uiStyle);
				}
		}

		///<summary>
		/// This method is used when the classType does not contain a parameterless
		/// constructor. It searches for the first constructor that ``fits'' and
		/// according to the defined Style in uiStyle for the uiPart.
		///</summary>
		private Widget CreateWithParams(Type classType, Part uiPart, Style uiStyle)
		{
		  System.Object gtkObject = null;
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
							Console.WriteLine(aiobe);
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
			  Console.WriteLine("Error in uiml document: required properties \"{0}\" are not available for {1} (of class {2})", paramsString, uiPart.Identifier, uiPart.Class);
			  Environment.Exit(-1);
			  return null; //useless statement
			 }
			 else
			 {
			  //create the arguments for the constructor from the property array
			  //and pass it to the constuctor immediately
			  try{ return (Widget)construct.Invoke(TypeDecoder.Instance.GetMultipleArgs(props,tparamTypes));}
			  catch 
			  { 
				  String paramsString = "";
				  for(int j=0; j< parameters.Length; j++)
					  paramsString +=  parameters[j].Identifier + ":" + parameters[j].Type + ", ";
				  Console.WriteLine("Error in uiml document: required properties \"{0}\" are not available for {1} (of class {2})", paramsString, uiPart.Identifier, uiPart.Class);
				  Environment.Exit(-1);
				  return null; //useless statement
			  }
		  }
		}


		///<summary>
		/// Checks if Type t is a Type that takes care of layout management
		///</summary>
		///<remarks>
		/// Should be optimized to speed up rendering
		///</remarks>
		private bool DoesLayout(Type t)
		{
			for(int i=0; i<LAYOUT.Length; i++)
			{
				Type layoutType = GuiAssembly.GetType(LAYOUT[i]);
				if(t.Equals(layoutType))
					return true;
			}
			return false;
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

		/* moved to super class
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
		}
		*/

		///<summary>
		/// Applies several properties to an individual concrete widget instance   
		/// relying on hard-coded knowledge about the widgets
		///</summary>
		///<param name="UiObject"></param>
		///<param name="part"></param>
		///<param name"s"></param>
		///<todo>
		// Change to the .custom format, like used for the gtk# bindings
		///</todo>
		protected override System.Object LoadAdHocProperties(ref System.Object uiObject, Part part, Style s)
		{
			if(part.Class == "TabPage")
			{
				/* FIXME: this implementation does not work because the TabPage has not yet been added
				 * to the Tabs parent! 
				 */
				Property p = s.SearchProperty(part.Identifier, "label");
				string label;
				
				if(p != null)
					label = (string)p.Value;
				else
					label = "";
					
				if(!part.Top) // if we have a parent
				{
					Notebook n = (Notebook)part.Parent.UiObject;
					n.SetTabLabelText((Widget)part.UiObject, label);
				}
			}
			
			return uiObject;
		}


		///<summary>
		/// Applies several properties to an individual concrete widget instance   
		/// relying on hard-coded knowledge about the widgets, after
        /// all the other properties have been set first.
		///</summary>
		///<param name="UiObject"></param>
		///<param name="part"></param>
		///<param name"s"></param>
		protected override System.Object LoadAdHocPropertiesAfter(ref System.Object uiObject, Part part, Style s)
		{
			if(part.Class == "Tree" || part.Class == "List")
			{
				Property p = s.SearchProperty(part.Identifier, "title");
                Gtk.TreeView tree = (Gtk.TreeView) uiObject;

				if (p == null)
                {
                    // add an empty column
                    Gtk.TreeViewColumn column = new Gtk.TreeViewColumn();
                    column.Title = (string) p.Value;
                    tree.AppendColumn(column);
                }

                for (int i = 0; i < tree.Columns.Length; i++)
                {
                    Gtk.TreeViewColumn col = tree.Columns[i];

                    Gtk.CellRendererText crt = new Gtk.CellRendererText();
                    col.PackStart(crt, true);
                    col.AddAttribute(crt, "text", i); 
                }

                tree.HeadersVisible = true;
			}
			
			return uiObject;
		}
	
		public const int SPACE = 3;
		public const string GTK_ASSEMBLY    = "gtk-sharp";
		public const string SYSTEM_ASSEMBLY = "mscorlib"; 
		public const string GDK_ASSEMBLY    = "gdk-sharp";
		public const string PANGO_ASSEMBLY  = "pango-sharp";
		public const string GLIB_ASSEMBLY   = "glib-sharp";

		public const int MAX_ASSSEMBLIES = 5;

		public const string CONTAINER = "Gtk.Container";
		public string[] LAYOUT = {"Gtk.HBox", "Gtk.VBox"}; //"Gtk.Table" //-->TODO; add table in a transparant way
		public const string PACKSTART = "PackStart";
		public const string PACKEND = "PackEnd";
		public const string ADD = "Add";

		public const string NAME = "gtk-sharp-1.0";

		public const Boolean HOMOGENEOUS = false;
		public const Int32 SPACING = 4;
		public const string CONSTRUCTOR = "constructor";
	}
}
