using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Qurre.API.DataBase
{
    public class DataBase
    {
        internal DataBase()
        {
            Thread thread = new Thread(() => _());
            thread.Start();
        }
        public bool Enabled { get; internal set; }
        public bool Connected { get; internal set; }
        public static DataBase Static => Server.DataBase;
        public IMongoDatabase GetDatabase(string name) => Client.GetDatabase(name);
        public IMongoCollection<BsonDocument> GetCollection(IMongoDatabase database, string name) => database.GetCollection<BsonDocument>(name);
        public List<BsonDocument> GetDocuments(IMongoCollection<BsonDocument> collection, BsonDocument parameters) => collection.Find(parameters).ToList();
        public object GetValue(BsonDocument document, string key) => document[key];
        internal MongoClient Client { get; private set; }
        internal MongoDataBase MongoDataBase { get; private set; }
        private void _()
        {
            Plugin.Config.GetString("qurre_database", "undefined", "Link-access to your database(MongoDB)");
            string _link = Plugin.Config.ConfigManager.GetString("qurre_database", "");
            if (_link != "" && _link != "undefined")
            {
                Enabled = true;
                try
                {
                    Client = new MongoClient(_link);
                    Connected = true;
                    MongoDataBase = new MongoDataBase(Client);
                }
                catch (Exception e) { Log.Error($"umm, error while connecting to MongoDB:\n{e}\n{e.StackTrace}"); }
            }
        }
    }
}