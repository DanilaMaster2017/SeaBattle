using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattleGame
{
    public partial class Form1 : Form
    {
        public static Field RightField { get; set; }
        public static Field LeftField { get; set; }

        public static Player RightPlayer { get; set; }
        public static Player LeftPlayer { get; set; }

        static ComboBox comboBox;

        public Form1()
        {
            InitializeComponent();

            LeftField = new Field();
            RightField = new Field();

            LeftPlayer = new HumanPlay(LeftField);

            GameController.GetLabel(label6, label7, label2, label4, label5, label1, label3);
         
            pictureBox1.Click += GameController.SoundButtonClicked;

            comboBox1.SelectedItem = comboBox1.Items[0];
            comboBox = comboBox1;

            GameController.BeforeGame += BeforeGame;
            GameController.BeginGame += BeginGame;
            GameController.EndGame += EndGame;

            MouseWheel += MouseEvent.MouseWheel;

            button1.Click += GameController.Begin_Clicked;
            button3.Click += GameController.Ok_Clicked;

            button2.Click += LeftField.RandomShips.RandomClicked;

            FillTable(tableLayoutPanel1, LeftField.CellField);
            FillTable(tableLayoutPanel2, RightField.CellField);
            LeftField.DisplayCompletionCell();
        }

        void FillTable(TableLayoutPanel table, SeaBattlePicture[,] PictBox)
        {
             int sizeCell = table.Width / table.ColumnCount;

             for (var i = 0; i < Field.Size; i++)
             {
                  for (var j = 0; j < Field.Size; j++)
                  { 
                       PictBox[i, j].Height = RightField.CellField[i, j].Width = sizeCell;
                       PictBox[i, j].Margin = new Padding(1, 1, 1, 1);
                       PictBox[i, j].RenderingMode = CellCondition.Empty;
                       table.Controls.Add(PictBox[i, j], j,i );
                  }
             }
        }

        void BeforeGame(object sender, EventArgs e)
        {
            button3.Visible = false;

            button1.Visible = true;
            button2.Visible = true;

            panel1.Visible = true;
        }

        void BeginGame(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;

            panel1.Visible = false;

            int complexity = GetComplexity();

            if (complexity == 0)
            {
                RightPlayer = new ComputerPlay(RightField);
            }
            else
            {
                RightPlayer = new OptimalComputerPlay(RightField);
            }

            RightPlayer.Oponent = LeftPlayer;
            LeftPlayer.Oponent = RightPlayer;

            RightPlayer.TransferMove += GameController.Transfer_Move;
            LeftPlayer.TransferMove += GameController.Transfer_Move;
        }

        void EndGame(object sender, EventArgs e)
        {
            RightPlayer.TransferMove -= GameController.Transfer_Move;
            LeftPlayer.TransferMove -= GameController.Transfer_Move;

            button3.Visible = true;
        }

        public static int GetComplexity()
        {
            return (comboBox).SelectedIndex;
        }

        void ButtonEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            button.BackColor = Color.Blue;
            button.ForeColor = Color.Gold;
        }

        void ButtonLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            button.BackColor = Color.DodgerBlue;
            button.ForeColor = Color.White;
        }
    }
}
