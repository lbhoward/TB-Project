using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TBProject.Terrain
{
    enum BlockType
    {
        None, Water, Mountain, Crater
    }

    class TerrainBlock
    {
        //Players Global Position in 3D space
        public Vector3 Position
        {
            get { return position; }
        }
        private Vector3 position;

        //Players Global Translation and Rotation expressed as a Matrix
        //This Matrix is passed to the Draw3D Function in Main for rendering
        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        //Imported .FBX model
        public Model Model3D
        {
            get { return model; }
        }
        private Model model;

        // Is the block selected?
        public bool Selected
        {
            get { return selected; } 
        }
        private bool selected;
        public void SetSelected(bool state) { selected = state; }

        // Returns true if block is not obstructed by terrain elements
        // Use this instead of if (water || mountains || hills) etc when determining if it is passable
        public bool Passable
        {
            get { return passable; }
        }
        private bool passable;

        // The type of terrain
        public BlockType BlockType
        {
            get { return blockType; }
        }
        private BlockType blockType;
        // Might want to change terrain in future, for example add craters from explosions
        public void SetBlockType(BlockType type, bool nPassable) { blockType = type; passable = nPassable; } 

        //Constructor
        public TerrainBlock(Vector3 setPos, String modelPath, ContentManager content, int type)
        {
            //Assign starting position
            position = setPos;
            //Translate model matrix to represent this
            world = Matrix.Identity;
            world.Translation = position;
            selected = false;

            //Load model
            model = content.Load<Model>(modelPath);

            switch (type)
            {
                case 0:
                    blockType = Terrain.BlockType.None;
                    passable = true;
                    break;
            }
        }
    }
}
