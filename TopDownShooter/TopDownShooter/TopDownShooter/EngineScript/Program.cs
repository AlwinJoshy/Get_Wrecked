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
    class Program
    {
        static public bool mousePressed = false;
        static private Music gameMusic = new Music("8BitMusic.ogg");

        static void Main(string[] args)
        {
            Init.window.KeyPressed += Window_KeyPressed;
            Init.window.KeyReleased += Window_KeyReleased;
            Init.window.MouseButtonPressed += Window_MouseButtonPressed;
            Init.window.MouseButtonReleased += Window_MouseButtonReleased;

            gameMusic.Loop = true;
            gameMusic.Play();

            Console.WriteLine("Programe Running...");
            Gizmo.StartGizmo();
            RandomSceneMaker.InitActions();
            GUIManager.GUISetUp();

            while (Init.window.IsOpen)
            {
                Init.window.DispatchEvents();
                // render section
                while (Init.mainClock.ElapsedTime.AsMilliseconds() >= Init.renderCountPerSecond + Init.lastRenderTime)
                {
                    Init.lastRenderTime = Init.mainClock.ElapsedTime.AsMilliseconds();

                    if (Init.currentScene == Init.SceneNames.startScene)
                    {
                        Init.window.Clear();
                        Init.window.Draw(Init.startImage);
                        Init.window.Display();
                    }

                    else if (Init.currentScene == Init.SceneNames.playScene)
                    {
                        RandomSceneMaker.GameFunctions();
                        Init.window.Clear();
                //        Init.window.SetView(Init.mainCamera.view);
                        AnimateObjects();
                        DrawObjects();
                        // physics calculation function
                        Init.deltaTime = Init.mainClock.ElapsedTime.AsMilliseconds();
                        Physics.allActions();
                        GUIManager.GUIDraw();
                        Gizmo.BufferPointsDraw();
                        Init.window.Display();
                    }
                    else if (Init.currentScene == Init.SceneNames.gameOverScene)
                    {
                        Init.window.Clear();
                        Init.window.Draw(Init.endImage);
                        Init.window.Display();
                    }
                }
            }
        }

        private static void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (mousePressed) mousePressed = false;
        }

        private static void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!mousePressed)
            {
                RandomSceneMaker.playerObj.ShootBullet();
                mousePressed = true;
            }
        }

        private static void Window_KeyReleased(object sender, KeyEventArgs e)
        {
            // one press buttons
            if (Init.isPressed)
            {
                Init.isPressed = false;
            }
            if (Init.isUpPressed && e.Code == Keyboard.Key.W) Init.isUpPressed = false;
            if (Init.isDownPressed && e.Code == Keyboard.Key.S) Init.isDownPressed = false;
            if (Init.isRightPressed && e.Code == Keyboard.Key.D) Init.isRightPressed = false;
            if (Init.isLeftPressed && e.Code == Keyboard.Key.A) Init.isLeftPressed = false;
        }

        private static void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            // one press buttons
            if (!Init.isPressed)
            {
                Init.isPressed = true;

                if (e.Code == Keyboard.Key.R)
                {
                    GenerateLevel();
                }
                else if (e.Code == Keyboard.Key.G)
                {
                    Gizmo.drawGizmo = !Gizmo.drawGizmo;
                }
                else if (e.Code == Keyboard.Key.J)
                {
                    Init.window.Close();
                }

            }
            if (!Init.isUpPressed && e.Code == Keyboard.Key.W) Init.isUpPressed = true;
            if (!Init.isDownPressed && e.Code == Keyboard.Key.S) Init.isDownPressed = true;
            if (!Init.isRightPressed && e.Code == Keyboard.Key.D) Init.isRightPressed = true;
            if (!Init.isLeftPressed && e.Code == Keyboard.Key.A) Init.isLeftPressed = true;
        }

        static private void AnimateObjects()
        {
            for (int i = 0; i < Init.allAnimations.Count; i++)
            {
                Init.allAnimations[i].Draw();
            }
        }

        static private void DrawObjects()
        {
            // draw background sprites
            for (int n = 0; n < 5; n++)
            {
                for (int i = 0; i < Init.allRenderers.Count; i++)
                {
                    if (Init.allRenderers[i].draw && Init.allRenderers[i].sortLayer == n)
                    {
                        if (Init.allRenderers[i].moving) Init.allRenderers[i].MoveSprite();

                        if (Init.mainCamera.IsVisible(Init.allRenderers[i].sprite.GetGlobalBounds()))
                        {
                            Init.allRenderers[i].DrawSprite();
                        }
                    }
                }
            }

        }
        static private void GenerateLevel()
        {
            RandomSceneMaker.GenerateLevel();
        //    RandomSceneMaker.makeWaypoints();
        }
    }
}
