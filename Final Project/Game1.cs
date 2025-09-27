using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameUtility;

namespace Final_Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Sets up constants for the gmaestate
        const int MENU = 0;
        const int PLAYING = 1;
        const int ENDGAME = 2;

        //Sets up constants for what state the samurai is in
        const int SAMURAI_IDLE = 0;
        const int SAMURAI_RUN = 1;
        const int SAMURAI_ATTACK_1 = 2;
        const int SAMURAI_ATTACK_2 = 3;
        const int SAMURAI_BLOCK = 4;
        const int SAMURAI_DASH = 5;
        const int SAMURAI_HURT = 6;
        const int SAMURAI_DEAD = 7;

        //Sets up constants for what state the ninja is in
        const int NINJA_IDLE = 0;
        const int NINJA_RUN = 1;
        const int NINJA_ATTACK_1 = 2;
        const int NINJA_THROW = 3;
        const int NINJA_HURT = 4;
        const int NINJA_DEAD = 5;

        //Variable forcooldown on dash
        Timer dashTimer;

        //Variable for what gamestate is currently on
        int gameState = MENU;

        //Variable for the states of the ninja and samurai 
        int samuraiState = SAMURAI_IDLE;
        int ninjaState = NINJA_IDLE;

        //Variable for the keyboard state and previous keyboard state
        KeyboardState kb;
        KeyboardState prevKb;

        //Variable for the mouse state and previous mouse state
        MouseState mouse;
        MouseState prevMouse;

        //Variable which holds the screen height and screen width
        int screenHeight;
        int screenWidth;

        //Array variable which holds all the ninja and samurai animations
        Animation[] samuraiAnims = new Animation[10];
        Animation[] ninjaAnims = new Animation[10];

        //Texture variable and rectangle for the menu background
        Texture2D menuBG;
        Rectangle menuBGRec;

        //Texture variable and rectangle variable for the game background
        Texture2D gameBG;
        Rectangle gameBGRec;

        //Texture variable and rectangle variable for the endgame background
        Texture2D endgameBG;
        Rectangle endgameBGRec;

        //Texture variable and rectangle variable for the playbutton
        Texture2D playButtonImg;
        Rectangle playButtonRec;

        //Texture variable and rectangle variable for the quit button
        Texture2D quitButtonImg;
        Rectangle quitButtonRec;

        //Texture variable and rectangle variable for the menu button
        Texture2D menuButtonImg;
        Rectangle menuButtonRec;

        //Texture variable for the platform
        Texture2D platform;

        //Rectangle variable for all three platforms
        Rectangle platformRec1;
        Rectangle platformRec2;
        Rectangle platformRec3;

        //Rectangle variable for the top of all three platforms
        Rectangle platformTopRec1;
        Rectangle platformTopRec2;
        Rectangle platformTopRec3;

        //Location variable of the samurai and the ninja 
        Vector2 samuraiLoc;
        Vector2 ninjaLoc;

        //Rectangles for the ninja's feet and their weapon hitbox
        Rectangle ninjaHitRec;
        Rectangle ninjaFeetRec;

        //Rectangles for the samurai's feet, body and their weapons hitbox
        Rectangle samuraiHitRec;
        Rectangle samuraiFeetRec;
        Rectangle samuraiRec;

        //Variables to hold all spritesheets for the samurai animation
        Texture2D samuraiRunImg;
        Texture2D samuraiIdleImg;
        Texture2D samuraiAttack1Img;
        Texture2D samuraiAttack2Img;
        Texture2D samuraiBlockImg;
        Texture2D samuraiDashimg;
        Texture2D samuraiHurtImg;
        Texture2D samuraiDeadImg;

        //Variables to hold all spritesheets for the ninja animation
        Texture2D ninjaIdleImg;
        Texture2D ninjaRunImg;
        Texture2D ninjaAttack1Img;
        Texture2D ninjaThrowImg;
        Texture2D ninjaHurtImg;
        Texture2D ninjaDeadImg;

        //Variables to hold the ninja's dart image, rectangle, and position
        Texture2D ninjaDartImg;
        Rectangle ninjaDartRec;
        Vector2 ninjaDartPos;

        //Variables to hold the title's font and the location
        SpriteFont titleFont;
        Vector2 titleLoc;

        //Variables to hold the default font and the location of the health displays
        SpriteFont regularFont;
        Vector2 healthLine1Loc;
        Vector2 healthLine2Loc;

        //Variables to hold the locations of the health counts
        Vector2 samuraiHealthLineLoc;
        Vector2 ninjaHealthLineLoc;

        //Samurai and Ninja's health
        int samuraiHealth = 130;
        int ninjaHealth = 100;

        //The Title
        string titleLine = "SHOGUN SHOWDOWN";

        //Caption for health
        string samuraiHealthLine = "Health: ";
        string ninjaHealthLine = "Health: ";

        //Variable to hold the winner's character and the location where it is put at
        string announceWinner= "";
        Vector2 announceWinnerLoc;

        //Variable which holds samurai jump speed and bool to determine if they are jumping
        double samuraiJumpSpeed = 0;
        bool samuraiJump = false;

        //Variable which holds ninja jump speed and bool to determine if they are jumping
        double ninjaJumpSpeed = 0;
        bool ninjaJump = false;

        //Determines which direction ninja and samurai is facing
        bool samuraiLeft = false;
        bool ninjaLeft = true;

        //Determines if samurai is attacking with default attack or dashing
        bool samuraiAttack = false;
        bool samuraiDash = false;

        //Determines if ninja is attacking with default attack or throwing projectiles
        bool ninjaAttack = false;
        bool ninjaThrow = false;

        //Determines if the samurai's image needs to be shifted
        bool samuraiImgShift = false;

        //Determines the direction of the samurai block
        bool samuraiBlockingLeft = false;
        bool samuraiBlockingRight = false;

        //Determines if ninja/samurai is attacking in general
        bool samuraiAttacking = false;
        bool ninjaAttacking = false;

        //Determines if samurai/ninja is dealing damage
        bool samuraiDealtDmg = false;
        bool ninjaDealtDmg = false;

        //Determines if samurai/ninja is stunned
        bool samuraiStunned = false;
        bool ninjaStunned = false;

        //Determines if the ninja projectile is actice, going left or right
        bool projectileActive = false;
        bool projectileLockedLeft = false;
        bool projectileLockedRight = false;

        //Determine wheter the samurai died or not when the game ends
        bool samuraiDied = false;

        //Determine which attack the samurai is using for the default attack
        int samuraiAlternatingAttack = 1;

        //Determines if there is a swing sound or block sound for the samurai
        bool samuraiSwingSound = false;
        bool samuraiBlockSound = false;

        //Variable to hold the menu, gameplay and endgame songs
        Song menuMusic;
        Song fightingMusic;
        Song endgameMusic;

        //Variables to hold all sound effects in the game
        SoundEffect slash1SFX;
        SoundEffect slash2SFX;
        SoundEffect swingSFX;
        SoundEffect dashSFX;
        SoundEffect kunaiSFX;
        SoundEffect throwSFX;
        SoundEffect blockSFX;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {

            //Makes the mouse visible
            IsMouseVisible = true;

            //Sets the width and height of the game window
            graphics.PreferredBackBufferWidth = 1500;
            graphics.PreferredBackBufferHeight = 850;

            //Applies the window screen changes
            graphics.ApplyChanges();

            //Sets screenwidth and screenheight variables to the screenwidth and screenheight
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Loads placeholder time for timer
            dashTimer = new Timer(0, true);

            //Loads idle spritesheet into variable
            samuraiIdleImg = Content.Load<Texture2D>("Images/Sprites/Samurai_Idle");

            //Creates starting location for samurai's location and feet location. Creates starting samurai rectangle and makes a placeholder for the samurai's blade hitbox 
            samuraiLoc = new Vector2(200, screenHeight - samuraiIdleImg.Height * 2);
            samuraiHitRec = new Rectangle((int)samuraiLoc.X, (int)samuraiLoc.Y, samuraiIdleImg.Width, samuraiIdleImg.Height);
            samuraiFeetRec = new Rectangle((int)(samuraiLoc.X + samuraiIdleImg.Width * 2 * 0.4), (int)(samuraiLoc.Y + samuraiIdleImg.Height * 2 * 0.90), (int)(samuraiIdleImg.Width * 2 * 0.2), (int)(samuraiIdleImg.Height * 2 * 0.1));
            samuraiRec = new Rectangle((int)samuraiLoc.X, (int)samuraiLoc.Y, samuraiIdleImg.Width, samuraiIdleImg.Height);

            //Loads samurai idle animation into array of animations
            samuraiAnims[0] = new Animation(samuraiIdleImg, 6, 1, 6, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 1000, samuraiLoc, 2f, 2f, true);

            //Loads samurai's run animation spritesheet and stores it as an animation in animation array
            samuraiRunImg = Content.Load<Texture2D>("Images/Sprites/Samurai_Run");
            samuraiAnims[1] = new Animation(samuraiRunImg, 8, 1, 8, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 700, samuraiLoc, 2f, 2f, true);

            //Loads samurai's first attack animation spritesheet and stores it as an animation in animation array
            samuraiAttack1Img = Content.Load<Texture2D>("Images/Sprites/Samurai_Attack_1");
            samuraiAnims[2] = new Animation(samuraiAttack1Img, 5, 1, 5, 0, Animation.NO_IDLE, 1, 300, samuraiLoc, 2f, 2f, true);

            //Loads samurai's second attack animation spritesheet and stores it as an animation in animation array
            samuraiAttack2Img = Content.Load<Texture2D>("Images/Sprites/Samurai_Attack_2");
            samuraiAnims[3] = new Animation(samuraiAttack2Img, 4, 1, 4, 0, Animation.NO_IDLE, 1, 400, samuraiLoc, 2f, 2f, true);

            //Loads samurai's block animation spritesheet and stores it as an animation in animation array
            samuraiBlockImg = Content.Load<Texture2D>("Images/Sprites/Samurai_Protection");
            samuraiAnims[4] = new Animation(samuraiBlockImg, 2, 1, 2, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 500, samuraiLoc, 2f, 2f, true);

            //Loads samurai's dash animation spritesheet and stores it as an animation in animation array
            samuraiDashimg = Content.Load<Texture2D>("Images/Sprites/Samurai_Dash");
            samuraiAnims[5] = new Animation(samuraiDashimg, 4, 1, 4, 0, Animation.NO_IDLE, 1, 400, samuraiLoc, 2f, 2f, true);

            //Loads samurai's hurt animation spritesheet and stores it as an animation in animation array
            samuraiHurtImg = Content.Load<Texture2D>("Images/Sprites/Samurai_Hurt");
            samuraiAnims[6] = new Animation(samuraiHurtImg, 3, 1, 3, 0, Animation.NO_IDLE, 1, 450, samuraiLoc, 2f, 2f, true);

            //Loads samurai's death animation spritesheet and stores it as an animation in animation array
            samuraiDeadImg = Content.Load<Texture2D>("Images/Sprites/Samurai_Dead");
            samuraiAnims[7] = new Animation(samuraiDeadImg, 6, 1, 6, 0, Animation.NO_IDLE, 1, 600, samuraiLoc, 2f, 2f, true);

            //Loads ninja idle spritesheet into variable
            ninjaIdleImg = Content.Load<Texture2D>("Images/Sprites/Ninja_Idle");

            //Creates starting ninja location and feet rectangle. Makes a placeholder for the blade hitbox
            ninjaLoc = new Vector2(screenWidth - 400, screenHeight - ninjaIdleImg.Height * 2);
            ninjaHitRec = new Rectangle((int)ninjaLoc.X, (int)ninjaLoc.Y, ninjaIdleImg.Width, ninjaIdleImg.Height);
            ninjaFeetRec = new Rectangle((int)(ninjaLoc.X + ninjaIdleImg.Width * 2 * 0.3), (int)(ninjaLoc.Y + ninjaIdleImg.Height * 2 * 0.90), (int)(ninjaIdleImg.Width * 2 * 0.4), (int)(ninjaIdleImg.Height * 2 * 0.1));

            //Stores ninja idle animation into animation array
            ninjaAnims[0] = new Animation(ninjaIdleImg, 6, 1, 6, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 1000, ninjaLoc, 2f, 2f, true);

            //Loads ninja's run animation spritesheet and stores it as an animation in animation array
            ninjaRunImg = Content.Load<Texture2D>("Images/Sprites/Ninja_Run");
            ninjaAnims[1] = new Animation(ninjaRunImg, 6, 1, 6, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 500, ninjaLoc, 2f, 2f, true);

            //Loads ninja's attack animation spritesheet and stores it as an animation in animation array
            ninjaAttack1Img = Content.Load<Texture2D>("Images/Sprites/Ninja_Attack_2");
            ninjaAnims[2] = new Animation(ninjaAttack1Img, 4, 1, 4, 0, Animation.NO_IDLE, 1, 200, ninjaLoc, 2f, 2f, true);

            //Loads ninja's throw animation spritesheet and stores it as an animation in animation array
            ninjaThrowImg = Content.Load<Texture2D>("Images/Sprites/Ninja_Attack_1");
            ninjaAnims[3] = new Animation(ninjaThrowImg, 6, 1, 6, 0, Animation.NO_IDLE, 1, 300, ninjaLoc, 2f, 2f, true);

            //Loads ninja's hurt animation spritesheet and stores it as an animation in animation array
            ninjaHurtImg = Content.Load<Texture2D>("Images/Sprites/Ninja_Hurt");
            ninjaAnims[4] = new Animation(ninjaHurtImg, 2, 1, 2, 0, Animation.NO_IDLE, 1, 350, ninjaLoc, 2f, 2f, true);

            //Loads ninja's death animation spritesheet and stores it as an animation in animation array
            ninjaDeadImg = Content.Load<Texture2D>("Images/Sprites/Ninja_Dead");
            ninjaAnims[5] = new Animation(ninjaDeadImg, 4, 1, 4, 0, Animation.NO_IDLE, 1, 500, ninjaLoc, 2f, 2f, true);

            //Loads platform image and rectangles where they are drawn
            platform = Content.Load<Texture2D>("Images/Sprites/Platform");
            platformRec1 = new Rectangle(GraphicCenterX(platform, 10), 430, platform.Width / 10, platform.Height / 10);
            platformRec2 = new Rectangle(GraphicCenterX(platform, 10) + 400, 650, platform.Width / 10, platform.Height / 10);
            platformRec3 = new Rectangle(GraphicCenterX(platform, 10) - 400, 650, platform.Width / 10, platform.Height / 10);

            //Loads rectangles on the top of each platform
            platformTopRec1 = new Rectangle(platformRec1.X, platformRec1.Y, platformRec1.Width, 10);
            platformTopRec2 = new Rectangle(platformRec2.X, platformRec2.Y, platformRec2.Width, 10);
            platformTopRec3 = new Rectangle(platformRec3.X, platformRec3.Y, platformRec3.Width, 10);

            //Loads playbutton image and rectangle they are drawn in
            playButtonImg = Content.Load<Texture2D>("Images/Sprites/StartButton");
            playButtonRec = new Rectangle(GraphicCenterX(playButtonImg, 2) + 15, 350, playButtonImg.Width / 2, playButtonImg.Height / 2);

            //Loads quitbutton image and rectangle they are drawn in
            quitButtonImg = Content.Load<Texture2D>("Images/Sprites/Quit Button");
            quitButtonRec = new Rectangle(20, 20, quitButtonImg.Width / 3, quitButtonImg.Height / 3);

            //Loads menubutton image and rectangle they are drawn in
            menuButtonImg = Content.Load<Texture2D>("Images/Sprites/Menu Button");
            menuButtonRec = new Rectangle(GraphicCenterX(menuButtonImg, 1), 500, menuButtonImg.Width, menuButtonImg.Height);

            //Loads menu background image and rectangle they are drawn in
            menuBG = Content.Load<Texture2D>("Images/Backgrounds/Menu Bg");
            menuBGRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Loads game background image and rectangle they are drawn in
            gameBG = Content.Load<Texture2D>("Images/Backgrounds/Game Bg");
            gameBGRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Loads endgame background and rectangle they are drawn in
            endgameBG = Content.Load<Texture2D>("Images/Backgrounds/Endgame Bg");
            endgameBGRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Loads title font and the title's location
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            titleLoc = new Vector2(GetCentreTextX(titleFont, titleLine), 100);
            
            //Loads the default font and the location of all strings written in the default font
            regularFont = Content.Load<SpriteFont>("Fonts/RegularFont");
            healthLine1Loc = new Vector2(20, 20);
            healthLine2Loc = new Vector2(screenWidth - 300, 20);
            samuraiHealthLineLoc = new Vector2(220, 20);
            ninjaHealthLineLoc = new Vector2(screenWidth - 100, 20);
            announceWinnerLoc = new Vector2(GetCentreTextX(regularFont, announceWinner), GetCentreTextY(regularFont, announceWinner));

            //Loads the ninja's dart image, the placeholder rectangle for the dart and a placeholder location
            ninjaDartImg = Content.Load<Texture2D>("Images/Sprites/Dart");
            ninjaDartRec = new Rectangle(-10, -10, ninjaDartImg.Width * 3, ninjaDartImg.Height * 3);
            ninjaDartPos = new Vector2(ninjaDartRec.X, ninjaDartRec.Y);

            //Loads all songs 
            menuMusic = Content.Load<Song>("Audio/Music/Menu Music");
            fightingMusic = Content.Load<Song>("Audio/Music/Fighting Music");
            endgameMusic = Content.Load<Song>("Audio/Music/Endgame Music");

            //Loads all soundeffects
            slash1SFX = Content.Load<SoundEffect>("Audio/Sound/Katana 1");
            slash2SFX = Content.Load<SoundEffect>("Audio/Sound/Katana 2");
            swingSFX = Content.Load<SoundEffect>("Audio/Sound/Swing");
            dashSFX = Content.Load<SoundEffect>("Audio/Sound/Dash");
            throwSFX = Content.Load<SoundEffect>("Audio/Sound/Throw");
            kunaiSFX = Content.Load<SoundEffect>("Audio/Sound/Kunai");
            blockSFX = Content.Load<SoundEffect>("Audio/Sound/Block");

            //Lowers volume of sound effects to 75%
            SoundEffect.MasterVolume = 0.75f;

            //Lowers volume of background music to 75% and repeats the songs
            MediaPlayer.Volume = 0.75f;
            MediaPlayer.IsRepeating = true;
        }
        protected override void UnloadContent()
        {

        }
        protected override void Update(GameTime gameTime)
        {

            //Updates mouse state and previous mouse state
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Updates keyboard state and previous keyboard state
            prevKb = kb;
            kb = Keyboard.GetState();

            //Updates code according to which gamestate the players are currently in
            switch (gameState)
            {
                case MENU:

                    //Plays menu music if music is not playing already
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(menuMusic);
                    }

                    //Resets all relevant bools, animations, values, locations and rectangles to original value if the player presses play. Changes gamestate to play
                    if (mouse.LeftButton == ButtonState.Pressed && !(prevMouse.LeftButton == ButtonState.Pressed))
                    {
                        if (playButtonRec.Contains(mouse.Position))
                        {
                            samuraiLeft = false;
                            ninjaLeft = true;
                            ninjaAttacking = false;
                            ninjaAttack = false;
                            samuraiAttacking = false;
                            samuraiAttack = false;
                            ninjaStunned = false;
                            samuraiStunned = false;
                            samuraiDash = false;
                            samuraiDied = false;
                            projectileActive = false;
                            ninjaState = NINJA_IDLE;
                            samuraiState = SAMURAI_IDLE;
                            ninjaDartRec.Y = -20;
                            samuraiAnims[7].Activate(true);
                            samuraiAnims[7].SetFrame(0);
                            samuraiAnims[7].Resume();
                            ninjaAnims[5].Activate(true);
                            ninjaAnims[5].SetFrame(0);
                            ninjaAnims[5].Resume();
                            gameState = PLAYING;
                            dashTimer.Activate();
                            samuraiHealth = 130;
                            ninjaHealth = 100;
                            samuraiLoc.X = 200;
                            samuraiLoc.Y = screenHeight - samuraiIdleImg.Height * 2;
                            ninjaLoc.X = screenWidth - 400;
                            ninjaLoc.Y = screenHeight - ninjaIdleImg.Height * 2;
                            MediaPlayer.Stop();
                        }
                    }

                    //Closes game if player clicks the quit button
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (quitButtonRec.Contains(mouse.Position))
                        {
                            Exit();
                        }
                    }

                    break;
                case PLAYING:

                    //Updates the dash cooldown timer
                    dashTimer.Update(gameTime);

                    //Plays the fighting music if music isn't already playing
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(fightingMusic);
                    }

                    //Updates the samurai's feet rectangle location and size
                    samuraiFeetRec.X = (int)(samuraiLoc.X + samuraiAnims[samuraiState].GetDestRec().Width * 0.45);
                    samuraiFeetRec.Y = (int)(samuraiLoc.Y + samuraiAnims[samuraiState].GetDestRec().Height * 0.95);
                    samuraiFeetRec.Width = (int)(samuraiAnims[samuraiState].GetDestRec().Width * 2 * 0.1);
                    samuraiFeetRec.Height = (int)(samuraiAnims[samuraiState].GetDestRec().Height * 2 * 0.05);

                    //Updates samurai's rectangle location and size
                    samuraiRec.X = (int)samuraiLoc.X - samuraiAnims[samuraiState].GetDestRec().Width / 3;
                    samuraiRec.Y = (int)samuraiLoc.Y + samuraiAnims[samuraiState].GetDestRec().Height / 3;
                    samuraiRec.Width = samuraiAnims[samuraiState].GetDestRec().Width / 3;
                    samuraiRec.Height = samuraiAnims[samuraiState].GetDestRec().Height - 80;

                    //Makes it so samurai is not blocking by default
                    samuraiBlockingLeft = false;
                    samuraiBlockingRight = false;

                    //Applies gravity to samurai
                    samuraiJumpSpeed -= 9.81 / 60;
                    samuraiLoc.Y -= (float)samuraiJumpSpeed;

                    //Updates ninja's feet rectangle location and size
                    ninjaFeetRec.X = (int)(ninjaLoc.X + ninjaAnims[ninjaState].GetDestRec().Width * 0.4);
                    ninjaFeetRec.Y = (int)(ninjaLoc.Y + ninjaAnims[ninjaState].GetDestRec().Height * 0.95);
                    ninjaFeetRec.Width = (int)(ninjaAnims[ninjaState].GetDestRec().Width * 2 * 0.2);
                    ninjaFeetRec.Height = (int)(ninjaAnims[ninjaState].GetDestRec().Height * 2 * 0.05);

                    //Applies gravity to ninja
                    ninjaJumpSpeed -= 9.81 / 60;
                    ninjaLoc.Y -= (float)ninjaJumpSpeed;

                    //Changes gamestate to endgame and announces winner based on which character's hp drops below zero first
                    if (samuraiHealth <= 0)
                    {
                        //Announces ninja wins, corrects the location of the announcement and stops fighting music. Changes gamestate to endgame
                        announceWinner = "Ninja Wins!";
                        announceWinnerLoc.Y = GetCentreTextY(regularFont, announceWinner);
                        announceWinnerLoc.X = GetCentreTextX(regularFont, announceWinner);
                        samuraiDied = true;
                        gameState = ENDGAME;
                        MediaPlayer.Stop();
                    }
                    else if (ninjaHealth <= 0)
                    {
                        //Announces samurai wins, corrects the location of the announcement and stops fighting music. Changes gamestate to endgame
                        announceWinner = "Samurai Wins!";
                        announceWinnerLoc.Y = GetCentreTextY(regularFont, announceWinner);
                        announceWinnerLoc.X = GetCentreTextX(regularFont, announceWinner);
                        gameState = ENDGAME;
                        MediaPlayer.Stop();
                    }

                    //Keeps samurai in the screen according to which side they walk in to
                    if (samuraiLoc.X < 0 - samuraiAnims[samuraiState].GetDestRec().Width/3 - 30)
                    {
                        //Keeps samurai image in screen but lets the whitespace in the image go off screen
                        samuraiLoc.X = 0 - samuraiAnims[samuraiState].GetDestRec().Width / 3 - 30;
                    }
                    else if (samuraiLoc.X > screenWidth - samuraiAnims[samuraiState].GetDestRec().Width/3 - 50)
                    {
                        //Keeps samurai image in screen but lets the whitespace in the image go off screen
                        samuraiLoc.X = screenWidth - samuraiAnims[samuraiState].GetDestRec().Width/3  - 50;
                    }

                    //Keeps ninja in the screen according to which side they walk in to
                    if (ninjaLoc.X < 0 - ninjaAnims[ninjaState].GetDestRec().Width + 140)
                    {
                        //Keeps ninja image in screen but lets the whitespace in the image go off screen
                        ninjaLoc.X = 0 - ninjaAnims[ninjaState].GetDestRec().Width + 140;
                    }
                    else if (ninjaLoc.X > screenWidth - ninjaAnims[ninjaState].GetDestRec().Width + 50)
                    {
                        //Keeps ninja image in screen but lets the whitespace in the image go off screen
                        ninjaLoc.X = screenWidth - ninjaAnims[ninjaState].GetDestRec().Width + 50;
                    }

                    //Shifts the image of the samurai based on which way they are facing
                    if (samuraiImgShift == false)
                    {
                        if (samuraiLeft == true)
                        {
                            //Shifts image towards the left once
                            samuraiLoc.X -= 50;
                            samuraiImgShift = true;
                        }
                    }
                    //Shifts the image right once if samurai is no longer facing left
                    else if (samuraiLeft == false)
                    {
                        samuraiLoc.X += 50;
                        samuraiImgShift = false;
                    }

                    //Changes the samurai's animation and movement based on what interaction they are having
                    if (samuraiStunned == true)
                    {
                        //Sets the animation state of the samurai to hurt if they get stunned
                        samuraiState = SAMURAI_HURT;
                        //Determines which way the samurai is hit based off the direction of the ninja hitting them
                        if (ninjaLeft == true)
                        {
                            //Slowly shifts samurai left
                            samuraiLoc.X -= 1;
                        }
                        else if (ninjaLeft == false)
                        {
                            //Slowly shifts samurai right
                            samuraiLoc.X += 1;
                        }

                        //Finishes stun movement if the animation finishes
                        if (samuraiAnims[6].IsFinished() == true)
                        {
                            samuraiStunned = false;
                            samuraiAnims[6].Activate(true);
                            samuraiAnims[6].SetFrame(0);
                        }
                    }
                    else if (kb.IsKeyDown(Keys.X) && !(prevKb.IsKeyDown(Keys.X)) && samuraiDash == false && dashTimer.IsFinished() == true)
                    {
                        //Resets and activates dash animation
                        samuraiAnims[5].Activate(true);
                        samuraiAnims[5].SetFrame(0);

                        //Resets dash cooldown to 8 seconds
                        dashTimer.ResetTimer(true, 8000);

                        //Determines that the samurai is dashing, is attacking in general and is making a swing sound
                        samuraiDash = true;
                        samuraiAttacking = true;
                        samuraiSwingSound = true;
                    }
                    else if (samuraiDash == true)
                    {
                        //Determines samurai swing sound is no longer playing in order to stop it from repeating
                        samuraiSwingSound = false;

                        //Sets samurai animation to dash
                        samuraiState = SAMURAI_DASH;
                        //Determines movement of dash based off direction the samurai is facing before dashing
                        if (samuraiLeft == true)
                        {
                            //Moves samurai left
                            samuraiLoc.X -= 12;
                        }
                        else if (samuraiLeft == false)
                        {
                            //Moves samurai right
                            samuraiLoc.X += 12;
                        }

                        //Updates samurai when dash is finished
                        if (samuraiAnims[5].IsFinished() == true)
                        {
                            //Determines that smaurai is no longer dashing, attack in general and dealing damage
                            samuraiDash = false;
                            samuraiAttacking = false;
                            samuraiDealtDmg = false;
                        }
                    }
                    else if (kb.IsKeyDown(Keys.Q) && samuraiAttack == false)
                    {
                        //Alternates the samurai's default attack value
                        samuraiAlternatingAttack = samuraiAlternatingAttack * -1;

                        //Determines that the samurai is default attacking, attacking in general and creating a swing sound
                        samuraiAttack = true;
                        samuraiAttacking = true;
                        samuraiSwingSound = true;

                        //Resets both animations for the default attack and activates them
                        samuraiAnims[2].Activate(true);
                        samuraiAnims[2].SetFrame(0);
                        samuraiAnims[3].Activate(true);
                        samuraiAnims[3].SetFrame(0);
                    }
                    else if (samuraiAttack == true)
                    {
                        //Determines swing sound is no longer playing so it doesn't repeat
                        samuraiSwingSound = false;
                        //Plays attack animation according to the value of the alternating attack
                        if (samuraiAlternatingAttack == -1)
                        {
                            //Plays first attack animation
                            samuraiState = SAMURAI_ATTACK_1;
                            //Updates code when attack is finished
                            if (samuraiAnims[2].IsFinished() == true)
                            {
                                //Determines samurai is no longer default attacking, attacking in general, and dealing damage
                                samuraiAttack = false;
                                samuraiAttacking = false;
                                samuraiDealtDmg = false;
                            }
                        }

                        else if (samuraiAlternatingAttack == 1)
                        {
                            //Plays second samurai attack animation
                            samuraiState = SAMURAI_ATTACK_2;
                            //Updates code when attack is over
                            if (samuraiAnims[3].IsFinished() == true)
                            {
                                //Determines samurai is no longer default attacking, attacking in general, and dealing damage
                                samuraiAttack = false;
                                samuraiAttacking = false;
                                samuraiDealtDmg = false;
                            }
                        }
                    }
                    else if (kb.IsKeyDown(Keys.Z))
                    {
                        //Makes samurai enter block animation as player holds down button
                        samuraiState = SAMURAI_BLOCK;
                        //Determines which direction the samurai is blocking based off direction they were facing before they started blocking
                        if (samuraiLeft == true)
                        {
                            //Determines samurai is blocking left
                            samuraiBlockingLeft = true;
                        }
                        else
                        {
                            //Determines samurai is blocking right
                            samuraiBlockingRight = true;
                        }
                    }
                    else if (kb.IsKeyDown(Keys.D))
                    {
                        //Moves samurai right and puts them in run animation
                        samuraiState = SAMURAI_RUN;
                        samuraiLoc.X += 4;

                        //Determines samurai is not facing left
                        samuraiLeft = false;
                    }
                    else if (kb.IsKeyDown(Keys.A))
                    {
                        //Moves samurai left and puts them in run animation
                        samuraiState = SAMURAI_RUN;
                        samuraiLoc.X -= 4;

                        //Determines samurai is facing left
                        samuraiLeft = true;
                    }
                    else
                    {
                        //Resets all animations and animates idle animation for samurai if nothing is being pressed
                        SamuraiResetAnimation(1);
                        SamuraiResetAnimation(2);
                        SamuraiResetAnimation(3);
                        SamuraiResetAnimation(4);
                        samuraiState = SAMURAI_IDLE;
                    }

                    //Updates samurai's jump code based off where they are and what they are pressing
                    if (kb.IsKeyDown(Keys.W) && samuraiJump == false && samuraiStunned == false)
                    {
                        //Increases samurai's jump speed and Y-location if they press W
                        samuraiJumpSpeed = 8.5;
                        samuraiLoc.Y -= 20;

                        //Pauses whichever animation they were currently in previous to jumping
                        samuraiAnims[1].Pause();

                        //Determine samurai is jumping
                        samuraiJump = true;
                    }
                    else if (samuraiLoc.Y >= screenHeight - samuraiIdleImg.Height * 2)
                    {
                        //Sets samurai jump speed to zero and sets Y-location to the ground
                        samuraiJumpSpeed = 0;
                        samuraiLoc.Y = screenHeight - samuraiAnims[samuraiState].GetDestRec().Height;

                        //Determines samurai is no longer jumping
                        samuraiJump = false;
                        
                        //Resumes animation 
                        samuraiAnims[1].Resume();
                    }
                    else if (platformTopRec1.Intersects(samuraiFeetRec))
                    {
                        //Sets samurai jump speed to zero and sets y-location to the bottom of the first platform
                        samuraiJumpSpeed = 0;
                        samuraiLoc.Y = platformTopRec1.Y - samuraiAnims[samuraiState].GetDestRec().Height;

                        //Determines samurai is no longer jumping
                        samuraiJump = false;

                        //Resumes animation
                        samuraiAnims[1].Resume();
                    }
                    else if (platformTopRec2.Intersects(samuraiFeetRec))
                    {
                        //Sets samurai jump speed to zero and sets y-location to the bottom of the second platform
                        samuraiJumpSpeed = 0;
                        samuraiLoc.Y = platformTopRec2.Y - samuraiAnims[samuraiState].GetDestRec().Height;

                        //Determines samurai is no longer jumping
                        samuraiJump = false;

                        //Resumes animation
                        samuraiAnims[1].Resume();
                    }
                    else if (platformTopRec3.Intersects(samuraiFeetRec))
                    {
                        //Sets samurai jump speed to zero and sets y-location to the bottom of the third platform
                        samuraiJumpSpeed = 0;
                        samuraiLoc.Y = platformTopRec3.Y - samuraiAnims[samuraiState].GetDestRec().Height;

                        //Determines samurai is no longer jumping
                        samuraiJump = false;

                        //Resumes animation
                        samuraiAnims[1].Resume();
                    }

                    //Changes the ninja's animation and movement based on what interaction they are having
                    if (ninjaStunned == true)
                    {
                        //Sets ninja to hurt animation if they get stunned
                        ninjaState = NINJA_HURT;
                        //Determines direction of movement during stun based of the direction the samurai is facing
                        if (samuraiLeft == true)
                        {
                            //Slowly moves ninja to the left
                            ninjaLoc.X -= (int)2;
                        }
                        else if (samuraiLeft == false)
                        {
                            //Slowly moves ninja to the right
                            ninjaLoc.X += (int)2;
                        }

                        //Updates code when stun is finished
                        if (ninjaAnims[4].IsFinished() == true)
                        {
                            //Determines ninja is no longer stunned
                            ninjaStunned = false;

                            //Resets the stun animation
                            ninjaAnims[4].Activate(true);
                            ninjaAnims[4].SetFrame(0);
                        }
                    }
                    else if (kb.IsKeyDown(Keys.O) && ninjaThrow == false && projectileActive == false)
                    {
                        //Determines ninja is throwing 
                        ninjaThrow = true;
                        
                        //Resets and activates throw animation
                        ninjaAnims[3].Activate(true);
                        ninjaAnims[3].SetFrame(0);

                        //Determines that projectile is active
                        projectileActive = true;

                        //Play's a throw sound effect
                        throwSFX.CreateInstance().Play();

                        //Determines samurai block sound effect can be activated
                        samuraiBlockSound = true;

                        //Determines direction of dart based off direction ninja is facing before throwing
                        if (ninjaLeft == true)
                        {
                            //Moves location and dart image to the left
                            ninjaDartPos.X = ninjaLoc.X;
                            ninjaDartRec.Y = (int)(ninjaLoc.Y + 100);
                        }
                        else if (ninjaLeft == false)
                        {
                            //Moves location and dart image to the right
                            ninjaDartPos.X = (int)(ninjaLoc.X + ninjaAnims[ninjaState].GetDestRec().Width);
                            ninjaDartRec.Y = (int)(ninjaLoc.Y + 100);
                        }
                    }
                    else if (ninjaThrow == true)
                    {
                        //Plays throw animation
                        ninjaState = NINJA_THROW;

                        //Determines ninja is attacking in general
                        ninjaAttacking = true;

                        //Updates code when throw animation is finished
                        if (ninjaAnims[3].IsFinished() == true)
                        {
                            //Determines that ninja is no longer throwing
                            ninjaThrow = false;
                        }
                    }
                    else if (kb.IsKeyDown(Keys.P) && ninjaAttack == false)
                    {
                        //Determines ninja is default attacking
                        ninjaAttack = true;

                        //Resets and activates ninja default animation
                        ninjaAnims[2].Activate(true);
                        ninjaAnims[2].SetFrame(0);

                        //Determines ninja is attacking in general
                        ninjaAttacking = true;

                        //Determines samurai block sound effect can be activated
                        samuraiBlockSound = true;
                    }
                    else if (ninjaAttack == true)
                    {
                        //animates ninja attack animation
                        ninjaState = NINJA_ATTACK_1;

                        //Updates code when ninja is finished attacking
                        if (ninjaAnims[2].IsFinished() == true)
                        {
                            //Determines ninja is no longer default attacking, attacking in general and dealing damage
                            ninjaAttack = false;
                            ninjaAttacking = false;
                            ninjaDealtDmg = false;
                        }
                    }    
                    else if (kb.IsKeyDown(Keys.Right))
                    {
                        //Puts ninja in run animation and moves them right
                        ninjaState = NINJA_RUN;
                        ninjaLoc.X += 6;

                        //Determines ninja is not facing left
                        ninjaLeft = false;
                    }
                    else if (kb.IsKeyDown(Keys.Left))
                    {
                        //Puts ninja in run animation and moves them left
                        ninjaState = NINJA_RUN;
                        ninjaLoc.X -= 6;

                        //Determines ninja is facing left
                        ninjaLeft = true;
                    }

                    else
                    {
                        //Resets all animations and animates idle animation for ninja if nothing is being pressed
                        NinjaResetAnimation(1);
                        NinjaResetAnimation(2);
                        NinjaResetAnimation(3);
                        ninjaState = NINJA_IDLE;
                    }

                    //Updates code if projective is active without direction
                    if (projectileActive == true && projectileLockedLeft == false && projectileLockedRight == false)
                    {
                        //Updates direction of projectile based on direction of ninja
                        if (ninjaLeft == true)
                        {
                            //Determines direction of projectile left
                            projectileLockedLeft = true;
                        }

                        else if (ninjaLeft == false)
                        {
                            //Determines direction of projectile Right
                            projectileLockedRight = true;
                        }
                    }

                    //Updates projectile based off the direction its facing and if its active
                    if (projectileLockedLeft == true && projectileActive == true)
                    {
                        //Moves projectile's rectangle and position left
                        ninjaDartPos.X -= 25;
                        ninjaDartRec.X = (int)ninjaDartPos.X;
                        
                        //Updates code if the projectile goes off screen
                        if (ninjaDartPos.X < 0 - ninjaDartRec.Width)
                        {
                            //Determines the projectile is no longer active, no longer locked in a direction, that the ninja is attacking in general and dealing damage
                            projectileActive = false;
                            projectileLockedLeft = false;
                            ninjaAttacking = false;
                            ninjaDealtDmg = false;
                        }
                    }
                    else if (projectileLockedRight == true && projectileActive == true)
                    {
                        //Moves projectile's rectangle and position right
                        ninjaDartPos.X += 25;
                        ninjaDartRec.X = (int)ninjaDartPos.X;

                        if (ninjaDartPos.X > screenWidth + ninjaDartRec.Width)
                        {
                            //Determines the projectile is no longer active, no longer locked in a direction, that the ninja is attacking in general and dealing damage
                            projectileActive = false;
                            projectileLockedRight = false;
                            ninjaAttacking = false;
                            ninjaDealtDmg = false;
                        }
                    }

                    //Updates ninja's jump code based off where they are and what they are pressing
                    if (kb.IsKeyDown(Keys.Up) && ninjaJump == false && ninjaStunned == false)
                    {
                        //Gives ninja jumop speed and moves their Y-location up
                        ninjaJumpSpeed = 8.5;
                        ninjaLoc.Y -= 20;

                        //Pauses ninja's animation prior to jumping
                        ninjaAnims[1].Pause();

                        //Determines that ninja is jumping
                        ninjaJump = true;
                    }
                    else if (ninjaLoc.Y >= screenHeight - ninjaIdleImg.Height * 2)
                    {
                        //Sets ninja's jump speed to zero and sets their location to the ground
                        ninjaJumpSpeed = 0;
                        ninjaLoc.Y = screenHeight - ninjaAnims[ninjaState].GetDestRec().Height;

                        //Determines ninja is no longer jumping
                        ninjaJump = false;

                        //Resumes animation
                        ninjaAnims[1].Resume();
                    }
                    else if (platformTopRec1.Intersects(ninjaFeetRec))
                    {
                        //Sets ninja's jump speed to zero and sets their location to the top of platform 1
                        ninjaJumpSpeed = 0;
                        ninjaLoc.Y = platformTopRec1.Y - ninjaAnims[ninjaState].GetDestRec().Height;

                        //Determines ninja is no longer jumping
                        ninjaJump = false;

                        //Resumes animation
                        ninjaAnims[1].Resume();
                    }
                    else if (platformTopRec2.Intersects(ninjaFeetRec))
                    {
                        //Sets ninja's jump speed to zero and sets their location to the top of platform 2
                        ninjaJumpSpeed = 0;
                        ninjaLoc.Y = platformTopRec2.Y - ninjaAnims[ninjaState].GetDestRec().Height;

                        //Determines ninja is no longer jumping
                        ninjaJump = false;

                        //Resumes animation
                        ninjaAnims[1].Resume();
                    }
                    else if (platformTopRec3.Intersects(ninjaFeetRec))
                    {
                        //Sets ninja's jump speed to zero and sets their location to the top of platform 3
                        ninjaJumpSpeed = 0;
                        ninjaLoc.Y = platformTopRec3.Y - ninjaAnims[ninjaState].GetDestRec().Height;

                        //Determines ninja is no longer jumping
                        ninjaJump = false;

                        //Resumes animation
                        ninjaAnims[1].Resume();
                    }

                    //Sets hitboxes up and deals damage based on conditions if samurai is attacking in general
                    if (samuraiAttacking == true)
                    {
                        //Sets up samurai's blade hitbox based off which way they are facing
                        if (samuraiLeft == true)
                        {
                            //Makes the blade hitbox on the left side of the samurai and makes it around half the width of the actual samurai
                            samuraiHitRec.X = (int)(samuraiLoc.X - samuraiAnims[samuraiState].GetDestRec().Width * 0.25);
                            samuraiHitRec.Y = (int)samuraiLoc.Y;
                            samuraiHitRec.Width = samuraiAnims[samuraiState].GetDestRec().Width / 2;
                            samuraiHitRec.Height = samuraiAnims[samuraiState].GetDestRec().Height;
                        }
                        else if (samuraiLeft == false)
                        {
                            //Makes the blade hitbox on the right side of the samurai and makes it around half the width of the actual samurai
                            samuraiHitRec.X = (int)(samuraiLoc.X + samuraiAnims[samuraiState].GetDestRec().Width * 0.75);
                            samuraiHitRec.Y = (int)samuraiLoc.Y;
                            samuraiHitRec.Width = samuraiAnims[samuraiState].GetDestRec().Width / 2;
                            samuraiHitRec.Height = samuraiAnims[samuraiState].GetDestRec().Height;
                        }

                        //Updates ninja's health and determines whether they are stunned based off which way the ninja is attacking. Plays sound effect on contact with ninja
                        if (samuraiHitRec.Contains((int)(ninjaLoc.X + ninjaAnims[ninjaState].GetDestRec().Width * 0.75), ninjaLoc.Y) && samuraiDealtDmg == false && samuraiLeft == false)
                        {
                            //Lowers ninja's health by 5
                            ninjaHealth -= 5;

                            //Determines ninja is stunned and that samurai dealt damage
                            samuraiDealtDmg = true;
                            ninjaStunned = true;

                            //plays specific attack sound based on what alternating attack the samurai used to hit
                            if (samuraiAlternatingAttack == 1)
                            {
                                //plays first attack sound
                                slash1SFX.CreateInstance().Play();
                            }
                            else if (samuraiAlternatingAttack == -1)
                            {
                                //plays second attack sound
                                slash2SFX.CreateInstance().Play();
                            }
                        }
                        else if (samuraiHitRec.Contains((int)(ninjaLoc.X + ninjaAnims[ninjaState].GetDestRec().Width * 0.25), ninjaLoc.Y) && samuraiDealtDmg == false && samuraiLeft == true)
                        {
                            ninjaHealth -= 5;
                            samuraiDealtDmg = true;
                            ninjaStunned = true;

                            //plays specific attack sound based on what alternating attack the samurai used to hit
                            if (samuraiAlternatingAttack == 1)
                            {
                                //plays first attack sound
                                slash1SFX.CreateInstance().Play();
                            }
                            else if (samuraiAlternatingAttack == -1)
                            {
                                //plays second attack sound
                                slash2SFX.CreateInstance().Play();
                            }
                        }
                        else if (samuraiSwingSound == true)
                        {
                            //Plays swing sound effect as ninja did not get hit by blade
                            swingSFX.CreateInstance().Play();
                        }
                    }

                    //Sets hitboxes up and deals damage based on conditions if ninja is attacking in general
                    if (ninjaAttacking == true)
                    {
                        //Determines ninja's attack hitbox based off which way the ninja is facing
                        if (ninjaLeft == true)
                        {
                            //Sets the attack hitbox on the left side of the ninja and makes it about half the width of the ninja
                            ninjaHitRec.X = (int)(ninjaLoc.X - ninjaAnims[ninjaState].GetDestRec().Width * 0.25);
                            ninjaHitRec.Y = (int)ninjaLoc.Y - ninjaAnims[ninjaState].GetDestRec().Height;
                            ninjaHitRec.Width = (int)(ninjaAnims[ninjaState].GetDestRec().Width / 1.5);
                            ninjaHitRec.Height = ninjaAnims[ninjaState].GetDestRec().Height;
                        }
                        else if (ninjaLeft == false)
                        {
                            //Sets the attack hitbox on the right side of the ninja and makes it about half the width of the ninja
                            ninjaHitRec.X = (int)(ninjaLoc.X + ninjaAnims[ninjaState].GetDestRec().Width * 1.25);
                            ninjaHitRec.Y = (int)ninjaLoc.Y - ninjaAnims[ninjaState].GetDestRec().Height;
                            ninjaHitRec.Width = (int)(ninjaAnims[ninjaState].GetDestRec().Width / 1.5);
                            ninjaHitRec.Height = ninjaAnims[ninjaState].GetDestRec().Height;
                        }

                        //Deals damage, determines the samurai is stunned and plays a sound effect based on the attack type of the ninja, which way the samurai is facing, which way the ninja is facing and which way the samurai is blocking if they are blocking
                        if (ninjaHitRec.Contains((int)(samuraiLoc.X + samuraiAnims[samuraiState].GetDestRec().Width * 0.25), samuraiLoc.Y) && ninjaDealtDmg == false && ninjaLeft == true && samuraiLeft == false && projectileActive == false)
                        {
                            //Plays sound effect and deals damage and stun based on if the samurai is blocking
                            if (samuraiBlockingRight == false)
                            {
                                //Deals damage
                                samuraiHealth -= 3;

                                //Determine ninja dealt damage and samurai is blocking
                                ninjaDealtDmg = true;
                                samuraiStunned = true;

                                //Plays attack sound effect
                                kunaiSFX.CreateInstance().Play();
                            }
                            else if (samuraiBlockSound == true)
                            {
                                //Plays block sound effect
                                blockSFX.CreateInstance().Play();

                                //Determines block sound effect is no longer playing so it doesn't repeat
                                samuraiBlockSound = false;
                            }
                        }
                        else if (ninjaHitRec.Contains((int)(samuraiLoc.X + samuraiAnims[samuraiState].GetDestRec().Width * 0.75), samuraiLoc.Y) && ninjaDealtDmg == false && ninjaLeft == false && samuraiLeft == false && projectileActive == false)
                        {
                            //Plays sound effect and deals damage and stun based on if the samurai is blocking
                            if (samuraiBlockingLeft == false)
                            {
                                //Deals damage
                                samuraiHealth -= 3;

                                //Determine ninja dealt damage and samurai is blocking
                                ninjaDealtDmg = true;
                                samuraiStunned = true;

                                //Plays attack sound effect
                                kunaiSFX.CreateInstance().Play();
                            }
                            else if (samuraiBlockSound == true)
                            {
                                //Plays block sound effect
                                blockSFX.CreateInstance().Play();

                                //Determines block sound effect is no longer playing so it doesn't repeat
                                samuraiBlockSound = false;
                            }
                        }
                        else if (ninjaHitRec.Contains((int)(samuraiLoc.X + samuraiAnims[samuraiState].GetDestRec().Width * 0.65), samuraiLoc.Y) && ninjaDealtDmg == false && ninjaLeft == true && samuraiLeft == true && projectileActive == false)
                        {
                            //Plays sound effect and deals damage and stun based on if the samurai is blocking
                            if (samuraiBlockingRight == false)
                            {
                                //Deals damage
                                samuraiHealth -= 3;

                                //Determine ninja dealt damage and samurai is blocking
                                ninjaDealtDmg = true;
                                samuraiStunned = true;

                                //Plays attack sound effect
                                kunaiSFX.CreateInstance().Play();
                            }
                            else if (samuraiBlockSound == true)
                            {
                                //Plays block sound effect
                                blockSFX.CreateInstance().Play();

                                //Determines block sound effect is no longer playing so it doesn't repeat
                                samuraiBlockSound = false;
                            }
                        }
                        else if (ninjaHitRec.Contains((int)(samuraiLoc.X + samuraiAnims[samuraiState].GetDestRec().Width * 1.25), samuraiLoc.Y) && ninjaDealtDmg == false && ninjaLeft == false && samuraiLeft == true && projectileActive == false)
                        {
                            //Plays sound effect and deals damage and stun based on if the samurai is blocking
                            if (samuraiBlockingLeft == false)
                            {
                                //Deals damage
                                samuraiHealth -= 3;

                                //Determine ninja dealt damage and samurai is blocking
                                ninjaDealtDmg = true;
                                samuraiStunned = true;

                                //Plays attack sound effect
                                kunaiSFX.CreateInstance().Play();
                            }
                            else if (samuraiBlockSound == true)
                            {
                                //Plays block sound effect
                                blockSFX.CreateInstance().Play();

                                //Determines block sound effect is no longer playing so it doesn't repeat
                                samuraiBlockSound = false;
                            }
                        }
                        else if (samuraiRec.Contains(ninjaDartRec) && ninjaDealtDmg == false && ninjaLeft == true)
                        {
                            //Plays sound effect and deals damage and stun based on if the samurai is blocking
                            if (samuraiBlockingRight == false)
                            {
                                //Deals damage
                                samuraiHealth -= 8;

                                //Determine ninja dealt damage and samurai is blocking
                                ninjaDealtDmg = true;
                                samuraiStunned = true;

                                //Plays attack sound effect
                                kunaiSFX.CreateInstance().Play();
                            }
                            else if (samuraiBlockSound == true)
                            {
                                //Plays block sound effect
                                blockSFX.CreateInstance().Play();

                                //Determines block sound effect is no longer playing so it doesn't repeat
                                samuraiBlockSound = false;
                            }
                        }
                        else if (samuraiRec.Contains(ninjaDartRec) && ninjaDealtDmg == false && ninjaLeft == false)
                        {
                            //Plays sound effect and deals damage and stun based on if the samurai is blocking
                            if (samuraiBlockingLeft == false)
                            {
                                //Deals damage
                                samuraiHealth -= 8;

                                //Determine ninja dealt damage and samurai is blocking
                                ninjaDealtDmg = true;
                                samuraiStunned = true;

                                //Plays attack sound effect
                                kunaiSFX.CreateInstance().Play();
                            }
                            else if (samuraiBlockSound == true)
                            {
                                //Plays block sound effect
                                blockSFX.CreateInstance().Play();

                                //Determines block sound effect is no longer playing so it doesn't repeat
                                samuraiBlockSound = false;
                            }
                        }
                    }
                
                    //Translates samurai in animation to the updated location
                    samuraiAnims[samuraiState].TranslateTo(samuraiLoc.X, samuraiLoc.Y);

                    //Translates ninja in animation to the updated location
                    ninjaAnims[ninjaState].TranslateTo(ninjaLoc.X, ninjaLoc.Y);

                    break;
                case ENDGAME:

                    //Plays endgame music if no music is already playing
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(endgameMusic);
                    }

                    //Exits the game if user clicks the exit button
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (quitButtonRec.Contains(mouse.Position))
                        {
                            Exit();
                        }
                    }

                    //Sets gamestate if button is pressed
                    if (mouse.LeftButton == ButtonState.Pressed && !(prevMouse.LeftButton == ButtonState.Pressed))
                    {
                        //Updates code if menu button is pressed
                        if (menuButtonRec.Contains(mouse.Position))
                        {
                            //Stops endgame music
                            MediaPlayer.Stop();

                            //Sets gamestate to the menu
                            gameState = MENU;
                        }
                    }

                    //Update death animation based on which character's death animation is played
                    if (samuraiAnims[7].IsFinished())
                    {
                        //Samurai death animation is set to last fram, activated and paused
                        samuraiAnims[7].Activate(true);
                        samuraiAnims[7].SetFrame(6);
                        samuraiAnims[7].Pause();
                    }
                    else if (ninjaAnims[5].IsFinished())
                    {
                        //Ninja death animation is set to last frame, activated and paused
                        ninjaAnims[5].Activate(true);
                        ninjaAnims[5].SetFrame(4);
                        ninjaAnims[5].Pause();
                    }
                    break;
            }   

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draws graphics based off which gamestate players are currently in
            switch (gameState)
            {
                case MENU:
                    //Draws menu background
                    spriteBatch.Draw(menuBG, menuBGRec, Color.White);

                    //Draws play and quit button
                    spriteBatch.Draw(playButtonImg, playButtonRec, Color.Blue);
                    spriteBatch.Draw(quitButtonImg, quitButtonRec, Color.White);

                    //Writes title and the shadow behind it
                    StringShadow(titleFont, titleLine, titleLoc, Color.Black, 5);
                    spriteBatch.DrawString(titleFont, titleLine, titleLoc, Color.Blue);

                    break;
                case PLAYING:
                    //Draws the game background with a lowered opacity
                    spriteBatch.Draw(gameBG, gameBGRec, Color.White * 0.7f);

                    //Draws all three platforms
                    spriteBatch.Draw(platform, platformRec1, Color.White);
                    spriteBatch.Draw(platform, platformRec2, Color.White);
                    spriteBatch.Draw(platform, platformRec3, Color.White);

                    //Draws ninja dart
                    spriteBatch.Draw(ninjaDartImg, ninjaDartRec, Color.White);

                    //Writes the shadows for the ninja and samurai's health caption
                    StringShadow(regularFont, samuraiHealthLine, healthLine1Loc, Color.Black, 3);
                    StringShadow(regularFont, ninjaHealthLine, healthLine2Loc, Color.Black, 3);

                    //Writes ninja and samurai's health caption
                    spriteBatch.DrawString(regularFont, samuraiHealthLine, healthLine1Loc, Color.White);
                    spriteBatch.DrawString(regularFont, ninjaHealthLine, healthLine2Loc, Color.White);

                    //Writes the shadows for the ninja and samurai's health count
                    StringShadow(regularFont, Convert.ToString(ninjaHealth), ninjaHealthLineLoc, Color.Black, 3);
                    StringShadow(regularFont, Convert.ToString(samuraiHealth), samuraiHealthLineLoc, Color.Black, 3);

                    //Writes ninja and samurai's health count
                    spriteBatch.DrawString(regularFont, Convert.ToString(ninjaHealth), ninjaHealthLineLoc, Color.White);
                    spriteBatch.DrawString(regularFont, Convert.ToString(samuraiHealth), samuraiHealthLineLoc, Color.White);

                    //Updates the animation drawing
                    samuraiAnims[samuraiState].Update(gameTime);
                    ninjaAnims[ninjaState].Update(gameTime);

                    //Draws samurai based off which way they are facing
                    if (samuraiLeft == false)
                    {
                        //Draws samurai facing right
                        samuraiAnims[samuraiState].Draw(spriteBatch, Color.White);
                    }
                    else if (samuraiLeft == true)
                    {
                        //Draws samurai facing left
                        samuraiAnims[samuraiState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                    }

                    //Draws ninja based off which way they are facing
                    if (ninjaLeft == false)
                    {
                        //Draws ninja facing right
                        ninjaAnims[ninjaState].Draw(spriteBatch, Color.White);
                    }
                    else if (ninjaLeft == true)
                    {
                        //Draws ninja facing left
                        ninjaAnims[ninjaState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                    }

                    break;
                case ENDGAME:

                    //Draws endgame background
                    spriteBatch.Draw(endgameBG, endgameBGRec, Color.White);

                    //Draws menu and quit buttons
                    spriteBatch.Draw(quitButtonImg, quitButtonRec, Color.White);
                    spriteBatch.Draw(menuButtonImg, menuButtonRec, Color.Red);

                    //Writes winner announcment and the shadow behind it
                    StringShadow(regularFont, announceWinner, announceWinnerLoc, Color.DarkRed, 3);
                    spriteBatch.DrawString(regularFont, announceWinner, announceWinnerLoc, Color.Red);

                    //Updates the death animation drawing
                    samuraiAnims[SAMURAI_DEAD].Update(gameTime);
                    ninjaAnims[NINJA_DEAD].Update(gameTime);
        
                    //Animates a character based off which one was beat in the gameplay
                    if (samuraiDied == true)
                    {
                        //Animates samurai dying
                        samuraiAnims[SAMURAI_DEAD].Draw(spriteBatch, Color.White);
                    }
                    else
                    {
                        //Animates ninja dying
                        ninjaAnims[NINJA_DEAD].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                    }

                    break;
            }

            spriteBatch.End();
            

            base.Draw(gameTime);
        }

        //Pre: Font exsists and string is not null
        //Post: Returns the X value to center text
        //Desc: Calculates where your text X location should be in order for your text to be centered
        private int GetCentreTextX(SpriteFont font, string text)
        {
            return (int)(screenWidth / 2 - font.MeasureString(text).X / 2);
        }

        //Pre: Font exsists and string is not null
        //Post: Returns the Y value to center text
        //Desc: Calculates where your text Y location should be in order for your text to be centered
        private int GetCentreTextY(SpriteFont font, string text)
        {
            return (int)(screenHeight / 2 - font.MeasureString(text).Y / 2);
        }

        //Pre: Texture is not null and size ratio is not negative
        //Post: Returns graphic X location to center it
        //Desc: Calculates where the X location should be for a graphic to be centered
        private int GraphicCenterX(Texture2D img, int sizeRatio)
        {
            Rectangle graphicRec = new Rectangle();
            graphicRec.X = screenWidth / 2 - img.Width / (2 * sizeRatio);
            return graphicRec.X;
        }

        //Pre: Font exsists, string is not null, Vector is not null and Color is valid
        //Post: None
        //Desc: Draws a shadow behind text to make it stand out and more aesthetically pleasing
        private void StringShadow(SpriteFont font, string line, Vector2 loc, Color color, int shadowSize)
        {
            Vector2 shadowLoc;
            shadowLoc.X = loc.X + shadowSize;
            shadowLoc.Y = loc.Y + shadowSize;
            spriteBatch.DrawString(font, line, shadowLoc, color);
        }

        //Pre: Animation number is within animation array and is valid for samurai
        //Post: None
        //Desc: Resets the samurai's animation back to the first frame
        private void SamuraiResetAnimation(int animationNum)
        {
            samuraiAnims[animationNum].SetFrame(0);
        }

        //Pre: Animation number is within animation array and is valid for ninja
        //Post: None
        //Desc: Resets the ninja's animation back to the first frame
        private void NinjaResetAnimation(int animationNum)
        {
            ninjaAnims[animationNum].SetFrame(0);
        }
    }
}