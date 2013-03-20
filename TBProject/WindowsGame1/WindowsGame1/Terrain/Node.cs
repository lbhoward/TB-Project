using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TBProject
{
    class Node
    {
        #region Declarations
        // Grid position
        // Point instead of Vector2, Points use ints so we don't need to convert from float
        public Point GridPosition 
        { 
            get { return gridPosition; } 
        }
        private Point gridPosition;

        // If closed, units cannot pass this position
        public bool Closed 
        { 
            get { return closed; } 
        }
        private bool closed;

        // Cost (Not including heuristic value
        public float Cost 
        { 
            get { return cost; } 
        }
        private float cost;

        // The next link in the path
        public Point Link 
        { 
            get { return link; } 
        }
        private Point link;

        // Is this node in the path? Used mainly for debug really
        public bool InPath 
        { 
            get { return inPath; } 
        }
        private bool inPath; 
        #endregion

        // Node constructor
        public Node(bool nClosed, float nCost, Point nLink, bool nInPath, Point nGridPosition)
        {
            closed = nClosed;
            cost = nCost;
            link = nLink;
            inPath = nInPath;
            gridPosition = nGridPosition;
        }

        public void SetCost(float nCost)
        {
            cost = nCost;
        }

        public void SetInPath(bool state)
        {
            inPath = state;
        }

        public void SetClosed(bool state)
        {
            closed = state;
        }

        public void SetLink(Point nLink)
        {
            link = nLink;
        }

        public void Reset()
        {
            closed = false;
            cost = 10000;
            inPath = false;
            link = new Point(-1, -1);
        }

        public float Heuristic(Point destination)
        {
            return Math.Abs(destination.X - gridPosition.X) + Math.Abs(destination.Y - gridPosition.Y);
        }
    }
}
