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
            PlaysSound(BeforeSound);

            Form1.RightField.MadeShot += Made_Shot;
            Form1.LeftField.MadeShot += Made_Shot;
        }

        public static void Begin_Clicked(object sender, EventArgs e)
        {
            EnabledSwitch(Form1.LeftField.CellField, false);

            if (MouseEvent.BeginArround) MouseEvent.EndedArround();

            BackgroundMusic.Stop();

            EndedGame = false;
            BeginGame(null, EventArgs.Empty);

            leftPlayer = Form1.LeftPlayer;
            rightPlayer = Form1.RightPlayer;         

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

            BackgroundMusic.Stop();
            PlaysSound(BeforeSound);
        }

        static void resetStatistika(GameStatistika statistika)
        {
            statistika.CountShips = Field.ShipsCount;   
            statistika.CountLeftShot = Field.Size * Field.Size;
            statistika.namePlayer.ForeColor = passColor;
        }

        public static void Transfer_Move(object sender, EventArgs e)
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
                PlaysEffect(ShotSound);

                if (result == CellCondition.Miss) PlaysEffect(MissSound);
                else
                if (result == CellCondition.Crippled) PlaysEffect(CrippledSound);
                else
                if (result == CellCondition.Drowned) PlaysEffect(DrownedShipsSound);
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
                PlaysSound(LossSound);
            }

            winStatistika.countWin++;
            winStatistika.namePlayer.ForeColor = winColor;

            lossStatistika.namePlayer.ForeColor = lossColor;          

            EndGame(null, EventArgs.Empty);

            score.Text = string.Format("{0}:{1}", leftStatistika.countWin, rightStatistika.countWin);
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

        static MediaPlayer BackgroundMusic = new MediaPlayer();
        static SoundPlayer SoundEffect = new SoundPlayer();

        static string BeforeSound = "pirates.mp3";
        static string EndedSound = "seaDemons.mp3";
        static string WinSound = "winner.mp3";
        static string LossSound = "loss.mp3";

        static string ShotSound = "shot.wav";
        static string MissSound = "miss.wav";
        static string CrippledSound = "crip.wav";
        static string DrownedShipsSound = "drownShips.wav";        

        static GameController()
        {
            SoundSwitch(0);

            BackgroundMusic.MediaEnded += EndedSoundPlay;     
        }

        private static void EndedSoundPlay(object sender, EventArgs e)
        {
            if (BackgroundMusic.Source.AbsolutePath.Contains(BeforeSound))
            {
                PlaysSound(BeforeSound);
            }
            else
            {
                PlaysSound(EndedSound);
            }
        }

        public static void SoundButtonClicked(object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox)sender;
            soundPlay = !soundPlay;

            if (soundPlay)
            {
                picture.BackgroundImage = Properties.Resources.PlaySound;
                SoundSwitch(0.5);
            }
            else
            {
                picture.BackgroundImage = Properties.Resources.StopSound;
                SoundSwitch(0);
            }
        }

        static void SoundSwitch(double volume)
        {
            BackgroundMusic.Volume = volume;
        }

        static void PlaysSound(string sound)
        {
            BackgroundMusic.Open(new Uri(path + sound, UriKind.RelativeOrAbsolute));
            BackgroundMusic.Play();
        }

        static void PlaysEffect(string sound)
        {
            SoundEffect.SoundLocation = path + sound;
            SoundEffect.PlaySync();
        }
        #endregion
    }
}
