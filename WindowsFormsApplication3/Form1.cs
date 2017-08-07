﻿using System;
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
        public static  ComputerField ComputerField { get; set; }
        public static PlayerField PlayerField { get; set; }

        static ComboBox comboBox;

        public Form1()
        {
            InitializeComponent();

            GameController.BeforeSound.Play();

            pictureBox1.Click += GameController.SoundButtonClicked;

            comboBox1.SelectedItem = comboBox1.Items[0];
            comboBox = comboBox1;

            GameController.BeforeGame += BeforeGame;
            GameController.BeginGame += BeginGame;
            GameController.EndGame += EndGame;

            GameController.GetLabel(label6, label7, label2, label4, label5, label1, label3);

            MouseWheel += MouseEvent.MouseWheel;

            button1.Click += GameController.Begin_Clicked;
            button3.Click += GameController.Ok_Clicked;

            PlayerField = new PlayerField();
            ComputerField = new ComputerField();

            button2.Click += PlayerField.randomShips.RandomClicked;

            button1.MouseEnter += MouseEvent.ButtonEnter;
            button2.MouseEnter += MouseEvent.ButtonEnter;
            button3.MouseEnter += MouseEvent.ButtonEnter;

            button1.MouseLeave += MouseEvent.ButtonLeave;
            button2.MouseLeave += MouseEvent.ButtonLeave;
            button3.MouseLeave += MouseEvent.ButtonLeave;

            FillTable(tableLayoutPanel1, PlayerField.PictBox);
            FillTable(tableLayoutPanel2, ComputerField.PictBox);
            PlayerField.DisplayCompletionCell();
            //   MessageBox.Show(field.PictBox[i, j].Margin.All.ToString());

        }

        void FillTable(TableLayoutPanel table, SeaBattlePicture[,] PictBox)
        {
             int sizeCell = tableLayoutPanel2.Width / tableLayoutPanel2.ColumnCount;

             for (var i = 0; i < Field.Size; i++)
             {
                  for (var j = 0; j < Field.Size; j++)
                  {

            PictBox[i, j].Height = ComputerField.PictBox[i, j].Width = sizeCell;
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
        }

        void EndGame(object sender, EventArgs e)
        {
            button3.Visible = true;
        }

        public static int GetComplexity()
        {
            return (comboBox).SelectedIndex;
        }
    }
}
