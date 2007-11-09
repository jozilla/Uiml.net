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

namespace Uiml.Rendering.TypeDecoding
{
		public struct Signature
		{
		    Type To;
		    Type From;
		    
		    public Signature(Type from, Type to)
		    {
		        From = from;
		        To = to;		        
		    }
		    
		    public override string ToString()
		    {
		        return string.Format("{0} => {1}", From, To);
		    }
		    
		    public bool IsValid
		    {
		        get { return !(From == null && To == null); }
		    }
		}
}
