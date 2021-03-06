using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace majorProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //menu variables
        private bool displayMenu = true;
        private bool subMenu = false;
        public Texture2D singlePix;
        private int selected = 0;
        private bool move = true;
        private bool select = true;
        private bool gameOver = false;
        private bool win = false;
        private int maxSelection = 2;
        private int minSelection = 0;

        // Time variables
        private float offset;
        private float time;
        private Stopwatch gameOverTimer = new Stopwatch();

        //Globals
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        //Effects
        public ArrayList explosionList = new ArrayList();
        

        // Music
        public Song bsong;

        //player
        Player human;

        //enemy list
        public ArrayList enemyList = new ArrayList();
        private Enemy[] activeList;
        public ArrayList removeList = new ArrayList();

        private EnemyShot[] shotList;

        // Sprites
        Texture2D enemyText;
        Texture2D enemyShot;
        Texture2D shotTexture;
        Texture2D humanTexture;
        Texture2D backgroundTexture;
        Texture2D explosionTexture;


        Boss boss;
        AnimatedSprite humanAnimatedTexture;

        // Fonts
        SpriteFont titleFont;
        SpriteFont font;

        public bool levelComplete = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // Define Window here
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Window.Title = ("Major Project");
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            LoadContent();
            Constants constants = new Constants();
            activeList = new Enemy[20];
            shotList = new EnemyShot[100000];

            // Create Level Reader
            LevelReader reader = new LevelReader();

            // Load Background

            ArrayList tmpRemoveLst = new ArrayList();

            foreach (Enemy enemy in reader.enemyList)
            {
                if (enemy is Grunt)
                {
                    enemy.init(enemyText, enemyShot, constants);
                    enemyList.Add(enemy);
                }
                else if (enemy is Boss)
                {
                    boss = (Boss)enemy;
                    boss.init(enemyText, enemyShot, constants);
                }
            }

            foreach (Boss boss in tmpRemoveLst)
            {
                enemyList.Remove(boss);
            }

            // load background song
            if (reader.levelSong != "none")
            {
                Uri uri = new Uri(reader.levelSong,UriKind.Relative);
                try
                {
                    bsong = Song.FromUri(reader.levelSong, uri);
                    //MediaPlayer.Play(bsong);
                }
                catch (System.ArgumentException)
                {
                    Console.Error.WriteLine("Error loading file: " + (string)reader.levelSong + " ... Ignoring");            
                }
            }

            // Load background
            if (reader.background != "none")
            {
                try
                {
                    Stream str = File.OpenRead(reader.background);
                    backgroundTexture = Texture2D.FromStream(GraphicsDevice, str);
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    Console.Error.WriteLine("Could not locate file: " + (string)reader.background + " Using default.");
                    backgroundTexture = singlePix;
                }

            }
            else
            {
                backgroundTexture = singlePix;
            }
            //create player
            humanAnimatedTexture = new AnimatedSprite(humanTexture, constants.HUMAN_NEUTRAL_FRAME, 
                constants.HUMAN_NEUTRAL_FRAME, constants.MAX_HUMAN_FRAMES, constants.HUMAN_SPRITE_WIDTH, 
                constants.HUMAN_SPRITE_HEIGHT);
           
            human = new Player(humanAnimatedTexture, shotTexture, constants.HUMAN_START_X, constants.HUMAN_START_Y, 
                constants.MAX_HUMAN_SPEED);
            
    
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load fonts
            titleFont = Content.Load<SpriteFont>("titleFont");
            font = Content.Load<SpriteFont>("font");

            enemyShot = Content.Load<Texture2D>("shot2");
            singlePix = Content.Load<Texture2D>("singlePix");
            enemyText = Content.Load<Texture2D>("Enemy1");
            shotTexture = Content.Load<Texture2D>("shot1");
            humanTexture = Content.Load<Texture2D>("player");
            explosionTexture = Content.Load<Texture2D>("explosion");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!boss.alive)
            {
                levelComplete = true;
            }

            if (human.lives < 0)
            {
                gameOver = true;
                gameOverTimer.Start();
            }

            KeyboardState state = Keyboard.GetState();

            if (displayMenu)
            {
                if (state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.Down))
                {
                    move = true;
                }

                if (state.IsKeyUp(Keys.Space))
                {
                    select = true;
                }

                if (state.IsKeyDown(Keys.Down))
                {
                    if (selected < maxSelection && move)
                    {
                        selected++;
                        move = false;
                    }
                }
                else if (state.IsKeyDown(Keys.Up))
                {
                    if (selected > minSelection && move)
                    {
                        selected--;
                        move = false;
                    }
                }
                if (state.IsKeyDown(Keys.Space) && select)
                {
                    select = false;
                    if (selected == 0)
                    {
                        displayMenu = false;
                    }
                    else if (selected == 1)
                    {
                        displayMenu = false;
                        subMenu = true;
                    }
                }
            }else if (subMenu)
            {
                if (state.IsKeyDown(Keys.Space) && select)
                {
                    select = false;
                    subMenu = false;
                    displayMenu = true;
                }

                if (state.IsKeyUp(Keys.Space))
                {
                    select = true;
                }
            }
            else if (gameOver)
            {
                if (gameOverTimer.ElapsedMilliseconds > 5000)
                {
                    //TODO write code to reset level
                    gameOver = false;
                    displayMenu = true;
                    gameOverTimer.Reset();
                }
            }
            //TODO write win code
            else
            {
                if (MediaPlayer.State == MediaState.Stopped && bsong != null)
                {
                    MediaPlayer.Play(bsong);
                }

                // update enemies
                updateEnemies(gameTime);
                updateBullets();
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // TODO: Add your update logic here
                for (int i = 0; i < shotList.Length; i++)
                {
                    if (shotList[i] != null)
                    {
                        if (shotList[i].collidsWith(human) && human.respawn == false)
                        {
                            human.hit = true;
                        }
                    }
                }
            }

            // TODO: add code to advance level
            if (levelComplete)
            {
                MediaPlayer.Stop();
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected void drawSubMenu(SpriteBatch batch)
        {
            KeyboardState state = Keyboard.GetState();

            batch.DrawString(titleFont, "Controls", new Vector2(200, 100), Color.Green);
            batch.DrawString(font, "Move:", new Vector2(200, 300), Color.Green);
            batch.DrawString(font, "Arrow Keys", new Vector2(400, 300), Color.Green);
            batch.DrawString(font, "Shoot:", new Vector2(200, 350), Color.Green);
            batch.DrawString(font, "z", new Vector2(400, 350), Color.Green);

            
        }

        protected void drawMenu(SpriteBatch batch)
        {
            

            spriteBatch.DrawString(titleFont, "Danmaku", new Vector2(200 , 100), Color.Green);
            spriteBatch.DrawString(font, "Version: Alpha 1.0", new Vector2(200, 210), Color.Green);
            spriteBatch.DrawString(font, "Press SPACE to select", new Vector2(200, 500), Color.Green);

            if (selected == 0)
            {
                spriteBatch.DrawString(font, "Start", new Vector2(200, 300), Color.White);
                spriteBatch.DrawString(font, "Controls", new Vector2(200, 350), Color.Green);
                spriteBatch.DrawString(font, "Quit", new Vector2(200, 400), Color.Green);
            }
            else if (selected == 1)
            {
                spriteBatch.DrawString(font, "Start", new Vector2(200, 300), Color.Green);
                spriteBatch.DrawString(font, "Controls", new Vector2(200, 350), Color.White);
                spriteBatch.DrawString(font, "Quit", new Vector2(200, 400), Color.Green);
            }else if (selected == 2)
            {
                spriteBatch.DrawString(font, "Start", new Vector2(200, 300), Color.Green);
                spriteBatch.DrawString(font, "Controls", new Vector2(200, 350), Color.Green);
                spriteBatch.DrawString(font, "Quit", new Vector2(200, 400), Color.White);
                if (!select)
                {
                    this.Exit();
                }
            }
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            if (subMenu)
            {
                drawSubMenu(spriteBatch);
            }
            else if (displayMenu)
            {
                drawMenu(spriteBatch);
            }
            else if(!win)
            {
                // correct clock only if offset is not set
                if (offset == null)
                {
                    offset = (float)gameTime.TotalGameTime.Seconds;
                }

                time = (float)gameTime.TotalGameTime.Seconds - offset;

                //Draw background
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 800, 600), Color.White);
                //draw enemies

                // if gameover, draw to screen
                if (gameOver)
                {
                    human.xPos = -200;
                    human.yPos = -200;
                    spriteBatch.DrawString(titleFont, "GAME OVER", new Vector2(150, 150), Color.White);
                }

                foreach (EnemyShot shot in shotList)
                {
                    if (shot != null)
                    {
                        shot.draw(spriteBatch);
                    }
                }

                //
                if (boss.appearTime <= time)
                {
                    boss.yPos = 400;
                    boss.draw(spriteBatch);
                }

                foreach (Enemy enemy in activeList)
                {
                    if (enemy != null)
                    {
                        if (time >= enemy.appearTime)
                        {
                            enemy.start = true;
                            enemy.draw(spriteBatch);
                        }
                    }
                }

                //handle player movement
                if (!human.respawn)
                {
                    Vector2 humanPos = human.updateState(gameTime, activeList, boss);
                    human.drawShots(spriteBatch);
                    human.sprite.draw(spriteBatch, humanPos);

                    // handle player explosions
                    if (human.hit)
                    {
                        Expolsion exp = new Expolsion(explosionTexture, 128, 128, 20);
                        explosionList.Add(exp);
                        human.die(exp, spriteBatch);
                    }
                }
                else
                {
                    human.respawnUpdate();
                    human.drawShots(spriteBatch);
                    Vector2 humanPos = human.updateState(gameTime, activeList, boss);
                    human.sprite.drawInvincible(spriteBatch, humanPos);
                }



                //draw effects and remove players/enemies
                updateEffects(spriteBatch);
                removeEnemies(spriteBatch);

                
                /*
                //hit box for debugging
                spriteBatch.Draw(singlePix, human.hitBox, Color.Red);
                // hit box for enemies
                foreach (Enemy enemy in activeList)
                {
                    if (enemy != null)
                    {
                        spriteBatch.Draw(singlePix, enemy.hitBox, Color.Yellow);
                    }
                }

                spriteBatch.Draw(singlePix, boss.hitBox, Color.Green);
                */           
            } 

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Updates active enemies
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// 

        protected void updateBullets()
        {
            for (int i = 0; i < shotList.Length; i++)
            {
                EnemyShot shot = shotList[i];
                if (shot != null)
                {
                    shot.update();

                    if (shot.isOutOfPlay())
                    {
                        shotList[i] = null;
                    }
                    else
                    {
                        shotList[i] = shot;
                    }
                }
            }
        }
        protected void updateEnemies(GameTime gameTime)
        {
            // Add enemies to the active list
            ArrayList localRemove = new ArrayList();
            foreach (Enemy enemy in enemyList)
            {
                if (time >= enemy.appearTime)
                {
                    // check for open spot in active list
                    for (int i = 0; i < activeList.Length; i++)
                    {
                        if (activeList[i] == null)
                        {
                            activeList[i] = enemy;
                            break;
                        }
                    }

                    // add to local remove list if unable to place
                    //if (!placed)
                    //{
                        localRemove.Add(enemy);
                    //}
                }
            }

            // Once added to the active list, remove it from the global list
            foreach (Enemy enemy in localRemove)
            {
                enemyList.Remove(enemy);
            }

            //Handle boss
            if (time >= boss.appearTime)
            {
                boss.update(human, shotList);
            }

            // check to see if each enemy is alive
            foreach (Enemy enemy in activeList)
            {
                // if enemy is in active list, and not null
                if (enemy != null)
                {
                    // update the enemy
                    enemy.update(human, shotList);

                    // if enemy is not alive, have it set to be removed
                    if (!enemy.alive)
                    {
                        //enemyList.Remove(enemy);
                        removeList.Add(enemy);
                    }

                    // update shots
                    ArrayList shotRemoveList = new ArrayList();  
                }
            }
        }

        /// <summary>
        /// Removes all defeated enemies from active lists
        /// </summary>
        /// <param name="batch">current sprite batch</param>
        protected void removeEnemies(SpriteBatch batch)
        {
            Expolsion exp = new Expolsion(explosionTexture, 128, 128, 20);

            //remove each enemy in the remove list from the main enemy list
            foreach (Enemy enemy in removeList)
            {
                //create a new explosion and add it to the explosion list
                explosionList.Add(exp);
                enemy.die(exp, batch);

                // re-open slot in active list 
                for (int i = 0; i < activeList.Length; i++)
                {
                    if (activeList[i] == enemy)
                    {
                        activeList[i] = null;
                    }
                }


            }

            if (!boss.alive)
            {
                boss.die(exp, batch);
            }
            removeList.Clear();
            
        }

        /// <summary>
        /// Update all effects on screen
        /// </summary>
        /// <param name="batch">current sprite batch</param>
        protected void updateEffects(SpriteBatch batch)
        {
            ArrayList expRemoveList = new ArrayList();
            foreach (Expolsion exp in explosionList)
            {
                exp.update();
                exp.draw(batch);
                if (exp.finished)
                {
                    expRemoveList.Add(exp);
                }
            }

            foreach (Expolsion exp in expRemoveList)
            {
                explosionList.Remove(exp);
            }
        }
    }
}
