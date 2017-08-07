using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
     public class PlayerField:Field
    {
        public PlayerField():base()
        {
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    PictBox[i, j].MouseClick+=MouseEvent.MouseClicked;

                    PictBox[i, j].MouseEnter+=MouseEvent.MouseEnter;
                    PictBox[i, j].MouseLeave+=MouseEvent.MouseLeave;                
                }
            }
        }

        public void DisplayCompletionCell()
        {
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (MatrixShips[i, j])
                    {
                        PictBox[i, j].RenderingMode = CellCondition.Completion;
                    }
                    else
                    {
                        PictBox[i, j].RenderingMode = CellCondition.Empty;
                    }
                }
            }
            
        }

    }
}
