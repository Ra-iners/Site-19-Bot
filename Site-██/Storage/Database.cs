using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Site___.Extensions;
namespace Site___.Storage
{
    public class Database
    {
        private readonly string _collectionName;
        public Database(string CollectioName)
        {
            _collectionName = "Database/" + CollectioName;
            if(!Directory.Exists(_collectionName))
                Directory.CreateDirectory(_collectionName);
        }
        
        public void Set(string Node, string Key, object Value)
        {
            if (!Directory.Exists(_collectionName + "/" + Node))
                Directory.CreateDirectory(_collectionName + "/" + Node);

            var path = Path.Combine(_collectionName, Node, Key);
            File.WriteAllText(path, Value.ToString());
        }
        public object Get(string Node, string Key)
        {
            if (!Directory.Exists(_collectionName + "/" + Node))
                return null;

            var path = Path.Combine(_collectionName, Node, Key);
            if (!File.Exists(path)) return null;
            return File.ReadAllText(path);
        }
    }
}
