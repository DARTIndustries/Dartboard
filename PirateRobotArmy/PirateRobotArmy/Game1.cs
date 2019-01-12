using GeometryLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameResources;
using MonoGameResources.Cameras;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShootInfo = PirateRobotArmyLib.ShootInfo;
namespace PirateRobotArmy
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const int PARTICLES_PER_DAMAGE = 15;

        const float DAMAGE_PER_PARTICLE = 1f / PARTICLES_PER_DAMAGE;

        const int Shoot_Fade_Duration = 25;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PirateRobotArmyLib.Game MainGame { get; }

        private Dictionary<PirateRobotArmyLib.Team, TeamRenderInfo> TeamRenderInfo;
        private Dictionary<PirateRobotArmyLib.Bot, BotRenderInfo> BotRenderInfo;

        private List<BotRenderInfo> DeadBots { get; }

        private List<Tuple<ShootInfo, int>> Shots;

        public ParticleManager GlobalParticles { get; }


        private Texture2D PIX;

        ImmediateCamera mainCamera;

        KeyboardStateController KeyControl = new KeyboardStateController();

        bool paused = false;

        public Game1(PirateRobotArmyLib.Game toSimulate)
        {
            MainGame = toSimulate;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            mainCamera = new ImmediateCamera(graphics)
            {
                BlendState = BlendState.NonPremultiplied,
                SortMode = SpriteSortMode.Immediate
            };
            mainCamera.X -= mainCamera.OriginalWidth / 2;
            mainCamera.Y -= mainCamera.OriginalHeight / 2;
            mainCamera.ZoomCenter = (float)Math.Min(mainCamera.OriginalWidth / (toSimulate.Constants.ArenaRadius * 2), mainCamera.OriginalHeight / (toSimulate.Constants.ArenaRadius * 2));
            Shots = new List<Tuple<ShootInfo, int>>();
            GlobalParticles = new ParticleManager();
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
            ImageLoader.Load(Content, GraphicsDevice);

            PIX = new Texture2D(GraphicsDevice, 1, 1);
            PIX.SetData(new Color[] { Color.White });


            TeamRenderInfo = new Dictionary<PirateRobotArmyLib.Team, TeamRenderInfo>();
            BotRenderInfo = new Dictionary<PirateRobotArmyLib.Bot, BotRenderInfo>();

            foreach (var team in MainGame.Teams)
            {
                TeamRenderInfo.Add(team, new TeamRenderInfo(team.Ai.GetType().GetCustomAttribute<PirateRobotArmyLib.TeamInfoAttribute>(), Content));

                foreach (var bot in team.BotAi.Keys)
                {
                    BotRenderInfo.Add(bot, new BotRenderInfo(bot));
                }
            }
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

            KeyControl.Update();
            if (KeyControl.IsKeyPressed(Keys.Space))
                paused = !paused;


            if (!paused || KeyControl.IsKeyPressed(Keys.OemPeriod))
            {
                MainGame.Tick();

                foreach (var bre in BotRenderInfo.Values)
                    bre.Update();

                foreach (var shot in MainGame.ShotsLastTick)
                {
                    Shots.Add(new Tuple<ShootInfo, int>(shot, Shoot_Fade_Duration));
                    if (shot.HitTarget != null)
                    {
                        if (shot.HpDamage == 0)
                        {
                            // Hit shield - Small particle effect
                            var particleBaseAngle = new Angle(shot.Trajectory.Start, shot.Trajectory.End);
                            // Hit HP - Big particle effect
                            for (double p = 0; p < shot.HpDamage; p += DAMAGE_PER_PARTICLE * 3)
                            {
                                var particleAngle = particleBaseAngle + GlobalRandom.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                                var velocity = particleAngle.CreateVector(GlobalRandom.NextFloat(0.1f, 0.4f));

                                GlobalParticles.AddParticle(new Particle(
                                    Particle.Combine(Particle.FrictionUpdate(.97f), Particle.Fade(10)),
                                    ImageLoader.SinglePixel,
                                    shot.Trajectory.End,
                                    velocity,
                                    GlobalRandom.Next(200, 260),
                                    0.1f,
                                    Color.Blue));
                            }
                        }
                        else
                        {
                            var particleBaseAngle = new Angle(shot.Trajectory.End, shot.Trajectory.Start);
                            // Hit HP - Big particle effect
                            for (double p = 0; p < shot.HpDamage; p += DAMAGE_PER_PARTICLE)
                            {
                                var particleAngle = particleBaseAngle + GlobalRandom.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                                var velocity = particleAngle.CreateVector(GlobalRandom.NextFloat(0.2f, 0.6f));

                                GlobalParticles.AddParticle(new Particle(
                                    Particle.Combine(Particle.FrictionUpdate(.97f), Particle.Fade(10)),
                                    ImageLoader.SinglePixel,
                                    shot.Trajectory.End,
                                    velocity,
                                    GlobalRandom.Next(200, 260),
                                    GlobalRandom.NextFloat(0.05f, 0.15f),
                                    Color.Brown));
                            }
                        }
                    }
                }

                for (int i = 0; i < Shots.Count; i++)
                {
                    if (Shots[i].Item2 == 0)
                    {
                        Shots.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        Shots[i] = new Tuple<ShootInfo, int>(Shots[i].Item1, Shots[i].Item2 - 1);
                    }
                }

                double leftBound = 0;
                double rightBound = 0;
                double topBound = 0;
                double bottomBound = 0;
                foreach (var bot in MainGame.Teams.SelectMany(x => x.BotAi.Keys))
                {
                    leftBound = Math.Min(bot.Position.X - 3, leftBound);
                    topBound = Math.Min(bot.Position.Y - 3, topBound);
                    rightBound = Math.Max(bot.Position.X + 3, rightBound);
                    bottomBound = Math.Max(bot.Position.Y + 3, bottomBound);
                }

                var distantX = Math.Max(Math.Abs(leftBound), Math.Abs(rightBound));
                var distantY = Math.Max(Math.Abs(topBound), Math.Abs(bottomBound));

                var xZoom = (mainCamera.OriginalWidth/2) / distantX;
                var yZoom = (mainCamera.OriginalHeight / 2) / distantY;

                var targetZoom = Math.Min(xZoom, yZoom);
                mainCamera.ZoomCenter = MathHelper.Lerp((float)mainCamera.ZoomCenter, (float)targetZoom, 0.1f);
                //mainCamera.X = -mainCamera.FullBounds.Value.Width / 2;
                //mainCamera.Y = -mainCamera.FullBounds.Value.Height / 2;

                GlobalParticles.Tick();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            mainCamera.StartRender(spriteBatch, GraphicsDevice);

            spriteBatch.Draw(ImageLoader.ArenaRing, 
                new Vector2(0, 0), 
                null, 
                Color.White, 
                0, 
                new Vector2(ImageLoader.ArenaRing.Width / 2, ImageLoader.ArenaRing.Height / 2),
                2* (float)MainGame.CurrentArenaRadius / ImageLoader.ArenaRing.Width, 
                SpriteEffects.None, 
                0);

            spriteBatch.Draw(ImageLoader.ArenaCenter,
                new Vector2(0, 0),
                null,
                Color.White,
                0,
                new Vector2(ImageLoader.ArenaCenter.Width / 2, ImageLoader.ArenaCenter.Height / 2),
                2 * 4f / ImageLoader.ArenaCenter.Width,
                SpriteEffects.None,
                0);

            foreach (var shot in Shots)
            {
                RenderHelper.DrawLine(spriteBatch, shot.Item1.Trajectory.Start, shot.Item1.Trajectory.End, .06, PIX, Color.White * (shot.Item2 / (float)Shoot_Fade_Duration), 0);
            }


            foreach (var kvp in BotRenderInfo)
                kvp.Value.Draw(spriteBatch, TeamRenderInfo[MainGame.Teams[kvp.Key.TeamNumber]]);

            foreach (var kvp in BotRenderInfo)
                kvp.Value.OverDraw(spriteBatch, TeamRenderInfo[MainGame.Teams[kvp.Key.TeamNumber]]);


            GlobalParticles.Draw(spriteBatch);

            mainCamera.StopRender(spriteBatch, GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}
