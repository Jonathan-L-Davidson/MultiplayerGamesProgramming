using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Packet_Library;
namespace Multiplayer_Games_Programming_Framework
{
	internal class PaddleController : Component
	{
		float m_Speed;
		Rigidbody m_Rigidbody;
		public PaddleController(GameObject gameObject) : base(gameObject)
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

			if (Keyboard.GetState().IsKeyDown(Keys.Up)) { input.Y = -1; }
			if (Keyboard.GetState().IsKeyDown(Keys.Down)) { input.Y = 1; }

			m_Rigidbody.m_Body.LinearVelocity = (m_Transform.Up * input.Y * m_Speed);
		}
	}
}