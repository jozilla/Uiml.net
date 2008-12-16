/*
  	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)
   
	 Copyright (C) 2005  Kris Luyten (kris.luyten@uhasselt.be)
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

	Authors: 
	Kris Luyten
	Jo Vermeulen <jo.vermeulen@uhasselt.be>
*/

namespace Uiml.Executing.Callers
{
	using Uiml;
	
	using System;
	using System.Xml;
	using System.Collections;
	using System.Reflection;

	using Uiml.Rendering;
    using Uiml.Rendering.TypeDecoding;
	
	/// <summary>
	/// This class represents a caller that invokes functionality available on the same
	/// computer as the renderer.
	/// </summary>
	public class LocalCaller : Caller
	{
		public LocalCaller(Call c) : base(c)
		{}

		protected override Type[] CreateInOutParamTypes(Uiml.Param[] parameters, out Hashtable outputPlaceholder)
		{
			outputPlaceholder = null;
			Type[] tparamTypes =  new Type[parameters.Length]; 
			int i=0;
			try
			{
				for(i=0; i<parameters.Length; i++)
				{
					tparamTypes[i] = Type.GetType(parameters[i].Type);
					int j = 0;
					while(tparamTypes[i] == null)	
						tparamTypes[i] = ((Assembly)ExternalLibraries.Instance.Assemblies[j++]).GetType(parameters[i].Type);
					//also prepare a placeholder when this is an output parameter
					if(parameters[i].IsOut)
					{
						if(outputPlaceholder == null)
							outputPlaceholder = new Hashtable();
						outputPlaceholder.Add(parameters[i].Identifier, null);
					}
				}
				return tparamTypes;
			}
				catch(ArgumentOutOfRangeException aore)
				{
					Console.WriteLine("Can not resolve type {0} of parameter {1} while calling method {2}",parameters[i].Type ,i , Call.Name);
					Console.WriteLine("Trying to continue without executing {0}...", Call.Name);
					throw aore;					
				}
		}
	
		public object ExecuteMethod(string concreteMethodName, Type objectType, out Hashtable outputParams)
		{
			// static methods
			return ExecuteMethod(concreteMethodName, objectType, null, out outputParams);
		}

		public object ExecuteMethod(string concreteMethodName, Type objectType, object obj/*, Logic l*/, out Hashtable outputParams)
		{
			Uiml.Param[] parameters;
			Hashtable outputPlaceholder = null;
			
			//if(l == null)
				parameters = Call.Renderer.Voc.GetMethodParams(Call.ObjectName, Call.MethodName);
			//else
			//	parameters = l.GetMethodParams(Call.ObjectName, Call.MethodName);

			//convert the params to types
			Type[] tparamTypes = null;
			try 
			{
				tparamTypes = CreateInOutParamTypes(parameters, out outputPlaceholder);
	    		}
			catch (ArgumentOutOfRangeException) 
	    		{
				outputParams = outputPlaceholder; // otherwise we get a compile error from Visual Studio
				return null; 
			}
			
			MethodInfo m = objectType.GetMethod(concreteMethodName, tparamTypes);
			System.Object[] args = new System.Object[tparamTypes.Length];

			for(int k=0; k<args.Length; k++)
			{
				//String propValue = (string)((Uiml.Executing.Param)Call.Params[k]).Value(Call.Renderer);
                // don't use strings, but the real complex types!
                object propValue = ((Uiml.Executing.Param)Call.Params[k]).Value(Call.Renderer);
				args[k] = TypeDecoder.Instance.GetArg(propValue, tparamTypes[k]);
			}
			
			Object result = null;
			try
			{
				result = m.Invoke(obj, args);
			}
			catch(System.Reflection.TargetInvocationException tie)
			{
				Console.WriteLine(tie);
				Console.WriteLine("Error while executing \"{0}\" from \"{1}\"; please check this method", m, obj.GetType());
				throw tie;
			}

			//get the updated parameters out of args
			for(int i=0; i<parameters.Length; i++)
				if(parameters[i].IsOut)
				{
					outputPlaceholder[parameters[i].Identifier] = args[i];
				}
			outputParams = outputPlaceholder;
			
			
			return result;
		}		

		public object ExecuteProperty(string concreteMethodName, Type objectType)
		{
			PropertyInfo pi = objectType.GetProperty(concreteMethodName);
			return pi.GetValue(null, null);
		}

		public object ExecuteField(string concreteMethodName, Type objectType)
		{
			FieldInfo fi = objectType.GetField(concreteMethodName);
			return fi.GetValue(null);
		}

		public object ExecuteProperty(string concreteMethodName, Type objectType, object obj)
		{
			PropertyInfo pi = objectType.GetProperty(concreteMethodName);
			return pi.GetValue(obj, null);
		}

		public object ExecuteField(string concreteMethodName, Type objectType, object obj)
		{
			FieldInfo fi = objectType.GetField(concreteMethodName);
			return fi.GetValue(obj);
		}

