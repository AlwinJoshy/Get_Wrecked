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
    static class Init
    {
        // window setup
        static public uint screenWidth = 800;
        static public uint screenHeight = 800;
        static public RenderWindow window = new RenderWindow(new VideoMode(screenWidth, screenHeight), "Micro-WAR");

        // level managenent
        public enum SceneNames
        {
            startScene,
            playScene,
            gameOverScene
        }
        static public SceneNames currentScene = 0;
        static public Sprite startImage = new Sprite(new Texture("Frontpage.png"));
        static public Sprite endImage = new Sprite(new Texture("gameOverPage.png"));
        

        // render sprite list
        static public List<SpriteRendered> allRenderers = new List<SpriteRendered>();

        //particle Animation
        static public List<SpriteAnimator> allAnimations = new List<SpriteAnimator>();

        // physics
        public enum PhysicsObjectTypes
        {
            deFault,
            player,
            enemyHot,
            enemyShock,
            enemyShoot,
            wall,
            bulletPlayer,
            bulletEnemy
        }

        // Random
        static public Random randGen = new Random();

        // time and framerate
        static public Clock mainClock = new Clock();
        static public float lastRenderTime = 0;
        static public float deltaTime = 0;
        static public float renderCountPerSecond = 1000 / 30;

        // keys and buttons
        static public bool isPressed = false;
        static public bool isUpPressed = false;
        static public bool isDownPressed = false;
        static public bool isLeftPressed = false;
        static public bool isRightPressed = false;

        // Camera and View
        static public Camera mainCamera = new Camera();


      
    }

}
