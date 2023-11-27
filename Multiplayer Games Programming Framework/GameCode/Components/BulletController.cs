using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer_Games_Programming_Framework.GameCode.Components.Player;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Framework
{
    internal class BulletController : Component
    {
        float damage;
        int owner;
        Rigidbody m_Rigidbody;
        public float timeToLive = 2.0f;
        public float deleteTimer;

        public bool authority;
        public BulletController(GameObject gameObject) : base(gameObject)
        {
            GameScene scene = (GameScene)m_GameObject.m_Scene;
            deleteTimer = scene.m_GameTimer + timeToLive;
        }

        protected override void Start(float deltaTime)
        {
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        }

        protected override void Update(float deltaTime)
        {
            GameScene scene = (GameScene)m_GameObject.m_Scene;
            if (deleteTimer <= scene.m_GameTimer)
            {
                m_GameObject.Destroy();
            }
            
            float rotateAngle = (float)Math.Atan2(-m_Rigidbody.m_Body.LinearVelocity.X, m_Rigidbody.m_Body.LinearVelocity.Y);
            m_Rigidbody.m_Body.Rotation = rotateAngle / MathF.PI * 180;

            base.Update(deltaTime);

        }

        public void Fire(int playerID, float damage, float speed, Vector2 dir)
        {
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
            this.damage = damage;
            owner = playerID;
            m_Rigidbody.m_Body.LinearVelocity = (dir * speed);
        }

        protected override void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
        {
            if (!authority) { return; }
            PlayerGO player = (PlayerGO)other.Body.Tag;
            if(player == null)
            {
                return;
            }

            int debugID = NetworkManager.m_Instance.playerID;

            PlayerEntity PE = player.GetComponent<PlayerEntity>();
            if(PE.GetID() == owner) { return; }
            PE.TakeDamage(damage);
            NetworkManager.m_Instance.UDPSendMessage(new NETHitRegister(damage, PE.GetID(), owner));
            m_GameObject.Destroy();
        }
    }
}
