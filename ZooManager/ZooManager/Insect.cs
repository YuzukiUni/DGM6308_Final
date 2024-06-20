using System;

namespace ZooManager
{
    public class Insect : Animal
    {
        public int boulderStayTurns { get; set; }
        public int grassStayTurns { get; set; }
        public int lastTurn { get; set; }

        public Insect(string name)
        {
            emoji = "🐞";
            species = "Insect";
            this.name = name;
            reactionTime = new Random().Next(2, 4); // reaction time of 1 (fast) to 3
            boulderStayTurns = 1; // stay on boulder for 1 turn
            grassStayTurns = 3; // stay on grass for 3 turns

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
  

    public void Stay(Occupant occupant, int turnCount)
        {
            int stayTerrain = turnCount - lastTurn;

            if (occupant is Boulder && stayTerrain < boulderStayTurns)
            {
                // Stay on boulder for boulderStayTurns turns
                Console.WriteLine("The insect is staying on the boulder for turn " + turnCount);
            }
            else if (occupant is Grass && stayTerrain < grassStayTurns)
            {
                // Stay on grass for grassStayTurns turns
                Console.WriteLine("The insect is staying on the grass for turn " + turnCount);
            }
            else
            {
                // Reset lastTurn if the insect moves to a new terrain
                lastTurn = turnCount;
                Console.WriteLine("The insect has moved to a new terrain on turn " + turnCount);
               newPlace();

            }
        }
        public void newPlace()
        {
           Random random = new Random();
    int direction = random.Next(4); // generates a random number between 0 and 3

    int newX = location.x;
    int newY = location.y;

    switch (direction)
    {
        case 0:
            // Move up
            newY++;
            break;
        case 1:
            // Move down
            newY--;
            break;
        case 2:
            // Move left
            newX--;
            break;
        case 3:
            // Move right
            newX++;
            break;
    }

    // Check if the new position is valid
    if (newX >= 0 && newX < Zone.numCellsX && newY >= 0 && newY < Zone.numCellsY) // Check if the new position is within the game area
    {
        Zone newZone = Game.animalZones[newY][newX]; // Get the new zone

        if (!newZone.IsEdge() && !newZone.IsBlocked && newZone.occupant == null) // Check if the new zone is not an edge, not blocked, and not occupied
        {
            // Move to the new position
            location.x = newX;
            location.y = newY;
            Console.WriteLine("The insect has moved to (" + location.x + ", " + location.y + ")");
        }
        else
        {
            Console.WriteLine("The insect cannot move to the new position because it is either an edge, blocked, or occupied.");
        }
    }
    else
    {
        Console.WriteLine("The insect cannot move to the new position because it is outside the game area.");
    }
        }
        public static Insect spawnInsect()
        {
            string name = "Vomm";
            return new Insect(name);
        }
    }
}
