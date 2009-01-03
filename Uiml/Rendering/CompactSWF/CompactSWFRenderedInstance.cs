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

	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/


namespace Uiml.Rendering.CompactSWF
{
    using System;
	using System.Windows.Forms;

	///<summary>
	/// This class serves as a container for rendering a SWF User Interface.
	/// The SWFRenderer will return an instance of this class. Use
	/// ShowIt to show the interface on screen.
	///</summary>
	public class CompactSWFRenderedInstance : Form, IRenderedInstance
	{
		public CompactSWFRenderedInstance()
		{
            this.Menu = new System.Windows.Forms.MainMenu();
            // window closed event
            Closed += new EventHandler(OnCloseWindow);
            // init event
            Load += new EventHandler(OnInit);
            // activated event
            Activated += new EventHandler(OnActivateWindow);
		}

		public CompactSWFRenderedInstance(string title)
		{		
			Text = title;
		}
	
		public void ShowIt()
		{
			try
			{
                this.Activate();
                this.Show();
                Application.Run(this);
			}
            catch (InvalidOperationException e)
            {
                //Application thread is already running...
                if (!this.Visible)
                {
                    this.Show();
                }
                else
                {
                    this.Activate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load SWF form: {0}", e);
                Console.WriteLine(e.StackTrace);
            }
		}

        public void CloseIt()
        {
            this.Close();
        }
		
		public void Add(Control c) 
		{
			this.Controls.Add(c); 			
		}

		public string Title
		{
			get
			{
				return Text;
			}
			set
			{
				Text = value;
			}
		}

        #region CloseWindow event
        public event EventHandler CloseWindow;
        public void OnCloseWindow(object sender, EventArgs e)
        {
            if (CloseWindow != null)
                CloseWindow(this, e);
        }
        #endregion

        #region Init event
        public event EventHandler Init;
        public void OnInit(object sender, EventArgs e)
        {
            if (Init != null)
                Init(this, e);
        }
        #endregion

        #region ActivateWindow event
        public event EventHandler ActivateWindow;
        public void OnActivateWindow(object sender, EventArgs e)
        {
            if (ActivateWindow != null)
                ActivateWindow(this, e);
        }
        #endregion
	}	
}
