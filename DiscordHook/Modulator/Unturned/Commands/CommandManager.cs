using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDG.Unturned;

namespace Modulator.Unturned.Commands
{
    public static class CommandManager
    {
        #region Variables
        private static List<UnturnedCommand> _Commands = new List<UnturnedCommand>();
        #endregion

        #region Handlers
        public delegate void CommandExecuteHandler(SteamPlayer player, UnturnedCommand command, ref bool cancel);
        #endregion

        #region Events
        public static event CommandExecuteHandler OnCommandExecute;
        #endregion

        static CommandManager()
        {
            ChatManager.onCheckPermissions += CheckPerms;
        }

        #region Functions
        public static void Register(UnturnedCommand command)
        {
            if (_Commands.Contains(command))
                return;

            _Commands.Add(command);
        }
        public static T Register<T>() where T : UnturnedCommand
        {
            T cmd = (T)_Commands.FirstOrDefault(a => a.GetType() == typeof(T));

            if (cmd != null)
                return cmd;
            cmd = Activator.CreateInstance<T>();

            Register(cmd);
            return cmd;
        }

        public static void Deregister(UnturnedCommand command)
        {
            if (!_Commands.Contains(command))
                return;

            _Commands.Remove(command);
        }
        public static void Deregister<T>() where T : UnturnedCommand
        {
            T cmd = (T)_Commands.FirstOrDefault(a => a.GetType() == typeof(T));

            if (cmd == null)
                return;
            Deregister(cmd);
        }

        public static string[] SplitCommand(string command)
        {
            if (command.Substring(0, 1) == "@" || command.Substring(0, 1) == "/")
                command = command.Substring(1);
        }
        #endregion

        #region Event Functions
        static void CheckPerms(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            
        }
        #endregion
    }
}
