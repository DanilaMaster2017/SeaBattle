using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattleGame
{
    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public struct Location
    {
        public int IndexI
        {
            get; set;
        }

        public int IndexJ
        {
            get;set;
        }

        public Location(int i,int j)
        {
            IndexI = i;
            IndexJ = j;
        }

    }

    public class Ships : PrototypeShips
    {
        bool[] crippled;

        public Field Field { get; set; }

        Location[] deckLocation;

        public Ships(Location location, Orientation orientation, int size, Field field) :
            base(location, orientation, size)
        {
            Field = field;

            crippled = new bool[Size];
            crippled.Initialize();

            deckLocation = new Location[Size];

            for (var i = 0; i < Size; i++)
            {
                if (ShipsOrientation == Orientation.Vertical)
                {
                    Field.MatrixShips[ShipsLocation.IndexI + i, ShipsLocation.IndexJ] = true;
                    Field.PictBox[ShipsLocation.IndexI + i, ShipsLocation.IndexJ].Affiliation = this;
                    deckLocation[i] =
                         new Location(ShipsLocation.IndexI + i, ShipsLocation.IndexJ);
                }
                else
                {
                    Field.MatrixShips[ShipsLocation.IndexI, ShipsLocation.IndexJ + i] = true;
                    Field.PictBox[ShipsLocation.IndexI, ShipsLocation.IndexJ + i].Affiliation = this;
                    deckLocation[i] =
                         new Location(ShipsLocation.IndexI, ShipsLocation.IndexJ + i);
                }
            }
        }

        public Ships(Ships ships):
            this(ships.ShipsLocation, ships.ShipsOrientation, ships.Size, ships.Field)
        {
          
        }

        public void Destruction()
        {
            for (var i = 0; i < Size; i++)
            {
                if (ShipsOrientation == Orientation.Vertical)
                {
                    Field.MatrixShips[ShipsLocation.IndexI + i, ShipsLocation.IndexJ] = false;
                    Field.PictBox[ShipsLocation.IndexI + i, ShipsLocation.IndexJ].Affiliation = null;
                }
                else
                {
                    Field.MatrixShips[ShipsLocation.IndexI, ShipsLocation.IndexJ + i] = false;
                    Field.PictBox[ShipsLocation.IndexI, ShipsLocation.IndexJ + i].Affiliation = null;
                }
            }
        }

        public bool CheckDrowned(Location location)
        {
            bool check = true;

            for (var i = 0; i < Size; i++)
            {
                if ((location.IndexI == deckLocation[i].IndexI) &&
                    (location.IndexJ == deckLocation[i].IndexJ))
                { crippled[i] = true; break; }
            }

            foreach (var value in crippled)
            {
                if (!value) check = false;
            }

            return check;
        }

        public bool ChekShips(Location location)
        {
            bool check = false;

            for (var i = 0; i < Size; i++)
            {
                if ((location.IndexI == deckLocation[i].IndexI) &&
                    (location.IndexJ == deckLocation[i].IndexJ))
                { check = true; break; }
            }

            return check;
        }

        public void MarkShips(bool[,] MatrixSips)
        {
            for (var i = deckLocation[0].IndexI - 1; i <= deckLocation[Size - 1].IndexI + 1; i++)
            {
                for (var j = deckLocation[0].IndexJ - 1; j <= deckLocation[Size - 1].IndexJ + 1; j++)
                {
                    if (GeneralStaticFunction.PreventionIndexRange(i, j)) MatrixSips[i, j] = true;
                }
            }
        }
      
    }
}
