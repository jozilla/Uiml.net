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

	using Uiml.Peers;

	using System;
	using System.Xml;
	using System.IO;
	using System.Collections;

	using Uiml.Rendering;
	using Uiml.Executing;

	
	///<summary>
	/// Main application class; serves as a comand-line front-end for
	/// the uiml.net library
	///</summary>
	public abstract class UimlFrontEnd
	{
		static public String VERSION = "0.0.6-pre (09-07-2004)";
		private String uiFileName;
		private ArrayList aList = new ArrayList();
		protected IRenderedInstance instance;
		protected static IRenderer renderer;
		protected UimlDocument uimlDoc;
		private BackendFactory backendFactory;


		public UimlFrontEnd()
		{
			backendFactory = new BackendFactory();
		}

		public UimlFrontEnd(string uimlDoc, string uimlLib) : this()
		{
			
			try
			{
				UimlDocument feUimlDoc = new UimlDocument(uimlDoc);	
				renderer =  (new BackendFactory()).CreateRenderer(feUimlDoc.Vocabulary);
				instance = renderer.Render(feUimlDoc);
				ExternalLibraries.Instance.Add(uimlLib);
				feUimlDoc.Connect(this);
				instance.ShowIt();
			}
				catch(Exception e)
				{
					//no interace available, use command-line options!
					throw new NoGuiAvailableException();
				}
		}


	   public UimlDocument UimlDoc
		{
			get { return uimlDoc; }
			set { uimlDoc = value;}
		}

		
		static public void Version()
		{
			Console.WriteLine("uiml.net version {0}", VERSION);
		}

		public static IRenderer CurrentRenderer
		{
			get { return renderer; }
		}

		public abstract void OpenUimlFile();

		public String UimlFileName
		{
			get { return uiFileName; }
			set { uiFileName = value; }
		}

		public void Render()
		{
			try
			{
				UimlDoc  = new UimlDocument(UimlFileName);		
				IRenderer renderer =  backendFactory.CreateRenderer(uimlDoc.Vocabulary);			
				IRenderedInstance instance = renderer.Render(uimlDoc);
				instance.ShowIt();			

			}
				catch(Exception e)
				{
					Console.WriteLine(e);
					Console.WriteLine("Could not create GUI for {0} with uiml.net", UimlFileName);
				}
		}
	}
}

