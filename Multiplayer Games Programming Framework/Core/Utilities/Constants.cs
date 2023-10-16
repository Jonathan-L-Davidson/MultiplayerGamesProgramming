using nkast.Aether.Physics2D.Dynamics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Multiplayer_Games_Programming_Framework
{
	internal static class Constants
	{
		public static Vector2 m_Gravity = new Vector2(0, 9.81f);
		public static readonly int m_ScreenWidth = 1500;
		public static readonly int m_ScreenHeight = 1000;

		public static float m_PhysicsWidth = ScreenToPhysics(m_ScreenWidth);
		public static float m_PhysicsHeight = ScreenToPhysics(m_ScreenHeight);

		public static float ScreenToPhysics(float value)
		{
			return value * 0.02f;
		}

		public static Vector2 ScreenToPhysics(Vector2 value)
		{
			return new Vector2(ScreenToPhysics(value.X), ScreenToPhysics(value.Y));
		}

		public static float PhysicstoScreen(float value)
		{
			return value * 50.0f;
		}

		public static Vector2 PhysicstoScreen(Vector2 value)
		{
			return new Vector2(PhysicstoScreen(value.X), PhysicstoScreen(value.Y));
		}

		public static Category GetCategoryByName(string name)
		{
			switch (name)
			{
				case "All":
					return Category.All;
				case "None":
					return Category.None;
				case "Player":
					return Category.Cat1;
				case "Ball":
					return Category.Cat2;
				case "Wall":
					return Category.Cat3;
			}

			Debug.WriteLine("Category not found: " + name + ". Returning Category.None");
			return Category.None;
		}
	}
}