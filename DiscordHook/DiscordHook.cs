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
using Rocket.Core.Commands;
using Rocket.Core;
using Rocket.API;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Permissions;

namespace DiscordHook
{
    public class DiscordHook : RocketPlugin<Configuration>
    {
        #region Variables
        private bool Voting = false;
        private int VotesYes = 0;
        private int VotesNo = 0;

        private Dictionary<SteamPlayer, Hurt> ShitCollecter = new Dictionary<SteamPlayer, Hurt>();
        private Dictionary<SteamPlayer, DateTime> NoTP = new Dictionary<SteamPlayer, DateTime>();
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

        public static int Players { get; private set; } = 0;

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "server_status_title", "Server Status Update" },
            { "server_status_start", "Server Started!" },
            { "server_status_stop", "Server Shutdown!" },
            { "server_status_arena_start", "Arena Match Started!" },
            { "server_status_arena_stop", "Arena Match Ended!" },

            { "player_status_title", "Player Status Update" },
            { "player_status_join", "Player Joined!" },
            { "player_status_leave", "Player Left!" },
            { "player_status_count", "Players Online: {0}" },

            { "player_chat", "Chat" },
            { "player_chat_global", "Global Chat" },
            { "player_chat_local", "Local Chat" },
            { "player_chat_group", "Group Chat" },

            { "player_command_title", "Execute Command" },
            { "admin_command_title", "Execute Admin Command" },

            { "player_nick", "Nickname: {0}" },

