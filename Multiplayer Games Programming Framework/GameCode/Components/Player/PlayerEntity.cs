using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Packet_Library;
using Multiplayer_Games_Programming_Framework.GameCode.Components.Player;

namespace Multiplayer_Games_Programming_Framework
{
    internal class PlayerEntity : Component
    {
        public int playerID;
        public string spriteState = "Ball";

        public Vector2 playerInput;

        public float m_Speed { get; private set; }
        public float m_maxSpeed { get; private set; }
        public  Rigidbody m_Rigidbody { get; private set; }
        public Vector2 m_movementLoop { get; private set; }

        public float health;

        public PlayerEntity(GameObject gameObject) : base(gameObject)
        {
            m_Speed = 75;
            health = 100.0f;
        }

        protected override void Start(float deltaTime)
        {
            // Start player info here.
            GetRigidbody();
        }

        public void GetRigidbody()
        {
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        }

        protected override void Update(float deltaTime)
        {
            Vector2 Movement = m_Transform.Right * this.m_movementLoop.X + m_Transform.Up * this.m_movementLoop.Y;
            m_Rigidbody.m_Body.ApplyLinearImpulse(Movement * m_Speed * deltaTime);
            UpdateMovement();
        }

        public void SetMovementLoop(Vector2 loop)
        {
            this.m_movementLoop = loop;
        }

        public void NetUpdate()
        {
            lock (this)
            {
                string spriteID = this.m_GameObject.GetComponent<SpriteRenderer>().m_Texture.ToString();
                PlayerData data = GetData();

                NETPlayerUpdate playerUpdate = new NETPlayerUpdate(data);
                NetworkManager.m_Instance.TCPSendMessage(playerUpdate);
            }
        }

        public int GetID()
        {
            return playerID;
        }

        public PlayerData GetData()
        {

            PlayerData data = new PlayerData();
            data.playerID = GetID();
            data.health = this.health;
            data.x = m_Rigidbody.m_Transform.Position.X;
            data.y = m_Rigidbody.m_Transform.Position.Y;
            data.spriteID = spriteState;
            data.isPlaying = true;
            return data;
        }
        private void UpdateMovement()
        {
            SetMovementLoop(playerInput);
        }

        public void TakeDamage(int damage, int attacker)
        {
            lock (this)
            {
                health -= damage;
                //NetworkManager.m_Instance.TCPSendMessage(new NETHitRegister(damage, attacker));
            }
        }

    }
}
