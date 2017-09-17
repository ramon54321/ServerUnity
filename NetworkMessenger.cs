using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkSystem
{
    public class NetworkMessenger
    {
        private static NetworkMessenger networkMessenger;

        public static NetworkMessenger GetInstance()
        {
            if (networkMessenger == null)
                networkMessenger = new NetworkMessenger();
            return networkMessenger;
        }

        private NetworkMessenger(){}

        private Thread listenerThread;
        private UdpClient udpClient;

        public void OpenClient(int port)
        {
            udpClient = new UdpClient(port);
        }

        public void Close()
        {
            try
            {
                NetworkManager.Log("Closing.");
                //listenerThread.Abort();
                udpClient.Close();
            }
            catch (Exception e)
            {
                NetworkManager.Log("Closing Failed!");
                NetworkManager.Log("Handled: " + e.ToString());
            }
        }

        public void Listen(IPAddress ipAddress, MessageProcessor messageProcessor)
        {
            listenerThread = new Thread(delegate ()
            {

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                string receivedData;
                byte[] receiveByteArray;
                try
                {
                    NetworkManager.Log("Listening on port " + ((IPEndPoint) udpClient.Client.LocalEndPoint).Port);
                    while (true)
                    {
                        receiveByteArray = udpClient.Receive(ref remoteEndPoint);

                        //NetworkManager.Log(remoteEndPoint.Address.ToString() + " -- " + remoteEndPoint.Port.ToString());

                        receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);

                        //NetworkManager.Log("Received: " + receivedData);

                        messageProcessor.ProcessMessage(receivedData, remoteEndPoint);
                    }
                }
                catch (Exception e)
                {
                    NetworkManager.Log("Handled: " + e.ToString());
                }
                finally
                {
                    try
                    {
                        udpClient.Dispose();
                    }
                    catch (Exception e)
                    {
                        NetworkManager.Log("Handled: " + e.ToString());
                    }
                }
            });
            listenerThread.Start();
        }

        public void SendMessage(IPAddress ipAddress, int port, string message)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);

            byte[] sendBuffer = Encoding.ASCII.GetBytes(message);
            try
            {
                udpClient.Send(sendBuffer, sendBuffer.Length, remoteEndPoint);
                //udpClient.se
            }
            catch (Exception e)
            {
                NetworkManager.Log("Handled: " + e.ToString());
            }
        }
    }
}
