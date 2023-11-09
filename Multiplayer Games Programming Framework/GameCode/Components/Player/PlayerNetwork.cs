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
    internal class PlayerNetwork : Component
    {
        public int playerID;

        PlayerEntity player;
        PlayerData initPlayerData;

        public Vector2 playerInput;
        public PlayerNetwork(GameObject gameObject, PlayerData data) : base(gameObject)
        {
            this.initPlayerData = data;
        }
        protected override void Start(float deltaTime)
        {
            player = m_GameObject.GetComponent<PlayerEntity>();
            if(player == null)
            {
                throw new Exception("PlayerEntity not found!");
            }
            player.health = initPlayerData.health;
            player.m_Transform.Position = new Vector2(initPlayerData.x, initPlayerData.y);
            player.NetUpdate();
        }

        protected override void Update(float deltaTime)
        {
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            player.SetMovementLoop(playerInput);
        }

        public void TakeDamage(int damage, int attacker)
        {
            lock (this)
            {
                player.health -= damage;
                //NetworkManager.m_Instance.TCPSendMessage(new NETHitRegister(damage, attacker));
            }
        }
    }
}
