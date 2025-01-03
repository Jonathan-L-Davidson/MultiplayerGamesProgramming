﻿using Microsoft.Xna.Framework.Input;
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
        bool bulletShot = false;
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
            GameScene gameScene = (GameScene)m_GameObject.m_Scene;
            if (!gameScene.manager.m_Game.IsActive) { return; }
            Vector2 input = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W)) { input.Y = -1; }    // Up
            if (Keyboard.GetState().IsKeyDown(Keys.A)) { input.X = -1; }    // Left
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { input.Y = 1; }     // Down
            if (Keyboard.GetState().IsKeyDown(Keys.D)) { input.X = 1; }     // Right


            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!bulletShot)
                {
                    float x, y;
                    MouseState MS = Mouse.GetState();
                    x = MS.X;
                    y = MS.Y;

                    Vector2 dir = new Vector2(x - m_Transform.Position.X, y - m_Transform.Position.Y);
                    dir.Normalize();

                    PlayerEntity PE = m_GameObject.GetComponent<PlayerEntity>();
                    PE.Shoot(dir, true);
                    NetworkManager.m_Instance.UDPSendMessage(new NETPlayerShoot(dir.X, dir.Y, NetworkManager.m_Instance.playerID));
                    bulletShot = true;
                }
            }
            else
            {
                bulletShot = false;
            }

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
            System.Numerics.Vector2 loop, position, velocity;
            loop.X = player.playerInput.X;
            loop.Y = player.playerInput.Y;

            position.X = player.m_GameObject.m_Transform.Position.X;
            position.Y = player.m_GameObject.m_Transform.Position.Y;

            Rigidbody rb = player.m_GameObject.GetComponent<Rigidbody>();
            velocity.X = rb.m_Body.LinearVelocity.X;
            velocity.Y = rb.m_Body.LinearVelocity.Y;

            NETPlayerMove movePacket = new NETPlayerMove(loop, position, velocity, player.playerID);
            NetworkManager.m_Instance.UDPSendMessage(movePacket);
        }

    }
}
