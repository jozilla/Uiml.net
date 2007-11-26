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
    using System.Xml.Serialization;
	using System.IO;
	using System.Collections;

	using Uiml.LayoutManagement;

	///<summary>
	///Represents the interface element:
	///   &lt;!ELEMENT interface (structure|style|content|behavior)*&gt;
	///   &lt;!ATTLIST interface id NMTOKEN #IMPLIED 
	///             source CDATA #IMPLIED 
	///             how (union|cascade|replace) "replace" 
	///             export (hidden|optional|required) "optional"&gt;
	///</summary>
    [XmlRoot("interface")]
	public class Interface : UimlAttributes, IUimlElement, ICloneable{

		//private Structure m_structure;
		private ArrayList m_structure = new ArrayList();
		//private Style     m_style;
		private ArrayList m_style = new ArrayList();
		//private Behavior  m_behavior;
		private ArrayList m_behavior = new ArrayList();
		//private Content m_content
		private ArrayList m_content = new ArrayList();
		private ArrayList m_layout = new ArrayList();

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
		
        public virtual object Clone()
		{
			Interface clone  = new Interface();
            clone.m_structure = cloneList(m_structure);
            clone.m_style = cloneList(m_style);
            clone.m_behavior = cloneList(m_behavior);
            for (int i = 0; i < clone.m_behavior.Count; i++)
            {
                Behavior behavior = (Behavior)clone.m_behavior[i];
                behavior.PartTree = ((Structure)clone.UStructure[0]).Top;
            }
            clone.m_content = cloneList(m_content);
            clone.m_layout = cloneList(m_layout);

            return clone;
        }

        private ArrayList cloneList(ArrayList list)
        {
            if(list != null)
            {
                ArrayList cloned = new ArrayList();
                for(int i = 0; i < list.Count; i++)
                {
                    IUimlElement element = (IUimlElement)list[i];
                    cloned.Add(element.Clone());
                }
                return cloned;
            }

            return null;
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
							case LAYOUT:
								Layout l = new Layout(xnl[i], (Structure)UStructure[0]);
								m_layout.Add(l);
								try
								{
									// add layout to part itself
									((Structure)UStructure[0]).Top.SearchPart(l.PartName).AddLayout(l);
								}
								catch (NullReferenceException nre)
								{
								  Console.WriteLine("Specified part [{0}] for layout does not exist", l.PartName);
								}
								break;
						}
					}
				}
			}else{
				//throw Exception
			}

		}

        [XmlIgnore]
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

        [XmlIgnore]
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

        [XmlIgnore]
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

        [XmlIgnore]
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

        [XmlIgnore]
		public ArrayList ULayout
		{
			get
			{
				if (m_layout.Count == 0)
 					return null;
 				else
					return m_layout; 
 			}
 			set { m_layout.Add(value); }
		}

        [XmlIgnore]
		public bool HasLayout
		{
			get { return ULayout != null; }
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

        [XmlIgnore]
		public ArrayList Children
		{
			get 
			{ 
				ArrayList al = new ArrayList();
				al.Add(UStructure);
				al.Add(UStyle);
				al.Add(UBehavior);
				al.Add(UContent);
				al.Add(ULayout);
				return al; 
			} 
		}
		
		public const string STRUCTURE = "structure";
		public const string STYLE     = "style";
		public const string CONTENT   = "content";
		public const string BEHAVIOR  = "behavior";
		//public const string INTERFACE = "interface";
		public const string IAM       = "interface";
		public const string LAYOUT    = "layout";
	}
}
