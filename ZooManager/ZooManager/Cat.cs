namespace ZooManager
{
    public class Cat : Animal
    {

        public Cat(string name)
        {
            emoji = "🐱";
            species = "cat";
            this.name = name;
            reactionTime = new Random().Next(1, 7); // reaction time 1 to 7 Cat
        }

        public override void Activate()
        {
            base.Activate();
            if (!Hunt()) Flee();
            avoidBoulder();
        }
        public bool Flee()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "snake", "cat"))
            {
                if (Game.Retreat(this, Direction.down)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.down, "snake", "cat"))
            {
                if (Game.Retreat(this, Direction.up)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.left, "snake", "cat"))
            {
                if (Game.Retreat(this, Direction.right)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.right, "snake", "cat"))
            {
                if (Game.Retreat(this, Direction.left)) return true;
            }
            return false;
        }

        public bool Hunt()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "mouse", "cat"))
            {
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "mouse", "cat"))
            {
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "mouse", "cat"))
            {
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "mouse", "cat"))
            {
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }

        // The cat must avoid boulders for two grids, regardless of other animals being in the route.
        public bool avoidBoulder()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "boulder", "cat"))
            {
                if (!Game.Seek(location.x, location.y, Direction.down, "snake", "cat") && !Game.Seek(location.x, location.y, Direction.down, "insect", "cat") && !Game.Seek(location.x, location.y, Direction.down, "mouse", "cat"))
                {
                    if (Game.Retreat(this, Direction.down) && Game.Retreat(this, Direction.down)) return true;
                }
            }
            if (Game.Seek(location.x, location.y, Direction.down, "boulder", "cat"))
            {
                if (!Game.Seek(location.x, location.y, Direction.up, "snake", "cat") && !Game.Seek(location.x, location.y, Direction.up, "insect", "cat") && !Game.Seek(location.x, location.y, Direction.up, "mouse", "cat"))
                {
                    if (Game.Retreat(this, Direction.up) && Game.Retreat(this, Direction.up)) return true;
                }
            }
            if (Game.Seek(location.x, location.y, Direction.left, "boulder", "cat"))
            {
                if (!Game.Seek(location.x, location.y, Direction.right, "snake", "cat") && !Game.Seek(location.x, location.y, Direction.right, "insect", "cat") && !Game.Seek(location.x, location.y, Direction.right, "mouse", "cat"))
                {
                    if (Game.Retreat(this, Direction.right) && Game.Retreat(this, Direction.right)) return true;
                }
            }
            if (Game.Seek(location.x, location.y, Direction.right, "boulder", "cat"))
            {
                if (!Game.Seek(location.x, location.y, Direction.left, "snake", "cat") && !Game.Seek(location.x, location.y, Direction.left, "insect", "cat") && !Game.Seek(location.x, location.y, Direction.left, "mouse", "cat"))
                {
                    if (Game.Retreat(this, Direction.left) && Game.Retreat(this, Direction.left)) return true;
                }
            }
            return false;
        }

    }
}
