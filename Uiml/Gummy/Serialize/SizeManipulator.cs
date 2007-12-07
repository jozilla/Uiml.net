using System;
using System.Collections.Generic;
using System.Text;
using Uiml.Gummy.Domain;
using System.Drawing;
using Uiml.Rendering;

namespace Uiml.Gummy.Serialize
{
    public abstract class SizeManipulator : BaseManipulator
    {
        public SizeManipulator(DomainObject dom, IRenderer renderer) : base (dom,renderer)
        {             
        }

        public abstract Size Size
        {
            get;
            set;
        }
      
    }
}
