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
	using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    using Uiml.Utils;
	using Uiml.Rendering;
	using Uiml.Executing;

	
	///<summary>
	/// Main application class; serves as a comand-line front-end for
	/// the uiml.net library
	///</summary>
	public abstract class UimlFrontEnd
	{
		private String uiFileName;
        protected UimlDocument uimlDoc;

        private string m_frontendFile;
        private string m_frontendLib;
		
        private ArrayList aList = new ArrayList();
        private Dictionary<string, IRenderedInstance> renderedUIs = new Dictionary<string, IRenderedInstance>();

		protected static IRenderer renderer;

        public UimlFrontEnd()
        {
        }

		public UimlFrontEnd(string frontendFile, string frontendLib) : this()
		{
            SetupFrontEndFiles(frontendFile, frontendLib);
            LoadFrontEnd();
        }

        private void SetupFrontEndFiles(string frontendFile, string frontendLib)
        {
            // always load the front-end files from the front-ends/ 
            // subdirectory.
            m_frontendFile = Path.Combine(Uiml.Utils.Location.FrontEndDirectory, frontendFile);
            m_frontendLib = frontendLib;
        }

        private void LoadFrontEnd()
        {
			try //try to load a GUI as front-end
			{
                UimlDocument feUimlDoc = new UimlDocument(m_frontendFile);
				renderer =  (new BackendFactory()).CreateRenderer(feUimlDoc.Vocabulary);
                IRenderedInstance instance = renderer.Render(feUimlDoc);
                ExternalLibraries.Instance.Add(m_frontendLib);
				feUimlDoc.Connect(this);
                // add to the list
                renderedUIs.Add(m_frontendFile, instance);
                instance.ShowIt();
			}
			catch(Exception e)
			{
				//no interface available, use command-line options!
				throw new NoGuiAvailableException(e);
			}
		}


	   	public UimlDocument UimlDoc
		{
			get { return uimlDoc; }
			set { uimlDoc = value;}
		}

		
		static public void Version()
		{
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine("uiml.net version {0}", v);
		}

		public static IRenderer CurrentRenderer
		{
			get { return renderer; }
		}

        public abstract Assembly GuiAssembly
        {
            // for dynamically loading the code for 
            // System.Windows.Forms specific stuff 
            // (e.g. OpenFileDialog).
            //
            // This assembly will be loaded already.
            get;
        }

		public abstract void OpenUimlFile();
        public abstract void Quit();

		public String UimlFileName
		{
			get { return uiFileName; }
			set { uiFileName = value; }
		}

        public String FriendlyUimlFileName
        {
            get { return Path.GetFileName(uiFileName); }
        }

        public void Render()
        {
            Render(UimlFileName);
        }

        /// <summary>
        /// Displays a new UIML dialog. Will render it if necessary, otherwise just try to bring it to the front.
        /// </summary>
        /// <param name="file">The UIML file to display (or render)</param>
        /// <param name="replace">Indicates whether Uiml.net should hide the current window when displaying the new one.</param>
        public void Show(string file, bool replace)
        {
            // transform file to key format
            string key = FileToDictionaryKey(file);

            // make sure that existing files just get focused again, instead of re-rendering them!
            if (renderedUIs.ContainsKey(key))
            {
                renderedUIs[key].ShowIt();
            }
            else
            {
                Render(file);
            }
        }

        private void Render(string file)
        {
			try
			{
                UimlDoc = new UimlDocument(file);	
				Console.WriteLine("render [" + uimlDoc.Vocabulary + "]-[" + file + "]");
				IRenderer renderer =  (new BackendFactory()).CreateRenderer(uimlDoc.Vocabulary);	
				IRenderedInstance instance = renderer.Render(uimlDoc);
                // connect the frontend to the document, to allow communication at runtime
                UimlDoc.Connect(this);
                // connect the rendered instance to the document
                UimlDoc.Instance = instance;
                // add to the list
                renderedUIs.Add(FileToDictionaryKey(file), instance);
                // connect close event
                instance.CloseWindow += new EventHandler(UimlDocument_Closed);
                instance.ShowIt();
			}
			catch (NoRendererAvailableException rue)
			{
				Console.WriteLine("No suitable renderer found:");
				Console.WriteLine("  Please check if the rendering backend corresponding to the vocabulary is available");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
#if !COMPACT
				Console.WriteLine(e.StackTrace);				
#endif
				Console.WriteLine("Could not create GUI for {0} with uiml.net", UimlFileName);
			}
		}

        private string FileToDictionaryKey(string file)
        {
            string dictionaryKey = Path.GetFileName(file);
            dictionaryKey.Replace("uiml://", string.Empty);

            return dictionaryKey;
        }

        void UimlDocument_Closed(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, IRenderedInstance> item in renderedUIs)
            {
                if (item.Value == sender)
                {
                    renderedUIs.Remove(item.Key);
                    return;
                }
            }
        }
	}
}

