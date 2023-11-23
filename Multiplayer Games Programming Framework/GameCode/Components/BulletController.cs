using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer_Games_Programming_Framework.GameCode.Components.Player;

namespace Multiplayer_Games_Programming_Framework
{
    internal class BulletController : Component
    {
        float damage;
        int owner;
        Rigidbody m_Rigidbody;
        public BulletController(GameObject gameObject) : base(gameObject)
        {

        }

        protected override void Start(float deltaTime)
        {
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
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
            PlayerGO player = other.Body.Tag as PlayerGO;
            if(player == null)
            {
                return;
            }
            PlayerEntity PE = player.GetComponent<PlayerEntity>();
            PE.TakeDamage(damage, owner);
        }
    }
}
