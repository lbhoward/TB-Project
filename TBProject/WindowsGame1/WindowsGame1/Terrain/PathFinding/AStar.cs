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
        

        // The completed path for the unit
        public List<Point> FinalPath
        {
            get { return finalPath; }
        }
        private List<Point> finalPath;

        private Point nextClosed;
        private bool done;
        #endregion

        #region Intialising
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nMap">Pass the current map</param>
        public AStar(Terrain.TerrainMap map)
        {
            nodes = new Node[map.MapSize, map.MapSize];

            for (int nodesX = 0; nodesX < map.MapSize; ++nodesX)
                for (int nodesY = 0; nodesY < map.MapSize; ++nodesY)
                    nodes[nodesX, nodesY] = new Node(new Point(nodesX, nodesY));

            finalPath = new List<Point>();
        }

        #endregion

        #region Build Path
        /// <summary>
        /// Builds the path for the unit to follow and stores it in finalPath
        /// </summary>
        /// <param name="startPoint">Points use ints only so no need to convert from float with Vector2s</param>
        /// <param name="destination"></param>
        

        #endregion
    }
}
