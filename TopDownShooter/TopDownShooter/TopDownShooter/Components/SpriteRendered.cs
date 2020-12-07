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
    public class SpriteRendered
    {
        public bool moving = false;
        public bool draw;
        public int sortLayer;
        public Sprite sprite;
        public Transform transform;

        public SpriteRendered(Texture image, int sortLayer, bool state)
        {
            draw = state;
            this.sortLayer = sortLayer;
            sprite = new Sprite(image);
            Init.allRenderers.Add(this);
        }
        public void DrawSprite()
        {
            Init.window.Draw(sprite);
        }

        public void MoveSprite()
        {
            sprite.Position = this.transform.position;
            sprite.Rotation = this.transform.angle;
        }
    }
}
