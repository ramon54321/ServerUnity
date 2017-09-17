using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NetworkSystem
{
    public abstract class MessageProcessor
    {
        public virtual void ProcessMessage(string message, IPEndPoint remoteEndpoint)
        {

        }
    }
}
