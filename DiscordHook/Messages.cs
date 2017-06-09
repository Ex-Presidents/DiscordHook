using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDG.Unturned;
using Rocket.Unturned.Player;

namespace DiscordHook
{
    public static class Messages
    {
        public static JObject Generate_ServerStatus(string message, ServerSetting bot)
        {
            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();

            objAuthor.Add("name", Provider.serverName);
            if (bot.LinkServer)
                objAuthor.Add("url", bot.ServerLink);

            objEmbed.Add("title", DiscordHook.Instance.Translations.Instance["server_status_title"]);
            objEmbed.Add("description", message);
            objEmbed.Add("color", int.Parse(bot.ColorLoadShutdown, NumberStyles.HexNumber));
            objEmbed.Add("author", objAuthor);
            arrEmbeds.Add(objEmbed);

            obj.Add("username", Provider.serverName);
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            return obj;
        }

        public static JObject Generate_PlayerStatus(string message, SteamPlayer player, ServerSetting bot)
        {
            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();
            JObject objFooter = new JObject();

            objAuthor.Add("name", player.playerID.playerName);
            if (bot.LinkSenderProfile)
                objAuthor.Add("url", "http://steamcommunity.com/profiles/" + player.playerID.steamID.ToString());
            objAuthor.Add("icon_url", UnturnedPlayer.FromSteamPlayer(player).SteamProfile.AvatarFull.AbsoluteUri);

            objFooter.Add("text", string.Format(DiscordHook.Instance.Translations.Instance["player_status_count"], Provider.clients.Count(a => a.player != null)));

            objEmbed.Add("title", DiscordHook.Instance.Translations.Instance["player_status_title"]);
            objEmbed.Add("description", message);
            objEmbed.Add("color", int.Parse(bot.ColorJoinLeave, NumberStyles.HexNumber));
            if (bot.ShowPlayerCount)
                objEmbed.Add("footer", objFooter);
            objEmbed.Add("author", objAuthor);
            arrEmbeds.Add(objEmbed);

            obj.Add("username", Provider.serverName);
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            return obj;
        }

        public static JObject Generate_VoteStatus(string message, bool fld, int yes, int no, int needed, string voter, string voted, ServerSetting bot)
        {
            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();
            JArray arrFields = new JArray();
            JObject objData = new JObject();

            objAuthor.Add("name", Provider.serverName);
            if (bot.LinkServer)
                objAuthor.Add("url", bot.ServerLink);

            objData.Add("name", DiscordHook.Instance.Translations.Instance["vote_status_data"]);
            objData.Add("value", string.Format(DiscordHook.Instance.Translations.Instance["vote_status_yes"], yes) + "\n" +
                string.Format(DiscordHook.Instance.Translations.Instance["vote_status_no"], no) + "\n" +
                string.Format(DiscordHook.Instance.Translations.Instance["vote_status_needed"], needed) + "\n" + 
                string.Format(DiscordHook.Instance.Translations.Instance["vote_status_origin"], voter) + "\n" +
                string.Format(DiscordHook.Instance.Translations.Instance["vote_status_target"], voted));
            objData.Add("inline", true);
            arrFields.Add(objData);

            objEmbed.Add("title", DiscordHook.Instance.Translations.Instance["vote_status_title"]);
            objEmbed.Add("description", message);
            objEmbed.Add("color", int.Parse(bot.ColorVoting, NumberStyles.HexNumber));
            objEmbed.Add("author", objAuthor);
            if (fld)
                objEmbed.Add("fields", arrFields);
            arrEmbeds.Add(objEmbed);

            obj.Add("username", Provider.serverName);
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            return obj;
        }

        public static JObject Generate_Command(string message, SteamPlayer player, ServerSetting bot)
        {
            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();
            JObject objFooter = new JObject();

            objAuthor.Add("name", player.playerID.playerName);
            if (bot.LinkSenderProfile)
                objAuthor.Add("url", "http://steamcommunity.com/profiles/" + player.playerID.steamID.ToString());
            objAuthor.Add("icon_url", UnturnedPlayer.FromSteamPlayer(player).SteamProfile.AvatarFull.AbsoluteUri);

            objFooter.Add("text", string.Format(DiscordHook.Instance.Translations.Instance["player_nick"], player.playerID.nickName));

            objEmbed.Add("title", DiscordHook.Instance.Translations.Instance["player_command_title"]);
            objEmbed.Add("description", message);
            objEmbed.Add("color", int.Parse(bot.ColorCommands, NumberStyles.HexNumber));
            objEmbed.Add("footer", objFooter);
            objEmbed.Add("author", objAuthor);
            arrEmbeds.Add(objEmbed);

            obj.Add("username", Provider.serverName);
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            return obj;
        }

        public static JObject Generate_Chat(string message, string title, SteamPlayer player, ServerSetting bot)
        {
            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();
            JObject objFooter = new JObject();

            objAuthor.Add("name", player.playerID.playerName);
            if (bot.LinkSenderProfile)
                objAuthor.Add("url", "http://steamcommunity.com/profiles/" + player.playerID.steamID.ToString());
            objAuthor.Add("icon_url", UnturnedPlayer.FromSteamPlayer(player).SteamProfile.AvatarFull.AbsoluteUri);

            objFooter.Add("text", string.Format(DiscordHook.Instance.Translations.Instance["player_nick"], player.playerID.nickName));

            objEmbed.Add("title", title);
            objEmbed.Add("description", message);
            objEmbed.Add("color", int.Parse(bot.ColorChat, NumberStyles.HexNumber));
            objEmbed.Add("footer", objFooter);
            objEmbed.Add("author", objAuthor);
            arrEmbeds.Add(objEmbed);

            obj.Add("username", Provider.serverName);
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            return obj;
        }

        public static JObject Generate_Death(string message, SteamPlayer victim, SteamPlayer killer, ServerSetting bot)
        {
            if (killer == null)
                return null;

            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();
            JArray arrFields = new JArray();
            JObject objKiller = new JObject();

            objAuthor.Add("name", victim.playerID.playerName);
            if (bot.LinkSenderProfile)
                objAuthor.Add("url", "http://steamcommunity.com/profiles/" + victim.playerID.steamID.ToString());
            objAuthor.Add("icon_url", UnturnedPlayer.FromSteamPlayer(victim).SteamProfile.AvatarFull.AbsoluteUri);

            objKiller.Add("name", DiscordHook.Instance.Translations.Instance["player_death_killer"]);
            objKiller.Add("value", string.Format(DiscordHook.Instance.Translations.Instance["player_death_killername"], killer.playerID.playerName) + "\n" +
                string.Format(DiscordHook.Instance.Translations.Instance["player_death_killernick"], killer.playerID.nickName) + "\n" +
                string.Format(DiscordHook.Instance.Translations.Instance["player_death_killer64"], killer.playerID.steamID));
            objKiller.Add("inline", true);
            arrFields.Add(objKiller);

            objEmbed.Add("title", DiscordHook.Instance.Translations.Instance["player_death_title"]);
            objEmbed.Add("description", message);
            objEmbed.Add("color", int.Parse(bot.ColorDeath, NumberStyles.HexNumber));
            objEmbed.Add("author", objAuthor);
            objEmbed.Add("fields", arrFields);
            arrEmbeds.Add(objEmbed);

            obj.Add("username", Provider.serverName);
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            return obj;
        }
    }
}
