using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoToSQLSync
{
    class Program
    {
        static void Main(string[] args)
        {
            // mongodb://192.168.10.99
            const string connectionString = "mongodb://127.0.0.1:27017?replicaSet=rs0";
            MongoClient mongoClient = new MongoClient(connectionString);            

            MongoDatabase local = mongoClient.GetServer().GetDatabase("local");            

            //var reg = local.GetCollection("Sal_Head_Mst");
            //var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            //var rec = reg.FindOne().ToJson(jsonWriterSettings);

            var opLog = local.GetCollection("oplog.rs");

            GetLastEntryInOpLog(opLog);
            
            Console.ReadKey();
        }

        public static void GetLastEntryInOpLog(MongoCollection<BsonDocument> collection)
        {
            BsonValue lastId = BsonMinKey.Value;
            while (true)
            {
                var query = Query.GT("ts", lastId);
                var cursor = collection.Find(query)
                    .SetFlags(QueryFlags.TailableCursor | QueryFlags.AwaitData)
                    .SetSortOrder(SortBy.Ascending("$natural"));

                var count = 0;
                foreach (var document in cursor)
                {
                    lastId = document["ts"];
                    Console.WriteLine("LastId is {0}", lastId);
                    Console.WriteLine(document);
                    count++;
                }

                if (count == 0)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
            }
        }
 


    }
}
