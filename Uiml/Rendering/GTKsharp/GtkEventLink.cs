/*
 	 Uiml.Net: a Uiml.Net renderer (http://lumumba.luc.ac.be/kris/research/uiml.net/)

	 Copyright (C) 2004  Kris Luyten (kris.luyten@luc.ac.be)
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

namespace Uiml.Rendering.GTKsharp
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Gtk;
	using GtkSharp;

	using Uiml;
	using Uiml.Executing;
	using Uiml.Rendering;
	

	///<summary>
	///Connects a condition from a rule with a renderer. It executes the condition
	///when an event is invoked and passes it a reference to the current renderer
	///</summary>
	public class GtkEventLink
	{
		IExecutable m_exer;
		IRenderer m_renderer;

		public GtkEventLink(Condition c, IRenderer renderer)
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

