using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TBProject.PathFinding
{
    class AStar
    {
        #region Declarations
        // Array of nodes
        public Node[,] Nodes
        {
            get { return nodes; }
        }
        private Node[,] nodes;

        // The current map to perform path finding on
        public Terrain.TerrainMap Map
        {
            get { return map; }
        }
        private Terrain.TerrainMap map;

        // The completed path for the unit
        public List<Point> FinalPath
        {
            get { return finalPath; }
        }
        private List<Point> finalPath;

        private Point nextClosed;
        private bool done;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nMap">Pass the current map</param>
        public AStar(Terrain.TerrainMap nMap)
        {
            map = nMap;
            nodes = new Node[map.MapSize, map.MapSize];

            for (int nodesX = 0; nodesX < map.MapSize; ++nodesX)
                for (int nodesY = 0; nodesY < map.MapSize; ++nodesY)
                    nodes[nodesX, nodesY] = new Node(new Point(nodesX, nodesY));

            finalPath = new List<Point>();
        }

        /// <summary>
        /// Builds the path for the unit to follow and stores it in finalPath
        /// </summary>
        /// <param name="startPoint">Points use ints only so no need to convert from float with Vector2s</param>
        /// <param name="destination"></param>
        public void Build(Point startPoint, Point destination)
        {
            // Reset all nodes to default values before finding the path
            for (int nodesX = 0; nodesX < map.MapSize; ++nodesX)
            {
                for (int nodesY = 0; nodesY < map.MapSize; ++nodesY)
                {
                    nodes[nodesX, nodesY].Reset();

                    // Close nodes that are impassable
                    if (!map.ValidPosition(startPoint, new Point(nodesX, nodesY)))
                        nodes[nodesX, nodesY].SetClosed(true);
                }
            }

            // Start point has lowest cost to begin with
            nodes[startPoint.X, startPoint.Y].SetCost(0f);
            Node currentLowestCostNode = nodes[startPoint.X, startPoint.Y];

            // No need to build a path if we're already at the destination.
            if (startPoint != destination)
            {
                // Whilst the destination is not in the closed list of nodes, find a path to it.
                while (!nodes[destination.X, destination.Y].Closed)
                {
                    currentLowestCostNode.SetCost(100000f);
                    currentLowestCostNode.SetLink(new Point(1, 1));

                    // if this node has a lower cost than the current lowest cost AND it's *NOT* closed.
                    for (int i = 0; i < map.MapSize; i++)
                        for (int j = 0; j < map.MapSize; j++)
                            if ((nodes[i, j].Cost + nodes[i, j].Heuristic(destination)) < currentLowestCostNode.Cost && !nodes[i, j].Closed)
                                currentLowestCostNode = nodes[i, j];

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
                            // THIS CAN BE IMPROVED WITH YOUR FANCY NAND LOGIC THINGY YOU WERE TALKING ABOUT THAT ONE TIME...
                            if ((newLoc.X >= 0 && newLoc.X < map.MapSize) && (newLoc.Y >= 0 && newLoc.Y < map.MapSize))
                            {
                                // Adjacent
                                if ((x != 0 && y == 0) || (x == 0 && y != 0))
                                {
                                    float newCost = currentLowestCostNode.Cost + 1.0f;
                                    if ((newCost < nodes[newLoc.X, newLoc.Y].Cost)
                                           && map.ValidPosition(newLoc))
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
                                               && map.ValidPosition(newLoc))
                                        {
                                            nodes[newLoc.X, newLoc.Y].SetCost(newCost);
                                            nodes[newLoc.X, newLoc.Y].SetLink(currentLowestCostNode.GridPosition);
                                        }
                                    }

                            }
                        }
                    }
                } // End of While Loop

                // Now create the finalPath
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
    }
}
