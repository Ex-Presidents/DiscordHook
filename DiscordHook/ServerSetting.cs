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
        public bool SendArenaUpdates;
        public bool SendVoting;
        public bool SendLoadShutdown;
        public bool SendCommands;
        public bool SendPotentialAbuse;
        public bool SendDeaths;
        public bool LinkSenderProfile;
        public bool LinkServer;
        public string ServerLink;
        public bool ShowPlayerCount;
        public bool AbuseTeleport;
        public string ColorChat;
        public string ColorJoinLeave;
        public string ColorVoting;
        public string ColorServerStatus;
        public string ColorCommands;
        public string ColorDeath;
        public string ColorAbuse;

        public static ServerSetting Create()
        {
            ServerSetting ss = new ServerSetting();

            ss.URL = "https://www.discordapp.com/";

            ss.SendGlobalMessages = true;
            ss.SendLocalMessages = false;
            ss.SendGroupMessages = false;
            ss.SendJoinLeave = true;
            ss.SendArenaUpdates = true;
            ss.SendVoting = true;
            ss.SendLoadShutdown = true;
            ss.SendCommands = false;
            ss.SendPotentialAbuse = true;
            ss.SendDeaths = true;
            ss.LinkSenderProfile = true;
            ss.LinkServer = false;
            ss.ServerLink = "https://www.google.com/";
            ss.ShowPlayerCount = true;
            ss.AbuseTeleport = true;

            ss.ColorChat = "FFFF00";
            ss.ColorJoinLeave = "00FF00";
            ss.ColorVoting = "FF7700";
            ss.ColorServerStatus = "00FFFF";
            ss.ColorCommands = "0000FF";
            ss.ColorDeath = "BD00FF";
            ss.ColorAbuse = "FF0000";

            return ss;
        }
    }
}
