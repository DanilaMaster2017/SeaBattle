using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SeaBattleGame
{
    enum MoveState { KnownDirection, UnknownShipsLocation, UnknownDirection }

    enum Direction { Left, Top, Reight, Bottom }
      

    public class ComputerPlay
    {
        bool oneGuessing = false;

        Random random = new Random();
        public static bool[,] CheckShot = new bool[Field.Size, Field.Size];
        bool[] ChekDirection = new bool[4];

        int count;

        Location currentShot;
        Location firstShot;

        protected int IntactCell { get; set; }
        int intactDirection = 4;

        MoveState moveState = MoveState.UnknownShipsLocation;
        Direction direction;

        CellCondition shotState;

        public ComputerPlay()
        {
            shotState = CellCondition.Miss;
            GeneralStaticFunction.FalseToMatrix(CheckShot);
            IntactCell = Field.Size * Field.Size;
        }

        public void Move()
        {
           
            if (GameController.EndedGame) return;
            //GameController.ShotComputer();
            if (shotState == CellCondition.Miss) GameController.BeginComputerMove();

            CountingIntactCell();

            switch (moveState)
            {
                case MoveState.UnknownShipsLocation:
                    LongShot();
                    break;

                case MoveState.UnknownDirection:
                    GuessingDirection();
                    break;

                case MoveState.KnownDirection:
                    SureShot();
                    break;
            }

            if (shotState == CellCondition.Miss) GameController.BeginPlayerMove();
        }

        protected virtual void CountingIntactCell()
        {
            IntactCell = 0;

            foreach (bool value in CheckShot) if(!value) IntactCell++;
        }

        void LongShot()
        {
            int shot = random.Next(0, IntactCell);

            Location newShot = OverrideShot(CheckShot, shot);

            shotState = Form1.PlayerField.Shot(
                Form1.PlayerField.PictBox[newShot.IndexI, newShot.IndexJ]);

            if (shotState == CellCondition.Crippled)
            {
                moveState = MoveState.UnknownDirection;
                oneGuessing = true;
                firstShot = newShot;
            }
            currentShot = newShot;

            if (shotState != CellCondition.Miss) Move();
        }

        protected virtual Location OverrideShot(bool[,] CheckShot, int shot)
        {
            return GeneralStaticFunction.FromNumberLocation(CheckShot, shot);
        }

        void GuessingDirection()
        {

            if (oneGuessing) CountingAllowableDirection();

            int shot = random.Next(0, intactDirection);
            count = 0;

            int numberDirection = -1;

            for (var i = 0; i < 4; i++)
            {
                    if (!ChekDirection[i])
                    {
                        if (count == shot)
                        {
                            ChekDirection[i] = true;
                            intactDirection--;
                            numberDirection = i;                 
                        }
                        count++;
                    }
            }

            CellCondition shotResult = ShotDirection((Direction)numberDirection);

            if (shotResult == CellCondition.Crippled)
            {
                direction = (Direction)numberDirection;
                moveState = MoveState.KnownDirection;
            }

            if (shotState != CellCondition.Miss) Move();
        }

        void CountingAllowableDirection()
        {
            for (var i = 0; i < ChekDirection.Length; i++) ChekDirection[i] = false;

            oneGuessing = false;
            intactDirection = 0;

            if ((currentShot.IndexJ > 0) && (!CheckShot[currentShot.IndexI, currentShot.IndexJ - 1]))
            {
                 intactDirection++;               
            }
            else ChekDirection[0] = true;

            if ((currentShot.IndexI > 0) && (!CheckShot[currentShot.IndexI - 1, currentShot.IndexJ]))
            {
                intactDirection++;             
            }
            else ChekDirection[1] = true;

            if ((currentShot.IndexJ < Field.Size - 1) && (!CheckShot[currentShot.IndexI, currentShot.IndexJ + 1]))
            {
               intactDirection++;             
            }
            else ChekDirection[2] = true;

            if ((currentShot.IndexI < Field.Size - 1) && (!CheckShot[currentShot.IndexI + 1, currentShot.IndexJ]))
            {
                intactDirection++;                
            }
            else ChekDirection[3] = true;
        }

        void SureShot()
        {
            int di = 0;
            int dj = 0;

            if (direction == Direction.Left) dj = -1;
            if (direction == Direction.Reight) dj = 1;
            if (direction == Direction.Top) di = -1;
            if (direction == Direction.Bottom) di = 1;

            CellCondition shotState;

            if ((GeneralStaticFunction.PreventionIndexRange(currentShot.IndexI + di, currentShot.IndexJ + dj))
                && (!CheckShot[currentShot.IndexI + di, currentShot.IndexJ + dj]))
            {
                shotState = ShotDirection(direction);
            }
            else
            {
                ChangeDirection();
                shotState = ShotDirection(direction);
            }
            
            if (shotState == CellCondition.Miss) ChangeDirection();
            else Move();
        }

        void ChangeDirection()
        {
            if (direction == Direction.Left) direction = Direction.Reight;
            else
            if (direction == Direction.Reight) direction = Direction.Left;

            if (direction == Direction.Top) direction = Direction.Bottom;
            else
            if (direction == Direction.Bottom) direction = Direction.Top;

            currentShot = firstShot;
        }

        CellCondition ShotDirection(Direction direciton)
        {
            int di = 0;
            int dj = 0;

            if (direciton == Direction.Left)
            {
                dj = -1; di = 0;
            }

            if (direciton == Direction.Top)
            {
                dj = 0; di = -1;
            }

            if (direciton == Direction.Reight)
            {
                dj = 1; di = 0;
            }

            if (direciton == Direction.Bottom)
            {
                dj = 0; di = 1;
            }
           
            shotState = Form1.PlayerField.Shot(
                Form1.PlayerField.PictBox[currentShot.IndexI + di, currentShot.IndexJ + dj]);
            CheckShot[currentShot.IndexI + di, currentShot.IndexJ + dj] = true;


            if (shotState != CellCondition.Miss)
            {
                currentShot.IndexI += di;
                currentShot.IndexJ += dj;
            }

            if (shotState == CellCondition.Drowned)
                moveState = MoveState.UnknownShipsLocation;

            return shotState;
        }

    }
}
