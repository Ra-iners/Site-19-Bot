using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Site___.Extensions;
using Site___.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Modules.Commands.Private
{
    public class Staff : ModuleBase<ShardedCommandContext>
    {
        [Command("Kick", RunMode = RunMode.Async)]
        public async Task KickUser(IGuildUser User = null, [Remainder] string Reason = "No reason specified")
        {
            Reason = Reason.ToRules();
            if (User is null)
            {
                await Context.Channel.SendMessageAsync("You must specify a user to kick.");
                return;
            }
            // Check if the user can be kicked
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.KickMembers))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Kick Members permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }
            if (User.HasPermission(GuildPermission.ManageMessages))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Not allowed";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Not allowed" +
                    $"", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are not allowed to kick this user.\nThey have administrative roles (ex; ManageMessages, KickMembers).```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }

            try
            {
                // DM User
                EmbedBuilder DM = new();
                DM.Title = $"You have been __kicked__ from {Context.Guild.Name}";
                DM.WithColor(247, 52, 58);
                DM.AddField("Reason", Reason);
                DM.WithAuthor(Context.Guild.Name, Context.Guild.IconUrl);
                DM.WithFooter($"Kicked by {Context.User.Username}#{Context.User.Discriminator}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                DM.WithThumbnailUrl(Context.Guild.IconUrl);

                await User.SendMessageAsync(embed: DM.Build());

                await User.KickAsync(Reason, new RequestOptions() { AuditLogReason = $"Kicked by {Context.User.Username}#{Context.User.Discriminator}" });
            }
            catch
            {
                // Occurs if it fails to DM the user
                await Context.Message.ReplyAsync("User was kicked, however I was unable to DM them.");
            }

            Globals.Log = $"**Kick** | {Context.User.Username}#{Context.User.Discriminator} ({Context.User.Id}) kicked {User.Username}#{User.Discriminator} ({User.Id}) for {Reason}";
            EmbedBuilder Response = new();
            Response.Title = ":white_check_mark: Successfully Kicked!";
            Response.WithColor(119, 178, 85);
            Response.WithAuthor(User);
            Response.WithFooter($"{Context.Guild.Name} | Kick User", Context.Guild.IconUrl);
            Response.WithThumbnailUrl(User.GetAvatarUrl() ?? User.GetDefaultAvatarUrl());
            Response.WithDescription($@"**Username:** ``{User.Username}#{User.Discriminator}``
**ID:** ``{User.Id}``
**Joined At:** <t:{User.JoinedAt.GetValueOrDefault().ToUnixTimeSeconds()}:F>
**Created At:** <t:{User.CreatedAt.ToUnixTimeSeconds()}:F>
**Reason:** ```{Reason}```");

            await Context.Message.ReplyAsync(embed: Response.Build());
            (Context.User as IGuildUser).IncrimentModstat(Enums.Modstat.Kicks);
        }


        [Command("Ban", RunMode = RunMode.Async)]
        public async Task BanUser(IGuildUser User = null, [Remainder] string Reason = "No reason specified")
        {
            Reason = Reason.ToRules();
            if (User is null)
            {
                await Context.Channel.SendMessageAsync("You must specify a user to ban.");
                return;
            }
            // Check if the user can be banned
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.BanMembers))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Ban Members permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }
            if (User.HasPermission(GuildPermission.ManageMessages))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Not allowed";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Not allowed" +
                    $"", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are not allowed to ban this user.\nThey have administrative roles (ex; ManageMessages, KickMembers).```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }

            try
            {
                // DM User
                EmbedBuilder DM = new();
                DM.Title = $"You have been __banned__ from {Context.Guild.Name}";
                DM.WithColor(247, 52, 58);
                DM.AddField("Reason", Reason);
                DM.WithAuthor(Context.Guild.Name, Context.Guild.IconUrl);
                DM.WithFooter($"Banned by {Context.User.Username}#{Context.User.Discriminator}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                DM.WithThumbnailUrl(Context.Guild.IconUrl);

                ComponentBuilder cmp = new();
                cmp.WithButton("Appeal", null, ButtonStyle.Link, Emoji.Parse(":envelope:"), "https://docs.google.com/forms/d/e/1FAIpQLSeaqsdVAWuloy6DiciERdZZaEgNWxpLzr2LpCWMVsVaHeNFIg/viewform");
                await User.SendMessageAsync(embed: DM.Build(), components: cmp.Build());

                await User.BanAsync(0, Reason, new RequestOptions() { AuditLogReason = $"Banned by {Context.User.Username}#{Context.User.Discriminator}" });
            }
            catch
            {
                // Occurs if it fails to DM the user
                await Context.Message.ReplyAsync("User was banned, however I was unable to DM them.");
            }

            Globals.Log = $"**Ban** | {Context.User.Username}#{Context.User.Discriminator} ({Context.User.Id}) banned {User.Username}#{User.Discriminator} ({User.Id}) for {Reason}";
            EmbedBuilder Response = new();
            Response.Title = ":white_check_mark: Successfully Banned!";
            Response.WithColor(119, 178, 85);
            Response.WithAuthor(User);
            Response.WithFooter($"{Context.Guild.Name} | Ban User", Context.Guild.IconUrl);
            Response.WithThumbnailUrl(User.GetAvatarUrl() ?? User.GetDefaultAvatarUrl());
            Response.WithDescription($@"**Username:** ``{User.Username}#{User.Discriminator}``
**ID:** ``{User.Id}``
**Joined At:** <t:{User.JoinedAt.GetValueOrDefault().ToUnixTimeSeconds()}:F>
**Created At:** <t:{User.CreatedAt.ToUnixTimeSeconds()}:F>
**Reason:** ```{Reason}```");

            await Context.Message.ReplyAsync(embed: Response.Build());
            (Context.User as IGuildUser).IncrimentModstat(Enums.Modstat.Bans);
        }
        [Command("Ban", RunMode = RunMode.Async)]
        public async Task BanUser(ulong User = 0, [Remainder] string Reason = "No reason specified")
        {
            Reason = Reason.ToRules();
            if (User is 0)
            {
                await Context.Channel.SendMessageAsync("You must specify a user to ban.");
                return;
            }

            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.BanMembers))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Ban Members permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }

            try
            {
                await Context.Guild.AddBanAsync(User, 0, Reason, new RequestOptions() { AuditLogReason = $"Banned by {Context.User.Username}#{Context.User.Discriminator}" });
            }
            catch
            {
                // Occurs if it fails to DM the user
                await Context.Message.ReplyAsync("I was unable to ban this user.");
            }

            Globals.Log = $"**Ban** | {Context.User.Username}#{Context.User.Discriminator} ({Context.User.Id}) banned ``{User}`` for {Reason}";

            await Context.Message.ReplyAsync("Succesfully banned user.");
            (Context.User as IGuildUser).IncrimentModstat(Enums.Modstat.Bans);
        }
        [Command("Unban", RunMode = RunMode.Async)]
        [Alias("Pardon")]
        public async Task Unban(string User = null)
        {
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.BanMembers))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Ban Members permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }
            if(User == null)
            {
                await Context.Message.ReplyAsync("Enter a username or UserID");
            }

            var Bans = await Context.Guild.GetBansAsync().FlattenAsync();
            var GUser = Bans.FirstOrDefault(x => x.User.Username.ToLower() == User.ToLower());
            if (GUser == null)
            {
                await Context.Message.ReplyAsync("I couldn't find that user in the ban list.");
                return;
            }
            await Context.Guild.RemoveBanAsync(GUser.User, new RequestOptions() { AuditLogReason = $"Unbanned by: {Context.User.Username}#{Context.User.Discriminator}" });
        }
        [Command("Unban", RunMode = RunMode.Async)]
        [Alias("Pardon")]
        public async Task Unban(ulong User = 0)
        {
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.BanMembers))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Ban Members permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }

            try
            {
                await Context.Guild.RemoveBanAsync(User, new RequestOptions() { AuditLogReason = $"Unbanned by: {Context.User.Username}#{Context.User.Discriminator}" });
                await Context.Message.ReplyAsync("User unbanned");
            }
            catch
            {
                await Context.Message.ReplyAsync("User via given ID does not exist");
            }
        }
        [Command("Softban", RunMode = RunMode.Async)]
        public async Task Softban(IGuildUser User = null, int DaysToPrune = 1, [Remainder] string Reason = "No reason specified")
        {
            Reason = Reason.ToRules();
            if (User is null)
            {
                await Context.Channel.SendMessageAsync("You must specify a user to ban.");
                return;
            }
            // Check if the user can be banned
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.BanMembers))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Ban Members permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }
            if (User.HasPermission(GuildPermission.ManageMessages))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Not allowed";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Not allowed" +
                    $"", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are not allowed to ban this user.\nThey have administrative roles (ex; ManageMessages, KickMembers).```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }

            try
            {
                await Context.Guild.AddBanAsync(User, DaysToPrune, Reason, new RequestOptions() { AuditLogReason = $"Banned by {Context.User.Username}#{Context.User.Discriminator}" });
                await Context.Guild.RemoveBanAsync(User.Id);
            }
            catch
            {
                // Occurs if it fails to DM the user
                await Context.Message.ReplyAsync("I was unable to soft-ban this user.");
            }

            Globals.Log = $"**Soft-Ban** | {Context.User.Username}#{Context.User.Discriminator} ({Context.User.Id}) soft-banned ``{User}`` for {Reason} while pruning {DaysToPrune} days.";

            EmbedBuilder Response = new();
            Response.Title = ":white_check_mark: Successfully Soft-Banned!";
            Response.WithColor(119, 178, 85);
            Response.WithAuthor(User);
            Response.WithFooter($"{Context.Guild.Name} | Soft-Ban User", Context.Guild.IconUrl);
            Response.WithThumbnailUrl(User.GetAvatarUrl() ?? User.GetDefaultAvatarUrl());
            Response.WithDescription($@"**Username:** ``{User.Username}#{User.Discriminator}``
**ID:** ``{User.Id}``
**Joined At:** <t:{User.JoinedAt.GetValueOrDefault().ToUnixTimeSeconds()}:F>
**Created At:** <t:{User.CreatedAt.ToUnixTimeSeconds()}:F>
**Reason:** ```{Reason}```");

            await Context.Message.ReplyAsync(embed: Response.Build());
            (Context.User as IGuildUser).IncrimentModstat(Enums.Modstat.Kicks);
        }
        [Command("Purge", RunMode = RunMode.Async)]
        [Alias("Clear")]
        public async Task Purge(int Count=0, PurgeFlags Flags= PurgeFlags.All)
        {
            if (Count is 0)
            {
                await Context.Channel.SendMessageAsync("You must specify a number of messages to purge.");
                return;
            }
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ManageMessages))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Manage Messages permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }
            Globals.Log = "**Purge** | " + Context.User.Username + "#" + Context.User.Discriminator + " (" + Context.User.Id + ") purged ``" + Count + "`` messages in ``" + Context.Channel.Name + "``.";
            var Messages = await Context.Channel.GetMessagesAsync(Count).FlattenAsync();
            
            List<IMessage> ToBeDeleted = new();
            foreach(var Message in Messages)
            {
                if (Flags == PurgeFlags.All)
                    ToBeDeleted.Add(Message);
                else if (Flags == PurgeFlags.Bots && Message.Author.IsBot)
                    ToBeDeleted.Add(Message);
                else if (Flags == PurgeFlags.Webhooks && Message.Author.IsWebhook)
                    ToBeDeleted.Add(Message);
                else if (Flags == PurgeFlags.Users && !Message.Author.IsBot && !Message.Author.IsWebhook)
                    ToBeDeleted.Add(Message);
            }

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(ToBeDeleted);

            await Context.Channel.SendMessageAsync($"Succesfully purged {Count} messages.");
        }
        [Command("Warn", RunMode = RunMode.Async)]
        public async Task Warn(IGuildUser User=null, [Remainder] string Reason = "No reason specified")
        {
            try
            {
                Reason = Reason.ToRules();
                if (User is null)
                {
                    await Context.Channel.SendMessageAsync("You must specify a user to warn.");
                    return;
                }
                if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ManageMessages))
                {
                    EmbedBuilder Perms = new();
                    Perms.Title = ":tools: Insufficient permissions";
                    Perms.WithColor(247, 52, 58);
                    Perms.WithAuthor(Context.User as IGuildUser);
                    Perms.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                    Perms.AddField("Error", "```You are missing the Manage Members permission for this command```");
                    Perms.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Message.ReplyAsync(embed: Perms.Build());
                    return;
                }
                Database WarnDB = new($"Users/{User.Id}");

                int Case = Configuration.IncrimentCase();

                var WarnData = new IWarning()
                {
                    Reason = Reason,
                    WarnedBy = Context.User.Username + "#" + Context.User.Discriminator + " (" + Context.User.Id + ")",
                    WarnedAt = DateTime.Now,
                    Expires = DateTime.Now.AddDays(14)
                };
                WarnDB.Set("Warnings", $"{Case}", JsonConvert.SerializeObject(WarnData));

                EmbedBuilder DM = new();
                DM.Title = ":warning: You have been warned!";
                DM.WithColor(247, 52, 58);
                DM.WithAuthor(Context.Guild.Name, Context.Guild.IconUrl);
                DM.WithFooter($"Warned by {Context.User.Username}#{Context.User.Discriminator}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                DM.AddField(new EmbedFieldBuilder()
                {
                    Name = "Reason",
                    Value = Reason,
                    IsInline = true
                });
                DM.AddField(new EmbedFieldBuilder()
                {
                    Name = "Expires",
                    Value = "<t:" + DateTimeOffset.Now.AddDays(14).ToUnixTimeSeconds() + ":R>",
                    IsInline = true
                });

                await User.SendMessageAsync(embed: DM.Build());
                Globals.Log = "**Warn** | " + Context.User.Username + "#" + Context.User.Discriminator + " (" + Context.User.Id + ") warned ``" + User.Username + "#" + User.Discriminator + " (" + User.Id + ")`` for ``" + Reason + "``.";

                EmbedBuilder Result = new();
                Result.Title = ":white_check_mark: Successfully Warned!";
                Result.WithColor(119, 178, 85);
                Result.WithAuthor(User);
                Result.WithFooter($"{Context.Guild.Name} | Warn User", Context.Guild.IconUrl);
                Result.WithThumbnailUrl(User.GetAvatarUrl() ?? User.GetDefaultAvatarUrl());
                Result.WithDescription($@"**Username:** ``{User.Username}#{User.Discriminator}``
**ID:** ``{User.Id}``
**Joined At:** <t:{User.JoinedAt.GetValueOrDefault().ToUnixTimeSeconds()}:F>
**Created At:** <t:{User.CreatedAt.ToUnixTimeSeconds()}:F>
**Reason:** ```{Reason}```");

                await Context.Message.ReplyAsync(embed: Result.Build());
                (Context.User as IGuildUser).IncrimentModstat(Enums.Modstat.Warnings);
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
        [Command("Warnings", RunMode = RunMode.Async)]
        public async Task ViewWarnings(IGuildUser User=null, bool ViewExpired = false)
        {
            User ??= Context.User as IGuildUser;
            try
            {
                if ((Context.User as IGuildUser).HasPermission(GuildPermission.ManageMessages) || User == Context.User as IGuildUser)
                {

                    // Okay so since I had the DB written like really horribly
                    // The easiest way to do this is to just get the warnings as files
                    // and then parse them into a list of warnings

                    // Allow to view warnings
                    if (Directory.Exists($"Database/Users/{User.Id}/Warnings"))
                    {
                        Dictionary<int, IWarning> Warnings = new(); // Case Number : Warning Object
                       
                        foreach (string Warning in Directory.GetFiles($"Database/Users/{User.Id}/Warnings"))
                        {
                            int Case = int.Parse(Path.GetFileName(Warning)); // The file name is supposed to be a number, nothing else
                            Warnings.Add(Case, JsonConvert.DeserializeObject<IWarning>(File.ReadAllText(Warning)));
                        }
                        if (Warnings.Count is 0)
                        {
                            await Context.Channel.SendMessageAsync("This user has no warnings.");
                            return;
                        }
                        EmbedBuilder WarningsEmbed = new();
                        WarningsEmbed.Title = ":warning: Warnings";
                        WarningsEmbed.WithColor(247, 52, 58);
                        WarningsEmbed.WithAuthor(User);
                        WarningsEmbed.WithFooter($"{Context.Guild.Name} | Warnings", Context.Guild.IconUrl);
                        WarningsEmbed.WithThumbnailUrl(User.GetAvatarUrl() ?? User.GetDefaultAvatarUrl());

                        foreach(var Warning in Warnings)
                        {
                            if(ViewExpired)
                                WarningsEmbed.AddField($"W {Warning.Key}", $"Reason: ```{Warning.Value.Reason}```\nExpired?: {(Warning.Value.Expires < DateTime.Now ? "🟢" : "🔴")}", true);
                            else
                            {
                                if (Warning.Value.Expires < DateTime.Now)
                                    continue;
                                WarningsEmbed.AddField($"W {Warning.Key}", $"Reason: ```{Warning.Value.Reason}```\nExpired?: {(Warning.Value.Expires < DateTime.Now ? "🟢" : "🔴")}", true);
                            }
                        }
                        
                        await Context.Message.ReplyAsync(embed: WarningsEmbed.Build());
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("This user has no warnings.");
                    }
                }
                else
                {
                    EmbedBuilder Perms = new();
                    Perms.Title = ":tools: Insufficient permissions";
                    Perms.WithColor(247, 52, 58);
                    Perms.WithAuthor(Context.User as IGuildUser);
                    Perms.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                    Perms.AddField("Error", "```You are missing the Manage Members permission for this command```");
                    Perms.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Message.ReplyAsync(embed: Perms.Build());
                    return;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
        [Command("ForceQOTD", RunMode = RunMode.Async)]
        [Summary("Forcefully creates a new QOTD right now with the given question")]
        public async Task ForceQOTD([Remainder] string Input)
        {
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.Administrator))
            {
                EmbedBuilder Result = new();
                Result.Title = ":tools: Insufficient permissions";
                Result.WithColor(247, 52, 58);
                Result.WithAuthor(Context.User as IGuildUser);
                Result.WithFooter($"{Context.Guild.Name} | Insufficient permissions", Context.Guild.IconUrl);
                Result.AddField("Error", "```You are missing the Administrator permission for this command```");
                Result.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Message.ReplyAsync(embed: Result.Build());
                return;
            }

            var QOTD = Context.Guild.GetChannel(846240999609073674) as ITextChannel; // QOTD Channel

            EmbedBuilder Question = new();
            Question.WithColor(240, 64, 64);
            Question.AddField("Question", Input);
            Question.WithAuthor(Context.User);
            Question.WithCurrentTimestamp();
            Question.WithThumbnailUrl("https://cdn.discordapp.com/attachments/874423096428355674/963005116397522994/qotd2.png");

            var Message = await QOTD.SendMessageAsync("<@&847573204623949904>", embed: Question.Build());
            await QOTD.CreateThreadAsync("QOTD", ThreadType.PublicThread, ThreadArchiveDuration.OneDay, Message, false, 10800);
            await Context.Message.ReplyAsync("QOTD Created");
        }
    }   
}
