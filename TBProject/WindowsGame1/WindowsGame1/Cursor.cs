using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TBProject
{
    class Cursor
    {
        public Vector2 Position
        {
            get { return position; }
        }
        private Vector2 position;

        public Terrain.TerrainMap Map
        {
            get { return map; }
        }
        private Terrain.TerrainMap map;

        public Cursor(Terrain.TerrainMap nMap)
        {
            map = nMap;
        }

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp))
                ++position.Y;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                ++position.X;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))
                --position.Y;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                --position.X;

            position.X = MathHelper.Clamp(position.X, 0, map.MapSize);
            position.Y = MathHelper.Clamp(position.Y, 0, map.MapSize);

            Console.Write("X: " + position.X + "\t");
            Console.WriteLine("Y: " + position.Y);
        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
