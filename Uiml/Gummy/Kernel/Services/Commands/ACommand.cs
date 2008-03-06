using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public abstract class ACommand : ICommand
    {
        protected List<ICommand> m_commands = new List<ICommand>();
        protected string m_label = "";
        protected bool m_enabled = true;

        public virtual List<ICommand> SubCommands
        {
            get
            {
                return m_commands;
            }
            set
            {
                m_commands = value;
            }
        }
                
        public abstract void Execute();
        public abstract void Undo();
        public virtual string Label
        {
            get
            {
                return m_label;
            }
            set
            {
                m_label = value;
            }
        }
        public virtual bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }
    }
}
