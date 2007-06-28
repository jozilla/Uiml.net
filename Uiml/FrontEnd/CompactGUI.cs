/*
    Uiml.Net: a .Net UIML renderer (http://research.edm.uhasselt.be/kris/research/uiml.net)

	 Copyright (C) 2004  Kris Luyten (kris.luyten@uhasselt.be)
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

//[assembly: System.Reflection.AssemblyKeyFile ("uiml.net.snk")]



namespace Uiml.FrontEnd{

	using System;
	using System.Collections;
    using System.IO;
	using System.Reflection;
	using System.Xml;
	
	using Uiml.Utils.Reflection;
	using Uiml.Rendering;
	using Uiml.Executing;
	using Uiml.Executing.Binding;

	/// <summary>
	/// GUI front-end to select an UIML file for the Pocket PC
	/// </summary>
	public class CompactGUI : UimlFrontEnd
	{
		static public string UIMLFILE = "compactgui.uiml";
		static public string UIMLLIB = "uiml.net-core-cf";
		public const string SWF_ASSEMBLY = "System.Windows.Forms";
        public const string EXAMPLES_SUBDIR = "uiml.net-cf_examples";

        public static string examplesDir;

		public CompactGUI() : base(UIMLFILE, UIMLLIB)
		{
		}

        static CompactGUI()
        {
            string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            examplesDir = Path.Combine(myDocs, EXAMPLES_SUBDIR + Path.DirectorySeparatorChar);

            // copy the examples to the \My Documents folder first
            CopyExamples();
        }

		public static void Main(string[] args)
		{
			new CompactGUI();
		}

        private static void CopyExamples()
        {
            // create target directory if not exists
            if (!Directory.Exists(examplesDir))
                Directory.CreateDirectory(examplesDir);

            // copy all files
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            string appExamplesDir = Path.Combine(Path.Combine(appDir, "examples"), "swf-cf");

            foreach (string filename in Directory.GetFiles(appExamplesDir, "*"))
            {
                string targetFilename = Path.Combine(examplesDir, Path.GetFileName(filename));
                File.Copy(filename, targetFilename, true);
            }
        }

        public override Assembly GuiAssembly
        {
            get
            {
                return ExternalLibraries.Instance.GetAssembly(SWF_ASSEMBLY);
            }
        }

		public void AddLibrary(String lib)
		{
		}

		//[UimlEventHandler("ButtonPressed")]
		public override void OpenUimlFile()
		{
            //OpenFileDialog ofd = new OpenFileDialog();
            Type ofClassType = GuiAssembly.GetType("System.Windows.Forms.OpenFileDialog");
			Object fs = Activator.CreateInstance(ofClassType);
			
            //ofd.Filter = "UIML files (*.uiml)|*.uiml|All files (*.*)|*.*" ;
			PropertyInfo filterMe = ofClassType.GetProperty("Filter");
			filterMe.SetValue(fs, "UIML files (*.uiml)|*.uiml|All files (*.*)|*.*" , null);
	
            // set initial directory to examples dir 
            // ('\My Documents\{EXAMPLES_SUBDIR}\')
            string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string examplesDir = Path.Combine(myDocs, EXAMPLES_SUBDIR);
            //ofd.InitialDirectory = examplesDir;
            PropertyInfo initDir = ofClassType.GetProperty("InitialDirectory");
            initDir.SetValue(fs, examplesDir, null);

			//if(ofd.ShowDialog() == DialogResult.OK)
			MethodInfo runner = ofClassType.GetMethod("ShowDialog");
			Object o = runner.Invoke(fs, null);

            Type tdr = GuiAssembly.GetType("System.Windows.Forms.DialogResult");
			FieldInfo fInfo = tdr.GetField("OK");
			Object ok = fInfo.GetValue(o);
			
			//	UimlFileName = ofd.FileName;
			if(ok.Equals(o))
			{
				PropertyInfo fname = ofClassType.GetProperty("FileName");
				UimlFileName = (string)fname.GetValue(fs, null);
			}

			//fs.Hide();
		}

        public override void Quit()
        {
            Type app = GuiAssembly.GetType("System.Windows.Forms.Application");

            MethodInfo exitInfo = app.GetMethod("Exit", new Type[] {} );
            exitInfo.Invoke(null, null);
        }
	}
}
