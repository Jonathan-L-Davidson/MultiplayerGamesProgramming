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
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.VisualBasic.FileIO;

namespace Multiplayer_Games_Programming_Framework;

internal class NetworkManager
{
	private static NetworkManager Instance;

	public Scene activeScene;
	public int playerID = -1;

    RSACryptoServiceProvider m_RsaProvider;
    RSAParameters m_serverPublicKey;
    RSAParameters m_clientPrivateKey; //this is this owners private key
    public RSAParameters m_clientPublicKey; //this is the public key of the other owner

    NETEncryptedPacket EncryptPacket(Packet packet)
	{
		// use m_serverPublicKey
		lock (m_RsaProvider)
		{
			m_RsaProvider.ImportParameters(m_serverPublicKey); //sets the providers current key to be the other owner
			string json = packet.ToJson(); //serialise the animal
			byte[] encryptedData = m_RsaProvider.Encrypt(Encoding.UTF8.GetBytes(json), false); //encrypt the data into a byte array
			NETEncryptedPacket encryptedPacket = new NETEncryptedPacket(encryptedData, playerID);
			return encryptedPacket;
		}
    }

    Packet DecryptPacket(NETEncryptedPacket packet)
	{
		// use m_clientPrivateKey
		lock (m_RsaProvider)
		{
            m_RsaProvider.ImportParameters(m_clientPrivateKey); //sets the providers current key to be this owners private key
            byte[] decrypted = m_RsaProvider.Decrypt(packet.data, false); //decrypt the byte array
            string json = Encoding.UTF8.GetString(decrypted); //get the json string
            return Packet.Deserialise(json); ; //Deserialise the json into a packet
        }
    }

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
    UdpClient m_udpClient;

    NetworkStream m_netStream;
	StreamReader m_netReader;
	StreamWriter m_netWriter;

	NetworkManager()
	{
		m_tcpClient = new();
        m_udpClient = new();
    }

    public bool Connect(string ip, int port)
	{
		try
		{
			m_tcpClient.Connect(ip, port);
			m_netStream = m_tcpClient.GetStream();
            m_udpClient.Connect(ip, port);

            m_netReader = new StreamReader(m_netStream, Encoding.UTF8);
			m_netWriter = new StreamWriter(m_netStream, Encoding.UTF8);

            m_RsaProvider = new RSACryptoServiceProvider(2048); //init the service and set the size of the keys (note the higher the value the stronger the encryption but it is slower)
            m_clientPublicKey = m_RsaProvider.ExportParameters(false); //false provides the public key
            m_clientPrivateKey = m_RsaProvider.ExportParameters(true); //true provides the private key

            Run();
			return true;
		}
		catch (Exception e)
		{
			Debug.WriteLine($"Error while connecting: {e.Message}");
		}

		return false;
	}

	public void Logout()
	{
		TCPSendMessage(new NETPlayerLogout(playerID));
        m_tcpClient.Close();
    }
	public void Run()
	{
		Thread TcpThread = new Thread(new ThreadStart(TcpProcessServerResponse));
		TcpThread.Name = "TCP NetHandler";
		TcpThread.Start();
		UdpProcessServerResponse();
    }

