using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChillServerClient
{
    internal class UI
    {
        private SpriteFont baseFont;
        private bool hasJoinedServer = false;
        private GraphicsDevice _graphicsDevice;
        int boxWidth = 150;
        int boxHeight = 26;
        Vector2 coor1;
        Vector2 coor2;
        Texture2D rect1;
        Texture2D rect2;
        bool isHovered1;
        bool isHovered2;
        bool clickRegistered1;
        bool isLoading;
        Thread newTest;
        int loadingMsg;

        Color[] data;

        ConnectionToServer myConnection = new ConnectionToServer();// "192.168.1.75");

        public UI(ContentManager content, GraphicsDevice graphicsDevice)
        {
            baseFont = content.Load<SpriteFont>("Fonts/BasicFont");
            _graphicsDevice = graphicsDevice;

            rect1 = new Texture2D(_graphicsDevice, boxWidth, boxHeight);
            rect2 = new Texture2D(_graphicsDevice, boxWidth, boxHeight);
            data = new Color[boxWidth * boxHeight];
            rect1.SetData(ChangeColour(data, Color.White));
            rect2.SetData(ChangeColour(data, Color.White));
            coor1 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 20, _graphicsDevice.Viewport.Height / 2.0f - 29);
            coor2 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 20, _graphicsDevice.Viewport.Height / 2.0f + 11);
            newTest = new Thread(new ThreadStart(Test));
        }

        public void Test()
        {
            isLoading = true;

            loadingMsg = 1;
            if (myConnection.CreateUserAsync("dddd", "dawdawf").GetAwaiter().GetResult() == true)
            {
                loadingMsg = 2;
                if (myConnection.LoginUserAsync("dddd", "dawdawf").GetAwaiter().GetResult() == true)
                {
                    loadingMsg = 3;
                    myConnection.ConnectToServer("192.168.68.107:11000");
                }
            }
            loadingMsg = 4;
            isLoading = false;
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePoint = new Point(mouseState.X, mouseState.Y);
            //Rectangle rectangle1 = new Rectangle((int)coor1.X - rect1.Width / 2, (int)coor1.Y - rect1.Height / 2, rect1.Width, rect1.Height);
            //Rectangle rectangle2 = new Rectangle((int)coor2.X - rect2.Width / 2, (int)coor2.Y - rect2.Height / 2, rect2.Width, rect2.Height);
            Rectangle rectangle1 = new Rectangle((int)coor1.X, (int)coor1.Y, rect1.Width, rect1.Height);
            Rectangle rectangle2 = new Rectangle((int)coor2.X, (int)coor2.Y, rect2.Width, rect2.Height);
            if (rectangle1.Contains(mousePoint) && isHovered1 != true && mouseState.LeftButton == ButtonState.Released)
            {
                isHovered1 = true;
                rect1.SetData(ChangeColour(data, Color.LightBlue));
            }
            else if (!rectangle1.Contains(mousePoint) && isHovered1 == true)
            {
                rect1.SetData(ChangeColour(data, Color.White));
                isHovered1 = false;
            }
            if (mouseState.LeftButton == ButtonState.Pressed && isHovered1 && !clickRegistered1)
            {
                if (isLoading != true)
                {
                    newTest = new Thread(new ThreadStart(Test));
                    newTest.Start();
                }
                clickRegistered1 = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released && clickRegistered1)
            {
                clickRegistered1 = false;
            }

            if (rectangle2.Contains(mousePoint) && isHovered2 != true && mouseState.LeftButton == ButtonState.Released)
            {
                isHovered2 = true;
                rect2.SetData(ChangeColour(data, Color.LightBlue));
            }
            else if (!rectangle2.Contains(mousePoint) && isHovered2 == true)
            {
                rect2.SetData(ChangeColour(data, Color.White));
                isHovered2 = false;
            }

            //if (true)
            //{
            //    myConnection.ConnectToServer("");
            //}
        }

        public Color[] ChangeColour(Color[] colourObject, Color newColour)
        {
            for (int i = 0; i < colourObject.Length; i++)
            {
                colourObject[i] = newColour;
            }
            return colourObject;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
            if (hasJoinedServer)
            {

            } else
            {
                if (isLoading == true)
                {
                    _spriteBatch.DrawString(baseFont, "Loading... " + loadingMsg, new Vector2(0, 0), Color.White);
                }
                _spriteBatch.DrawString(baseFont, "Username:", new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 120, _graphicsDevice.Viewport.Height/2.0f - 25), Color.White);
                _spriteBatch.DrawString(baseFont, "Password:", new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 120, _graphicsDevice.Viewport.Height/2.0f + 15), Color.White);
                _spriteBatch.Draw(rect1, coor1, Color.White);
                _spriteBatch.Draw(rect2, coor2, Color.White);
            }

            _spriteBatch.End();

        }
    }
}
