using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordHook
{
    public class ServerSetting
    {
        public string URL;
        public bool SendGlobalMessages;
        public bool SendLocalMessages;
        public bool SendGroupMessages;
        public bool SendJoinLeave;
        public bool SendVoting;
        public bool SendLoadShutdown;
        public bool SendCommands;
        public bool SendDeaths;
        public bool LinkSenderProfile;
        public bool LinkServer;
        public string ServerLink;
        public bool ShowPlayerCount;
        public bool ShowPotentialAbuse;
        public string ColorChat;
        public string ColorJoinLeave;
        public string ColorVoting;
        public string ColorLoadShutdown;
        public string ColorCommands;
        public string ColorDeath;

        public static ServerSetting Create()
        {
            ServerSetting ss = new ServerSetting();

            ss.URL = "https://www.discordapp.com/";

            ss.ShowPotentialAbuse = true;
            ss.SendGlobalMessages = true;
            ss.SendLocalMessages = false;
            ss.SendGroupMessages = false;
            ss.SendJoinLeave = true;
            ss.SendVoting = true;
            ss.SendLoadShutdown = true;
            ss.SendCommands = false;
            ss.SendDeaths = true;
            ss.LinkSenderProfile = true;
            ss.LinkServer = false;
            ss.ServerLink = "https://www.google.com/";
            ss.ShowPlayerCount = true;

            ss.ColorChat = "FFFF00";
            ss.ColorJoinLeave = "00FF00";
            ss.ColorVoting = "FF0000";
            ss.ColorLoadShutdown = "00FFFF";
            ss.ColorCommands = "0000FF";
            ss.ColorDeath = "BD00FF";

            return ss;
        }
    }
}
