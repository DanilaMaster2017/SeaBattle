using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
    class shotEventArgs:EventArgs
    {
        public readonly CellCondition shotResult;

        public shotEventArgs(CellCondition result)
        {
            shotResult = result; 
        } 
    }
}
