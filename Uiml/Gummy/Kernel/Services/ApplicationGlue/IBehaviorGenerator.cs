using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public interface IBehaviorGenerator
    {
        XmlNode GenerateLogic(XmlDocument doc);
        XmlNode GenerateBehavior(XmlDocument doc);
        void Update(MethodModel m);
    }
}
