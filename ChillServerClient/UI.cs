using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
        private bool isLoggedIn = false;
        private GraphicsDevice _graphicsDevice;
        int boxWidth1 = 150;
        int boxHeight1 = 26;
        int boxWidth2 = 110;
        int boxHeight2 = 26;
        int boxWidth3 = 220;
        int boxHeight3 = 26;
        int boxWidth4 = 150;
        int boxHeight4 = 26;
        Vector2 coor1;
        Vector2 coor2;
        Vector2 coor3;
        Vector2 coor4;
        Vector2 coor5;
        Vector2 coor6;
        Texture2D rect1;
        Texture2D rect2;
        Texture2D rect3;
        Texture2D rect4;
        Texture2D rect5;
        Texture2D rect6;
        bool isHovered1;
        bool isHovered2;
        bool isHovered3;
        bool isHovered4;
        bool isHovered5;
        bool isHovered6;
        bool clickRegistered1;
        bool clickRegistered2;
        bool clickRegistered3;
        bool clickRegistered4;
        bool clickRegistered5;
        bool clickRegistered6;


        private Keys[] lastPressedKeys;
        bool shiftDown;

        int writingArea = 0;

        bool isLoading;
        Thread newTest;
        string username = "username";
        string password = "password";
        string serverIp = "Ip Address";//192.168.68.107:11000

        Color[] data1;
        Color[] data2;
        Color[] data3;
        Color[] data4;

        ConnectionToServer myConnection = new ConnectionToServer();// "192.168.1.75");

        public UI(ContentManager content, GraphicsDevice graphicsDevice)
        {
            baseFont = content.Load<SpriteFont>("Fonts/BasicFont");
            _graphicsDevice = graphicsDevice;
            lastPressedKeys = new Keys[0];

            rect1 = new Texture2D(_graphicsDevice, boxWidth1, boxHeight1);
            rect2 = new Texture2D(_graphicsDevice, boxWidth1, boxHeight1);
            rect3 = new Texture2D(_graphicsDevice, boxWidth2, boxHeight2);
            rect4 = new Texture2D(_graphicsDevice, boxWidth2, boxHeight2);
            rect5 = new Texture2D(_graphicsDevice, boxWidth3, boxHeight3);
            rect6 = new Texture2D(_graphicsDevice, boxWidth4, boxHeight4);
            data1 = new Color[boxWidth1 * boxHeight1];
            data2 = new Color[boxWidth2 * boxHeight2];
            data3 = new Color[boxWidth3 * boxHeight3];
            data4 = new Color[boxWidth4 * boxHeight4];
            rect1.SetData(ChangeColour(data1, Color.White));
            rect2.SetData(ChangeColour(data1, Color.White));
            rect3.SetData(ChangeColour(data2, Color.White));
            rect4.SetData(ChangeColour(data2, Color.White));
            rect5.SetData(ChangeColour(data3, Color.White));
            rect6.SetData(ChangeColour(data4, Color.White));
            coor1 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 20, _graphicsDevice.Viewport.Height / 2.0f - 29);
            coor2 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 20, _graphicsDevice.Viewport.Height / 2.0f + 11);
            coor3 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 115, _graphicsDevice.Viewport.Height / 2.0f + 51);
            coor4 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f + 10, _graphicsDevice.Viewport.Height / 2.0f + 51);
            coor5 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 70, _graphicsDevice.Viewport.Height / 2.0f - 11);
            coor6 = new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 80, _graphicsDevice.Viewport.Height / 2.0f + 21);
            //newTest = new Thread(new ThreadStart(Test));
        }

        private void OnKeyDown(Keys key)
        {
            string newString;
            if ((key == Keys.LeftShift || key == Keys.RightShift) && shiftDown == false)
            {
                shiftDown = true;
            }
            if (((key >= Keys.A && key <= Keys.Z) || (key >= Keys.D0 && key <= Keys.D9)) && writingArea <= 2)
            {
                if (shiftDown)
                {
                    newString = key.ToString();
                } else
                {
                    newString = key.ToString().ToLower();
                    if (key >= Keys.D0 && key <= Keys.D9)
                    {
                        newString = newString[newString.Length - 1].ToString();
                    }
                }
                switch (writingArea)
                {
                    case 1:
                        username += newString;
                        break;
                    case 2:
                        password += newString;
                        break;
                }
            }
            else if((key >= Keys.D0 && key <= Keys.D9 || key == Keys.OemPeriod) && writingArea == 3)
            {
                char newChar;
                if (shiftDown && key == Keys.OemPeriod)
                {
                    newChar = ':';
                } else if (key == Keys.OemPeriod)
                {
                    newChar = '.';
                }
                else
                {
                    newString = key.ToString().ToLower();
                    newChar = newString[newString.Length - 1];
                }
                serverIp += newChar;
            }
        }

        private void OnKeyUp(Keys key)
        {
            if ((key == Keys.LeftShift || key == Keys.RightShift) && shiftDown == true)
            {
                shiftDown = false;
            } else if (key == Keys.Back)
            {
                switch (writingArea)
                {
                    case 1:
                        if (username.Length > 0)
                        {
                            username = username.Remove(username.Length - 1, 1);
                        }
                        break;
                    case 2:
                        if (password.Length > 0)
                        {
                            password = password.Remove(password.Length - 1, 1);
                        }
                        break;
                    case 3:
                        if (serverIp.Length > 0)
                        {
                            serverIp = serverIp.Remove(serverIp.Length - 1, 1);
                        }
                        break;
                }
            }
        }

        public void Test(int value)
        {
            isLoading = true;
            switch (value)
            {
                case 1:
                    myConnection.CreateUserAsync(username, password).GetAwaiter().GetResult();
                    break;
                case 2:
                    if (myConnection.LoginUserAsync(username, password).GetAwaiter().GetResult() == true)
                    {
                        isLoggedIn = true;
                    }
                    break;
                case 3:
                    if (myConnection.ConnectToServer(serverIp) == true)
                    {
                        hasJoinedServer = true;
                    }
                    break;
            }
            //if (myConnection.CreateUserAsync(username, password).GetAwaiter().GetResult() == true)
            //{
            //    if (myConnection.LoginUserAsync(username, password).GetAwaiter().GetResult() == true)
            //    {
            //        myConnection.ConnectToServer(serverIp);// "192.168.68.107:11000");
            //    }
            //}
            isLoading = false;
        }

        public void Update(GameTime gameTime)
        {
            if (writingArea > 0)
            {
                KeyboardState kbState = Keyboard.GetState();
                Keys[] pressedKeys = kbState.GetPressedKeys();

                foreach (Keys key in lastPressedKeys)
                {
                    if (!pressedKeys.Contains(key))
                    {
                        OnKeyUp(key);
                    }
                }

                foreach (Keys key in pressedKeys)
                {
                    if (!lastPressedKeys.Contains(key))
                    {
                        OnKeyDown(key);
                    }
                }

                lastPressedKeys = pressedKeys;
            }

            if (hasJoinedServer != true)
            {
                MouseState mouseState = Mouse.GetState();
                Point mousePoint = new Point(mouseState.X, mouseState.Y);
                if (isLoggedIn != true)
                {
                    Rectangle rectangle1 = new Rectangle((int)coor1.X, (int)coor1.Y, rect1.Width, rect1.Height);
                    if (rectangle1.Contains(mousePoint) && isHovered1 != true && mouseState.LeftButton == ButtonState.Released)
                    {
                        isHovered1 = true;
                        rect1.SetData(ChangeColour(data1, Color.LightBlue));
                    }
                    else if (!rectangle1.Contains(mousePoint) && isHovered1 == true)
                    {
                        rect1.SetData(ChangeColour(data1, Color.White));
                        isHovered1 = false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed && isHovered1 && !clickRegistered1)
                    {
                        writingArea = 1;
                        username = "";
                        clickRegistered1 = true;
                    }
                    else if (mouseState.LeftButton == ButtonState.Released && clickRegistered1)
                    {
                        clickRegistered1 = false;
                    }

                    Rectangle rectangle2 = new Rectangle((int)coor2.X, (int)coor2.Y, rect2.Width, rect2.Height);
                    if (rectangle2.Contains(mousePoint) && isHovered2 != true && mouseState.LeftButton == ButtonState.Released)
                    {
                        isHovered2 = true;
                        rect2.SetData(ChangeColour(data1, Color.LightBlue));
                    }
                    else if (!rectangle2.Contains(mousePoint) && isHovered2 == true)
                    {
                        rect2.SetData(ChangeColour(data1, Color.White));
                        isHovered2 = false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed && isHovered2 && !clickRegistered2)
                    {
                        writingArea = 2;
                        password = "";
                        clickRegistered2 = true;
                    }
                    else if (mouseState.LeftButton == ButtonState.Released && clickRegistered2)
                    {
                        clickRegistered2 = false;
                    }

                    Rectangle rectangle3 = new Rectangle((int)coor3.X, (int)coor3.Y, rect3.Width, rect3.Height);
                    if (rectangle3.Contains(mousePoint) && isHovered3 != true && mouseState.LeftButton == ButtonState.Released)
                    {
                        isHovered3 = true;
                        rect3.SetData(ChangeColour(data2, Color.LightBlue));
                    }
                    else if (!rectangle3.Contains(mousePoint) && isHovered3 == true)
                    {
                        rect3.SetData(ChangeColour(data2, Color.White));
                        isHovered3 = false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed && isHovered3 && !clickRegistered3)
                    {
                        if (isLoading != true)
                        {
                            Test(1);
                        }
                        clickRegistered3 = true;
                    }
                    else if (mouseState.LeftButton == ButtonState.Released && clickRegistered3)
                    {
                        clickRegistered3 = false;
                    }

                    Rectangle rectangle4 = new Rectangle((int)coor4.X, (int)coor4.Y, rect4.Width, rect4.Height);
                    if (rectangle4.Contains(mousePoint) && isHovered4 != true && mouseState.LeftButton == ButtonState.Released)
                    {
                        isHovered4 = true;
                        rect4.SetData(ChangeColour(data2, Color.LightBlue));
                    }
                    else if (!rectangle4.Contains(mousePoint) && isHovered4 == true)
                    {
                        rect4.SetData(ChangeColour(data2, Color.White));
                        isHovered4 = false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed && isHovered4 && !clickRegistered4)
                    {
                        if (isLoading != true)
                        {
                            Test(2);
                        }
                        clickRegistered4 = true;
                    }
                    else if (mouseState.LeftButton == ButtonState.Released && clickRegistered4)
                    {
                        clickRegistered4 = false;
                    }
                } else
                {

                    Rectangle rectangle5 = new Rectangle((int)coor5.X, (int)coor5.Y, rect5.Width, rect5.Height);
                    if (rectangle5.Contains(mousePoint) && isHovered5 != true && mouseState.LeftButton == ButtonState.Released)
                    {
                        isHovered5 = true;
                        rect5.SetData(ChangeColour(data3, Color.LightBlue));
                    }
                    else if (!rectangle5.Contains(mousePoint) && isHovered5 == true)
                    {
                        rect5.SetData(ChangeColour(data3, Color.White));
                        isHovered5 = false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed && isHovered5 && !clickRegistered5)
                    {
                        writingArea = 3;
                        serverIp = "";
                        clickRegistered5 = true;
                    }
                    else if (mouseState.LeftButton == ButtonState.Released && clickRegistered5)
                    {
                        clickRegistered5 = false;
                    }

                    Rectangle rectangle6 = new Rectangle((int)coor6.X, (int)coor6.Y, rect6.Width, rect6.Height);
                    if (rectangle6.Contains(mousePoint) && isHovered6 != true && mouseState.LeftButton == ButtonState.Released)
                    {
                        isHovered6 = true;
                        rect6.SetData(ChangeColour(data4, Color.LightBlue));
                    }
                    else if (!rectangle6.Contains(mousePoint) && isHovered6 == true)
                    {
                        rect6.SetData(ChangeColour(data4, Color.White));
                        isHovered6 = false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed && isHovered6 && !clickRegistered6)
                    {
                        if (isLoading != true)
                        {
                            Test(3);
                        }
                        clickRegistered6 = true;
                    }
                    else if (mouseState.LeftButton == ButtonState.Released && clickRegistered6)
                    {
                        clickRegistered6 = false;
                    }
                    //if (isLoading != true)
                    //{
                    //    newTest = new Thread(new ThreadStart(Test));
                    //    newTest.Start();
                    //}
                }
            }
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
                    _spriteBatch.DrawString(baseFont, "Loading... ", new Vector2(0, 0), Color.White);
                }
                if (isLoggedIn != true)
                {
                    _spriteBatch.DrawString(baseFont, "Username:", new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 120, _graphicsDevice.Viewport.Height / 2.0f - 25), Color.White);
                    _spriteBatch.DrawString(baseFont, "Password:", new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 120, _graphicsDevice.Viewport.Height / 2.0f + 15), Color.White);
                    _spriteBatch.Draw(rect1, coor1, Color.White);
                    _spriteBatch.Draw(rect2, coor2, Color.White);
                    _spriteBatch.Draw(rect3, coor3, Color.White);
                    _spriteBatch.Draw(rect4, coor4, Color.White);
                    _spriteBatch.DrawString(baseFont, username, new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 10, _graphicsDevice.Viewport.Height / 2.0f - 25), Color.Black);
                    _spriteBatch.DrawString(baseFont, password, new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 10, _graphicsDevice.Viewport.Height / 2.0f + 15), Color.Black);
                    _spriteBatch.DrawString(baseFont, "Create User", new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 102, _graphicsDevice.Viewport.Height / 2.0f + 55), Color.Black);
                    _spriteBatch.DrawString(baseFont, "Login", new Vector2(_graphicsDevice.Viewport.Width / 2.0f + 46, _graphicsDevice.Viewport.Height / 2.0f + 55), Color.Black);
                }
                else
                {
                    _spriteBatch.DrawString(baseFont, "Server IP:", new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 150, _graphicsDevice.Viewport.Height / 2.0f - 6), Color.White);
                    _spriteBatch.Draw(rect5, coor5, Color.White);
                    _spriteBatch.DrawString(baseFont, serverIp, new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 40, _graphicsDevice.Viewport.Height / 2.0f - 6), Color.Black);
                    _spriteBatch.Draw(rect6, coor6, Color.White);
                    _spriteBatch.DrawString(baseFont, "Connect", new Vector2(_graphicsDevice.Viewport.Width / 2.0f - 35, _graphicsDevice.Viewport.Height / 2.0f + 25), Color.Black);
                }
            }

            _spriteBatch.End();

        }
    }
}
