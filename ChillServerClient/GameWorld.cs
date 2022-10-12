using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using System.Net;

namespace ChillServerClient
{
    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ConnectionToServer myConnection;
        private Texture2D background;
        private Texture2D garden;
        private Texture2D tables;
        public Vector2 speed;


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
            _graphics.SynchronizeWithVerticalRetrace = true;    //Enable VSync.
            _graphics.ApplyChanges();                           //Needed for the resolution to change

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Asset layers
            background = Content.Load<Texture2D>("Backgrounds\\FuldBgwMat");    //Floor and walls
            tables = Content.Load<Texture2D>("Backgrounds\\Tabled");            //Tables
            garden = Content.Load<Texture2D>("Backgrounds\\Tabled");            //Garden

            //Server Connection
            //string hostName = Dns.GetHostName()
            //string myIP = Dns.GetHostEntry(hostName).AddressList[3].ToString();
            //myConnection = new ConnectionToServer("87.49.251.155");// "192.168.1.75");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSeaGreen);
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            // TODO: Add your drawing code here
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}