namespace ZooManager
{
    public class Cat : Animal
    {
        private bool isOnGrass;
        private int turnsOnGrass = 0;
        public static int numCellsX = 11;
        public static int numCellsY = 11;

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
        public void moveBoulders()
        {
            if (location.y > 0 &&
                Game.animalZones[location.y - 1][location.x].occupant is Boulder)
            {
                Boulder boulder = (Boulder)Game.animalZones[location.y - 1][location.x].occupant;
                boulder.Kick(Direction.down);
            }
            if (location.y < Game.numCellsY - 1 &&
                Game.animalZones[location.y + 1][location.x].occupant is Boulder)
            {
                Boulder boulder = (Boulder)Game.animalZones[location.y + 1][location.x].occupant;
                boulder.Kick(Direction.up);
            }

            if (location.x > 0 &&
                Game.animalZones[location.y][location.x - 1].occupant is Boulder)
            {
                Boulder boulder = (Boulder)Game.animalZones[location.y][location.x - 1].occupant;
                boulder.Kick(Direction.right);
            }
            if (location.x < Game.numCellsX - 1 &&
                Game.animalZones[location.y][location.x + 1].occupant is Boulder)
            {
                Boulder boulder = (Boulder)Game.animalZones[location.y][location.x + 1].occupant;
                boulder.Kick(Direction.left);
            }
        }

    }
}
