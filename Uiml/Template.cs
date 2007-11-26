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


namespace Uiml{
	
	using System;
	using System.Xml;
	using System.Collections;

	///<summary>
	///Implementation for the template element
	///    &lt;!ELEMENT template 
	///                   (behavior|constant|content|d-class|d-component| 
	///                    interface |logic|part|peers|presentation|property| 
	///                    restructure|rule |script|structure|style)&gt;
	///    &lt;!ATTLIST template id NMTOKEN #IMPLIED&gt;
	///
	/// Templates that are resolved only if they are "sourced" by an element.
	/// This means we have to execute an open file operation each time an element
	/// sources a template. 
	///</summary>
	public class Template : IUimlElement{
		public String m_identifier;
		public IUimlElement m_top;

		public Template()
		{
		}

		public Template(string identifier)
		{
			m_identifier = identifier;
			//TODO:  remove UimlTool reference
			//LoadTemplate(identifier, UimlTool.FileName);
		}

		public Template(Uri turi)
		{
			m_identifier = turi.Fragment;
			if (m_identifier != "" && m_identifier[0] == '#')
				m_identifier = m_identifier.Substring(1); // cut off '#'
				
			//split the identifier and the actual uri
			LoadTemplate(m_identifier, turi.GetLeftPart(UriPartial.Path));
		}

		public Template(XmlNode n) : this()
		{
			Process(n);
		}

        public virtual object Clone()
        {
            Template clone = new Template();

            clone.m_identifier = m_identifier;
            if(m_top != null)
            {
                clone.m_top = (IUimlElement)m_top.Clone();
            }

            return clone;
        }

		private void LoadTemplate(string tidentifier, string path)
		{			
			if(TemplateRepository.Instance.Query(tidentifier)==null)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(path);
				//search for the template with identifier tidentifier
				//and process it accordingly
				XmlNode n = SearchTemplate(doc, tidentifier);
				if(n==null)
					Console.WriteLine("Template {0} could not be found in {1}",tidentifier, path);
				else
					Process(n);
			}
			else
				throw new TemplateAlreadyProcessedException(tidentifier);
		}

		///<summary>
		///Does a recursive search for a template with the identifier tidentifier. 
		///</summary>
		private XmlNode SearchTemplate(XmlNode xmlNode, String tidentifier)
		{
			XmlNode found=null;
			XmlAttributeCollection attr = xmlNode.Attributes;
			
			if((xmlNode.Name == IAM) && (attr.GetNamedItem(ID) != null))
			{
				if(attr.GetNamedItem(ID).Value == tidentifier)
					return xmlNode;
			}
			
			if(xmlNode.HasChildNodes)
			{
				XmlNodeList xnl = xmlNode.ChildNodes;
				for(int i=0; i<xnl.Count; i++)
				{
					found=SearchTemplate(xnl[i],tidentifier);
					if(found!=null)
						return found;
				}
			}
			return null;
		}

		

		public void Process(XmlNode n)
		{
			if(n.Name != IAM)
				return;

			// process identifier
			XmlAttributeCollection attr = n.Attributes;
			if(attr.GetNamedItem(ID) != null)
				 Identifier = attr.GetNamedItem(ID).Value;
			
			// need a Factory here to replace the 
			// overloaded case here with a polymorf create method
			XmlNodeList xnl = n.ChildNodes;
			switch(xnl[0].Name)
			{
				//TODO: change the Behavior impelementation to allow 
				//behavior creation as part of a template
				//case Behavior.IAM:
				//	m_top = new Behavior(xnl[0]);
				//	break;
				case Constant.IAM:
					m_top = new Constant(xnl[0]);
					break;
				case Content.IAM:
					m_top = new Content(xnl[0]);
					break;
				case Interface.IAM:
					m_top = new Interface(xnl[0]);
					break;
				case Logic.IAM:
					m_top = new Logic(xnl[0]);
					break;
				case Part.IAM:
					m_top = new Part(xnl[0]);
					break;
				case Peer.IAM:
					m_top = new Peer(xnl[0]);
					break;
				case Presentation.IAM:
					m_top = new Presentation(xnl[0]);
					break;
				case Property.IAM:
					m_top = new Property(xnl[0]);
					break;
				case Restructure.IAM:
					m_top = new Restructure(xnl[0]);
					break;
				case Executing.Rule.IAM:
					m_top = new Executing.Rule(xnl[0]);
					break;
				case Executing.Script.IAM:
					m_top = new Executing.Script(xnl[0]);
					break;
				case Structure.IAM:
					m_top = new Structure(xnl[0]);
					break;
				case Style.IAM:
					m_top = new Style(xnl[0]);
					break;
				default:
					Console.WriteLine("Templates can not contain \"{0}\" ", xnl[0].Name);
					break;
			}

			//register this template in the repository
			TemplateRepository.Instance.Register(this);
		}

		public string Identifier
		{
			get { return m_identifier;  }
			set { m_identifier = value; }
		}

		public IUimlElement Top
		{
			get { return m_top; }
		}

		public ArrayList Children
		{
			get 
			{ 
				ArrayList a = new ArrayList();
				a.Add(Top); 
				return a; 
			}
		}

		public static ITemplateResolver GetResolver(string how)
		{
			switch(how)
			{
				case UimlAttributes.UNION :
					return new UnionTemplateResolver();
				case UimlAttributes.REPLACE :
					return new ReplaceTemplateResolver();
				case UimlAttributes.CASCADE :
					return new CascadeTemplateResolver();
				default:
					Console.WriteLine("Unknown template strategy \"{0}\"", how);
					return null;
			}
		}


		public const string IAM = "template";
		public const string ID = "id";

	}
}
