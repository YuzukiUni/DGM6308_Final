﻿using System;

namespace ZooManager
{
    public class Insect : Animal
    {

        public Insect(string name)
        {
            emoji = "🐞";
            species = "insect";
            this.name = name;
            reactionTime = new Random().Next(1, 4); // reaction time of 1 (fast) to 4
        }

        public override void Activate()
        {
            base.Activate();
            if (encounterBoulder()) Game.Die(this, location.x, location.y);
            else if (Pass()) 
            Console.WriteLine("I am an insect. I can pass through grass.");
            else if (!Hunt()) Flee();
        }


        // Insect meets boulder will dead by collapse
        public bool encounterBoulder()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "boulder", "insect") ||
                Game.Seek(location.x, location.y, Direction.down, "boulder", "insect") ||
                Game.Seek(location.x, location.y, Direction.left, "boulder", "insect") ||
                Game.Seek(location.x, location.y, Direction.right, "boulder", "insect"))
            {
                return true;
            }
            return false;
        }

        // Insect can pass the grass
        public bool Pass()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "grass", "insect") ||
                Game.Seek(location.x, location.y, Direction.down, "grass", "insect") ||
                Game.Seek(location.x, location.y, Direction.left, "grass", "insect") ||
                Game.Seek(location.x, location.y, Direction.right, "grass", "insect"))
            {
                return true;
            }
            return false;
        }

        // Follow Game.cs seek and retreat

        public bool Flee()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "snake", "insect"))
            {
                if (Game.Retreat(this, Direction.down)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.down, "snake", "insect"))
            {
                if (Game.Retreat(this, Direction.up)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.left, "snake", "insect"))
            {
                if (Game.Retreat(this, Direction.right)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.right, "snake", "insect"))
            {
                if (Game.Retreat(this, Direction.left)) return true;
            }
            return false;
        }


        public bool Hunt()
        {
            // Check one square in each direction
            if (Game.Seek(location.x, location.y, Direction.up, "cat", "insect", 1))
            {
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "cat", "insect", 1))
            {
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "cat", "insect", 1))
            {
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "cat", "insect", 1))
            {
                Game.Attack(this, Direction.right);
                return true;
            }
            // Check two squares in each direction
            else if (Game.Seek(location.x, location.y, Direction.up, "cat", "insect", 2))
            {
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "cat", "insect", 2))
            {
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "cat", "insect", 2))
            {
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "cat", "insect", 2))
            {
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }



        // Spawning Insect if mouse killed by cat
        public static Insect spawnInsect()
        {
            string name = "Vomm";
            return new Insect(name);
        }
    }
}
