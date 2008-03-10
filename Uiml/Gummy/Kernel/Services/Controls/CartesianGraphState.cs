using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    ///<summary>
    //A class that is responsible for the interaction with the cartesian graph
    ///</summary>
    public abstract class CartesianGraphState
    {
        protected CartesianGraph m_graph = null;

        public CartesianGraphState(CartesianGraph cartesianGraph)
        {
            CartesianGraph = cartesianGraph;
        }

        public virtual CartesianGraph CartesianGraph
        {
            get
            {
                return m_graph;
            }
            set
            {
                m_graph = value;
            }
        }

        public abstract void DestroyEvents();
    }
}
