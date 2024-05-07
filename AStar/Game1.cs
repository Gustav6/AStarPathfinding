using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AStar
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static SpriteFont font;
        private Texture2D tileTexture;
        private Tile[,] tileMap;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 1080,
                PreferredBackBufferWidth = 1920
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            tileMap = new Tile[20, 10];

            for (int x = 0; x < tileMap.GetLength(0); x++)
            {
                for (int y = 0; y < tileMap.GetLength(1); y++)
                {
                    tileMap[x, y] = new Tile(tileTexture, new Vector2(x * tileTexture.Width, y * tileTexture.Height), x, y);
                }
            }

            // Sets starting tile to 0, 0 and tries to find path to 1, 1
            Tile target = tileMap[2, 2];

            AStar.FindPath(tileMap, target, 0, 0);
            target.color = Color.Green;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            tileTexture = CreateTexture(64, 64, pixel => Color.White);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        private Texture2D CreateTexture(int width, int height, Func<int, Color> paint)
        {
            Texture2D texture = new(GraphicsDevice, width, height);

            Color[] colorArray = new Color[width * height];

            for (int pixel = 0; pixel < colorArray.Length; pixel++)
            {
                colorArray[pixel] = paint(pixel);
            }

            texture.SetData(colorArray);

            return texture;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for (int x = 0; x < tileMap.GetLength(0); x++)
            {
                for (int y = 0; y < tileMap.GetLength(1); y++)
                {
                    tileMap[x, y].Draw(_spriteBatch);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}