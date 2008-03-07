using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Uiml;

namespace Uiml.Gummy.Serialize
{
    public interface IPropertyControlFactory
    {
        Control CreatePropertyControl(Property p);
    }
}
