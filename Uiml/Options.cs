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


namespace CommandLine
{
	using System;
	using System.Collections;

	public class Options
	{
		private Hashtable m_options;
		private int m_usedOptions; 

		public Options(string[] args, string[] possibilities)
		{
			Store(args,possibilities);			
		}

		private void Store(string[] args, string[] possibilities)
		{
			Console.WriteLine();
			m_options = new Hashtable();
			m_usedOptions = 0;
			for(int i=0; i<possibilities.Length; i++)
			{
				String value = "";
				bool found = false;
				int j=0;
				while((!found)&&(j<args.Length))
				{						
					if(args[j].Equals("-" + possibilities[i]))
					{
						found = true;
						value = "-";
						m_usedOptions++;
						try
						{
							if(!args[j+1].StartsWith("-"))
								value = args[j+1];
						}
							catch(IndexOutOfRangeException iore)
							{
							}
					}
					j++;
				}
				m_options.Add( possibilities[i], value);
			}
		}

		public int NrProperties
		{
			get
			{
				return m_options.Count;
			}
		}

		public int NrSwitches
		{
			get
			{
				return m_usedOptions;
			}
			
		}

		public bool IsUsed(string key)
		{
			if(this[key].Length != 0)
				return true;
			return false;
		}

		public bool HasArgument(string key)
		{
			if(this[key].Length == 0)
				return false;
			if(this[key].Equals("-"))
				return false;
			return true;
		}

		public String this[String key]
		{
			get
			{
				return (String)m_options[key];
			}
		}
	}
}
