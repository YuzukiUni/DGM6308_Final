using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;

namespace ZooManager
{
    public class Game
    {
        static public List<List<Zone>> animalZones = new List<List<Zone>>();
        static public Zone holdingPen = new Zone(-1, -1, null);
        static public int numCellsX = 11;
        static public int numCellsY = 11;
        static public int turnCount = 20;
        public static int catCount { get; private set; } = 0;
        public static int snakeCount { get; private set; } = 0;
        public List<Cat> cats;
        public List<Boulder> boulders;
        public static bool gameWin = false;
        public static bool gameEnd = false;

        public Game()
        {
            SetUpGame();
        }
        public static void SetUpGame()
        {
            animalZones.Clear();
            for (var y = 0; y < numCellsY; y++)
            {
                List<Zone> rowList = new List<Zone>();
                for (var x = 0; x < numCellsX; x++)
                {
                    Zone zone = new Zone(x, y);
                    zone.IsBlocked = zone.IsEdge();
                    rowList.Add(zone);
                }
                animalZones.Add(rowList);
            }

            for (int i = 0; i < 6; i++)
            {
                // Generate Mice and Insect
                generativeInsect();
                generativeMouse();
            }

            // Generate objects
            generateObject();
        }

        public static void ZoneClick(Zone clickedZone)
        {
            Console.Write("Got animal ");
            Console.WriteLine(clickedZone.emoji == "" ? "none" : clickedZone.emoji);
            Console.Write("Held animal is ");
            Console.WriteLine(holdingPen.emoji == "" ? "none" : holdingPen.emoji);
            if (clickedZone.occupant != null) clickedZone.occupant.ReportLocation();
            if (holdingPen.occupant != null && clickedZone.occupant == null)
            {
                // put animal in zone from holding pen
                Console.WriteLine("Placing " + holdingPen.emoji);
                clickedZone.occupant = holdingPen.occupant;
                clickedZone.occupant.location = clickedZone.location;
                holdingPen.occupant = null;
                Console.WriteLine("Empty spot now holds: " + clickedZone.emoji);
                ActivateAnimals();
            }
            else if (holdingPen.occupant != null && clickedZone.occupant != null)
            {
                // Check if we are trying to place a Boulder on a Grass
                if (holdingPen.occupant is Boulder && clickedZone.occupant is Grass)
                {
                    // Replace the Grass with the Boulder
                    Console.WriteLine("Replacing " + clickedZone.emoji + " with " + holdingPen.emoji);
                    clickedZone.occupant = holdingPen.occupant;
                    clickedZone.occupant.location = clickedZone.location;
                    holdingPen.occupant = null;
                    Console.WriteLine("Grass spot now holds: " + clickedZone.emoji);
                    ActivateAnimals();
                }
                else
                {
                    Console.WriteLine("Could not place animal.");
                    // Don't activate animals since user didn't get to do anything
                }
            }
            generativeMouse();
            turnCount--;
            winCondition();
        }


        static public void generativeMouse()
        {
            Random random = new Random();
            // For each turn, generate new insect with 50%
            if (random.Next(0, 2) == 1)
            {
                bool foundEmptyZone = false;
                while (!foundEmptyZone)
                {
                    int x = random.Next(0, numCellsX);
                    int y = random.Next(0, numCellsY);
                    if (animalZones[y][x].occupant == null)
                    {
                        Mouse newMouse = new Mouse("RandomMouse");
                        animalZones[y][x].occupant = newMouse;
                        Console.WriteLine("Generated a new mouse at: (" + x + ", " + y + ")");
                        foundEmptyZone = true;
                    }
                }
            }
        }
        static public void generativeInsect()
        {
            // For each turn, 50% to generate new insect
            Random random = new Random();
            if (random.Next(0, 2) == 1)
            {
                bool emptyZone = false;
                while (!emptyZone)
                {
                    int x = random.Next(0, numCellsX);
                    int y = random.Next(0, numCellsY);
                    if (animalZones[y][x].occupant == null)
                    {
                        Insect newInsect = new Insect("RandomInsects");
                        animalZones[y][x].occupant = newInsect;
                        Console.WriteLine("Generated a new Insect at: (" + x + ", " + y + ")");
                        emptyZone = true;
                    }
                }
            }
        }
        static public void generateObject()
        {
            Random random = new Random();
            int numBoulders = random.Next(3, 6);
            int numGrass = random.Next(3, 6);

            for (int i = 0; i < numBoulders; i++)
            {
                bool emptyZone = false;
                while (!emptyZone)
                {
                    int x = random.Next(0, numCellsX);
                    int y = random.Next(0, numCellsY);
                    if (animalZones[y][x].occupant == null)
                    {
                        Boulder newBoulder = new Boulder();
                        animalZones[y][x].occupant = newBoulder;
                        Console.WriteLine("Generated a new boulder at: (" + x + ", " + y + ")");
                        emptyZone = true;
                    }
                }
            }

            for (int i = 0; i < numGrass; i++)
            {
                bool foundEmptyZone = false;
                while (!foundEmptyZone)
                {
                    int x = random.Next(0, numCellsX);
                    int y = random.Next(0, numCellsY);
                    if (animalZones[y][x].occupant == null)
                    {
                        Grass newGrass = new Grass();
                        animalZones[y][x].occupant = newGrass;
                        Console.WriteLine("Generated a new grass at: (" + x + ", " + y + ")");
                        foundEmptyZone = true;
                    }
                }
            }
        }


