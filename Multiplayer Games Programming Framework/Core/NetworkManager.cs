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
using Myra;
using System.Linq;
using Multiplayer_Games_Programming_Framework.GameCode.Components.Player;

namespace Multiplayer_Games_Programming_Framework;

internal class NetworkManager
{
	private static NetworkManager Instance;

	public Scene activeScene;
	public int playerID = -1;

	public Dictionary<int, PlayerNetwork> players = new Dictionary<int, PlayerNetwork>();

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
		}
		catch (Exception e)
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
				Packet packet = DeserialisePacket(msg);

				HandlePacket(packet);

			}
		}
		catch (Exception e)
		{
			Debug.WriteLine($"TCP Process Error: {e.Message}");
		}
	}

	public void TCPSendMessage(Packet packet)
	{
		m_netWriter.WriteLine(packet.ToJson());
		m_netWriter.Flush();

	}

	private Packet DeserialisePacket(string msg)
	{
		return Packet.Deserialise(msg);
	}

	private void HandlePacket(Packet packet)
	{
		switch (packet.m_type)
		{
			case (PacketType.OBJUPDATE):

				break;
			case (PacketType.PLAYERLOGIN):
				NETPlayerLogin loginPacket = (NETPlayerLogin)packet;
				HandleLogin(loginPacket);
				break;
            case (PacketType.PLAYERPLAY):
                NETPlayerList playPacket = (NETPlayerList)playPacket;
                HandlePlayerJoin(playPacket);
                break;
            case (PacketType.PLAYERLIST):
				NETPlayerList listPacket = (NETPlayerList)packet;
				InitPlayerList(listPacket);
				break;
			case (PacketType.PLAYERMOVE):
				NETPlayerMove movePacket = (NETPlayerMove)packet;
				HandePlayerMovement(movePacket);
				break;
		}
	}

	public void Login()
	{
		NETMessage message = new NETMessage($"Hello server!!");
		TCPSendMessage(message);


	}

	private void HandleLogin(NETPlayerLogin loginPacket)
	{
		this.playerID = loginPacket.playerID;
	}

	private void HandePlayerMovement(NETPlayerMove movePacket)
	{
		PlayerNetwork playerRef = players[movePacket.playerID];
		if (playerRef != null)
		{
			playerRef.playerInput = new Vector2(movePacket.x, movePacket.y);
		}
	}


	private void InitPlayerList(NETPlayerList playerList)
	{
		if (playerList.playerID != playerID)
		{
			return; // Error! This isn't for us.
		}

		foreach (PlayerData player in playerList.players)
		{
			Transform pos = new Transform(new Vector2(player.x, player.y));
			CreateNetworkPlayer(player.playerID, pos, player);
		}
	}

	private void CreateNetworkPlayer(int id, Transform pos, PlayerData playerInfo)
	{
		PlayerGO newPlayer = GameObject.Instantiate<PlayerGO>(activeScene, pos);

		newPlayer.AddComponent(new PlayerEntity(newPlayer));
		newPlayer.AddComponent(new PlayerNetwork(newPlayer, playerInfo));

	}

	//private void CreateNetworkPlayer(int id)
	//{
	//	PlayerData data = new PlayerData
	//	{
	//		health = 100.0f,
	//		playerID = id,
	//		x = 50.0f,
	//		y = 50.0f
	//	};

	//	Transform pos = new Transform(new Vector2(data.x, data.y));

	//	PlayerGO newPlayer = GameObject.Instantiate<PlayerGO>(activeScene, pos);

	//	newPlayer.AddComponent(new PlayerEntity(newPlayer));
	//	newPlayer.AddComponent(new PlayerNetwork(newPlayer, data));

	//}
}
