using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
    enum SearchMode {OneDeckSearch=1, TwoDeckSearch=2, FourDeckSearch=4}

    enum Diagonal {Main, Side}

    public class OptimalComputerPlay:ComputerPlay
    {
        SearchMode searchMode;

        Random random=new Random();
        Diagonal diagonal;

        int[] ShipsCount;

        public OptimalComputerPlay():base()
        {
            ShipsCount=new int[] { Field.OnedeckShips, Field.TwodeckShips, Field.ThreedeckShips, Field.FourdeckShips };
            diagonal = (Diagonal)random.Next(0,2);
            searchMode = SearchMode.FourDeckSearch;
            GameController.ShipsDrown += ChangeMode;  
        }

        void ChangeMode(object sender, EventArgs e)
        {
            int size = ((Ships)sender).Size;

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
            if (diagonal == Diagonal.Main) MCountingIntactCell();
            else SCountingIntactCell();
        }

        void MCountingIntactCell()
        {
            IntactCell = 0;

            int begin = 0;
            for (var i = -1 + (int)searchMode; i <= 2 * (Field.Size - 1); i += (int)searchMode)
            {
                if (i > (Field.Size - 1)) begin = i - (Field.Size - 1);

                for (var j = begin; ((i - j >= 0) && (j < Field.Size)); j++)
                {
                    if (!CheckShot[j, i - j]) IntactCell++;
                }
            }            
        }

        void SCountingIntactCell()
        {
            int begin = 0;
            IntactCell = 0;

            for (var i = Field.Size - (int)searchMode; i > -Field.Size; i -= (int)searchMode)
            {
                if (i<0) begin = -i;

                for (var j = begin; ((i + j  < Field.Size) && (j < Field.Size)); j++)
                {
                    if (!CheckShot[j, i + j]) IntactCell++;
                }
            }
        }

        protected override Location OverrideShot(bool[,] CheckShot, int shot)
        {
            if (diagonal == Diagonal.Main) return MFromShotLocation(CheckShot, shot);
            else return SFromShotLocation(CheckShot, shot); 
        }


        Location SFromShotLocation(bool[,] Matrix, int shot)
        {  
            int count = 0;

            Location newLocation = new Location();
            int begin = 0;

            for (var i = Field.Size - (int)searchMode; i > -Field.Size; i -= (int)searchMode)
            {
                if (i < 0) begin = -i;

                for (var j = begin; ((i + j < Field.Size) && (j < Field.Size)); j++)
                {
                   if (!Matrix[j, i + j])
                    {
                        if (count == shot)
                        {
                            Matrix[j, i + j] = true;
                            newLocation.IndexI = j;
                            newLocation.IndexJ = i + j;
                        }
                        count++;
                    } 
                }
            }
            return newLocation;
        }

        Location MFromShotLocation(bool[,] Matrix, int shot)
        {
            int count = 0;

            Location newLocation = new Location();

            int begin = 0;
            for (var i = -1 + (int)searchMode; i <= 2 * (Field.Size - 1); i += (int)searchMode)
            {
                if (i > (Field.Size - 1)) begin = i - (Field.Size - 1);

                for (var j = begin; ((i - j >= 0) && (j < Field.Size)); j++)
                {
                    if (!Matrix[j, i - j])
                    {
                        if (count == shot)
                        {
                            Matrix[j, i - j] = true;
                            newLocation.IndexI = j;
                            newLocation.IndexJ = i - j;
                        }
                        count++;
                    }
                }
            }

            return newLocation;
        }
    }
}
