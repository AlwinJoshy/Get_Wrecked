using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using SFML.System;

namespace TopDownShooter
{
    public class SpriteAnimator
    {
        public SpriteRendered spriteRend;
        public int gridX;
        public int gridY;
        public float scale;
        private int currentX = 0;
        private int currentY = 0;
        private IntRect spriteBound;
        private float frameStepTime;
        private float nextFrameTime;
        public bool animate = false;

        public SpriteAnimator(Texture tex, int gridX, int gridY, float frameStepTime)
        {
            spriteRend = new SpriteRendered(tex, 3, true);
            spriteRend.sprite.Texture = tex;
            spriteRend.draw = false;
            spriteBound = new IntRect(0, 0, (int)tex.Size.X / gridX, (int)tex.Size.Y / gridY);
            spriteRend.sprite.TextureRect = spriteBound;
            spriteRend.sprite.Origin = new Vector2f((tex.Size.X / gridX ) * 0.5f, (tex.Size.Y / gridY) * 0.5f);

            this.gridX = gridX;
            this.gridY = gridY;

            this.frameStepTime = frameStepTime;
            Init.allAnimations.Add(this);
        }

        public void Start(Vector2f position)
        {
            spriteRend.sprite.Position = position;
            spriteRend.draw = true;
            currentX = 0;
            currentY = 0;
            animate = true;
        }

        public void Draw()
        {
            if (animate)
            {
                if (Init.mainClock.ElapsedTime.AsSeconds() >= nextFrameTime)
                {
                    spriteBound.Left = spriteBound.Width * currentX;
                    spriteBound.Top = spriteBound.Height * currentY;
                    spriteRend.sprite.TextureRect = spriteBound;

                    if (currentX < gridX - 1)
                    {
                        currentX++;
                    }
                    else
                    {
                        currentX = 0;
                        currentY++; 
                    }

                    if (currentX == gridX - 1 && currentY == gridY)
                    {
                        animate = false;
                        spriteRend.draw = false;
                    }

                    nextFrameTime = Init.mainClock.ElapsedTime.AsSeconds() + frameStepTime;
                }
            }
        }

    }
}
