using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Storage
{
    public static class Configuration
    {
        public static object Get(string Key)
        {
            JObject Config = JObject.Parse(File.ReadAllText("Configuration.json"));
            return Config["Discord"][Key].ToString();
        }

        public static void Set(string Key, string Value)
        {
            JObject Config = JObject.Parse(File.ReadAllText("Configuration.json"));
            Config["Discord"][Key] = Value;
            File.WriteAllText("Configuration.json", Config.ToString());
        }
        public static void Set(string Key, int Value)
        {
            JObject Config = JObject.Parse(File.ReadAllText("Configuration.json"));
            Config["Discord"][Key] = Value;
            File.WriteAllText("Configuration.json", Config.ToString());
        }

        public static int IncrimentCase()
        {
            int Current = int.Parse(Get("Incriment").ToString());
            Current++;
            Set("Incriment", Current);
            return Current;
        }
    }
}
