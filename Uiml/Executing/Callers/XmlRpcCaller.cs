/*
  	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)
   
	 Copyright (C) 2003  Kris Luyten (kris.luyten@uhasselt.be)
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

	Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>
*/

using System;
using System.Collections;
using System.Reflection;
using Nwc.XmlRpc;

namespace Uiml.Executing.Callers
{
	/// <summary>
	/// This class represents a remote caller, using the XML-RPC protocol. It is the opposite of LocalCaller.
	/// </summary>
	public class XmlRpcCaller : Caller
	{
		private XmlRpcRequest m_request;
		private XmlRpcResponse m_response;
		private string m_url;
		
		public XmlRpcCaller(Call c, string url) : base(c)
		{
			m_url = HTTP_PREFIX + url;
		}

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
		
		public override object Execute(out Hashtable outputParams)
		{
			// assign output params
			outputParams = null;
			
			Hashtable outputPlaceholder = null;

			// create new XmlRpcRequest object
			XmlRpcRequest client = new XmlRpcRequest();

			// fill in the concrete method name
			client.MethodName = Call.Renderer.Voc.GetMethodCmp(Call.MethodName, Call.ObjectName);	
			
			// provide the input parameters	
			Uiml.Param[] parameters = Call.Renderer.Voc.GetMethodParams(Call.ObjectName, Call.MethodName);
			
			Type[] tparamTypes = null;
			try
			{
				tparamTypes = CreateInOutParamTypes(parameters, out outputPlaceholder);
			}
			catch(ArgumentOutOfRangeException) 
			{ 
				return null; 
			}
			
			for (int k = 0; k < parameters.Length; k++)
			{
				string propValue = (string) ((Uiml.Executing.Param) Call.Params[k]).Value(Call.Renderer);
				client.Params.Add(Call.Renderer.Decoder.GetArg(propValue, tparamTypes[k]));
			}

			if (m_request == null || !client.ToString().Equals(m_request.ToString()))
			{
				XmlRpcResponse response = client.Send(m_url);
				
				// cache response and request
				m_request = client;
				m_response = response;
			}
			
			if (m_response.IsFault)
			{
				Console.WriteLine("Fault {0}: {1}", m_response.FaultCode, m_response.FaultString);
				return null;
			}
			else
			{
				return m_response.Value;
			}
		}

		public const string HTTP_PREFIX = "http://";
	}
}
