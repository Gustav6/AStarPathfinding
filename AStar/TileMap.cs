using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class TileMap
    {
        public static Tile[,] Map { get; private set; }
        public static List<Tile> tiles = new();
        public static Texture2D tileTexture;
        public static Texture2D solidTileTexture;

        public static void CreateMap(int[,] map)
        {
            Map = new Tile[map.GetLength(0), map.GetLength(1)];

            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Texture2D texture;
                    TIleType type;

                    if (map[x, y] == 1)
                    {
                        texture = solidTileTexture;
                        type = TIleType.unPassable;
                    }
                    else
                    {
                        texture = tileTexture;
                        type = TIleType.passable;
                    }

                    Map[x, y] = new Tile(texture, new Vector2(x * tileTexture.Width + x * 2, y * tileTexture.Height + y * 2), x, y, type);

                    tiles.Add(Map[x, y]);
                }
            }
        }

        public static void DrawMap(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y].Draw(spriteBatch);
                }
            }
        }
    }
}
