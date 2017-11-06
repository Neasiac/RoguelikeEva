using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Scenes;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RoguelikeEva : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public RoguelikeEva()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SceneManager.Instance.ContentManager = Content;
            SceneManager.Instance.GraphicsManager = graphics;
            //SceneManager.Instance.WorldScale = 150;
            SceneManager.Instance.SetScene(new IntroScene());
            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            /*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/

            if (SceneManager.Instance.Exiting)
                Exit();

            SceneManager.Instance.Update(gameTime);

            // If scene has changed during this update, bail out
            /*if (!SceneManager.Instance.Update(gameTime))
                return;*/

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, SceneManager.Instance.TransformMatrix);
            SceneManager.Instance.Render(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
