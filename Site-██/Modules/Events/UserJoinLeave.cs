using Discord;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Modules.Events
{
    public class UserJoinLeave
    {
        public void Initialize()
        {
            Log.Debug("{Module} Module initialized", GetType().Name + ".cs");
            var JoinLeave = Globals.Client.GetChannel(874449462674747403) as ITextChannel;
            
            Globals.Client.UserJoined += async(User) =>
            {
                if (User.Guild.Id != 696437457620828250) return; // Ignore if not from Site-██

                EmbedBuilder Log = new();
                Log.WithAuthor(User);
                Log.WithColor(Color.Green);
                Log.AddField("User Joined!", $"Welcome {User.Mention} to the server!\nWe hope you enjoy here!");
                Log.WithTimestamp(User.CreatedAt);
                Log.WithFooter("Created at");

                await JoinLeave.SendMessageAsync(embed:Log.Build());
            };

            Globals.Client.UserLeft += async (Guild,User) =>
            {
                if (Guild.Id != 696437457620828250) return; // Ignore if not from Site-██

                EmbedBuilder Log = new();
                Log.WithAuthor(User);
                Log.WithColor(Color.Red);
                Log.AddField("User Left!", $"Goodbye {User.Mention}..!\nWas nice having you here!");
                Log.WithTimestamp(User.CreatedAt);
                Log.WithFooter("Created at");

                await JoinLeave.SendMessageAsync(embed: Log.Build());
            };
        }
    }
}
