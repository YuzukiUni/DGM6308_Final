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
                // If the zone is on the edge, it cannot have an occupant.
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

        // Check if the blocked zone is on the edge of the grid or in the center.
        public bool IsEdge()
        {
            return location.x == 0 || location.x == numCellsX - 1 ||
                   location.y == 0 || location.y == numCellsY - 1 ||
                   (location.x >= numCellsX / 2 - 1 && location.x <= numCellsX / 2 + 1 &&
                    location.y >= numCellsY / 2 - 1 && location.y <= numCellsY / 2 + 1);
        }
    }
}

