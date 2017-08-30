using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
    class MainEnumerator : IEnumerator, IGetLocation
    {
        int i;
        int j;
        int begin;

        OptimalComputerPlay player;
        bool[,] Matrix;

        public MainEnumerator(bool[,] Matrix)
        {
            player = ((OptimalComputerPlay)Form1.RightPlayer);
            this.Matrix = Matrix;

            Reset();
        }

        public bool MoveNext()
        {
            bool result = true;
            j++;

            if (!((i - j >= 0) && (j < Field.Size)))
                result = UpdateI();

            return result;
        }

        public void Reset()
        {
            i = -1;
            j = Field.Size;
            begin = 0;
        }

        public object Current
        {
            get { return Matrix[j, i - j]; }
        }

        bool UpdateI()
        {
            i += (int)player.SearchMod;

            if (i > (Field.Size - 1)) begin = i - (Field.Size - 1);
            j = begin;

            return (i <= 2 * (Field.Size - 1));
        }

        public Location GetLocation()
        {
            Matrix[j, i - j] = true;
            return new Location(j, i - j);
        }

        /*
        void MainCountingIntactCell()
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
        */
    }
}
