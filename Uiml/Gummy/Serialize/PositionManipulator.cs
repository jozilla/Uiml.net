using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Uiml.Gummy.Domain;
using Uiml.Rendering;

namespace Uiml.Gummy.Serialize
{
    public abstract class PositionManipulator : BaseManipulator
    {

        public PositionManipulator(DomainObject dom, IRenderer renderer) : base(dom,renderer)
        {
        }


        public abstract Point Position
        {
            get;
            set;
        }
    }
}
