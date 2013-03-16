using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace TBProject
{
    class Cursor
    {
        #region Declarations
        // Position in 3D space
        public Vector2 Position
        {
            get { return position; }
        }
        private Vector2 position;
        // Position in grid coords
        public Vector2 GridPosition
        {
            get { return gridPosition; }
        }
        private Vector2 gridPosition;
        // 3D Model
        public Model Model3D
        {
            get { return model; }
        }
        private Model model;
        // 3D Matrix
        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        // Delay timer for cursor movement
        private int mapSize;
        private TimeSpan cursorMoveTime;
        private TimeSpan previousCursorMoveTime;
        #endregion

        #region Initilisation Code
        // Constructor
        public Cursor(int setMapSize, ContentManager nContent)
        {
            mapSize = setMapSize;
            model = nContent.Load<Model>("Models\\Player\\Cursor");
            world = Matrix.Identity;
            world.Translation = new Vector3(0, 0, 0);
            cursorMoveTime = TimeSpan.FromMilliseconds(300f);
        }
        #endregion

        #region Update Code
        public void Update(GameTime gameTime)
        {
            bool detectChange = false; //DEBUG: Test for change in X or Y to reduce Console.WriteLine
            if (gameTime.TotalGameTime - previousCursorMoveTime > cursorMoveTime)
            {
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp))
                {
                    --position.Y;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                    detectChange = true;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                {
                    ++position.X;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                    detectChange = true;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))
                {
                    ++position.Y;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                    detectChange = true;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                {
                    --position.X;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                    detectChange = true;
                }
            }
            position.X = MathHelper.Clamp(position.X, 0, mapSize-1);
            position.Y = MathHelper.Clamp(position.Y, 0, mapSize-1);

            if (detectChange)
            {
                world = Matrix.Identity;
                world.Translation = new Vector3(1 * position.X, 0, 1 * position.Y);
                Console.WriteLine("X: {0} - Y: {1}", position.X, position.Y); //Y: 2D <> Z: 3D
            }
        }
        #endregion

        #region Drawing Code
        public void Draw(Camera camera)
        {
            DrawModel(camera);
        }

        //3D Model Draw Function
        //DrawModel(Model, MatrixBelongingToModel)
        private void DrawModel(Camera camera)
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
        #endregion
    }
}
