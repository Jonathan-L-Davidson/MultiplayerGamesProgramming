using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components.Player
{
    internal class PlayerController : Component
    {
        PlayerEntity player;
        public PlayerController(GameObject gameObject) : base(gameObject)
        {
            
        }

        protected override void Start(float deltaTime)
        {
            player = m_GameObject.GetComponent<PlayerEntity>();
            player.playerID = NetworkManager.m_Instance.playerID;
        }

        protected override void Update(float deltaTime)
        {
            CheckInput();
        }

        private void CheckInput()
        {
            Vector2 input = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W)) { input.Y = -1; }    // Up
            if (Keyboard.GetState().IsKeyDown(Keys.A)) { input.X = -1; }    // Left
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { input.Y = 1; }     // Down
            if (Keyboard.GetState().IsKeyDown(Keys.D)) { input.X = 1; }     // Right

            if (input != player.playerInput)
            {
                UpdateMovement(input);
            }
        }
        private void UpdateMovement(Vector2 input)
        {
            player.playerInput = input;
            player.SetMovementLoop(input);
            UpdateNetworkMovement();
        }
        private void UpdateNetworkMovement()
        {
            System.Numerics.Vector2 loop;
            loop.X = player.playerInput.X;
            loop.Y = player.playerInput.Y;

            NETPlayerMove movePacket = new NETPlayerMove(loop, NetworkManager.m_Instance.playerID);
            NetworkManager.m_Instance.TCPSendMessage(movePacket);
        }

    }
}
