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


namespace Uiml.Rendering
{
	using System;
	using System.Collections;
	using System.Reflection;

	public abstract class TypeDecoder : ITypeDecoder
	{	
		public object GetArg(object o, Type t)
		{
			if (t.IsPrimitive)
			{
				return ConvertPrimitive(t, o);
			}
			else
			{
				return ConvertComplex(t, o);
			}
		}

		public object[] GetArgs(Property p, Type[] types)
		{
		
			object[] args = new object[types.Length];
					
			for (int i = 0; i < types.Length; i++)
			{   
				if (types[i].IsPrimitive)
					args[i] = ConvertPrimitive(types[i], p);
				else
					args[i] = ConvertComplex(types[i], p);
			}
			
			return args;
		}
	
		///<summary>
		/// Given an array of properties and an array of types, this method will
		/// create an array of objects by converting each property value 
		/// (p[i].Value) into its appropriate type according to the Type array 
		/// (types[i]).
		///</summary>
		public object[] GetMultipleArgs(Property[] p, Type[] types)
		{
			object[] args= new object[types.Length];
			
			for (int i = 0; i < types.Length; i++)
			{   
				if (types[i].IsPrimitive)
					args[i] = ConvertPrimitive(types[i], p[i]);
				else
					args[i] = ConvertComplex(types[i], p[i]);
			}
			
			return args;
		}

		/// <summary>
		/// Helper function to convert a property's value to a primitive type 
		/// (e.g. char, int, float, ...).
		/// </summary>
		protected object ConvertPrimitive(Type t, Property p)
		{
			return ConvertPrimitive(t, (System.String)p.Value);
		}
		
		/// <summary>
		/// Utility function to convert an arbitrary object to a primitive type 
		/// using the object's Parse method (like the one in System.String).
		/// </summary>
		protected object ConvertPrimitive(Type t, System.Object oValue)
		{
			string value = (string)oValue;
			if(oValue is string)
				value = (string)oValue;
			else if(t.FullName == "System.String")
				return oValue.ToString();
					
			try
			{
				MethodInfo method = t.GetMethod(PARSE, new Type [] { value.GetType() });
				return method.Invoke(null, new System.Object [] { value } );
			}
			catch(Exception e)
			{
				return value;
			}
		}
		
		/// <summary>
		/// Helper function to convert a property to a complex type.
		/// </summary>
		protected abstract object ConvertComplex(Type t, Property p);
		
		/// <summary>
		/// Utility function to convert an arbitrary object to a complex type.
		/// </summary>
		protected abstract object ConvertComplex(Type t, object oValue);
		
		/// <summary>
		/// Utility function to convert a UIML constant to a string array.
		/// </summary>
		protected string[] DecodeStringArray(Constant constant)
		{
			ArrayList strArrayList = new ArrayList();
			IEnumerator enumConstants = constant.Children.GetEnumerator();
			while(enumConstants.MoveNext())
			{
				Constant child = (Constant)enumConstants.Current;
				strArrayList.Add(child.Value);
			}
			return (string[]) (strArrayList.ToArray(Type.GetType("System.String")));
		}

		
		public static string PARSE = "Parse";
	}

}

