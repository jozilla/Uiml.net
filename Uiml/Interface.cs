/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.uhasselt.be/kris/research/uiml.net)

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




namespace Uiml{

	using System;
	using System.Xml;
	using System.IO;
	using System.Collections;

	///<summary>
	///Represents the interface element:
	///   &lt;!ELEMENT interface (structure|style|content|behavior)*&gt;
	///   &lt;!ATTLIST interface id NMTOKEN #IMPLIED 
	///             source CDATA #IMPLIED 
	///             how (union|cascade|replace) "replace" 
	///             export (hidden|optional|required) "optional"&gt;
	///</summary>
	public class Interface : UimlAttributes, IUimlElement, ICloneable{

		//private Structure m_structure;
		private ArrayList m_structure = new ArrayList();
		//private Style     m_style;
		private ArrayList m_style = new ArrayList();
		//private Behavior  m_behavior;
		private ArrayList m_behavior = new ArrayList();
		//private Content m_cont;ent
		private ArrayList m_content = new ArrayList();

		public Interface(XmlNode nTopStructure)
		{
			Process(nTopStructure);
		}
		
		///<summary>
		///private constructor for clone operation
		///</summary>
		private Interface()
		{
		}

		public void Process(XmlNode n)
		{
			if(n.Name == IAM){
				ReadAttributes(n);
				if(n.HasChildNodes){
					XmlNodeList xnl = n.ChildNodes;
					int i = 0;
					for(i=0; i<xnl.Count; i++){
						switch(xnl[i].Name){
							case STRUCTURE:
							   //UStructure = new Structure(xnl[i]);
								m_structure.Add(new Structure(xnl[i]));
								break;
							case STYLE:
								//UStyle = new Style(xnl[i]);
								m_style.Add(new Style(xnl[i]));
								break;
							case CONTENT:
								m_content.Add(new Content(xnl[i]));
								break;
							case BEHAVIOR:
								//UBehavior = new Behavior(xnl[i], UStructure.Top);
								m_behavior.Add(new Behavior(xnl[i], ((Structure)UStructure[0]).Top));
								break;
						}
					}
				}
			}else{
				//throw Exception
			}

		}

		public ArrayList UStructure
		{
			get { 
				if(m_structure.Count == 0)
					return null;
				else
					return m_structure;  
			}
			set { m_structure.Add(value); }
		}

		public ArrayList UStyle
		{
			get {
				if(m_style.Count == 0)
					return null;
				else
					return m_style;  
			}
			set { m_style.Add(value); }
		}

		public ArrayList UBehavior
		{
			get { 
				if(m_behavior.Count == 0)
					return null;
				else
					return m_behavior;  
			}
			set { m_behavior.Add(value); }
		}

		public ArrayList UContent
		{
			get {
				if(m_content.Count == 0)
					return null;
				else
				 return m_content; 
			}
			set { m_content.Add(value); }
		}
		


		public void AttachPeers(ArrayList peers)
		{
			//isolate the Logic subelements of the "peers"	
			ArrayList logics = new ArrayList();
			IEnumerator enumPeers = peers.GetEnumerator();
			while(enumPeers.MoveNext())
			{
				IEnumerator enumLogic = ((Peer)enumPeers.Current).Logic.GetEnumerator();
				while(enumLogic.MoveNext())
					logics.Add((Logic)enumLogic.Current);
			}
			if(m_behavior != null)
				foreach(Behavior b in m_behavior)
					b.AttachPeers(logics);
		}

		public ArrayList Children
		{
			get 
			{ 
				ArrayList al = new ArrayList();
				al.Add(UStructure);
				al.Add(UStyle);
				al.Add(UBehavior);
				al.Add(UContent);
				return al; 
			} 
		}
		
		//IClonaeble implementation:
		public object Clone()
		{
			Interface iamclone  = new Interface();
			if(UBehavior != null)
				iamclone.UBehavior  = (ArrayList)UBehavior.Clone();
			if(UContent != null)
				iamclone.UContent   = (ArrayList)UContent.Clone();
			if(UStructure != null)
				iamclone.UStructure = (ArrayList)UStructure.Clone();
			if(UStyle != null)
				iamclone.UStyle     = (ArrayList)UStyle.Clone();
			return iamclone;
		}


		public const string STRUCTURE = "structure";
		public const string STYLE     = "style";
		public const string CONTENT   = "content";
		public const string BEHAVIOR  = "behavior";
		//public const string INTERFACE = "interface";
		public const string IAM = "interface";
	
	}
}
