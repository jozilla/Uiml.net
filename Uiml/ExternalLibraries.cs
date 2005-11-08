/*
  	 Uiml.Net: a Uiml.Net renderer (http://research.edm.uhasselt.be/kris/research/uiml.net/)
   
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

namespace Uiml
{

	using System;
	using System.Collections;
	using System.Reflection;

	///<summary>
	/// Serves as a repository of assemblies that can be used by the backend and
	/// by the uiml document (behavior, logic, "embedded" scripts
	///</summary>
	public class ExternalLibraries : Hashtable 
	{

		private static ExternalLibraries m_instance = null;
		private ArrayList m_list;
		private bool m_dirty = true;

		//private ArrayList m_externalLibs;

		private ExternalLibraries() : base()
		{
			//m_externalLibs = new ArrayList();
		}


		public void Add(String libRef)
		{
			//try to load assembly:
			try
			{
				Assembly b = Assembly.LoadFrom(libRef);
				base.Add(libRef, b);
				m_dirty=true;
			}
				catch(ArgumentNullException ane)
				{
					Console.WriteLine("Could not load assembly: no name given");
				}
				catch(System.IO.FileNotFoundException fnfe)
				{
					Console.WriteLine("Could not locate assembly {0}", libRef);
				}
				#if !COMPACT
				catch(BadImageFormatException bimf)
				{
					Console.WriteLine("Library {0} is not a valid assembly", libRef);
				}
				#endif
				catch(System.Security.SecurityException se)
				{
					Console.WriteLine("Usage of assembly {0} is not permitted", libRef);
				}
		}

		public void Add(String key, Assembly lib)
		{
			if(!base.ContainsKey(key))
			{
				base.Add(key, lib);
				m_dirty=true;
			}
		}

		public Assembly GetAssembly(String tkey)
		{
			//the Hashtable "Item" property is for languages that do not support operator overloading
			return (Assembly)base[tkey];
		}

		public IEnumerator LoadedAssemblies
		{
			get {	return base.Values.GetEnumerator(); }
		}

		///<summary>
		///Converts the values in this hashtable into an ArrayList. 
		//To speed things up, the list will be created only the first time,
		//or if m_dirty is set.
		///</summary>
		public ArrayList Assemblies
		{
			get {
				if((m_list==null)||(m_dirty))
				{
					m_list = new ArrayList();
					IEnumerator v = GetEnumerator();
					while(v.MoveNext())
						m_list.Add((Assembly)((DictionaryEntry)v.Current).Value);
					m_dirty = false;
				}
				return m_list;
			}
		}
	


		public static ExternalLibraries Instance
		{
			get
			{
				if(m_instance == null)
					m_instance = new ExternalLibraries();
				return m_instance;
			}

			set { m_instance = value; }
		}
	}
}
