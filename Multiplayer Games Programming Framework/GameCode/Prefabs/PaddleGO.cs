using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace Multiplayer_Games_Programming_Framework
{
	internal class PaddleGO : GameObject
	{
		public PaddleGO(Scene scene, Transform transform) : base(scene, transform)
		{
			SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "Square(10x10)"));
			sr.m_DepthLayer = 1;

			Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Kinematic, 1, sr.m_Size / 2));
			rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 1.0f, Vector2.Zero, Constants.GetCategoryByName("Player"), Constants.GetCategoryByName("Ball") | Constants.GetCategoryByName("Wall"));
		}
	}
}