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

        private Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();

        public bool AddClient(ClientData clientData)
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

        public List<ClientData> GetClients()
        {
            List<ClientData> list = new List<ClientData>(clients.Values);
            return list;
        }

        public ClientData GetClient(int clientId)
        {
            ClientData clientData = null;
            clients.TryGetValue(clientId, out clientData);
            return clientData;
        }

        public ClientData GetClientByEntityId(int entityId)
        {
            foreach(ClientData client in clients.Values)
            {
                if(client.Entity.id == entityId)
                    return client;
            }
            return null;
        }

        public bool SendMessageToClient(int clientId, string message)
        {
            ClientData clientData = GetClient(clientId);

            // -- Check if client exists
            if (clientData == null)
                return false;

            clientData.SendMessage(message);
            return true;
        }

        public void SendMessageToAllClients(string message)
        {
            foreach (ClientData clientData in clients.Values)
            {
                clientData.SendMessage(message);
            }
        }

        public void SendMessageToAllButOneClient(int clientIdToIgnore, string message)
        {
            foreach (ClientData clientData in clients.Values)
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
