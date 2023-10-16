using System;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Collision.Shapes;

namespace Multiplayer_Games_Programming_Framework
{
	internal class Rigidbody : Component
	{
		private struct FixtureData
		{
			public float width;
			public float height;
			public float radius;
			public float restitution;
			public float friction;
			public Vector2 offset;
			public Category collisionGroup;
			public Category collidesWith;

			public FixtureData(float width, float height, float radius, float restitution, float friction, Vector2 offset, Category collisionGroup, Category collidesWith)
			{
				this.width = width;
				this.height = height;
				this.radius = radius;
				this.restitution = restitution;
				this.friction = friction;
				this.offset = offset;
				this.collisionGroup = collisionGroup;
				this.collidesWith = collidesWith;
			}
		}

		public Body m_Body { get; private set; }

		public Rigidbody(GameObject gameObject, BodyType type, float mass, Vector2 centre) : base(gameObject)
		{
			Vector2 pos = new Vector2(Constants.ScreenToPhysics(m_Transform.Position.X), Constants.ScreenToPhysics(m_Transform.Position.Y));

			m_Body = gameObject.m_Scene.m_World.CreateBody(pos, m_Transform.Rotation, type);
			m_Body.Mass = mass;
			m_Body.Tag = m_GameObject;
			m_Body.LocalCenter = centre;
			m_Body.OnCollision += m_GameObject.CollisionEnter;
			m_Body.OnSeparation += m_GameObject.CollisionExit;

			m_Transform.OnScaleChanged += ResetScale;
		}

		public override void Destroy()
		{
			m_GameObject.m_Scene.m_World.Remove(m_Body);
			m_Transform.OnScaleChanged -= ResetScale;
			base.Destroy();
		}

		public Fixture CreateCircule(float radius, float restitution, float friction, Vector2 offset, Category collisionGroup = Category.Cat1, Category collidesWith = Category.All)
		{
			float pRadius = Constants.ScreenToPhysics(radius);
			Vector2 pOffset = new Vector2(Constants.ScreenToPhysics(offset.X), Constants.ScreenToPhysics(offset.Y));

			float fixRadius = Math.Max(pRadius * m_Transform.Scale.X, pRadius * m_Transform.Scale.Y);
			float density = m_Body.Mass / (fixRadius * fixRadius);

			Fixture fixture = m_Body.CreateCircle(fixRadius, density, pOffset);

			fixture.CollisionCategories = collisionGroup;
			fixture.CollidesWith = collidesWith;
			fixture.Tag = new FixtureData(0, 0, radius, restitution, friction, offset, collisionGroup, collidesWith);

			fixture.Restitution = restitution;
			fixture.Friction = friction;

			return fixture;
		}

		public Fixture CreateRectangle(float width, float height, float restitution, float friction, Vector2 offset, Category collisionGroup = Category.Cat1, Category collidesWith = Category.All)
		{
			float pWidth = Constants.ScreenToPhysics(width);
			float pHeight = Constants.ScreenToPhysics(height);
			Vector2 pOffset = new Vector2(Constants.ScreenToPhysics(offset.X), Constants.ScreenToPhysics(offset.Y));

			float fixWidth = pWidth * m_Transform.Scale.X;
			float fixHeight = pHeight * m_Transform.Scale.Y;
			float density = m_Body.Mass / (fixWidth * fixHeight);

			Fixture fixture = m_Body.CreateRectangle(fixWidth, fixHeight, density, pOffset);
			fixture.CollisionCategories = collisionGroup;
			fixture.CollidesWith = collidesWith;
			fixture.Tag = new FixtureData(width, height, 0, restitution, friction, offset, collisionGroup, collidesWith);
			fixture.Restitution = restitution;
			fixture.Friction = friction;

			return fixture;
		}

		void ResetScale()
		{
			FixtureCollection fc = m_Body.FixtureList;
			for (int i = fc.Count - 1; i >= 0; i--)
			{
				Fixture fix = fc[i];

				switch (fix.Shape.ShapeType)
				{
					case ShapeType.Circle:
						FixtureData fd = (FixtureData)fix.Tag;
						m_Body.Remove(fix);
						CreateCircule(fd.radius, fd.restitution, fd.friction, fd.offset, fd.collisionGroup, fd.collidesWith);
						break;
					case ShapeType.Polygon:
						fd = (FixtureData)fix.Tag;
						m_Body.Remove(fix);
						CreateRectangle(fd.width, fd.height, fd.restitution, fd.friction, fd.offset, fd.collisionGroup, fd.collidesWith);
						break;
				}
			}
		}

		override protected void Update(float deltaTime)
		{
			m_Transform.Position = new Vector2(Constants.PhysicstoScreen(m_Body.Position.X), Constants.PhysicstoScreen(m_Body.Position.Y));
			m_Transform.Rotation = m_Body.Rotation;
		}

		public void UpdatePosition(Vector2 position) 
		{ 
			m_Body.Position = new Vector2(Constants.ScreenToPhysics(position.X), Constants.ScreenToPhysics(position.Y)); 
		}
	}
}