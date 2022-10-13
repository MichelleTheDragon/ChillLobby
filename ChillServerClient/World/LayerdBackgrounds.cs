using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChillServerClient.World
{
    internal class LayerdBackgrounds : Components
    {
        private bool _isMoving; //Will the layer move by itself?
        private float _backLayer; //Bg layer
        private float _movingSpeed;  //moving speed of the background.
        private List<Sprites> _sprites; //List of the backgrounds, characters and other assets
        //private readonly Player _player; //If we want the player to affect something while moveing.

        /// <summary>
        /// To set the layer the background lies on
        /// </summary>
        public float Layer
        {
            get { return _backLayer; }  //Get this data from where the background is being made
            set
            {
                _backLayer = value;                         //layer has value
                foreach (var sprite in _sprites)    //and for each texture/psrite in backgrounds list
                {
                    sprite.Layer = _backLayer;              //give it a layer
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="scrollingSpeed"></param>
        /// <param name="constSpeed"></param>
        public LayerdBackgrounds(Texture2D texture, float scrollingSpeed, bool constSpeed = false)
            : this(new List<Texture2D>() { texture, texture }, scrollingSpeed, constSpeed)
        {

        }

        /// <summary>
        /// Constructor with fields
        /// </summary>
        /// <param name="textures"></param>
        /// <param name="scrollingSpeed"></param>
        /// <param name="constSpeed"></param>
        public LayerdBackgrounds(List<Texture2D> textures, float scrollingSpeed, bool constSpeed = false)
        {
            _movingSpeed = scrollingSpeed;            //set the speed of it moving on the screen
            _isMoving = constSpeed;             //is it oving constantly without being invoked to?
            _sprites = new List<Sprites>();  //this is a new list

            for (int i = 0; i < textures.Count; i++)    //as long as this applies:
            {
                var texture = textures[i];              //save texture/sprite in the constructor list

                _sprites.Add(new Sprites(texture)    //add this texture to the background list
                {
                    //Position = new Vector2(i * texture.Width - 1, GameWorld.WantedHeight + texture.Height) //Keeps image on bottom
                                                                                                           //i * texture.Width = moves the next sprite out of the screen
                });
            }
        }

        /// <summary>
        /// draw the textures
        /// </summary>
        /// <param name="gametime"></param>
        /// <param name="_spriteBatch"></param>
        public override void Draw(GameTime gametime, SpriteBatch _spriteBatch)
        {
            foreach (var sprite in _sprites)        //as long as this applies
            {
                sprite.Draw(gametime, _spriteBatch);    //Draw them
            }
        }

        /// <summary>
        /// update their position
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
            ApplySpeed(gametime);   //speed of them moving
            CheckPosition();        //Do this method
        }

        /// <summary>
        /// Apply speed to the background so its moving :D
        /// </summary>
        /// <param name="gametime"></param>
        private void ApplySpeed(GameTime gametime) //Speed for the gametime and the moving background
        {

            foreach (var sprite in _sprites) // For every sprite in that list
            {
                sprite.Position.X -= _movingSpeed;     //set their position to be moved
            }
        }
        /// <summary>
        /// background positon and if it needs to be moved
        /// </summary>
        private void CheckPosition()
        {
            for (int i = 0; i < _sprites.Count; i++)    //As long as this applies
            {
                var sprite = _sprites[i];               //save the background as a sprite

                if (sprite.Rect.Right <= 0)                 //and if it is on the right
                {
                    var index = i - 1;                      //moving it to the 2 position

                    if (index < 0)                          //and if it is number 0 in the list
                    {
                        index = _sprites.Count - 1;     //move it on the other side. 0123 - 0-1 = 3 and now its 1230

                        sprite.Position.X = _sprites[index].Rect.Right - _movingSpeed * 2; //To avoid a white line besteen sprites
                    }
                }
            }
        }
    }
}
