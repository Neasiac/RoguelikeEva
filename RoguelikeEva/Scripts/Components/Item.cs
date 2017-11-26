using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components
{
    class Item : Component
    {
        Action<Character> Effect;

        public Item(Action<Character> effect)
        {
            Effect = effect ?? throw new ArgumentNullException();
        }

        public void Use(Character chara)
        {
            Effect(chara);
            Parent.Active = false;
        }
    }
}