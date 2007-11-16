/*
 *   Uiml.Net: a Uiml.Net renderer 
 *   http://research.edm.uhasselt.be/~uiml/
 *   
 *   Copyright (C) 2003-2007  Expertise Centre for Digital Media
 *                            Hasselt University
 *                            http://edm.uhasselt.be/
 *                       
 *   This program is free software; you can redistribute it and/or
 *   modify it under the terms of the GNU Lesser General Public License
 *   as published by the Free Software Foundation; either version 2.1
 *   of	the License, or (at your option) any later version.
 *   
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Lesser General Public License for more details.
 *   
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program; if not, write to the Free Software
 *   Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 *   Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Uiml.Rendering.TypeDecoding
{
	/// <summary>
    /// Class to register arbitrary functions that convert from one type to 
    /// another. This way type decoders can be added on the fly. Every backend
    /// just registers its own type decoding functions with this class, thereby
    /// removing the need for backend-specific type decoders.
    /// </summary>
    public class TypeDecoderRegistry
    {
        private Dictionary<Signature, List<Delegate>> m_decoders;
                        
		public TypeDecoderRegistry() 
		{
		    m_decoders = new Dictionary<Signature, List<Delegate>>();
		}
		
		public void Register(Type t)
		{   
		    // register all public and static methods
		    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
		    {
    		    // only methods with the typedecoder attribute will be registered
		        Register(m); 
		    }
		}
		
		public void Register(Assembly a)
		{
		    // register all types in a certain assembly
		    foreach (Type t in a.GetTypes())
		    {
		        Register(t);
		    }
		}

		/// <summary>
		/// Register a new function for type decoding. The from and to types are
		/// automatically extracted.
		/// </summary>
	    /// <param name="method">
	    /// The function we want to register as a type decoder.
	    /// </param>		
		/// <exception cref="Uiml.Rendering.InvalidTypeDecoderException"/>
		/// Thrown when <paramref>method</paramref> does not have exactly one
		/// argument and a non-void return type.
		/// </exception>
		public void Register(MethodInfo method)
		{
		    if (!(method.IsPublic && method.IsStatic))
		    {
		        throw new InvalidTypeDecoderMethodException(
		            "Only public and static methods are supported",
		            method
		        );
		    }
		
		    // search for attributes
	        object[] attrs = method.GetCustomAttributes(true);

	        foreach (object a in attrs)
	        {
	            if (a is TypeDecoderMethodAttribute) // found one
	            {
	                TypeDecoderMethodAttribute attr = (TypeDecoderMethodAttribute) a;
	                
                    try
                    {
                        // register it
                        Register(method, attr.Dependency); 
                        return;
                    }
                    catch (InvalidTypeDecoderMethodException ide)
                    {
                        //TODO
                        Console.WriteLine(ide);
                    }
                }
	        }
		}		
		
		/// <summary>
		/// Register a new function for type decoding. The from and to types are
		/// automatically extracted.
		/// </summary>
	    /// <param name="method">
	    /// The function we want to register as a type decoder.
	    /// </param>		
	    /// <param name="dependency">
	    /// The signature of another typedecoder that we depend on.
	    /// </param>
		/// <exception cref="Uiml.Rendering.InvalidTypeDecoderException"/>
		/// Thrown when <paramref>method</paramref> does not have exactly one
		/// argument and a non-void return type.
		/// </exception>
		protected void Register(MethodInfo method, Signature dependency)
		{
		    // if it's a valid (non-null) signature, and no decoder can be found
		    if (dependency.IsValid && !HasDecoder(dependency))
		    {
		        // TODO: throw exception
		        Console.WriteLine("Could not add {0} because dependency {1} could not be found",
		                          method, dependency);
		    }
		    
		    try
		    {
		        // get return type
		        Type to = method.ReturnType;
		        // get first parameter
		        Type from = method.GetParameters()[0].ParameterType;
		        // we use the System.Convert<TInput, TOutput> generic delegate
		        Type delegateType = typeof(Converter<,>).MakeGenericType(from, to);
		        
		        // create the delegate
		        #if COMPACT
		        // this requires .NET CF 3.5!
                Delegate d = Delegate.CreateDelegate(delegateType, null, method);
		        #else
		        Delegate d = Delegate.CreateDelegate(delegateType, method);
		        #endif
		        
    		    // create a signature object to serve as a key for 
    		    // the decoders dictionary
    		    Signature sig = new Signature(from, to);
		        
		        // Add it to the dictionary
		        AddDecoder(sig, d);
		    }
		    catch (Exception e)
		    {
		        // TODO: throw InvalidTypeDecoderMethodException
		        Console.WriteLine(e);
		    }
		}
		
		protected void AddDecoder(Signature sig, Delegate d)
		{
		    if (m_decoders.ContainsKey(sig))
		    {
		        // add it to the existing list
		        m_decoders[sig].Add(d);
		    }
		    else
		    {
		        // create a new list containing the delegate
		        List<Delegate> l = new List<Delegate>();
		        l.Add(d);
		        m_decoders[sig] = l;
		    }
		}
	    
	    /// <summary>
	    /// Get a list of all decoder functions corresponding for these types.
	    /// </summary>
	    /// <param name="from">The type that we want to decode from</param>
  	    /// <param name="to">The type that we want to decode to</param>
	    /// <returns>
	    /// The list of matching decoder functions.
	    /// </returns>
	    /// <remarks>
        /// Internally, this list consists of 
        /// <see cref="System.Convert<TInput, TOutput>"/> delegates.
        /// </remarks>
		public List<Delegate> FindAll(Type from, Type to)
		{
		    return FindAll(new Signature(from, to));
		}
		
		public List<Delegate> FindAll(Signature sig)
		{
            if (m_decoders.ContainsKey(sig))
            {
                // return the list of delegates according to the given signature
                return m_decoders[sig];
            }
            else
            {
    		    // FIXME: make sure that we also can search on subclasses,
    		    // e.g. object <=> string should work on everything!

                // return an empty list
                return new List<Delegate>();
            }
		}

	    /// <summary>
	    /// Get a list of decoder function pairs that can be invoked in
        /// sequence to get to the requested conversion. 
	    /// </summary>
	    /// <param name="from">The type that we want to decode from</param>
  	    /// <param name="to">The type that we want to decode to</param>
	    /// <returns>
	    /// The list of matching decoder function pairs to invoke in
        /// sequence.
	    /// </returns>
		public List<Signature[]> FindIndirect(Type from, Type to)
		{
		    return FindIndirect(new Signature(from, to));
		}

        public List<Signature[]> FindIndirect(Signature sig)
        {
            // we need to go from sig.From to sig.To
            // => look for two decoders to do this:
            // 
            // [sig.From => unknown] and [unknown => sig.To].
            // 
            // TODO: allow multiple levels (although this might
            // become inefficient). In fact that would be pretty
            // simple:
            //
            // suppose we have these decoders and there is no 
            // decoder [A => B] available:
            //
            // [sig.From => A] and [B => sig.To]
            //
            // We need to repeat the process we perform now to 
            // search for an A => B decoder. If that doesn't work out,
            // we continue with all other combinations.

            List<Signature> toUnknown = new List<Signature>();
            List<Signature> fromUnknown = new List<Signature>();
            List<Signature[]> decoderPairs = new List<Signature[]>(); 

            foreach (Signature s in m_decoders.Keys)
            {
                if (s.From == sig.From)
                    toUnknown.Add(s);

                if (s.To == sig.To)
                    fromUnknown.Add(s);
            }

            foreach (Signature tu in toUnknown)
            {
                foreach (Signature fu in fromUnknown)
                {
                    if (tu.To == fu.From)
                    {
                        // found one!
                        decoderPairs.Add(new Signature[] { tu, fu });
                    }
                }
            }

            return decoderPairs;
        }

	    /// <summary>
	    /// Checks whether a decoder exists for a certain signature.
	    /// </summary>
	    /// <param name="from">The type that we want to decode from</param>
  	    /// <param name="to">The type that we want to decode to</param>
	    /// <returns>
	    /// True if a decoder was found.
	    /// </returns>
		public bool HasDecoder(Type from, Type to)
		{
		    return HasDecoder(new Signature(from, to));
		}
		
		public bool HasDecoder(Signature s)
		{
		    return m_decoders.ContainsKey(s);
		}
    }
}
