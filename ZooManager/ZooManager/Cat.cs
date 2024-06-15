namespace ZooManager
{
    public class Cat : Animal
    {
        private bool isOnGrass;
        private int turnsOnGrass = 0;

        public Cat(string name)
        {
            emoji = "🐱";
            species = "cat";
            this.name = name;
            reactionTime = new Random().Next(1, 6); // reaction time 1 (fast) to 5 (medium)Cat
            isOnGrass = false; // Initialized false 
        }

        public override void Activate()
        {
            base.Activate();
            Console.WriteLine("I am a cat. Meow.");
            Hunt();
            if (isOnGrass)
            {
                turnsOnGrass++;
                if (turnsOnGrass >= 2)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        if (Game.Seek(location.x, location.y, (Direction)i, "grass"))
                        {
                            Game.Attack(this, (Direction)i);
                            break;
                        }
                    }
                    isOnGrass = false;
                    turnsOnGrass = 0;
                }
            }

            for (var i = 0; i < 4; i++)
            {
                if (Game.Seek(location.x, location.y, (Direction)i, "grass"))
                {
                    if (!isOnGrass)
                    {
                        isOnGrass = true;
                        turnsOnGrass = 1;
                    }
                    break;
                }
                else if (isOnGrass)
                {
                    isOnGrass = false;
                    turnsOnGrass = 0;
                }
            }
        }

        public void Hunt()
        {
            if (Game.Seek(location.x, location.y, Direction.up, "mouse"))
            {
                Game.Attack(this, Direction.up);
            }
            else if (Game.Seek(location.x, location.y, Direction.down, "mouse"))
            {
                Game.Attack(this, Direction.down);
            }
            else if (Game.Seek(location.x, location.y, Direction.left, "mouse"))
            {
                Game.Attack(this, Direction.left);
            }
            else if (Game.Seek(location.x, location.y, Direction.right, "mouse"))
            {
                Game.Attack(this, Direction.right);
            }
        }

        public void KickBoulder(Boulder boulder)
        {
            int distance = 3; // set the distance to 3
            boulder.location.x += distance;
        }
    }
}
