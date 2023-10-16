using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Server
{
	internal class Server
	{
		TcpListener m_TcpListener;

		ConcurrentDictionary<int, ConnectedClient> m_Clients;

		public Server(string ipAddress, int port)
		{
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}

		private void ClientMethod(int index)
		{
		}
	}
}
