using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Site___.Extensions;
using Discord;
using System.Reflection;

namespace Site___.Modules.Events
{
    public class SuggestionReactions
    {
        public void Initialize()
        {
            Log.Debug("{Module} Module initialized", GetType().Name + ".cs");
            Globals.Client.MessageReceived += async (msg) =>
            {
                if (msg.Channel.Id != 811536427186913281) return; // Ignore all messages except in the suggestion channel
                if ((msg.Author as SocketGuildUser).HasPermission(GuildPermission.ManageMessages) || msg.Author.IsBot) return; // Ignore all messages from bots or staff

                if (msg.Content.ToLower().Contains("suggestion") && msg.Content.ToLower().Contains("description"))
                {
                    IEmote green = Emote.Parse("<:green:808021717087027200>");
                    IEmote red = Emote.Parse("<:red:808021717192015933>");

                    await msg.AddReactionAsync(green);
                    await msg.AddReactionAsync(red);
                }
                else
                {
                    var notice = await (msg as IUserMessage).ReplyAsync("Please follow the format of\n```Suggestion: your idea here\nDescription: the description of your idea\n```");
                    await Task.Delay(3000);
                    await msg.DeleteAsync();
                    await notice.DeleteAsync();
                }
            };
        }
    }
}
