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
        public Vector2 Position { get; private set; }
        public Color color = Color.White;

        public int MapPositionX { get; private set; }
        public int MapPositionY { get; private set; }
        public bool IsSolid { get; private set; }

        public int gCost; // Cost from starting tile
        public int hCost; // How far away from end node
        public int fCost; // gCost + hCost

        public Tile(Texture2D _texture, Vector2 position, int x, int y)
        {
            texture = _texture;
            Position = position;
            MapPositionX = x;
            MapPositionY = y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, color);

            spriteBatch.DrawString(Game1.font, hCost.ToString(), new Vector2(Position.X + texture.Width / 2 - 10, Position.Y + texture.Height / 2 - 5), Color.Red);
            spriteBatch.DrawString(Game1.font, gCost.ToString(), new Vector2(Position.X + texture.Width / 2 + 10, Position.Y + texture.Height / 2 - 5), Color.Green);
        }
    }
}
