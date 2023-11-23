using System;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace Multiplayer_Games_Programming_Framework;

internal class BulletGO : GameObject
{
	public float m_speed = 2.0f;
	public int playerID;
	public float damage;
	public Vector2 dir;
	public BulletGO(Scene scene, Transform transform) : base(scene, transform)
	{
		SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "bullet"));
		sr.m_DepthLayer = 0;

		Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 0.1f, sr.m_Size / 2));
		rb.m_Body.IgnoreGravity = true;
		rb.m_Body.FixedRotation = true;
		rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 0.0f, Vector2.Zero, Constants.GetCategoryByName("Bullet"), Constants.GetCategoryByName("Shootable"));

		AddComponent(new BulletController(this));
	}

	public void Go()
	{
		BulletController BC = GetComponent<BulletController>();
		BC.Fire(playerID, damage, m_speed, dir);

    }
}