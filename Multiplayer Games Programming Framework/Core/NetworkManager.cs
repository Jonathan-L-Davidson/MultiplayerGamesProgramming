using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Multiplayer_Games_Programming_Packet_Library;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Linq.Expressions;

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

		TcpClient m_tcpClient;
		NetworkStream m_netStream;
		StreamReader m_netReader;
		StreamWriter m_netWriter;

		NetworkManager()
		{
			m_tcpClient = new TcpClient();
		}

		public bool Connect(string ip, int port)
		{
			try
			{
				m_tcpClient.Connect(ip, port);
				m_netStream = m_tcpClient.GetStream();
				m_netReader = new StreamReader(m_netStream, Encoding.UTF8);
				m_netWriter = new StreamWriter(m_netStream, Encoding.UTF8);

				Run();
				return true;
			} catch(Exception e)
			{
                Debug.WriteLine($"Error while connecting: {e.Message}");
			}

			return false;
		}

		public void Run()
		{
			Thread TcpThread = new Thread(new ThreadStart(TcpProcessServerResponse));
			TcpThread.Name = "TCP NetHandler";
			TcpThread.Start();
		}

		private void TcpProcessServerResponse()
		{
			try
			{
				while (m_tcpClient.Connected)
				{
					string msg = m_netReader.ReadLine();
					Debug.WriteLine($"Message recieved: {msg}");

				}
			}
			catch (Exception e)
			{
                Debug.WriteLine($"TCP Process Error: {e.Message}");
			}
		}

		public void TCPSendMessage(string message)
		{
			m_netWriter.WriteLine(message);
			m_netWriter.Flush();
		}

		public void Login()
		{
			TCPSendMessage("Login");
		}
	}
}
