using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Server
{
    internal class ConnectedClient
    {
        UdpClient m_UdpSender;

        Socket m_socket;
        NetworkStream m_netStream;
        StreamReader m_netReader;
        StreamWriter m_netWriter;

        PlayerData m_playerData;

        RSACryptoServiceProvider m_RsaProvider;
        public RSAParameters m_serverPublicKey; //this is this owners public key
        RSAParameters m_serverPrivateKey; //this is this owners private key
        RSAParameters m_clientPublicKey; //this is the public key of the other owner

        public bool IsPlaying() { return m_playerData.isPlaying; }
        public void SetPlaying(bool playing) {  m_playerData.isPlaying = playing; }
        public void SetID(int id) { m_playerData.playerID = id; }
        public int GetID() {  return m_playerData.playerID; }
        public PlayerData GetData() { return m_playerData; }
        public bool active { get; private set; } = true;
        public void SetClientKey(RSAParameters key) { m_clientPublicKey = key; }
        public ConnectedClient(object socket)
		{
            if (socket == null) { throw new ArgumentNullException("socket missing"); }

            m_socket = (Socket)socket;
            IPEndPoint? endPoint = m_socket.RemoteEndPoint as IPEndPoint;
            if(endPoint == null) { throw new ArgumentNullException("No endpoint found!"); }
            
            m_UdpSender = new UdpClient(endPoint);

            m_RsaProvider = new RSACryptoServiceProvider(2048); //init the service and set the size of the keys (note the higher the value the stronger the encryption but it is slower)
            m_serverPublicKey = m_RsaProvider.ExportParameters(false); //false provides the public key
            m_serverPrivateKey = m_RsaProvider.ExportParameters(true); //true provides the private key

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

        public void UpdateTransform(NETPlayerUpdate update)
        {
            lock (this)
            {
                m_playerData.x = update.data.x;
                m_playerData.y = update.data.y;
            }
        }
        public void UpdateTransform(NETPlayerMove update)
        {
            lock (this)
            {
                m_playerData.x = update.posX;
                m_playerData.y = update.posY;
                m_playerData.velX = update.velX;
                m_playerData.velY = update.velY;
            }
        }

        public string ReadTCP()
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
                return "";
            }

            return "";
        }

		public void Send(Packet packet, bool isUDP = false, bool encryption = true)
		{
            if (!active) return;
            try
            {
                if (!isUDP)
                {
                    string data = PreparePacket(packet, encryption);
                    m_netWriter.WriteLine(data);
                    m_netWriter.Flush();
                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(PreparePacket(packet, encryption));
                    m_UdpSender.Send(bytes, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Logout();
                return;
            }
		}

        private string PreparePacket(Packet packet, bool encryption = true)
        {
            if (encryption) 
            { 
                packet = EncryptPacket(packet); 
            }
            return packet.ToJson();
        }

        private Packet EncryptPacket(Packet packet)
        {
            lock (m_RsaProvider)
            {
                m_RsaProvider.ImportParameters(m_clientPublicKey); //sets the providers current key to be the other owner
                string json = packet.ToJson(); //serialise the animal
                byte[] encryptedData = m_RsaProvider.Encrypt(Encoding.UTF8.GetBytes(json), false); //encrypt the data into a byte array
                NETEncryptedPacket encryptedPacket = new NETEncryptedPacket(encryptedData, GetID());
                return encryptedPacket;
            }
        }

        public Packet DecryptPacket(NETEncryptedPacket packet)
        {
            m_RsaProvider.ImportParameters(m_serverPrivateKey); //sets the providers current key to be this owners private key
            byte[] decrypted = m_RsaProvider.Decrypt(packet.data, false); //decrypt the byte array
            string json = Encoding.UTF8.GetString(decrypted); //get the json string
            return Packet.Deserialise(json); ; //Deserialise the json into a packet
        }

        public bool StartGame(NETPlayerPlay start)
        {
            lock (this)
            {
                m_playerData = start.data;
                SetPlaying(true);
                return true;
            }
        }

        public void Logout()
        {
            lock (this)
            {
                SetPlaying(false);
                active = false;
                Send(new NETPlayerLogout(GetID()));
            }
        }
    }
}
