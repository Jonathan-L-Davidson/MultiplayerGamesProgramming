using Multiplayer_Games_Programming_Packet_Library;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Multiplayer_Games_Programming_Packet_Library
{
	public enum PacketType
	{
		NULL = 0,
		MSG,
        POSITION,
        PLAYERMOVE,
        OBJUPDATE,
    }

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
            var options = new JsonSerializerOptions
            {
                Converters = { new PacketConverter() },
                IncludeFields = true,
            };

            return JsonSerializer.Deserialize<Packet>(json, options);
        }
    }



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
                    if (typeProperty.GetByte() == (byte)PacketType.POSITION)
                    {
                        return JsonSerializer.Deserialize<NETPosition>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.PLAYERMOVE)
                    {
                        return JsonSerializer.Deserialize<NETPlayerMove>(root.GetRawText(), options);
                    }

                    //if (typeProperty.GetByte() == (byte)AnimalType.CAT)
                    //{
                    //    return JsonSerializer.Deserialize<Cat>(root.GetRawText(), options);
                    //}
                    // Add more types here
                }
            }

            throw new JsonException("Unknown type");
        }

        public override void Write(Utf8JsonWriter writer, Packet value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

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

    public class NETPosition : Packet
    {
        [JsonPropertyName("Position")]
        public Vector2 position;
        public object objRef;

        public NETPosition(Vector2 position, object objRef)
        {
            m_type = PacketType.POSITION;
            this.position = position;
            this.objRef = objRef;
        }
    }

    public class NETPlayerMove : Packet
    {
        [JsonPropertyName("X")]
        public float x;
        [JsonPropertyName("Y")]
        public float y;
        
        [JsonPropertyName("PlayerID")]
        public string playerID;

        public NETPlayerMove()
        {
            m_type = PacketType.PLAYERMOVE;
        }

        public NETPlayerMove(Vector2 position, string playerID)
        {
            m_type = PacketType.PLAYERMOVE;
            this.x = position.X;
            this.y = position.Y;
            this.playerID = playerID;
        }

        public NETPlayerMove(float x, float y, string playerID)
        {
            m_type = PacketType.PLAYERMOVE;
            this.x = x;
            this.y = y;
            this.playerID = playerID;
        }
    }


}

