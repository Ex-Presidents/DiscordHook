using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using System.Xml.Serialization;
using SDG.Unturned;

namespace DiscordHook
{
    public class Configuration : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "Bot")]
        public List<ServerSetting> Bots;
        public string ServerName;

        public void LoadDefaults()
        {
            Bots = new List<ServerSetting>()
            {
                ServerSetting.Create()
            };
            ServerName = Provider.serverName;
        }
    }
}
