using System;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace Multiplayer_Games_Programming_Framework;

internal class BallGO : GameObject
{
	public BallGO(Scene scene, Transform transform) : base(scene, transform)
	{
		SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "ball"));
		sr.m_DepthLayer = 0;

		Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 0.1f, sr.m_Size / 2));
		rb.m_Body.IgnoreGravity = true;
		rb.m_Body.FixedRotation = true;
		rb.CreateCircule(Math.Max(sr.m_Size.X, sr.m_Size.Y) / 2, 0.0f, 0.0f, Vector2.Zero, Constants.GetCategoryByName("Ball"), Constants.GetCategoryByName("Player") | Constants.GetCategoryByName("Wall"));
		
		AddComponent(new BallControllerComponent(this));
	}
}