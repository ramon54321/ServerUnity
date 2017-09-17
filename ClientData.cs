using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ServerConsole
{
    public class ClientData
    {
        public static int GetClientIdFromIpEndpoint(IPEndPoint ipEndpoint)
        {
            return ipEndpoint.Address.GetHashCode() + ipEndpoint.Port.GetHashCode();
        }

        public int ClientId { get; }
        public Data_User data_User { get; }
        public Data_Agent data_Agent { get; }
        public IPAddress IpAddress { get; }
        public int Port { get; }
        private Entity _entity;
        public Entity Entity { get { return this._entity; } set{_entity = value; _entity.clientData = this; } }

        private int pingsSent = 0;

        public void IncrementPingsSent()
        {
            pingsSent++;
        }

        public void ResetPingsSent()
        {
            pingsSent = 0;
        }

        public int GetPingsSent()
        {
            return pingsSent;
        }

        public ClientData(int clientId, Data_User data_User, Data_Agent data_Agent, IPAddress ipAddress, int port)
        {
            this.ClientId = clientId;
            this.data_User = data_User;
            this.data_Agent = data_Agent;
            this.IpAddress = ipAddress;
            this.Port = port;
        }
        
        public void KickClient()
        {
            Console.WriteLine("Client " + ClientId + " kicked from server.");

            // -- Send kick signal
            SendMessage("/kick/");

            // -- Tell other clients client disconnected
            Program.clientManager.SendMessageToAllButOneClient(ClientId, "/clientdisconnected/" + Entity.id + "/");

            // -- Remove client entity
            DeleteLocalData();
        }

        public void DeleteLocalData()
        {
            // -- Remove client entity
            Program.gameManager.entityManager.RemoveEntity(Entity.id);

            // -- Remove client
            Program.clientManager.RemoveClient(ClientId);

            Console.WriteLine("Client local data removed.");
        }

        public void SendMessage(string message)
        {
            Program.networkMessenger.SendMessage(IpAddress, Port, message);
        }
    }
}
