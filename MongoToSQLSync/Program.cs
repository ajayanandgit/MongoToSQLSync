using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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

            var opLog = local.GetCollection("oplog.rs");

            GetLastEntryInOpLog(opLog);
            
            Console.ReadKey();
        }

        public static void GetLastEntryInOpLog(MongoCollection<BsonDocument> collection)
        {
            BsonValue lastId = BsonMinKey.Value;
            string connectionString = ConfigurationManager.ConnectionStrings["RulesEntities"].ConnectionString;
            while (true)
            {
                Console.WriteLine("Query last changed data");
                var query = Query.GT("ts", lastId);
                var cursor = collection.Find(query)
                    .SetFlags(QueryFlags.TailableCursor | QueryFlags.AwaitData)
                    .SetSortOrder(SortBy.Ascending("$natural"));


                Console.WriteLine("found {0} record(s)", cursor.Count());
                foreach (var document in cursor)
                {
                    lastId = document["ts"];
                    Console.WriteLine("LastId is {0}", lastId);
                    Console.WriteLine(document);

                    // dynamic data = JObject.Parse(document.ToString());
                    string[] table = document["ns"].ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (document["op"].ToString() == "i" && table.Length > 2 && table[2].Equals("[OfficeTypeMaster]", StringComparison.OrdinalIgnoreCase))
                    {
                        // string tableName = data.ns.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        // insert
                        //var assembly = Assembly.GetExecutingAssembly();
                        //var type = assembly.GetTypes()
                        //    .First(t => t.Name == tableName);                        
                        //var obj = Activator.CreateInstance(type);

                        var mst = new UnitOfWork<OfficeTypeMaster>(connectionString);

                        //var jsonWriterSettings1 = new JsonWriterSettings { OutputMode = JsonOutputMode.TenGen };
                        //var data1 = document["o"].ToJson(jsonWriterSettings1);
                        
                        //var jsonWriterSettings2 = new JsonWriterSettings { OutputMode = JsonOutputMode.Shell };
                        //var data2 = document["o"].ToJson(jsonWriterSettings2);
                        
                        //var jsonWriterSettings3 = new JsonWriterSettings { OutputMode = JsonOutputMode.JavaScript };
                        //var data3 = document["o"].ToJson(jsonWriterSettings3);

                        //foreach (BsonElement elm in document.Elements)
                        //{
                        //    var tt = elm.Name;
                        //}

                        //var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
                        //var data = document["o"].ToJson(jsonWriterSettings);

                        // this works except binary
                        // var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<OfficeTypeMaster1>(data);

                        var data = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<OfficeTypeMaster>(document["o"].ToJson());
                        mst.Repository.Insert(data);
                        mst.Save();
                    }
                }


                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }

        public static void CustomSerializer()
        {
            try
            {
                MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<Address>(cm =>
                    {
                        cm.AutoMap();
                        cm.GetMemberMap(a => a.ZipCode).SetSerializer(new ZipCodeSerializer());
                    });


                MongoClient mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
                var server = mongoClient.GetServer();
                var database = server.GetDatabase("test");
                var collection = database.GetCollection<Address>("test");

                collection.Drop();
                collection.Insert(new BsonDocument());
                collection.Insert(new BsonDocument("ZipCode", BsonNull.Value));
                collection.Insert(new BsonDocument("ZipCode", "12345"));
                collection.Insert(new BsonDocument("ZipCode", 56789));

                foreach (var document in collection.FindAll())
                {
                    Console.WriteLine(document.ToJson());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }
    }
}
