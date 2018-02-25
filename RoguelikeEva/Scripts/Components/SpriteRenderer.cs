using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components
{
    /// <summary>
    /// Handles storing and rendering of the visual representation of the GameObject.
    /// Takes scale factor of the Transform component into account.
    /// </summary>
    class SpriteRenderer : RenderableComponent, IDisposable
    {
        public Texture2D Sprite { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public float Layer { get; set; }
        public Color Color { get; set; }
        Transform Trans;

        public SpriteRenderer(Texture2D sprite)
            : this(sprite, null, .5f)
        { }

        public SpriteRenderer(Texture2D sprite, float layer)
            : this(sprite, null, layer)
        { }

        public SpriteRenderer(Texture2D sprite, Rectangle? sourceRectangle)
            : this(sprite, sourceRectangle, .5f)
        { }

        public SpriteRenderer(Texture2D sprite, Rectangle? sourceRectangle, float layer)
        {
            Sprite = sprite;
            SourceRectangle = sourceRectangle;
            Layer = layer;
            Color = Color.White;
        }

        public override void OnStart()
        {
            Trans = GetComponent<Transform>();

            if (Trans == null)
                throw new InvalidOperationException("SpriteRenderer requires a Transform.");
        }
        
        public override void Render(SpriteBatch graphics)
        {
            graphics.Draw(Sprite,
                          Trans.Position /** SceneManager.Instance.WorldScale*/,
                          SourceRectangle,
                          Color,
                          0,
                          Vector2.Zero,
                          Trans.Scale,
                          SpriteEffects.None,
                          Layer);
        }

        public void Dispose()
        {
            Sprite.Dispose();
        }
    }
}