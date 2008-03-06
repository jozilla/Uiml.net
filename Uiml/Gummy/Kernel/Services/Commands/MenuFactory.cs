using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services.Commands
{
    public class MenuFactory
    {
        public static void CreateMenu(List<ICommand> commands, ref Menu menu)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                MenuItem menuItem = CreateMenuItem(commands[i]);
                menu.MenuItems.Add(menuItem);
            }
        }

        private static MenuItem CreateMenuItem(ICommand command)
        {
            MenuItem menuItem = new MenuItem(command.Label);
            menuItem.Enabled = command.Enabled;
            MenuItemEventHandler handler = new MenuItemEventHandler(command, menuItem);
            foreach (ICommand subCommand in command.SubCommands)
            {
                menuItem.MenuItems.Add(CreateMenuItem(subCommand));
            }
            return menuItem;
        }

        private class MenuItemEventHandler
        {
            ICommand m_command = null;
            MenuItem m_item = null;
            EventHandler m_handler = null;

            public MenuItemEventHandler(ICommand command, MenuItem item)   
            {
                //Set the private variables
                m_command = command;
                m_item = item;
                //Couple the handler
                m_handler = new EventHandler(itemClicked);
                m_item.Click += m_handler;
            }

            void itemClicked(object sender, EventArgs e)
            {
                m_command.Execute();
            }

            ~MenuItemEventHandler()
            {
                m_item.Click -= m_handler;
            }
        }
    }
}
