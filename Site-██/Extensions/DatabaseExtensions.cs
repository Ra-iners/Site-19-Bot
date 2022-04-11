using Discord;
using Discord.WebSocket;
using Site___.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Extensions
{
    public static class DatabaseExtensions
    {
        #region IGuildUser
        public static void Set(this IGuildUser User, string Key, object Value)
        {
            Database _db = new Database("Users");
            _db.Set(User.Id.ToString(), Key, Value);
        }
        public static object Get(this IGuildUser User, string Key)
        {
            Database _db = new Database("Users");
            return _db.Get(User.Id.ToString(), Key);
        }
        #endregion

        #region IUser
        public static void Set(this IUser User, string Key, object Value)
        {
            Database _db = new Database("Users");
            _db.Set(User.Id.ToString(), Key, Value);
        }
        public static object Get(this IUser User, string Key)
        {
            Database _db = new Database("Users");
            return _db.Get(User.Id.ToString(), Key);
        }
        #endregion
        
        #region SocketUser
        public static void Set(this SocketUser User, string Key, object Value)
        {
            Database _db = new Database("Users");
            _db.Set(User.Id.ToString(), Key, Value);
        }
        public static object Get(this SocketUser User, string Key)
        {
            Database _db = new Database("Users");
            return _db.Get(User.Id.ToString(), Key);
        }
        #endregion

        #region SocketGuildUser
        public static void Set(this SocketGuildUser User, string Key, object Value)
        {
            Database _db = new Database("Users");
            _db.Set(User.Id.ToString(), Key, Value);
        }
        public static object Get(this SocketGuildUser User, string Key)
        {
            Database _db = new Database("Users");
            return _db.Get(User.Id.ToString(), Key);
        }
        #endregion

        #region SocketGuild
        public static void Set(this SocketGuild Guild, string Key, object Value)
        {
            Database _db = new Database("Guilds");
            _db.Set(Guild.Id.ToString(), Key, Value);
        }
        public static object Get(this SocketGuild Guild, string Key)
        {
            Database _db = new Database("Guilds");
            return _db.Get(Guild.Id.ToString(), Key);
        }
        #endregion
        
        #region IGuild
        public static void Set(this IGuild Guild, string Key, object Value)
        {
            Database _db = new Database("Guilds");
            _db.Set(Guild.Id.ToString(), Key, Value);
        }
        public static object Get(this IGuild Guild, string Key)
        {
            Database _db = new Database("Guilds");
            return _db.Get(Guild.Id.ToString(), Key);
        }
        #endregion
    }
}
