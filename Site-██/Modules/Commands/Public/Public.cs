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
    }
}
