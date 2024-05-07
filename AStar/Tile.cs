using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class Tile
    {
        private Texture2D texture;
        private Vector2 position;
        public Color color = Color.White;

        public int MapPositionX { get; private set; }
        public int MapPositionY { get; private set; }
        public bool IsSolid { get; private set; }

        public int gCost; // Cost from starting tile
        public int hCost; // How far away from end node
        public int fCost; // gCost + hCost

        public Tile(Texture2D _texture, Vector2 _position, int x, int y)
        {
            texture = _texture;
            position = _position;
            MapPositionX = x;
            MapPositionY = y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, color);

            spriteBatch.DrawString(Game1.font, hCost.ToString(), new Vector2(position.X + texture.Width / 2, position.Y + texture.Height / 2), Color.Blue);
        }
    }
}
