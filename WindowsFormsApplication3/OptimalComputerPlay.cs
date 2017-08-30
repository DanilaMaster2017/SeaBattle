using System;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
    public enum SearchMode { OneDeckSearch=1, TwoDeckSearch=2, FourDeckSearch=4 }

    enum Diagonal { Main, Side }

    public class OptimalComputerPlay : ComputerPlay, IEnumerable
    {
        SearchMode searchMode;

        public SearchMode SearchMod
        {
            get { return searchMode; }
            set { searchMode = value; }
        }

        IGetLocation location;

        Random random = new Random(DateTime.Now.Millisecond);
        Diagonal diagonal;

        int[] ShipsCount;

        public OptimalComputerPlay(Field field):base(field)
        {
            ShipsCount=new int[] 
            {
                Field.OnedeckShips,
                Field.TwodeckShips,
                Field.ThreedeckShips,
                Field.FourdeckShips
            };

            diagonal = (Diagonal)random.Next(0,2);
            searchMode = SearchMode.FourDeckSearch;         
        }

        protected override Ship MarkDrownedShip()
        {
           Ship ship = base.MarkDrownedShip();

            ChangeMode(ship);

            return ship;
        }

        void ChangeMode(Ship ship)
        { 
            int size = ship.Size;

            ShipsCount[size - 1]--;

            if ((ShipsCount[3] == 0) && (ShipsCount[2] == 0) && (ShipsCount[1] == 0))
            {
                searchMode = SearchMode.OneDeckSearch;
            }
            else if (ShipsCount[3] == 0)
            {
                searchMode = SearchMode.TwoDeckSearch;
            }
        }

        protected override void CountingIntactCell()
        {
            IntactCell = 0;

            foreach(var value in this)
            {
                if (!(bool)value)
                {
                    IntactCell++;
                }             
            }            
        }
        
        protected override Location OverrideShot(bool[,] Matrix, int shot)
        {
            int count = 0;

            Location newLocation = new Location();

            foreach(var value in this)
            {
                    bool chek = (bool)value;

                    if (!chek)
                    {
                        if (count == shot)
                        {
                            newLocation = location.GetLocation();
                        }
                        count++;                 
                }
            }
            return newLocation;
        }

        public IEnumerator GetEnumerator()
        {
            if (diagonal == Diagonal.Main)
            {
                location = new MainEnumerator(CheckShot);
            }
            else
            {
                location = new SideEnumerator(CheckShot);
            }

            return (IEnumerator)location;
        }
    }
}
