using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tetris
{

    public class MainViewModel : BaseViewModel
    {
        #region Constants
         const int MAX_HEIGHT = 24;
         const int MAX_WIDTH = 16;
         const int MAX_COL = 3;
         const int MAX_ROW = 3;
        #endregion
         
        #region Types
        public delegate void TetrisBoardDelegate(Command obj);
        public event TetrisBoardDelegate TetrisEvent;
        static public Matrix[,] Mat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
        static public Matrix[,] tempMat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
        static public bool IsPilesRemove = false;
        static public Matrix[,] NextBlock = new Matrix[5, 5];
        public int[,] CurrentBlock = new int[MAX_ROW, MAX_COL];
        public int CurrentBlockRow = 0;
        public int CurrentBlockCol = 0;
        public int CurrentBlockSize = 0;
        public int blockBaseLand = 0;
        public int LeftBoundary = 0;
        public int RightBoundary = 0;
        bool isDone = false;
        int blockNumber = 0;
        int NextBlockNumber = 0;
        Random rnd = new Random();
        public bool isCurrentBlockLanded = false;
        private static readonly object lockObj = new object();
        static int moveUpDown = Utility.Common.DOWN_WALKER;
        static int moveLeftRight = Utility.Common.LEFT_RIGHT_WALKER;
        DispatcherTimer timer;
        static bool PauseFlag = true;
        int milliSecond = Utility.Common.TIME_SPAN;
        #endregion

        #region Properties
        
        private object tetrisBoard;
        public object TetrisBoard
        {
            get { return tetrisBoard; }
            set
            {
                tetrisBoard = value;
                Notify("TetrisBoard");
            }
        }

        private object blockBoard;
        public object BlockBoard
        {
            get 
            { 
                return blockBoard; 
            }
            set 
            { 
                blockBoard = value;
                Notify("BlockBoard");
            }
        }
        

        private int score;
        public int Score
        {
            get { return score; }
            set
            {
                score = value;
                if (Score > HighScore)
                    UpdateHighScore(); 
                Notify("Score");
            }
        }

        private int highScore;
        public int HighScore
        {
            get { return highScore; }
            set 
            { 
                highScore = value;
                Notify("HighScore");
            }
        }
       

        private string startPauseResume;
        public string StartPauseResume
        {
            get { return startPauseResume; }
            set 
            { 
                startPauseResume = value;
                Notify("StartPauseResume");
            }
        }

        private bool isGameOver;
        public bool IsGameOver
        {
            get { return isGameOver; }
            set 
            { 
                isGameOver = value;
                Notify("IsGameOver");
            }
        }
            
        private int gameLevel;
        public int GameLevel
        {
            get { return gameLevel; }
            set 
            {
                gameLevel = value;
                Notify("GameLevel");
            }
        }
        
        #endregion

        #region Commands
       
        public ICommand LeftCommand { get; set; }
        public ICommand RightCommand { get; set; }
        public ICommand RotateLeftCommand { get; set; }
        public ICommand RotateRightCommand { get; set; }
        public ICommand LandCommand { get; set; }
        public ICommand StartGameCommand { get; set; }
        public ICommand ResetGameCommand { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            GameLevel = 0;
            StartPauseResume = Utility.Common.Start;
            LeftCommand = new DelegateCommand(LeftCommandHandler);
            RightCommand = new DelegateCommand(RightCommandHandler);
            RotateLeftCommand = new DelegateCommand(RotateLeftCommandHandler);
            RotateRightCommand = new DelegateCommand(RotateRightCommandHandler);
            LandCommand = new DelegateCommand(LandCommandHandler);
            StartGameCommand = new DelegateCommand(StartGameCommandHandler);
            ResetGameCommand = new DelegateCommand(ResetGameCommandHandler);
            UpdateHighScore();
            InitializeTetris();
            InitializeNextBlock();

            NextBlockNumber = rnd.Next(1, Utility.Common.NUMBER_OF_BLOCKS + 1);
            UpdateNextBlock();
            blockNumber = rnd.Next(1, Utility.Common.NUMBER_OF_BLOCKS + 1);
            GetNewBlock(blockNumber);

            this.TetrisEvent += new TetrisBoardDelegate(TetrisEventHandler);
            moveUpDown = Utility.Common.DOWN_WALKER;
            moveLeftRight = Utility.Common.LEFT_RIGHT_WALKER;
            Score = 0;
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(DispatcherTimerHandler);
            timer.Interval = new TimeSpan(0, 0, 0, 0, milliSecond);
           // timer.Start();
        }

        #endregion

        /// <summary>
        /// Update Next block handler...
        /// </summary>
        private void UpdateNextBlock()
        {
            try
            {
                int[,] temp = BlockFactory.GetBlock(NextBlockNumber);
                int BlockSize = temp.Length;
                int BlockRow = temp.GetLength(0);
                int BlockCol = BlockSize / BlockRow;
                NextBlock = new Matrix[5, 5];
                InitializeNextBlock();
                for (int i = 0; i < 5 && i < BlockRow; i++)
                {
                    for (int j = 0; j < 5 && j < BlockCol; j++)
                    {
                        NextBlock[i + 1, j + 1].data = temp[i, j];
                    }
                }
                BlockBoard = NextBlock;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Reset Game Handler...
        /// </summary>
        private void ResetGameCommandHandler()
        {
            try
            {
                GameLevel = 1;
                timer.Stop();
                IsGameOver = false;
                PauseFlag = false;
                Score = 0;
                moveUpDown = Utility.Common.DOWN_WALKER;
                moveLeftRight = Utility.Common.LEFT_RIGHT_WALKER;
                Mat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
                tempMat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
                TetrisBoard = Mat;
                milliSecond = Utility.Common.TIME_SPAN;
                timer.Interval = new TimeSpan(0, 0, 0, 0, milliSecond);
                timer.Start();
                StartPauseResume = Utility.Common.Pause;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Update High score handler..
        /// </summary>
        private void UpdateHighScore()
        {
            try
            {
                string strScore = string.Empty;
                string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string path = string.Format(@"{0}\{1}", userDirectory, "TetrisScore.txt");
                if (!File.Exists(path))
                {

                    File.Create(path).Close();
                    File.WriteAllText(path, Score.ToString());
                }
                else
                {
                    strScore = File.ReadAllText(path);
                    HighScore = Convert.ToInt32(strScore);
                }
                if (Score > HighScore)
                {
                    HighScore = Score;
                    File.WriteAllText(path, Score.ToString());
                }
                //File.WriteAllText(path, "1MAX_HEIGHT-1");
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Start Game command Handler...
        /// </summary>
        private void StartGameCommandHandler()
        {
            try
            {
                if (StartPauseResume == Utility.Common.Start)
                {
                    GameLevel = 1;
                    IsGameOver = false;
                    PauseFlag = false;
                    Score = 0;
                    moveUpDown = Utility.Common.DOWN_WALKER;
                    moveLeftRight = Utility.Common.LEFT_RIGHT_WALKER;
                    Mat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
                    tempMat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
                    TetrisBoard = Mat;
                    timer.Start();
                    UpdateNextBlock();
                    StartPauseResume = Utility.Common.Pause;
                }
                else if (StartPauseResume == Utility.Common.Pause)
                {
                    PauseFlag = true;
                    timer.Stop();
                    StartPauseResume = Utility.Common.Resume;
                }
                else if (StartPauseResume == Utility.Common.Resume)
                {
                    PauseFlag = false;
                    timer.Start();
                    StartPauseResume = Utility.Common.Pause;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Dispatcher Timer Handler... ticks at every 'x' milisecond 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimerHandler(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => GenerateNewBlock()), DispatcherPriority.SystemIdle);
        }
        


        /// <summary>
        /// Generate new block based on the block type-id
        /// </summary>
        /// <param name="blockType"></param>
        private void GetNewBlock(int blockType)
        {
            try
            {
                CurrentBlock = BlockFactory.GetBlock(blockType);
                CurrentBlockSize = CurrentBlock.Length;
                CurrentBlockRow = CurrentBlock.GetLength(0);
                CurrentBlockCol = CurrentBlockSize / CurrentBlockRow;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            
        }
  
        /// <summary>
        /// Tetris Event Handler...
        /// </summary>
        /// <param name="cmd"></param>
        private void TetrisEventHandler(Command cmd)
        {
            try
            {
                lock (lockObj)
                {
                    switch (cmd)
                    {
                        case Command.DOWN: PaintCanvasHandler(); break;
                        case Command.LEFT: PaintCanvasHandler(); break;
                        case Command.RIGHT: PaintCanvasHandler(); break;
                        case Command.ROTATE_LEFT: PaintCanvasHandler(); break;
                        case Command.ROTATE_RIGHT: PaintCanvasHandler(); break;
                        case Command.LAND: PaintCanvasHandler(); break;
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }
        
        /// <summary>
        /// New Block Generater..
        /// </summary>
        private void GenerateNewBlock()
        {
            try
            {
                if (!IsPilesRemove)
                {
                    if (moveUpDown + CurrentBlockRow > MAX_HEIGHT - 1)
                    {
                        isDone = true;
                    }
                    if (isDone)
                    {
                        UpdateMatrix(ref Mat);
                        tempMat = Mat.Clone() as Matrix[,];
                        moveUpDown = Utility.Common.DOWN_WALKER;
                        moveLeftRight = Utility.Common.LEFT_RIGHT_WALKER;

                        blockNumber = NextBlockNumber;
                        GetNewBlock(blockNumber);
                        NextBlockNumber = rnd.Next(1, Utility.Common.NUMBER_OF_BLOCKS + 1);
                        UpdateNextBlock();
                        isDone = false;
                    }
                    moveUpDown++;
                    if (TetrisEvent != null)
                        TetrisEvent.Invoke(Command.DOWN);
                }
                else
                {
                    RemovePilesFromBoard();
                    IsPilesRemove = false;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Remove piles from board.
        /// </summary>
        private void RemovePilesFromBoard()
        {
            try
            {
                //Remove Handler...
                int bottom = MAX_HEIGHT - 1;
                int mul = 1;
                while (bottom >= 0)
                {
                    int count = 0;
                    for (int i = 0; i < MAX_WIDTH; i++)
                    {
                        if (Mat[bottom, i].data == Utility.Common.DOWN_WALKER)
                            count++;
                    }
                    if (count == MAX_WIDTH)
                    {
                        for (int i = 0; i < MAX_WIDTH; i++)
                        {
                            Mat[bottom, i].data = Utility.Common.DOWN_WALKER;
                        }


                        for (int b = bottom; b > 0; --b)
                        {
                            for (int i = 0; i < MAX_WIDTH; i++)
                            {
                                Mat[b, i] = Mat[b - 1, i];
                            }
                        }

                        Score = Score + 10 * mul;
                        if (Score > Utility.Common.LEVEL_SCORE * GameLevel)
                        {
                            GameLevel = GameLevel + 1;
                            milliSecond = milliSecond - (milliSecond / 10);
                            if (milliSecond >= 0)
                            {
                                timer.Interval = new TimeSpan(0, 0, 0, 0, milliSecond);
                            }
                        }
                        mul++;
                        bottom++;
                    }
                    bottom--;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Remove piles handler and updating dependency property...
        /// </summary>
        private void RemovePilesHandler()
        {
            try
            {
                Matrix[,] temp = new Matrix[MAX_HEIGHT, MAX_WIDTH];
                temp = Mat.Clone() as Matrix[,];
                Mat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
                Mat = temp.Clone() as Matrix[,];
                TetrisBoard = Mat;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Handler to check if piles are available to be removed.
        /// </summary>
        /// <returns></returns>
        private bool IsPilesRemoved()
        {
            bool flag = false;
            try
            {
                //Remove Handler...
                int bottom = MAX_HEIGHT - 1;
                int mul = 1;
                while (bottom >= 0)
                {
                    int count = 0;
                    for (int i = 0; i < MAX_WIDTH; i++)
                    {
                        if (Mat[bottom, i].data == 1)
                            count++;
                    }
                    if (count == MAX_WIDTH)
                    {
                        flag = true;
                        for (int i = 0; i < MAX_WIDTH; i++)
                        {
                            Mat[bottom, i].data = Utility.Common.DOWN_WALKER;
                        }

                        mul++;
                    }
                    bottom--;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            return flag;
        }

        /// <summary>
        /// Update Tatris Matrix 
        /// </summary>
        /// <param name="Mat"></param>
        private void UpdateMatrix(ref Matrix[,] Mat)
        {
            try
            {
                for (int i = 0; i < MAX_HEIGHT; i++)
                {
                    for (int j = 0; j < MAX_WIDTH; j++)
                    {
                        if (Mat[i, j].data == 1)
                            Mat[i, j].isLanded = true;
                        else
                            Mat[i, j].isLanded = false;
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Initialize Tetris Board.
        /// </summary>
        private void InitializeTetris()
        {
            try
            {
                for (int i = 0; i < MAX_HEIGHT; i++)
                {
                    for (int j = 0; j < MAX_WIDTH; j++)
                    {
                        if (Mat[i, j].isLanded == false)
                            Mat[i, j].data = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                var xx = ex.Message;
            }
        }

        /// <summary>
        /// Initialize Next Tetris block to be landed.
        /// </summary>
        private void InitializeNextBlock()
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        NextBlock[i, j].data = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }
        
        /// <summary>
        /// Paint Canvas Handler- Providing values to the dependency property..
        /// </summary>
        private void PaintCanvasHandler()
        {
            try
            {
                bool flag = true;
                //Creating new instance of matrix since dependency property callback method call if the reference of an object changed.
                Mat = new Matrix[MAX_HEIGHT, MAX_WIDTH];
                Mat = tempMat.Clone() as Matrix[,];
                
                //Fill all the empty area or non-landed are with zeros..
                InitializeTetris();

                //block width with respect to relative position in the board. blockwith->landSize...
                int landSize = moveLeftRight + CurrentBlockCol;

                FindBlockBaseLandRowHandler(landSize);

               // Check if game is over...
                if (blockBaseLand < 2)
                {
                    timer.Stop();
                    DisplayGameOverBoard();
                    return;
                }

                // base line of a block to be landed..
                int baseLine = blockBaseLand + CurrentBlockRow;

                
                flag = CheckBlockOverlapHandler(flag);

                //If blocks are above base line .. then move it.
                BlockMoveHandler();

                //If blocks are  on the base line and can be fitted into partial empty area....
                BlockLandHandler();

                //Input for dependency property to paint canvas....
                TetrisBoard = Mat;

                RemoveBlocksIfAny(flag);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Remove row of blocks if found any..
        /// </summary>
        /// <param name="flag"></param>
        private void RemoveBlocksIfAny(bool flag)
        {
            try
            {
                if (moveUpDown >= blockBaseLand || !flag || moveUpDown + CurrentBlockRow > MAX_HEIGHT - 1)
                {
                    isDone = true;
                    if (IsPilesRemoved())
                    {
                        RemovePilesHandler();
                        IsPilesRemove = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Block to be landed.
        /// </summary>
        private void BlockLandHandler()
        {
            try
            {
                if (blockBaseLand != MAX_HEIGHT - 1 && moveUpDown >= 0)
                {
                    blockBaseLand++;
                    for (int j = 0; j < CurrentBlockRow; j++)
                    {
                        for (int k = 0; k < CurrentBlockCol; k++)
                        {
                            if (CurrentBlock[j, k] == 1)
                            {
                                Mat[moveUpDown + j, k + moveLeftRight].data = CurrentBlock[j, k];
                            }
                            if (Mat[moveUpDown + j, k + moveLeftRight].data == 1)
                                Mat[moveUpDown + j, k + moveLeftRight].isLanded = false;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Block move handler and updating tatris matrix.
        /// </summary>
        private void BlockMoveHandler()
        {
            try
            {
                if (moveLeftRight + CurrentBlockCol - 1 < MAX_WIDTH && moveUpDown + CurrentBlockRow - 1 <= blockBaseLand)
                {
                    for (int j = 0; j < CurrentBlockRow; j++)
                    {
                        for (int k = 0; k < CurrentBlockCol; k++)
                        {
                            if ((moveUpDown + j) <= blockBaseLand && (moveUpDown + j) >= 0 && (moveLeftRight + k) < MAX_WIDTH && (moveLeftRight + k) >= 0)
                                Mat[moveUpDown + j, k + moveLeftRight].data += CurrentBlock[j, k];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Handler to check if blocks are overlapped.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        private bool CheckBlockOverlapHandler(bool flag)
        {
            try
            {
                int[,] OverlapMatrix = new int[MAX_ROW, MAX_COL];
                OverlapMatrix = CurrentBlock.Clone() as int[,];
                Matrix[,] tMat = Mat.Clone() as Matrix[,];
                for (int i = 0; i < CurrentBlockRow && moveUpDown + i < MAX_HEIGHT; i++)
                {
                    for (int j = 0; j < CurrentBlockCol; j++)
                    {
                        if (moveLeftRight + j >= 0 && moveLeftRight + j < MAX_WIDTH && moveUpDown + i >= 0)
                        {
                            if (tMat[moveUpDown + i, moveLeftRight + j].data + OverlapMatrix[i, j] > 1)
                            {
                                isDone = true;
                                moveUpDown--;
                                flag = false;
                                break;
                            }
                        }
                    }

                    if (flag == false) break;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            return flag;
        }

        /// <summary>
        /// Find a base line where a block can land safely or without overlapping..
        /// </summary>
        /// <param name="landSize"></param>
        private void FindBlockBaseLandRowHandler(int landSize)
        {
            try
            {
                for (int j = MAX_HEIGHT - 1; j >= 0; j--)
                {
                    int c = 0;
                    for (int i = moveLeftRight; i < landSize && i < MAX_WIDTH; i++)
                    {
                        if (Mat[j, i].data == 0)
                            c++;
                    }
                    if (c == CurrentBlockCol)
                    {
                        blockBaseLand = j;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Display Game over board. Set Tetris block to empty to show a game over text
        /// </summary>
        private void DisplayGameOverBoard()
        {
            try
            {
                for (int i = 0; i < MAX_HEIGHT; i++)
                {
                    for (int j = 0; j < MAX_WIDTH; j++)
                    {
                        Mat[i, j].data = 0;
                    }
                }

                IsGameOver = true;
                StartPauseResume = Utility.Common.Start;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        #region Command Handler...
        /// <summary>
        /// Make block Move down / move down fast
        /// </summary>
        public void LandCommandHandler()
        {
            try
            {
                if (!PauseFlag)
                {
                    int max = 0;
                    moveUpDown++;

                    if (CurrentBlockRow == MAX_ROW) // If block row is 3.
                        max = 21;
                    else if (CurrentBlockRow == 2) // If block row is 2.
                        max = 22;
                    else
                        max = MAX_HEIGHT - 1; // if Block row is 1.
                    if (moveUpDown <= max)
                    {
                        if (TetrisEvent != null)
                            TetrisEvent.Invoke(Command.LAND);
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Make right move of a block 
        /// </summary>
        public void RightCommandHandler()
        {
            try
            {
                if (!PauseFlag)
                {
                    if (!isDone || (moveUpDown - 1 + CurrentBlockRow == MAX_HEIGHT - 1))
                    {
                        //timer.Stop();
                        RightBoundary = -1;
                        int c = 0;
                        if (moveLeftRight >= 0 && moveLeftRight + CurrentBlockCol <= MAX_WIDTH)
                        {
                            for (int i = moveUpDown; i < moveUpDown + CurrentBlockRow && i >= 0 && i < MAX_HEIGHT && moveLeftRight + CurrentBlockCol < MAX_WIDTH; ++i)
                            {
                                if (Mat[i, moveLeftRight + CurrentBlockCol].data + CurrentBlock[i - moveUpDown, CurrentBlockCol - 1] <= 1)
                                {
                                    c++;
                                }

                            }
                            if (c == CurrentBlockRow)
                                moveLeftRight++;

                            if (TetrisEvent != null)
                                TetrisEvent.Invoke(Command.RIGHT);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Make left move of a block
        /// </summary>
        public void LeftCommandHandler()
        {
            try
            {
                if (!PauseFlag)
                {
                    if (!isDone || (moveUpDown - 1 + CurrentBlockRow == MAX_HEIGHT - 1))
                    {
                        LeftBoundary = -1;
                        int c = 0;
                        if (moveLeftRight >= 0 && moveLeftRight + CurrentBlockCol <= MAX_WIDTH)
                        {

                            for (int i = moveUpDown; i < moveUpDown + CurrentBlockRow && i >= 0 && i < MAX_HEIGHT && moveLeftRight > 0; ++i)
                            {
                                if (Mat[i, moveLeftRight - 1].data + CurrentBlock[i - moveUpDown, 0] <= 1)
                                    c++;
                            }
                            if (c == CurrentBlockRow)
                                moveLeftRight--;

                            if (TetrisEvent != null)
                                TetrisEvent.Invoke(Command.LEFT);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Rotate block in clockwise direction.
        /// </summary>
        public void RotateRightCommandHandler()
        {
            try
            {
                if (!PauseFlag)
                {
                    if (!isDone)
                    {
                        //timer.Stop();
                        if (moveLeftRight + CurrentBlockRow - 1 >= MAX_WIDTH)
                        {
                            moveLeftRight--;
                            if (moveLeftRight + CurrentBlockRow - 1 >= MAX_WIDTH)
                                moveLeftRight--;
                            //return;
                        }
                        int[,] temp = new int[MAX_ROW, MAX_COL];
                        int[,] temp1 = new int[MAX_ROW, MAX_COL];
                        for (int i = 0; i < CurrentBlockRow; i++)
                        {
                            for (int j = 0; j < CurrentBlockCol; j++)
                            {
                                temp[i, j] = CurrentBlock[i, j];
                            }
                        }


                        //
                        for (int i = 0; i < MAX_ROW; i++)
                        {
                            for (int j = 0; j < MAX_COL; j++)
                            {
                                temp1[i, j] = temp[MAX_ROW - (i + 1), j];
                            }
                        }

                        //Transpose...
                        for (int i = 0; i < MAX_ROW; i++)
                        {
                            for (int j = 0; j < MAX_COL; j++)
                            {
                                temp[i, j] = temp1[j, i];
                            }
                        }

                        SwapRowCol();

                        temp = ModifyBlock(ref temp);

                        CurrentBlock = temp.Clone() as int[,];

                        if (TetrisEvent != null)
                            TetrisEvent.Invoke(Command.ROTATE_RIGHT);
                        //timer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        /// <summary>
        /// Rotate block in Anti-clockwise direction
        /// </summary>
        public void RotateLeftCommandHandler()
        {
            try
            {
                if (!PauseFlag)
                {
                    if (!isDone)
                    {
                        //timer.Stop();
                        if (moveLeftRight + CurrentBlockRow - 1 >= MAX_WIDTH)
                        {
                            moveLeftRight--;
                            if (moveLeftRight + CurrentBlockRow - 1 >= MAX_WIDTH)
                                moveLeftRight--;
                            //return;
                        }
                        int[,] temp = new int[MAX_ROW, MAX_COL];
                        for (int i = 0; i < CurrentBlockRow; i++)
                        {
                            for (int j = 0; j < CurrentBlockCol; j++)
                            {
                                temp[j, i] = CurrentBlock[i, CurrentBlockCol - (j + 1)];
                            }
                        }

                        SwapRowCol();

                        temp = ModifyBlock(ref temp);

                        CurrentBlock = temp.Clone() as int[,];

                        if (TetrisEvent != null)
                            TetrisEvent.Invoke(Command.ROTATE_LEFT);
                        //timer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }
        #endregion

        /// <summary>
        /// Padding Rows and columns...
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private int[,] ModifyBlock(ref int[,] temp)
        {
            try
            {
                int[,] temp1 = new int[MAX_ROW, MAX_COL];
                int c = 0;

                for (int i = 0; i < MAX_COL; i++)
                {
                    if (temp[0, i] == 0)
                        c++;
                }
                if (c == MAX_COL)
                {
                    for (int i = 1; i < MAX_ROW; i++)
                    {
                        for (int j = 0; j < MAX_COL; j++)
                            temp1[i - 1, j] = temp[i, j];
                    }

                    temp = temp1.Clone() as int[,];
                }
                ///

                c = 0;
                for (int i = 0; i < MAX_COL; i++)
                {
                    if (temp[2, i] == 0)
                        c++;
                }

                if (c == MAX_COL)
                {
                    for (int i = 0; i < MAX_ROW; i++)
                    {
                        for (int j = 0; j < MAX_COL; j++)
                            temp1[i, j] = temp[i, j];
                    }

                    temp = temp1.Clone() as int[,];
                }

                ///
                while (true)
                {
                    c = 0;
                    for (int i = 0; i < MAX_ROW; i++)
                    {
                        if (temp[i, 0] == 0)
                            c++;
                    }

                    if (c == MAX_ROW)
                    {
                        for (int i = 0; i < MAX_ROW; i++)
                        {
                            for (int j = 0; j < MAX_COL && j + 1 < MAX_COL; j++)
                                temp1[i, j] = temp[i, j + 1];
                        }
                        temp = temp1.Clone() as int[,];
                    }
                    else
                        break;
                }
                ///
                c = 0;
                for (int i = 0; i < MAX_ROW; i++)
                {
                    if (temp[i, 2] == 0)
                        c++;
                }

                if (c == MAX_ROW)
                {
                    for (int i = 0; i < MAX_ROW; i++)
                    {
                        for (int j = 0; j < MAX_COL; j++)
                            temp1[i, j] = temp[i, j];
                    }
                    temp = temp1.Clone() as int[,];
                }
                return temp;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// Swap row and column - if Rotations happen
        /// </summary>
        private void SwapRowCol()
        {
            try
            {
                int t = CurrentBlockCol;
                CurrentBlockCol = CurrentBlockRow;
                CurrentBlockRow = t;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

     }
}
