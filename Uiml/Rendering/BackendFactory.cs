/*
 	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)

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

namespace Uiml.Rendering
{
	using System;
	using System.Reflection;
	using System.Collections;

	public class BackendFactory
	{
		public String[] assemblies = { 
                                              "uiml-gtk-sharp",
                                              "uiml-wx-net",
                                              "uiml-swf",
                                              "uiml-compact-swf" 
                                             };

		public String[] renderers = { 
                                             "Uiml.Rendering.GTKsharp.GtkRenderer",
                                             "Uiml.Rendering.WXnet.WxRenderer",
                                             "Uiml.Rendering.SWF.SWFRenderer",
                                             "Uiml.Rendering.CompactSWF.CompactSWFRenderer"
                                            };

		public const string NAME = "NAME";

		public BackendFactory()
		{
		}

		///<summary>
		/// Creates a renderer for a given vocabulary name
		///</summary>
		public IRenderer CreateRenderer(String name)
		{

			/* old, static code to load backend renderers
			try
			{
				switch(name)
				{
					
					//Uncomment this if you want to use GTK# as a backend renderer
					case Uiml.Rendering.GTKsharp.GtkRenderer.NAME:
						return new Uiml.Rendering.GTKsharp.GtkRenderer();
					
					//Uncomment this if you want to use wx.NET as a backend renderer
					case Uiml.Rendering.WXnet.WxRenderer.NAME:
						return new Uiml.Rendering.WXnet.WxRenderer();
				
					//Uncomment this if you want to use System.Windows.Forms as a backend renderer
					case Uiml.Rendering.SWF.SWFRenderer.NAME:
						return new Uiml.Rendering.SWF.SWFRenderer();

					//Uncomment this if you want to use System.Windows.Forms on 
					//the .NET Compact framework as a backend renderer
					case Uiml.Rendering.CompactSWF.CompactSWFRenderer.NAME:
						return new Uiml.Rendering.CompactSWF.CompactSWFRenderer();						

					default:
						return renderer;
				}

			}
			catch(Exception e)
			{
				Console.WriteLine("Could not find required libraries for the {0} widget set", name);
				Console.WriteLine("Please make sure the required libraries to use {0} are installed on your platform,", name);
				Console.WriteLine("Or choose another rendering backend.");
				return null;
			}
			*/

			//new code; try to load backend renderer dynamically:
			
			//IRenderer renderer = null;
			Console.WriteLine("Looking for {0} renderer", name);
			for (int i=0; i< renderers.Length; i++)
			{
				try
				{
					Assembly a = Assembly.LoadWithPartialName(assemblies[i]);
					Type t = a.GetType(renderers[i]);
					FieldInfo m = t.GetField(NAME);
					String dynname = (String)m.GetValue(t);
					Console.Write("Renderer for {0} vocabulary", dynname);
					if(dynname == name)
					{
						Console.WriteLine("...match. OK! Loading renderer type {0}.", t);
						return (IRenderer)Activator.CreateInstance(t);
					}
					else
						Console.WriteLine("...no match with {0}", name);
				}
				catch(Exception e)
				{
					// do nothing here: an exception means the backend renderer specified 
					// in assemblies[i] is not available
					// Console.WriteLine(e);
				}
			}

			// if we ever get here, this means no suitable renderer was found!
			throw new NoRendererAvailableException();
		}

	}
}

