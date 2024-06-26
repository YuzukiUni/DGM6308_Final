namespace ZooManager
{
    public class Snake : Animal
    {
        public Snake(string name)
        {
            emoji = "🐍";
            species = "snake";
            this.name = name;
            reactionTime = new Random().Next(2, 6); // reaction time 2 to 6 snake
        }

        public override void Activate()
        {
            base.Activate();
            if (!Hunt()) Flee();
            if (Pass()) Console.WriteLine("I am a snake. I can pass through grass and boulders.");
        }

        public bool Pass()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "grass", "snake") ||
                Game.Seek(location.x, location.y, Direction.down, "grass", "snake") ||
                Game.Seek(location.x, location.y, Direction.left, "grass", "snake") ||
                Game.Seek(location.x, location.y, Direction.right, "grass", "snake") ||
                Game.Seek(location.x, location.y, Direction.up, "boulder", "snake") ||
                Game.Seek(location.x, location.y, Direction.down, "boulder", "snake") ||
                Game.Seek(location.x, location.y, Direction.left, "boulder", "snake") ||
                Game.Seek(location.x, location.y, Direction.right, "boulder", "snake"))
            {
                return true;
            }
            return false;
        }

        public bool Flee()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "mouse", "snake"))
            {
                if (Game.Retreat(this, Direction.down)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.down, "mouse", "snake"))
            {
                if (Game.Retreat(this, Direction.up)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.left, "mouse", "snake"))
            {
                if (Game.Retreat(this, Direction.right)) return true;
            }
            if (Game.Seek(location.x, location.y, Direction.right, "mouse", "snake"))
            {
                if (Game.Retreat(this, Direction.left)) return true;
            }
            return false;
        }

        public bool Hunt()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "insect", "snake"))
            {
                Console.WriteLine("Found an insect ");
                Game.Attack(this, Direction.up);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "insect", "snake"))
            {
                Console.WriteLine("Found an insect ");
                Game.Attack(this, Direction.down);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "insect", "snake"))
            {
                Console.WriteLine("Found an insect");
                Game.Attack(this, Direction.left);
                return true;
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "insect", "snake"))
            {
                Console.WriteLine("Found an insect ");
                Game.Attack(this, Direction.right);
                return true;
            }
            return false;
        }
    }
}
