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



namespace Uiml{

	using System;
	using System.Xml;
	using System.Collections;

	public class Style : IUimlElement, ICloneable{

		private ArrayList m_properties;
		private string m_identifier;
		private string m_source;

		public Style(){
			m_properties = new ArrayList();
		}

		public Style(XmlNode n) : this()
		{
			Process(n);
		}

		/// <summary>
		/// Useful for applying a template in the same instance
		/// as the one that should be changed.
		/// </summary>
		public void Change(Style other)
		{
			m_properties = other.m_properties;
			m_identifier = other.m_identifier;
			m_source = other.m_source;
		}
		
		public string Identifier
		{
			get { return m_identifier; }
			set { m_identifier = value; }
		}

		public string Source
		{
			get { return m_source; }
			set {	m_source = value;	}
		}

		public void Process(XmlNode n)
		{
			if(n.Name != STYLE)
				return;

			// process attributes
			foreach (XmlAttribute a in n.Attributes)
			{
				if (a.Name == "source")
				{
					Source = a.Value;
					break;
				}
			}

			XmlNodeList xnl = n.ChildNodes;
			for(int i=0; i<xnl.Count; i++)
				m_properties.Add(new Property(xnl[i]));
			
			if (Source != null)
				ApplyTemplate();
		}

		public void ApplyTemplate()
		{
			Template t = new Template(new Uri(Source));
			ITemplateResolver rtr = Template.GetResolver("replace");
			rtr.Resolve(t, this);
		}

		public IEnumerator GetNamedProperties(string identifier)
		{
			ArrayList props = new ArrayList();

			IEnumerator enumAll = m_properties.GetEnumerator();
			while(enumAll.MoveNext())
			{			
				if( ((Property)enumAll.Current).PartName == identifier)
				{
					props.Add(enumAll.Current);
				}
			}

			return props.GetEnumerator();
		}

		///<summary>
		/// Searches for a specific property of a given part
		///</summary>
		public Property SearchProperty(string partIdentifier, string partProperty)
		{
			IEnumerator enumAll = m_properties.GetEnumerator();
			while(enumAll.MoveNext())
			{
				Property p = (Property)enumAll.Current;
				if((p.PartName == partIdentifier) && (p.Name == partProperty))
					return p;
			}
			return null;
		}

		public IEnumerator GetClassProperties(string className)
		{
			ArrayList props = new ArrayList();
			IEnumerator enumAll = m_properties.GetEnumerator();

			while(enumAll.MoveNext())
			{
				if( ((Property)enumAll.Current).PartClass == className)
					props.Add(enumAll.Current);
			}

			return props.GetEnumerator();
		}

		///<summary>
		///Get the properties that match the given set of parameters for the identifier.
		///</summary>
		public Property[] GetProperties(string identifier, Param[] parameters)
		{
			IEnumerator enumAll = GetNamedProperties(identifier);
			Property[] props = new Property[parameters.Length];
			int i = 0;
			int j=1;
			while(enumAll.MoveNext())
			{
				if(i<parameters.Length)
				{
					if(((Property)enumAll.Current).Name == parameters[i].Identifier)
					{
						props[i++] = (Property)enumAll.Current;
						enumAll.Reset();//Make sure all named properties are traversed again; there is no
						                //constraint on the order in which the parameters are specified
											 //in the uiml document
					}
				}
			}
			return props;
		}

		///<description>
		/// Given a set of types and parameter names, this method searches for the best matching
		/// property in the style definition
		///</description>
		public Property SearchMatchingStyle(Type[] types, string[] tparams)
		{
			return null;
		}

		public ArrayList Children
		{
			get { return m_properties; }
			set { m_properties = value; }
		}
		
		//ICloneable interface implementation:
		public object Clone()
		{
			Style iamclone = new Style();
			iamclone.Identifier = (string)Identifier.Clone();
			iamclone.Source     = (string)Source.Clone();
			iamclone.Children   = (ArrayList)Children.Clone();
			return iamclone;
		}

		public const string STYLE = "style"; // deprecated
		public const string IAM = "style";
	}
}
