/*
    Uiml.Net: a .Net UIML renderer (http://research.edm.luc.ac.be/kris/research/uiml.net)

	 Copyright (C) 2004  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

//[assembly: System.Reflection.AssemblyKeyFile ("uiml.net.snk")]

namespace Uiml.FrontEnd
{

	using System;
	using System.Collections;
	using System.Reflection;
	using Uiml.Rendering;
	using Uiml.Executing;
	using Uiml.Executing.Binding;
	using System.Xml;

	/// <summary>
	/// GUI front-end to select an UIML file for SWF
	/// </summary>
	public class SwfGUI : UimlFrontEnd
	{
		static public string UIMLFILE = @"compactgui.uiml";
		static public string UIMLLIB=@"uimldotnet.dll";
		public const string SWF_ASSEMBLY = "System.Windows.Forms";

		public SwfGUI() : base(UIMLFILE, UIMLLIB)
		{
		}
	
		public void AddLibrary(String lib)
		{
		}

		//[UimlEventHandler("ButtonPressed")]
		public override void OpenUimlFile()
		{
			//dynamically load the code to create "OpenFileDialog"
			Assembly guiAssembly = Assembly.LoadWithPartialName(SWF_ASSEMBLY);
			if(guiAssembly == null) //should never happen!
				Console.WriteLine("Can not find SWF Assembly");

			//OpenFileDialog ofd = new OpenFileDialog();
			Type ofClassType = guiAssembly.GetType("System.Windows.Forms.OpenFileDialog");
			Object fs = Activator.CreateInstance(ofClassType, null);
			//ofd.Filter = "UIML files (*.uiml)|*.uiml|All files (*.*)|*.*" ;
			PropertyInfo filterMe = ofClassType.GetProperty("Filter");
			filterMe.SetValue(fs, "UIML files (*.uiml)|*.uiml|All files (*.*)|*.*" , null);
		
	
			//if(ofd.ShowDialog() == DialogResult.OK)
			MethodInfo runner = ofClassType.GetMethod("ShowDialog", new Type[0]);
			Object o = runner.Invoke(fs, null);
			Type tdr = guiAssembly.GetType("System.Windows.Forms.DialogResult");
			//PropertyInfo pInfo =  tdr.GetProperty("OK");
			FieldInfo fInfo = tdr.GetField("OK");
			Object ok = fInfo.GetValue(o);
			
			if(ok.Equals(o))
			{
				PropertyInfo fname = ofClassType.GetProperty("FileName");
				UimlFileName = (string)fname.GetValue(fs, null);
			}

			//fs.Hide();


		}


	}
}
