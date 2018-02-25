using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vegricht.RoguelikeEva.Components.Core;
using Microsoft.Xna.Framework.Content;

namespace Vegricht.RoguelikeEva.Scenes.Core
{
    /// <summary>
    /// Handles switching between different scenes and comunication between
    /// the virtual world and Kernel object.
    /// </summary>
    sealed class SceneManager
    {
        // Is singleton
        static SceneManager _instance;
        SceneManager() { }
        
        //float _worldScale;
        List<RenderableComponent> RenderableComponents = new List<RenderableComponent>();

        public event Func<Matrix> TransformMatrixGenerator;

        public ContentManager ContentManager
        {
            get;
            set;
        }

        public GraphicsDeviceManager GraphicsManager
        {
            get;
            set;
        }

        public Rectangle ViewportRectangle
        {
            get
            {
                return GraphicsManager.GraphicsDevice.Viewport.Bounds;
            }
        }

        /*public float WorldScale
        {
            get
            {
                if (Math.Abs(_worldScale) < 1e-6)
                    _worldScale = 1;

                return _worldScale;
            }

            set
            {
                _worldScale = value;
            }
        }*/

        public Matrix TransformMatrix
        {
            get
            {
                if (TransformMatrixGenerator == null)
                    return Matrix.Identity;
                
                return TransformMatrixGenerator();
            }
        }

        public static SceneManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SceneManager();

                return _instance;
            }
        }

        public Scene CurrentScene
        {
            get;
            private set;
        }

        public bool Exiting
        {
            get;
            private set;
        }
        
        public void SetScene(Scene scene)
        {
            // Dispose of any IDisposable Component in the last scene
            if (CurrentScene != null)
                foreach (GameObject obj in CurrentScene)
                    foreach (Component component in obj)
                        if (component is IDisposable)
                            ((IDisposable)component).Dispose();

            // Singal that we want to exit
            if (scene == null)
            {
                Exiting = true;
                return;
            }

            // Clear all the GameObjects and Windows Forms Controls
            RenderableComponents.Clear();

            // Setup new scene
            CurrentScene = scene;
            CurrentScene.Initiate(ContentManager);
        }

        public void TerminateGame()
        {
            SetScene(null);
        }

        public void RegisterRenderableComponent(RenderableComponent component)
        {
            RenderableComponents.Add(component);
        }

        public bool Update(GameTime gameTime)
        {
            Scene localCurrentScene = CurrentScene;

            foreach (GameObject obj in CurrentScene)
            {
                // Update all the active GameObjects
                if (obj.Active)
                    obj.Update(gameTime);

                // If the GameObject changed the scene, bail out
                if (localCurrentScene != CurrentScene)
                    return false;
            }

            return true;
        }

        public void Render(SpriteBatch graphics)
        {
            foreach (RenderableComponent component in RenderableComponents)
                if (component.Active)
                    component.Render(graphics);
        }
    }
}