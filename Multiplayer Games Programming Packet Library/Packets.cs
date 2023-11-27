using Multiplayer_Games_Programming_Packet_Library;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Multiplayer_Games_Programming_Packet_Library
{
    public struct PlayerData
    {
        public int playerID;
        public float x;
        public float y;
        public float velX;
        public float velY;
        public float rotation;
        public string spriteID;
        public float health;
        public bool isPlaying;
    }

    public struct ObjData
    {
        public int objID;
        public float x;
        public float y;
        public string spriteID;
        public KeyValuePair<string, string> optionalData;
    }

    public enum PacketType
	{
		NULL = 0,
		MSG,
        PLAYERMOVE,
        PLAYERLOGIN,
        PLAYERPLAY,
        PLAYERUPDATE,
        PLAYERCREATE,
        OBJUPDATE,
        PLAYERLOGOUT,
        ENCRYPTED,
        PLAYERHIT,
        PLAYERSHOOT,
    }

    #region "Packet"
    public class Packet
	{
		[JsonPropertyName("Type")]
		public PacketType m_type { get; set; } = PacketType.NULL;

        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new PacketConverter() },
                IncludeFields = true,
            };

            return JsonSerializer.Serialize(this, options);
        }

        public static Packet? Deserialise(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new PacketConverter() },
                    IncludeFields = true,
                };

                return JsonSerializer.Deserialize<Packet>(json, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
                return new Packet();
            }
        }
    }
    #endregion

    #region "Packet Conversion"
    class PacketConverter : JsonConverter<Packet>
    {
        public override Packet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("Type", out var typeProperty))
                {
                    if (typeProperty.GetByte() == (byte)PacketType.ENCRYPTED)
                    {
                        return JsonSerializer.Deserialize<NETEncryptedPacket>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.MSG)
                    {
                        return JsonSerializer.Deserialize<NETMessage>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERMOVE)
                    {
                        return JsonSerializer.Deserialize<NETPlayerMove>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERLOGIN)
                    {
                        return JsonSerializer.Deserialize<NETPlayerLogin>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERPLAY)
                    {
                        return JsonSerializer.Deserialize<NETPlayerPlay>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERUPDATE)
                    {
                        return JsonSerializer.Deserialize<NETPlayerUpdate>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERCREATE)
                    {
                        return JsonSerializer.Deserialize<NETPlayerCreate>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERLOGOUT)
                    {
                        return JsonSerializer.Deserialize<NETPlayerLogout>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERHIT)
                    {
                        return JsonSerializer.Deserialize<NETHitRegister>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERSHOOT)
                    {
                        return JsonSerializer.Deserialize<NETPlayerShoot>(root.GetRawText(), options);
                    }
                    
                }
            }

            throw new JsonException("Unknown type");
        }

        public override void Write(Utf8JsonWriter writer, Packet value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
    #endregion


    #region "Encrypted Packet"
    public class NETEncryptedPacket : Packet
    {
        [JsonPropertyName("ByteArray")]
        public byte[] data;
        [JsonPropertyName("PlayerID")]
        public int playerID;

        public NETEncryptedPacket()
        {
            m_type = PacketType.ENCRYPTED;
        }

        public NETEncryptedPacket(byte[] data, int playerID)
        {
            m_type = PacketType.ENCRYPTED;
            this.playerID = playerID;
            this.data = data;
        }
    }
    #endregion "Encrypted Packet"

    #region "Message Packet"
    public class NETMessage : Packet
    {
        [JsonPropertyName("Message")]
        public string? message;

        public NETMessage()
        {
            m_type = PacketType.MSG;
        }

        public NETMessage(string message)
        {
            m_type = PacketType.MSG;
            this.message = message;
        }

        public void PrintMessage() { Console.WriteLine(message); }
    }
    #endregion "Message Packet"

    #region "Player Movement Packet"
    public class NETPlayerMove : Packet
    {
        [JsonPropertyName("X")]
        public float x;
        [JsonPropertyName("Y")]
        public float y;

        [JsonPropertyName("PosX")]
        public float posX;
        [JsonPropertyName("PosY")]
        public float posY;
        
        [JsonPropertyName("VelX")]
        public float velX;
        [JsonPropertyName("VelY")]
        public float velY;

        [JsonPropertyName("PlayerID")]
        public int playerID;

        public NETPlayerMove()
        {
            m_type = PacketType.PLAYERMOVE;
        }

        public NETPlayerMove(Vector2 input, Vector2 position, Vector2 velocity, int playerID)
        {
            m_type = PacketType.PLAYERMOVE;
            this.x = input.X;
            this.y = input.Y;
            
            this.posX = position.X;
            this.posY = position.Y;

            this.velX = velocity.X;
            this.velY = velocity.Y;

            this.playerID = playerID;

        }

        public NETPlayerMove(float x, float y, int playerID)
        {
            m_type = PacketType.PLAYERMOVE;
            this.x = x;
            this.y = y;
            this.playerID = playerID;
        }
    }
    #endregion

    #region "Player Login Packet"
    public class NETPlayerLogin : Packet
    {
        [JsonPropertyName("PlayerID")]
        public int playerID;

        [JsonPropertyName("Public Key")]
        public RSAParameters publicKey;

        public NETPlayerLogin()
        {
            m_type = PacketType.PLAYERLOGIN;
        }

        public NETPlayerLogin(int playerID, RSAParameters publicKey)
        {
            m_type = PacketType.PLAYERLOGIN;
            this.playerID = playerID;
            this.publicKey = publicKey;
        }

    }
    #endregion

    #region "Player Logout Packet"
    public class NETPlayerLogout : Packet
    {
        [JsonPropertyName("PlayerID")]
        public int playerID;

        public NETPlayerLogout()
        {
            m_type = PacketType.PLAYERLOGOUT;
        }

        public NETPlayerLogout(int playerID)
        {
            m_type = PacketType.PLAYERLOGOUT;
            this.playerID = playerID;
        }

    }
    #endregion

    #region "Player Create Packet"
    public class NETPlayerCreate : Packet
    {
        [JsonPropertyName("Data")]
        public PlayerData data;

        public NETPlayerCreate()
        {
            m_type = PacketType.PLAYERCREATE;
        }

        public NETPlayerCreate(PlayerData data)
        {
            m_type = PacketType.PLAYERCREATE;
            this.data = data;
        }

    }
    #endregion

    #region "Player Update Packet"
    public class NETPlayerUpdate : Packet
    {
        [JsonPropertyName("Data")]
        public PlayerData data;

        public NETPlayerUpdate()
        {
            m_type = PacketType.PLAYERUPDATE;
        }

        public NETPlayerUpdate(PlayerData data)
        {
            m_type = PacketType.PLAYERUPDATE;
            this.data = data;
        }

    }
    #endregion

    #region "Player Play Packet"
    public class NETPlayerPlay : Packet
    {
        [JsonPropertyName("PlayerID")]
        public int playerID;
        [JsonPropertyName("Data")]
        public PlayerData data;

        public NETPlayerPlay()
        {
            m_type = PacketType.PLAYERPLAY;
        }

        public NETPlayerPlay(int playerID, PlayerData data)
        {
            m_type = PacketType.PLAYERPLAY;
            this.playerID = playerID;
            this.data = data;
        }

    }
    #endregion

    #region "Game Object Update Packet"
    public class NETObjectUpdate : Packet
    {
        [JsonPropertyName("Data")]
        public ObjData data;


        public NETObjectUpdate()
        {
            m_type = PacketType.OBJUPDATE;
        }

        public NETObjectUpdate(ObjData data)
        {
            m_type = PacketType.OBJUPDATE;
            this.data = data;
        }

    }
    #endregion

    #region "Player Hit"
    public class NETHitRegister : Packet
    {
        [JsonPropertyName("Damage")]
        public float damage;

        [JsonPropertyName("Attacked Victim")]
        public int victimID;

        [JsonPropertyName("Attacker")]
        public int attackerID;


        public NETHitRegister()
        {
            m_type = PacketType.PLAYERHIT;
        }

        public NETHitRegister(float damage, int victimID, int attackerID)
        {
            m_type = PacketType.PLAYERHIT;
            this.damage = damage;
            this.victimID = victimID;
            this.attackerID = attackerID;
        }

    }
    #endregion

    #region "Player Shoot"
    public class NETPlayerShoot : Packet
    {
        [JsonPropertyName("Dir X")]
        public float dirX;
        [JsonPropertyName("Dir Y")]
        public float dirY;

        [JsonPropertyName("ID")]
        public int playerID;

        public NETPlayerShoot()
        {
            m_type = PacketType.PLAYERSHOOT;
        }

        public NETPlayerShoot(float dirX, float dirY, int playerID)
        {
            m_type = PacketType.PLAYERSHOOT ;
            this.dirX = dirX;
            this.dirY = dirY;
            this.playerID = playerID;
        }

    }
    #endregion
}
