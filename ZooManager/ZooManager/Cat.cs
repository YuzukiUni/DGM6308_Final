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
            Console.WriteLine("I am a cat. Meow.");
            if (!Hunt()) Flee();
            avoidBoulder();
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
            if (Game.Seek(location.x, location.y, Direction.up, "mouse"))
            {
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "mouse"))
            {
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "mouse"))
            {
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "mouse"))
            {
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }

        // The cat must avoid boulders for two grids, regardless of other animals being in the route.
        public bool avoidBoulder()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "boulder"))
            {
                if (!Game.Seek(location.x, location.y, Direction.down, "snake") && !Game.Seek(location.x, location.y, Direction.down, "insect") && !Game.Seek(location.x, location.y, Direction.down, "mouse"))
                {
                    if (Game.Retreat(this, Direction.down) && Game.Retreat(this, Direction.down)) return true;
                }
            }
            if (Game.Seek(location.x, location.y, Direction.down, "boulder"))
            {
                if (!Game.Seek(location.x, location.y, Direction.up, "snake") && !Game.Seek(location.x, location.y, Direction.up, "insect") && !Game.Seek(location.x, location.y, Direction.up, "mouse"))
                {
                    if (Game.Retreat(this, Direction.up) && Game.Retreat(this, Direction.up)) return true;
                }
            }
            if (Game.Seek(location.x, location.y, Direction.left, "boulder"))
            {
                if (!Game.Seek(location.x, location.y, Direction.right, "snake") && !Game.Seek(location.x, location.y, Direction.right, "insect") && !Game.Seek(location.x, location.y, Direction.right, "mouse"))
                {
                    if (Game.Retreat(this, Direction.right) && Game.Retreat(this, Direction.right)) return true;
                }
            }
            if (Game.Seek(location.x, location.y, Direction.right, "boulder"))
            {
                if (!Game.Seek(location.x, location.y, Direction.left, "snake") && !Game.Seek(location.x, location.y, Direction.left, "insect") && !Game.Seek(location.x, location.y, Direction.left, "mouse"))
                {
                    if (Game.Retreat(this, Direction.left) && Game.Retreat(this, Direction.left)) return true;
                }
            }
            return false;
        }

    }
}