	private void TcpProcessServerResponse()
	{

		while (m_tcpClient.Connected)
		{
			try
			{
				string msg = m_netReader.ReadLine();

				Debug.WriteLine($"{playerID} | TCP Message recieved: {msg}");
				if( msg == null)
				{ 
					Debug.WriteLine("Unknown message recieved!");
					Logout();
					continue;
				}
				Packet packet = DeserialisePacket(msg);

				HandlePacket(packet);
			}
			catch (Exception e)
			{
				Debug.WriteLine($"TCP Process Error: {e.Message}");
			}
		}
	}
    async Task UdpProcessServerResponse()
    {
        while (m_tcpClient.Connected)
        {
			try
			{
				UdpReceiveResult receiveResult = await m_udpClient.ReceiveAsync();
				byte[] receivedData = receiveResult.Buffer;

				string msg = Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);
				Debug.WriteLine($"CLIENT - UDP Message recieved: {msg}");
				Packet packet = DeserialisePacket(msg);

				HandlePacket(packet);
			}
			catch (SocketException e)
			{
                Debug.WriteLine("Client UDP Read Method exception: " + e.Message);
			}
		}
    }

    public void TCPSendMessage(Packet packet, bool encryption = true)
	{
		if (encryption)
		{
			packet = EncryptPacket(packet);
		}
		m_netWriter.WriteLine(packet.ToJson());
		m_netWriter.Flush();

	}

	public void UDPSendMessage(Packet packet, bool encryption = true)
	{
        if (encryption)
        {
            packet = EncryptPacket(packet);
        }
        byte[] bytes = Encoding.UTF8.GetBytes(packet.ToJson());
        m_udpClient.Send(bytes, bytes.Length);
	}

	private Packet DeserialisePacket(string msg)
	{
		return Packet.Deserialise(msg);
	}

	private void HandlePacket(Packet packet)
	{
		switch (packet.m_type)
		{
			case PacketType.ENCRYPTED:
				NETEncryptedPacket encryptedPacket = (NETEncryptedPacket)packet;
				Packet decryptedPacket = DecryptPacket(encryptedPacket);
				HandlePacket(decryptedPacket);
				break;
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
            case (PacketType.PLAYERLOGOUT):
                NETPlayerLogout logoutPacket = (NETPlayerLogout)packet;
                HandePlayerLogout(logoutPacket);
                break;
			case (PacketType.PLAYERHIT):
				NETHitRegister hitReg = (NETHitRegister)packet;
				HandlePlayerHit(hitReg);
				break;
			case (PacketType.PLAYERSHOOT):
				NETPlayerShoot shootingPacket = (NETPlayerShoot)packet;
				HandlePlayerShoot(shootingPacket);
				break;
        }
	}

	public void Login()
	{
		NETMessage message = new NETMessage($"Hello server!!");
		TCPSendMessage(message, false);
	}

	private void HandleLogin(NETPlayerLogin loginPacket)
	{
		this.playerID = loginPacket.playerID;
		this.m_serverPublicKey = loginPacket.publicKey;
        TCPSendMessage(new NETPlayerLogin(playerID, m_clientPublicKey), encryption: false);
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
				playerRef.m_Rigidbody.m_Body.LinearVelocity = new Vector2(movePacket.velX, movePacket.velY);
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
		newPlayer.Init();
		newPlayer.SetCollision();

		GameScene gameScene = (GameScene)activeScene;
		if(gameScene == null) { return null; }

        Dictionary<int, PlayerEntity> players = gameScene.GetPlayers();
        players.Add(data.playerID, entity);

		return entity;
	}

	private void HandePlayerLogout(NETPlayerLogout logoutPacket)
	{
		GameScene gameScene = (GameScene)activeScene;
		PlayerEntity entity = gameScene.GetPlayers().ElementAt(logoutPacket.playerID).Value;
		if(entity.GetID() == playerID)
		{
			TCPSendMessage(new NETPlayerLogout(playerID));
			gameScene.EndGame();
		}
		if(entity.m_GameObject == null) { return; }
		lock (entity)
		{
			entity.m_GameObject.Destroy();
		}
		
	}

	private void HandlePlayerHit(NETHitRegister hitReg)
	{
        GameScene gameScene = (GameScene)activeScene;
        PlayerEntity entity = gameScene.GetPlayers().ElementAt(hitReg.victimID).Value;

		Debug.WriteLine($"Player {hitReg.victimID} gets hit by Player {hitReg.attackerID} for {hitReg.damage} damage!");
		entity.TakeDamage(hitReg.damage);

    }

    private void HandlePlayerShoot(NETPlayerShoot shoot)
	{
        GameScene gameScene = (GameScene)activeScene;
        PlayerEntity entity = gameScene.GetPlayers().ElementAt(shoot.playerID).Value;
		entity.Shoot(new Vector2(shoot.dirX, shoot.dirY));
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
