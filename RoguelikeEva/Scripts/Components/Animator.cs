using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;

namespace Vegricht.RoguelikeEva.Components
{
    /// <summary>
    /// Manages swapping of sprite rendered by SpriteRenderer component
    /// defined by animations and inner state.
    /// </summary>
    class Animator : Component
    {
        public Dictionary<string, Animation> Animations { get; private set; }
        public string CurrentAnimationKey { get; private set; }

        SpriteRenderer Renderer;
        int LocalAnimationVersion;

        public Animator()
        {
            Animations = new Dictionary<string, Animation>();
        }

        public void Play(string key)
        {
            if (key == null)
                throw new ArgumentNullException();

            if (!Animations.ContainsKey(key))
                throw new InvalidOperationException("There is no animation with key \"" + key + "\" in this Animator.");

            // Reset the animation, so it can be played again, in case it's been already played before
            CurrentAnimationKey = key;
            Animations[CurrentAnimationKey].Reset();
        }

        public override void OnStart()
        {
            Renderer = GetComponent<SpriteRenderer>();

            if (Renderer == null)
                throw new InvalidOperationException("Animator requires a SpriteRenderer.");

            if (Animations.Count == 0)
                throw new InvalidOperationException("Animator requires at least one animation.");

            if (CurrentAnimationKey == null)
                throw new ArgumentNullException("Animator::Play(String) has to be called at leas once before adding Animator component to a GameObject");
        }

        public override void Update(GameTime gameTime)
        {
            Animations[CurrentAnimationKey].Update(gameTime.ElapsedGameTime.Milliseconds);

            // If the sprite has changed, distribute the change into the SpriteRenderer component
            if (LocalAnimationVersion != Animations[CurrentAnimationKey].Version)
            {
                Renderer.Sprite = Animations[CurrentAnimationKey].CurrentSprite;
                Renderer.SourceRectangle = Animations[CurrentAnimationKey].CurrentSourceRectangle;
            }

            LocalAnimationVersion = Animations[CurrentAnimationKey].Version;
        }
    }
}