            { "abuse_title", "Potential Admin Abuse"},
            { "abuse_vanish", "Killed a player while in vanish" },
            { "abuse_godmode", "Killed a player while in god mode" },
            { "abuse_teleport", "Killed a player after teleporting to them" },
            { "abuse_kill_victim", "Victim" },
            { "abuse_kill_victimname", "Steam Name: {0}" },
            { "abuse_kill_victimnick", "Nick Name: {0}" },
            { "abuse_kill_victim64", "Steam64: {0}" },

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
            { "vote_status_update", "Voting Information" },
        };
        #endregion

        protected override void Load()
        {
            if (Instance != null)
                return;
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
            LevelManager.onArenaMessageUpdated += new ArenaMessageUpdated(OnArenaMessage);
            R.Commands.OnExecuteCommand += new RocketCommandManager.ExecuteCommand(OnExecuteCommand);

            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendLoadShutdown)
                    Sender.SendSingle(Messages.Generate_ServerStatus(Translations.Instance["server_status_start"], bot), bot);
        }

        #region Mono Functions
        void Update()
        {
            List<SteamPlayer> remove = new List<SteamPlayer>();
            foreach (SteamPlayer player in NoTP.Keys)
                if ((DateTime.Now - NoTP[player]).TotalSeconds >= Configuration.Instance.AbuseTeleportKillTimeoutSeconds)
                    remove.Add(player);
            while(remove.Count > 0)
            {
                NoTP.Remove(remove[0]);
                remove.Remove(remove[0]);
            }

            if (!ChatManager.voteAllowed)
                return;
            bool isVoting = (bool)fIsVoting.GetValue(null);
            byte votesYes = (byte)fVotesYes.GetValue(null);
            byte votesNo = (byte)fVotesNo.GetValue(null);
            byte votesNeeded = (byte)fVotesNeeded.GetValue(null);
            CSteamID voteOrigin = (CSteamID)fVoteOrigin.GetValue(null);
            CSteamID voteTarget = (CSteamID)fVoteTarget.GetValue(null);

            if (!isVoting && isVoting == Voting)
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
                    if (votesYes != VotesYes)
                        Sender.SendSingle(Messages.Generate_VoteStatus(Translations.Instance["vote_status_votedyes"], true, votesYes, votesNo, votesNeeded, Provider.clients.First(a => a.playerID.steamID == voteOrigin).playerID.playerName, Provider.clients.First(a => a.playerID.steamID == voteTarget).playerID.playerName, bot), bot);
                    if (votesNo != VotesNo)
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
            if (player == null)
                return;
            if (player.life == null)
                return;
            if (!player.life.isDead)
                return;
            if (killer == null || killer == CSteamID.Nil)
                return;
            UnturnedPlayer Killer = UnturnedPlayer.FromCSteamID(killer);
            string cause;

            if (Killer.VanishMode)
            {
                for (int i = 0; i < Instance.Configuration.Instance.Bots.Count; i++)
                    if (Instance.Configuration.Instance.Bots[i].SendPotentialAbuse)
                        Sender.SendSingle(Messages.Generate_AbuseKill(Translations.Instance["abuse_vanish"], UnturnedPlayer.FromCSteamID(killer).SteamPlayer(), player.channel.owner, Instance.Configuration.Instance.Bots[i]), Instance.Configuration.Instance.Bots[i]);
            }
            else if (Killer.GodMode)
            {
                for (int i = 0; i < Instance.Configuration.Instance.Bots.Count; i++)
                    if (Instance.Configuration.Instance.Bots[i].SendPotentialAbuse)
                        Sender.SendSingle(Messages.Generate_AbuseKill(Translations.Instance["abuse_godmode"], UnturnedPlayer.FromCSteamID(killer).SteamPlayer(), player.channel.owner, Instance.Configuration.Instance.Bots[i]), Instance.Configuration.Instance.Bots[i]);
            }
            else if (NoTP.ContainsKey(Killer.SteamPlayer()))
            {
                for (int i = 0; i < Instance.Configuration.Instance.Bots.Count; i++)
                    if (Instance.Configuration.Instance.Bots[i].SendPotentialAbuse)
                        Sender.SendSingle(Messages.Generate_AbuseKill(Translations.Instance["abuse_teleport"], UnturnedPlayer.FromCSteamID(killer).SteamPlayer(), player.channel.owner, Instance.Configuration.Instance.Bots[i]), Instance.Configuration.Instance.Bots[i]);
                NoTP.Remove(Killer.SteamPlayer());
            }

            switch (death)
            {
                case EDeathCause.BLEEDING:
                    cause = Translations.Instance["player_death_bleeding"];
                    break;
                case EDeathCause.CHARGE:
                    cause = Translations.Instance["player_death_charge"];
                    break;
                case EDeathCause.GRENADE:
                    cause = Translations.Instance["player_death_granade"];
                    break;
                case EDeathCause.GUN:
                    cause = Translations.Instance["player_death_gun"];
                    break;
                case EDeathCause.LANDMINE:
                    cause = Translations.Instance["player_death_landmine"];
                    break;
                case EDeathCause.MELEE:
                    cause = Translations.Instance["player_death_melee"];
                    break;
                case EDeathCause.MISSILE:
                    cause = Translations.Instance["player_death_missile"];
                    break;
                case EDeathCause.PUNCH:
                    cause = Translations.Instance["player_death_punched"];
                    break;
                case EDeathCause.ROADKILL:
                    cause = Translations.Instance["player_death_roadkill"];
                    break;
                case EDeathCause.SHRED:
                    cause = Translations.Instance["player_death_shred"];
                    break;
                case EDeathCause.SPLASH:
                    cause = Translations.Instance["player_death_splash"];
                    break;
                default:
                    cause = Translations.Instance["player_death"];
                    break;
            }

            for (int i = 0; i < Instance.Configuration.Instance.Bots.Count; i++)
                if (Instance.Configuration.Instance.Bots[i].SendDeaths)
                    Sender.SendSingle(Messages.Generate_Death(cause, player.channel.owner, UnturnedPlayer.FromCSteamID(killer).SteamPlayer(), Instance.Configuration.Instance.Bots[i]), Instance.Configuration.Instance.Bots[i]);
        }

        private void OnShutdown()
        {
            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendLoadShutdown)
                    Sender.SendSingle(Messages.Generate_ServerStatus(Translations.Instance["server_status_stop"], bot), bot);
        }

        private void OnPlayerJoin(SteamPlayer player)
        {
            ShitCollecter.Add(player, new Hurt(OnPlayerDeath));

            Players++;
            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendJoinLeave)
                    Sender.SendSingle(Messages.Generate_PlayerStatus(Translations.Instance["player_status_join"], player, bot), bot);
            player.player.life.onHurt += ShitCollecter[player];
        }

        private void OnPlayerLeave(SteamPlayer player)
        {
            if (!ShitCollecter.ContainsKey(player))
                return;
            player.player.life.onHurt -= ShitCollecter[player];
            ShitCollecter.Remove(player);

            Players--;
            foreach (ServerSetting bot in Configuration.Instance.Bots)
                if (bot.SendJoinLeave)
                    Sender.SendSingle(Messages.Generate_PlayerStatus(Translations.Instance["player_status_leave"], player, bot), bot);
        }

        private void OnPlayerChat(SteamPlayer player, EChatMode mode, ref Color color, string text, ref bool visible)
        {
            if (text.StartsWith("/") || text.StartsWith("@"))
            {
                if (UnturnedPlayer.FromSteamPlayer(player).HasPermission("discordhook.nocommandlog"))
                    return;
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
                if ((mode == EChatMode.GLOBAL && bot.SendGlobalMessages) || (mode == EChatMode.GROUP && bot.SendGroupMessages) || (mode == EChatMode.LOCAL && bot.SendLocalMessages))
                    Sender.SendSingle(Messages.Generate_Chat(text, title, player, bot), bot);
        }

        private void OnExecuteCommand(IRocketPlayer player, IRocketCommand command, ref bool cancel)
        {
            UnturnedPlayer ply = (UnturnedPlayer)player;

            if(command is CommandTp)
                NoTP.Add(ply.SteamPlayer(), DateTime.Now);
        }

        private void OnArenaMessage(EArenaMessage message)
        {
            if(message == EArenaMessage.PLAY)
            {
                foreach (ServerSetting bot in Configuration.Instance.Bots)
                    if (bot.SendArenaUpdates)
                        Sender.SendSingle(Messages.Generate_ServerStatus(Translations.Instance["server_status_arena_start"], bot), bot);
            }
            else if(message == EArenaMessage.WIN || message == EArenaMessage.LOSE)
            {
                foreach (ServerSetting bot in Configuration.Instance.Bots)
                    if (bot.SendArenaUpdates)
                        Sender.SendSingle(Messages.Generate_ServerStatus(Translations.Instance["server_status_arena_stop"], bot), bot);
            }
        }
        #endregion
    }
}
