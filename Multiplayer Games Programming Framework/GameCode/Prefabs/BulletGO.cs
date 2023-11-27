using System;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace Multiplayer_Games_Programming_Framework;

internal class BulletGO : GameObject
{
	public float m_speed = 12.0f;
	public int playerID;
	public float damage;
	public Vector2 dir;

	public BulletGO(Scene scene, Transform transform) : base(scene, transform)
	{
		AddComponent(new BulletController(this));
		m_Transform = transform;
	}

	public void Init()
	{
        SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "bullet"));
        sr.m_DepthLayer = 0;

        Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 0.1f, sr.m_Size / 2));
        rb.m_Body.IgnoreGravity = true;
        rb.m_Body.FixedRotation = true;

		Vector2 offsetSpawn = new Vector2(m_Transform.Position.X + (dir.X * (sr.m_Size.X * 2)), m_Transform.Position.Y + (dir.Y * (sr.m_Size.Y * 2)));
		rb.UpdatePosition(offsetSpawn);
    }

	public void CreateHitbox()
	{
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (GetComponent<BulletController>().authority)
		{
			rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 0.0f, Vector2.Zero, Constants.GetCategoryByName("Bullet"), Constants.GetCategoryByName("Shootable"));
		}
		else
		{
            rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 0.0f, Vector2.Zero, Constants.GetCategoryByName("Bullet"), Constants.GetCategoryByName("Player"));
        }
    }

    public void Go()
	{
		BulletController BC = GetComponent<BulletController>();

		BC.Fire(playerID, damage, m_speed, dir);

    }
}