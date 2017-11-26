using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components
{
    class FontRenderer : RenderableComponent
    {
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public float Layer { get; set; }
        public Color Color { get; set; }
        Transform Trans;
        
        public FontRenderer(SpriteFont font, string text)
            : this(font, text, .5f)
        { }

        public FontRenderer(SpriteFont font, string text, float layer)
        {
            Font = font;
            Text = text;
            Layer = layer;
            Color = Color.White;
        }

        public override void OnStart()
        {
            Trans = GetComponent<Transform>();

            if (Trans == null)
                throw new InvalidOperationException("FontRenderer requires a Transform.");
        }
        
        public override void Render(SpriteBatch graphics)
        {
            graphics.DrawString(Font,
                                Text,
                                Trans.Position,
                                Color,
                                0,
                                Vector2.Zero,
                                Trans.Scale,
                                SpriteEffects.None,
                                Layer);
        }
    }
}