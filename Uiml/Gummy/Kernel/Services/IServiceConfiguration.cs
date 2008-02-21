using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services
{
    public interface IServiceConfiguration
    {
        System.Windows.Forms.Control ServiceConfigurationControl { get ; }
        bool Optional { get; }
        bool Ready { get; }
        IService Service { get; }
        event ReadyStateChangedEventHandler ReadyStateChanged;
    }

    public delegate void ReadyStateChangedEventHandler(IServiceConfiguration sender, ReadyStateChangedEventArgs e);

    public class ReadyStateChangedEventArgs : EventArgs
    {
        private bool m_ready;

        public bool Ready
        {
            get { return m_ready; }
        }

        public ReadyStateChangedEventArgs(bool ready)
        {
            m_ready = ready;
        }
    }
}
