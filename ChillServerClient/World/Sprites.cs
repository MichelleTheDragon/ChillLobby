using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChillServerClient.World
{
    internal class Sprites : Components
    {
        protected float _layer { get; set; }
        protected Texture2D _texture;
        public Vector2 Position;

        public float Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        public Sprites(Texture2D texture)
        {
            _texture = texture;
        }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public override void Draw(GameTime gametime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(_texture, Position, null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);
        }

        public override void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }
    }
}
