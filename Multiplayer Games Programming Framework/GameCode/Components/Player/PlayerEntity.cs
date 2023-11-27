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
using System.Diagnostics;

namespace Multiplayer_Games_Programming_Framework
{
    internal class PlayerEntity : Component
    {
        public int playerID;
        public string spriteState = "Ball";

        public Vector2 playerInput;

        public float m_Speed { get; private set; }
        public float m_drag { get; private set; }
        public float m_maxSpeed { get; private set; }
        public  Rigidbody m_Rigidbody { get; private set; }
        public Vector2 m_movementLoop { get; private set; }

        public float health;
        public float damage;

        public PlayerEntity(GameObject gameObject, int playerID) : base(gameObject)
        {
            m_Speed = 125;
            m_drag = 7.5f;
            health = 100.0f;
            damage = 10.0f;
            this.playerID = playerID;
        }

        protected override void Start(float deltaTime)
        {
            // Start player info here.
            GetRigidbody();
        }

        public void GetRigidbody()
        {
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
            m_Rigidbody.m_Body.LinearDamping = m_drag;
        }

        protected override void Update(float deltaTime)
        {
            Vector2 Movement = m_Transform.Right * this.m_movementLoop.X + m_Transform.Up * this.m_movementLoop.Y;
            if (m_Rigidbody != null)
            {
                m_Rigidbody.m_Body.ApplyLinearImpulse(Movement * m_Speed * deltaTime);
                UpdateMovement();
            }
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
                NetworkManager.m_Instance.UDPSendMessage(playerUpdate);
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
            data.x = m_Transform.Position.X;
            data.y = m_Transform.Position.Y;
            data.velX = m_Rigidbody.m_Body.LinearVelocity.X;
            data.velY = m_Rigidbody.m_Body.LinearVelocity.Y;
            data.rotation = m_Transform.Rotation;
            data.spriteID = spriteState;
            data.isPlaying = true;
            return data;
        }
        private void UpdateMovement()
        {
            SetMovementLoop(playerInput);
        }

        public void TakeDamage(float damage)
        {
            lock (this)
            {
                health -= damage;
                Debug.WriteLineIf((GetID() == NetworkManager.m_Instance.playerID), $"Player {NetworkManager.m_Instance.playerID} took {damage} damage! Health is now at: {health}");
            }
        }


        public void Shoot(Vector2 dir, bool authority = false)
        {
            BulletGO bullet = GameObject.Instantiate<BulletGO>(m_GameObject.m_Scene, new Transform(new Vector2(m_Transform.Position.X, m_Transform.Position.Y)));
            bullet.playerID = GetID();
            bullet.damage = damage;
            bullet.dir = dir;
            bullet.GetComponent<BulletController>().authority = authority;
            bullet.Init();
            bullet.CreateHitbox();
            bullet.Go();
        }
    }
}
