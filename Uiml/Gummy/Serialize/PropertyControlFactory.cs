using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Uiml;

namespace Uiml.Gummy.Serialize
{
    public class PropertyControlFactory : IPropertyControlFactory
    {
        public PropertyControlFactory()
        {
        }

        public virtual Control CreatePropertyControl(Property p)
        {
            
            return createTextboxControl();
        }

        protected Control createTextboxControl()
        {
            return new TextBox();
        }
    }
}
