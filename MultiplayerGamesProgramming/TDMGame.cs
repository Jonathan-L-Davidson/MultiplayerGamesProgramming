using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerGamesProgramming
{
    public class TDMGame : Game
    {
        Texture2D ball;
        Vector2 ballPos;
        float ballSpeed;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public TDMGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ballPos = new Vector2(_graphics.PreferredBackBufferWidth/2, _graphics.PreferredBackBufferHeight/2); // Middle of the screen.
            ballSpeed = 100.0f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ball = Content.Load<Texture2D>("ball");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            HandleInput(gameTime);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(ball, ballPos, null, Color.White, 0f, new Vector2(ball.Width/2, ball.Height/2),Vector2.One,SpriteEffects.None,0f);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected void HandleInput(GameTime gameTime)
        {
            var keystate = Keyboard.GetState();

            if (keystate.IsKeyDown(Keys.Up))
            {
                ballPos.Y -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keystate.IsKeyDown(Keys.Down))
            {
                ballPos.Y += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keystate.IsKeyDown(Keys.Left))
            {
                ballPos.X -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keystate.IsKeyDown(Keys.Right))
            {
                ballPos.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}