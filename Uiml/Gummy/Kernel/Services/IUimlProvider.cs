using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services
{
    interface IUimlProvider
    {
        List<IUimlElement> GetUimlElements();
        List<string> GetUimlElementsXml();
    }
}
