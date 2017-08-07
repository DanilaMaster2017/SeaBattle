using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SeaBattleGame
{
   public class Field
   {
        static public int Size { get; set; } = 10;
       
        public SeaBattlePicture[,] PictBox;

        public bool[,] MatrixShips { get; set; }

        

        public static int FourdeckShips { get; } = 1;
        public static int ThreedeckShips { get; } = 2;
        public static int TwodeckShips { get; } = 3;
        public static int OnedeckShips { get; } = 4;

        public static int ShipsCount { get; } = FourdeckShips + ThreedeckShips + TwodeckShips + OnedeckShips;

        // public List<Ships> ListShips=new List<Ships>();

        public RandomShips randomShips;

        public Field()
        {
            MatrixShips = new bool[Size, Size];
            MatrixShips.Initialize();

            
            PictBox = new SeaBattlePicture[Size, Size];

            for (var  i = 0; i<Size ; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    PictBox[i, j] = new SeaBattlePicture();
                    PictBox[i, j].PictureLocation = new Location(i, j);
                    MatrixShips[i, j] = false;                 
                }
            }

            FillingShips();   
        }

        void FillingShips()
        {
            /*
            ListShips.Add(new Ships(new Location(4,2),Orientation.Horizontal,4, this));
            ListShips.Add(new Ships(new Location(7, 0), Orientation.Horizontal, 3, this));
            ListShips.Add(new Ships(new Location(0, 6), Orientation.Vertical, 3, this));
            ListShips.Add(new Ships(new Location(1, 1), Orientation.Vertical, 2, this));
            ListShips.Add(new Ships(new Location(4, 8), Orientation.Horizontal, 2, this));
            ListShips.Add(new Ships(new Location(6, 7), Orientation.Vertical, 2, this));
            ListShips.Add(new Ships(new Location(5, 0), Orientation.Horizontal, 1, this));
            ListShips.Add(new Ships(new Location(6, 4), Orientation.Horizontal, 1, this));
            ListShips.Add(new Ships(new Location(8, 5), Orientation.Horizontal, 1, this));
            ListShips.Add(new Ships(new Location(9, 2), Orientation.Horizontal, 1, this));
            */
            randomShips = new RandomShips(this);

        }

        void DrownedShips(Ships ships)
        {
            Location location = ships.ShipsLocation;
            Orientation orientation = ships.ShipsOrientation;
            int size = ships.Size;

            if (this is PlayerField) ships.MarkShips(ComputerPlay.CheckShot);

            for (var i = 0; i < size; i++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    PictBox[location.IndexI, location.IndexJ+i].RenderingMode = CellCondition.Drowned;
                }
                else
                {
                    PictBox[location.IndexI+i, location.IndexJ].RenderingMode = CellCondition.Drowned;
                }
            }
        }

        

        public CellCondition Shot(SeaBattlePicture picture)
        {
            if (MatrixShips[picture.PictureLocation.IndexI, picture.PictureLocation.IndexJ])
            {
                if (picture.Affiliation.CheckDrowned(picture.PictureLocation))
                {
                    DrownedShips(picture.Affiliation);
                    GameController.ShipsDrowned(picture.Affiliation);
                }
                else
                {
                    picture.RenderingMode = CellCondition.Crippled;
                }
            }
            else
            {
                picture.RenderingMode = CellCondition.Miss;
            }

            GameController.ShotUpdater(this,picture.RenderingMode);
            return picture.RenderingMode;
        }  
   }
}

