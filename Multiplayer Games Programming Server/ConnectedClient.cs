﻿using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Numerics;
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

        PlayerData m_playerData;


        public bool IsPlaying() { return m_playerData.isPlaying; }
        public void SetPlaying(bool playing) {  m_playerData.isPlaying = playing; }
        public void SetID(int id) { m_playerData.playerID = id; }
        public int GetID() {  return m_playerData.playerID; }
        public PlayerData GetData() { return m_playerData; }
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
            try
            {
                string data = PreparePacket(packet);
                m_netWriter.WriteLine(data);
                m_netWriter.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return;
            }
		}

        private string PreparePacket(Packet packet)
        {
            return packet.ToJson();
        }

        public bool StartGame()
        {
            lock (this)
            {
                NETPlayerUpdate createCharacter = new NETPlayerUpdate();
                createCharacter.data = GetData();

                Send(createCharacter);

                SetPlaying(true);
                return true;
            }
        }
    }
}
