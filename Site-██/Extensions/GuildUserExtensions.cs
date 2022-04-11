using Discord;
using Newtonsoft.Json.Linq;
using Serilog;
using Site___.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Extensions
{
    public static class GuildUserExtensions
    {
        // Helper extension cause it doesn't return true by default if a user doesn't have kick perms but has admin perms
        public static bool HasPermission(this IGuildUser User, GuildPermission Permission)
        {
            if (User.GuildPermissions.Has(Permission) || User.GuildPermissions.Has(GuildPermission.Administrator))
                return true;
            else
                return false;
        }
        public static void IncrimentModstat(this IGuildUser User, Modstat type, int amount=1)
        {
            if (User.Get("Modstats") is null)
                User.Set("Modstats", new JObject() { 
                ["Warnings"] = 0,
                ["Mutes"] = 0,
                ["Kicks"] = 0,
                ["Bans"] = 0});
            

            JObject Modstats = JObject.Parse((string)User.Get("Modstats"));
            Log.Information(Modstats.ToString());
            switch(type)
            {
                case Modstat.Mutes:
                    Modstats["Mutes"] = (int)Modstats["Mutes"] + amount;
                    break;
                case Modstat.Warnings:
                    Modstats["Warnings"] = (int)Modstats["Warnings"] + amount;
                    break;
                case Modstat.Kicks:
                    Modstats["Kicks"] = (int)Modstats["Kicks"] + amount;
                    break;
                case Modstat.Bans:
                    Modstats["Bans"] = (int)Modstats["Bans"] + amount;
                    break;
            }
            User.Set("Modstats", Modstats.ToString());
        }
    }
}
