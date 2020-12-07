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
    public struct AABBBox
    {
        public bool active;
        public Vector2f min;
        public Vector2f dimension;
    }
}
