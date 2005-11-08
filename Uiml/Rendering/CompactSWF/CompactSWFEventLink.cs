/*
 	 Uiml.Net: a Uiml.Net renderer (http://lumumba.uhasselt.be/kris/research/uiml.net/)

	 Copyright (C) 2004  Kris Luyten (kris.luyten@uhasselt.be)
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

namespace Uiml.Rendering.CompactSWF
{
	using System;
	using System.Collections;
	using System.Reflection;

	using System.Windows.Forms;
	
	using Uiml;
	using Uiml.Executing;
	using Uiml.Rendering;
	

	///<summary>
	///Connects a condition from a rule with a renderer. It executes the condition
	///when an event is invoked and passes it a reference to the current renderer
	///</summary>
	public class CompactSWFEventLink
	{
		IExecutable m_exer;
		IRenderer m_renderer;

		public CompactSWFEventLink(Condition c, IRenderer renderer)
		{
			m_exer = c;
			m_renderer  = renderer;
		}

		virtual public void Execute(System.Object o, EventArgs arg)
		{
			m_exer.Execute(m_renderer);
		}
	}
}

