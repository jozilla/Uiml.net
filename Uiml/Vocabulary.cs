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
	using System.Xml.XPath;
	using System.IO;

	///<summary>
	/// This class provides an interface for querying uiml peers (vocabularies), defining
	/// mappings from abstract identifiers to the concrete implementations
	///
	/// This class needs serious rewriting to avoid redundancy
	///</summary>
	public abstract class Vocabulary
	{

		private string m_vocName;
		protected XPathNavigator m_vocabulary;

		public Vocabulary()
		{}

		public Vocabulary(string vocName)
		{
			VocabularyName = vocName;
		}

		public string VocabularyName
		{
			get {	return m_vocName;  }
			set { m_vocName = value; }			
		}


		public const string MapsOnExpr_1 = "//d-class[@id='";
		public const string MapsOnExpr_2 = "']/@maps-to";
		public string MapsOn(string abstractName)
		{
			XPathExpression xpExpr = m_vocabulary.Compile(MapsOnExpr_1 + abstractName + MapsOnExpr_2);
			return SelectFirst(xpExpr, abstractName);
		}

		public const string MapsOnComponentExpr_1 = "//d-component[@id='";
		public string MapsOnComponent(string abstractName)
		{
			XPathExpression xpExpr = m_vocabulary.Compile(MapsOnComponentExpr_1 + abstractName + MapsOnExpr_2);
			return SelectFirst(xpExpr, abstractName);
		}

		public const string GetMethodExpr_1 = "//d-class[@id='";
		public const string GetMethodExpr_2 = "']/d-method[@id='";
		public const string GetMethodExpr_3 = "']/@maps-to";
		public string GetMethod(string identifier, string abstractName)
		{
			XPathExpression xpExpr = m_vocabulary.Compile(GetMethodExpr_1 + abstractName + GetMethodExpr_2 + identifier + GetMethodExpr_3);
			return SelectFirst(xpExpr, identifier + ":" + abstractName);
		}


		public const string GetMethodComponentExpr_1 = "//d-component[@id='";
		public string GetMethodComponent(string identifier, string abstractName)
		{			
			XPathExpression xpExpr = m_vocabulary.Compile(GetMethodComponentExpr_1 + abstractName + GetMethodExpr_2 + identifier + GetMethodExpr_3);
			return SelectFirst(xpExpr, identifier + ":" + abstractName);
		}


		public const string GetGetPropertyExpr_1 = "//d-class[@id='";
		public const string GetGetPropertyExpr_2 = "']/d-property[@id='";
		public const string GetGetPropertyExpr_3 = "' and @maps-type='getMethod']/@maps-to";
		public string GetGetProperty(string identifier, string abstractName)
		{
			XPathExpression xpExpr = m_vocabulary.Compile(GetGetPropertyExpr_1 + abstractName + GetGetPropertyExpr_2 + identifier + GetGetPropertyExpr_3);
			return SelectFirst(xpExpr, abstractName + ":" + identifier);
		}

		public const string GetSetPropertyExpr_1 = "//d-class[@id='";
		public const string GetSetPropertyExpr_2 = "']/d-property[@id='";
		public const string GetSetPropertyExpr_3 = "' and @maps-type='setMethod']/@maps-to";
		public string GetSetProperty(string identifier, string abstractName)
		{
			XPathExpression xpExpr = m_vocabulary.Compile(GetSetPropertyExpr_1 + abstractName + GetSetPropertyExpr_2 + identifier + GetSetPropertyExpr_3);
			return SelectFirst(xpExpr, abstractName + ":" + identifier);
		}

	   
		public const string GetEventForExpr_1 = "//d-class[@id='";
		public const string GetEventForExpr_2 = "']/d-property[d-param/@type='";
		public const string GetEventForExpr_3 = "']/@maps-to";
		public string GetEventFor(string abstractClassName, string delegateTypeName)
		{
			XPathExpression xpExpr = m_vocabulary.Compile(GetEventForExpr_1 + abstractClassName + GetEventForExpr_2 + delegateTypeName + GetEventForExpr_3);
			return SelectFirst(xpExpr, abstractClassName + ":" + delegateTypeName);
		}



		private string SelectFirst(XPathExpression xpExpr, string searchingFor)
		{
			XPathNodeIterator xpni = m_vocabulary.Select(xpExpr);
			
			if(!xpni.MoveNext())
			{
				throw new MappingNotFoundException(searchingFor);
			}
			else
			{
				return xpni.Current.Value;
			}
		}



		public const string GetParamsExpr_1 = "//d-class[@id='";
		public const string GetParamsExpr_2 =  "']/d-property[@id='";
		public const string GetParamsExpr_3 = "']/d-param";
		public Param[] GetParams(string identifier, string abstractName)
		{
			XPathExpression xpExpr = m_vocabulary.Compile(GetParamsExpr_1 + abstractName + GetParamsExpr_2 + identifier + GetParamsExpr_3);
			XPathExpression xpCountExpr = m_vocabulary.Compile("count(" + GetParamsExpr_1 + abstractName + GetParamsExpr_2 + identifier + GetParamsExpr_3 + ")");
			XPathNodeIterator xpni = m_vocabulary.Select(xpExpr);
			if(!xpni.MoveNext())
			{
				throw new MappingNotFoundException(identifier + ":" + abstractName);
			}
			else
			{
				int nrp = Int32.Parse(m_vocabulary.Evaluate(xpCountExpr).ToString());
				Param[] parameters = new Param[nrp];
				int i = 0;
				do
				{
					Param p = new Param();
					XPathNavigator xpnParam = xpni.Current.Clone();
				
				   p.Value      = xpnParam.Value;
					p.Identifier = xpnParam.GetAttribute(Param.ID, xpnParam.NamespaceURI); 
					p.Type       = xpnParam.GetAttribute(Param.TYPE, xpnParam.NamespaceURI);
				   parameters[i++] = p;
				}
				while(xpni.MoveNext());
				
				return parameters;
			}

		}

		public const string GetMethodParamsExpr_1 = "//d-component[@id='";
		public const string GetMethodParamsExpr_2 =  "']/d-method[@id='";
		public const string GetMethodParamsExpr_3 = "']/d-param";
		public Param[] GetMethodParams(string componentName, string methodName)
		{
			string xpexprstr =  GetMethodParamsExpr_1 + componentName + GetMethodParamsExpr_2 + methodName + GetMethodParamsExpr_3;
			XPathExpression xpExpr = m_vocabulary.Compile(xpexprstr);
			XPathExpression xpCountExpr = m_vocabulary.Compile("count(" + xpexprstr + ")");
			
			int nrp = Int32.Parse(m_vocabulary.Evaluate(xpCountExpr).ToString());
			if(nrp == 0)
				return new Param[0];
		
			XPathNodeIterator xpni = m_vocabulary.Select(xpExpr);
			if(!xpni.MoveNext())
			{
				throw new MappingNotFoundException("Parameters of " + methodName + ":" + componentName);
			}
			else
			{
				Param[] parameters = new Param[nrp];
				int i = 0;
				do
				{
					Param p = new Param();
					XPathNavigator xpnParam = xpni.Current.Clone();
				
					p.Identifier = xpnParam.GetAttribute(Param.ID, xpnParam.NamespaceURI); 
					p.Type       = xpnParam.GetAttribute(Param.TYPE, xpnParam.NamespaceURI);
				   parameters[i++] = p;
				}
				while(xpni.MoveNext());
				
				return parameters;
			}

		}


		private void SetParam(ref Param p, String name, String value)
		{
			switch(name)
			{
				case Param.TYPE:
					p.Type = value;
					break;
				case Param.ID:
					p.Identifier = value;
					break;
			}
			
		}


		public String Identifier
		{
			get
			{
				XPathExpression xpExpr = m_vocabulary.Compile("//presentation/@base");
				XPathNodeIterator xpni = m_vocabulary.Select(xpExpr);
				xpni.MoveNext();
				return xpni.Current.Value;
			}
		}


		public const string VOCABULARY_BASE  = "http://uiml.org/toolkits/";
		public const string VOCABULARY_BASE2 = "http://lumumba.luc.ac.be/kris/projects/uiml.net/";
		public const string VOCABULARY_EXT   = "uiml";
		public const string CONSTRUCTOR      = "constructor";
	}

}


					/*
					//Other Possibility 1:
					if(xpnParam.MoveToFirstAttribute())
					{
						SetParam(ref p, xpnParam.Name, xpnParam.Value);
										
       				while (xpnParam.MoveToNextAttribute())
						{
							SetParam(ref p, xpnParam.Name, xpnParam.Value);
						}
					}
					parameters[i++] = p;
					

					//Other Possibility 2:
					string selectType = "@" + Param.TYPE;
					XPathNodeIterator type = xpnParam.Select(selectType);
					if(type.MoveNext())
					{
						SetParam(ref p, Param.TYPE, type.Current.Value);
					}

					string selectIdentifier = "@" + Param.ID;
					XPathNodeIterator ident = xpnParam.Select(selectIdentifier);
					if(ident.MoveNext())
					{
						SetParam(ref p, Param.ID, ident.Current.Value);
					}
					*/

