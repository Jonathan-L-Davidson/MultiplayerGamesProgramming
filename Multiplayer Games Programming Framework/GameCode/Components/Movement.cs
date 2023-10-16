using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Multiplayer_Games_Programming_Framework
{
    internal class Movement : Component
    {
        float m_Speed;
		Rigidbody m_Rigidbody;
        public Movement(GameObject gameObject) : base(gameObject)
        {
            m_Speed = 10;
        }

		protected override void Start(float deltaTime)
		{
			m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
		}

		protected override void Update(float deltaTime)
        {
			Vector2 input = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))	{ input.X = -1; }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))	{ input.X = 1; }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))		{ input.Y = -1; }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))	{ input.Y = 1; }

			Vector2 Movement = (m_Transform.Right * input.X) + (m_Transform.Up * input.Y);
			m_Rigidbody.m_Body.ApplyLinearImpulse(Movement * m_Speed * deltaTime);

			if (Keyboard.GetState().IsKeyDown(Keys.E))
			{
				m_Rigidbody.m_Body.Rotation += MathHelper.ToRadians(5 * deltaTime);
				m_Transform.SetScale(m_Transform.Scale - new Vector2(0.1f, 0.1f));
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Q))
			{
				m_Rigidbody.m_Body.Rotation -= MathHelper.ToRadians(5 * deltaTime);
				m_Transform.SetScale(m_Transform.Scale + new Vector2(0.1f, 0.1f));
			}
		}
	}
}
