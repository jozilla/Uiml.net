/*
 	 Uiml.Net: a Uiml.Net renderer (http://lumumba.uhasselt.be/kris/research/uiml.net/)

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
	
	Author: 
		Jo Vermeulen
		jo.vermeulen@student.luc.ac.be
*/

namespace Uiml.Rendering
{
    using System;
    using System.Collections;
    using System.Reflection;

    using Uiml;
    using Uiml.Executing;
    using Uiml.Rendering;


    ///<summary>
    ///Links the events from the concrete widget set with the behavior specified in a UIML document
    ///</summary>
    public class EventLinker : IEventLinker
    {
        ConditionLinker m_linker;

        public EventLinker(IRenderer renderer)
        {
            m_linker = new ConditionLinker(renderer);
        }

        public void Link(Structure uiStruct, Behavior uiBehavior)
        {
            m_linker.Link(uiStruct, uiBehavior);
        }
    }
}

