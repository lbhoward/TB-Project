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
    enum Faction { R , G, B, Y };

    class Unit
    {
        #region Declarations
        //The number of blocks this unit may move per turn
        public int MoveLimit
        {
            get { return moveLimit; }
        }
        protected int moveLimit;
        //The overall health value of the unit
        public int HP
        {
            get { return healthPoints; }
        }
        protected int healthPoints;
        //The number of remaining units within this unit
        public int RemainingUnits
        {
            get { return remainingUnits; }
        }
        protected int remainingUnits;
        //The faction (or player) this unit belongs to
        public Faction Allegiance
        {
            get { return allegiance; }
        }
        protected Faction allegiance;
        //3D Matrix
        public Matrix World
        {
            get { return world; }
        }
        protected Matrix world;
        //Imported .FBX model
        public Model Model3D
        {
            get { return model; }
        }
        protected Model model;
        #endregion

        //Constructor
        public Unit(String setFaction, int setMoveLimit)
        {
            //To which faction (player) does this unit belong?
            allegiance = (Units.Faction)Enum.Parse(typeof(Units.Faction),setFaction); //Well that was a pain in the arse.
            healthPoints = 100;
            remainingUnits = healthPoints / 10;
            moveLimit = setMoveLimit;
        }

        //Code to Attack
    }
}
