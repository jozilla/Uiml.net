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

	Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>
*/

using System;
using System.Collections;
using System.Reflection;
using Uiml;
using Uiml.Peers;

namespace Uiml.Executing.Callers
{
	public class CallerFactory
	{
		private Call m_call;
		
		public CallerFactory(Call c)
		{
			m_call = c;
		}
		
		/// <summary>
		/// Creates a caller for a given &lt;call&gt; tag. 
		/// </summary>
		public Caller CreateCaller()
		{

			// get location from vocabulary
			try{
				Location l = Call.Renderer.Voc.GetLocationCmp(Call.ObjectName);			
				if (l == null) // no location specified
					return new LocalCaller(Call); // local by default
				else
				{	
					switch(l.Type)
					{
						case Location.Protocol.XmlRpc:
							return LoadCaller(XML_RPC_LIB, XML_RPC_CALLER, new object[] { Call, l.Value });
						//case Location.Protocol.Soap:
							// TODO
							//return null;
						case Location.Protocol.Local:
							return new LocalCaller(Call);
						default:
							return null;
					}
				}
			}
			catch(MappingNotFoundException nfe)									
			{
						//could be an inline script
						return  new LocalCaller(Call);
			}
		}
		
		private Caller LoadCaller(string lib, string caller, object[] parameters)
		{
			Caller result = null;

			//Console.Write("Looking for {0} library... ", lib);
			try
			{
				Assembly a = Assembly.LoadWithPartialName(lib);
				Console.Write("Dynamically loading XML-RPC library... ");
				Console.WriteLine("OK!");

				//Console.Write("Loading caller {0}... ", caller);
				Console.Write("Dynamically loading XML-RPC caller... ");
				Type t = a.GetType(caller);
				result = (Caller) Activator.CreateInstance(t, parameters);
				Console.WriteLine("OK!");
			}
			catch(Exception e)
			{
				Console.WriteLine("FAILED!");
				Console.WriteLine("Trying to continue...");
			}

			return result;
		}

		public Call Call
		{
			get { return m_call; }
			set { m_call = value; }
		}

		public const string XML_RPC_LIB = "uiml-xml-rpc";
		public const string XML_RPC_CALLER = "Uiml.Executing.Callers.XmlRpcCaller";
	}
}
