using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerConsole
{
    public class ClientManager
    {
        private readonly int MAX_CLIENTS;

        public ClientManager(int maxClients)
        {
            this.MAX_CLIENTS = maxClients;
        }

        private Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public bool AddClient(Client clientData)
        {
            if (clients.Count >= MAX_CLIENTS)
                return false;
            clients.Add(clientData.ClientId, clientData);
            return true;
        }

        public void RemoveClient(int clientId)
        {
            clients.Remove(clientId);
        }

        public List<Client> GetClients()
        {
            List<Client> list = new List<Client>(clients.Values);
            return list;
        }

        public Client GetClient(int clientId)
        {
            Client clientData = null;
            clients.TryGetValue(clientId, out clientData);
            return clientData;
        }

        public Client GetClientByEntityId(int entityId)
        {
            foreach(Client client in clients.Values)
            {
                if(client.Entity.id == entityId)
                    return client;
            }
            return null;
        }

        public bool SendMessageToClient(int clientId, string message)
        {
            Client clientData = GetClient(clientId);

            // -- Check if client exists
            if (clientData == null)
                return false;

            clientData.SendMessage(message);
            return true;
        }

        public void SendMessageToAllClients(string message)
        {
            foreach (Client clientData in clients.Values)
            {
                clientData.SendMessage(message);
            }
        }

        public void SendMessageToAllButOneClient(int clientIdToIgnore, string message)
        {
            foreach (Client clientData in clients.Values)
            {
                // -- Check for client to skip
                if (clientData.ClientId == clientIdToIgnore)
                    continue;

                // -- Else send message
                clientData.SendMessage(message);
            }
        }
    }
}
