/*
  	 Uiml.Net: a Uiml.Net renderer (http://lumumba.uhasselt.be/kris/research/uiml.net/)
   
	 Copyright (C) 2003  Kris Luyten (kris.luyten@uhasselt.be)
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


namespace Uiml.Rendering.WXnet
{

	using System;
	using System.Drawing;
	using wx;

	///<summary>
	/// This class serves as a container for rendering a Wx.NET User Interface.
	/// The WxRenderer will return an instance of this class. Use
	/// ShowIt to show the interface on screen.
	///</summary>
	public class WxRenderedInstance : App, IRenderedInstance{
		private Frame m_topFrame;
		private String m_title;


		public WxRenderedInstance(String title)
		{
			Console.WriteLine("In constructor");
			m_title = title;
		}

		public Frame TopFrame
		{
			get {	
				Console.WriteLine("In TopFrame getter, before test: {0}", m_topFrame);
				if(m_topFrame==null)
					m_topFrame = new ContainerFrame(m_title);
				Console.WriteLine("In TopFrame getter, after test: {0}", m_topFrame);
				return m_topFrame; 
			}
		}
	

		public override bool OnInit()
		{
			Console.WriteLine("In OnInit, before show: {0}", m_topFrame);
			TopFrame.Show(true);
			Console.WriteLine("In OnInit, after show: {0}", m_topFrame);
			return true;
		}

		[STAThread]
		public void ShowIt()
		{
			Console.WriteLine("let's show");
			Run();
		}




	}

	class ContainerFrame : Frame
	{
		public ContainerFrame(string title) :	base(null, -1, title, wxDefaultPosition, wxDefaultSize, wxDEFAULT_FRAME_STYLE )
		{
			Console.WriteLine("In ContainerFrame constructor");
		}
	}
	
}
