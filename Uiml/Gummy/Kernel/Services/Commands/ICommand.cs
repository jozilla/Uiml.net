using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public interface ICommand
    {
        //Executing the command (set a new state somewhere)
        void Execute();
        //Undo the command (revert to the state before the execution command)
        void Undo();
        //The label of the command
        string Label
        {
            get;
            set;
        }
        //SubCommands
        List<ICommand> SubCommands
        {
            get;
            set;
        }

        bool Enabled
        {
            get;
            set;
        }
    }
}
