using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Server
{
    internal class ConnectedClient
    {
        Socket m_socket;
        NetworkStream m_netStream;
        StreamReader m_netReader;
        StreamWriter m_netWriter;

        public int id { get; set; }

        public bool active { get; private set; } = true;

        public ConnectedClient(object socket)
		{
            if (socket == null) { throw new ArgumentNullException("socket missing"); }

            m_socket = (Socket)socket;

            m_netStream = new NetworkStream(m_socket, false);
            m_netReader = new StreamReader(m_netStream, Encoding.UTF8);
            m_netWriter = new StreamWriter(m_netStream, Encoding.UTF8);

        }

        public void Close()
		{
            m_socket.Close();
            m_netStream.Close();
            m_netReader.Close();
            m_netWriter.Close();
        }

		public string Read()
		{
            try
            {
                string message;

                while ((message = m_netReader.ReadLine()) != null)
                {
                    return message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "";

        }

		public void Send(Packet packet)
		{
            string data = PreparePacket(packet);
            m_netWriter.WriteLine(data);
            m_netWriter.Flush();
		}

        private string PreparePacket(Packet packet)
        {
            return packet.ToJson();
        }
	}
}
