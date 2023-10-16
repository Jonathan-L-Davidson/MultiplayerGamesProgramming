using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework;
using System;
using System.Collections.Generic;
using nkast.Aether.Physics2D.Dynamics;
using Multiplayer_Games_Programming_Framework.Core;
using System.Data;
using System.Diagnostics;
using Multiplayer_Games_Programming_Framework.GameCode.Components;

namespace Multiplayer_Games_Programming_Framework
{
	internal class GameScene : Scene
	{
		List<GameObject> m_GameObjects = new();

		BallGO m_Ball;
		PaddleGO m_PlayerPaddle;
		PaddleGO m_RemotePaddle;

		BallControllerComponent m_BallController;

		Random m_Random = new Random();
		
		GameModeState m_GameModeState;

		float m_GameTimer;

		public GameScene(SceneManager manager) : base(manager)
		{
			m_GameModeState = GameModeState.AWAKE;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			float screenWidth = Constants.m_ScreenWidth;
			float screenHeight = Constants.m_ScreenHeight;

			m_Ball = GameObject.Instantiate<BallGO>(this, new Transform(new Vector2(screenWidth / 2, screenHeight / 2), new Vector2(1, 1), 0));
			m_BallController = m_Ball.GetComponent<BallControllerComponent>();

			//if (NetworkManager.m_Instance.m_Index == 0)
			{
				m_PlayerPaddle = GameObject.Instantiate<PaddleGO>(this, new Transform(new Vector2(100, 500), new Vector2(5, 20), 0));
				m_PlayerPaddle.AddComponent(new PaddleController(m_PlayerPaddle));

				m_RemotePaddle = GameObject.Instantiate<PaddleGO>(this, new Transform(new Vector2(screenWidth - 100, 500), new Vector2(5, 20), 0));
				m_RemotePaddle.AddComponent(new PaddleNetworkController(m_RemotePaddle, 1));
			}
			//else
			//{
			//	m_RemotePaddle = GameObject.Instantiate<PaddleGO>(this, new Transform(new Vector2(100, 500), new Vector2(5, 20), 0));
			//	m_RemotePaddle.AddComponent(new PaddleNetworkController(m_RemotePaddle, 0));

			//	m_PlayerPaddle = GameObject.Instantiate<PaddleGO>(this, new Transform(new Vector2(screenWidth - 100, 500), new Vector2(5, 20), 0));
			//	m_PlayerPaddle.AddComponent(new PaddleController(m_PlayerPaddle));
			//}

			//Border
			Vector2[] wallPos = new Vector2[]
			{
				new Vector2(screenWidth/2, 0), //top
				new Vector2(screenWidth, screenHeight/2), //right
				new Vector2(screenWidth/2, screenHeight), //bottom
				new Vector2(0, screenHeight/2) //left
			};

			Vector2[] wallScales = new Vector2[]
			{
				new Vector2(screenWidth / 10, 10), //top
				new Vector2(10, screenHeight/10), //right
				new Vector2(screenWidth/10, 10), //bottom
				new Vector2(10, screenHeight/10) //left
			};

			for (int i = 0; i < 4; i++)
			{
				GameObject go = GameObject.Instantiate<GameObject>(this, new Transform(wallPos[i], wallScales[i], 0));
				SpriteRenderer sr = go.AddComponent(new SpriteRenderer(go, "Square(10x10)"));
				Rigidbody rb = go.AddComponent(new Rigidbody(go, BodyType.Static, 10, sr.m_Size / 2));
				rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 1.0f, Vector2.Zero, Constants.GetCategoryByName("Wall"), Constants.GetCategoryByName("All"));
				go.AddComponent(new ChangeColourOnCollision(go, Color.Red));
				m_GameObjects.Add(go);
			}
		}

		protected override string SceneName()
		{
			return "GameScene";
		}

		protected override World CreateWorld()
		{
			return new World(Constants.m_Gravity);
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);

			m_GameTimer += deltaTime;

			switch (m_GameModeState)
			{
				case GameModeState.AWAKE:
					m_GameModeState = GameModeState.STARTING;
					break;

				case GameModeState.STARTING:
					m_BallController.Init(10, new Vector2((float)m_Random.NextDouble(), (float)m_Random.NextDouble()));
					m_BallController.StartBall();
					
					m_GameModeState = GameModeState.PLAYING;

					break;

				case GameModeState.PLAYING:

					if(m_GameTimer > 60)
					{
						m_Ball.Destroy();
						m_GameModeState = GameModeState.ENDING;
					}

					break;

				case GameModeState.ENDING:

					Debug.WriteLine("Game Over");
					break;
				default:
					break;
			}
		}
	}
}