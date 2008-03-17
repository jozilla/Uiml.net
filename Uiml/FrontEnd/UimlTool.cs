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

	using Uiml.Peers;

	using System;
	using System.Xml;
	using System.IO;
	
	using Uiml.Rendering;

	using Uiml.Executing;

	
	///<summary>
	/// Main application class; serves as a comand-line front-end for
	/// the uiml.net library. A graphical frontend for uiml.net will be automatically
	/// chosen.
	///</summary>
	public class UimlTool
	{
		public static String FileName;
		
		[STAThread]
		public static void Main(string[] args)
		{
			UimlFrontEnd uef = null;
			Options opt = new Options(args, CommandLine.options);			
			//Console.WriteLine(opt);
			if(opt.NrSwitches == 0)
			{				
				//if there were no switches/arguments, try to use a GUI front-end
				//check whether this executable is working on Compact .Net
				#if COMPACT
					uef = new CompactGUI();
				#else
					//try the Gtk# GUI first and then the Windows.Forms GUI
					try{ uef= new GtkGUI(); }
						catch(Exception excep)
						{ //the compact SWF GUI also works on with normal SWF
							try{ uef = new SwfGUI(); }
							   catch(Exception excep2)
								{
									//no GUI availble, try commandline
									uef = new CommandLine(opt);
								}
						}
				#endif
			}
			else
			{
				uef = new CommandLine(opt);
			}
            Console.In.ReadLine();
		}
	}
}
