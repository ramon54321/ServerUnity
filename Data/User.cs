using System;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ToyArmyServer
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

		public User()
		{
	        Id = ObjectId.GenerateNewId();
	    }

        public Agent AddAgentToUser()
        {
            Agent currentAgent = DatabaseManager.GetAgentFromUser(this);
            if(currentAgent != null)
                return currentAgent;
            Agent agent = new Agent(){ User = DatabaseManager.GetReferenceToObject("Users", Id) };
            DatabaseManager.agentsCollection.InsertOne(agent);
            return agent;
        }

        public static User GetUserByUsername(string username)
        {
            return DatabaseManager.usersCollection.Find(userx => userx.Username == username).ToCursor().First();
        } 
    }
}
