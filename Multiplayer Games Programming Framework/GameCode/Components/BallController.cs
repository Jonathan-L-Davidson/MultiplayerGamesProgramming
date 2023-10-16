using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace Multiplayer_Games_Programming_Framework
{
	internal class BallControllerComponent : Component
	{
		float m_Speed;
		Vector2 m_InitDirection;
		Rigidbody m_Rigidbody;
		public BallControllerComponent(GameObject gameObject) : base(gameObject)
		{
		}

		protected override void Start(float deltaTime)
		{
			m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
		}
		public void Init(float speed, Vector2 direction)
		{
			m_Speed = speed;
			m_InitDirection = direction;
		}

		public void StartBall()
		{
			m_Rigidbody.m_Body.LinearVelocity = (m_InitDirection * m_Speed);
		}

		protected override void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
		{
			Vector2 normal = contact.Manifold.LocalNormal;
			Vector2 velocity = m_Rigidbody.m_Body.LinearVelocity;
			Vector2 reflection =  Vector2.Reflect(velocity, normal);
			m_Rigidbody.m_Body.LinearVelocity = reflection * 1.0f;
		}
	}
}
