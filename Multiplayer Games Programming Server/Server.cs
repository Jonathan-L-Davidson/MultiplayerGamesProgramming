using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using Multiplayer_Games_Programming_Packet_Library;
using System.Text;
using System.Linq;
using System.Numerics;

namespace Multiplayer_Games_Programming_Server
{
    internal class Server
    {
        
        TcpListener m_TcpListener;
        int m_port;
        IPAddress m_ipAddress;

        int m_maxClients = 10;

        ConcurrentDictionary<int, ConnectedClient> m_Clients;

        List<GameObject> m_gameObjects;

        public Server(string ipAddress, int port)
        {
            m_ipAddress = IPAddress.Parse(ipAddress);
            m_port = port;
            
            m_TcpListener = new TcpListener(m_ipAddress, m_port);

            m_Clients = new();
            m_gameObjects = new();
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
                    clientThread.Name = $"Client {newID}";
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
            client.SetID((int)index);
            if(client == null) { return; }

            client.Send(new NETPlayerLogin(client.GetID()));

            while (client.active)
            {
                string recieved = client.Read();
                if(recieved != null && recieved.Length > 0)
                {
                    Console.WriteLine($"Recieved Packet: {recieved}");
                    Packet? packet = Packet.Deserialise(recieved);
                    if (packet != null)
                    {
                        HandlePacket(packet);
                    }
                }

            }

            m_Clients.Remove(client.GetID(), out client);
            client?.Close();

        }

        private void HandlePacket(Packet packet)
        {
            if (packet == null) { return; };

            switch(packet.m_type)
            {
                case PacketType.MSG:
                    NETMessage msg = (NETMessage)packet;
                    msg.PrintMessage();
                    break;
                case PacketType.NULL:
                    break;
                case PacketType.PLAYERMOVE:
                    NETPlayerMove move = (NETPlayerMove)packet;
                    RelayPacket(move, move.playerID);
                    break;
                case PacketType.PLAYERPLAY:
                    NETPlayerPlay start = (NETPlayerPlay)packet;
                    ConnectedClient client = m_Clients[start.playerID];
                    if (client.StartGame(start) == true)
                    {
                        SceneSync(m_Clients[start.playerID]);
                    }
                    RelayPacket(new NETPlayerCreate(client.GetData()), start.playerID); // TODO create netplayercreate again, make an new handler
                    break;
                case PacketType.PLAYERUPDATE:
                    NETPlayerUpdate update = (NETPlayerUpdate)packet;
                    RelayPacket(update);
                    break;
                default:
                    Console.WriteLine($"ERROR: PacketType missing, was: {packet.m_type}");
                    break;

            }
        }

        /// <summary>
        /// Relays the packet to all connected clients.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="originID">Pass the client ID if you do not want to send to that client.</param>
        /// <param name="activePlayers">Boolean to only send to players who are playing.</param>
        private void RelayPacket(Packet packet, int originID = -1, bool activePlayers = false)
        {
            if (packet == null) { return;}

            foreach(ConnectedClient client in m_Clients.Values)
            {
                if(client.GetID() == originID)
                {
                    continue;
                }

                if (activePlayers)
                {
                    if (!client.IsPlaying()) // If the player is not playing.
                    {
                        continue;
                    }
                }

                client.Send(packet);
            }
        }

        private void UpdateObject(NETObjectUpdate updatePacket)
        {
            GameObject? gameObj = m_gameObjects[updatePacket.data.objID];

            if (gameObj == null)
            {
                gameObj = CreateObject(updatePacket);
            }

            gameObj.UpdateData(updatePacket.data);
            RelayPacket(updatePacket);
        }

        private GameObject CreateObject(NETObjectUpdate updatePacket)
        {
            GameObject obj = new GameObject();
            obj.UpdateData(updatePacket.data);

            m_gameObjects.Add(obj);

            return obj;
        }

        private void SceneSync(ConnectedClient client)
        {
            if (m_gameObjects.Count > 0)
            {
                lock (m_gameObjects) {
                    foreach (GameObject obj in m_gameObjects)
                    {
                        NETObjectUpdate updateObj = new NETObjectUpdate(obj.GetData());
                        client.Send(updateObj);
                    }
                }
            }
            lock (m_Clients) {
                foreach(ConnectedClient connectedClient in m_Clients.Values)
                {
                    if (connectedClient.IsPlaying())
                    {
                        NETPlayerUpdate updatePlayer = new NETPlayerUpdate(connectedClient.GetData());
                        client.Send(updatePlayer);
                    }
                }
            }
        }
    }
}
