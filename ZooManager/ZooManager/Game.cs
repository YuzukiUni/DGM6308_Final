using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
        public static int winCount { get; private set; } = 0;
        public static int loseCount { get; private set; } = 0;

        public Game()
        {
            SetUpGame();
        }
        public static void SetUpGame()
        {
            // Clear the game for each round
            animalZones.Clear();
            // Note one-line variation of for loop below!
            for (var y = 0; y < numCellsY; y++)
            {
                List<Zone> rowList = new List<Zone>();
                for (var x = 0; x < numCellsX; x++)
                {
                    Zone zone = new Zone(x, y);
                    // Set the zone as blocked if it is on the edge of the game grid.
                    zone.IsBlocked = zone.IsEdge();
                    rowList.Add(zone);
                }
                animalZones.Add(rowList);
            }

            // Generate Mice and Insect 4-6 total
            Random random = new Random();
            int totalMice = random.Next(2, 4); // Generate a random number between 2 and 4 for mice
            int totalInsects = random.Next(2, 4); // Generate a random number between 2 and 4 for insects

            // Generate the mice
            for (int i = 0; i < totalMice; i++)
            {
                generativeMouse();
            }

            // Generate the insects
            for (int i = 0; i < totalInsects; i++)
            {
                generativeInsect();
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
                // Take animal in zone from holding pen
                Console.WriteLine("Taking " + holdingPen.emoji);
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
            // Generate a new mouse and insect in the game after each turn.
            generativeMouse();
            generativeInsect();
            // Decrease the turn count by one after each turn.
            turnCount--;
            // Check the win condition after each turn to see if the game has ended.
            winCondition();
        }
        public static void endTurn()
        {
            // At the end of each turn, randomly generate 0-1 mouse or 0-1 insect
            Random random = new Random();
            if (random.Next(0, 2) == 0)
            {
                generativeMouse();
            }
            else
            {
                generativeInsect();
            }
        }

        // Randomly generate mouse
        static public void generativeMouse()
        {
            Random random = new Random();
            List<Zone> emptyZones = new List<Zone>();
            // Find all empty and unblocked zones to place
            for (int y = 0; y < numCellsY; y++)
            {
                for (int x = 0; x < numCellsX; x++)
                {
                    if (animalZones[y][x].occupant == null && !animalZones[y][x].IsBlocked)
                    {
                        emptyZones.Add(animalZones[y][x]);
                    }
                }
            }
            // If there are any empty and unblocked zones, place a new Mouse
            if (emptyZones.Count > 0)
            {
                int index = random.Next(emptyZones.Count);
                Mouse newMouse = new Mouse("Sneaky");
                emptyZones[index].occupant = newMouse;
                Console.WriteLine("Generated a new mouse at: (" + emptyZones[index].location.x + ", " + emptyZones[index].location.y + ")");
            }
        }

        // Randomly generate insect
        static public void generativeInsect()
        {
            Random random = new Random();
            List<Zone> emptyZones = new List<Zone>();
            // Find all empty and unblocked zones to place
            for (int y = 0; y < numCellsY; y++)
            {
                for (int x = 0; x < numCellsX; x++)
                {
                    if (animalZones[y][x].occupant == null && !animalZones[y][x].IsBlocked)
                    {
                        emptyZones.Add(animalZones[y][x]);
                    }
                }
            }
            // If there are any empty and unblocked zones, place a new Insect
            if (emptyZones.Count > 0)
            {
                int index = random.Next(emptyZones.Count);
                Insect newInsect = new Insect("Vomm");
                emptyZones[index].occupant = newInsect;
                Console.WriteLine("Generated a new insect at: (" + emptyZones[index].location.x + ", " + emptyZones[index].location.y + ")");
            }
        }

        // Generate objects in the game, including grass and boulder
        static public void generateObject()
        {
            Random random = new Random();
            int numBoulders = random.Next(3, 7);
            int numGrass = random.Next(3, 7);
            for (int i = 0; i < numBoulders; i++) 
            {
                bool emptyZone = false;

                // Keep looking for an empty zone in the animalZones grid to place a new boulder
                while (!emptyZone)
                {
                    int x = random.Next(0, numCellsX);
                    int y = random.Next(0, numCellsY);

                    if (animalZones[y][x].occupant == null)
                    {
                        // If it is, create a new Boulder object and place it in the zone
                        Boulder newBoulder = new Boulder();
                        animalZones[y][x].occupant = newBoulder;
                        Console.WriteLine("Generated a new boulder at: (" + x + ", " + y + ")");
                        emptyZone = true;
                    }
                }
            }

            //Generate the specified number of grass
            for (int i = 0; i < numGrass; i++)
            {
                bool foundEmptyZone = false;

                // Keep looking for an empty zone in the animalZones grid to place a new grass
                while (!foundEmptyZone)
                {
                    int x = random.Next(0, numCellsX);
                    int y = random.Next(0, numCellsY);
                    // Check if the zone at the generated coordinates is empty
                    // If it is, create a new Grass object and place it in the zone
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

        //Tuple Ref:https://learn.microsoft.com/en-us/dotnet/api/system.tuple-2?view=net-8.0
        static public void AddToHolding(string occupantType)
        {
            if (holdingPen.occupant != null) return;
            if (occupantType == "cat")
            {
                if (catCount >= 6)
                {
                    // Create a list of all the cats' positions gor over 6 cats
                    List<Tuple<int, int>> catPositions = new List<Tuple<int, int>>();
                    for (int y = 0; y < numCellsY; y++)
                    {
                        // Check if the occupant of the current grid is a Cat.
                        for (int x = 0; x < numCellsX; x++)
                        {
                            if (animalZones[y][x].occupant is Cat)
                            {
                               // add the current position the list of cat positions
                                catPositions.Add(new Tuple<int, int>(y, x));
                            }
                        }
                    }

                    // Randomly select a cat to remove if over 6 cats
                    Random rand = new Random();
                    int index = rand.Next(catPositions.Count);
                    //specific cat’s position
                    Tuple<int, int> selectCatPosition = catPositions[index];
                    animalZones[selectCatPosition.Item1][selectCatPosition.Item2].occupant = null;
                    catCount--;
                    Console.WriteLine("Randomly removed a cats.");
                }
                holdingPen.occupant = new Cat("Fluffy");
                catCount++;
            }
            if (occupantType == "snake")
            {
                if (snakeCount >= 6)
                {
                    // Create a list of all the snakes' positions
                    List<Tuple<int, int>> snakePositions = new List<Tuple<int, int>>();
                    for (int y = 0; y < numCellsY; y++)
                    {
                        for (int x = 0; x < numCellsX; x++)
                        {
                            // Check if the occupant of the current grid is a Snake.
                            if (animalZones[y][x].occupant is Snake)
                            {                               
                                // Add the current position the list of snake positions
                                snakePositions.Add(new Tuple<int, int>(y, x));
                            }
                        }
                    }

                    // Randomly select a snake to remove
                    Random rand = new Random();
                    int index = rand.Next(snakePositions.Count);
                    //specific snake’s position
                    Tuple<int, int> selectSnakePosition = snakePositions[index];
                    animalZones[selectSnakePosition.Item1][selectSnakePosition.Item2].occupant = null;
                    snakeCount--;
                    Console.WriteLine("Randomly removed a snakes.");
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
                                Console.WriteLine($"Detected an insect at {x}, {y - 1}");
                                // Snake eats the insect
                                animalZones[y - 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x}, {y - 1}");
                                // 20% chance to spawn a Grass
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y - 1][x].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Insect)
                            {
                                Console.WriteLine($"Detected an insect at {x}, {y + 1}");

                                // Snake eats the insect
                                animalZones[y + 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x}, {y + 1}");
                                // 20% chance to spawn a Grass
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y +1][x].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Insect)
                            {
                                Console.WriteLine($"Detected an insect at {x-1}, {y }");

                                // Snake eats the insect
                                animalZones[y][x - 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x - 1}, {y}");
                                // 20% chance to spawn a Grass
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y][x - 1].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Insect)
                            {
                                Console.WriteLine($"Detected an insect at {x+1}, {y}");
                                // Snake eats the insect
                                animalZones[y][x + 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the insect at {x + 1}, {y}");
                                // 20% chance to spawn a Grass
                                if (new Random().NextDouble() < 0.2)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y][x +1].occupant = newGrass;
                                }
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
                                // 30% chance to spawn a Insect
                                if (new Random().NextDouble() < 0.3)
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
                                // 30% chance to spawn a Insect
                                if (new Random().NextDouble() < 0.3)
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
                                // 30% chance to spawn a Insect
                                if (new Random().NextDouble() < 0.3)
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
                                // 30% chance to spawn a Insect
                                if (new Random().NextDouble() < 0.3)
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
                            if (y > 1 && animalZones[y - 2][x].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y - 2][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y - 2}");
                                // 10% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y - 2][x].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 2 && animalZones[y + 2][x].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y + 2][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y + 2}");
                                // 10% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y + 2][x].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.left:
                            if (x > 1 && animalZones[y][x - 2].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y][x - 2].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x - 2}, {y}");
                                // 10% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y][x - 2].occupant = newGrass;
                                }
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 2 && animalZones[y][x + 2].occupant is Cat)
                            {
                                // Insect eats the cat
                                animalZones[y][x + 2].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x + 2}, {y}");
                                // 10% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y][x + 2].occupant = newGrass;
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
                    if (y > 0 && (animalZones[y - 1][x].occupant == null || (animalZones[y - 1][x].occupant is Grass && (runner is Insect || runner is Snake))))
                    {
                        if (animalZones[y - 1][x].occupant is Grass)
                        {
                            animalZones[y - 1][x].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            animalZones[y - 1][x].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        return true; // retreat was successful
                    }
                    return false; // retreat was not successful
                case Direction.down:
                    if (y < numCellsY - 1 && (animalZones[y + 1][x].occupant == null || (animalZones[y + 1][x].occupant is Grass && (runner is Insect || runner is Snake))))
                    {
                        if (animalZones[y + 1][x].occupant is Grass)
                        {
                            animalZones[y + 1][x].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            animalZones[y + 1][x].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        return true;
                    }
                    return false;
                case Direction.left:
                    if (x > 0 && (animalZones[y][x - 1].occupant == null || (animalZones[y][x - 1].occupant is Grass && (runner is Insect || runner is Snake))))
                    {
                        if (animalZones[y][x - 1].occupant is Grass)
                        {
                            animalZones[y][x - 1].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            animalZones[y][x - 1].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        return true;
                    }
                    return false;
                case Direction.right:
                    if (x < numCellsX - 1 && (animalZones[y][x + 1].occupant == null || (animalZones[y][x + 1].occupant is Grass && (runner is Insect || runner is Snake))))
                    {
                        if (animalZones[y][x + 1].occupant is Grass)
                        {
                            animalZones[y][x + 1].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            animalZones[y][x + 1].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        return true;
                    }
                    return false;
            }
            return false; // fallback
        }

        // For Animal kileed by objects, for now is Insects
        public static void Die(Insect insect, int x, int y)
        {
            Console.WriteLine("The insect has encountered a boulder and died.");
            // Remove the insect from the game
            animalZones[y][x].occupant = null;
            // 50% chance to spawn a new Insect in random direction
            if (new Random().NextDouble() < 0.5)
            {
                Insect newInsect = Insect.spawnInsect();
                // Randomly choose a direction
                Direction d = (Direction)new Random().Next(4); 
                switch (d)
                {
                    case Direction.up:
                        if (y > 1)
                        {
                            animalZones[y - 2][x].occupant = newInsect;
                        }
                        break;
                    case Direction.down:
                        if (y < numCellsY - 2)
                        {
                            animalZones[y + 2][x].occupant = newInsect;
                        }
                        break;
                    case Direction.left:
                        if (x > 1)
                        {
                            animalZones[y][x - 2].occupant = newInsect;
                        }
                        break;
                    case Direction.right:
                        if (x < numCellsX - 2)
                        {
                            animalZones[y][x + 2].occupant = newInsect;
                        }
                        break;
                }
            }
        }

        // Checks if the game has been won or lost.
        public static bool winCondition()
        {
            int insectCount = 0;
            int mouseCount = 0;
            foreach (var row in animalZones)
            {
                foreach (var zone in row)
                {
                    // If the occupant of the current zone is an Insect or Mouse, increment the insect counter.
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

            // If there are no insects and no mice left within 20 turns, player wins, otherwise, player lose by gameEnd
            if (insectCount == 0 && mouseCount == 0)
            {
                gameEnd = true;
                gameWin = true;
                winCount++;
                Console.WriteLine("You wins !");
            }
            if (turnCount <= 0)
            {
                gameEnd = true;
                loseCount++;
                Console.WriteLine("You have no more turns, You lose !");
            }
            return gameEnd;
        }

        // Resets the game to its initial state.
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
            gameEnd = true;
            Console.WriteLine("GG！");
        }
    }
}
