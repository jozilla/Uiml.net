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
using Uiml.LayoutManagement;
using Cassowary;

namespace Uiml.LayoutManagement
{
	/// <summary>
	/// This class builds a constraint system from the UIML document,
	/// which will be fed to the solver.
	/// </summary>
	public class ConstraintSystem
	{
		private ArrayList m_constraints;
		private ClSimplexSolver m_solver;

		public ConstraintSystem(ArrayList layouts)
		{
			m_constraints = new ArrayList();
			m_solver = new ClSimplexSolver();
			Process(layouts);
		}

		public ConstraintSystem(Layout l)
		{
			m_constraints = new ArrayList();
			m_solver = new ClSimplexSolver();
			Process(l);
		}

		public ConstraintSystem(UimlDocument doc) : this(doc.UInterface.ULayout)
		{}

		protected void Process(Layout l)
		{
			foreach(ClConstraint c in l.Constraints)
			{
				m_constraints.Add(c);
			}
		}

		protected void Process(ArrayList layouts)
		{
			foreach (Layout l in layouts)
			{
				Process(l);
			}
			_SortConstraints(); // make sure stay constraints are at front
		}

		protected void _SortConstraints()
		{
			ArrayList stay_constraints = new ArrayList();
			ArrayList non_stay_constraints = new ArrayList();
				
			foreach (ClConstraint c in m_constraints)
			{
				if (c is ClStayConstraint)
				{
					stay_constraints.Add(c);
				}
				else
				{
					non_stay_constraints.Add(c);
				}
			}

			m_constraints.Clear();
			m_constraints.AddRange(stay_constraints);
			m_constraints.AddRange(non_stay_constraints);
		}

		public void Solve()
		{
			foreach (ClConstraint c in m_constraints)
			{
				try 
				{
					m_solver.AddConstraint(c);
				} 
				catch(ExClTooDifficult td)
				{
					Console.WriteLine("{0} -> {1}", td, c);
					Console.WriteLine("Trying to continue...");
				}
				catch(ExClRequiredFailure rf)
				{
					Console.WriteLine("{0} -> {1}", rf, c);
					Console.WriteLine("Trying to continue...");
				}
				catch(ExClInternalError ie)
				{
					Console.WriteLine("{0} -> {1}", ie, c);
					Console.WriteLine("Trying to continue...");
				}
				catch(ExClError ce)
				{
					Console.WriteLine("{0} -> {1}", ce, c);
					Console.WriteLine("Trying to continue...");
				}
			}
			m_solver.Solve();
		}

		public void Resolve()
		{
			foreach (LayoutProperty lp in LayoutPropertyRepository.Instance)
			{
				m_solver.SuggestValue(lp.Variable, (double) lp.Value);
			}

			m_solver.Resolve();
		}
	}
}
