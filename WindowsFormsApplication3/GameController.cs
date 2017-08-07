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
    public class GameController
    {
        static Random random = new Random();

        enum Players {Computer,Player}

        static bool soundPlay = true;

        static string path = Environment.CurrentDirectory + "\\PlaySound\\";

        public static MediaPlayer BeforeSound = new MediaPlayer();
        static MediaPlayer EndedSound = new MediaPlayer();
        static MediaPlayer WinSound = new MediaPlayer();
        static MediaPlayer LoseSound = new MediaPlayer();

        static SoundPlayer ShotSound;
        static SoundPlayer MissSound;
        static SoundPlayer CrippledSound;
        static SoundPlayer DrownedShipsSound;

        public static event EventHandler ShipsDrown;

        public static bool EndedGame { get; set; }

        public static event EventHandler BeginGame;
        public static event EventHandler EndGame;
        public static event EventHandler BeforeGame;

        static string shipsWord = "Кораблей:";
        static string shotWord = "Выстрелов:";

        static Label shipsComputer;
        static Label shotComputer;
        static Label nameComputer;

        static Label shipsPlayer;
        static Label shotPlayer;
        static Label namePlayer;

        static Label score;

        static SeaColor moveColor = SeaColor.Gold;
        static SeaColor passColor =SeaColor.Black;

        static SeaColor winColor = SeaColor.Green;
        static SeaColor lossColor = SeaColor.Red;

        static int playerWin = 0;
        static int computerWin = 0;

        static int playerShips = Field.ShipsCount;
        static int computerShips = Field.ShipsCount;

        static int playerShot=Field.Size * Field.Size;
        static int computerShot= Field.Size * Field.Size;

        static GameController()
        {      
            ShotSound = new SoundPlayer(path + "выстрел2.wav");
            MissSound = new SoundPlayer(path+"мимо.wav");
            CrippledSound = new SoundPlayer(path+"подбит.wav");
            DrownedShipsSound = new SoundPlayer(path + "потоп корабля.wav");

            BeforeSound.Open(new Uri(path + "pirates.mp3", UriKind.RelativeOrAbsolute));
            EndedSound.Open(new Uri(path + "морские дьяволы.mp3", UriKind.RelativeOrAbsolute));
            WinSound.Open(new Uri(path + "победа.mp3", UriKind.RelativeOrAbsolute)); ;
            LoseSound.Open(new Uri(path + "проигрыш.mp3", UriKind.RelativeOrAbsolute));

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

        static int  PlayerShips
        {
            set
            {
                playerShips = value;
                shipsPlayer.Text = shipsWord + value;
                if (playerShips == 0) GameOver(Players.Computer);
            }
            get { return playerShips; }
        }

        static int ComputerShips
        {
            set
            {
                computerShips = value;
                shipsComputer.Text = shipsWord + value;
                if (computerShips == 0) GameOver(Players.Player);
            }
            get { return computerShips; }
        }

        static int ComputerShot
        {
            get {return computerShot; } 
            set
            {
                computerShot = value;
                shotComputer.Text = shotWord + computerShot;
            }
        }

        static int PlayerShot
        {
            get { return playerShot; }
            set
            {
                playerShot = value;
                shotPlayer.Text = shotWord + playerShot;
            }
        }

        public static void GetLabel( Label shipsComputer, Label shotComputer, Label nameComputer,
            Label shipsPlayer, Label shotPlayer, Label namePlayer, Label score)
        {
            GameController.shipsComputer=shipsComputer;
            GameController.shotComputer=shotComputer;
            GameController.nameComputer=nameComputer;

            GameController.shipsPlayer=shipsPlayer;
            GameController.shotPlayer=shotPlayer;
            GameController.namePlayer=namePlayer;

            GameController.score = score;
        }

        public static void Begin_Clicked(object sender, EventArgs e)
        {
            if (MouseEvent.BeginArround) MouseEvent.EndedArround();

            EndedGame = false;
            BeginGame(new object(), new EventArgs());

            BeforeSound.Pause();

            int complexity = Form1.GetComplexity();

            if (complexity == 0)
            {
                Form1.ComputerField.computerPlayer = new ComputerPlay();
            }
            else
            {
                Form1.ComputerField.computerPlayer = new OptimalComputerPlay();
            }

            EnabledSwitch(Form1.PlayerField.PictBox, false);

            Players OneMove = (Players)random.Next(0, 2);
            if (OneMove == Players.Computer) Form1.ComputerField.computerPlayer.Move();

            EnabledSwitch(Form1.ComputerField.PictBox, true);

            namePlayer.ForeColor = moveColor;
        }        

        public static void Ok_Clicked(object sender, EventArgs e)
        {
            BeforeGame(new object(), new EventArgs());
            EnabledSwitch(Form1.PlayerField.PictBox, true);

            foreach (var value in Form1.ComputerField.PictBox)
                value.RenderingMode = CellCondition.Empty;

            Form1.PlayerField.randomShips.RandomArrangement();
            Form1.PlayerField.DisplayCompletionCell();

            Form1.ComputerField.randomShips.RandomArrangement();

            PlayerShips = Field.ShipsCount;
            ComputerShips = Field.ShipsCount;

            PlayerShot = Field.Size * Field.Size;
            ComputerShot = Field.Size * Field.Size;

            namePlayer.ForeColor = passColor;
            nameComputer.ForeColor = passColor;

            EndedSound.Stop();
            BeforeSound.Play();
        }

        public static void ShipsDrowned(Ships ships)
        {
            if (ships.Field is PlayerField)
            {
                PlayerShips--;
                if (ShipsDrown != null) ShipsDrown(ships, new EventArgs());
            }
            else
            {
                ComputerShips--;
            }
        }

        public static void BeginComputerMove()
        {
            namePlayer.ForeColor = passColor;
            nameComputer.ForeColor = moveColor;
            
            namePlayer.Invalidate();
            namePlayer.Update();
            nameComputer.Invalidate();
            nameComputer.Update();
        
        }

        public static void BeginPlayerMove()
        {
            namePlayer.ForeColor = moveColor;
            nameComputer.ForeColor = passColor;
        }

        public static void ShotUpdater(Field field, CellCondition result)
        {
            if (field is ComputerField) PlayerShot--;
            else ComputerShot--;

            if (soundPlay)
            {
                ShotSound.PlaySync();

                if (result == CellCondition.Miss) MissSound.PlaySync();
                else
                if (result == CellCondition.Crippled) CrippledSound.PlaySync();
                else
                if (result == CellCondition.Drowned) DrownedShipsSound.PlaySync();
            }
            Thread.Sleep(500);
        }

        static void GameOver(Players winner)
        {
            EndedGame = true;
            EnabledSwitch(Form1.ComputerField.PictBox,false);

            if (winner == Players.Computer)
            {
                computerWin++;
                nameComputer.ForeColor = winColor;
                namePlayer.ForeColor = lossColor;
                PlaysSound(LoseSound);
            }
            else
            {
                playerWin++;
                nameComputer.ForeColor = lossColor;
                namePlayer.ForeColor = winColor;
                PlaysSound(WinSound);

            }

            EndGame(new object(), new EventArgs());

            score.Text = string.Format("{0}:{1}", playerWin, computerWin);
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
            //sound.Position = new TimeSpan(0);
            sound.Stop();
            sound.Play();
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
    }
}
