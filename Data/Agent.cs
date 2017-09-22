using System;
using System.Text;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ToyArmyServer
{
    public class Agent
    {
        public ObjectId Id { get; set; }
        public MongoDBRef User { get; set; }
        public List<GameItems.GameItemInstance> Inventory { get; set; }
        public string PrimaryWeaponInstanceId { get; set; }
        public string PrimaryMagazineInstanceId { get; set; }

		public Agent()
		{
	        Id = ObjectId.GenerateNewId();
            Inventory = new List<GameItems.GameItemInstance>();
        }

        // -- Adds a new item to inventory, with a new ID (Spawns a brand new item)
        public void SpawnItem(string systemName)
        {
            // -- Update self
            Update();

            // -- Unique values will be as set in classes
            Inventory.Add(DatabaseManager.JsonToObject<GameItems.GameItemInstance>(GameItems.GameItemInstance.GetNewItemInstance(systemName).ToJson()));
            Save();
        }

        // -- Removes the item from the inventory without transferring it (Destroys the item permanently)
        public bool DestroyItem(int index)
        {
            // -- Update self
            Update();

            if(Inventory.Count <= index)
                return false;

            Inventory.RemoveAt(index);
            Save();
            return true;
        }

        // -- Transfers the instance to another agent, removing it from this agent and adding to another
        public bool TransferItem(int index, string usernameReceiver)
        {
            // -- Check if receiver is not current owner
            if(DatabaseManager.GetUserFromAgent(this).Username == usernameReceiver)
            {
                Console.WriteLine("Sender and receiver of item the same. No transfer happening.");
                return false;
            }

            // -- Update self
            Update();

            if(Inventory.Count <= index)
            {
                Console.WriteLine("No item found at index. No transfer happening.");
                return false;
            }

            GameItems.GameItemInstance instance = Inventory[index];

            Agent agent = DatabaseManager.GetAgentFromUser(DatabaseManager.usersCollection.Find(usr => usr.Username == usernameReceiver).ToCursor().First());

            Console.WriteLine("TargetName: " + usernameReceiver);
            Console.WriteLine("TargetActualName: " + DatabaseManager.GetUserFromAgent(agent).Username);

            agent.Inventory.Add(Inventory[index]);
            agent.Save();
            Console.WriteLine("TargetI: " + agent.GetInventoryJson());

            Inventory.RemoveAt(index);
            Save();
            Console.WriteLine("OwnI: " + GetInventoryJson());

            return true;
        }

        public void SetInventoryFromJson(string json)
        {
            if(json == "")
                return;

            List<GameItems.GameItem> items = new List<GameItems.GameItem>();
            items = DatabaseManager.JsonArrayToList<GameItems.GameItem>(json);
            
            Inventory.Clear();

            StringBuilder sb = new StringBuilder();
            foreach(GameItems.GameItem item in items)
            {
               sb.Append(item.GetItemInstanceJson());
               sb.Append("~");
            }
            sb.Remove(sb.Length-1, 1);

            Inventory = DatabaseManager.JsonArrayToList<GameItems.GameItemInstance>(sb.ToString());

            Save();
        }

        public string GetInventoryJson()
        {
            // -- Update self
            Update();

            if(Inventory.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach(GameItems.GameItemInstance gii in Inventory)
            {
               sb.Append(gii.GetItemJson());
               sb.Append("~");
            }
            sb.Remove(sb.Length-1, 1);
            return sb.ToString();
        }

        public void Update()
        {
            Inventory = DatabaseManager.agentsCollection.Find(agt => agt.Id == Id).ToCursor().First().Inventory;
        }

        public void Save()
		{
			DatabaseManager.agentsCollection.ReplaceOne(obj => obj.Id == this.Id, this);
		}
    }
}
