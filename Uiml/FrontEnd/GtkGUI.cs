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



namespace Uiml.FrontEnd{

	using System;
	using System.Collections;
	using System.Reflection;
	using Uiml.Rendering;
	using Uiml.Executing;
	using Uiml.Executing.Binding;
	using System.Xml;


	/// <summary>
	/// GUI front-end to select an UIML file with Gtk
	/// </summary>
	public class GtkGUI : UimlFrontEnd
	{
		static public string UIMLFILE = @"gtkgui.uiml";
		static public string UIMLLIB=@"uiml.net.dll";
		public const string GTK_ASSEMBLY    = "gtk-sharp";


		public GtkGUI() : base(UIMLFILE, UIMLLIB)
		{
		}

		public static void Main(string[] args)
		{
			new GtkGUI();
		}

		public void AddLibrary(String lib)
		{
		}

		//[UimlEventHandler("ButtonPressed")]
		public override void OpenUimlFile()
		{
			//dynamically load the code to create "FileSelection"
			Assembly guiAssembly = Assembly.Load(GTK_ASSEMBLY);
			if(guiAssembly == null)
				Console.WriteLine("Can not find GTK# Assembly");
			
			//FileSelection fs = new FileSelection ("Choose a file");
			Type ofClassType = guiAssembly.GetType("Gtk.FileSelection");
			Object fs = Activator.CreateInstance(ofClassType, new System.Object[] { "Choose a file" } );			
			Console.WriteLine("Loaded object {0}",fs);
         //fs.Run ();
			MethodInfo runner = ofClassType.GetMethod("Run");
			Console.WriteLine("Retrieved runner {0}", runner);
			runner.Invoke(fs, null);
			//UimlFileName = fs.Filename;
			PropertyInfo fname = ofClassType.GetProperty("Filename");			
			UimlFileName = (string)fname.GetValue(fs, null);

			//fs.Hide();
			MethodInfo hideMe = ofClassType.GetMethod("Hide");
			hideMe.Invoke(fs, null);

			if(UimlFileName == "")
				UimlFileName = null;
		}
	}
}