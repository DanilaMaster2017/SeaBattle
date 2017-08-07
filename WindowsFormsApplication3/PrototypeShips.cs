using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
    public class PrototypeShips
    {
        int size;
        Orientation shipsOrientation;
        Location shipsLocation;

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public Orientation ShipsOrientation
        {
            get { return shipsOrientation; }
            set { shipsOrientation = value; }
        }

        public Location ShipsLocation
        {
            get { return shipsLocation; }
            set { shipsLocation = value; }
        }

        public PrototypeShips(Location location,Orientation orientation, int size)
        {
            ShipsLocation = location;
            ShipsOrientation = orientation;
            Size = size;
        }

        public PrototypeShips()
        { }
    }
}
