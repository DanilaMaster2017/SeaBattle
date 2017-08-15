using System;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Media;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SeaColor = System.Drawing.Color;

namespace SeaBattleGame
{

    static public class GameController
    {
        class GameStatistika
        {
            public Label labelShip;
            public Label labelShot;
            public Label namePlayer;

            public int countWin { get; set; }
            int countLeftShot;
            int countShips;

            public GameStatistika(Label namePlayer, Label labelShip, Label labelShot)
            {
                this.labelShip = labelShip;
                this.labelShot = labelShot;
                this.namePlayer = namePlayer;

                countWin=0;
                countShips = Field.ShipsCount;
                countLeftShot = Field.Size * Field.Size;
            }

            public int CountShips
            {
                set
                {
                    countShips = value;
                    labelShip.Text = shipsWord + value;
                    if (countShips == 0)
                    {
                        GameOver(this);

                    }
                }
                get { return countShips; }
            }

            public int CountLeftShot
            {
                get { return countLeftShot; }
                set
                {
                    countLeftShot = value;
                    labelShot.Text = shotWord + countLeftShot;
                }
            }
        }

        static Player rightPlayer;
        static Player leftPlayer;

        static GameStatistika rightStatistika;
        static GameStatistika leftStatistika;

        static Random random = new Random(DateTime.Now.Millisecond);

        enum Side { Left, Right }

        public static bool EndedGame { get; set; }

        public static event EventHandler BeginGame;
        public static event EventHandler EndGame;
        public static event EventHandler BeforeGame;

        static string shipsWord = "Кораблей:";
        static string shotWord = "Выстрелов:";

        static Label score;

        static SeaColor moveColor = SeaColor.Gold;
        static SeaColor passColor = SeaColor.Black;

        static SeaColor winColor = SeaColor.Green;
        static SeaColor lossColor = SeaColor.Red;

        public static void GetLabel(Label rightShips, Label rightShot, Label rightName,
            Label leftShips, Label leftShot, Label leftName, Label score)
        {
            leftStatistika = new GameStatistika(leftName, leftShips, leftShot);
            rightStatistika = new GameStatistika(rightName, rightShips, rightShot);

            GameController.score = score;

            Init();
        }

        static void Init()
        {
            Form1.RightField.MadeShot += Made_Shot;
            Form1.LeftField.MadeShot += Made_Shot;
        }

        public static void Begin_Clicked(object sender, EventArgs e)
        {
            EnabledSwitch(Form1.LeftField.CellField, false);

            if (MouseEvent.BeginArround) MouseEvent.EndedArround();     

            BeforeSound.Pause();

            EndedGame = false;
            BeginGame(null, EventArgs.Empty);

            leftPlayer = Form1.LeftPlayer;
            rightPlayer = Form1.RightPlayer;

            rightPlayer.Oponent = leftPlayer;
            leftPlayer.Oponent = rightPlayer;

            rightPlayer.TransferMove += Transfer_Move;
            leftPlayer.TransferMove += Transfer_Move;          

            Side OneMove = (Side)random.Next(0, 2);

            if (OneMove == Side.Right)
            {
                Transfer_Move(leftPlayer, EventArgs.Empty);
            }
            else
            {
                Transfer_Move(rightPlayer, EventArgs.Empty);
            }

            EnabledSwitch(Form1.RightField.CellField, true);
        }        

        public static void Ok_Clicked(object sender, EventArgs e)
        {
            BeforeGame(null, EventArgs.Empty);
            EnabledSwitch(Form1.LeftField.CellField, true);

            foreach (var value in Form1.RightField.CellField)
                value.RenderingMode = CellCondition.Empty;

            Form1.LeftField.RandomShips.RandomArrangement();
            Form1.LeftField.DisplayCompletionCell();

            Form1.RightField.RandomShips.RandomArrangement();

            resetStatistika(rightStatistika);
            resetStatistika(leftStatistika);

            EndedSound.Stop();
            BeforeSound.Play();
        }

        static void resetStatistika(GameStatistika statistika)
        {
            statistika.CountShips = Field.ShipsCount;   
            statistika.CountLeftShot = Field.Size * Field.Size;
            statistika.namePlayer.ForeColor = passColor;
        }

        static void Transfer_Move(object sender, EventArgs e)
        {             
            Player player = (Player)sender;

            if (player == rightPlayer)
            {
                rightStatistika.namePlayer.ForeColor = passColor;
                leftStatistika.namePlayer.ForeColor = moveColor;
            }
            else
            {
                leftStatistika.namePlayer.ForeColor = passColor;
                rightStatistika.namePlayer.ForeColor = moveColor;
            }

            Refresh();

            player.Oponent.Move();           
        }

