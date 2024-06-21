﻿namespace ZooManager
{
    public class Snake : Animal
    {
        public Snake(string name)
        {
            emoji = "🐍";
            species = "snake";
            this.name = name;
            reactionTime = new Random().Next(1, 2);
        }

        public override void Activate()
        {
            base.Activate();
            Console.WriteLine("I am a snake. Voodoo.");
            if (!Hunt()) Flee();

        }

        public bool Flee()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "mouse"))
            {
                if (Game.Retreat(this, Direction.down)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.down, "mouse"))
            {
                if (Game.Retreat(this, Direction.up)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.left, "mouse"))
            {
                if (Game.Retreat(this, Direction.right)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.right, "mouse"))
            {
                if (Game.Retreat(this, Direction.left)) return true;
            }
            return false;
        }

        public bool Hunt()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "insect"))
            {
                Console.WriteLine("Found an insect ");
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "insect"))
            {
                Console.WriteLine("Found an insect ");
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "insect"))
            {
                Console.WriteLine("Found an insect");
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "insect"))
            {
                Console.WriteLine("Found an insect ");
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }
    }
    }
