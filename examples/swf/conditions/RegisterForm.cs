/* To compile this, link with the uiml.net-core library. On Mono you
 * would do something like:
 *
 * $ gmcs -target:library -r:uiml.net-core.dll RegisterForm.cs
 *
 */ 

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Uiml;

public class RegisterForm
{
    public RegisterForm()
    {

    }

    public static void ShowMessageBox(string msg)
    {
        Type mBox = GuiAssembly.GetType("System.Windows.Forms.MessageBox");
        MethodInfo mi = mBox.GetMethod("Show", new Type[] { typeof(string) });
        mi.Invoke(null, new object[] { msg });
    }

    public static Assembly GuiAssembly
    {
        get
        {
            return ExternalLibraries.Instance.GetAssembly("System.Windows.Forms");
        }
    }
}