        static public void AddToHolding(string occupantType)
        {
            if (holdingPen.occupant != null) return;
            if (occupantType == "cat")
            {
                if (catCount >= 8)
                {
                    // Create a list of all the cats' positions
                    List<Tuple<int, int>> catPositions = new List<Tuple<int, int>>();
                    for (int y = 0; y < numCellsY; y++)
                    {
                        for (int x = 0; x < numCellsX; x++)
                        {
                            if (animalZones[y][x].occupant is Cat)
                            {
                                catPositions.Add(new Tuple<int, int>(y, x));
                            }
                        }
                    }

                    // Randomly select a cat to remove
                    Random rand = new Random();
                    int index = rand.Next(catPositions.Count);
                    Tuple<int, int> selectedCatPosition = catPositions[index];
                    animalZones[selectedCatPosition.Item1][selectedCatPosition.Item2].occupant = null;
                    catCount--;
                    Console.WriteLine("Randomly removed a cat due to over limit.");
                }
                holdingPen.occupant = new Cat("Fluffy");
                catCount++;
            }
            if (occupantType == "snake")
            {
                if (snakeCount >= 8)
                {
                    // Create a list of all the snakes' positions
                    List<Tuple<int, int>> snakePositions = new List<Tuple<int, int>>();
                    for (int y = 0; y < numCellsY; y++)
                    {
                        for (int x = 0; x < numCellsX; x++)
                        {
                            if (animalZones[y][x].occupant is Snake)
                            {
                                snakePositions.Add(new Tuple<int, int>(y, x));
                            }
                        }
                    }

                    // Randomly select a snake to remove
                    Random rand = new Random();
                    int index = rand.Next(snakePositions.Count);
                    Tuple<int, int> selectedSnakePosition = snakePositions[index];
                    animalZones[selectedSnakePosition.Item1][selectedSnakePosition.Item2].occupant = null;
                    snakeCount--;
                    Console.WriteLine("Randomly removed a snake due to over limit.");
                }
                holdingPen.occupant = new Snake("Slither");
                snakeCount++;
            }

            if (occupantType == "mouse") holdingPen.occupant = new Mouse("Squeaky");
            if (occupantType == "grass") holdingPen.occupant = new Grass();
            if (occupantType == "boulder")
            {
                // Create a new boulder
                Boulder boulder = new Boulder();

                // Only place the boulder in the holding pen
                holdingPen.occupant = boulder;
                Console.WriteLine($"Holding pen occupant at {holdingPen.occupant.location.x},{holdingPen.occupant.location.y}");
            }
        }
        public static void ActivateAnimals()
        {
            for (var r = 1; r < 11; r++) // reaction times from 1 to 10
            {
                for (var y = 0; y < numCellsY; y++)
                {
                    for (var x = 0; x < numCellsX; x++)
                    {
                        var zone = animalZones[y][x];
                        if (zone.occupant as Animal != null && ((Animal)zone.occupant).reactionTime == r)
                        {
                            ((Animal)zone.occupant).Activate();
                        }
                    }
                }
            }
        }
        static public bool Seek(int x, int y, Direction d, string target)
        {
            switch (d)
            {
                case Direction.up:
                    y--;
                    break;
                case Direction.down:
                    y++;
                    break;
                case Direction.left:
                    x--;
                    break;
                case Direction.right:
                    x++;
                    break;
            }
            if (y < 0 || x < 0 || y > numCellsY - 1 || x > numCellsX - 1) return false;
            if (animalZones[y][x].occupant == null) return false;
            if (animalZones[y][x].occupant.species == target)
            {
                return true;
            }
            return false;
        }
        public static void Attack(Animal attacker, Direction d)
        {
            Console.WriteLine($"{attacker.name} is attacking {d.ToString()}");
            int x = attacker.location.x;
            int y = attacker.location.y;

            switch (attacker)
            {
                case Snake snake:
                    // Snake's attack behavior
                    switch (d)
                    {
                        case Direction.up:
                            if (y > 0 && animalZones[y - 1][x].occupant is Insect)
                            {
                                // Snake eats the insect
                                animalZones[y - 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x}, {y - 1}");
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Insect)
                            {
                                // Snake eats the insect
                                animalZones[y + 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x}, {y + 1}");
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Insect)
                            {
                                // Snake eats the insect
                                animalZones[y][x - 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x - 1}, {y}");
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Insect)
                            {
                                // Snake eats the insect
                                animalZones[y][x + 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x + 1}, {y}");
                            }
                            break;
            }
            break;
                case Cat cat:
                    // Cat's attack behavior
                    switch (d)
                    {
                        case Direction.up:
                            if (y > 0 && animalZones[y - 1][x].occupant is Mouse)
                            {
                                // Cat eats the mouse
                                animalZones[y - 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the mouse at {x}, {y - 1}");
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Insect newInsect = Insect.spawnInsect();
                                    animalZones[y - 1][x].occupant = newInsect;
                                }
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Mouse)
                            {
                                // Cat eats the mouse
                                animalZones[y + 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the mouse at {x}, {y + 1}");
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Insect newInsect = Insect.spawnInsect();
                                    animalZones[y + 1][x].occupant = newInsect;
                                }
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Mouse)
                            {
                                // Cat eats the mouse
                                animalZones[y][x - 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the mouse at {x - 1}, {y}");
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Insect newInsect = Insect.spawnInsect();
                                    animalZones[y][x - 1].occupant = newInsect;
                                }
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Mouse)
                            {
                                // Cat eats the mouse
                                animalZones[y][x + 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the mouse at {x + 1}, {y}");
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Insect newInsect = Insect.spawnInsect();
                                    animalZones[y][x + 1].occupant = newInsect;
                                }
                            }
                            break;

                    }
                    break;
                case Insect insect:
                    // Insect's attack behavior
                    switch (d)
                    {
                        case Direction.up:
                            if (y > 0 && animalZones[y - 1][x].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y - 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y - 1}");
                                // 20% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y - 1][x].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y + 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y + 1}");
                                // 20% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y + 1][x].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y][x - 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x - 1}, {y}");
                                // 20% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y][x - 1].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y][x + 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x + 1}, {y}");
                                // 20% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y][x + 1].occupant = newGrass;
                                }
                            }
                            break;
                    }
                    break;
                case Mouse mouse:
                    // Mouse's attack behavior
                    switch (d)
                    {
                        case Direction.up:
                            if (y > 0 && animalZones[y - 1][x].occupant is Grass)
                            {
                                // Mouse eats the grass
                                animalZones[y - 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the grass at {x}, {y - 1}");
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Grass)
                            {
                                // Mouse eats the grass
                                animalZones[y + 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the grass at {x}, {y + 1}");
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Grass)
                            {
                                // Mouse eats the grass
                                animalZones[y][x - 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the grass at {x - 1}, {y}");
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Grass)
                            {
                                // Mouse eats the grass
                                animalZones[y][x + 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the grass at {x + 1}, {y}");
                            }
                            break;
                    }
                    break;

    }
        }


        static public bool Retreat(Animal runner, Direction d)
        {
            Console.WriteLine($"{runner.name} is retreating {d.ToString()}");
            int x = runner.location.x;
            int y = runner.location.y;

            switch (d)
            {
                case Direction.up:
                    if (y > 0 && animalZones[y - 1][x].occupant == null)
                    {
                        animalZones[y - 1][x].occupant = runner;
                        animalZones[y][x].occupant = null;
                        return true; // retreat was successful
                    }
                    return false; // retreat was not successful
                case Direction.down:
                    if (y < numCellsY - 1 && animalZones[y + 1][x].occupant == null)
                    {
                        animalZones[y + 1][x].occupant = runner;
                        animalZones[y][x].occupant = null;
                        return true;
                    }
                    return false;
                case Direction.left:
                    if (x > 0 && animalZones[y][x - 1].occupant == null)
                    {
                        animalZones[y][x - 1].occupant = runner;
                        animalZones[y][x].occupant = null;
                        return true;
                    }
                    return false;
                case Direction.right:
                    if (x < numCellsX - 1 && animalZones[y][x + 1].occupant == null)
                    {
                        animalZones[y][x + 1].occupant = runner;
                        animalZones[y][x].occupant = null;
                        return true;
                    }
                    return false;
            }
            return false; // fallback
        }
        public static bool Move(Occupant mover, Direction d)
        {
            Console.WriteLine($"{mover.species} is moving {d.ToString()}");
            int x = mover.location.x;
            int y = mover.location.y;

            switch (d)
            {
                case Direction.up:
                    if (y > 0)
                    {
                        if (animalZones[y - 1][x].occupant == null)
                        {
                            animalZones[y - 1][x].occupant = mover;
                            animalZones[y][x].occupant = null;
                            return true; // move was successful
                        }
                        else if (animalZones[y - 1][x].occupant is Boulder)
                        {
                            Boulder boulder = (Boulder)animalZones[y - 1][x].occupant;
                            if (boulder.Move(Direction.up)) // boulder moves in the same direction as the cat
                            {
                                animalZones[y - 1][x].occupant = mover;
                                animalZones[y][x].occupant = null;
                                return true; // move was successful
                            }
                        }
                    }
                    break;
                case Direction.down:
                    if (y < Game.numCellsY - 1)
                    {
                        if (animalZones[y + 1][x].occupant == null)
                        {
                            animalZones[y + 1][x].occupant = mover;
                            animalZones[y][x].occupant = null;
                            return true; // move was successful
                        }
                        else if (animalZones[y + 1][x].occupant is Boulder)
                        {
                            Boulder boulder = (Boulder)animalZones[y + 1][x].occupant;
                            if (boulder.Move(Direction.down)) // boulder moves in the same direction as the cat
                            {
                                animalZones[y + 1][x].occupant = mover;
                                animalZones[y][x].occupant = null;
                                return true; // move was successful
                            }
                        }
                    }
                    break;
                case Direction.left:
                    if (x > 0)
                    {
                        if (animalZones[y][x - 1].occupant == null)
                        {
                            animalZones[y][x - 1].occupant = mover;
                            animalZones[y][x].occupant = null;
                            return true; // move was successful
                        }
                        else if (animalZones[y][x - 1].occupant is Boulder)
                        {
                            Boulder boulder = (Boulder)animalZones[y][x - 1].occupant;
                            if (boulder.Move(Direction.left)) // boulder moves in the same direction as the cat
                            {
                                animalZones[y][x - 1].occupant = mover;
                                animalZones[y][x].occupant = null;
                                return true; // move was successful
                            }
                        }
                    }
                    break;
                case Direction.right:
                    if (x < Game.numCellsX - 1)
                    {
                        if (animalZones[y][x + 1].occupant == null)
                        {
                            animalZones[y][x + 1].occupant = mover;
                            animalZones[y][x].occupant = null;
                            return true; // move was successful
                        }
                        else if (animalZones[y][x + 1].occupant is Boulder)
                        {
                            Boulder boulder = (Boulder)animalZones[y][x + 1].occupant;
                            if (boulder.Move(Direction.right)) // boulder moves in the same direction as the cat
                            {
                                animalZones[y][x + 1].occupant = mover;
                                animalZones[y][x].occupant = null;
                                return true; // move was successful
                            }
                        }
                    }
                    break;
            }
            return false; // fallback
        }

        public void RemoveBoulder(Boulder boulder)
        {
            boulders.Remove(boulder);
        }
        public static bool winCondition()
        {
            int insectCount = 0;
            int mouseCount = 0;
            foreach (var row in animalZones)
            {
                foreach (var zone in row)
                {
                    if (zone.occupant is Insect)
                    {
                        insectCount++;
                    }
                    else if (zone.occupant is Mouse)
                    {
                        mouseCount++;
                    }
                }
            }
            if (insectCount == 0 && mouseCount == 0)
            {
                gameEnd = true;
                gameWin = true;
            }
            if (turnCount <= 0)
            {
                gameEnd = true;
                Console.WriteLine("You have no more turns, You lose !");
            }

            return gameEnd;
        }

        public static void resetGame()
        {
            animalZones.Clear();
            turnCount = 20;
            gameEnd = false;
            gameWin = false;
            SetUpGame();

            Console.WriteLine("Game Loading...");
        }
        public static void endGame()
        {
           gameEnd= true;
            Console.WriteLine("GG！");
        }

    }
}

