using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public interface IBehaviorGenerator
    {
        Logic GenerateLogic();
        Behavior GenerateBehavior();
    }
}
