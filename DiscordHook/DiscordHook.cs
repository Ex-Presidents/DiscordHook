using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned;
using Logger = Rocket.Core.Logging.Logger;
using UnityEngine;

namespace DiscordHook
{
    public class DiscordHook : RocketPlugin<Configuration>
    {
        #region Variables
        private bool Voting = false;
        private int VotesYes = 0;
        private int VotesNo = 0;
        #endregion

        #region Fields
        private FieldInfo fIsVoting;
        private FieldInfo fVotesYes;
        private FieldInfo fVotesNo;
        private FieldInfo fVotesNeeded;
        private FieldInfo fVoteOrigin;
        private FieldInfo fVoteTarget;
        #endregion

        #region Properties
        public static DiscordHook Instance { get; private set; }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "server_status_title", "Server Status Update" },
            { "server_status_start", "Server Started!" },
            { "server_status_stop", "Server Shutdown!" },

            { "player_status_title", "Player Status Update" },
            { "player_status_join", "Player Joined!" },
            { "player_status_leave", "Player Left!" },
            { "player_status_count", "Players Online: {0}" },

            { "player_chat", "Chat" },
            { "player_chat_global", "Global Chat" },
            { "player_chat_local", "Local Chat" },
            { "player_chat_group", "Group Chat" },

            { "player_command_title", "Execute Command" },

            { "player_nick", "Nickname: {0}" },

            { "player_death_title", "Player Death Update" },
            { "player_death", "Died of unknown causes" },
            { "player_death_bleeding", "Died from bleeding" },
            { "player_death_gun", "Died from getting shot" },
            { "player_death_melee", "Died from getting beat" },
            { "player_death_punched", "Died from getting punched" },
            { "player_death_roadkill", "Died from being ran over" },
            { "player_death_granade", "Died from granade" },
            { "player_death_shred", "Died in pieces" },
            { "player_death_landmine", "Died from not watching his step" },
            { "player_death_missile", "Died from being shot by a missile" },
            { "player_death_charge", "Died from getting too close to a charge" },
            { "player_death_splash", "Splat and he is dead" },
            { "player_death_killer", "Killer" },
            { "player_death_killername", "Steam Name: {0}" },
            { "player_death_killernick", "Nick Name: {0}" },
            { "player_death_killer64", "Steam64: {0}" },

