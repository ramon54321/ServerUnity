using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace ServerConsole
{   
    public delegate void ClientSwitched();

    public class ChunkTracker
    {
        // -- This event fires when any client changes chunk
        public static event ClientSwitched ClientSwitchEvent;

        public IDictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();

        public bool SwitchClient(ClientData clientData, string fromChunkString, string toChunkString)
        {
            if(clientData == null)
                return false;

            bool removeClient = GetChunk(fromChunkString).RemoveClient(clientData);
            bool addClient = GetChunk(toChunkString).AddClient(clientData);

            if(!removeClient || !addClient)
                return false;

            ClientSwitchEvent();

            return true;
        }

        public Chunk GetChunk(string chunk)
        {
            // -- If there is no chunk in the position, make one
            if(!chunks.ContainsKey(chunk))
            {
                chunks.Add(chunk, new Chunk());
            }

            return chunks[chunk];
        }

        public Chunk GetChunk(int x, int y)
        {
            return GetChunk(x + "_" + y);
        }
    }

    public class Chunk
    {
        private List<ClientData> clientsInChunk = new List<ClientData>();

        public bool AddClient(ClientData clientData)
        {
            if(clientData == null)
                return false;
            
            clientsInChunk.Add(clientData);
            return true;
        }

        public bool RemoveClient(ClientData clientData)
        {
            if(clientData == null)
                return false;

            clientsInChunk.Remove(clientData);
            return true;
        }

        public List<ClientData> GetClientsInChunk()
        {
            return this.clientsInChunk;
        }
    }
}