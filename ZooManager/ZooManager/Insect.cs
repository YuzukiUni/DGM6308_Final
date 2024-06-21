using System;

namespace ZooManager
{
    public class Insect : Animal
    {

        public Insect(string name)
        {
            emoji = "🐞";
            species = "insect";
            this.name = name;
            reactionTime = new Random().Next(3,6); // reaction time of 1 (fast) to 3
          

        }

        public override void Activate()
        {
            base.Activate();
            Console.WriteLine("I am an insect. Vomm.");
            if (!Hunt()) Flee();

        }
       
        public bool Flee()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "snake"))
            {
                if (Game.Retreat(this, Direction.down)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.down, "snake"))
            {
                if (Game.Retreat(this, Direction.up)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.left, "snake"))
            {
                if (Game.Retreat(this, Direction.right)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.right, "snake"))
            {
                if (Game.Retreat(this, Direction.left)) return true;
            }
            return false;
        }

        public bool Hunt()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "cat"))
            {
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "cat"))
            {
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "cat"))
            {
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "cat"))
            {
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }
  

    
        public static Insect spawnInsect()
        {
            string name = "Vomm";
            return new Insect(name);
        }
    }
}
