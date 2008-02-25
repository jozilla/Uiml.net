using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public abstract class ACommand : ICommand
    {
        protected List<ICommand> m_commands = new List<ICommand>();

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
        public abstract string Label
        {
            get;
        }
    }
}
