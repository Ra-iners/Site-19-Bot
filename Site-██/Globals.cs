using Discord.Commands;
using Discord.Webhook;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using Site___.Storage;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Site___
{
    public static class Globals
    {
        public static DiscordShardedClient Client = null;
        public static CommandService Commands = null;
        public static IServiceProvider Services = null;
        public static string Prefix = (string)Configuration.Get("Prefix");
        public static Dictionary<string, string> Rules = new Dictionary<string, string>()
        {
            ["A1"] = "Respect all users of the server, regardless of how much you like them or not. Treat others the way you want to be treated.",
            ["A2"] = "Do not send a lot of small messages or images right after each other, do not disrupt the chat by spamming.",
            ["A3"] = "Do not send any content that can be deemed NSFW, this includes cropped images of said material.",
            ["A4"] = "You are not allowed to advertise anywhere on this server unless a channel is meant for it. including advertising in server members' direct messages. This also includes posting something in <#709012601639796778> and then going to channels like #advertising and saying stuff like \"Hey guys check the server I posted in #advertising\", you will get the same punishment as if you were advertising in any other channel if you do this.",
            ["A5"] = "If a staff member asks you to stop something, then stop, or more severe action may be taken against you.",
            ["A6"] = "Insults, bigotry of any kind (ex: homophobia, transphobia, racism, sexism) are not allowed.",
            ["A7"] = "Do not bring topics like politics and religion into the server.",
            ["A7-A"] = "Symbols like swastikas and communist ones (hammer & sickle) are not allowed, and will get you punished, this includes memes that support them",
            ["A8"] = "Alterative account miss-use to evade punishment.",
            ["A9"] = "You are not allowed to impersonate any staff member on the server.",
            ["A10"] = "This is because we are not able to moderate any other languages than English, however, common words as Konichiwa and Déjà vu & similar, are allowed.",

            ["B1"] = "Inappropriate Avatars/Usernames/Nicknames are not allowed on the server, please change them.",
            ["B2"] = "Pinging several users without their permission/request.",
            ["B3"] = "You must follow the Discord Terms of Service while in this server.",
            ["B4"] = "The server owner reserves the permissions to ban any users for any given reason, even if its not against the rules, and is not required to provide a reason on why the ban was issued.",
            ["B4-A"] = "N/A",
            ["B5"] = "Joking about insensitive topics such as bombings, school shootings, political propaganda and other topics will result in a warn and a mute. This discord is meant to be an escape from the outside world so we don’t need politics, drama and insensitive jokes here.",
            ["B6"] = "Please do not ping high ranking staff or Rainers for trivial reasons, this will result in a warn and a mute. If it is important please make a ticket or follow chain of command. Chain of command basically means go through lower staff before going to higher staff members.",
            ["B7"] = "The usage of bot commands in any other channel than <#709012714160390235> can result in a warning.",

            ["C1"] = "Use common sense when posting messages, if you have a feeling that it shouldn't be posted even though it isn't stated so in the rules, then you probably shouldn't",
            ["C2"] = "We understand that mental and physical health is important, but please do not talk about sensitive subjects like suicide, self-harm, and similar. Otherwise you may be muted. Seek professional help if needed.",
            ["C3"] = "if you have the permission to upload emojis, do not upload stickers, they are under the same permission as emojis, but without staff permission, uploading stickers will get you punished. If you have the permission for emojis, do not delete any of them, unless allowed by staff",
            ["C3-A"] = "Emojis that are low quality (white background, low resolution(unless that is a part of the joke in it)) will be removed.",
            ["C4"] = "This is a guild for everyone, not just you, if you tell someone to leave or harass them when they join, you will be warned & muted."
        };
        
        // I know this is horrible
        // I wanted to do a funny, it works, It's funky, its not practical, but i wanna keep it.
        // :)
        public static string Log {
            set
            {
                DiscordWebhookClient Web = new((string)Configuration.Get("ActionLog"));
                Web.SendMessageAsync(value, allowedMentions:Discord.AllowedMentions.None);
            }                
        }
    }
}
