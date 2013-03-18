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
        public TerrainBlock TerrainBlock(int i, int j)
        {
            return terrainBlocks[i,j];
        }
        private TerrainBlock[,] terrainBlocks;

        // Array for obstructions (water, trees, mountains etc)
        public int[,] Obstructions
        {
            get { return obstructions; }
        }
        private int[,] obstructions;
        //The terrainMap holds a bulk array of all units occupying it,
        //therefore some elements will be NULL (corresponding to TerrainBlock).
        //This allows cursor to remain in sync.
        public Units.Unit Unit(int i, int j)
        {
            return units[i, j];
        }
        private Units.Unit[,] units;
        //Size of the map, determined by TXT first line length (loaded later)
        public int MapSize
        {
            get { return mapSize; }
        }
        private int mapSize = 0;
        private Cursor cursor;
        #endregion

        #region Initiliasation Code
        //TerrainMap Constructor
        public TerrainMap(String levelLoadName, String unitLoadName, ContentManager nContent)
        {
            
            BuildMap(levelLoadName, nContent);
            PopulateMap(unitLoadName, nContent);
            cursor = new Cursor(mapSize, nContent);
        }

        //Read from TXT file and create TerrainBlock array
        private void BuildMap(string levelLoadName, ContentManager content)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(levelLoadName))
            {
                string line = reader.ReadLine();
                mapSize = line.Length; // 
                obstructions = new int[mapSize, mapSize];

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

                        // Passable, normal block
                        case '0':
                            terrainBlocks[x, y] = new TerrainBlock(new Vector3(1*x,0,1*y), "Models\\Terrain\\Terrain", content);
                            obstructions[x, y] = 0;
                            break;
                        
                        // Impassable block
                        case'1':
                            terrainBlocks[x, y] = new TerrainBlock(new Vector3(1 * x, 0, 1 * y), "Models\\Terrain\\Terrain", content);
                            obstructions[x, y] = 1;
                            break;
                    }
                }
            }
        }

        //Read from TXT to add units to the map
        private void PopulateMap(string levelLoadName, ContentManager content)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(levelLoadName))
            {
                string line = reader.ReadLine();

                units = new Units.Unit[mapSize, mapSize]; //Instatiate array to the corresponding size

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
                    switch (lines[y][(x*2)+1]) //Move in chunks of 2 (X Only) as the UNITS.TXT file is formated in 2 chars [FACTION,UNIT_TYPE]
                                               //+1 accesses UNIT_TYPE
                    {
                        //IMPORTANT: Model Loading Conventions: XNA does not accept the extension, only the asset name.
                        //                                      Do not include the 'Content\\'.
                        //                                      Do use \\ escape sequence.
                        case '1': //INFANTRY
                            String f = "";
                            units[x, y] = new Units.Infantry(f+lines[y][x*2],2, new Vector3(x*1, 0, y*1), content); //Cast the Char representing FACTION
                            break;
                    }
                }
            }
        }
        #endregion

        #region Help Functions
        /// <summary>
        /// Returns true if the desired block is passable
        /// Passable = Free of enemy units and is not obstructed by terrain elements or off the edge of the map
        /// PS thanks, Patrick!
        /// </summary>
        /// <param name="myUnit">Friendly unit selected, used to determine player allegiance</param>
        /// <param name="position">Position on the map to check</param>
        /// <returns></returns>
        public bool ValidPosition(Point myUnitPosition, Point desiredPosition)
        {
            // If position contains an enemy unit, it is impassable.
            if (units[desiredPosition.X, desiredPosition.Y].Allegiance != units[myUnitPosition.X, myUnitPosition.Y].Allegiance)
                return false;

            // Checks that we're looking within the confines of the map size.
            if (desiredPosition.X < 0) return false;
            if (desiredPosition.X >= mapSize) return false;
            if (desiredPosition.Y < 0) return false;
            if (desiredPosition.Y >= mapSize) return false;

            return (obstructions[desiredPosition.X, desiredPosition.Y] == 0);
        }

        public bool ValidPosition(Point desiredPosition)
        {
            // Checks that we're looking within the confines of the map size.
            if (desiredPosition.X < 0) return false;
            if (desiredPosition.X >= mapSize) return false;
            if (desiredPosition.Y < 0) return false;
            if (desiredPosition.Y >= mapSize) return false;

            return (obstructions[desiredPosition.X, desiredPosition.Y] == 0);
        }
        #endregion

        #region Update Code
        public void Update(GameTime gameTime)
        {
            cursor.Update(gameTime);
        }
        #endregion

        #region Drawing Code
        public void Draw(Camera camera)
        {
            //Draw Terrain
            //David King drew issue with using int i, int j ETC. 'Variables should always have context'.
            //He also incoured the use of ++pre-increment, as it more effecient than post-increment++.
            for (int mapHeight = 0; mapHeight < MapSize; ++mapHeight)
            {
                for (int mapWidth = 0; mapWidth < MapSize; ++mapWidth)
                {
                    DrawModel(terrainBlocks[mapWidth, mapHeight].Model3D, terrainBlocks[mapWidth, mapHeight].World, camera);

                    if (units[mapWidth, mapHeight] != null)
                        DrawModel(units[mapWidth, mapHeight].Model3D, units[mapWidth,mapHeight].World, camera);

                    //Test to access Selected Unit
                    //if (units[(int)cursor.Position.X, (int)cursor.Position.Y] != null)
                    //  Console.WriteLine("Unit Selected - Faction: {0} - HP: {1}", units[(int)cursor.Position.X, (int)cursor.Position.Y].Allegiance.ToString(), units[(int)cursor.Position.X, (int)cursor.Position.Y].HP);
                }
            }

            //Draw Cursor
            cursor.Draw(camera);
        }
        //3D Model Draw Function
        //DrawModel(Model, MatrixBelongingToModel)
        private void DrawModel(Model model, Matrix world, Camera camera)
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
