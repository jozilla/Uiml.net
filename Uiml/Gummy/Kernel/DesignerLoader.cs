using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Uiml.Gummy.Serialize;
using Uiml.Utils.Reflection;

namespace Uiml.Gummy.Kernel
{
    public class DesignerLoader
    {
        public String[] assemblies = { 
                                              "gummy-swf",
                                              "gummy-swing"
                                             };

        public String[] designers = { 
                                              "Uiml.Gummy.Serialize.SWF.SWFUimlSerializer",
                                              "Uiml.Gummy.Serialize.Swing.SwingUimlSerializer"
                                             };

        public const string NAME = "NAME";
        
        public DesignerLoader()
        {
        }

        public IUimlSerializer CreateSerializer(string name)
        {
            Console.WriteLine("Looking for {0} proxy object", name);
            for (int i = 0; i < designers.Length; i++)
            {
                try
                {
                    Assembly a = AssemblyLoader.LoadAny(assemblies[i]);
                    Type t = a.GetType(designers[i]);
                    FieldInfo m = t.GetField(NAME);
                    String dynname = (String)m.GetValue(t);
                    Console.Write("Proxy object for {0} platform", dynname);
                    if (dynname == name)
                    {
                        Console.WriteLine("...match. OK! Loading proxy object {0}.", t);
                        return (IUimlSerializer)Activator.CreateInstance(t);
                    }
                    else
                        Console.WriteLine("...no match with {0}", name);
                }
                catch (Exception e)
                {
                    // do nothing here: an exception means the backend renderer specified 
                    // in assemblies[i] or one of its dependencies is not available
                    // Console.WriteLine(e);
                }
            }

            return null;
        }
    }
}
