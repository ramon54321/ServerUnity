using System;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ServerConsole
{
    public class Data_User
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

		public Data_User()
		{
	        Id = ObjectId.GenerateNewId();
	    }

        public Data_Agent AddAgentToUser()
        {
            Data_Agent currentAgent = DatabaseManager.GetAgentFromUser(this);
            if(currentAgent != null)
                return currentAgent;
            Data_Agent agent = new Data_Agent(){ User = DatabaseManager.GetReferenceToObject("Users", Id) };
            DatabaseManager.agentsCollection.InsertOne(agent);
            return agent;
        }

        public static Data_User GetUserByUsername(string username)
        {
            return DatabaseManager.usersCollection.Find(userx => userx.Username == username).ToCursor().First();
        } 
    }
}
