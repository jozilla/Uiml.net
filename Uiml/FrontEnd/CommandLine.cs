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
	
	using Uiml.Rendering;

	using Uiml.Executing;

	
	///<summary>
	/// Serves as a comand-line front-end for the uiml.net library.
	///</summary>
	public class CommandLine : UimlFrontEnd
	{
		static public String[] options = {"voc","uiml","help","libs","version", "log"};
		static public char LIBSEP;

		public static String FileName;
		
	
		public CommandLine(Options opt) : base()
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
				document = opt[options[1]];
				UimlFileName = document;

				if(opt.IsUsed(options[0]))
				{
					vocabulary = opt[options[0]];
					Render();
				}
				else
					Render();
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
	

		static public Uri InputFile = null;



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

	   public override void OpenUimlFile()
		{
			//useless for command line
		}


		/*
		public UimlTool(string fName, string libs) : base(fName, libs) 
		{
//			Console.WriteLine("Separate or external vocabulary specification not supported yet.");
//			Console.WriteLine("Change the peer reference in the UIML document to change the vocabulary.");
		}
		
		
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

		public static void Main(string[] args)
		{
			new CommandLine(new Options(args, options));
		}
		
	}

}
