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

        // Nodes for pathfinding
        public Node[,] Nodes
        {
            get { return nodes; }
        }
        private Node[,] nodes;
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

        private bool done;
        private Point nextClosed;
        private List<Point> finalPath;


        private TimeSpan cursorMoveTime;
        private TimeSpan previousCursorMoveTime;
        #endregion

        #region Initiliasation Code
        //TerrainMap Constructor
        public TerrainMap(String levelLoadName, String unitLoadName, ContentManager nContent)
        {
            BuildMap(levelLoadName, nContent);
            PopulateMap(unitLoadName, nContent);
            cursor = new Cursor(mapSize, nContent);

            nodes = new Node[mapSize,mapSize];
            for (int nodesX = 0; nodesX < mapSize; ++nodesX)
                for (int nodesY = 0; nodesY < mapSize; ++nodesY)
                    nodes[nodesX, nodesY] = new Node(false, 10000f, new Point(-1, -1), false, new Point(nodesX, nodesY));

            cursorMoveTime = TimeSpan.FromMilliseconds(300f);
            finalPath = new List<Point>();
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

                        // Passable, normal block
                        case '0':
                            terrainBlocks[x, y] = new TerrainBlock(new Vector3(1*x,0,1*y), "Models\\Terrain\\Terrain", content, 0);
                            break;
                        
                        // Impassable block
                        case'1':
                            terrainBlocks[x, y] = new TerrainBlock(new Vector3(1 * x, 0, 1 * y), "Models\\Terrain\\Terrain", content, 1);
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
            if (units[desiredPosition.X, desiredPosition.Y] == null)
                return false;

            if (units[desiredPosition.X, desiredPosition.Y].Allegiance != units[myUnitPosition.X, myUnitPosition.Y].Allegiance)
                return false;

            // Checks that we're looking within the confines of the map size.
            if (desiredPosition.X < 0) return false;
            if (desiredPosition.X >= mapSize) return false;
            if (desiredPosition.Y < 0) return false;
            if (desiredPosition.Y >= mapSize) return false;

            return (terrainBlocks[desiredPosition.X, desiredPosition.Y].Passable == true);
        }

        public bool ValidPosition(Point desiredPosition)
        {
            // Checks that we're looking within the confines of the map size.
            if (desiredPosition.X < 0) return false;
            if (desiredPosition.X >= mapSize) return false;
            if (desiredPosition.Y < 0) return false;
            if (desiredPosition.Y >= mapSize) return false;

            return (terrainBlocks[desiredPosition.X, desiredPosition.Y].Passable == true);
        }

        public void BuildPath(Point startPoint, Point destination)
        {
            // Reset all the data for the next path finding
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    nodes[i, j].Reset();

                    // If the player cannot access a square, mark it closed. There's no need to incorporate it into the path finding
                    if (!ValidPosition(new Point(i, j)))
                        nodes[i, j].SetClosed(true);
                }
            }

            nodes[startPoint.X, startPoint.Y].SetCost(0.0f); // The start point has the lowest cost to begin with.
            Node currentLowestCostNode = nodes[startPoint.X, startPoint.Y]; // set the current node as the bot position
            // While the destination is not in the closed list
            if (startPoint != destination)
            {
                while (!nodes[destination.X, destination.Y].Closed)
                {
                    // set the currentNode to a new node not on the grid with a high cost so we can find the lowest cost that does exist.
                    currentLowestCostNode = new Node(false, 100000f, new Point(1, 1), false, new Point(0, 0));

                    for (int i = 0; i < mapSize; i++)
                    {
                        for (int j = 0; j < mapSize; j++)
                        {
                            // if this node has a lower cost than the current lowest cost node AND it's *NOT* closed.
                            if ((nodes[i, j].Cost + nodes[i, j].Heuristic(destination)) < currentLowestCostNode.Cost && !nodes[i, j].Closed)
                            {
                                currentLowestCostNode = nodes[i, j]; // This will always be the start point on the first pass.
                            }
                        }
                    }

                    currentLowestCostNode.SetClosed(true);

                    // now we've found the node with the lowest cost, do some magic!
                    // Set the cost for all adjacent locations (+1 for straight. +1.4 for diagonal)
                    // if its new cost is lower than its current cost AND it's a valid location to go to.
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            Point newLoc = new Point(currentLowestCostNode.GridPosition.X + x, currentLowestCostNode.GridPosition.Y + y);

                            // Make sure we're looking within the bounds of the room
                            if ((newLoc.X >= 0 && newLoc.X < mapSize) && (newLoc.Y >= 0 && newLoc.Y < mapSize))
                            {
                                // Adjacent
                                if ((x != 0 && y == 0) || (x == 0 && y != 0))
                                {
                                    float newCost = currentLowestCostNode.Cost + 1.0f;
                                    if ((newCost < nodes[newLoc.X, newLoc.Y].Cost)
                                           && ValidPosition(newLoc))
                                    {
                                        nodes[newLoc.X, newLoc.Y].SetCost(newCost);
                                        nodes[newLoc.X, newLoc.Y].SetLink(currentLowestCostNode.GridPosition);
                                    }
                                }
                                else
                                    // Diagonal
                                    if (x != 0 && y != 0)
                                    {
                                        float newCost = currentLowestCostNode.Cost + 1.4f;
                                        if ((newCost < nodes[newLoc.X, newLoc.Y].Cost)
                                               && ValidPosition(newLoc))
                                        {
                                            nodes[newLoc.X, newLoc.Y].SetCost(newCost);
                                            nodes[newLoc.X, newLoc.Y].SetLink(currentLowestCostNode.GridPosition);
                                        }
                                    }
                            }
                        }

                    }
                }// end while loop

                // Create the final path
                done = false;
                finalPath.Clear();
                nextClosed = destination;
                while (!done)
                {
                    nodes[nextClosed.X, nextClosed.Y].SetInPath(true);
                    finalPath.Add(nodes[nextClosed.X, nextClosed.Y].GridPosition);
                    nextClosed = nodes[nextClosed.X, nextClosed.Y].Link;
                    if (nextClosed == startPoint)
                    {
                        nodes[nextClosed.X, nextClosed.Y].SetInPath(true);
                        finalPath.Add(nodes[nextClosed.X, nextClosed.Y].GridPosition);
                        done = true;
                    }
                }
            }
        }        
        #endregion

        #region Update Code
        public void Update(GameTime gameTime)
        {
            cursor.Update(gameTime);
            if (gameTime.TotalGameTime - previousCursorMoveTime > cursorMoveTime)
            {
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (cursor.State == CursorState.FreeSelection)
                    {
                        if (units[(int)cursor.Position.X, (int)cursor.Position.Y] != null)
                        {
                            cursor.SetUnitSelected(new Point((int)cursor.Position.X, (int)cursor.Position.Y));
                            previousCursorMoveTime = gameTime.TotalGameTime;
                            cursor.SetState(CursorState.FriendlySelected);
                        }
                    }
                    else if (cursor.State == CursorState.FriendlySelected)
                    {
                        BuildPath(cursor.UnitSelected, new Point((int)cursor.Position.X, (int)cursor.Position.Y));
                        cursor.SetState(CursorState.FreeSelection);
                        previousCursorMoveTime = gameTime.TotalGameTime;
                    }
                }
            }

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
