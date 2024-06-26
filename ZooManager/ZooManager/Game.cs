﻿using Microsoft.AspNetCore.Components.Web;
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
        static public int turnCount = 15;
        public static int catCount { get; private set; } = 0;
        public static int snakeCount { get; private set; } = 0;
        public static int mouseCount { get; private set; } = 0;
        public static int insectCount { get; private set; } = 0;
        public List<Cat> cats;
        public List<Boulder> boulders;
        public static bool gameWin = false;
        public static bool gameEnd = false;
        public static int winCount { get; private set; } = 0;
        public static int loseCount { get; private set; } = 0;
        public static List<Animal> animals = new List<Animal>();

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
                    rowList.Add(zone);
                }
                animalZones.Add(rowList);
            }
            // Generate Mice and Insect 2-4total
            Random random = new Random();
            int totalMice = random.Next(2, 5);
            int totalInsects = random.Next(2, 5); 

            // Generate the mice
            for (int i = 0; i < totalMice; i++)
            {
                generativeMouse();
                mouseCount++;
            }
            // Generate the insects
            for (int i = 0; i < totalInsects; i++)
            {
                generativeInsect();
                insectCount++;
            }
            // Generate objects
            generateObject();
            // Generate 10 unique blocked locations randomly at the beginning
            List<Point> blockedGrid = new List<Point>();
            while (blockedGrid.Count <10)
            {
                // Generate a random x and y of the game grid
                int x = random.Next(numCellsX);
                int y = random.Next(numCellsY);
                Point newPoint = new Point { x = x, y = y };
                // Check if the new generated block grid is a duplicate or occupied, if so , generate new one
                bool isDuplicate = false;
                foreach (var point in blockedGrid)
                {
                    if (point.x == newPoint.x && point.y == newPoint.y)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate && animalZones[y][x].occupant == null)
                {
                    blockedGrid.Add(newPoint);
                    animalZones[y][x].IsBlocked = true;
                }
                else if (animalZones[y][x].occupant is Insect || animalZones[y][x].occupant is Mouse)
                {
                    blockedDeath(animalZones[y][x].occupant);
                }

            }
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
                }
            }
            turnCount--;
            winCondition();
        }

        public static void endTurn()
        {
            // At the end of each turn, randomly decide whether to generate a mouse, an insect, or nothing
            Random random = new Random();
            double chance = random.NextDouble();
            // 20% chance to generate a mouse
            if (chance < 0.2) 
            {
                generativeMouse();
                mouseCount++;
            }
            // 20% chance to generate an insect
            else if (chance >= 0.2 && chance < 0.4)
            {
                generativeInsect();
                insectCount++;
            }
            // Activate each animal's behavior
            ActivateAnimals();
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
                int indexZone = random.Next(emptyZones.Count);
                Mouse newMouse = new Mouse("Sneaky");
                emptyZones[indexZone].occupant = newMouse;
                Console.WriteLine("Generated a new mouse at: (" + emptyZones[indexZone].location.x + ", " + emptyZones[indexZone].location.y + ")");
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
                int indexZone = random.Next(emptyZones.Count);
                Insect newInsect = new Insect("Vomm");
                emptyZones[indexZone].occupant = newInsect;
                Console.WriteLine("Generated a new insect at: (" + emptyZones[indexZone].location.x + ", " + emptyZones[indexZone].location.y + ")");
            }
        }

        // Generate objects in the game, including grass and boulder
        static public void generateObject()
        {
            Random random = new Random();
            int numBoulders = random.Next(4, 9);
            int numGrass = random.Next(1, 3);
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
                        // Check if the occupant of the current grid is a cat
                        for (int x = 0; x < numCellsX; x++)
                        {
                            if (animalZones[y][x].occupant is Cat)
                            {
                               // Add the current position the list of cat positions
                                catPositions.Add(new Tuple<int, int>(y, x));
                            }
                        }
                    }
                    // If there are any cat positions,randomly select one of them.
                    if (catPositions.Count >=5)
                    {
                        Random rand = new Random();
                        int index = rand.Next(catPositions.Count);
                        // Remove the cat at the selected position.
                        Tuple<int, int> selectCatPosition = catPositions[index];
                        animalZones[selectCatPosition.Item1][selectCatPosition.Item2].occupant = null;
                        catCount--;
                        Console.WriteLine("Randomly removed a cat.");
                    }
                }
                holdingPen.occupant = new Cat("Fluffy");
                catCount++;
            }
            if (occupantType == "snake")
            {
                if (snakeCount >= 5)
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
                    // If there are any snake positions,randomly select one of them.
                    if (snakePositions.Count > 0)
                    {
                        Random rand = new Random();
                        int index = rand.Next(snakePositions.Count);
                        // Remove the snake at the selected position.
                        Tuple<int, int> selectSnakePosition = snakePositions[index];
                        animalZones[selectSnakePosition.Item1][selectSnakePosition.Item2].occupant = null;
                        snakeCount--;
                        Console.WriteLine("Randomly removed a snake.");
                    }
                }
                holdingPen.occupant = new Snake("Snake");
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
            // After adding the new occupant, activate all animals
            Console.WriteLine("Start to activate all animals...");
            ActivateAnimals();
            Console.WriteLine("all animals...");
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
                            // Activate the occupant
                            ((Animal)zone.occupant).Activate();

                            // If the occupant is at the edge (IsBlocked), handle its death
                            if (zone.IsBlocked)
                            {
                                blockedDeath(zone.occupant);
                            }
                        }
                    }
                }
            }
        }

        //Ref:https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.seek?view=net-8.0
        static public bool Seek(int x, int y, Direction d, string target, string seeker, int range = 1)
        {
            for (int i = 0; i < range; i++)
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
                // continue searching when encountering an empty zone
                if (animalZones[y][x].occupant == null) continue; 
                if (animalZones[y][x].occupant.species == "grass" && (seeker == "insect" || seeker == "snake")) continue; // insects and snakes can pass through grass
                if (animalZones[y][x].occupant.species == "boulder" && seeker == "snake") continue; // snakes can pass through boulders
                if (animalZones[y][x].occupant.species == target)
                {
                    return true;
                }
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
                                // Check if the insect can retreat
                                if (!Retreat((Animal)animalZones[y - 1][x].occupant, Direction.down))
                                {
                                    // Snake eats the insect
                                    animalZones[y - 1][x].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the insect at {x}, {y - 1}");
                                    // Decrease the count of insects
                                    insectCount--;
                                }
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Insect)
                            {
                                if (!Retreat((Animal)animalZones[y + 1][x].occupant, Direction.up))
                                {
                                    animalZones[y + 1][x].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the insect at {x}, {y + 1}");
                                    insectCount--;
                                }
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Insect)
                            {
                                if (!Retreat((Animal)animalZones[y][x-1].occupant, Direction.right))
                                {
                                    animalZones[y][x - 1].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the insect at {x - 1}, {y}");
                                    insectCount--;
                                }
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Insect)
                            {
                                if (!Retreat((Animal)animalZones[y][x+1].occupant, Direction.left))
                                {
                                    animalZones[y][x + 1].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the insect at {x + 1}, {y}");
                                    insectCount--;
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
                                // Check if the mouse can retreat
                                if (!Retreat((Animal)animalZones[y - 1][x].occupant, Direction.down))
                                {
                                    // Cat eats the mouse
                                    animalZones[y - 1][x].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the mouse at {x}, {y - 1}");
                                    // Decrease the count of mice
                                    mouseCount--;
                                }
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Mouse)
                            {
                                if (!Retreat((Animal)animalZones[y + 1][x].occupant, Direction.up))
                                {
                                    animalZones[y + 1][x].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the mouse at {x}, {y + 1}");
                                    mouseCount--;
                                }
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Mouse)
                            {
                                if (!Retreat((Animal)animalZones[y][x - 1].occupant, Direction.right))
                                {
                                    animalZones[y][x - 1].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the mouse at {x - 1}, {y}");
                                    mouseCount--;
                                }
                            }
                            break;

                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Mouse)
                            {
                                if (!Retreat((Animal)animalZones[y][x + 1].occupant, Direction.left))
                                {
                                    animalZones[y][x + 1].occupant = null;
                                    Console.WriteLine($"{attacker.name} eats the mouse at {x + 1}, {y}");
                                    //  the count of mice
                                    mouseCount--;
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
                            if (y >= 1 && animalZones[y - 1][x].occupant is Cat && !animalZones[y - 1][x].IsBlocked)
                            {
                                // Insect eats the cat
                                animalZones[y - 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y - 1}");
                                // 10% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y - 1][x].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                }
                            }
                            else if (y >= 2 && animalZones[y - 2][x].occupant is Cat && !animalZones[y - 2][x].IsBlocked)
                            {
                                // Insect eats the cat
                                animalZones[y - 2][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y - 2}");
                                // 10% chance to spawn a Grass object
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y - 2][x].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                }
                            }
                            break;
                        case Direction.down:
                            if (y < numCellsY - 1 && animalZones[y + 1][x].occupant is Cat && !animalZones[y + 1][x].IsBlocked)
                            {
                                animalZones[y + 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y + 1}");
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y + 1][x].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                }
                            }
                            else if (y < numCellsY - 2 && animalZones[y + 2][x].occupant is Cat && !animalZones[y + 2][x].IsBlocked)
                            {
                                animalZones[y + 2][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x}, {y + 2}");
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y + 2][x].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                }
                            }
                            break;
                        case Direction.left:
                            if (x >= 1 && animalZones[y][x - 1].occupant is Cat && !animalZones[y][x - 1].IsBlocked)
                            {
                                animalZones[y][x - 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x - 1}, {y}");
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    animalZones[y][x - 1].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                }
                            }
                            else if (x >= 2 && animalZones[y][x - 2].occupant is Cat && !animalZones[y][x - 2].IsBlocked)
                            {
                                Console.WriteLine($"{attacker.name} eats the cat at {x - 2}, {y}");
                                animalZones[y][x - 2].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x - 2}, {y}");
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Grass newGrass = new Grass();
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                    animalZones[y][x - 2].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                }
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Cat && !animalZones[y][x + 1].IsBlocked)
                            {
                                Console.WriteLine($"{attacker.name} eats the cat at {x + 1}, {y}");
                                animalZones[y][x + 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x + 1}, {y}");
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                    Grass newGrass = new Grass();
                                    animalZones[y][x + 1].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                }
                            }
                            else if (x < numCellsX - 2 && animalZones[y][x + 2].occupant is Cat && !animalZones[y][x + 2].IsBlocked)
                            {
                                Console.WriteLine($"{attacker.name} eats the cat at {x + 2}, {y}");
                                animalZones[y][x + 2].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the cat at {x + 2}, {y}");
                                if (new Random().NextDouble() < 0.1)
                                {
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
                                    Grass newGrass = new Grass();
                                    animalZones[y][x + 2].occupant = newGrass;
                                    Console.WriteLine("Spawning Grass for Cats Dead...");
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
                                animalZones[y + 1][x].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the grass at {x}, {y + 1}");
                            }
                            break;
                        case Direction.left:
                            if (x > 0 && animalZones[y][x - 1].occupant is Grass)
                            {
                                animalZones[y][x - 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the grass at {x - 1}, {y}");
                            }
                            break;
                        case Direction.right:
                            if (x < numCellsX - 1 && animalZones[y][x + 1].occupant is Grass)
                            {
                                animalZones[y][x + 1].occupant = null;
                                Console.WriteLine($"{attacker.name} eats the grass at {x + 1}, {y}");
                            }
                            break;
                    }
                    break;
            }
        }
        public static void blockedDeath(Occupant occupant)
        {
            // Check if the occupant is an Animal
            if (occupant is Animal animal)
            {
                // Check if the animal is at a blocked location (Mouse, Insects)
                if (animalZones[animal.location.y][animal.location.x].IsBlocked)
                {
                    if (animal is Insect)
                    {
                        insectCount--;
                        Console.WriteLine("Insect removed. Current insect count: " + insectCount);
                    }
                    else if (animal is Mouse)
                    {
                        mouseCount--;
                        Console.WriteLine("Mouse removed. Current mouse count: " + mouseCount);
                    }

                    // Remove the animal from the game
                    animals.Remove(animal);
                    animalZones[animal.location.y][animal.location.x].occupant = null;
                }
            }
        }

        // C#  as Operator Keyword ref:https://www.geeksforgeeks.org/c-sharp-as-operator-keyword/
        static public bool Retreat(Animal runner, Direction d)
        {
            Console.WriteLine($"{runner.name} is retreating {d.ToString()}");
            int x = runner.location.x;
            int y = runner.location.y;
            switch (d)
            {
                case Direction.up:
                    // Check the animal retreat movement
                    if (y > 0 && (animalZones[y - 1][x].occupant == null ||
                                (animalZones[y - 1][x].occupant is Grass && (runner is Insect || runner is Mouse)) ||
                                (animalZones[y - 1][x].occupant is Boulder && runner is Snake)))
                    {
                        // If there is a Boulder for Insect, the Insect dies
                        if (animalZones[y - 1][x].occupant is Boulder && runner is Insect)
                        {
                            Die(runner as Insect, x, y - 1);
                            return false; 
                        }
                        // If there is Grass and Insect or a Mouse,  tries to pass through the grass
                        if (animalZones[y - 1][x].occupant is Grass && (runner is Insect || runner is Mouse))
                        {
                            // Check if the grid after the Grass is empty
                            if (y > 1 && animalZones[y - 2][x].occupant == null)
                            {
                                animalZones[y - 2][x].occupant = runner;
                                animalZones[y][x].occupant = null;
                                runner.location.y = y - 2;
                                runner.location.x = x;
                                return true; 
                            }
                            return false; 
                        }
                        // If there is Grass or a Boulder, covers it
                        if (animalZones[y - 1][x].occupant is Grass || animalZones[y - 1][x].occupant is Boulder)
                        {
                            animalZones[y - 1][x].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            // Move to the new location
                            animalZones[y - 1][x].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        // Update the location after retreat
                        runner.location.y = y - 1;
                        runner.location.x = x;
                        // If the new location is blocked grid, the insects and mouse dies
                        if (animalZones[y - 1][x].IsBlocked)
                        {
                            blockedDeath(runner);
                        }
                        return true;
                    }
                    return false; 

                case Direction.down:
                    if (y < numCellsY - 1 && (animalZones[y + 1][x].occupant == null ||
                                (animalZones[y + 1][x].occupant is Grass && (runner is Insect || runner is Mouse)) ||
                                (animalZones[y + 1][x].occupant is Boulder && runner is Snake)))
                    {
                        if (animalZones[y + 1][x].occupant is Boulder && runner is Insect)
                        {
                            Die(runner as Insect, x, y + 1);
                            return false;
                        }
                        if (animalZones[y + 1][x].occupant is Grass && (runner is Insect || runner is Mouse))
                        {
                            if (y < numCellsY - 2 && animalZones[y + 2][x].occupant == null)
                            {
                                animalZones[y + 2][x].occupant = runner;
                                animalZones[y][x].occupant = null;
                                runner.location.y = y + 2;
                                runner.location.x = x;
                                return true;
                            }
                            return false;
                        }
                        if (animalZones[y + 1][x].occupant is Grass || animalZones[y + 1][x].occupant is Boulder)
                        {
                            animalZones[y + 1][x].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            animalZones[y + 1][x].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        runner.location.y = y + 1;
                        runner.location.x = x;
                        if (animalZones[y + 1][x].IsBlocked)
                        {
                            blockedDeath(runner);
                        }
                        return true;
                    }
                    return false;

                case Direction.left:
                    if (x > 0 && (animalZones[y][x - 1].occupant == null ||
                                (animalZones[y][x - 1].occupant is Grass && (runner is Insect || runner is Mouse)) ||
                                (animalZones[y][x - 1].occupant is Boulder && runner is Snake)))
                    {
                        if (animalZones[y][x - 1].occupant is Boulder && runner is Insect)
                        {
                            Die(runner as Insect, x - 1, y);
                            return false;
                        }
                        if (animalZones[y][x - 1].occupant is Grass && (runner is Insect || runner is Mouse))
                        {
                            if (x > 1 && animalZones[y][x - 2].occupant == null)
                            {
                                animalZones[y][x - 2].occupant = runner;
                                animalZones[y][x].occupant = null;
                                runner.location.y = y;
                                runner.location.x = x - 2;
                                return true;
                            }
                            return false;
                        }
                        if (animalZones[y][x - 1].occupant is Grass || animalZones[y][x - 1].occupant is Boulder)
                        {
                            animalZones[y][x - 1].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            animalZones[y][x - 1].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        runner.location.y = y;
                        runner.location.x = x - 1;
                        if (animalZones[y][x - 1].IsBlocked)
                        {
                            blockedDeath(runner);
                        }
                        return true;
                    }
                    return false;

                case Direction.right:
                    if (x < numCellsX - 1 && (animalZones[y][x + 1].occupant == null ||
                                (animalZones[y][x + 1].occupant is Grass && (runner is Insect || runner is Mouse)) ||
                                (animalZones[y][x + 1].occupant is Boulder && runner is Snake)))
                    {
                        if (animalZones[y][x + 1].occupant is Boulder && runner is Insect)
                        {
                            Die(runner as Insect, x + 1, y);
                            return false;
                        }
                        if (animalZones[y][x + 1].occupant is Grass && (runner is Insect || runner is Mouse))
                        {
                            if (x < numCellsX - 2 && animalZones[y][x + 2].occupant == null)
                            {
                                animalZones[y][x + 2].occupant = runner;
                                animalZones[y][x].occupant = null;
                                runner.location.y = y;
                                runner.location.x = x + 2;
                                return true;
                            }
                            return false;
                        }
                        if (animalZones[y][x + 1].occupant is Grass || animalZones[y][x + 1].occupant is Boulder)
                        {
                            animalZones[y][x + 1].occupant.CoveredBy = runner;
                        }
                        else
                        {
                            animalZones[y][x + 1].occupant = runner;
                        }
                        animalZones[y][x].occupant = null;
                        runner.location.y = y;
                        runner.location.x = x + 1;
                        if (animalZones[y][x + 1].IsBlocked)
                        {
                            blockedDeath(runner);
                        }
                        return true;
                    }
                    return false;
            }
            return false;
        }


        // For Animal killed by objects------Insects
        public static void Die(Insect insect, int x, int y)
        {
            // Remove the insect from the game
            animalZones[y][x].occupant = null;
            insectCount--;
            Console.WriteLine("The insect has encountered a boulder and died.");

            // 50% chance to spawn a new Insect in random direction
            if (new Random().NextDouble() < 0.5)
            {
                Insect newInsect = Insect.spawnInsect();
                Console.WriteLine("Respawning New Insects, Vomm!");
                insectCount++;
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
                    // If the occupant of the current zone is an Insect or Mouse, count++
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
            insectCount = 0;
            mouseCount = 0;
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
