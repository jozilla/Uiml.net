/*
	 uiml.net: a Uiml .NET renderer (http://research.edm.luc.ac.be/kris/research/uiml.net)
    
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


namespace Uiml{

	using System;
	using System.Xml;
	using System.Collections;

	public class UimlDocument : IUimlElement {
		private Interface m_interface;
		private ArrayList m_peers;
		private Head m_head;

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

		public UimlDocument(XmlNode uimlTopNode){
			Load(uimlTopNode);
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
			set {	m_interface = value;	}
		}

		public Head UHead
		{
			get { return m_head;}
			set { m_head = value; }
		}

		public IEnumerator Peers
		{
			get { return m_peers.GetEnumerator(); }
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

		public void Connect(Object o, String identifier)
		{
			//TODO: support multiple structures
			Part p = ((Structure)UInterface.UStructure[0]).SearchPart(identifier);
			if(p!=null)
				p.Connect(o, this);
		}

		/**
		 * Search the part of the interface that matches identifier.
		 **/
		public Part SearchPart(String identifier)
		{
			//TODO: support multiple structures
			return ((Structure)UInterface.UStructure[0]).SearchPart(identifier);
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

		public const string PEER      = "peers";
		public const string UIML      = "uiml";
		public const string INTERFACE = "interface";
		public const string STRUCTURE = "structure";
		public const string STYLE     = "style";
		public const string PROPERTY  = "property";
		public const string CONSTANT  = "constant";
	}

}
