/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.luc.ac.be/kris/research/uiml.net)

	 Copyright (C) 2003  Kris Luyten (kris.luyten@luc.ac.be)
	                     Expertise Centre for Digital Media (http://edm.luc.ac.be)
								Limburgs Universitair Centrum

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU General Public License
	as published by the Free Software Foundation; either version 2
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/


namespace Uiml{

	using System;
	using System.Xml;
	using System.Collections;

	using Uiml.Executing;

	///<summary>
	///Represents the behavior element:
	/// &lt;!ELEMENT behavior (rule*)&gt; 
	/// &lt;!ATTLIST behavior id NMTOKEN #IMPLIED 
	///              source CDATA #IMPLIED 
	///              how (union|cascade|replace) "replace" 
	///              export (hidden|optional|required) "optional"&gt;
	///</summary>
	public class Behavior : UimlAttributes, IUimlElement{
		private ArrayList m_rules;
		private Part      m_partTree;

		public Behavior()
		{
			m_rules = new ArrayList();
		}

		public Behavior(XmlNode n, Part topPart) : this()
		{
			m_partTree = topPart;
			Process(n);
		}

		public void Process(XmlNode n)
		{
			ReadAttributes(n);
			
			if(n.Name == BEHAVIOR)
			{
				if(n.HasChildNodes)
				{
					XmlNodeList xnl = n.ChildNodes;
					for(int i=0; i<xnl.Count; i++)
						m_rules.Add(new Rule(xnl[i], m_partTree));
				}

			}
		}

		public void AttachPeers(ArrayList logics)
		{
			
			IEnumerator enumRules = Rules;
			while(enumRules.MoveNext())
			{
				AttachPeers((Rule)enumRules.Current, logics);
			}
		}

		private void AttachPeers(IUimlElement iue, ArrayList logics)
		{
			if(iue is Uiml.Executing.Rule)
			{									
				((Rule)iue).Action.AttachLogic(logics);
			}

			if(iue.Children != null)
			{
				IEnumerator enumChildren = iue.Children.GetEnumerator();
				while(enumChildren.MoveNext())
				{
					AttachPeers((IUimlElement)enumChildren.Current, logics);
				}				
			}
		}

		public IEnumerator Rules
		{
			get {	return m_rules.GetEnumerator(); }
		}

		public ArrayList Children
		{
			get { return m_rules; }
		}

		public const string BEHAVIOR = "behavior";
	}
}

