using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattleGame
{
    public class MouseEvent
    {
        static PrototypeShips protoShips = null;
        static Ships ships = null;

        public static bool BeginArround { get; set; } = false;
        static bool checkValidLocation = false;

        public static void MouseClicked(object sender, MouseEventArgs e)
        {          
                SeaBattlePicture cell = (SeaBattlePicture)sender;            

            if (!BeginArround)
            {
                if (cell.Affiliation == null) return;
                BeginArround = true;

                ships = cell.Affiliation;
                ships.Destruction();
                Form1.PlayerField.randomShips.RecalculationCompletingCell(ships);

                Location endLocation = new Location(cell.PictureLocation.IndexI - (1 - (int)ships.ShipsOrientation),
                    cell.PictureLocation.IndexJ - (int)ships.ShipsOrientation);
                DisplayFildPlace(ships.ShipsLocation,endLocation);

                protoShips = new PrototypeShips(cell.PictureLocation, ships.ShipsOrientation, ships.Size);
                checkValidLocation = Form1.PlayerField.randomShips.CheckLocation(protoShips, protoShips.ShipsLocation);
                DisplayPrototypeShip();

            }
            else
            {
                BeginArround = false;

                Ships newShips;

                checkValidLocation = Form1.PlayerField.randomShips.CheckLocation(protoShips, protoShips.ShipsLocation);
                if (checkValidLocation)
                {
                    newShips = new Ships(protoShips.ShipsLocation, protoShips.ShipsOrientation,
                        protoShips.Size, Form1.PlayerField);
                }
                else
                {
                    newShips = new Ships(ships);

                    Location endedLocation = new Location(
               cell.PictureLocation.IndexI + (1 - (int)protoShips.ShipsOrientation) * (protoShips.Size - 1),
               cell.PictureLocation.IndexJ + (int)protoShips.ShipsOrientation * (protoShips.Size - 1));

                    DisplayFildPlace(cell.PictureLocation, endedLocation);
                }

                Form1.PlayerField.randomShips.AddShips(newShips);

                Location endLocation = new Location(newShips.ShipsLocation.IndexI + 
                    (1 - (int)newShips.ShipsOrientation) * (newShips.Size - 1),
                    newShips.ShipsLocation.IndexJ + (int)newShips.ShipsOrientation * (newShips.Size - 1));

                DisplayFildPlace(newShips.ShipsLocation, endLocation);

                ships = null;
            }
           
        }

        public static void MouseEnter(object sender, EventArgs e)
        {
            if (ships == null) return;

            SeaBattlePicture cell = (SeaBattlePicture)sender;

            protoShips.ShipsLocation = new Location(cell.PictureLocation.IndexI,
                cell.PictureLocation.IndexJ);

            checkValidLocation = Form1.PlayerField.randomShips.CheckLocation(protoShips, protoShips.ShipsLocation);
            DisplayPrototypeShip();
        }

        public static void MouseLeave(object sender, EventArgs e)
        {
            if (ships == null) return;

            SeaBattlePicture cell = (SeaBattlePicture)sender;

            Location endLocation = new Location(
                cell.PictureLocation.IndexI + ( 1 - (int)protoShips.ShipsOrientation) * (protoShips.Size - 1),
                cell.PictureLocation.IndexJ + (int)protoShips.ShipsOrientation * (protoShips.Size - 1));

            DisplayFildPlace(cell.PictureLocation, endLocation);            
        }

        public static void MouseWheel(object sender, MouseEventArgs e)
        {
            if (protoShips == null) return;

            if (e.Delta > 0)
            {
                if (protoShips.ShipsOrientation == Orientation.Horizontal) return;
                protoShips.ShipsOrientation = Orientation.Horizontal;
            }
            else
            {
                if (protoShips.ShipsOrientation == Orientation.Vertical) return;
                protoShips.ShipsOrientation = Orientation.Vertical;
            }

            Location beginLocation = new Location(
                protoShips.ShipsLocation.IndexI + (int)protoShips.ShipsOrientation,
                protoShips.ShipsLocation.IndexJ + (1 - (int)protoShips.ShipsOrientation));

            Location endLocation = new Location(
                protoShips.ShipsLocation.IndexI + (int)protoShips.ShipsOrientation * (protoShips.Size - 1),
                protoShips.ShipsLocation.IndexJ + (1 - (int)protoShips.ShipsOrientation) * (protoShips.Size - 1));

            DisplayFildPlace(beginLocation,endLocation);

            checkValidLocation = Form1.PlayerField.randomShips.CheckLocation(protoShips, protoShips.ShipsLocation);
            DisplayPrototypeShip();
        }

        static void DisplayPrototypeShip()
        {
            CellCondition condition;

            if (checkValidLocation)
            {
                condition = CellCondition.ValidLocation;
            }
            else
            {
                condition = CellCondition.NotValidLocation;
            }

            int indexI;
            int indexJ;
             
            for (var i=0; i<protoShips.Size; i++)
            {
                indexI = protoShips.ShipsLocation.IndexI + (1 - (int)protoShips.ShipsOrientation) * i;
                indexJ = protoShips.ShipsLocation.IndexJ + (int)protoShips.ShipsOrientation * i;

                if (GeneralStaticFunction.PreventionIndexRange(indexI, indexJ))
                {
                    Form1.PlayerField.PictBox[indexI, indexJ].RenderingMode = condition;
                }
            }
        }

        static void DisplayFildPlace (Location begLocation, Location endLocation)
        {
            CellCondition condition;
            for (var i = begLocation.IndexI; i <= endLocation.IndexI; i++)
            {
                for (var j = begLocation.IndexJ; j <= endLocation.IndexJ; j++)
                {
                    if (GeneralStaticFunction.PreventionIndexRange(i, j))
                    { 
                        if (Form1.PlayerField.MatrixShips[i, j])
                        {
                            condition = CellCondition.Completion;
                        }
                        else
                        {
                            condition = CellCondition.Empty;
                        }
                        Form1.PlayerField.PictBox[i, j].RenderingMode = condition;
                    }
                }                     
            }
        }

        public static void EndedArround()
        {
            BeginArround = false;

            Ships  newShips = new Ships(ships);

            Form1.PlayerField.randomShips.AddShips(newShips);

            Location endLocation = new Location(newShips.ShipsLocation.IndexI + (1 - (int)newShips.ShipsOrientation) * (newShips.Size - 1),
                newShips.ShipsLocation.IndexJ + (int)newShips.ShipsOrientation * (newShips.Size - 1));
            DisplayFildPlace(newShips.ShipsLocation, endLocation);

            ships = null;
        }

        public static void ButtonEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            button.BackColor = System.Drawing.Color.Blue;
            button.ForeColor = System.Drawing.Color.Gold;
        }

        public static void ButtonLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            button.BackColor = System.Drawing.Color.DodgerBlue;
            button.ForeColor = System.Drawing.Color.White;
        }
    }
}
