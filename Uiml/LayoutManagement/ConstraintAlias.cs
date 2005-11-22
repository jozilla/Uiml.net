/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.uhasselt.be/kris/research/uiml.net)

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

	Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>
*/

using System;
using System.Collections;
using System.Xml;

using Cassowary;

namespace Uiml.LayoutManagement
{
	public class ConstraintAlias
	{
		private Values m_type;
		private string m_rule;
		
		public ConstraintAlias()
		{
		}

		public ConstraintAlias(string name, string parameters) : this()
		{
			Process(name, parameters);
		}

		public void Process(string name, string parameters)
		{
			ProcessSemantics(name);
			ProcessParameters(parameters);
		}
		
		public void ProcessSemantics(string name)
		{
			switch (name)
			{
				case LEFT_OF:
					m_type = Values.LeftOf;
					break;
				case RIGHT_OF:
					m_type = Values.RightOf;
					break;
				case ABOVE:
					m_type = Values.Above;
					break;
				case BELOW:
					m_type = Values.Below;
					break;
				case LEFT_ALIGNED:
					m_type = Values.LeftAligned;
					break;
				case RIGHT_ALIGNED:
					m_type = Values.RightAligned;
					break;
				case TOP_ALIGNED:
					m_type = Values.TopAligned;
					break;
				case BOTTOM_ALIGNED:
					m_type = Values.BottomAligned;
					break;
				case BEHIND:
					m_type = Values.Behind;
					break;
				case IN_FRONT_OF:
					m_type = Values.InFrontOf;
					break;
			}
		}

		public void ProcessParameters(string parameters)
		{
			string param1, param2;
			string[] splittedParams;
			
			splittedParams = parameters.Split(new char[] { PARAM_DELIMITER });

			try
			{
				param1 = splittedParams[0];
				param2 = splittedParams[1];

				switch (Type)
				{
					case Values.LeftOf:
						m_rule = string.Format("{0}.right <= {1}.left", param1, param2);
						break;
					case Values.RightOf:
						m_rule = string.Format("{0}.left >= {1}.right", param1, param2);
						break;
					case Values.Above:
						m_rule = string.Format("{0}.bottom <= {1}.top", param1, param2);
						break;
					case Values.Below:
						m_rule = string.Format("{0}.top >= {1}.bottom", param1, param2);
						break;
					case Values.LeftAligned:
						m_rule = string.Format("{0}.left = {1}.left", param1, param2);
						break;
					case Values.RightAligned:
						m_rule = string.Format("{0}.right = {1}.right", param1, param2);
						break;
					case Values.TopAligned:
						m_rule = string.Format("{0}.top = {1}.top", param1, param2);
						break;
					case Values.BottomAligned:
						m_rule = string.Format("{0}.bottom = {1}.bottom", param1, param2);
						break;
					case Values.Behind:
						// we don't have strict inequalities, so use an extra term 
						// to enforce strict inequality
						m_rule = string.Format("{0}.depth >= {1}.depth + 1", param1, param2);
						break;
					case Values.InFrontOf:
						// we don't have strict inequalities, so use an extra term 
						// to enforce strict inequality
						m_rule = string.Format("{0}.depth + 1 <= {1}.depth", param1, param2);
						break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Processing constraint alias failed: " + e);
			}
		}
		
		public string Rule
		{
			get { return m_rule; }
		}
		
		public Values Type
		{
			get { return m_type; }
		}

		public enum Values { 
			LeftOf, 
			RightOf, 
			Above, 
			Below, 
			LeftAligned, 
			RightAligned, 
			TopAligned, 
			BottomAligned, 
			Behind,
			InFrontOf
		}
		
		public const string LEFT_OF 		= "left-of";
		public const string RIGHT_OF 		= "right-of";
		public const string ABOVE 		= "above";
		public const string BELOW 		= "below";
		public const string LEFT_ALIGNED 	= "left-aligned";
		public const string RIGHT_ALIGNED 	= "right-aligned";
		public const string TOP_ALIGNED 	= "top-aligned";
		public const string BOTTOM_ALIGNED 	= "bottom-aligned";
		public const string BEHIND		= "behind";
		public const string IN_FRONT_OF		= "in-front-of";

		public const char PARAM_DELIMITER = ',';
	}
}
