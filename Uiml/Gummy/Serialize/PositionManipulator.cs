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

        public PositionManipulator(DomainObject dom) : base(dom)
        {
        }


        public abstract Point Position
        {
            get;
            set;
        }
    }
}
