using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TBProject.Terrain
{
    class TerrainMap
    {
        #region Declarations
        //Access Function to call a single Block within the array that
        //makes up the entire terrain map.
        public TerrainBlock TerrainBlocks(int i, int j)
        {
            return terrainBlocks[i,j];
        }
        private TerrainBlock[,] terrainBlocks;

        //Size of the map, determined by TXT first line length (loaded later)
        public int MapSize
        {
            get { return mapSize; }
        }
        private int mapSize = 0;

        //// The currently selected block
        //public TerrainBlock CurrentlySelected
        //{
        //    get { return currentlySelected; }
        //}
        //private TerrainBlock currentlySelected;

        //// The previously selected block
        //public TerrainBlock PreviouslySelected
        //{
        //    get { return previouslySelected; }
        //}
        //private TerrainBlock previouslySelected;

        //// The block which is currently highlighed by the cursor
        //public TerrainBlock CurrentlyHighlighted 
        //{ 
        //    get { return currentlySelected; } 
        //}
        //private TerrainBlock currentlyHighlighted;

        private Vector2 currentlyHighlighted; public Vector2 CurrentlyHighlighted { get { return currentlyHighlighted; } }
        #endregion

        //TerrainMap Constructor
        public TerrainMap(String levelLoadName, ContentManager content)
        {
            BuildMap(levelLoadName, content);
            currentlyHighlighted = new Vector2(0, 0);
        }

        //Read from TXT file and create TerrainBlock array
        private void BuildMap(string levelLoadName, ContentManager content)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(levelLoadName))
            {
                string line = reader.ReadLine();
                mapSize = line.Length; // 

                terrainBlocks = new TerrainBlock[mapSize, mapSize]; //Instatiate array to the corresponding size

                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }

            for (int y = 0; y < mapSize; ++y)
            {
                for (int x = 0; x < mapSize; ++x)
                {
                    switch (lines[y][x])
                    {
                        //IMPORTANT: Model Loading Conventions: XNA does not accept the extension, only the asset name.
                        //                                      Do not include the 'Content\\'.
                        //                                      Do use \\ escape sequence.
                        case '0':
                            terrainBlocks[x, y] = new TerrainBlock(new Vector3(1*x,0,1*y), "Models\\Terrain\\Grass_Block", content);
                            break;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp))
                ++currentlyHighlighted.Y;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                ++currentlyHighlighted.X;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))
                --currentlyHighlighted.Y;
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                --currentlyHighlighted.X;

            currentlyHighlighted.X = MathHelper.Clamp(currentlyHighlighted.X, 0, mapSize);
            currentlyHighlighted.Y = MathHelper.Clamp(currentlyHighlighted.Y, 0, mapSize);

            Console.Write("X: " + currentlyHighlighted.X + "\t");
            Console.WriteLine("Y: " + currentlyHighlighted.Y);
        }
    }
}
