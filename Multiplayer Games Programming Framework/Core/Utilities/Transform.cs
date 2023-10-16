using System;
using Microsoft.Xna.Framework;

namespace Multiplayer_Games_Programming_Framework
{
	class Transform
	{
		public Vector2 Position;
		public Vector2 Scale { get; private set; }
		public float Rotation;
		public event Action OnScaleChanged;

			enum Direction
			{
				Up,
				Down,
				Left,
				Right
			}

		public Transform()
		{
			Position = new Vector2(0, 0);
			Scale = new Vector2(1, 1);
			Rotation = 0;

			Direction direction = Direction.Up;

			switch (direction)
			{
				case Direction.Up:
					Console.WriteLine("Direction is Up");
					break;
				case Direction.Down:
					Console.WriteLine("Direction is Down");
					break;
				case Direction.Left:
					Console.WriteLine("Direction is Left");
					break;
				case Direction.Right:
					Console.WriteLine("Direction is Right");
					break;
				default:
					break;
			}
		}

		public Transform(Vector2 position)
		{
			Position = position;
			Scale = new Vector2(1, 1);
			Rotation = 0;
		}

		public Transform(Vector2 position, Vector2 scale, float rotation)
		{
			Position = position;
			Scale = scale;
			Rotation = rotation;
		}

		public void SetScale(Vector2 scale)
		{
			Scale = scale;
			OnScaleChanged?.Invoke();
		}

		public Vector2 Right
		{
			get
			{
				float RotationRad = MathHelper.ToRadians(Rotation);
				Vector2 right = new Vector2((float)Math.Cos(RotationRad), (float)Math.Sin(RotationRad)) ;
				right.Normalize();
				return right;
			}
		}

		public Vector2 Up
		{
			get
			{
				float RotationRad = MathHelper.ToRadians(Rotation);
				Vector2 up = new Vector2((float)-Math.Sin(RotationRad), (float)Math.Cos(RotationRad));
				up.Normalize();
				return up;
			}
		}
	}
}
