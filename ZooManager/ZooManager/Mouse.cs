﻿using System;
namespace ZooManager
{
    public class Mouse : Animal
    {
        public Mouse(string name)
        {
            emoji = "🐭";
            species = "mouse";
            this.name = name; // "this" to clarify instance vs. method parameter
            reactionTime = new Random().Next(1, 4); // reaction time of 1 (fast) to 3
            /* Note that Mouse reactionTime range is smaller than Cat reactionTime,
             * so mice are more likely to react to their surroundings faster than cats!
             Mouse*/
        }

        public override void Activate()
        {
            base.Activate();
            Console.WriteLine("I am a mouse. Squeak.");
            if (!Hunt()) Flee();
        }

        /* Updated to work like Elephant Flee */
        public bool Flee()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "cat") && !Game.Seek(location.x, location.y, Direction.up, "boulder"))
            {
                if (Game.Retreat(this, Direction.down)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.down, "cat") && !Game.Seek(location.x, location.y, Direction.down, "boulder"))
            {
                if (Game.Retreat(this, Direction.up)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.left, "cat") && !Game.Seek(location.x, location.y, Direction.left, "boulder"))
            {
                if (Game.Retreat(this, Direction.right)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.right, "cat") && !Game.Seek(location.x, location.y, Direction.right, "boulder"))
            {
                if (Game.Retreat(this, Direction.left)) return true;
            }
            return false;
        }

        public bool Hunt()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "grass") && !Game.Seek(location.x, location.y, Direction.up, "boulder"))
            {
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "grass") && !Game.Seek(location.x, location.y, Direction.down, "boulder"))
            {
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "grass") && !Game.Seek(location.x, location.y, Direction.left, "boulder"))
            {
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "grass") && !Game.Seek(location.x, location.y, Direction.right, "boulder"))
            {
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }
    }
}
