using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Discord;

namespace Site___.Modules.Events
{
    public class DailyQOTD
    {
        public async void Initialize()
        {
            await InvokeQOTD();
            
            System.Timers.Timer t = new()
            {
                Interval = 86400000, // 1 day = 86400000ms
            };
            t.Elapsed += async(sender, e) =>
            {
                await InvokeQOTD();
            };
            t.Start();
        }
        public async Task InvokeQOTD()
        {
            /*
            var QOTD = Globals.Client
                .GetGuild(696437457620828250)
                .GetChannel(846240999609073674) as ITextChannel; // QOTD Channel
            */
            var QOTD = Globals.Client
                .GetGuild(874407172535099473)
                .GetChannel(874449462674747403) as ITextChannel; // QOTD Channel

            // Get random post from submissions
            var Submissions = Globals.Client
                .GetGuild(696437457620828250)
                .GetChannel(846241073852055594) as ITextChannel; // Submissions Channel

            var Messages = await Submissions.GetMessagesAsync(30).FlattenAsync();
            var RandomMessage = Messages.ElementAt(new Random().Next(Messages.Count()));

            EmbedBuilder Question = new();
            Question.WithColor(240, 64, 64);
            Question.AddField("Question", RandomMessage.Content);
            Question.WithAuthor(RandomMessage.Author);
            Question.WithCurrentTimestamp();
            Question.WithThumbnailUrl("https://cdn.discordapp.com/attachments/874423096428355674/963005116397522994/qotd2.png");

            var Message = await QOTD.SendMessageAsync("<@&847573204623949904>", embed: Question.Build());
            await QOTD.CreateThreadAsync("QOTD", ThreadType.PublicThread, ThreadArchiveDuration.OneDay, Message, false, 10800);
        }
    }
}
