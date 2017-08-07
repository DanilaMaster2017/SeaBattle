using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleGame
{
    public class RandomShips
    {
        static Random random = new Random();

        public List<Ships> ListShips = new List<Ships>();

        bool[,] completingCell = new bool [Field.Size,Field.Size];
        bool[,] notvalidCellLocation = new bool[Field.Size,Field.Size];

        Field field;

        static int validCell;

        PrototypeShips prototype=new PrototypeShips();
        
        int oneDimencional;


        public RandomShips(Field field)
        {
            this.field = field;
            NewArrangement();
        }

        void NewArrangement()
        {
            GeneralStaticFunction.FalseToMatrix(completingCell);

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < i + 1; j++)
                {               
                    prototype.ShipsOrientation = (Orientation)random.Next(0, 2);
                    prototype.Size = 4 - i;

                    CountingValidCell(prototype);
                    oneDimencional = random.Next(0, validCell);

                    prototype.ShipsLocation =
                        GeneralStaticFunction.FromNumberLocation(notvalidCellLocation, oneDimencional);
               
                    ListShips.Add(new Ships(prototype.ShipsLocation, prototype.ShipsOrientation,
                        prototype.Size, field));
                    ListShips[ListShips.Count - 1].MarkShips(completingCell);
                }
            }
        }

        void CountingValidCell(PrototypeShips prototype)
        {
            GeneralStaticFunction.FalseToMatrix(notvalidCellLocation);

            validCell = 0;
            for (var i = 0; i < Field.Size; i++)
            {
                for (var j = 0; j < Field.Size; j++)
                {
                    if (CheckLocation(prototype, new Location(i, j))) validCell++;
                }
            }
        }

        public bool CheckLocation(PrototypeShips prototype, Location location)
        {
            int dj = 0;
            int di = 0;

            if (prototype.ShipsOrientation == Orientation.Horizontal) dj = 1;
            if (prototype.ShipsOrientation == Orientation.Vertical) di = 1;

            for (var i = 0; i<prototype.Size; i++)
            {
                if ((!GeneralStaticFunction.PreventionIndexRange
                    (location.IndexI + i * di, location.IndexJ + i * dj)) ||
                    (completingCell[location.IndexI + i * di, location.IndexJ + i * dj]))
                    {
                        notvalidCellLocation[location.IndexI, location.IndexJ] = true;
                        return false;
                    }
            }

            return true;
        }

        public void RecalculationCompletingCell(Ships ships)
        {
            ListShips.Remove(ships);
            GeneralStaticFunction.FalseToMatrix(completingCell);

            foreach (var value in ListShips) value.MarkShips(completingCell);            
        }

        public void AddShips(Ships ships)
        {
            ListShips.Add(ships);
            ships.MarkShips(completingCell);
        }

        public void RandomArrangement()
        {

            foreach (var value in field.randomShips.ListShips)
                value.Destruction();

            field.randomShips.ListShips.Clear();

            NewArrangement();
        }

        public void RandomClicked(object sender, EventArgs e)
        {
            RandomArrangement();
            Form1.PlayerField.DisplayCompletionCell();
        }
    }
}
