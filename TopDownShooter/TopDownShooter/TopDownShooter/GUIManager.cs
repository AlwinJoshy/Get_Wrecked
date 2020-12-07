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
    static class GUIManager
    {

        static View guiLayer;

        static private RectangleShape healthBarBackground;
        static private RectangleShape healthBar;

        static private Text screenText;
        static private Text scoreText;

        static public void GUISetUp()
        {
            guiLayer = new View();
            healthBarBackground = new RectangleShape();
            healthBarBackground.FillColor = Color.Red;
            healthBarBackground.Position = new Vector2f(10, 15);
            healthBarBackground.Size = new Vector2f(200, 10);
            healthBarBackground.OutlineThickness = 2;
            healthBarBackground.OutlineColor = new Color(128, 128, 128);

            healthBar = new RectangleShape();
            healthBar.FillColor = Color.Green;
            healthBar.Position = new Vector2f(10, 15);
            healthBar.Size = new Vector2f(200, 10);

            screenText = new Text("....",new Font("OpenSansRegular.ttf"));
            screenText.Position = new Vector2f(220, 0);

            scoreText = new Text("Total Score : 0000 ", new Font("OpenSansRegular.ttf"));
            scoreText.Position = new Vector2f(420, 0);
        }


        static public void GUIDraw()
        {

            //  Calculate Healthbar Length
            healthBar.Size = new Vector2f(200 * (Math.Max(RandomSceneMaker.currentHealth, 0) / RandomSceneMaker.maxHealth), 10);

            screenText.DisplayedString = RandomSceneMaker.currentHealth.ToString();

            scoreText.DisplayedString = "Total Score : " + RandomSceneMaker.totalScore;

            // Draw to last GUI layer
            Init.window.SetView(guiLayer);
            Init.window.Draw(healthBarBackground);
            Init.window.Draw(healthBar);

            Init.window.Draw(screenText);

            Init.window.Draw(scoreText);
            Init.window.SetView(Init.mainCamera.view);
        }

    }
}
