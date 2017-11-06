using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace Vegricht.RoguelikeEva.Components.Core
{
    /// <summary>
    /// Acestor to all components which would like to be rendered.
    /// </summary>
    abstract class RenderableComponent : Component
    {
        abstract public void Render(SpriteBatch graphics);
    }
}