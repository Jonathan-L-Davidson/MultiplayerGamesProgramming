using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
    internal class Shake : Component
    {
        float m_Speed;
        SpriteRenderer m_spriteRenderer;
        public Shake(GameObject gameObject) : base(gameObject)
        {
        }

        protected override void Start(float deltaTime)
        {
            m_spriteRenderer = m_GameObject.GetComponent<SpriteRenderer>();
            if (m_spriteRenderer == null)
            {
                throw new Exception("Object does not contain a sprite renderer.");
            }
        }
        public void Init(float speed)
        {
            m_Speed = speed;
        }

        //public void Shake(Vector2 shakeBounds, int duration)
        //{
        //    Random rand = new Random();
        //    int timer = 0;
        //    while (timer < duration)
        //    {
        //        float randX = (float)rand.Next(shakeBounds.X, shakeBounds.Y);
        //        float randY = (float)rand.Next(shakeBounds.X, shakeBounds.Y);
                
        //        timer++;

        //    }
        //}

    }
}
