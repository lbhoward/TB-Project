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

namespace TBProject.Units
{
    class Infantry : Unit
    {
        public Infantry(String setFaction, int setMoveList, Vector3 setPosition, ContentManager nContent): base(setFaction,setMoveList)
        {
            model = nContent.Load<Model>("Models\\Units\\Infantry\\Infantry");
            world = Matrix.Identity;
            world.Translation = setPosition;
        }
    }
}
