/*
    Uiml.Net: a .Net UIML renderer (http://research.edm.luc.ac.be/kris/research/uiml.net)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
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
	using Uiml.Rendering;
	using Uiml.Executing;
	using Uiml.Executing.Binding;
	using System.Xml;
	using Gtk;

	/// <summary>
	/// GUI front-end to select an UIML file for the Pocket PC
	/// </summary>
	public class GtkGUI
	{
		static public string UIMLFILE = @"gtkgui.uiml";
		private ArrayList aList = new ArrayList();
		private String uiFileName;
		private UimlDocument uimlDoc;
		private IRenderedInstance instance;
		private IRenderer renderer;

		public GtkGUI()
		{
			//OpenFileDialog ofd = new OpenFileDialog();
			//if(ofd.ShowDialog() == DialogResult.OK)
			//{
				try
				{
					//uimlDoc = new UimlDocument(ofd.FileName);
					//XmlDocument doc = new XmlDocument();
					//doc.Load("\\Program Files\\uimldotnetcf\\compactgui.uiml");
					//XmlTextReader reader = new XmlTextReader("\\Program Files\\uimldotnetcf\\compactgui.uiml");
					//doc.Load(UIMLFILE);
					uimlDoc = new UimlDocument(UIMLFILE);
					//uimlDoc = new UimlDocument(UIMLFILE);
					renderer =  (new BackendFactory()).CreateRenderer(uimlDoc.Vocabulary);
					instance = renderer.Render(uimlDoc);
					ExternalLibraries.Instance.Add(@"uiml.net.dll");
					uimlDoc.Connect(this);
					instance.ShowIt();
				}
				catch(Exception e){ Console.WriteLine(e); }
			//}
		}

		public static void Main(string[] args)
		{
			new GtkGUI();
		}

		public void AddLibrary(String lib)
		{
		}

		//[UimlEventHandler("ButtonPressed")]
		public void OpenUimlFile()
		{
			FileSelection fs = new FileSelection ("Choose a file");
         fs.Run ();
			uiFileName = fs.Filename;
			fs.Hide();
			if(uiFileName == "")
				uiFileName = null;
		}

		public String UimlFileName
		{
			get { return uiFileName; }
		}

		public void Render()
		{
			try
			{
				UimlDocument uimlDoc = new UimlDocument(UimlFileName);
				IRenderer renderer =  (new BackendFactory()).CreateRenderer(uimlDoc.Vocabulary);
				IRenderedInstance instance = renderer.Render(uimlDoc);
				instance.ShowIt();
			}
				catch(Exception e)
				{
				}
		}
	}
}
