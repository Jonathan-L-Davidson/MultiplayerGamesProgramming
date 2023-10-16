using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace Multiplayer_Games_Programming_Framework
{
	internal class ChangeColourOnCollision : Component
	{
		Color m_Colour;
		Color m_OgColour;
		SpriteRenderer m_SpriteRenderer;

		public ChangeColourOnCollision(GameObject gameObject, Color col) : base(gameObject)
		{
			m_Colour  = col;
			m_SpriteRenderer = m_GameObject.GetComponent<SpriteRenderer>();
			m_OgColour = m_SpriteRenderer.m_Color;
		}

		protected override void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
		{
			m_SpriteRenderer.m_Color = m_Colour;
		}

		protected override void OnCollisionExit(Fixture sender, Fixture other, Contact contact)
		{
			m_SpriteRenderer.m_Color = m_OgColour;
		}
	}
}
