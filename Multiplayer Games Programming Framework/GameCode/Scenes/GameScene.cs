﻿using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework;
using System;
using System.Collections.Generic;
using nkast.Aether.Physics2D.Dynamics;
using Multiplayer_Games_Programming_Framework.Core;
using System.Data;
using System.Diagnostics;
using Multiplayer_Games_Programming_Framework.GameCode.Components;
using Multiplayer_Games_Programming_Framework.GameCode.Components.Player;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Framework
{
	internal class GameScene : Scene
	{
		public SceneManager manager { get; private set; }

		List<GameObject> m_GameObjects = new();

		Dictionary<int, PlayerEntity> m_players = new();

		BallGO m_Ball;
		PlayerGO m_PlayerPaddle;

		BallControllerComponent m_BallController;

		Random m_Random = new Random();
		
		GameModeState m_GameModeState;

		public float m_GameTimer;

		public GameScene(SceneManager manager) : base(manager)
		{
			m_GameModeState = GameModeState.AWAKE;
			this.manager = manager;
			m_players = new();
		}

		public override void LoadContent()
		{
			base.LoadContent();

			float screenWidth = Constants.m_ScreenWidth;
			float screenHeight = Constants.m_ScreenHeight;

			m_PlayerPaddle = GameObject.Instantiate<PlayerGO>(this, new Transform(new Vector2(100, 500), new Vector2(1, 1), 0));
			m_PlayerPaddle.AddComponent(new PlayerEntity(m_PlayerPaddle, NetworkManager.m_Instance.playerID));
			m_PlayerPaddle.AddComponent(new PlayerController(m_PlayerPaddle));
			m_PlayerPaddle.Init();
			m_PlayerPaddle.SetCollision(true);
			m_players.Add(NetworkManager.m_Instance.playerID, m_PlayerPaddle.GetComponent<PlayerEntity>());

            NetworkManager.m_Instance.TCPSendMessage(new NETPlayerPlay(NetworkManager.m_Instance.playerID, m_PlayerPaddle.GetComponent<PlayerEntity>().GetData()));

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
			DelQueue();

            m_GameTimer += deltaTime;

			switch (m_GameModeState)
			{
				case GameModeState.AWAKE:
					m_GameModeState = GameModeState.STARTING;
					break;

				case GameModeState.STARTING:
					m_GameModeState = GameModeState.PLAYING;

					break;

				case GameModeState.PLAYING:

					//if(m_GameTimer > 60)
					//{
					//	m_Ball.Destroy();
					//	m_GameModeState = GameModeState.ENDING;
					//}

					break;

				case GameModeState.ENDING:

					Debug.WriteLine("Game Over");
					break;
				default:
					break;
			}
		}

		public void EndGame()
		{
			m_GameModeState = GameModeState.ENDING;

		}

		public Dictionary<int, PlayerEntity> GetPlayers() { return m_players; }
    }
}