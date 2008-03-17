using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public interface IMethodParameterBinding
    {
        XmlNode GetUiml(XmlDocument doc);
        void Break();
    }
}
