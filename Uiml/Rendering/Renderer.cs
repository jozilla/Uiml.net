/*
 	 Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)
    
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

ChangeLog:
	* 07-09-2004 ApplyProperties uses the correct targetObject now (Jo Vermeulen)
*/

namespace Uiml.Rendering
{

	using Uiml;

	using System;
	using System.Collections;
	using System.Reflection;

	///<summary>
	///  This class implementens widget set independent behavior for the 
	/// widget set specific backends. It implements the general core (widget creation,
	//  applying proerties on widgets) of the rendering backends.
	///</summary>
	public abstract class Renderer : IRenderer
	{
		private ITypeDecoder m_typeDecoder;
		private Assembly m_guiAssembly;
		private Vocabulary m_voc;
		private Part m_top;

		protected bool m_stopOnError = true;

		
		abstract public IRenderedInstance Render(UimlDocument uimlDoc);
		abstract public IPropertySetter   PropertySetter   { get; }	
		abstract protected System.Object LoadAdHocProperties(ref System.Object uiObject, Part part, Style s);


		///<summary>
		/// the mapping voabulary that is used to convert the uiml document into the target widget set.
		///</summary>
		public Vocabulary Voc 
		{ 
			get { return m_voc;  }
			set { m_voc = value; }
		}

		///<summary>
		/// Provides the Decoder for the specific backend that is being used. The decoder converts
		/// (typeless) data values out of the UIML document to the types required by the backend.
		///</summary>
		public ITypeDecoder Decoder          
		{
			get { return m_typeDecoder;  } 
			set { m_typeDecoder = value; }
		}

		///<summary>
		/// The assmebly that containts the widgets used by the backend renderer.
		///</summary>
		///<remarks>
		/// This needs to be changed to support a widget sets whose widgets are spread
		/// over several assemblies (.dll files)
		///</remarks>
		public Assembly GuiAssembly
		{
			get { return m_guiAssembly;  }
			set { m_guiAssembly = value; }
		}

		///<summary>
		/// The top part of the structure out of the uiml document that is being rendered.
		///</summary>
		///<remarks>
		/// The Top part is not always the root of the structure tree of the interface.
		/// A subtree of the structure can be rendered by the backend, by just passing the
		/// part top node of the subtree.
		///</remarks>
		public Part Top
		{
			get { return m_top;  }
			set { m_top = value; }
		}



		///<summary>
		/// Applies several properties to an individual concrete widget instance.
		/// It implements two stages:
		///   - In the first stage the widget properties are applied completely dynamic:
		///   relying on the reflection mechanisms the Style properties are queried and loaded
		///   - When the first stage fails, another method is called using ad hoc knowledge
		///   about the widget and its properties
		///</summary>
		protected System.Object ApplyProperties(System.Object uiObject, Part part, Style style)
		{
			//should be provided by another "component" instead of by a function: 
			//this way the structure can be reused more easily

			try
			{
				LoadPartProperties(ref uiObject, part);
				LoadNamedProperties(ref uiObject, part, style);
				LoadClassProperties(ref uiObject, part, style);
				LoadAdHocProperties(ref uiObject, part, style);
			}
				catch(MappingNotFoundException mnfe)
				{
					Console.WriteLine("Could not find appropriate mapping: " + mnfe.ToString());
					if(m_stopOnError)
						Environment.Exit(0);
				}
			return uiObject;
		}

		private System.Object LoadPartProperties(ref System.Object uiObject, Part part)
		{
			string className  = Voc.MapsOn(part.Class);
			Type classType = GuiAssembly.GetType(className);
		
			IEnumerator enumProps = part.Properties;
			while(enumProps.MoveNext())
			{
				Property p = (Property)enumProps.Current;
				uiObject = ApplyProperty(ref uiObject, p, part, classType);	
			}
			return uiObject;

		}

		///<summary>
		///Loads the named properties available in style for the gtkObject widget
		//</summary>
		private System.Object LoadNamedProperties(ref System.Object uiObject, Part part, Style style)
		{
			string className  = Voc.MapsOn(part.Class);
			Type classType = GuiAssembly.GetType(className);

			IEnumerator enumProps = style.GetNamedProperties(part.Identifier);
			while(enumProps.MoveNext())
			{
				Property p = (Property)enumProps.Current;
				uiObject = ApplyProperty(ref uiObject, p, part, classType);				
			}
			return uiObject;
		}


		///<summary>
		///Loads the class properties available in style for the gtkObject widget
		//</summary>
		private System.Object LoadClassProperties(ref System.Object uiObject, Part part, Style style)
		{
			string className  = Voc.MapsOn(part.Class);
			Type classType = GuiAssembly.GetType(className);

			IEnumerator enumProps = style.GetClassProperties(part.Identifier);
			while(enumProps.MoveNext())
			{
				Property p = (Property)enumProps.Current;
				uiObject = ApplyProperty(ref uiObject, p, part, classType);				
			}
			return uiObject;
		}



