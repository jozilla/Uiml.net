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

	using Uiml.Peers;

	using System;
	using System.Xml;
	using System.IO;

	using CommandLine;

	using Uiml.Rendering;

	using Uiml.Executing;

	
	///<summary>
	/// Main application class; serves as a comand-line front-end for
	/// the uiml.net library
	///</summary>
	public class UimlTool : UimlFrontEnd
	{
		static public String[] options = {"voc","uiml","help","libs","version", "log"};
		static public char LIBSEP;

		public static String FileName;
//		public static log4net.ILog logger;
		
		
		public static void Main(string[] args)
		{
			UimlFrontEnd uef = null;
			Options opt = new Options(args, options);
			if(opt.NrSwitches == 0)
			{
				//if there were no switches/arguments, try to use a GUI front-end
				//check whether this executable is working on Compact .Net
				#if COMPACT
					uef = new CompactGUI();
				#else
					//try the Gtk# GUI first and then the Windows.Form GUI
					try{
						uef= new GtkGUI(); 
					}
						catch(Exception excep)
						{
							//the compact SWF GUI also works on with normal SWF
							uef = new SwfGUI(); 
						}
				#endif
			}
			else
			{
				CommandLine(opt);
			}		
			Console.WriteLine("the end");
			
		}

		static public void CommandLine(Options opt)
		{
			if(opt[options[2]].Equals("-"))
			{
				Help();
				return;
			}				
			string document="", vocabulary="";

			if(opt.IsUsed(options[5])) //initialise logging facilities
			{
				if(!opt.HasArgument(options[5]))
				{
					Console.WriteLine("You have to specify an appender for logging");
					Console.WriteLine("Available appenders: log4net.Appender.ConsoleAppender log4net.Appender.CountingAppender");
				}
				else
				{
					//logger =  log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
					//SimpleLayout sl = new SimpleLayout();
					//IAppender appender =  
					//TODO
				}
				
			}
			else
			{
				//TODO
			}
		   
			if(opt.IsUsed(options[1])&&opt.HasArgument(options[1]))
			{
				if(opt.IsUsed(options[3]))
				{
					LoadLibraries(opt[options[3]]);
				}
				UimlTool ut;
				document = opt[options[1]];
				if(opt.IsUsed(options[0]))
				{
					vocabulary = opt[options[0]];
					ut = new UimlTool(document,vocabulary);
				}
				else
					ut = new UimlTool(document);					
			}
			else if(opt.IsUsed(options[4]))
			{
				Version();
			}
			else
			{
				Console.WriteLine("You have to specify a uiml document as input");
				Help();
			}
		}

		static public void Help()
		{
			Console.WriteLine("                                                                              	  ");
			Console.WriteLine("         Uiml.Net: a free Uiml 3.0 renderer for .Net                           	  ");
			Console.WriteLine("                                                                             	  ");
			Console.WriteLine("Copyright: Expertise Centre for Digital Media -- Limburgs Universitair Centrum	  ");
			Console.WriteLine("contact: kris.luyten@luc.ac.be                                                	  ");
			Console.WriteLine("web: http://research.edm.luc.ac.be/kris/projects/uiml.net                         ");
			Console.WriteLine("                                                                          	     ");
			Console.WriteLine("Please email the bugs you find using this tool to us                     	        ");
			Console.WriteLine("                                                                         	        ");
			Console.WriteLine("Options:                                                                     	  ");
			Console.WriteLine("    -uiml <file>                 Specify the input file (required)                ");
			Console.WriteLine("    -help                        Print this message                               ");
			Console.WriteLine("    -libs <file["+LIBSEP+"file"+LIBSEP+"...]>        The libraries containing te application logic        ");	
			Console.WriteLine("    -log <appender>                                                               ");
			Console.WriteLine("    -version                     Print version info                               ");	
		}

		public override void OpenUimlFile()
		{
			//is useless here...
		}



		static public void LoadLibraries(String libs)
		{
			ExternalLibraries eLib = ExternalLibraries.Instance;
			int j = libs.IndexOf(LIBSEP);
			while(j!=-1)
			{
				String nextLibrary = libs.Substring(0,j);
				eLib.Add(nextLibrary);
				libs = libs.Substring(j+1,libs.Length-j-1);
				j = libs.IndexOf(LIBSEP);
			}
			eLib.Add(libs);
		}


		static public Uri InputFile = null;

		public UimlTool(string fName)
		{
			UimlFileName = fName;
			Render();
		}

		public UimlTool(string fName, string voc)
		{
			Console.WriteLine("Separate or external vocabulary specification not supported yet.");
			Console.WriteLine("Change the peer reference in the UIML document to change the vocabulary.");
		}
/*
		public void Load(String fName)
		{
			Load(fName,null);
		}

	
		public void Load(String fName, String strVoc)
		{
			FileName = fName;
			//get the current directory:
			//InputFile = new Uri("file://" + Application.ExecutablePath() + "/" + FileName);

			if(strVoc!=null)
				new Vocabulary(strVoc);
			try
			{
				XmlDocument doc = new XmlDocument();
				Console.WriteLine("Loading UIML document...");
				doc.Load(fName);
				//doc.Load("copy.uiml");
				Console.WriteLine("Processing UIML document...");
				Process(doc);
				Console.WriteLine("Rendering UIML document...");
				Render();
			}
			catch(System.IO.FileNotFoundException sif)
			{
				Console.WriteLine("Could not read file {0}",FileName);
				#if !COMPACT
				Environment.Exit(-1);
				#else
				//TODO
				;
				#endif
			}
		}
		*/



		private void Process(XmlNode n)
		{
			UimlDoc = new UimlDocument(n);
		}

		/*
		private void Render()
		{
			Console.WriteLine(Document.UHead);
			//m_renderer = new GtkRenderer();
			//IRenderedInstance instance = m_renderer.Render(Document);
			//instance.ShowIt();

			//Check the vocabulary, and whether there is a registerd Renderer for this vocabulary			
			m_renderer =  backendFactory.CreateRenderer(Document.Vocabulary);
			Console.WriteLine("Loading renderer for {0}", Document.Vocabulary);
			if(m_renderer == null)
			{
				Console.WriteLine("No Vocabulary specified, please update the <peers> section of {0}", FileName);
				#if !COMPACT
				Environment.Exit(-1);
				#else
				//TODO
				;
				#endif
			}
			Console.WriteLine("Building ui instance");
			IRenderedInstance instance = m_renderer.Render(Document);
			Console.WriteLine("Showing the ui");
			instance.ShowIt();
		}
		*/


		public const string UIML = "uiml";

	}

}
