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
        private static WebClient weebclient;

        static Sender()
        {
            weebclient = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback = (o, certificate, chain, errors) => true;
            weebclient.Headers[HttpRequestHeader.ContentType] = "application/json";
        }

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

        public static bool Send(JObject WebHook)
        {
            try
            {
                string returnData = weebclient.UploadString(DiscordHook.Instance.Configuration.Instance.URL, WebHook.ToString(Formatting.None));

                return string.IsNullOrEmpty(returnData);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
