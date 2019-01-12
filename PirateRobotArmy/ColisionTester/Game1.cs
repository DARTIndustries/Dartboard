using GeometryLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameResources;
using System;
using System.Collections.Generic;

namespace ColisionTester
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Circle> Circles;
        Line line;

        Texture2D SinglePixel;
        Texture2D CircleOutline;

        List<Vector> HitLocations;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Random r = new Random();
            Circles = new List<Circle>()
            {
                new Circle(new Vector(100,100,true), 32),
                new Circle(new Vector(300,300,true), 32),
                new Circle(new Vector(150,300,true), 32),
                new Circle(new Vector(150,150,true), 32),
            };

            for(int i = 0; i < 30; i++)
            {
                Circles.Add(new Circle(new Vector(r.Next(800), r.Next(600), true), 16));
            }

            line = new Line(new Vector(0, 0, true), new Vector(0, 0, true));
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SinglePixel = Content.Load<Texture2D>("Spix");
            CircleOutline = Content.Load<Texture2D>("Circle_Outline");
            HitLocations = new List<Vector>();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var state = Mouse.GetState();
            if(state.LeftButton == ButtonState.Pressed)
            {
                line.Start = state.Position;
            }
            if(state.RightButton == ButtonState.Pressed)
            {
                line.End = state.Position;
            }


            HitLocations.Clear();
            var tmpLine = line.Clone();
            foreach(var circle in Circles)
            {
                var hit = circle.FirstIntersection(tmpLine);
                if(hit.HasValue)
                {
                    HitLocations.Add(hit.Value);
                    tmpLine.End = hit.Value;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            foreach(var circle in Circles)
            {
                spriteBatch.Draw(CircleOutline, circle.Position, null, Color.White, 0, new Vector2(16, 16), (float)circle.Radius/16f, SpriteEffects.None, 0);
            }

            RenderHelper.DrawLine(spriteBatch, line.Start, line.End, 1, SinglePixel, Color.Blue);
            RenderHelper.DrawLine(spriteBatch, line.Start + Vector.UnitX * 4, line.Start - Vector.UnitX * 4, 1, SinglePixel, Color.Red);
            RenderHelper.DrawLine(spriteBatch, line.Start + Vector.UnitY * 4, line.Start - Vector.UnitY * 4, 1, SinglePixel, Color.Red);
            RenderHelper.DrawLine(spriteBatch, line.End + Vector.UnitX * 4, line.End - Vector.UnitX * 4, 1, SinglePixel, Color.Green);
            RenderHelper.DrawLine(spriteBatch, line.End + Vector.UnitY * 4, line.End - Vector.UnitY * 4, 1, SinglePixel, Color.Green);


            foreach(var hit in HitLocations)
            {
                RenderHelper.DrawLine(spriteBatch, hit - new Vector(3, 3, true), hit + new Vector(3, 3, true), 1, SinglePixel, Color.Orange);
                RenderHelper.DrawLine(spriteBatch, hit - new Vector(-3, 3, true), hit + new Vector(-3, 3, true), 1, SinglePixel, Color.Orange);
            }

            if(HitLocations.Count > 0)
            {
                var last = HitLocations[HitLocations.Count - 1];
                RenderHelper.DrawLine(spriteBatch, last - new Vector(4, 4, true), last + new Vector(4, 4, true), 2, SinglePixel, Color.Red);
                RenderHelper.DrawLine(spriteBatch, last - new Vector(-4, 4, true), last + new Vector(-4, 4, true), 2, SinglePixel, Color.Red);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
