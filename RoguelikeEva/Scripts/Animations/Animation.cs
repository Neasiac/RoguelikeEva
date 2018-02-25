using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Vegricht.RoguelikeEva.Animations
{
    /// <summary>
    /// Describes an animation and handles updates of its frames.
    /// </summary>
    class Animation
    {
        public List<Frame> Frames
        {
            get;
            private set;
        }

        public Texture2D CurrentSprite
        {
            get
            {
                return Frames[CurrentFrameId].Sprite;
            }
        }

        public Rectangle? CurrentSourceRectangle
        {
            get
            {
                return Frames[CurrentFrameId].SourceRectangle;
            }
        }

        public int Length
        {
            get
            {
                int total = 0;

                foreach (Frame frame in Frames)
                    total += frame.Duration;

                return total;
            }
        }

        // Indicates change of the current sprite since last update
        public int Version
        {
            get;
            private set;
        }

        int CurrentFrameId;
        float TimePassed;

        public Animation(IEnumerable<Frame> frames)
        {
            Frames = new List<Frame>(frames);
        }

        public Animation()
        {
            Frames = new List<Frame>();
        }
        
        public void Reset()
        {
            CurrentFrameId = 0;
            TimePassed = 0;
            Version++;
        }

        public void Update(float deltaTime)
        {
            TimePassed += deltaTime;

            // Should we proceed to the next frame?
            if (Frames[CurrentFrameId].Duration <= TimePassed)
            {
                TimePassed = 0;
                Version++;

                // Wrap around = repeat the animation
                if (++CurrentFrameId == Frames.Count)
                    CurrentFrameId = 0;
            }
        }
    }
}