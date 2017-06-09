using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Logger = Rocket.Core.Logging.Logger;

namespace DiscordHook
{
    public static class Sender
    {
        public static JObject GenerateWebhook(string message, string username = null, string avatarURL = null, bool tts = false)
        {
            JObject obj = new JObject();

            obj.Add("content", message);
            if (!string.IsNullOrEmpty(username))
                obj.Add("username", username);
            if (!string.IsNullOrEmpty(avatarURL))
                obj.Add("avatar_url", avatarURL);
            obj.Add("tts", tts);

            return obj;
        }

        public static bool SendSingle(JObject WebHook, ServerSetting bot)
        {
            try
            {
                using(WebClient weebClient = new WebClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback = (o, certificate, chain, errors) => true;
                    weebClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    weebClient.UploadStringAsync(new Uri(bot.URL), WebHook.ToString(Formatting.None));
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool[] Send(JObject WebHook)
        {
            bool[] Returns = new bool[DiscordHook.Instance.Configuration.Instance.Bots.Count];

            for (int i = 0; i < Returns.Length; i++)
                Returns[i] = SendSingle(WebHook, DiscordHook.Instance.Configuration.Instance.Bots[i]);

            return Returns;
        }
    }
}
