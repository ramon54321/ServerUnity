using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ServerConsole
{
    public class DatabaseManager
    {
        public static MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");
        public static IMongoDatabase database = null;
        public static IMongoCollection<Data_User> usersCollection;
		public static IMongoCollection<Data_Agent> agentsCollection;
		public static IMongoCollection<GameItems.GameItem> itemBlueprintsCollection;

        public DatabaseManager()
        {
            database = mongoClient.GetDatabase("toyarmy");

            // -- Register class mapping

            usersCollection = GetCollection<Data_User>("Users");
			agentsCollection = GetCollection<Data_Agent>("Agents");
            itemBlueprintsCollection = GetCollection<GameItems.GameItem>("ItemBlueprints");

            // -- Insert new blueprint
            /*
            GameItems.GameItem_Magazine wep = new GameItems.GameItem_Magazine();

            wep.InstanceType = new string[] { "GameItemInstance", "GameItemInstance_Magazine" };
            wep.ItemCode = "3.0.0";
            wep.ItemVersion = "1.0.0";
            wep.ItemNameSystem = "magazine_5strip";
            wep.ItemNameLong = "5 Round Strip Magazine";
            wep.ItemNameShort = "5rd Strip Mag";
            wep.ItemValue = 5;
            wep.ItemDescription = "A simple wrench for fixing things.";
            wep.ItemIsCarryable = true;
            wep.ItemIsStackable = false;
            wep.ItemStackMax = 0;
            wep.ItemWeight = 0.18f;
            wep.ItemVolume = "1x1";
            wep.ClassMagazineType = GameItems.MagazineType.strip_5;

            itemBlueprintsCollection.InsertOne(wep);
            */

            // -- Get access to agent
            //Data_User user = usersCollection.Find(userx => userx.Username == "Ramon").ToCursor().First();
            //Data_Agent agent = GetAgentFromUser(user);

            //agent.TransferItem(0, "Ramon");

            //agentsCollection.InsertOne(new Data_Agent(){ User = GetReferenceToObject("Users", "59b00c677759890f0ba4eb70") });

            //Data_User.GetUserByUsername("Sheldon").AddAgentToUser();

            //Data_Agent ramonsAgent = GetAgentFromUser(Data_User.GetUserByUsername("Ramon"));
            //Data_Agent sheldonsAgent = GetAgentFromUser(Data_User.GetUserByUsername("Sheldon"));

            //ramonsAgent.TransferItem(0, "Sheldon");

            //GetAgentFromUser(Data_User.GetUserByUsername("Ramon")).SpawnItem("m1903_springfield_rifle");

            //user.AddAgentToUser();

            //agent.SpawnItem("wrench");

            //agent.InsertItem("m1903_springfield_rifle");
            //agent.Save();

            //List<GameItems.GameItem> items = new List<GameItems.GameItem>();
            //items = JsonArrayToList<GameItems.GameItem>(agent.GetInventoryJson());

            // -- Get json of item
            //string json = ObjectToJson(theGun);

            // -- Get item from json
            //GameItems.GameItem_Weapon wep = JsonToObject<GameItems.GameItem_Weapon>(json);

            //agent.Save();

            /* Create new blueprint
            GameItems.GameItem_Weapon wep = new GameItems.GameItem_Weapon();

            wep.InstanceType = new string[] { "GameItemInstance", "GameItemInstance_Weapon" };
            wep.ItemCode = "2.0.0";
            wep.ItemVersion = "1.0.0";
            wep.ItemNameSystem = "m1903_springfield_rifle";
            wep.ItemNameLong = "M1903 Springfield Rifle";
            wep.ItemNameShort = "M1903";
            wep.ItemValue = 440;
            wep.ItemDescription = "A basic bolt action rifle, which can be used at medium to long range.";
            wep.ItemIsCarryable = true;
            wep.ItemIsStackable = false;
            wep.ItemStackMax = 0;
            wep.ItemWeight = 3.94f;
            wep.ItemVolume = "4x1";
            wep.ClassReloadTime = 2.2f;
            wep.ClassRPS = 1f;
            wep.ClassBarrelVelocityCoef = 1.08f;
            wep.ClassUnaccuracy = 0.16f;
            wep.ClassNoiseCoef = 1.02f;
            wep.ClassMagazineTypeList = new List<GameItems.MagazineType>();
            wep.ClassMagazineTypeList.Add(GameItems.MagazineType.strip_5);
            wep.ClassMagazineTypeList.Add(GameItems.MagazineType.strip_8);
            wep.ClassMagazineTypeList.Add(GameItems.MagazineType.strip_12);
            wep.ClassMagazineTypeList.Add(GameItems.MagazineType.strip_box_20);
            wep.ClassMagazineTypeList.Add(GameItems.MagazineType.strip_box_25);

            itemBlueprintsCollection.InsertOne(wep);
            */
        }

        public static List<T> JsonArrayToList<T>(string json)
        {
            List<T> objectList = new List<T>();

            string[] separatingChars = { "~" };
            string[] docs = json.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string doc in docs) {
                objectList.Add(BsonSerializer.Deserialize<T>(BsonDocument.Parse(doc)));
            }
            return objectList;
        }

        public static T JsonToObject<T>(string json)
        {
            T obj = BsonSerializer.Deserialize<T>(BsonDocument.Parse(json));
            return obj;
        }

        public static string ObjectToJson(Object obj)
        {
            return obj.ToJson();
        }

		public static Data_Agent GetAgentById(ObjectId id)
        {
            Data_Agent agent = null;
            try
			{
				agent = agentsCollection.Find(agt => agt.Id == id).ToCursor().First();
            }
            catch (Exception e)
			{
				Console.WriteLine("Handled error.\n" + e.StackTrace);
			}
            return agent;
        }

		public static Data_User GetUserById(ObjectId id)
        {
            Data_User user = null;
            try
			{
				user = usersCollection.Find(usr => usr.Id == id).ToCursor().First();
            }
            catch (Exception e)
			{
				Console.WriteLine("Handled error.\n" + e.StackTrace);
			}
            return user;
        }

		public static Data_User GetUserByUsername(string username)
        {
            Data_User user = null;
            try
			{
				user = usersCollection.Find(usr => usr.Username == username).ToCursor().First();
            }
            catch (Exception e)
			{
				Console.WriteLine("Handled error.\n" + e.StackTrace);
			}
            return user;
        }

		public static Data_User GetUserByEmail(string email)
        {
            Data_User user = null;
            try
			{
				user = usersCollection.Find(usr => usr.Email == email).ToCursor().First();
            }
            catch (Exception e)
			{
				Console.WriteLine("Handled error.\n" + e.StackTrace);
			}
            return user;
        }

        public static Data_Agent GetAgentFromUser(Data_User user)
        {
            Data_Agent agent = null;
            try
			{
				agent = agentsCollection.Find(agt => agt.User.Id == user.Id).ToCursor().First();
            }
            catch (Exception e)
			{
				Console.WriteLine("Handled error.\n" + e.StackTrace + e.Message);
			}
            return agent;
        }

		public static Data_User GetUserFromAgent(Data_Agent agent)
		{
			Data_User user = null;
			try
			{
                user = GetObjectFromReference<Data_User>(agent.User);
            }
            catch (Exception e)
			{
				Console.WriteLine("Handled error.\n" + e.StackTrace);
			}
            return user;
        }

        public static MongoDBRef GetReferenceToObject(string collectionName, ObjectId id)
        {
            return new MongoDBRef(collectionName, id);
        }

        public static T GetObjectFromReference<T>(MongoDBRef reference)
		{
			try
			{
                return GetCollection<T>(reference.CollectionName).Find(new BsonDocument("_id", reference.Id)).ToCursor().First();
            }
            catch (Exception e)
			{
                Console.WriteLine("Handled error.\n" + e.StackTrace);
                return default(T);
            }
		}

        public static IMongoCollection<T> GetCollection<T>(string collection)
        {
            return database.GetCollection<T>(collection);
        }
    }
}
