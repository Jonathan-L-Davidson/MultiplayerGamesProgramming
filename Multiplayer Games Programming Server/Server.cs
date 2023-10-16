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

        ConcurrentDictionary<int, ConnectedClient> m_Clients;

        public Server(string ipAddress, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            m_TcpListener = new TcpListener(ip, port);


        }

        public void Start()
        {
            try
            {
                m_TcpListener.Start();
                Console.WriteLine($"Server started on {m_TcpListener.LocalEndpoint}");

                Socket socket = m_TcpListener.AcceptSocket();
                Console.WriteLine("Connection made");

                ClientMethod(socket);
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

        private void ClientMethod(Socket index)
        {
            try
            {
                string message;
                NetworkStream stream = new NetworkStream(index, false);
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

                while ((message = reader.ReadLine()) != null)
                {
                    Console.WriteLine($"Recieved: {message}");

                    writer.WriteLine("Hello world!");
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                index.Close();
            }
        }
    }
}
