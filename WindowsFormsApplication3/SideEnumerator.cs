using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
    class SideEnumerator : IEnumerator, IGetLocation
    {
        int i;
        int j;
        int begin;

        OptimalComputerPlay player;
        bool[,] Matrix;

        public SideEnumerator(bool[,] Matrix)
        {
            player = ((OptimalComputerPlay)Form1.RightPlayer);
            this.Matrix = Matrix;

            Reset();
        }

        public bool MoveNext()
        {
            bool result = true;
            j++;

            if (!((i + j < Field.Size) && (j < Field.Size)))
                result = UpdateI();

            return result;
        }

        public void Reset()
        {
            i = Field.Size;
            j = Field.Size;
            begin = 0;
        }

        public object Current
        {
            get { return Matrix[j, i + j]; }
        }

        bool UpdateI()
        {
            i -= (int)player.SearchMod;

            if (i < 0) begin = -i;
            j = begin;

            return (i > -Field.Size);
        }

        public Location GetLocation()
        {
            Matrix[j, i + j] = true;
            return new Location(j, i + j);
        }
        /*
         void SideCountingIntactCell()
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
         */
    }
}
