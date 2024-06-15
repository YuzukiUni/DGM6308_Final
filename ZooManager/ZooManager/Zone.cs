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

        public bool IsEdge()
        {
            int midX = numCellsX / 2; 
            int midY = numCellsY / 2;
            return location.x >= midX - 1 && location.x <= midX + 1 && location.y >= midY - 1 && location.y <= midY + 1;
        }

    }
}

