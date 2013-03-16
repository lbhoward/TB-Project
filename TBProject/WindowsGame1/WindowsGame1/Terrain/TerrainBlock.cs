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

        //Constructor
        public TerrainBlock(Vector3 setPos, String modelPath, ContentManager content)
        {
            //Assign starting position
            position = setPos;
            //Translate model matrix to represent this
            world = Matrix.Identity;
            world.Translation = position;
            selected = false;

            //Load model
            model = content.Load<Model>(modelPath);
        }
    }
}
