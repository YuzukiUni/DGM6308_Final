using System;

namespace ZooManager
{
    public class Zone
    {
        public static int numCellsX = 11; 
        public static int numCellsY = 11; 
        private Occupant _occupant = null;

        public Occupant occupant
        {
            get { return _occupant; }
            set
            {
                if (IsEdge())
                {
                    _occupant = null;
                }
                else
                {
                    _occupant = value;
                    if (_occupant != null)
                    {
                        _occupant.location = location;
                    }
                }
            }
        }

        public Point location = new Point();
        public bool IsBlocked { get; set; }

        public string emoji
        {
            get
            {
                if (occupant == null) return "";
                return occupant.emoji;
            }
        }

        public string rtLabel
        {
            get
            {
                if (occupant as Animal == null) return "";
                return ((Animal)occupant).reactionTime.ToString();
            }
        }

        public Zone(int x, int y, Occupant occupant = null)
        {
            location.x = x;
            location.y = y;
            this.occupant = occupant;
        }
        public static Zone zoneWithGrass()
        {
            for (int y = 0; y < numCellsY; y++)
            {
                for (int x = 0; x < numCellsX; x++)
                {
                    if (Game.animalZones[y][x].occupant is Grass)
                    {
                        return Game.animalZones[y][x];
                    }
                }
            }
            return null; 
        }
        public bool IsEdge()
        {
            return location.x == 0 || location.x == numCellsX - 1 ||
           location.y == 0 || location.y == numCellsY - 1;
        }

    }
}

