using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattleGame
{

    public class ComputerField:Field
    {
        public ComputerPlay computerPlayer { get; set; }  

        public ComputerField():base()
        {
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    PictBox[i, j].MouseClick += PictureBox_Click;
                    PictBox[i, j].Enabled = false;
                }
            }
        }

        void PictureBox_Click(object sender, MouseEventArgs e)
        {
            SeaBattlePicture picture = (SeaBattlePicture)sender;
            picture.Enabled = false;

          //  GameController.ShotPlayer();

            CellCondition shotResult = Form1.ComputerField.Shot(picture);

            if (shotResult == CellCondition.Miss)
            {
                computerPlayer.Move();
            }
        }


    }
}
