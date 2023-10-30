using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
    internal class PlayerController : Component
    {
        int playerID;
        float m_Speed;
        float m_maxSpeed;
        Rigidbody m_Rigidbody;
        Vector2 m_movementLoop;

        public PlayerController(GameObject gameObject) : base(gameObject)
        {
            m_Speed = 40;
            m_maxSpeed = 50;
        }

        protected override void Start(float deltaTime)
        {
            // Start player info here.
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
            m_Rigidbody.m_Body.Mass = 20;
            m_Rigidbody.m_Body.BodyType = nkast.Aether.Physics2D.Dynamics.BodyType.Dynamic;
        }

        protected override void Update(float deltaTime)
        {
            CheckInput();

            Vector2 Movement = (m_Transform.Right * m_movementLoop.X) + (m_Transform.Up * m_movementLoop.Y);
            m_Rigidbody.m_Body.ApplyLinearImpulse(Movement * m_Speed * deltaTime);
        }

        private void CheckInput()
        {
            Vector2 input = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W)) { input.Y = -1; }    // Up
            if (Keyboard.GetState().IsKeyDown(Keys.A)) { input.X = -1; }    // Left
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { input.Y = 1; }     // Down
            if (Keyboard.GetState().IsKeyDown(Keys.D)) { input.X = 1; }     // Right

            if(input != m_movementLoop)
            {
                UpdateMovement(input);
            }
        }
        private void UpdateMovement(Vector2 input)
        {
            m_movementLoop = input;
            UpdateNetworkMovement();
        }

        private void UpdateNetworkMovement()
        {
            System.Numerics.Vector2 loop;
            loop.X = m_movementLoop.X;
            loop.Y = m_movementLoop.Y;

            NETPlayerMove movePacket = new NETPlayerMove(loop, m_GameObject.m_Name);
            NetworkManager.m_Instance.TCPSendMessage(movePacket);
        }
    }
}
