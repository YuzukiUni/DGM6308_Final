using System;
using System.Collections.Generic;

namespace ZooManager
{
    public static class Game
    {
        static public List<List<Zone>> animalZones = new List<List<Zone>>();
        static public Zone holdingPen = new Zone(-1, -1, null);
        static public int numCellsX = 11; 
        static public int numCellsY = 11;
        static public int turnCount = 0;
        public static int catCount { get; private set; } = 0;
        static public void SetUpGame()
        {
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


        static public void ZoneClick(Zone clickedZone)
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
                turnCount++;
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
                    turnCount++;
                }
                else
                {
                    Console.WriteLine("Could not place animal.");
                    // Don't activate animals since user didn't get to do anything
                }
            }
            generativeInsect();
            generativeMouse();

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
                bool foundEmptyZone = false;
                while (!foundEmptyZone)
                {
                    int x = random.Next(0, numCellsX);
                    int y = random.Next(0, numCellsY);
                    if (animalZones[y][x].occupant == null)
                    {
                        Insect newInsect = new Insect("RandomInsects");
                        animalZones[y][x].occupant = newInsect;
                        Console.WriteLine("Generated a new Insect at: (" + x + ", " + y + ")");
                        foundEmptyZone = true;
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
                if (catCount >= 5)
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
            if (occupantType == "mouse") holdingPen.occupant = new Mouse("Squeaky");
            if (occupantType == "grass") holdingPen.occupant = new Grass();
            if (occupantType == "boulder")
            {
                Boulder boulder = new Boulder();
                Zone zone = Zone.zoneWithGrass();
                if (zone != null)
                {
                    zone.occupant = boulder;
                    holdingPen.occupant = boulder;

                }
                Console.WriteLine($"Holding pen occupant at {holdingPen.occupant.location.x},{holdingPen.occupant.location.y}");
            }
        }
        static public void ActivateAnimals()
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

        static public void Attack(Animal attacker, Direction d)
        {
            Console.WriteLine($"{attacker.name} is attacking {d.ToString()}");
            int x = attacker.location.x;
            int y = attacker.location.y;

            switch (d)
            {
                case Direction.up:
                    if (y > 0 && animalZones[y - 1][x].occupant is Mouse)
                    {
                        // 20% chance to spawn an insect
                        if (new Random().NextDouble() < 0.2)
                        {
                            Insect newInsect = Insect.spawnInsect();
                            animalZones[y - 1][x].occupant = newInsect;
                        }
                        else
                        {
                            animalZones[y - 1][x].occupant = null;
                        }
                    }
                    break;
                case Direction.down:
                    if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Mouse)
                    {
                        // 20% chance to spawn an insect
                        if (new Random().NextDouble() < 0.2)
                        {
                            Insect newInsect = Insect.spawnInsect();
                            animalZones[y + 1][x].occupant = newInsect;
                        }
                        else
                        {
                            animalZones[y + 1][x].occupant = null;
                        }
                    }
                    break;
                case Direction.left:
                    if (x > 0 && animalZones[y][x - 1].occupant is Mouse)
                    {
                        // 20% chance to spawn an insect
                        if (new Random().NextDouble() < 0.2)
                        {
                            Insect newInsect = Insect.spawnInsect();
                            animalZones[y][x - 1].occupant = newInsect;
                        }
                        else
                        {
                            animalZones[y][x - 1].occupant = null;
                        }
                    }
                    break;
                case Direction.right:
                    if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Mouse)
                    {
                        // 20% chance to spawn an insect
                        if (new Random().NextDouble() < 0.2)
                        {
                            Insect newInsect = Insect.spawnInsect();
                            animalZones[y][x + 1].occupant = newInsect;
                        }
                        else
                        {
                            animalZones[y][x + 1].occupant = null;
                        }
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
    }
}

