using Discord;
using Site___.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Modules.Interactions
{
    public class CreateTicket
    {
        public void Initialize()
        {
            Globals.Client.SelectMenuExecuted += async (Menu) =>
            {
                if (Menu.Data.CustomId != "create_ticket") return;

                var Reason = Menu.Data.Values.First();
                var Tickets = Globals.Client.GetChannel(873708122605228072) as ITextChannel;
                int Case = Configuration.IncrimentCase();

                var Thread = await Tickets.CreateThreadAsync($"Ticket: {Case}", ThreadType.PrivateThread, ThreadArchiveDuration.OneDay);
                await Thread.SendMessageAsync($"By: {Menu.User.Mention}\nReason: {Reason}\n\n{Menu.User.Mention} please explain your problem fruther, so a <@&696437457633148950> member could assist you");
                await Menu.RespondAsync($"Ticket created in {Thread.Mention}, please go there to be assisted further.", ephemeral:true);

                Globals.Log = $"**Ticket** | Ticket created by {Menu.User.Mention} | Case: {Case} | Reason: {Reason} | In {Thread.Mention}";
            };
        }
    }
}
