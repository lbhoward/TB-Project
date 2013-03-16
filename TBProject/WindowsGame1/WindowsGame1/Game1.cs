using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TBProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        //Levels
        Terrain.TerrainMap testMap;

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
            //Initialise the camera
            //new Camera(Position(GLOBAL),LookAt(GLOBAL),AspectRatio(W/H));
            //IMPORTANT: Y+/-: Up/Down
            //           Z+/-: Forward/Backward
            camera = new Camera(new Vector3(0,10,10), Vector3.Forward,
                (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height);

            //Levels
            testMap = new Terrain.TerrainMap("Content/Levels/Campaign/One/1.txt", "Content/Levels/Campaign/One/1-Units.txt", Content);

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
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //DEBUG CAMERA
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                camera.Position = new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z + 0.1f);
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                camera.Position = new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z - 0.1f);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                camera.Position = new Vector3(camera.Position.X - 0.1f, camera.Position.Y, camera.Position.Z);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                camera.Position = new Vector3(camera.Position.X + 0.1f, camera.Position.Y, camera.Position.Z);
            camera.Update();

            testMap.Update(gameTime);

            base.Update(gameTime);
        }

        //3D Model Draw Function
        //DrawModel(Model, MatrixBelongingToModel)
        private void DrawModel(Model model, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Call the currently loaded TerrainMaps draw function
            testMap.Draw(camera);
            
            base.Draw(gameTime);
        }
    }
}
