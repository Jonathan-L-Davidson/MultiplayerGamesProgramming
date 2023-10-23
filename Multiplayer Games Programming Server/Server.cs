using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using Multiplayer_Games_Programming_Packet_Library;
using System.Text;

namespace Multiplayer_Games_Programming_Server
{
    internal class Server
    {
        
        TcpListener m_TcpListener;
        int m_port;
        IPAddress m_ipAddress;

        int m_maxClients = 10;

        ConcurrentDictionary<int, ConnectedClient> m_Clients;

        public Server(string ipAddress, int port)
        {
            m_ipAddress = IPAddress.Parse(ipAddress);
            m_port = port;
            
            m_TcpListener = new TcpListener(m_ipAddress, m_port);

            m_Clients = new ConcurrentDictionary<int, ConnectedClient>();

        }

        public void Start()
        {
            try
            {
                m_TcpListener.Start();
                Console.WriteLine($"Server started on {m_TcpListener.LocalEndpoint}");
                bool active = true;

                while (active)
                {
                    Socket socket = m_TcpListener.AcceptSocket();
                    Console.WriteLine($"Connection made from {socket.RemoteEndPoint}");

                    int newID = 0;
                    ConnectedClient client = new ConnectedClient(socket);
                    
                    // There isn't a case where client isn't added to this list and continues. Atleast I hope so, we can assume lines after this will continue.
                    while (!m_Clients.TryAdd(newID, client))
                    {
                        if(newID > m_maxClients)
                        {
                            client.Close();
                            return;
                        }
                        newID++;
                    }

                    Thread clientThread = new Thread(new ParameterizedThreadStart(ClientMethod));
                    clientThread.Start(newID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            m_TcpListener?.Stop();
        }

        private void ClientMethod(object index)
        {

            ConnectedClient? client = m_Clients.ElementAt((int)index).Value;
            client.id = (int)index;
            if(client == null) { return; }

            while (client.active)
            {
                client.Read();
            }

            client.Close();

        }
    }
}
