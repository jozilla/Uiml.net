using System;
using System.Reflection;
using System.IO;

namespace Uiml.Utils.Reflection
{
	public class TypeLoader
	{
        public static Type LoadType(string name)
        {
            Type t = Type.GetType(name, false);

            if (t != null)
                return t;
            else
            {
                // try again, cycle through all assemblies
                foreach (Assembly a in ExternalLibraries.Instance.Assemblies)
                {
                    t = a.GetType(name, false);
                    if (t != null)
                        return t;
                }

                return t; // if we get here: t == null
            }
        }
	}
}
