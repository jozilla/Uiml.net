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
	using System.Collections;

	public class Style : UimlAttributes, IUimlElement, ICloneable {

		private ArrayList m_properties;

		public Style(){
			m_properties = new ArrayList();
		}

		public Style(XmlNode n) : this()
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name != STYLE)
				return;

			ReadAttributes(n);
			
			XmlNodeList xnl = n.ChildNodes;
			for(int i=0; i<xnl.Count; i++)
				m_properties.Add(new Property(xnl[i]));
			
			if(SourceAvailable)
			{
				ITemplateResolver templateResolver = Template.GetResolver(How);
				Template t = TemplateRepository.Instance.Query(Source);				
				templateResolver.Resolve(t,this);
			}
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

		public void RemoveProperty(Property p)
		{
			m_properties.Remove(p);
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