        static void Refresh()
        {
            rightStatistika.namePlayer.Invalidate();
            leftStatistika.namePlayer.Invalidate();
            leftStatistika.namePlayer.Update();
            rightStatistika.namePlayer.Update();
            Thread.Sleep(100);
        }

        static void Made_Shot(object sender, EventArgs e)
        {
            Field field = (Field)sender;

            if (field == Form1.RightField)
            {
                leftStatistika.CountLeftShot--;
            }
            else
            {
                rightStatistika.CountLeftShot--;
            }

            CellCondition result = ((shotEventArgs)e).shotResult;

            if (result == CellCondition.Drowned)
            {
                if (field == Form1.RightField)
                {
                    rightStatistika.CountShips--;
                }
                else
                {
                    leftStatistika.CountShips--;
                }
            }

            if (soundPlay)
            {
                ShotSound.PlaySync();

                if (result == CellCondition.Miss) MissSound.PlaySync();
                else
                if (result == CellCondition.Crippled) CrippledSound.PlaySync();
                else
                if (result == CellCondition.Drowned) DrownedShipsSound.PlaySync();
            }
        }

        static void GameOver(GameStatistika lossStatistika)
        {
            EndedGame = true;
            EnabledSwitch(Form1.RightField.CellField,false);

            GameStatistika winStatistika;

            if (lossStatistika.Equals(rightStatistika))
            {
                winStatistika = leftStatistika;
                PlaysSound(WinSound);
            }
            else
            {
                winStatistika = rightStatistika;
                PlaysSound(LoseSound);
            }

            winStatistika.countWin++;
            winStatistika.namePlayer.ForeColor = winColor;

            lossStatistika.namePlayer.ForeColor = lossColor;          

            EndGame(null, EventArgs.Empty);

            score.Text = string.Format("{0}:{1}", leftStatistika.countWin, rightStatistika.countWin);

            rightPlayer.TransferMove -= Transfer_Move;
            leftPlayer.TransferMove -= Transfer_Move;
        }

        static void EnabledSwitch(SeaBattlePicture[,] Matrix, bool value)
        {
            for (var i = 0; i < Field.Size; i++)
            {
                for (var j = 0; j < Field.Size; j++)
                {
                    Matrix[i, j].Enabled = value;
                }
            }
        }

        #region music

        static bool soundPlay = false;

        static string path = Environment.CurrentDirectory + "\\PlaySound\\";

        public static MediaPlayer BeforeSound = new MediaPlayer();
        static MediaPlayer EndedSound = new MediaPlayer();
        static MediaPlayer WinSound = new MediaPlayer();
        static MediaPlayer LoseSound = new MediaPlayer();

        static SoundPlayer ShotSound;
        static SoundPlayer MissSound;
        static SoundPlayer CrippledSound;
        static SoundPlayer DrownedShipsSound;

        static GameController()
        {
            ShotSound = new SoundPlayer(path + "shot.wav");
            MissSound = new SoundPlayer(path + "miss.wav");
            CrippledSound = new SoundPlayer(path + "crip.wav");
            DrownedShipsSound = new SoundPlayer(path + "drownShips.wav");

            BeforeSound.Open(new Uri(path + "pirates.mp3", UriKind.RelativeOrAbsolute));
            EndedSound.Open(new Uri(path + "seaDemons.mp3", UriKind.RelativeOrAbsolute));
            WinSound.Open(new Uri(path + "winner.mp3", UriKind.RelativeOrAbsolute)); 
            LoseSound.Open(new Uri(path + "loss.mp3", UriKind.RelativeOrAbsolute));

            SoundSwitch(0);

            WinSound.MediaEnded += EndedSoundPlay;
            LoseSound.MediaEnded += EndedSoundPlay;

            BeforeSound.MediaEnded += BeforeSound_MediaEnded;       
        }

        private static void BeforeSound_MediaEnded(object sender, EventArgs e)
        {
            PlaysSound(BeforeSound);
        }

        private static void EndedSoundPlay(object sender, EventArgs e)
        {
            PlaysSound(EndedSound);
        }

        public static void SoundButtonClicked(object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox)sender;
            soundPlay = !soundPlay;

            if (soundPlay)
            {
                picture.BackgroundImage = Properties.Resources.PlaySound;
                SoundSwitch(1);
            }
            else
            {
                picture.BackgroundImage = Properties.Resources.StopSound;
                SoundSwitch(0);
            }
        }

        static void SoundSwitch(double volume)
        {
            BeforeSound.Volume = volume;
            EndedSound.Volume = volume;
            WinSound.Volume = volume;
            LoseSound.Volume = volume;
        }

        static void PlaysSound(MediaPlayer sound)
        {
            sound.Position = new TimeSpan(0);
            sound.Play();
        }
        #endregion
    }
}
