using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportMongoDB.WebApp
{
    public class Repository
    {
        private IMongoDatabase db;

        private string connectionString;
        private string databaseName;

        public Repository()
        {
            connectionString = System.Environment.GetEnvironmentVariable("connectionstring");
            connectionString = "mongodb://admin:nRZxUXse9u1HMUuc@SG-Gooders-22079.servers.mongodirector.com:27017/?authSource=admin";
            databaseName = "gooders-prod";

            MongoClient client = new MongoClient(connectionString);
            db = client.GetDatabase(databaseName);
        }

        public void ImportCollection(string name)
        {
            var fileName = Path.Combine(System.Environment.CurrentDirectory, $"{name}.csv");
            var collection = db.GetCollection<Dictionary<object, object>>(name);
            var collectionList = collection.Find(new BsonDocument()).ToList();

            var header = "";
            foreach (var line in collectionList[0])
            {
                header += line.Key + ";";
                Console.WriteLine(line.Value);
            }
            header += "\n";
            File.AppendAllText(fileName, header);

            for (var i = 0; i < collectionList.Count; i++)
            {
                Console.WriteLine($"{i}/{collectionList.Count}");
                var colums = "";
                foreach (var line in collectionList[i])
                {
                    colums += line.Value + ";";
                }
                colums += "\n";
                File.AppendAllText(fileName, colums);

            };

            Console.WriteLine("Completed!!!");
        }

        public List<string> GetCollectionsNamesList()
        {
            List<string> list = new List<string>(); ;

            foreach (var item in db.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
            {
                var name = Regex.Split(item[4]["ns"].ToString(), $"({databaseName}.)");

                list.Add(name[2].ToString());
            }

            return list;
        }
    }
}