		///<summary>
		///This is the implementation of the method specified in the IPropertySetter Interface
		///For now it is implemented in the rendering engine itself, but when it becomes
		///too complex the IPropertySetter implementation will be isolated from
		///this rendering class.
		///</summary>
		 ///<param name="part">The part on which prop will be applied</param>
		///<param name="prop">the property that will be applied</param>
		///<remarks>
		/// If part is null, the top part will be assumed
		///</remarks>
		public void ApplyProperty(Part part, Property prop)
		{
			if(part == null)
				part = m_top;
			Part p = part.SearchPart(prop.PartName);
			if(prop.Lazy)
				prop.Resolve(this);
			string className  = Voc.MapsOn(p.Class);
			Type classType = GuiAssembly.GetType(className);
			System.Object uiObj =  p.UiObject;
			ApplyProperty(ref uiObj, prop, p, classType);	
		}


		///<summary>
		/// This method sets the concrete property on a concrete widget. It is the most important
		/// method to get the defined properties reflected in the User Interface
		///</summary>
		///<param name="uiObject"></param>
		///<param name="p"></param>
		///<param name="part"></param>
		///<param name="tclassType"></param>
		private System.Object ApplyProperty(ref System.Object uiObject, Property p, Part part, Type tclassType)
		{
				string setter = Voc.GetSetProperty(p.Name, part.Class);

				//try to use Properties first,
				//if that fails, look for the appropriate setters in the available methods
				try
				{
					Type classType = tclassType;
					System.Object targetObject = uiObject;
					PropertyInfo pInfo = null;
					int j = setter.IndexOf('.');
					while(j!=-1)
					{
						String parentType = setter.Substring(0,j);
						setter=setter.Substring(j+1,setter.Length-j-1);
						pInfo = classType.GetProperty(parentType);
						classType = pInfo.PropertyType;
						targetObject = pInfo.GetValue(targetObject, null);
						j = setter.IndexOf('.');
					}

					
					///thanks to Rafael "Monoman" Teixeira for this code
					//pInfo = classType.GetProperty(setter);
               MemberInfo[] arrayMemberInfo = classType.FindMembers(MemberTypes.Method  | MemberTypes.Property,
							                                               BindingFlags.Public | BindingFlags.Instance,
																						  Type.FilterName, setter);
					//if lazy, execute it!
					if(p.Lazy)
						p.Resolve(this);
					
					if (arrayMemberInfo == null || arrayMemberInfo.Length == 0) 
					{
						 // throw some error here, about not having the appropriate member
						Console.WriteLine("Warning: could not load setter \"{0}\" for {1} (type {2}), please check your vocabulary", setter, part.Identifier, tclassType.FullName); 
						return uiObject;
					}
					if (arrayMemberInfo[0] is PropertyInfo)
					{
						pInfo = (PropertyInfo)arrayMemberInfo[0];
						System.Object bla= Decoder.GetArg(p.Value, pInfo.PropertyType);
						pInfo.SetValue(targetObject, bla, null);
					}
					else
						{
							Console.WriteLine("setter \"{0}\" is a Method", setter );
							Param[] paramTypes = Voc.GetParams(p.Name, part.Class);
							//convert the params to types
							Type[] tparamTypes = new Type[paramTypes.Length];
							for(int i=0; i<paramTypes.Length; i++)
							{
								tparamTypes[i] = null;
								int k = 0;
								while(tparamTypes[i] == null)
									tparamTypes[i] = ((Assembly)ExternalLibraries.Instance.Assemblies[k++]).GetType(paramTypes[i].Type);
							   
							}
							MethodInfo m = classType.GetMethod(setter, tparamTypes);
							System.Object[] args = Decoder.GetArgs(p, tparamTypes);
	
							// We must invoke it on the target Object, not on the UI Object!							
							//m.Invoke(uiObject, args);
							m.Invoke(targetObject, args);
					   }
					}
					/*
					catch(TypeLoadException tle)
					{
						return ApplyAdHocProperty(ref uiObject, part, p);
					} 
					*/
					catch(NullReferenceException nre)
					{
						Console.WriteLine("Warning: could not load setter \"{0}\" for {1} (type {2}), please check your vocabulary", setter, part.Identifier, tclassType.FullName);
					}
					catch(Exception e)
					{
						Console.WriteLine("Setting property [{0}] with value [{1}] failed for [{2}]", setter, p.Value, part.Identifier);
						Console.WriteLine(e);
						Console.WriteLine("Trying to continue...");
					}
				return uiObject;
		}




	}
}
