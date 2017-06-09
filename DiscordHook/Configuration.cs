using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using System.Xml.Serialization;

namespace DiscordHook
{
    public class Configuration : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "Bot")]
        public List<ServerSetting> Bots;

        public void LoadDefaults()
        {
            Bots = new List<ServerSetting>()
            {
                ServerSetting.Create()
            };
        }
    }
}
