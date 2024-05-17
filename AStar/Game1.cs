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
        private Tile target;
        private Tile start;
        public static MouseState currentState;
        public static MouseState prevState;
        private static KeyboardState keyCurrentState;
        private static KeyboardState keyPrevState;

        public static SpriteFont font;
        bool hasRunAStar;
        float timer;

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

            TileMap.CreateMap(new int[,]
            {
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 0, 1 },
                { 0, 0, 1, 1, 1, 0, 1, 0, 1 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            });
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            TileMap.tileTexture = CreateTexture(80, 80, pixel => Color.White);
            TileMap.solidTileTexture = CreateTexture(80, 80, pixel => Color.Black);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            prevState = currentState;
            currentState = Mouse.GetState();

            keyPrevState = keyCurrentState;
            keyCurrentState = Keyboard.GetState();

            SetStart();
            SetEnd();
            RunAStar();

            if (start != null && target != null && hasRunAStar)
            {
                if (timer <= 0)
                {
                    AStar.Temp();
                    timer = 0.05f;
                }
                else
                {
                    timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            base.Update(gameTime);
        }

        public void RunAStar()
        {
            if (KeyHasBeenPressed(Keys.Space))
            {
                if (start != null && target != null)
                {
                    AStar.FindPath(TileMap.Map, target, start);
                    hasRunAStar = true;
                }
            }
        }

        public void SetStart()
        {
            if (HasBeenPressed(currentState.LeftButton, prevState.LeftButton))
            {
                if (start != null)
                {
                    start.ResetTile();
                }

                for (int i = 0; i < TileMap.tiles.Count; i++)
                {
                    if (TileMap.tiles[i].Bounds.Intersects(GetBounds(true)))
                    {
                        start = TileMap.tiles[i];
                        start.color = Color.Blue;
                    }
                }
                hasRunAStar = false;
            }
        }

        public void SetEnd()
        {
            if (HasBeenPressed(currentState.RightButton, prevState.RightButton))
            {
                if (target != null)
                {
                    target.ResetTile();
                }

                for (int i = 0; i < TileMap.tiles.Count; i++)
                {
                    if (TileMap.tiles[i].Bounds.Intersects(GetBounds(true)))
                    {
                        target = TileMap.tiles[i];
                        target.color = Color.Green;
                    }
                }
                hasRunAStar = false;
            }
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

        public static bool HasBeenPressed(ButtonState _currentState, ButtonState _prevState)
        {
            if (_currentState == ButtonState.Pressed && _prevState == ButtonState.Released)
                return true;

            return false;
        }
        public static bool KeyHasBeenPressed(Keys key)
        {
            return keyCurrentState.IsKeyDown(key) && !keyPrevState.IsKeyDown(key);
        }

        public static Rectangle GetBounds(bool useCurrentState)
        {
            if (useCurrentState)
                return new Rectangle(currentState.X, currentState.Y, 1, 1);
            else
                return new Rectangle(prevState.X, prevState.Y, 1, 1);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            TileMap.DrawMap(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}