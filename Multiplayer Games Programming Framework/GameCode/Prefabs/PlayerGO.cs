using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;

namespace Multiplayer_Games_Programming_Framework;

internal class PlayerGO : GameObject
{
    public PlayerGO(Scene scene, Transform transform) : base(scene, transform)
    {
        m_Transform = transform;
    }

    public void Init()
    {
        PlayerEntity entity = GetComponent<PlayerEntity>();

        SpriteRenderer sr = AddComponent(new SpriteRenderer(this, entity.spriteState));
        sr.m_DepthLayer = 0;

        Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 0.1f, sr.m_Size / 2));
        rb.m_Body.IgnoreGravity = true;
        rb.m_Body.FixedRotation = true;
        rb.UpdatePosition(m_Transform.Position);
        entity.GetRigidbody();
    }

    public void SetCollision(bool localPlayer = false)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (localPlayer)
        {
            rb.CreateCircule(Math.Max(sr.m_Size.X, sr.m_Size.Y) / 2, 0.0f, 0.0f, Vector2.Zero, Constants.GetCategoryByName("Player"), Constants.GetCategoryByName("Bullet") | Constants.GetCategoryByName("Wall"));
        }
        else
        {
            rb.CreateCircule(Math.Max(sr.m_Size.X, sr.m_Size.Y) / 2, 0.0f, 0.0f, Vector2.Zero, Constants.GetCategoryByName("Shootable"), Constants.GetCategoryByName("Bullet") | Constants.GetCategoryByName("Wall") | Constants.GetCategoryByName("Player"));
        }
    }
}
