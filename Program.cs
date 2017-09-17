using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetworkSystem;
using System.Threading;

namespace ServerConsole
{
    public class Program
    {
        public static readonly float POSITION_FREQUENCY = 10; // Frequency of updates in hertz CHANGE ON CLIENT ALSO

        public static NetworkMessenger networkMessenger;
        public static ClientManager clientManager;
        public static MessageProcessor messageProcessor;
        public static GameManager gameManager;
        public static ChunkTracker ChunkTracker;
        public static WebServer webServer;
        public static DatabaseManager databaseManager;

        public static void Main(string[] args)
        {
            // -- Start database connection
            databaseManager = new DatabaseManager();

            // -- Run web server
            webServer = new WebServer();

            // -- Set up debugging
            NetworkManager.isUnity = false;

            // -- Set up network messenger
            networkMessenger = NetworkMessenger.GetInstance();

            // -- Set up client manager
            clientManager = new ClientManager(4);

            // -- Set up message processor
            messageProcessor = new MessageProcessorServer();

            // -- Set up game manager
            gameManager = new GameManager();

            // -- Open local socket for sending and receiving
            networkMessenger.OpenClient(11999);

            // -- Start listening
            networkMessenger.Listen(IPAddress.Any, messageProcessor);

            // -- Build server world
            BuildServerWorld();

            // -- Start ping
            Ping();

            // -- Start update
            Update();

            // -- Wait for exit command
            CommandLoop();

            // -- Close all
            networkMessenger.Close();
        }

        private static void BuildServerWorld()
        {
            gameManager.entityManager.CreateNewEntityWithAutoId(new Entity());
        }

        private static void CommandLoop()
        {
            while (true)
            {
                string command = Console.ReadLine();

                if (command == "")
                {
                    webServer.Close();
                    Environment.Exit(1);
                    return;
                }

                if (command == "kickall")
                {
                    // -- Kick all clients
                    foreach (ClientData clientData in clientManager.GetClients())
                    {
                        clientData.KickClient();
                    }
                    continue;
                }

                // -- Assume raw message
                string[] segments = command.Split(':');
                if(segments.Length > 1)
                {
                    int port = 0;
                    Int32.TryParse(segments[0], out port);

                    networkMessenger.SendMessage(IPAddress.Parse("127.0.0.1"), port, segments[1]);
                }
            }
        }

        private static void Ping()
        {
            Thread pingThread = new Thread(delegate ()
            {
                Console.WriteLine("Pinging thread started.");

                while (true)
                {
                    Thread.Sleep(1000);
                    clientManager.SendMessageToAllClients("/ping/");

                    foreach(ClientData clientData in clientManager.GetClients())
                    {
                        clientData.IncrementPingsSent();

                        if(clientData.GetPingsSent() > 2)
                        {
                            clientData.KickClient();
                        }
                    }
                }
            });
            pingThread.Start();
        }

        private static void Update()
        {
            Thread updateThread = new Thread(delegate ()
            {
                Console.WriteLine("Update thread started.");

                while (true)
                {
                    Thread.Sleep((int) (1000 / POSITION_FREQUENCY));

                    /*
                     * Foreach ClientA
                     *      Send all entity data
                     *      Foreach ClientB
                     *          If ClientB is ClientA
                     *              Ignore
                     *          Send to ClientA data of ClientB.entity
                     */

                    foreach (Entity entity in gameManager.entityManager.GetEntities())
                    {
                        entity.UpdateChunk();
                    }

                    List<ClientData> clients = clientManager.GetClients();

                    foreach (ClientData clientDataA in clients)
                    {
                        IList<Entity> entitiesInClientRange = gameManager.entityManager.GetEntitiesInSurroundingChunks(clientDataA);

                        foreach (Entity e in entitiesInClientRange)
                        {
                            ClientData clientDataB = e.clientData;
                            if(clientDataB != null)
                            {
                                if (clientDataB == clientDataA)
                                    continue;
                            }

                            clientManager.SendMessageToClient(clientDataA.ClientId, "/entitydata/" + e.id + "/" + e.position.x + "/" + e.position.y + "/" + e.rotation + "/");
                        }
                    }
                }
            });
            updateThread.Start();
        }
    }
}
