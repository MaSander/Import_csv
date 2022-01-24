using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImportAsCsv
{
    class Repository
    {

        private IMongoDatabase db;

        private string connectionString;
        private string databaseName;

        public Repository()
        {
            connectionString = System.Environment.GetEnvironmentVariable("connectionstring");
            connectionString = "mongodb://admin:nRZxUXse9u1HMUuc@SG-Gooders-22079.servers.mongodirector.com:27017/?authSource=admin";
            databaseName = "gooders-prod";

            var client = new MongoClient(connectionString);
            db = client.GetDatabase(databaseName);
        }

        public void ImportCollection(string name)
        {
            var fileName = Path.Combine(System.Environment.CurrentDirectory, $"{name}.csv");
            var collection = db.GetCollection<Dictionary<object, object>>(name);

            var collectionTest = db.GetCollection<Dictionary<string, Type>>(name);

            var collectionList = collection.Find(new BsonDocument()).ToList();

            Dictionary<string, Type> columns = new Dictionary<string, Type>();

            #region Cria cabeçalho do csv
            ///Todo que de fato for um 'object' tem suas keys transformadas em colunas: "{key}"

            string str = string.Empty;            
            foreach(var line in collectionList[0])
            {
                Console.WriteLine(line.Key.ToString() + " - " + line.Value.ToString());

                try
                {
                    str = line.Value.ToString();
                    if (str.ToLower() == "system.dynamic.expandoobject")
                    {
                        columns = columns.Concat(GetTheExpandoObjectKeys(line).ToDictionary(x => x.Key, x => x.Value)).ToDictionary(x => x.Key, x => x.Value);
                    }
                    else
                    {
                        columns.Add(line.Key.ToString(), typeof(string));
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    columns.Add(line.Key.ToString(), typeof(Exception));
                }
            }

            {
                string csvHeader = string.Empty;
                int i = 1;
                foreach(var column in columns.Keys)
                {
                    csvHeader += $"{column}";
                    if (i != columns.Keys.Count()) csvHeader += ";";
                    i++;
                }
                File.AppendAllText(fileName, csvHeader);
            }

            #endregion

            #region Separando e salvando valores no csv

            for (var i = 0; i < collectionList.Count; i++ )
            {
                Console.WriteLine($"{i}/{collectionList.Count}");

                var lineDatas = string.Empty;

                foreach (var line in collectionList[i])
                {
                    str = line.Value?.ToString();
                    if(str == null)
                    {
                        lineDatas += ";";
                    }
                    else if (str.ToLower() == "system.dynamic.expandoobject")
                    {
                        lineDatas += GetObjectValue(line.Value);
                    }
                    else if(str.ToLower() == "system.collections.generic.list`1[system.object]")
                    {
                        lineDatas += GetArrayValue(line.Value);
                    }
                    else
                    {
                        lineDatas += $"{str};";
                    }
                }

                lineDatas = lineDatas.Replace("\n", " ").Replace("\r", " ");

                File.AppendAllText(fileName, $"\n{lineDatas}");

            };

            #endregion

            Console.WriteLine("Completed!!!");
        }

        private string GetArrayValue(object value)
        {
            return ";";
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

        private Dictionary<string, Type> GetTheExpandoObjectKeys(KeyValuePair<object, object> data)
        {
            string key = (string)data.Key;

            var json = data.Value.ToJson();
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            Dictionary<string, Type> dict = dictionary.ToDictionary(x => $"{key}-{x.Key}", x => typeof(object));

            return dict;
        }

        private string GetObjectValue(object data)
        {
            string str = "";

            try
            {
                var json = data.ToJson();
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                foreach (var value in dictionary.Values)
                {
                    str += $"{value};";
                }
            }
            catch 
            {
                Console.WriteLine("Exeption to json");
            }

            return str;
        }
    }
}
