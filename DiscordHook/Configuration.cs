using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace DiscordHook
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string URL;
        public bool SendChatMessages;
        public bool SendJoinLeave;
        public bool SendVoting;
        public bool SendLoadShutdown;
        public bool LinkSenderProfile;
        public string ServerLink;
        public bool ShowPlayerCount;
        public bool ShowCommands;

        public void LoadDefaults()
        {
            URL = "ENTER URL HERE";

            SendChatMessages = true;
            SendJoinLeave = true;
            SendVoting = true;
            SendLoadShutdown = true;
            LinkSenderProfile = true;
            ServerLink = "google.com";
            ShowPlayerCount = true;
            ShowCommands = true;
        }
    }
}