		///<summary>
		/// It loads a method and its params from this call and executes it at runtime
		///</summary>
		public override object Execute(out Hashtable outputParams)
		{
			outputParams = null; // when something goes wrong, it still has a value
			
			if(Call.Name == null || Call.Name == "" || Call.Name == ".")
			{ //it is an "internal script"
				ArrayList l = Call.Children;
				Object script2 = null;				
				foreach(Object script in l)
				{
					script2 = script;
					((IExecutable)script).Execute();
				}				
				//only return the return value of the last script
				try
				{
					return ((Script)script2).ReturnValue;
				}
				catch(NullReferenceException nre)
				{
					Console.WriteLine("No script found while trying to Execute.");
					Console.WriteLine(nre);
				}
			}
			
			Type type = null;				
		
			try
			{
				//is the function available in the vocabulary or in one of the logic components?
				string concreteObjectName = null; 
				string concreteMethodName = null;
				
	
				concreteObjectName = Call.Renderer.Voc.MapsOnCmp(Call.ObjectName);
				concreteMethodName = Call.Renderer.Voc.GetMethodCmp(Call.MethodName, Call.ObjectName);

				if(Call.Connected)
				{	
					IEnumerator oe = Call.ConnectedObjects.GetEnumerator();
					object obj = null, result = null;
					while(oe.MoveNext())
					{
						obj = oe.Current;						
						if(obj.GetType().FullName == concreteObjectName)
						{
							type = obj.GetType();
							try { return ExecuteMethod(concreteMethodName, type, obj, out outputParams); } 
							catch(NullReferenceException)//method failed, try property
							{
									try { return ExecuteProperty(concreteMethodName, type,obj); }
								  //property failed, try field
							  catch(NullReferenceException)	{ return ExecuteField(concreteMethodName, type, obj); 	}
							}
						}						
					}
					
					if(type != null)
					{
						// FIXME: only returns the value of the last connected object's method invocation
						return result; 
					}
				}

				//first check whether the required object is an individual connected object
				IEnumerator enumObjects = ExternalObjects.Instance.LoadedObjects;				
				while((enumObjects.MoveNext()) && (type==null))
				{			
					Object obj = enumObjects.Current;
                    Type objType = obj.GetType();
                    Type mappedType = null;
                    foreach (Assembly a in ExternalLibraries.Instance.Assemblies)
                    {
                        try
                        {
                            mappedType = a.GetType(concreteObjectName, true);
                            break;
                        }
                        catch
                        {
                        }
                    }

                    // subclasses are OK as well!
                    if (objType.Equals(mappedType) || objType.IsSubclassOf(mappedType))
                    {
                        type = objType;

						try { return ExecuteMethod(concreteMethodName, type, obj, out outputParams); } 
						catch(NullReferenceException nre)//method failed, try property
						{
								try { return ExecuteProperty(concreteMethodName, type,obj); }
							  //property failed, try field
						  catch(NullReferenceException)	{ return ExecuteField(concreteMethodName, type, obj); }
						}
					}	
				}

			
				//If type is still null, try to find the concrete functionality in a library that has been loaded
				if(type == null)
				{
							IEnumerator enumAssemblies = ExternalLibraries.Instance.LoadedAssemblies;
					while((enumAssemblies.MoveNext()) && (type==null))
					{
						Assembly a = (Assembly)enumAssemblies.Current;
						#if !COMPACT
						type = a.GetType(concreteObjectName, false, true);
						#else
						type = a.GetType(concreteObjectName, false);
						#endif
					}
				}

				
				if(type == null)
				{
					Console.WriteLine("Could not find type {0} ({1}) -- aborting call execution", concreteObjectName, Call.ObjectName);
					return null;	
				}

                object res = null;

                try
                {
                    res = ExecuteMethod(concreteMethodName, type/*, logic*/, out outputParams);
                    return res;
                }
                catch (NullReferenceException)//method failed, try property
                {
                    try
                    {
                        return ExecuteProperty(concreteMethodName, type);
                    }
                    //property failed, try field
                    catch (NullReferenceException)
                    {
                        return ExecuteField(concreteMethodName, type);
                    }
                }
                #if COMPACT
                catch (Exception e)
                {
                    if (res == null)
                    {
                        // non-static method: try to create default object
                        object obj = Activator.CreateInstance(type);
                        Call.Connect(obj);
                        return ExecuteMethod(concreteMethodName, type, obj, out outputParams);
                    }
                    else
                        throw e;
                }
                #else
                catch (TargetException)
                {
                    // non-static method: try to create default object
                    object obj = Activator.CreateInstance(type);
                    Call.Connect(obj);
                    return ExecuteMethod(concreteMethodName, type, obj, out outputParams);
                }
                #endif
            }
            catch(MappingNotFoundException mnfe) 
			{
				Console.WriteLine("Vocabulary error: {0}", mnfe);
				return null;
			}
			catch(TargetInvocationException tie)
			{
				Console.WriteLine("Unable to execute call {0} on type {1}", Call.Name, type);
				Console.WriteLine("Reason:{0}", tie);
				return null; //replace by exception???
			}
			catch(Exception e)
			{
				Console.WriteLine("Unable to execute call {0}", Call.Name);
				Console.WriteLine("Reason:{0}", e);
				return null; //replace by exception???
			}
		}
	}
}

