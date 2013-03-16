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

        public Terrain.TerrainMap Map
        {
            get { return map; }
        }
        private Terrain.TerrainMap map;

        public Model Model3D
        {
            get { return model; }
        }
        private Model model;

        // Delay timer for cursor movement
        private TimeSpan cursorMoveTime;
        private TimeSpan previousCursorMoveTime;
        private ContentManager content;

        // Constructor
        public Cursor(Terrain.TerrainMap nMap, ContentManager nContent)
        {
            map = nMap;
            content = nContent;

            model = content.Load<Model>("Models\\Player\\Cursor");
            cursorMoveTime = TimeSpan.FromMilliseconds(300f);
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousCursorMoveTime > cursorMoveTime)
            {
                position.X = MathHelper.Clamp(position.X, 0, map.MapSize);
                position.Y = MathHelper.Clamp(position.Y, 0, map.MapSize);
                
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp))
                {
                    ++position.Y;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                {
                    ++position.X;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))
                {
                    --position.Y;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                {
                    --position.X;
                    previousCursorMoveTime = gameTime.TotalGameTime;
                }

                Console.Write("X: " + position.X + "\t");
                Console.WriteLine("Y: " + position.Y);
            }
        }

        public void Draw(GameTime gameTime)
        {
            /* TO DO
             * ADD DRAWING STUFFS HERE
             */
        }
    }
}
