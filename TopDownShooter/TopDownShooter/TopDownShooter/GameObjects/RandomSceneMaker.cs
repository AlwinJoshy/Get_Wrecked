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
    static public class RandomSceneMaker
    {
        // Room Generation variables
        static public int tileSize = 128;
        static public int sceneSize = 50;
        static public List<Sprite> floorTiles = new List<Sprite>();
        static public List<Wall> wallTiles = new List<Wall>();
        public enum LevelTileType
        {
            blank,
            floor
        };
        static public LevelTileType[,] sceneTile;
        public class Waypoint
        {
            public bool active;
            public int gridX;
            public int gridY;
            public Vector2f position;
            public Color col;
            public int gCost;
            public int hCost;
            public Vector2i parentId;


            public int fCost
            {
                get
                {
                    return gCost + hCost;
                }
            }
        }
        static public Waypoint[,] movePoints;
        static public Texture[] floorTileTypes = { new Texture("roomTile_1.png"),
                                              new Texture("roomTile_2.png"),
                                              new Texture("roomTile_3.png"),
                                              new Texture("roomTile_4.png")};
        public struct SceneWalker
        {
            public Vector2i position;
            public Vector2i direction;
        }

        static public List<SceneWalker> walkerList = new List<SceneWalker>();
        static public int maxWalkerCount = 30; // total chanceSpan
        static public int chanceSpan = 100; // total chanceSpan
        static public int dirShiftChance = 10; // out of chanceSpan
        static public int walkerKillChance = 3; // out of chanceSpan
        static public int walkerMakeChance = 20; // out of chanceSpan
        static public int totalWalkerIteration = 20; // total level generation action

        // player
        static public Vector2f playerPos = new Vector2f((64 * 50 / 2) + tileSize / 2, (64 * 50 / 2) + tileSize / 2);
        static public Player playerObj = new Player(playerPos);
        static public float maxHealth = 100;
        static public float currentHealth = 100;
        static public float totalScore = 0;

        // Enemy
        static public int pawnPointCount;
        static public int maxSpawnCount = 10;
        static public int spawnPointChance = 5;
        static public Vector2f[] spawnPoint = new Vector2f[5];
        static public Enemy[] AllEnemie = new Enemy[maxSpawnCount];
        static public Bullet[] bulletList = new Bullet[60];

        static public List<ParticleEffect> blastAndExplosions = new List<ParticleEffect>();

        static public void InitActions()
        {
            for (int i = 0; i < bulletList.Length; i++)
            {
                bulletList[i] = new Bullet(10, (int)Init.PhysicsObjectTypes.bulletEnemy, new Texture("enemy_bullet.png"));
            }
            
            sceneTile = new LevelTileType[sceneSize, sceneSize];
            movePoints = new Waypoint[sceneSize, sceneSize];


            // creates all enemies
            for (int i = 0; i < maxSpawnCount; i++)
            {
                int enemyType = Init.randGen.Next(0, 3);

                switch (enemyType)
                {
                    case 0:
                        AllEnemie[i] = new Enemy((int)Init.PhysicsObjectTypes.enemyHot, new Vector2f(0, 0));
                        break;

                    case 1:
                        AllEnemie[i] = new Enemy((int)Init.PhysicsObjectTypes.enemyShock, new Vector2f(0, 0));
                        break;

                    case 2:
                        AllEnemie[i] = new Enemy((int)Init.PhysicsObjectTypes.enemyShoot, new Vector2f(0, 0));
                        break;
                }
            }
            // create blue bullets
            for (int i = 0; i < 10; i++)
            {
                blastAndExplosions.Add(new ParticleEffect(new Texture("playerBulletBlast.png"), (int)Init.PhysicsObjectTypes.bulletPlayer));
            }

            for (int i = 0; i < 10; i++)
            {
                blastAndExplosions.Add(new ParticleEffect(new Texture("enemyrBulletBlast.png"), (int)Init.PhysicsObjectTypes.bulletEnemy));
            }
        }

        static public void OffAllBullets()
        {
            for (int i = 0; i < bulletList.Length; i++)
            {
                bulletList[i].rb.active = false;
                bulletList[i].spriteRend.draw = false;
            }
        }

        static public void bulletBlast(int typeId, Vector2f position)
        {
            for (int i = 0; i < blastAndExplosions.Count; i++)
            {
                if (typeId == blastAndExplosions[i].typeId && !blastAndExplosions[i].spriteAnim.animate)
                {
                    blastAndExplosions[i].spriteAnim.Start(position);
                    break;
                }
            }
        }


        static public void GameFunctions()
        {
            playerObj.Move(Init.isUpPressed, Init.isDownPressed, Init.isLeftPressed, Init.isRightPressed);

            for (int i = 0; i < AllEnemie.Length; i++)
            {
                if (AllEnemie[i].alive)
                {
                    AllEnemie[i].EnemyAIActions();
                }
            }
            // set camera to palyer position
            Init.mainCamera.view.Center = playerObj.transform.position;
        }

        static public void GenerateLevel()
        {
            Init.currentScene = Init.SceneNames.playScene;
            totalScore = 0;
            currentHealth = maxHealth;

            // level generation function
            DisableAllEnemy();
            Init.mainCamera.SetZoom();
            OffAllBullets();
            SetupTileGrid();
            CreateFloorTiles();
            CreateWalls();
            SetTiles();
            makeWaypoints();
            SpawnEnemy();
            
        }

        static private Vector2i RandomDir()
        {
            int directionNumber = Init.randGen.Next(0, 4);
            Vector2i sendDir;
            switch (directionNumber)
            {
                case 0: // move right
                    sendDir = new Vector2i(1, 0);
                    break;

                case 1: // move up
                    sendDir = new Vector2i(0, 1);
                    break;

                case 2: // move left
                    sendDir = new Vector2i(-1, 0);
                    break;

                default: // move down
                    sendDir = new Vector2i(0, -1);
                    break;
            }

            return sendDir;

        }

        static private void SetupTileGrid()
        {
            for (int n = 0; n < sceneSize; n++)
            {
                // moves in x axis
                for (int i = 0; i < sceneSize; i++)
                {
                    // moves in y axis
                    // sets all tile id to blank
                    sceneTile[n, i] = LevelTileType.blank;
                }
            }

            // create first walker
            SceneWalker walker = new SceneWalker();
            walker.direction = RandomDir();
            walker.position = new Vector2i((int)(sceneSize / 2), (int)(sceneSize / 2));
            walkerList.Clear();
            walkerList.Add(walker);
        }

        static private void CreateFloorTiles()
        {
            int loopCount = 0;
            do
            {
                // create floor tile
                foreach (SceneWalker selectedWalker in walkerList)
                {
                    // create floor on walker position
                    sceneTile[selectedWalker.position.X, selectedWalker.position.Y] = LevelTileType.floor;
                }

                // remove SceneWalkers
                for (int j = 0; j < walkerList.Count; j++)
                {
                    if (Init.randGen.Next(0, chanceSpan) < walkerKillChance && walkerList.Count > 1)
                    {
                        walkerList.RemoveAt(j);
                        break;
                    }
                }
                // SeneWalker Change Direction
                for (int j = 0; j < walkerList.Count; j++)
                {
                    if (Init.randGen.Next(0, chanceSpan) < dirShiftChance)
                    {
                        SceneWalker newSceneWalker = walkerList[j];
                        newSceneWalker.direction = RandomDir();
                        walkerList[j] = newSceneWalker;
                    }
                }

                // Add new SeneWalker
                for (int j = 0; j < walkerList.Count; j++)
                {
                    if (Init.randGen.Next(0, chanceSpan) < walkerMakeChance && walkerList.Count < maxWalkerCount)
                    {
                        SceneWalker newSceneWalker = new SceneWalker();
                        newSceneWalker.direction = RandomDir();
                        newSceneWalker.position = walkerList[j].position;
                        walkerList.Add(newSceneWalker);
                    }
                }

                // move SceneWalker
                for (int i = 0; i < walkerList.Count; i++)
                {
                    SceneWalker newSceneWalker = walkerList[i];
                    newSceneWalker.position = walkerList[i].position + walkerList[i].direction;
                    walkerList[i] = newSceneWalker;
                }

                // clamp scene walker

                for (int i = 0; i < walkerList.Count; i++)
                {
                    Vector2i walkerPosition = walkerList[i].position;

                    // clamps the position
                    if (walkerPosition.X <= 0) walkerPosition.X = 1;
                    else if (walkerPosition.X >= sceneSize) walkerPosition.X = sceneSize - 1;
                    if (walkerPosition.Y <= 0) walkerPosition.Y = 1;
                    else if (walkerPosition.Y >= sceneSize) walkerPosition.Y = sceneSize - 1;
                    SceneWalker newSceneWalker = walkerList[i];
                    newSceneWalker.position = walkerPosition;
                    walkerList[i] = newSceneWalker;
                }
                loopCount++;
            }
            while (loopCount < totalWalkerIteration);

        }

        static private void CreateWalls()
        {
            Vector2f pos = new Vector2f();
            Texture wallTopLeft = new Texture("roomwall_topLeft.png");
            Texture wallTopRight = new Texture("roomWall_topRight.png");
            Texture walldownLeft = new Texture("roomwall_downLeft.png");
            Texture walldownRight = new Texture("roomWall_downRight.png");
            Texture wallLeft = new Texture("roomwall_left.png");
            Texture wallRight = new Texture("roomWall_right.png");
            Texture wallTop = new Texture("roomWall_top.png");
            Texture wallBottom = new Texture("roomWall_down.png");

            for (int k = 0; k < wallTiles.Count; k++)
            {
                wallTiles[k].hideWall();
            }

            for (int i = 0; i < sceneSize; i++)
            {
                for (int n = 0; n < sceneSize; n++)
                {
                    pos.X = i; pos.Y = n; pos *= tileSize;

                    // make top left corner
                    if (n + 1 < sceneSize &&
                        i + 1 < sceneSize &&
                        sceneTile[i + 1, n] == LevelTileType.blank &&
                        sceneTile[i, n + 1] == LevelTileType.blank &&
                        sceneTile[i + 1, n + 1] == LevelTileType.floor &&
                        sceneTile[i, n] == LevelTileType.blank)
                    {
                        if (!lookForUsedWalls(pos, wallTopLeft)) wallTiles.Add(new Wall(wallTopLeft, pos));
                    }

                    // make top right corner
                    if (n + 1 < sceneSize &&
                        i - 1 >= 0 &&
                        sceneTile[i, n] == LevelTileType.blank &&
                        sceneTile[i - 1, n] == LevelTileType.blank &&
                        sceneTile[i, n + 1] == LevelTileType.blank &&
                        sceneTile[i - 1, n + 1] == LevelTileType.floor)
                    {
                        if (!lookForUsedWalls(pos, wallTopRight)) wallTiles.Add(new Wall(wallTopRight, pos));
                    }


                    // make bottom left corner
                    if (n - 1 >= 0 &&
                        i + 1 < sceneSize &&
                        sceneTile[i + 1, n] == LevelTileType.blank &&
                        sceneTile[i, n - 1] == LevelTileType.blank &&
                        sceneTile[i + 1, n - 1] == LevelTileType.floor &&
                        sceneTile[i, n] == LevelTileType.blank)
                    {
                        if (!lookForUsedWalls(pos, walldownLeft)) wallTiles.Add(new Wall(walldownLeft, pos));
                    }

                    // make bottom right corner
                    if (n - 1 >= 0 &&
                        i - 1 >= 0 &&
                        sceneTile[i - 1, n] == LevelTileType.blank &&
                        sceneTile[i, n - 1] == LevelTileType.blank &&
                        sceneTile[i - 1, n - 1] == LevelTileType.floor &&
                        sceneTile[i, n] == LevelTileType.blank)
                    {
                        if (!lookForUsedWalls(pos, walldownRight)) wallTiles.Add(new Wall(walldownRight, pos));
                    }

                    // right wall
                    if (i + 1 < sceneSize && sceneTile[i + 1, n] == LevelTileType.floor && sceneTile[i, n] == LevelTileType.blank)
                    {
                        if (!lookForUsedWalls(pos, wallLeft)) wallTiles.Add(new Wall(wallLeft, pos));
                    }
                    // left wall
                    if (i - 1 >= 0 && sceneTile[i - 1, n] == LevelTileType.floor && sceneTile[i, n] == LevelTileType.blank)
                    {
                        if (!lookForUsedWalls(pos, wallRight)) wallTiles.Add(new Wall(wallRight, pos));
                    }
                    // top wall
                    if (n + 1 < sceneSize && sceneTile[i, n + 1] == LevelTileType.floor && sceneTile[i, n] == LevelTileType.blank)
                    {
                        if (!lookForUsedWalls(pos, wallTop)) wallTiles.Add(new Wall(wallTop, pos));
                    }
                    // bottom wall
                    if (n - 1 >= 0 && sceneTile[i, n - 1] == LevelTileType.floor && sceneTile[i, n] == LevelTileType.blank)
                    {
                        if (!lookForUsedWalls(pos, wallBottom)) wallTiles.Add(new Wall(wallBottom, pos));
                    }
                }
            }
        }

        static private bool lookForUsedWalls(Vector2f pos, Texture img)
        {
            for (int i = 0; i < wallTiles.Count; i++)
            {
                if (wallTiles[i].spriteRend.draw == false)
                {
                    wallTiles[i].resetWall(pos, img);
                    return true;
                }
            }
            return false;
        }

        static private void SetTiles()
        {
            int spawnPointCount = 0;
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                spawnPoint[i] = new Vector2f(-1, -1);
            }

            for (int i = 0; i < Init.allRenderers.Count; i++)
            {
                if (Init.allRenderers[i].sortLayer == 0) Init.allRenderers[i].draw = false;
            }

            for (int i = 0; i < sceneSize; i++)
            {
                for (int n = 0; n < sceneSize; n++)
                {
                    bool foundBlank = false;
                    // create Floor tiles
                    if (sceneTile[i, n] == LevelTileType.floor)
                    {
                        // check of there is already floor tile.
                        for (int k = 0; k < Init.allRenderers.Count; k++)
                        {
                            if (Init.allRenderers[k].sortLayer == 0 && !Init.allRenderers[k].draw)
                            {
                                Init.allRenderers[k].draw = true;
                                Init.allRenderers[k].sprite.Texture = floorTileTypes[Init.randGen.Next(0, floorTileTypes.Length)];
                                Init.allRenderers[k].sprite.Position = new Vector2f(i * tileSize, n * tileSize);
                                foundBlank = true;
                                break;
                            }
                        }
                        if (!foundBlank)
                        {
                            SpriteRendered tile = new SpriteRendered(floorTileTypes[Init.randGen.Next(0, floorTileTypes.Length)], 0, true);
                            Init.allRenderers.Add(tile);
                            tile.sprite.Position = new Vector2f(i * tileSize, n * tileSize);
                        }

                        if (Init.randGen.Next(0, chanceSpan) < spawnPointChance && spawnPointCount < maxSpawnCount)
                        {
                            spawnPointCount += 1;
                            for (int j = 0; j < spawnPoint.Length; j++)
                            {
                                if (spawnPoint[j] == new Vector2f(-1, -1))
                                {
                                    spawnPoint[j] = new Vector2f((i * tileSize) + (tileSize / 2), (n * tileSize) + (tileSize / 2));
                                    
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            playerObj.transform.position = new Vector2f(tileSize * (sceneSize / 2) + tileSize / 2, tileSize * (sceneSize / 2) + tileSize / 2);
            Console.WriteLine(Init.allRenderers.Count);
            walkerList.Clear();
            Console.WriteLine(spawnPointCount + " spawn point created...");
        }


        static private void DisableAllEnemy()
        {
            for (int i = 0; i < AllEnemie.Length; i++)
            {
                AllEnemie[i].Disable();
            }
        }

        static private void SpawnEnemy()
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                if (spawnPoint[i] != new Vector2f(-1, -1))
                {
                    for (int n = 0; n < 20; n++)
                    {
                        int randEnemyNumber = Init.randGen.Next(0, AllEnemie.Length);
                        if (!AllEnemie[randEnemyNumber].alive)
                        {
                            AllEnemie[randEnemyNumber].spawnEnemy(spawnPoint[i]);
                            break;
                        }
                    }
                }
            }
        }

        static private int lastRespawnSpot = -1;
        static public void RespawnEnemy()
        {
            int crashProofCount = 0;
            while (true)
            {
                int i = Init.randGen.Next(0, AllEnemie.Length);
                if (!AllEnemie[i].alive)
                {
                    while (true)
                    {
                        int n = Init.randGen.Next(0, spawnPoint.Length);
                        if (n != lastRespawnSpot && spawnPoint[n] != new Vector2f(-1, -1))
                        {
                            if (!Init.mainCamera.IsVisible(new FloatRect(spawnPoint[n].X, spawnPoint[n].Y, 0, 0)))
                            {
                                AllEnemie[i].spawnEnemy(spawnPoint[n]);
                                lastRespawnSpot = n;
                                break;
                            }
                        }

                        else if (crashProofCount >= 100)
                        {
                            AllEnemie[i].spawnEnemy(spawnPoint[lastRespawnSpot]);
                            break;
                        }
                        crashProofCount++;
                    }
                    break;
                }
            }
        }
        /*
                    CircleShape spot = new CircleShape();
                    spot.Radius = 5;
                    spot.Origin = new Vector2f(5, 5);
                    for (int i = 0; i < sceneSize; i++)
                    {
                        for (int n = 0; n < sceneSize; n++)
                        {
                            if (movePoints[n, i].active)
                            {
                        //        spot.FillColor = movePoints[n, i].col;
                       //         spot.Position = movePoints[n, i].position;
                                Gizmo.DrawPoint(movePoints[n, i].position);
                            }
                        }
                    }

                }*/

        static private void TurnOffAllPoints()
        {
            for (int i = 0; i < sceneSize; i++)
            {
                for (int n = 0; n < sceneSize; n++)
                {
                    if (movePoints[n, i].active)
                    {
                        movePoints[n, i].col = Color.Black;
                    }
                }
            }
        }

        // A* path finding
       

        static public List<Vector2f> GeneratePath(Transform startPoint, Transform endPoint)
        {
            List<Waypoint> needToCheckPoints = new List<Waypoint>();
            List<Waypoint> closedPoints = new List<Waypoint>();
            List<Vector2f> wayPointList = new List<Vector2f>();

            int count = 0;

            // calculate waypint grid id
            Vector2i startPointId = new Vector2i((int)startPoint.position.X / tileSize, (int)startPoint.position.Y / tileSize);
            Vector2i endPointId = new Vector2i((int)endPoint.position.X / tileSize, (int)endPoint.position.Y / tileSize);
            Waypoint mp;
            mp = movePoints[startPointId.X, startPointId.Y];
            if (movePoints[startPointId.X, startPointId.Y].active == false) Console.WriteLine("error call...");
            needToCheckPoints.Add(item : movePoints[startPointId.X, startPointId.Y]);

            while (needToCheckPoints.Count > 1 || count <= 50)
            {
                count++;
                // no solution found
                if(needToCheckPoints.Count == 0) Console.WriteLine(" while Count = " + count + "the entity grid pos & state" + startPointId + " " + movePoints[startPointId.X, startPointId.Y].active);
                Waypoint currentNode = needToCheckPoints[0];

                for (int i = 1; i < needToCheckPoints.Count; i++)
                {
                    // select the one withe the lowest hurestic cost
                    if (needToCheckPoints[i].fCost == currentNode.fCost && needToCheckPoints[i].hCost < currentNode.hCost)
                    {
                        currentNode = needToCheckPoints[i];
                    }

                    // select the one withe the lowest f cost
                    else if (needToCheckPoints[i].fCost < currentNode.fCost)
                    {
                        currentNode = needToCheckPoints[i];
                    }
                }

                closedPoints.Add(currentNode);
                needToCheckPoints.Remove(currentNode);


                if (movePoints[endPointId.X, endPointId.Y].position == currentNode.position)
                {
                    wayPointList.Clear();
                    movePoints[endPointId.X, endPointId.Y].col = Color.Magenta;
                    wayPointList.Add(movePoints[endPointId.X, endPointId.Y].position);
                    Vector2i parentGridId = movePoints[endPointId.X, endPointId.Y].parentId;
                    while (movePoints[parentGridId.X, parentGridId.Y].position
                        != movePoints[startPointId.X, startPointId.Y].position)
                    {
                        wayPointList.Add(movePoints[parentGridId.X, parentGridId.Y].position);
                        movePoints[parentGridId.X, parentGridId.Y].col = Color.Yellow;
                        parentGridId = movePoints[parentGridId.X, parentGridId.Y].parentId;
                    }
                    movePoints[startPointId.X, startPointId.Y].col = Color.Magenta;
             //       wayPointList.Add(movePoints[startPointId.X, startPointId.Y].position);
                    wayPointList.Reverse();
               
                    return wayPointList;
                }


                foreach (Waypoint wp in GetKins(new Vector2i(currentNode.gridX, currentNode.gridY)))
                {
                    if (!wp.active || closedPoints.Contains(wp)) continue;
                    int neiborMoveCost = currentNode.gCost + FindDistance(new Vector2i(currentNode.gridX, currentNode.gridY), new Vector2i(wp.gridX, wp.gridY));

                    if (neiborMoveCost < wp.gCost || !needToCheckPoints.Contains(wp))
                    {
                        movePoints[wp.gridX, wp.gridY].gCost = neiborMoveCost;
                        movePoints[wp.gridX, wp.gridY].hCost = FindDistance(new Vector2i(wp.gridX, wp.gridY), new Vector2i(endPointId.X, endPointId.Y));
                        movePoints[wp.gridX, wp.gridY].parentId = new Vector2i(currentNode.gridX, currentNode.gridY);
                        movePoints[wp.gridX, wp.gridY].col = Color.Blue;
                        if (!needToCheckPoints.Contains(movePoints[wp.gridX, wp.gridY]))
                        {
                            needToCheckPoints.Add(movePoints[wp.gridX, wp.gridY]);
                        }
                    }
                }

            }
            Console.WriteLine("total path generation call total :" + count);
            return null;
        }

        static private int FindDistance(Vector2i posNodeA, Vector2i posNodeB)
        {
            int xDistance = Math.Abs(posNodeA.X - posNodeB.X);
            int yDistance = Math.Abs(posNodeA.Y - posNodeB.Y);

            if (xDistance > yDistance)
            {
                return (int)(14f * yDistance) + (xDistance - yDistance) * 10;
            }
            else
            {
                return (int)(14f * xDistance) + (yDistance - xDistance) * 10;
            }
        }

        static private List<Waypoint> GetKins(Vector2i pos)
        {
            List<Waypoint> kins = new List<Waypoint>();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if ((x == 0 && y == 0) || pos.X == 0 || pos.Y == 0) continue;
                        kins.Add(movePoints[pos.X + x, pos.Y + y]);
                }
            }
            return kins;
        }


        static public void makeWaypoints()
        {
            for (int n = 0; n < sceneSize; n++)
            {
                // moves in x axis
                for (int i = 0; i < sceneSize; i++)
                {
                    movePoints[n, i] = new Waypoint();
                    // moves in y axis
                    // sets all tile id to blank
                    if (sceneTile[n, i] == LevelTileType.blank)
                    {
                        movePoints[n, i].active = false;
                    }
                    else
                    {
                        movePoints[n, i].active = true;
                        movePoints[n, i].gridX = n;
                        movePoints[n, i].gridY = i;
                        movePoints[n, i].position = new Vector2f(n * tileSize + tileSize * 0.5f, i * tileSize + tileSize * 0.5f);
                        movePoints[n, i].col = Color.Black;
                    }
                }
            }
        }

        static public void ChangeHealth(int value)
        {
            currentHealth += value;

            if (currentHealth > 100) currentHealth = 100;
            else if (currentHealth < 0) currentHealth = 0;

            if (currentHealth == 0)
            {
                Init.mainCamera.ResetZoom();
                Init.mainCamera.view.Center = new Vector2f(Init.screenWidth/2, Init.screenHeight / 2);
                Init.currentScene = Init.SceneNames.gameOverScene;
            }
        }

    }
}

