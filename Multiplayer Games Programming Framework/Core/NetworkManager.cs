using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Multiplayer_Games_Programming_Packet_Library;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Multiplayer_Games_Programming_Framework.Core
{
	internal class NetworkManager
	{
		private static NetworkManager Instance;

		public static NetworkManager m_Instance
		{
			get
			{
				if (Instance == null)
				{
					return Instance = new NetworkManager();
				}
			
				return Instance;
			}
		}


		NetworkManager()
		{
		}

		public bool Connect(string ip, int port)
		{
			return false;
		}

		public void Run()
		{
		}

		private void TcpProcessServerResponse()
		{
		}

		public void TCPSendMessage(string message)
		{
		}

		public void Login()
		{
		}
	}
}
