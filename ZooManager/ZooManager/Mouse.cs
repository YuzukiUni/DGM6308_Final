using System;
namespace ZooManager
{
    public class Mouse : Animal
    {
        public Mouse(string name)
        {
            emoji = "🐭";
            species = "mouse";
            this.name = name; // "this" to clarify instance vs. method parameter
            reactionTime = new Random().Next(2, 4); // reaction time of 2(fast) to 4
      
        }

        public override void Activate()
        {
            base.Activate();
            if (!Hunt()) Flee(); 

        }
        // Follow Game.cs seek and retreat
        public bool Flee()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "cat", "mouse") && !Game.Seek(location.x, location.y, Direction.up, "boulder", "mouse"))
            {
                if (Game.Retreat(this, Direction.down)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.down, "cat", "mouse") && !Game.Seek(location.x, location.y, Direction.down, "boulder", "mouse"))
            {
                if (Game.Retreat(this, Direction.up)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.left, "cat", "mouse") && !Game.Seek(location.x, location.y, Direction.left, "boulder", "mouse"))
            {
                if (Game.Retreat(this, Direction.right)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.right, "cat", "mouse") && !Game.Seek(location.x, location.y, Direction.right, "boulder", "mouse"))
            {
                if (Game.Retreat(this, Direction.left)) return true;
            }
            return false;
        }

        public bool Hunt()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "grass", "mouse") && !Game.Seek(location.x, location.y, Direction.up, "boulder", "mouse"))
            {
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "grass", "mouse") && !Game.Seek(location.x, location.y, Direction.down, "boulder", "mouse"))
            {
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "grass", "mouse") && !Game.Seek(location.x, location.y, Direction.left, "boulder", "mouse"))
            {
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "grass", "mouse") && !Game.Seek(location.x, location.y, Direction.right, "boulder", "mouse"))
            {
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }


    }
}

