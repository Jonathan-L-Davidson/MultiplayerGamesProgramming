using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Multiplayer_Games_Programming_Framework
{
    internal class SpriteRenderer : Component
    {
        SpriteBatch m_SpriteBatch;
		public Texture2D m_Texture { get; private set; }
		public Vector2 m_Size { get; private set; }

        public float m_DepthLayer = 0;

        public Color m_Color = Color.White;

        public SpriteRenderer(GameObject gameObject, string texture) : base(gameObject)
        {
            m_SpriteBatch = gameObject.m_Scene.GetSpriteBatch();
            m_Texture = gameObject.m_Scene.GetContentManager().Load<Texture2D>(texture);

            if (m_Texture == null)
            {
                Console.WriteLine("Texture not found");
                return;
            }

            m_Size = new Vector2(m_Texture.Width, m_Texture.Height);
        }

		protected override void Draw(float deltaTime)
        {
            m_SpriteBatch.Draw(m_Texture, m_Transform.Position, null, m_Color, MathHelper.ToRadians(m_Transform.Rotation), m_Size / 2, m_Transform.Scale, new SpriteEffects(), m_DepthLayer);
        }
    }
}
