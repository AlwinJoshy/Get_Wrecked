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
    public class Camera
    {
        public View view;
        public View guiLayer;

        public Camera()
        {
            view = new View();
        }

        public bool IsVisible(FloatRect spriteBox)
        {
            return 
                view.Center.X - view.Size.X / 2 < spriteBox.Left + spriteBox.Width &&
                view.Center.X + view.Size.X / 2 > spriteBox.Left &&
                view.Center.Y - view.Size.Y / 2 < spriteBox.Top + spriteBox.Height &&
                view.Center.Y + view.Size.Y / 2 > spriteBox.Top ? true : false;
        }

        public void SetZoom()
        {
            view.Size = new Vector2f(600, 600);
          //  view.Zoom(0.5f);
        }

        public void ResetZoom()
        {
            view.Size = new Vector2f(Init.screenWidth, Init.screenHeight);
        }

        public void GUIObject()
        {
            Init.window.SetView(guiLayer);
        }

    }

}
