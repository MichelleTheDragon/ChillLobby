using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChillServerClient
{
    internal abstract class Components //for easy acces to acess it all over the project
    {
        public abstract void Draw(GameTime gametime, SpriteBatch _spriteBatch);
        public abstract void Update(GameTime gametime);
    }
}
