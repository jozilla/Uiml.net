/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)

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

namespace Uiml
{
	using Uiml.Peers;

	using System;
	using System.Xml;
	using System.Collections;


	public class Peer : UimlAttributes, IUimlElement
	{

		private ArrayList m_presentations;
		private ArrayList m_logic;

		private Presentation m_selected;

		public Peer(XmlNode nTopPeer)
		{
			m_presentations = new ArrayList();
			m_logic = new ArrayList();
			Process(nTopPeer);
		}

		public void AddPeer(Presentation presvoc)
		{
			m_presentations.Add(presvoc);
		}

		public void AddLogic(Logic l)
		{
			m_logic.Add(l);
		}


		public void Process(XmlNode n)
		{
			if(n.HasChildNodes){
				XmlNodeList xnl = n.ChildNodes;
				for(int i=0; i<xnl.Count; i++)
				{
					if(xnl[i].Name == PRESENTATION)
						AddPeer(new Presentation(xnl[i]));
					else if(xnl[i].Name == LOGIC)
					{
						Logic l = new Logic(xnl[i]); 
						AddLogic(l);
						// add logic to the vocabulary
						GetVocabulary().MergeLogic(l);
					}
				}
			}
		}

		///<summary>
		/// Checks whether this peer has a presentation vocabulary for pattern
		///</summary>
		public bool Provides(string pattern)
		{
			IEnumerator enumPres = m_presentations.GetEnumerator();
			while(enumPres.MoveNext())
			{
				if(((Presentation)enumPres.Current).Provides(pattern))
					return true;
			}
			return false;
		}

		///<summary>
		///Returns the Vocabulary that matches pattern
		///</summary>
		public Vocabulary GetVocabulary(string pattern)
		{
			IEnumerator enumPres = m_presentations.GetEnumerator();
			while(enumPres.MoveNext())
				if(((Presentation)enumPres.Current).Provides(pattern))
					return ((Presentation)enumPres.Current).UimlVocabulary;
			throw new VocabularyUnavailableException(pattern);
		}

		///<summary>
		///Returns the first Vocabulary that fits this Peer
		///</summary>
		public Vocabulary GetVocabulary()
		{
			IEnumerator enumPres = m_presentations.GetEnumerator();
			if(enumPres.MoveNext())
				return ((Presentation)enumPres.Current).UimlVocabulary;
			else
				throw new VocabularyUnavailableException("There is no vocabulary loaded");
		}

		///<summary>
		///If there are multiple presentations, one vocabulary must be selected as the ``current'' one.
		///</summary>
		///<remarks>
		///Precondition: Presentation p must be a member of this peer
		///</remarks>
		public void Select(Presentation p)
		{
			//TODO
		}

		///<summary>
		///If there are multiple presentations, one vocabulary must be selected as the ``current'' one.
		///</summary>
		///<remarks>
		///Precondition: the member must match with a presentation member of this peer
		///</remarks>
		public void Select(string pattern)
		{
			//TODO
		}

		public ArrayList Children
		{
			get
			{ 
				ArrayList al = new ArrayList();
				al.AddRange(m_presentations);
				al.AddRange(m_logic);
				return al; 
			}
		}

		public ArrayList Logic
		{
			get { return m_logic; }
		}



		public const string PRESENTATION = "presentation";
		public const string LOGIC        = "logic";
		public const string IAM          = "peers";
	}
}
