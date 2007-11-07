/*
	 uiml.net: a Uiml .NET renderer (http://research.edm.uhasselt.be/kris/research/uiml.net)
    
	 Copyright (C) 2005  Kris Luyten (kris.luyten@uhasselt.be)
	                     Expertise Centre for Digital Media (http://www.edm.uhasselt.be)
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


namespace Uiml {

	using System;
	using System.Xml;
	using System.Collections;

	using Uiml.LayoutManagement;
	
	public class UimlDocument : IUimlElement, IUimlComponent, ICloneable {
		private Interface m_interface;
		private ArrayList m_peers;
		private Head m_head;

		private bool m_hasLayout;
		private ConstraintSystem m_solver = null;

		///<summary>
		///Reads a UIML document in memory specified in fName
		///</summary>
		public UimlDocument(String fName)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(fName);
				Load(doc);
			}
			catch(Exception e)
			{					
				Console.WriteLine("Error loading {0} ({1})", fName, e);
			}
		}

		public UimlDocument(XmlNode uimlTopNode)
		{
			Load(uimlTopNode);
		}
		
		///<summary>
		///private parameterless constructor for cloning
		///</summary>
		private UimlDocument()
		{
		}

		private void Load(XmlNode uimlTopNode)
		{
			m_peers = new ArrayList();
			Process(uimlTopNode);
			m_interface.AttachPeers(m_peers);
		}

		public void Process(XmlNode n)
		{
			if(n.Name == Head.IAM)
				m_head = new Head(n);
			if(n.Name == Peer.IAM)
				AddPeer(new Peer(n));
			else if(n.Name == Interface.IAM)
				m_interface = new Interface(n);
			else if(n.Name == Template.IAM)
				TemplateRepository.Instance.Register(new Template(n));
			else if(n.HasChildNodes)
			{
				XmlNodeList xnl = n.ChildNodes;
				for(int i=0; i<xnl.Count; i++)
					Process(xnl[i]);
			}
		}

		public Interface UInterface 
		{
			get { return m_interface; }
			set {	m_interface = value; }
		}

		public Head UHead
		{
			get { return m_head; }
			set { m_head = value; }
		}

		public String Title
		{
			get 
			{
				if(UHead != null)
				{
					return UHead.Title;
				}
				else
					return "Uiml container";
			}
		}

		public IEnumerator Peers
		{
			get { return m_peers.GetEnumerator(); }
		}

        public ArrayList ListOfPeers
        {
            get { return m_peers; }
        }

		public void AddPeer(Peer newPeer)
		{
			m_peers.Add(newPeer);
		}

		///<summary>
		/// Searches for a specific vocabulary to map the classes on
		///</summary>
		public Peer SearchPeers(string pattern)
		{
			IEnumerator enumPeers = Peers;
			while(enumPeers.MoveNext())
			{
				if(((Peer)enumPeers.Current).Provides(pattern))
					return ((Peer)enumPeers.Current);
			}
			throw new VocabularyUnavailableException(pattern);
		}

		///<summary>
		///For now, only a single vocabulary peer will be taken into account
		///</summary>
		public String Vocabulary
		{
			get 
			{
				IEnumerator enumPeers = Peers;
				while(enumPeers.MoveNext())
				{
					return ((Peer)enumPeers.Current).GetVocabulary().Identifier;
				}
				return "";
			}
		}

		public void Connect(Object o)
		{
			//TODO: support multiple structures
			// connect to the top part
			((Structure)UInterface.UStructure[0]).Top.Connect(o, this);
			
			ExternalObjects.Instance.Add(o.GetType().ToString(),o);
		
			// connect to the rules		
			if(UInterface.UBehavior != null)
			{				
				IEnumerator er = ((Behavior)UInterface.UBehavior[0]).Rules;
				while(er.MoveNext())
				{
					((Executing.Rule)er.Current).Connect(o);
				}
			}			
		}

		public void Disconnect(Object o)
		{
			((Structure)UInterface.UStructure[0]).Top.Disconnect(o, this);
			ExternalObjects.Instance.Remove(o);
			if(UInterface.UBehavior != null)
			{				
				IEnumerator er = ((Behavior)UInterface.UBehavior[0]).Rules;
				while(er.MoveNext())
				{
					((Executing.Rule)er.Current).Disconnect(o);
				}
			}			

		}

		public void Connect(Object o, String identifier)
		{
			//TODO: support multiple structures
			Console.WriteLine(UInterface.UStructure);
			Console.WriteLine(UInterface.UStructure[0]);
			Part p = ((Structure)UInterface.UStructure[0]).SearchPart(identifier);
			if (p != null)
				p.Connect(o, this);
		}
	

		/**
		 * Search the part of the interface that matches identifier.
		 **/
		public Part SearchPart(String searchfor)
		{
			//TODO: support multiple structures					
			Structure t = (Structure)UInterface.UStructure[0];			
			Part p = (Part)t.SearchPart(searchfor);
			return p;
			//return ((Structure)UInterface.UStructure[0]).SearchPart(searchfor);
		}

		public void SolveLayoutProperties(Uiml.Rendering.IRenderer r)
		{
			try 
			{
				LayoutPropertyRepository.Instance.InitializeProperties(this, r);
				ArrayList layouts = new ArrayList();

				// process layouts
				Part top = ((Structure) UInterface.UStructure[0]).Top;

				top.GetLayouts(ref layouts);

				foreach (Layout l in layouts)
					l.SolveLayoutProperties();

				m_hasLayout = layouts.Count > 0;

				if (HasLayout)
				{
					// create constraint system
					m_solver = new ConstraintSystem(layouts);
					m_solver.Solve();				
				}
			}
			catch(IndexOutOfRangeException ioore)
			{
				Console.WriteLine("No layout specified, continuing anyway....");
			}
		}

		public void ResolveLayoutProperties(Uiml.Rendering.IRenderer r)
		{
			if (HasSolver)
			{
				Solver.Resolve();
			}
		}
		
		public ArrayList Children
		{
			get 
			{ 
				ArrayList al = new ArrayList();
				al.Add(UHead);
				al.Add(UInterface);
				al.Add(m_peers);
				return al; 
			}
		}

		public string PartTree() 
		{
			return ((Structure) UInterface.UStructure[0]).PartTree();
		}
		
		//ICloneable methods:
		public object Clone()
		{
			UimlDocument clone = new UimlDocument();
            if(m_peers != null)
            {
                clone.m_peers = new ArrayList();
                for(int i = 0; i < m_peers.Count; i++)
                {
                    clone.AddPeer((Peer)((Peer)m_peers[i]).Clone());
                }
            }
            if(m_interface != null)
                clone.m_interface = (Interface)m_interface.Clone();
            if(m_head != null)
                clone.m_head = (Head)m_head.Clone();
            clone.m_hasLayout = m_hasLayout;
            clone.m_solver = m_solver;
			return clone;
		}
		
		// IUimlComponent methods:
		//public ArrayList Children { get ; } //This method already exist to return a "complete" UIML document in an ArrayList
		public void Add(string pattern, IUimlComponent component) { /* do nothing */ }
		public void Remove(IUimlComponent component) { /* do nothing */ }		
		public UimlComposite Composite 
		{ 
			get { return null; } 
		}		
		public Hashtable CompChildren
		{
			get {return null; }
		} 
		
		public ConstraintSystem Solver
		{
			get { return m_solver; }
		}

		public bool HasSolver
		{
			get { return Solver != null; }
		}

		public bool HasLayout
		{
			get { return m_hasLayout; }
		}

		public const string PEER      = "peers";
		public const string UIML      = "uiml";
		public const string INTERFACE = "interface";
		public const string STRUCTURE = "structure";
		public const string STYLE     = "style";
		public const string PROPERTY  = "property";
		public const string CONSTANT  = "constant";
	}

}
