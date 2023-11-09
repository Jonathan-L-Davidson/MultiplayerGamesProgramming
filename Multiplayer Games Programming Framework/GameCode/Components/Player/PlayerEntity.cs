using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Packet_Library;


namespace Multiplayer_Games_Programming_Framework
{
    internal class PlayerEntity : Component
    {
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
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        }

        protected override void Update(float deltaTime)
        {
            Vector2 Movement = m_Transform.Right * m_movementLoop.X + m_Transform.Up * m_movementLoop.Y;
            m_Rigidbody.m_Body.ApplyLinearImpulse(Movement * m_Speed * deltaTime);
        }

        public void SetMovementLoop(Vector2 loop)
        {
            m_movementLoop = loop;
        }
    }
}
