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
	using System.Collections;


	//TODO
	//Redundancy with vocabulary has to be removed!!!
	//Will merge the duplicated functionality in a single class
	public class Logic : UimlAttributes, IUimlElement
	{

		private XPathNavigator m_logicDocument;

		public Logic()
		{
		}


		public Logic(XmlNode n) : this() 
		{
			Process(n);
		}

		public void Process(XmlNode n)
		{
			if(n.Name == IAM)
			{
				base.ReadAttributes(n);
				m_logicDocument = n.CreateNavigator();
			}
		}

		//WARNING TODO remove redundant code from Vocabulary: this code
		//is identical
		public const string GetMethodParamsExpr_1 = "//d-component[@id='";
		public const string GetMethodParamsExpr_2 =  "']/d-method[@id='";
		public const string GetMethodParamsExpr_3 = "']/d-param";
		public Param[] GetMethodParams(string componentName, string methodName)
		{
			string xpexprstr =  GetMethodParamsExpr_1 + componentName + GetMethodParamsExpr_2 + methodName + GetMethodParamsExpr_3;
			XPathExpression xpExpr = m_logicDocument.Compile(xpexprstr);
			XPathExpression xpCountExpr = m_logicDocument.Compile("count(" + xpexprstr + ")");
			
			int nrp = Int32.Parse(m_logicDocument.Evaluate(xpCountExpr).ToString());
			if(nrp == 0)
				return new Param[0];
		
			XPathNodeIterator xpni = m_logicDocument.Select(xpExpr);
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

		private string SelectFirst(XPathExpression xpExpr, string searchingFor)
		{
			XPathNodeIterator xpni = m_logicDocument.Select(xpExpr);
			
			if(!xpni.MoveNext())
			{
				throw new MappingNotFoundException(searchingFor);
			}
			else
			{
				return xpni.Current.Value;
			}
		}

		public const string MapsOnComponentExpr_1 = "//d-component[@id='";
		public const string MapsOnExpr_2 = "']/@maps-to";
		public string MapsOnComponent(string abstractName)
		{
			XPathExpression xpExpr = m_logicDocument.Compile(MapsOnComponentExpr_1 + abstractName + MapsOnExpr_2);
			return SelectFirst(xpExpr, abstractName);
		}

		
		public const string GetMethodExpr_2 = "']/d-method[@id='";
		public const string GetMethodExpr_3 = "']/@maps-to";
		public const string GetMethodComponentExpr_1 = "//d-component[@id='";
		public string GetMethodComponent(string identifier, string abstractName)
		{
			XPathExpression xpExpr = m_logicDocument.Compile(GetMethodComponentExpr_1 + abstractName + GetMethodExpr_2 + identifier + GetMethodExpr_3);
			return SelectFirst(xpExpr, identifier + ":" + abstractName);
		}

		public ArrayList Children
		{
			get { return null; }
		}

		public const string IAM = "logic";
	}
}

