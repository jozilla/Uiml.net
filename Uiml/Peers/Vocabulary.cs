/*
  	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)
   
	 Copyright (C) 2005  Kris Luyten (kris.luyten@uhasselt.be)
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

	Authors: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/

using System;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections;

using Uiml.Utils;

namespace Uiml.Peers
{
	/// <summary>
	/// Summary description for Vocabulary.
	/// </summary>
	public class Vocabulary : ICloneable
	{
		// a Hashtable of (name, DClass) key-value pairs
		protected Hashtable m_dictDCls;
		// a Hashtable of (name, DComponent) key-value pairs
		protected Hashtable m_dictDCmp;
		protected string m_identifier;
		protected XmlDocument m_doc;
		protected string m_vocName;

		public Vocabulary()
		{
			m_dictDCls = new Hashtable();
			m_dictDCmp = new Hashtable();
		}

		public Vocabulary(string vocName)
		{
			m_dictDCls = new Hashtable();
			m_dictDCmp = new Hashtable();
			m_vocName = vocName;
			
			Load(vocName);
		}

        public virtual object Clone()
        {
            Vocabulary clone = new Vocabulary();

            clone.m_identifier = m_identifier;
            clone.m_doc = (XmlDocument)m_doc.Clone();
            clone.m_vocName = m_vocName;
            
            IDictionaryEnumerator e = m_dictDCls.GetEnumerator();
            while (e.MoveNext())
            {
                clone.m_dictDCls[e.Key] = ((DClass)e.Value).Clone();
            }

            e = m_dictDCmp.GetEnumerator();
            while (e.MoveNext())
            {
                clone.m_dictDCmp[e.Key] = ((DComponent)e.Value).Clone();
            }

            return clone;
        
        }

		protected void Load(string vocName)
		{
            // First try to load the string "as is". If that doesn't
            // work, try to load it from other locations.
			m_doc = new XmlDocument();
			XmlTextReader xr = null;
			try
			{
				xr = new XmlTextReader(vocName);
				m_doc.Load(xr);
				Parse();
			}
			catch(Exception)
			{
				Console.WriteLine("Could not load '{0}' vocabulary, trying other locations:", vocName);

                string vocFile = Path.GetFileName(vocName);

                foreach (string loc in Uiml.Utils.Location.VocabularyLocations)
                {
                    try 
                    {
                        Console.Write(" * {0} ... ", loc);
                        string voc = Path.Combine(loc, vocFile);
                        xr = new XmlTextReader(voc);
                        m_doc.Load(xr);
                        Parse();
                        Console.WriteLine("[OK]");
                        return;
                    }
                    catch (Exception)
                    {
                        // ignore
                        Console.WriteLine("[Failed]");
                        continue;
                    }
                }

                // if we ever get here, no vocabulary could be loaded
                throw new VocabularyUnavailableException(vocName);
			}
		}

		public void MergeLogic(Logic l)
		{
			XmlNode logicNode = m_doc.ImportNode(l.Tag, true);

			XmlNode n = MoveToUimlElement(m_doc);
						
			if(n != null)
			{
				n.AppendChild(logicNode);			
				ParseLogic(logicNode);
			}
			else
				throw new Exception("UIML file not in correct format. Could not find uiml element.");
		}

		protected void Parse(XmlNode n)
		{
			IEnumerator enumNodes = n.ChildNodes.GetEnumerator();

			while(enumNodes.MoveNext())
			{
				XmlNode c = (XmlNode) enumNodes.Current;
				switch(c.Name)
				{
					case Presentation.IAM:
						ParsePresentation(c);
						break;
					case Logic.IAM:
						ParseLogic(c);
						break;
				}
			}
		}

		protected void Parse()
		{
			XmlNode n = MoveToUimlElement(m_doc);
			
			if(n != null)
				Parse(n);
			else
				throw new Exception("Vocabulary not in correct format. Could not find uiml element.");
		}

		protected XmlNode MoveToUimlElement(XmlNode n)
		{
			n = n.FirstChild;

			while(n != null && !(n.Name == "uiml" && n.NodeType == XmlNodeType.Element))			
			{
				n = n.NextSibling;
			}
			
			return n;
		}

		protected void ParsePresentation(XmlNode n)
		{
			XmlNode b = n.Attributes.GetNamedItem(Presentation.BASE);
			
			try
			{
				m_identifier = b.Value;
			}
			catch(NullReferenceException nre)
			{
				Console.WriteLine("The presentation has no base attribute, please check your UIML file!");
				Console.WriteLine(nre);
			}			

			IEnumerator enumChildren = n.ChildNodes.GetEnumerator();
			
			while(enumChildren.MoveNext())
			{
				XmlNode c = (XmlNode) enumChildren.Current;
				switch(c.Name)
				{
					case DClass.IAM:
						ParseDClass(c);
						break;
				}
			}
		}

		protected void ParseLogic(XmlNode n)
		{
			IEnumerator enumChildren = n.ChildNodes.GetEnumerator();
			
			while(enumChildren.MoveNext())
			{
				XmlNode c = (XmlNode) enumChildren.Current;
				switch(c.Name)
				{
					case DComponent.IAM:
						ParseDComponent(c);
						break;
				}
			}			
		}

		protected void ParseDClass(XmlNode n)
		{
			DClass cls = new DClass(n);
			AddDClass(cls.Identifier, cls);		
		}

		protected void ParseDComponent(XmlNode n)
		{
			DComponent cmp = new DComponent(n);
			AddDComponent(cmp.Identifier, cmp);			
		}

		/// <summary>
		/// Represents the &lt;presentation&gt; element's base attribute.
		/// </summary>		
		public string Identifier
		{
			get { return m_identifier; }
		}

		protected void AddDClass(string abstractName, DClass cls)
		{	
			try
			{
				m_dictDCls.Add(abstractName, cls);
			} 
			catch(ArgumentException ae) 
			{
				Console.WriteLine("Duplicate class entry: <{0}>", abstractName);
				Console.WriteLine("Please check the <d-class> elements in the <presentation> structure!");
			} 
		}

		protected void AddDComponent(string abstractName, DComponent cmp)
		{
			try
			{
				m_dictDCmp.Add(abstractName, cmp);
			}
			catch(ArgumentException ae)
			{
				Console.WriteLine("Duplicate component entry: <{0}>", abstractName);
				Console.WriteLine("Please check the <d-component> elements in the <logic> structure!");
			}
		}

		protected DClass FindDClass(string abstractName)
		{
			IDictionaryEnumerator e = m_dictDCls.GetEnumerator();
			
			while (e.MoveNext())
			{
				if(((string)e.Key) == abstractName)
					return (DClass) e.Value;
			}
			
			// no such class
			return null;
		}

		protected DComponent FindDComponent(string abstractName)
		{
			IDictionaryEnumerator e = m_dictDCmp.GetEnumerator();			

			while (e.MoveNext())
			{
				if(((string)e.Key) == abstractName)
					return (DComponent) e.Value;					
			}

			// no such component
			return null;
		}

		public string MapsOnCls(string abstractName)
		{
			DClass cls = FindDClass(abstractName);

			if(cls != null)
				return cls.MapsTo;
			else
				throw new MappingNotFoundException(abstractName);
		}

		public string MapsOnCmp(string abstractName)
		{
			DComponent cmp = FindDComponent(abstractName);

			if(cmp != null)
				return cmp.MapsTo;
			else
				throw new MappingNotFoundException(abstractName);
		}

		protected DMethod FindDMethod(ArrayList l, string identifier)
		{
			IEnumerator enumMethods = l.GetEnumerator();
			
			while(enumMethods.MoveNext())
			{
				DMethod method = ((DMethod)enumMethods.Current);
				
				if(method.Identifier == identifier)
					return method;				
			}

			// no such method
			return null; 
		}
		
		public string GetMethodCls(string identifier, string abstractName)
		{
			DClass cls = FindDClass(abstractName);

			if(cls == null)
				throw new MappingNotFoundException(abstractName);

			DMethod method = FindDMethod(cls.Search(typeof(DMethod)), identifier);

			if(method != null)
				return method.MapsTo;
			else
				throw new MappingNotFoundException(identifier);
		}

		public string GetMethodCmp(string identifier, string abstractName)
		{
			DComponent cmp = FindDComponent(abstractName);

			if(cmp == null)
				throw new MappingNotFoundException(abstractName);
			
			DMethod method = FindDMethod(cmp.Search(typeof(DMethod)), identifier);

			if(method != null)
				return method.MapsTo;
			else
				throw new MappingNotFoundException(identifier);
		}

		public Location GetLocationCmp(string abstractName)
		{
			DComponent cmp = FindDComponent(abstractName);

			if (cmp != null)
				return cmp.Location;
			else
				throw new MappingNotFoundException(abstractName);
		}

		protected DProperty FindDProperty(ArrayList l, string identifier)
		{
			IEnumerator e = l.GetEnumerator();

			while(e.MoveNext())
			{
				DProperty prop = ((DProperty)e.Current);
						
				if(prop.Identifier == identifier)
					return prop;
				
			}

			// no such property
			return null; 
		}

		protected DProperty FindDProperty(ArrayList l, string identifier, string mapsType)
		{
			IEnumerator e = l.GetEnumerator();

			while(e.MoveNext())
			{
				DProperty prop = ((DProperty)e.Current);
						
				if(prop.Identifier == identifier && prop.MapsType == mapsType)
					return prop;				
			}

			// no such property
			return null; 
		}

        protected DProperty GetDProperty(string identifier, string abstractName, string mapsType)
        {
            DClass cls = FindDClass(abstractName);

            if (cls == null)
                throw new MappingNotFoundException(abstractName);

            DProperty prop = FindDProperty(cls.Search(typeof(DProperty)), identifier, mapsType);

            if (prop != null)
                return prop;
            else
                throw new MappingNotFoundException(abstractName, identifier);
        }

		public string GetProperty(string identifier, string abstractName, string mapsType)
		{
            return GetDProperty(identifier, abstractName, mapsType).MapsTo;
		}

        public DProperty GetDPropertyGetter(string identifier, string abstractName)
        {
            return GetDProperty(identifier, abstractName, DProperty.GET_METHOD);
        }

        public DProperty GetDPropertySetter(string identifier, string abstractName)
        {
            return GetDProperty(identifier, abstractName, DProperty.SET_METHOD);
        }

		public string GetPropertyGetter(string identifier, string abstractName)
		{
			return GetProperty(identifier, abstractName, DProperty.GET_METHOD);
		}

		public string GetPropertySetter(string identifier, string abstractName)
		{
			return GetProperty(identifier, abstractName, DProperty.SET_METHOD);
		}

		public string MapsOnHandler(string abstractName)
		{
			DClass cls = FindDClass(abstractName);
			if(cls == null || cls.MapsType != "delegate")
				throw new MappingNotFoundException(abstractName);

			return cls.MapsTo;
		}

		public string GetEventFor(string abstractName, string delegateTypeName)
		{
			DClass cls = FindDClass(abstractName);
			if(cls == null)
				throw new MappingNotFoundException(abstractName);
			
			// search through all <d-property> elements
			IEnumerator enumProp = cls.Search(typeof(DProperty)).GetEnumerator();

			while(enumProp.MoveNext())
			{
				DProperty prop = ((DProperty)enumProp.Current);				

				// search through all <d-param> elements
				IEnumerator enumParam = prop.Search(typeof(DParam)).GetEnumerator();
	
				while(enumParam.MoveNext())
				{
					DParam param = ((DParam)enumParam.Current);

					if(param.Type == delegateTypeName)
						return prop.MapsTo;
				}
			}

			throw new MappingNotFoundException(delegateTypeName);
		}

        public DClass GetEventClassFor(DProperty prop)
        {
            IEnumerator enumParam = prop.Search(typeof(DParam)).GetEnumerator();

            while (enumParam.MoveNext())
            {
                DParam param = ((DParam)enumParam.Current);

                IDictionaryEnumerator e = m_dictDCls.GetEnumerator();
                
                while (e.MoveNext())
                {
                    DClass cls = (DClass) e.Value;
                    if (cls.MapsTo == param.Type)
                        return cls;
                }

                throw new MappingNotFoundException(prop.Identifier, param.Type);
            }
            
            throw new MappingNotFoundException(prop.Identifier);
        }

        public DParam[] GetParams(string identifier, string abstractName)
        {
           return GetParams(identifier, abstractName, null);
        }
		        
		public DParam[] GetParams(string identifier, string abstractName, string mapsType)
		{
			DClass cls = FindDClass(abstractName);
			if(cls == null)
				throw new MappingNotFoundException(abstractName);
			
            DProperty prop = null;
            if(mapsType == null)
                prop = FindDProperty(cls.Search(typeof(DProperty)), identifier);
            else
			    prop = FindDProperty(cls.Search(typeof(DProperty)), identifier, mapsType);

			if(prop != null)
				return (DParam[]) prop.Search(typeof(DParam)).ToArray(typeof(DParam));			
			else
				throw new MappingNotFoundException(identifier);
		}

		public DParam[] GetMethodParams(string componentName, string methodName)
		{
			DComponent cmp = FindDComponent(componentName);

			if(cmp == null)
				throw new MappingNotFoundException(componentName);

			DMethod method = FindDMethod(cmp.Search(typeof(DMethod)), methodName);
			if(method != null)
				return (DParam[]) method.Search(typeof(DParam)).ToArray(typeof(DParam));
			else
				throw new MappingNotFoundException(methodName);
		}

        public Hashtable DClasses
        {
            get { return m_dictDCls; }
        }

        public Hashtable DComponents
        {
            get { return m_dictDCmp; }
        }

		public string VocabularyName
		{
			get { return m_vocName; }
		}

        public const string VOCABULARY_EXT   = "uiml";		
	}
}
