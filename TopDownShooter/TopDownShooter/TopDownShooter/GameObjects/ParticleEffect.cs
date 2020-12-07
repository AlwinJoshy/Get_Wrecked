using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;

namespace TopDownShooter
{
    public class ParticleEffect
    {
        public SpriteAnimator spriteAnim;
        public int typeId = -1;

        public ParticleEffect(Texture tex, int tpyeId)
        {
            this.typeId = tpyeId;
            this.spriteAnim = new SpriteAnimator(tex, 4, 4, 0.0001f);
            this.spriteAnim.spriteRend.sprite.Rotation = Init.randGen.Next(0, 360);
        }
    }
}
