using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using ChillServerClient.World;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace ChillServerClient
{
    public class GameWorld : Game
    {
        #region properties
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //Server
        private ConnectionToServer myConnection;
        private UI myUI;
        //Background
        private Texture2D background;
        private Texture2D garden;
        private Texture2D tables;
        private List<LayerdBackgrounds> _backgroundList;   //List for the background effect.
        private List<Components> _gameComponents;           //omponents in the game

        public Vector2 speed;
        //Resolution
        public const int WantedWidth = 1600;    //The width I want for the window
        public const int WantedHeight = 900;    ///The height I want for the window
        //sound
        SoundEffect sound;  //For the sound effect on buttons/clicks
        Song song;          //for the background songs
        #endregion

        public GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Program settings
            Window.Title = "Chill Lobby";            //Sets the window title
            Window.AllowAltF4 = true;

            // Window sizeing
            _graphics.IsFullScreen = false;                     //Is it fullscreen? No.
            Window.AllowUserResizing = true;                    //Allows users to be able to drag the window to preferred size.
            _graphics.PreferredBackBufferWidth = 1600;          //Width of the window
            _graphics.PreferredBackBufferHeight = 900;          //Height of the window
            //_graphics.SynchronizeWithVerticalRetrace = true;    //Enable VSync.
            _graphics.ApplyChanges();                           //Needed for the resolution to change

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            myUI = new UI(Content, GraphicsDevice);

            //string hostName = Dns.GetHostName();
            //string myIP = Dns.GetHostEntry(hostName).AddressList[3].ToString();
            //myConnection = new ConnectionToServer();// "192.168.1.75");
            //myConnection.ConnectToServer(myIP);
            // TODO: use this.Content to load your game content here
            //Asset layers
            //background = Content.Load<Texture2D>("Backgrounds\\FuldBgwMat");    //Floor and walls
            //tables = Content.Load<Texture2D>("Backgrounds\\Tabled");            //Tables
            //garden = Content.Load<Texture2D>("Backgrounds\\Tabled");            //Garden

            //Server Connection
            //string hostName = Dns.GetHostName()
            //string myIP = Dns.GetHostEntry(hostName).AddressList[3].ToString();
            //myConnection = new ConnectionToServer("87.49.251.155");// "192.168.1.75");

            /// <summary>
            /// Backgrounds
            /// (ontent.Load<Texture2D>("Backgrounds\\Tabled"),_player, 0f, true)
            /// Texture = The texture sprite
            /// _player = if player affects the sprite (moves it by position - we dont use it)
            /// 0f      = 0 means layer is not moving by itself (scroling effect) it is also its speed. set it to 0.1 and itll move slowly to the left
            /// true    = Layer would be constant, repeating once it moved all the way to the left, itll reappear.
            /// Layer = the layer it lies on. I put 0.5 as standard.
            /// </summary>
            _backgroundList = new List<LayerdBackgrounds>()    //the list and this is what will be in it
            {
                 new LayerdBackgrounds(Content.Load<Texture2D>("Backgrounds\\FuldBgwMat"),  0f) //not moving //If player affects it, player property is put before the float
                {
                    Layer = 0.50f,  //The layer it lies on
                },
                new LayerdBackgrounds(Content.Load<Texture2D>("Backgrounds\\Tabled"), 0f)  //the moving light background
                {
                    Layer = 0.51f,  //The layer it lies on
                }
            };
            //Dummy player code til når player er sat ind, kan self rettes til
            //_player = new Player(PLAYERTEXTURE2D)
            //{
            //    Position = new Vector2(50, 50),
            //    Layerd = 0.60f;
            //};

            //sound
            song = Content.Load<Song>("Sounds\\You and the Sea");        //The background music
            MediaPlayer.IsRepeating = true; //for the loop
            MediaPlayer.Play(song);         //Plays it
        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //background
            foreach (var bgl in _backgroundList)    //Background loop = bgl
            {
                bgl.Update(gameTime);
            }

            myUI.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSeaGreen);       //Default background
            _spriteBatch.Begin(SpriteSortMode.FrontToBack); //the order in which the sprites for the background are. front first then the other lies behind it
            foreach (var bg in _backgroundList)             //bg = background
            {
                bg.Draw(gameTime, _spriteBatch);
            }
                //_spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                // TODO: Add your drawing code here
                _spriteBatch.End();
                myUI.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
    }
}
