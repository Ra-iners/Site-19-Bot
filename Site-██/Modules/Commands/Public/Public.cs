using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Modules.Commands.Public
{
    public class Public : ModuleBase<ShardedCommandContext>
    {
        [Command("Help", RunMode = RunMode.Async)]
        public async Task Help()
        {
            StringBuilder Response = new();
            foreach (var Module in Globals.Commands.Modules)
            {
                Response.AppendLine($"\n**{Module.Name}**");
                foreach (var Command in Module.Commands)
                {
                    Response.Append($"`{Command.Aliases.First()}`, ");
                }
            }
            
            await ReplyAsync(Response.ToString());
        }

        [Command("TicketDisplay", RunMode = RunMode.Async)]
        public async Task TicketDisplay()
        {
            EmbedBuilder CTicket = new();

            CTicket.Color = new Color(233, 89, 110);
            CTicket.Title = ":tickets: Create A Ticket.";
            CTicket.Description = $"Is someone breaking the rules? or maybe you have a question to ask?\nYou can directly contact staff by selecting your issue below";
            CTicket.WithFooter("Miss-use of this feature can get you punished!");

            ComponentBuilder cmp = new();
            cmp.WithSelectMenu(new SelectMenuBuilder()
            {
                CustomId = "create_ticket",
                Placeholder = "Select a Ticket reason",
                Options = new()
                {
                    new()
                    {
                        Label = "Rule Breaker",
                        Value = "Rule Breaker",
                        Description = $"Someone breaking the rules? Use this to report them."
                    },
                    new()
                    {
                        Label = "General Question",
                        Value = "General Question",
                        Description = $"Have a question that regular members cannot answer? Ask it here."
                    },
                    new()
                    {
                        Label = "Other",
                        Value = "Other",
                        Description = $"Is your reason not listed here? Use this instead."
                    }
                }
            });
            await Context.Message.ReplyAsync(null, embed: CTicket.Build(), components: cmp.Build());
        }
    }
}
