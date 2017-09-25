using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulator.Interfaces;
using System.Xml.Serialization;
using SDG.Unturned;

namespace DiscordHook
{
    public class Configuration : IConfiguration
    {
        [XmlArrayItem(ElementName = "Bot")]
        public List<ServerSetting> Bots;
        public string ServerName;
        public int AbuseTeleportKillTimeoutSeconds;

        public void LoadDefaults()
        {
            Bots = new List<ServerSetting>()
            {
                ServerSetting.Create()
            };
            ServerName = Provider.serverName;
            AbuseTeleportKillTimeoutSeconds = 5;
        }
    }
}