            { "vote_status_title", "Vote Status Update" },
            { "vote_status_data", "Vote Data" },
            { "vote_status_start", "Voting Started!" },
            { "vote_status_fail", "Voting Failed!" },
            { "vote_status_success", "Voting Succeeded!" },
            { "vote_status_needed", "Votes Needed: {0}" },
            { "vote_status_yes", "Votes YES: {0}" },
            { "vote_status_no", "Votes NO: {0}" },
            { "vote_status_origin", "Voter: {0}" },
            { "vote_status_target", "Voted: {0}" },
            { "vote_status_votedno", "Player voted NO!" },
            { "vote_status_votedyes", "Player voted YES!" },
            { "vote_status_update", "Voting Information" }
        };
        #endregion

        protected override void Load()
        {
            Instance = this;

            fIsVoting = typeof(ChatManager).GetField("isVoting", BindingFlags.NonPublic | BindingFlags.Static);
            fVoteOrigin = typeof(ChatManager).GetField("voteOrigin", BindingFlags.NonPublic | BindingFlags.Static);
            fVotesNeeded = typeof(ChatManager).GetField("votesNeeded", BindingFlags.NonPublic | BindingFlags.Static);
            fVotesNo = typeof(ChatManager).GetField("voteNo", BindingFlags.NonPublic | BindingFlags.Static);
            fVotesYes = typeof(ChatManager).GetField("voteYes", BindingFlags.NonPublic | BindingFlags.Static);
            fVoteTarget = typeof(ChatManager).GetField("voteTarget", BindingFlags.NonPublic | BindingFlags.Static);

            Provider.onEnemyConnected += new Provider.EnemyConnected(OnPlayerJoin);
            Provider.onEnemyDisconnected += new Provider.EnemyDisconnected(OnPlayerLeave);
            Provider.onServerShutdown += new Provider.ServerShutdown(OnShutdown);
            ChatManager.onChatted += new Chatted(OnPlayerChat);

            foreach(ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendLoadShutdown)
                    Sender.SendSingle(Messages.Generate_ServerStatus(Translations.Instance["server_status_start"], bot), bot);
        }

        #region Mono Functions
        void Update()
        {
            if (!ChatManager.voteAllowed)
                return;
            bool isVoting = (bool)fIsVoting.GetValue(null);
            byte votesYes = (byte)fVotesYes.GetValue(null);
            byte votesNo = (byte)fVotesNo.GetValue(null);
            byte votesNeeded = (byte)fVotesNeeded.GetValue(null);
            CSteamID voteOrigin = (CSteamID)fVoteOrigin.GetValue(null);
            CSteamID voteTarget = (CSteamID)fVoteTarget.GetValue(null);

            if(!isVoting && isVoting == Voting)
            {
                Voting = false;
                VotesYes = 0;
                VotesNo = 0;
                return;
            }

            foreach (ServerSetting bot in Configuration.Instance.Bots)
            {
                if (bot.SendVoting)
                {
                    if (isVoting != Voting)
                    {
                        if (isVoting)
                        {
                            Sender.SendSingle(Messages.Generate_VoteStatus(Translations.Instance["vote_status_start"], false, 0, 0, 0, "", "", bot), bot);
                            Sender.SendSingle(Messages.Generate_VoteStatus(Translations.Instance["vote_status_update"], true, votesYes, votesNo, votesNeeded, Provider.clients.First(a => a.playerID.steamID == voteOrigin).playerID.playerName, Provider.clients.First(a => a.playerID.steamID == voteTarget).playerID.playerName, bot), bot);
                        }
                        else
                        {
                            if (votesYes >= votesNeeded)
                                Sender.SendSingle(Messages.Generate_VoteStatus(Translations.Instance["vote_status_success"], false, 0, 0, 0, "", "", bot), bot);
                            else
                                Sender.SendSingle(Messages.Generate_VoteStatus(Translations.Instance["vote_status_fail"], false, 0, 0, 0, "", "", bot), bot);
                        }
                    }
                    if(votesYes != VotesYes)
                        Sender.SendSingle(Messages.Generate_VoteStatus(Translations.Instance["vote_status_votedyes"], true, votesYes, votesNo, votesNeeded, Provider.clients.First(a => a.playerID.steamID == voteOrigin).playerID.playerName, Provider.clients.First(a => a.playerID.steamID == voteTarget).playerID.playerName, bot), bot);
                    if(votesNo != VotesNo)
                        Sender.SendSingle(Messages.Generate_VoteStatus(Translations.Instance["vote_status_votedno"], true, votesYes, votesNo, votesNeeded, Provider.clients.First(a => a.playerID.steamID == voteOrigin).playerID.playerName, Provider.clients.First(a => a.playerID.steamID == voteTarget).playerID.playerName, bot), bot);
                }
            }
            Voting = isVoting;
            VotesYes = votesYes;
            VotesNo = votesNo;
        }
        #endregion

        #region Event Functions
        private void OnPlayerDeath(Player player, byte damage, Vector3 force, EDeathCause death, ELimb limb, CSteamID killer)
        {
            if (!player.life.isDead)
                return;
            if(killer == null || killer == CSteamID.Nil)
                return;
            string cause;

            if (death == EDeathCause.BLEEDING)
                cause = Translations.Instance["player_death_bleeding"];
            else if (death == EDeathCause.CHARGE)
                cause = Translations.Instance["player_death_charge"];
            else if (death == EDeathCause.GRENADE)
                cause = Translations.Instance["player_death_granade"];
            else if (death == EDeathCause.GUN)
                cause = Translations.Instance["player_death_gun"];
            else if (death == EDeathCause.LANDMINE)
                cause = Translations.Instance["player_death_landmine"];
            else if (death == EDeathCause.MELEE)
                cause = Translations.Instance["player_death_melee"];
            else if (death == EDeathCause.MISSILE)
                cause = Translations.Instance["player_death_missile"];
            else if (death == EDeathCause.PUNCH)
                cause = Translations.Instance["player_death_punched"];
            else if (death == EDeathCause.ROADKILL)
                cause = Translations.Instance["player_death_roadkill"];
            else if (death == EDeathCause.SHRED)
                cause = Translations.Instance["player_death_shred"];
            else if (death == EDeathCause.SPLASH)
                cause = Translations.Instance["player_death_splash"];
            else
                cause = Translations.Instance["player_death"];

            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendDeaths)
                    Sender.SendSingle(Messages.Generate_Death(cause, player.channel.owner, Provider.clients.FirstOrDefault(a => a.playerID.steamID == killer), bot), bot);
        }

        private void OnShutdown()
        {
            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendLoadShutdown)
                    Sender.SendSingle(Messages.Generate_ServerStatus(Translations.Instance["server_status_stop"], bot), bot);
        }

        private void OnPlayerJoin(SteamPlayer player)
        {
            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendJoinLeave)
                    Sender.SendSingle(Messages.Generate_PlayerStatus(Translations.Instance["player_status_join"], player, bot), bot);
            player.player.life.onHurt += new Hurt(OnPlayerDeath);
        }

        private void OnPlayerLeave(SteamPlayer player)
        {
            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendJoinLeave)
                    Sender.SendSingle(Messages.Generate_PlayerStatus(Translations.Instance["player_status_leave"], player, bot), bot);
        }

        private void OnPlayerChat(SteamPlayer player, EChatMode mode, ref Color color, string text, ref bool visible)
        {
            if(text.StartsWith("/") || text.StartsWith("@"))
            {
                foreach (ServerSetting bot in Configuration.Instance.Bots)
                    if (bot.SendCommands)
                        Sender.SendSingle(Messages.Generate_Command(text, player, bot), bot);
                return;
            }
            string title;

            if (mode == EChatMode.GLOBAL)
                title = Translations.Instance["player_chat_global"];
            else if (mode == EChatMode.GROUP)
                title = Translations.Instance["player_chat_group"];
            else if (mode == EChatMode.LOCAL)
                title = Translations.Instance["player_chat_local"];
            else
                title = Translations.Instance["player_chat"];

            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendChatMessages)
                    Sender.SendSingle(Messages.Generate_Chat(text, title, player, bot), bot);
        }
        #endregion
    }
}
