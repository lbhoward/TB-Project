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
    enum CursorState
    {
        FreeSelection, FriendlySelected, 
    }

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

        public CursorState State { get { return state; } } public void SetState(CursorState newState) { state = newState; }
        private CursorState state;

        // Delay timer for cursor movement
        private int mapSize;
        private TimeSpan cursorMoveTime;
        private TimeSpan previousCursorMoveTime;
        private Point unitSelected;
        public Point UnitSelected { get { return unitSelected; } } public void SetUnitSelected(Point selection) { unitSelected = selection; }
        #endregion

        #region Initilisation Code
        // Constructor
        public Cursor(int setMapSize, ContentManager nContent)
        {
            mapSize = setMapSize;
            model = nContent.Load<Model>("Models\\Player\\Cursor");
            world = Matrix.Identity;
            world.Translation = new Vector3(0, 0, 0);
            cursorMoveTime = TimeSpan.FromMilliseconds(100f);
            state = CursorState.FreeSelection;
        }
        #endregion

        #region Update Code
        public void Update(GameTime gameTime)
        {
            bool detectChange = false; //DEBUG: Test for change in X or Y to reduce Console.WriteLine
            if (gameTime.TotalGameTime - previousCursorMoveTime > cursorMoveTime)
            {
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp) || Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    --position.Y;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                    detectChange = true;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight) || Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    ++position.X;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                    detectChange = true;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown) || Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    ++position.Y;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                    detectChange = true;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft) || Keyboard.GetState().IsKeyDown(Keys.A))
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
