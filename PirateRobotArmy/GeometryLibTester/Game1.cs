using GeometryLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameResources;

namespace GeometryLibTester
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Texture2D Pixel;
        Texture2D CircleImage;

        Circle Circle;
        Line Line;

        Vector2? IntersectionA;
        Vector2? IntersectionB;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            Circle = new Circle(new Vector(50, 50, true), 16);
            Line = new Line(new Vector(0, 0, true), new Vector(10, 10, true));
            IntersectionA = IntersectionB = null;
            base.Initialize();
            IsMouseVisible = true;
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new Color[] { Color.White });

            CircleImage = Content.Load<Texture2D>("Circle_Outline");
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var mouseState = Mouse.GetState();

            Line.End = mouseState.Position;

            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                Line.Start = mouseState.Position;
            }

            Vector[] intersections = Circle.Intersections(Line);
            if(intersections.Length > 0)
            {
                IntersectionA = intersections[0];
            }
            else
            {
                IntersectionA = null;
            }
            if(intersections.Length > 1)
            {
                IntersectionB = intersections[1];
            }
            else
            {
                IntersectionB = null;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(CircleImage, (Vector2)Circle.Position, null, Color.White, 0, new Vector2(16,16), 1f, SpriteEffects.None, 0);
            RenderHelper.DrawLine(spriteBatch, Line.Start, Line.End, 1, Pixel, Color.Blue);

            if(IntersectionA.HasValue)
            {
                RenderHelper.DrawLine(spriteBatch, IntersectionA.Value - new Vector2(3, 3), IntersectionA.Value + new Vector2(3, 3), 1, Pixel, Color.Red);
                RenderHelper.DrawLine(spriteBatch, IntersectionA.Value - new Vector2(-3, 3), IntersectionA.Value + new Vector2(-3, 3), 1, Pixel, Color.Red);
            }

            if (IntersectionB.HasValue)
            {
                RenderHelper.DrawLine(spriteBatch, IntersectionB.Value - new Vector2(3, 3), IntersectionB.Value + new Vector2(3, 3), 1, Pixel, Color.Green);
                RenderHelper.DrawLine(spriteBatch, IntersectionB.Value - new Vector2(-3, 3), IntersectionB.Value + new Vector2(-3, 3), 1, Pixel, Color.Green);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
