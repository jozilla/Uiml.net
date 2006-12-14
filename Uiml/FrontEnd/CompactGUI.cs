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
		//static public string UIMLFILE = @"compactgui.uiml";
		static public string UIMLFILE = @"\\Program Files\\uimldotnetcf\\compactgui.uiml";
		static public string UIMLLIB = @"uimldotnetcf";
		public const string SWF_ASSEMBLY = "System.Windows.Forms";

		public CompactGUI() : base(UIMLFILE, UIMLLIB)
		{
		}

		public static void Main(string[] args)
		{
			new CompactGUI();
		}

		public void AddLibrary(String lib)
		{
		}

		//[UimlEventHandler("ButtonPressed")]
		public override void OpenUimlFile()
		{
			//dynamically load the code to create "OpenFileDialog"
			Assembly guiAssembly = AssemblyLoader.LoadFromGacOrAppDir(SWF_ASSEMBLY);
			//OpenFileDialog ofd = new OpenFileDialog();
			Type ofClassType = guiAssembly.GetType("System.Windows.Forms.OpenFileDialog");
			Object fs = Activator.CreateInstance(ofClassType, null);
			//ofd.Filter = "UIML files (*.uiml)|*.uiml|All files (*.*)|*.*" ;
			PropertyInfo filterMe = ofClassType.GetProperty("Filter");
			filterMe.SetValue(fs, "UIML files (*.uiml)|*.uiml|All files (*.*)|*.*" , null);
		
	
			//if(ofd.ShowDialog() == DialogResult.OK)
			MethodInfo runner = ofClassType.GetMethod("ShowDialog");
			Object o = runner.Invoke(fs, null);
			Object ok = (guiAssembly.GetType("System.Windows.Forms..DialogResult")).GetProperty("OK").GetValue(null, null);
			
			//	UimlFileName = ofd.FileName;
			if(ok == o)
			{
				PropertyInfo fname = ofClassType.GetProperty("FileName");
				UimlFileName = (string)fname.GetValue(fs, null);
			}

			//fs.Hide();


		}


	}
}
