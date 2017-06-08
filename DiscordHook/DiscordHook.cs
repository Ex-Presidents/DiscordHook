using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace DiscordHook
{
    public class DiscordHook : RocketPlugin<Configuration>
    {
        public static DiscordHook Instance { get; private set; }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "server_start", "{0} has been started!" },
            { "server_stop", "{0} has been shutdown!" }
        };

        protected override void Load()
        {
            Instance = this;

            Provider.onServerShutdown += new Provider.ServerShutdown(OnServerShutdown);

            if (Configuration.Instance.SendLoadShutdown)
                Sender.Send(Sender.GenerateWebhook(string.Format(Translations.Instance["server_start"], Provider.serverName)));
        }

        private void OnServerShutdown()
        {
            if (Configuration.Instance.SendLoadShutdown)
                Sender.Send(Sender.GenerateWebhook(string.Format(Translations.Instance["server_stop"], Provider.serverName)));
        }


    }
}
