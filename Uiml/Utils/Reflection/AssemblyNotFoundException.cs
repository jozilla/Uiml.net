using System;
using System.Reflection;

namespace Uiml.Utils.Reflection
{	
	public class AssemblyNotFoundException : Exception 
	{
		public AssemblyNotFoundException(string message) : base(message)
		{
		}

		public AssemblyNotFoundException(string query, Exception inner) : base(query, inner)
		{
		}
	}
}