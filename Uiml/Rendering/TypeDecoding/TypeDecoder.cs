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


namespace Uiml.Rendering.TypeDecoding
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	
	/// <summary>
    /// Class to convert between different types, amongst others to convert
    /// (typeless) data values from the UIML document to the types required by
    /// the specific toolkit (e.g. '255,0,0' to an instance of 
    /// <see cref="System.Drawing.Color"/>).
    /// </summary>
    /// <remarks>
    /// Implements the Singleton pattern. 
    /// <seealso cref="http://msdn2.microsoft.com/en-us/library/ms954629.aspx"/>
    /// </remarks>
	public sealed class TypeDecoder : ITypeDecoder
	{
	    protected TypeDecoderRegistry m_registry;
	
	    protected TypeDecoderRegistry Registry
	    {
	        get { return m_registry; } 
	        set { m_registry = value; }
	    }
	
        // Singleton
        public static readonly TypeDecoder Instance = new TypeDecoder();
                
		private TypeDecoder() 
		{
		    Registry = new TypeDecoderRegistry();
		}
		
		public void Register(Type t)
		{
		    Registry.Register(t);
		}
		
		public void Register(Assembly a)
		{
		    Registry.Register(a);
		}
		
		/// <summary>
		/// Register a new function for type decoding. The from and to types are
		/// automatically extracted.
		/// </summary>
	    /// <param name="method">
	    /// The function we want to register as a type decoder.
	    /// </param>
		/// <exception cref="Uiml.Rendering.TypeDecoderRegistry.InvalidDecoderException"/>
		/// Thrown when <paramref>method</paramref> does not have exactly one
		/// argument and a non-void return type.
		/// </exception>
		public void Register(MethodInfo method)
		{
		    Registry.Register(method);
		}
	
		public object GetArg(object o, Type t)
		{
            object result;

			if (t.IsPrimitive)
			{
				result = ConvertPrimitive(t, o);
			}
			else
			{
				result = ConvertComplex(t, o);
			}

            if (result == null)
            {
                try
                {
                    result = SystemConvert(t, o);
                }
                catch (Exception e)
                {
                    // ignore
                }
            }

            return result;
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
		protected object ConvertComplex(Type t, Property p)
		{
            return ConvertComplex(t, p.Value);
		}
		
		/// <summary>
		/// Utility function to convert an arbitrary object to a complex type.
		/// </summary>
		protected object ConvertComplex(Type t, object oValue)
		{
		    if (t == oValue.GetType())
		        return oValue;    
		    
		    object returnVal = null;
		    
		    // get all type conversions from oValue's type to to Type t
		    List<Delegate> decoders = Registry.FindAll(oValue.GetType(), t);

            // if we find no decoders for these types, try to find
            // decoder combinations to solve it. 
            //
            // A possible use case is converting from a type to a
            // Constant, instead of converting to a widget
            // set-specific container 
            // (e.g. System.Windows.Forms.ListViewItem[]).
            if (decoders.Count == 0)
            {
                // perform a deep search
                List<Signature[]> sigs = Registry.FindIndirect(oValue.GetType(), t);
                foreach (Signature[] pair in sigs)
                {
                    List<Delegate> toUnknown = Registry.FindAll(pair[0]);
                    List<Delegate> fromUnknown = Registry.FindAll(pair[1]);

                    // go through the entire list, normally this will
                    // only contain one element, but if it does not,
                    // we will continue to loop until we find a working
                    // conversion.
                    foreach (Delegate tu in toUnknown)
                    {
                        foreach (Delegate fu in fromUnknown)
                        {
                            try
                            {
                                // invoke the decoders in sequence
                                return fu.Method.Invoke(fu.Target, new object[] { 
                                    tu.Method.Invoke(tu.Target, new object[] { oValue })
                                    });
                            }
                            catch (Exception e)
                            {
                                // TODO
                                Console.WriteLine(e);
                            }
                        }
                    }

                }
            }

		    foreach (Delegate d in decoders)
		    {
		        try
		        {
		            // FIXME: log this
		            //Console.WriteLine("found delegate {0}, invoking it", d);
    		        return d.Method.Invoke(d.Target, new object[] { oValue });
    		    }
    		    catch (Exception e)
    		    {
    		        // TODO
    		        Console.WriteLine(e);
    		    }
		    }
		    
            return null;
		}

        protected object SystemConvert(Type t, object o)
        {
            Type convertType = Type.GetType("System.Convert");
            string methodName = string.Format("To{0}", t.Name);
            MethodInfo convertMethod = convertType.GetMethod(methodName, new Type[] { o.GetType() });
            return convertMethod.Invoke(null, new object[] { o });
        }
		
		public static string PARSE = "Parse";
	}

}

