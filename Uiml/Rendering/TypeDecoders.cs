/*
 *   Uiml.Net: a Uiml.Net renderer 
 *   http://research.edm.uhasselt.be/~uiml/
 *   
 *   Copyright (C) 2003-2007  Expertise Centre for Digital Media
 *                            Hasselt University
 *                            http://edm.uhasselt.be/
 *                       
 *   This program is free software; you can redistribute it and/or
 *   modify it under the terms of the GNU Lesser General Public License
 *   as published by the Free Software Foundation; either version 2.1
 *   of	the License, or (at your option) any later version.
 *   
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Lesser General Public License for more details.
 *   
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program; if not, write to the Free Software
 *   Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 *   Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>
 */

using System;
using System.Collections;
using System.Collections.Generic;

using Uiml.Rendering.TypeDecoding;

namespace Uiml.Rendering
{
	public class TypeDecoders
	{
        [TypeDecoderMethod]
		public static string[] DecodeStringArray(Constant constant)
		{
		    List<string> strList = new List<string>();
		    
			IEnumerator enumConstants = constant.Children.GetEnumerator();
			while(enumConstants.MoveNext())
			{
				Constant child = (Constant)enumConstants.Current;
				strList.Add((string) child.Value);
			}
			
			return strList.ToArray();
		}
	}
}
