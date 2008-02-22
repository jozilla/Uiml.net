using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services
{
    public interface IServiceContainer
    {
        List<IService> Services
        {
            get;
        }

        IService GetService(string name);
        void AttachService(IService service);
    }
}
