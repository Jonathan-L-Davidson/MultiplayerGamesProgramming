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

        [JsonPropertyName("PlayerID")]
        public int playerID;

        public NETPlayerMove()
        {
            m_type = PacketType.PLAYERMOVE;
        }

        public NETPlayerMove(Vector2 input, Vector2 position, int playerID)
        {
            m_type = PacketType.PLAYERMOVE;
            this.x = input.X;
            this.y = input.Y;
            
            this.posX = position.X;
            this.posY = position.Y;

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

        public NETPlayerLogin()
        {
            m_type = PacketType.PLAYERLOGIN;
        }

        public NETPlayerLogin(int playerID)
        {
            m_type = PacketType.PLAYERLOGIN;
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
}
