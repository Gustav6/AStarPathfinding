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
        public Rectangle Bounds { get; private set; }
        public Color color = Color.White;

        public int MapPositionX { get; private set; }
        public int MapPositionY { get; private set; }
        public bool IsSolid { get; private set; }

        public int gCost; // Cost from starting tile
        public int hCost; // How far away from end node
        public int fCost; // GCost + hCost
        public Tile parent; // What node that "owns" the current node

        public string text = "";
        public Vector2 fontSize;

        public Tile(Texture2D _texture, Vector2 position, int x, int y, TIleType type)
        {
            texture = _texture;
            Position = position;
            MapPositionX = x;
            MapPositionY = y;

            Bounds = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            if (type == TIleType.unPassable)
            {
                IsSolid = true;
            }
        }

        public void ResetTile()
        {
            text = string.Empty;
            gCost = 0;
            hCost = 0;
            fCost = 0;
            color = Color.White;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, color);

            //spriteBatch.DrawString(Game1.font, hCost.ToString(), new Vector2(Position.X + texture.Width / 2 - 20, Position.Y + texture.Height / 2 - 5), Color.Yellow);
            spriteBatch.DrawString(Game1.font, gCost.ToString(), new Vector2(Position.X + texture.Width / 2 - 5, Position.Y + texture.Height / 2 - 5), Color.Black);

            if (fCost != 0)
            {
                //spriteBatch.DrawString(Game1.font, fCost.ToString(), new Vector2(Position.X + texture.Width / 2 - fontSize.X / 2, Position.Y + texture.Height / 2 - fontSize.Y / 2), Color.Black);
            }

            spriteBatch.DrawString(Game1.font, text, new Vector2(Position.X + texture.Width / 2 - 15, Position.Y + texture.Height / 2 - 5), Color.Black);
        }
    }

    public enum TIleType
    {
        passable,
        unPassable,
    }
}
