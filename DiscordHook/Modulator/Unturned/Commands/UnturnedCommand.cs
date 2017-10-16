using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDG.Unturned;

namespace Modulator.Unturned.Commands
{
    public abstract class UnturnedCommand
    {
        #region Abstract Properties
        public abstract string Command { get; }

        public abstract string Help { get; }
        public abstract string Info { get; }
        #endregion

        #region Abstract Functions
        public abstract void OnExecute(SteamPlayer player, string param);
        #endregion

        #region Override Functions
        public int CompareTo(UnturnedCommand command) => Command.CompareTo(command.Command);
        #endregion
    }
}
