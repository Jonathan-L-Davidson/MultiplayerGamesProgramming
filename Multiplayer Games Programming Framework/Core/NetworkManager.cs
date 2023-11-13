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
		while (m_tcpClient.Connected)
		{
			try
			{
				string msg = m_netReader.ReadLine();

				Debug.WriteLine($"Message recieved: {msg}");
				Packet packet = DeserialisePacket(msg);

				HandlePacket(packet);
			}
			catch (Exception e)
			{
				Debug.WriteLine($"TCP Process Error: {e.Message}");
			}
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
                NETPlayerPlay playPacket = (NETPlayerPlay)packet;
                //HandlePlayerJoin(playPacket);
                break;
			case (PacketType.PLAYERCREATE):
				NETPlayerCreate createPacket = (NETPlayerCreate)packet;
				CreateNetworkPlayer(createPacket.data);
				break;
            case (PacketType.PLAYERUPDATE):
                NETPlayerUpdate updatePlayerPacket = (NETPlayerUpdate)packet;
				UpdatePlayer(updatePlayerPacket);
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
		lock(activeScene)
		{
			GameScene gameScene = (GameScene)activeScene;
			PlayerEntity playerRef = gameScene.GetPlayers()[movePacket.playerID];
			if (playerRef != null)
			{
				playerRef.playerInput = new Vector2(movePacket.x, movePacket.y);
				playerRef.m_Rigidbody.UpdatePosition(new Vector2(movePacket.posX, movePacket.posY));
			}
		}
	}


	private void UpdatePlayer(NETPlayerUpdate playerUpdate)
	{

		GameScene gameScene = (GameScene)activeScene;

		if(gameScene == null) { return; }

		Dictionary<int, PlayerEntity> players = gameScene.GetPlayers();
        // Check if ID is in the player entity list.
        // if not, create a new player.

        PlayerEntity entity = players.ElementAt(playerUpdate.data.playerID).Value;

		if(entity == null)
		{
			return;
		}

		// Set data from playerUpdate to player, using a lock to prevent any conflict in data.
		
	}

	private PlayerEntity CreateNetworkPlayer(PlayerData data)
	{
		Vector2 pos = new Vector2(data.x, data.y);
		Transform trans = new Transform();
		trans.Position = pos;
		
		PlayerGO newPlayer = GameObject.Instantiate<PlayerGO>(activeScene, trans);

		PlayerEntity entity = new PlayerEntity(newPlayer, data.playerID);

        newPlayer.AddComponent(entity);

		GameScene gameScene = (GameScene)activeScene;
		if(gameScene == null) { return null; }

        Dictionary<int, PlayerEntity> players = gameScene.GetPlayers();
        players.Add(data.playerID, entity);

		return entity;
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
