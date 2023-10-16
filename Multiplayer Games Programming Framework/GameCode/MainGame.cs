using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Multiplayer_Games_Programming_Framework.Core;

namespace Multiplayer_Games_Programming_Framework
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        public SpriteBatch m_SpriteBatch { get; private set; }

        SceneManager m_SceneManager;

		public MainGame()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.PreferredBackBufferWidth = Constants.m_ScreenWidth;
            m_Graphics.PreferredBackBufferHeight = Constants.m_ScreenHeight;

            IsFixedTimeStep = true;

			Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            m_SceneManager = new SceneManager(this);
        }

        protected override void Update(GameTime gameTime)
        {

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            m_SceneManager.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_SpriteBatch.Begin(SpriteSortMode.FrontToBack);

            m_SceneManager.Draw(deltaTime);

            m_SpriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}




