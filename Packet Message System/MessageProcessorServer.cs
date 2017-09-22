using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ToyArmyServer
{
    class MessageProcessorServer : NetworkSystem.MessageProcessor
    {
        public override void ProcessMessage(string message, IPEndPoint remoteEndpoint)
        {
            /* --- SERVER SIDE
             * /connectrequest/
             * /disconnectrequest/
             */

            int clientId = Client.GetClientIdFromIpEndpoint(remoteEndpoint);

            string[] segments = message.Split('/');

            // -- Connecting and Disconnecting

            if (segments[1] == "connectrequest")
            {
                // -- Check if client exists
                if (Program.clientManager.GetClient(clientId) != null)
                {
                    Program.clientManager.SendMessageToClient(clientId, "/connectrequestdenied/");

                    Console.WriteLine("Client attempted to connect but was already connected.");
                    return;
                }

                string username = segments[2];
                string password = segments[3];

                // -- Check if client has user in database
                List<User> users = DatabaseManager.usersCollection.Find(obj => obj.Username == username).ToList();
                // -- Null check query
                if (users.Count == 0)
                {
                    Program.clientManager.SendMessageToClient(clientId, "/connectrequestdenied/");

                    Console.WriteLine("Client connected with incorrect username.");
                    return;
                }
                // -- Compare passwords
                if (password != users[0].Password)
                {
                    Program.clientManager.SendMessageToClient(clientId, "/connectrequestdenied/");

                    Console.WriteLine("Client connected with incorrect password.");
                    return;
                }

                // -- Create new client data and add to list
                Client clientData = new Client(clientId, users[0], DatabaseManager.GetAgentFromUser(users[0]), remoteEndpoint.Address, remoteEndpoint.Port);
                Program.clientManager.AddClient(clientData);

                // -- Create new entity for client and link to client data
                Entity entity = Program.gameManager.entityManager.CreateNewEntityWithAutoId(EntityType.Player);
                clientData.Entity = entity;

                // -- Tell client connection is accepted + their entity id
                Agent agent = DatabaseManager.GetAgentFromUser(users[0]);
                Program.clientManager.SendMessageToClient(clientId, "/connectrequestaccepted/" + entity.id + "/" + agent.GetInventoryJson() + "/");

                // -- Send client data of all other clients
                /*
                 * For each client
                 *  If sender client -> Ignore
                 *  Else
                 *      Send - Client.entity.id
                 *      Send - Data update (Client.entity.id)
                 */
                foreach (Client clientDataIt in Program.clientManager.GetClients())
                {
                    if (clientDataIt == clientData)
                        continue;

                    // -- Send the info of other client on server to newly connected client
                    Program.clientManager.SendMessageToClient(clientId, "/spawnentity/" + clientDataIt.Entity.id + "/" + clientDataIt.Entity.EntityType + "/");
                }

                // -- Tell other clients about new client
                //Program.clientManager.SendMessageToAllButOneClient(clientId, "/newclient/" + entity.id + "/");

                Console.WriteLine("Client connected and entity data created.");
            }
            else if (segments[1] == "disconnectrequest")
            {
                // -- Check if client does NOT exist
                if (Program.clientManager.GetClient(clientId) == null)
                {
                    Program.clientManager.SendMessageToClient(clientId, "/disconnectrequestdenied/");

                    Console.WriteLine("Client tried to disconnect, but does not exist on server.");

                    return;
                }

                Program.clientManager.SendMessageToClient(clientId, "/disconnectrequestaccepted/");

                // -- Get client data
                Client clientData = Program.clientManager.GetClient(clientId);

                // -- Tell other clients about client disconnect
                Program.clientManager.SendMessageToAllButOneClient(clientId, "/clientdisconnected/" + clientData.Entity.id + "/");

                // -- Clear local data
                clientData.DeleteLocalData();

                Console.WriteLine("Client disconnected.");
            }
            else if (segments[1] == "invalidnewclient")
            {
                Console.WriteLine("Client received invalid new client data. (Check connectrequest processing.)");
            }
            else if (segments[1] == "invalidclientspawn")
            {
                Console.WriteLine("Client received invalid client data for existing server client. (Check connectrequest processing.)");
            }
            else if (segments[1] == "invalidclientdisconnect")
            {
                Console.WriteLine("Client received invalid client disconnect.");
            }

            // -- Kicking and Timeout

            else if (segments[1] == "pingresponse")
            {
                // -- Check if client does NOT exist
                if (Program.clientManager.GetClient(clientId) == null)
                {
                    Program.clientManager.SendMessageToClient(clientId, "/nonrecognizedclient/");

                    Console.WriteLine("Client tried to send message, but does not exist on server.");

                    return;
                }

                Program.clientManager.GetClient(clientId).ResetPingsSent();
            }

            // -- Entity data

            else if (segments[1] == "invalidentityid")
            {
                Console.WriteLine("Client received invalid entity id.");
            }
            else if (segments[1] == "entitydata")
            {
                int id = 0;
                if (!Int32.TryParse(segments[2], out id))
                {
                    // -- Failed to parse id, send invalid flag
                    Console.WriteLine("Cant parse issued entity id.");
                }
                float posX = float.Parse(segments[3]);
                float posY = float.Parse(segments[4]);
                float rot = float.Parse(segments[5]);

                Entity entity = Program.gameManager.entityManager.GetEntityById(id);
                if(entity != null)
                {
                    entity.SetPosition(posX, posY);
                    entity.rotation = rot;
                }
            }
            else if (segments[1] == "entityinforequest")
            {
                int id = 0;
                if (!Int32.TryParse(segments[2], out id))
                {
                    // -- Failed to parse id, send invalid flag
                    Console.WriteLine("Cant parse issued entity id.");
                }
                Entity e = Program.gameManager.entityManager.GetEntityById(id);
                Program.clientManager.GetClient(clientId).SendMessage("/spawnentity/" + e.id + "/" + e.EntityType + "/");
            }
            else if (segments[1] == "entityhit")
            {
                Console.WriteLine("Damaging entity.");

                int id = 0;
                if (!Int32.TryParse(segments[2], out id))
                {
                    // -- Failed to parse id, send invalid flag
                    Console.WriteLine("Cant parse issued entity id.");
                }
                float damage = float.Parse(segments[3]);

                Entity entity = Program.gameManager.entityManager.GetEntityById(id);
                if(entity != null)
                    entity.entityHealthSystem.Damage(damage);
            }

            // -- Effects

            else if (segments[1] == "effect")
            {
                Program.clientManager.SendMessageToAllButOneClient(clientId, message);
            }

            // -- Inventory

            else if (segments[1] == "setinventory")
            {
                Client client = Program.clientManager.GetClient(clientId);
                Console.WriteLine("Setting inventory in database for " + client.data_User.Username);

                client.data_Agent.SetInventoryFromJson(segments[2]);

                client.SendMessage("/setinventory/" + client.data_Agent.GetInventoryJson() + "/");

                // -- Update client primary weapon
                client.SendMessage("/setprimary/" + client.data_Agent.PrimaryWeaponInstanceId + "/" + client.data_Agent.PrimaryMagazineInstanceId + "/");
            }
            else if (segments[1] == "itemtransfer")
            {
                int index = 0;
                if (!Int32.TryParse(segments[2], out index))
                {
                    // -- Failed to parse id, send invalid flag
                    Console.WriteLine("Cant parse issued item index.");
                    return;
                }
                int entityId = 0;
                if (!Int32.TryParse(segments[3], out entityId))
                {
                    // -- Failed to parse id, send invalid flag
                    Console.WriteLine("Cant parse issued entity id.");
                    return;
                }

                Client client = Program.clientManager.GetClient(clientId);
                Client clientTarget = Program.clientManager.GetClientByEntityId(entityId);
                if(clientTarget == null)
                {
                    Console.WriteLine("Cant find entity with given id. No transfer happening.");
                    return;
                }

                Console.WriteLine("Transferring item from " + client.data_User.Username + " to " + clientTarget.data_User.Username);

                // -- Transfer from client to target
                client.data_Agent.TransferItem(index, clientTarget.data_User.Username);

                // -- Update client inventory
                client.SendMessage("/setinventory/" + client.data_Agent.GetInventoryJson() + "/");

                // -- Update Target client inventory
                clientTarget.SendMessage("/setinventory/" + clientTarget.data_Agent.GetInventoryJson() + "/");
            }
            else if (segments[1] == "setprimaryweapon")
            {
                Client client = Program.clientManager.GetClient(clientId);
                Console.WriteLine("Setting primary weapon in database for " + client.data_User.Username);

                client.data_Agent.PrimaryWeaponInstanceId = segments[2];
                client.data_Agent.Save();
            }
            else if (segments[1] == "setprimarymagazine")
            {
                Client client = Program.clientManager.GetClient(clientId);
                Console.WriteLine("Setting primary magazine in database for " + client.data_User.Username);

                client.data_Agent.PrimaryMagazineInstanceId = segments[2];
                client.data_Agent.Save();
            }
        }
    }
}
