using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tetris.Cherub
{
    public partial class Form1 : Form
    {
        Graphics panelGraphic; // The main panel display
        Graphics nextPanel;    // Next shape panel display
        Graphics bufferGraphic; // The graphic for buffering purpose
        Bitmap bufferBitmap;

        RegistryKey regkey = Registry.CurrentUser.OpenSubKey("MartinSoft", true);
        
        const int blockInRow = 10; // the number of the block in a rows
        const int initialInterval = 500; // the initial interval of the timer
        const int sizeOfNextPanel = 5;
        int sizeofGrid; // of each grid in pixels
        int[,] Grid = new int[blockInRow, 2 * blockInRow];
        int[,] nextGrid = new int[sizeOfNextPanel, sizeOfNextPanel];
        int[,] permanentBlock = new int[blockInRow, 2 * blockInRow];
        int[,] movingBlock = new int[blockInRow, 2 * blockInRow];
        int blockStartX = (blockInRow / 2) - 1; // the x point where new shape is coming
        int blockStartY = 0; // the y point where new shape is coming
        int blockCurrentX; //the x location of current moving shape
        int blockCurrentY; //the y location of current moving shape
        Shape currentShape;
        Shape nextShape;
        Random random = new Random(DateTime.Now.Millisecond);
        int completedRows; //the number of rows that have been completed
        int nextType; // the next shape in integer
        int currentType; // the current shape type in integer
        int score; // the score of current game
        int level; //the level of current game
        int mode; // the mode of current game
        bool isPaused; //the variable to check if the game is paused


        public Form1()
        {
            if (regkey == null)
            {
                regkey = Registry.CurrentUser.CreateSubKey("MartinSoft");
                regkey.SetValue("HighestScore", 0);
            }

            
            InitializeComponent();
            mode = 0; // Normal Mode
            bufferBitmap = new Bitmap(panel1.Width, panel1.Height);
            panelGraphic = panel1.CreateGraphics();
            nextPanel = panel2.CreateGraphics();
            bufferGraphic = Graphics.FromImage(bufferBitmap);
            sizeofGrid = panel1.Width / blockInRow;
            newGame();
                        
        }

        private void drawPanel()
        {
            int column, row;
            bufferGraphic.Clear(panel1.BackColor);
            updateGrid();
            
            for (row = 0; row < blockInRow * 2; row++)
                for (column = 0; column < blockInRow; column++)
                    if (Grid[column, row] > 0)
                    {
                        Color color;

                        if (Grid[column, row] == 1)
                            color = Color.DeepSkyBlue;
                        else if (Grid[column, row] == 2)
                            color = Color.Crimson;
                        else if (Grid[column, row] == 3)
                            color = Color.Green;
                        else if (Grid[column, row] == 4)
                            color = Color.DarkOrange;
                        else if (Grid[column, row] == 5)
                            color = Color.Yellow;
                        else if (Grid[column, row] == 6)
                            color = Color.Violet;
                        else
                            color = Color.Purple;

                        Brush brush = new LinearGradientBrush(new Rectangle((column * sizeofGrid) + 1, (row * sizeofGrid) + 1, sizeofGrid - 2, sizeofGrid - 2), Color.WhiteSmoke, color, 60);
                        bufferGraphic.FillRectangle(brush, (column * sizeofGrid) + 1, (row * sizeofGrid) + 1, sizeofGrid - 2, sizeofGrid - 2);
                    }
            panelGraphic.DrawImage(bufferBitmap, 0, 0);
        }

        private void updateGrid()
        {
            int column, row;

            clearGrid();
            for (row = 0; row < blockInRow * 2; row++)
                for (column = 0; column < blockInRow; column++)
                {
                    if (permanentBlock[column, row] > 0)
                        Grid[column, row] = permanentBlock[column, row];
                    if (movingBlock[column, row] > 0)
                        Grid[column, row] = movingBlock[column, row];
                }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           drawPanel();
           drawNextPanel();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            moveBlock();
            drawPanel();
        }

        private void putShape()
        {
            clearMovingBlock();
            int index;

            for(index = 0; index < 4; index++)
                movingBlock[blockCurrentX + currentShape.blockX[index], blockCurrentY + currentShape.blockY[index]] = 
                    currentShape.color;
            
        }

        private void newShape()
        {
            blockCurrentX = blockStartX;
            blockCurrentY = blockStartY;
            
            currentType = nextType;

            currentShape = new Square(1, 1);
            switch (currentType)
            {
                case 1: currentShape = new Line(1, 1);
                    break;
                case 2: currentShape = new RightL(1, 1);
                    break;
                case 3: currentShape = new LeftL(1, 1);
                    break;
                case 4: currentShape = new RightZ(1, 1);
                    break;
                case 5: currentShape = new LeftZ(1, 1);
                    break;
                case 6: currentShape = new TShape(1, 1);
                    break;
            }
            int index;
            for (index = 0; index < 4; index++)
            {
                currentShape.blockX[index] = nextShape.blockX[index];
                currentShape.blockY[index] = nextShape.blockY[index];
            }
            currentShape.direction = nextShape.direction;
            currentShape.color = nextShape.color;

            generateNextShape();
        }

        private void generateNextShape()
        {
            int color = random.Next(1, 8);
            // Insane Mode
            if (mode == 1)
                color = random.Next(1, 3);
            nextType = random.Next(0, 7);
            switch (nextType)
            {
                case 0: nextShape = new Square(color, 0);
                    break;
                case 1: nextShape = new Line(color, random.Next(0, 2));
                    break;
                case 2: nextShape = new RightL(color, random.Next(0, 4));
                    break;
                case 3: nextShape = new LeftL(color, random.Next(0, 4));
                    break;
                case 4: nextShape = new RightZ(color, random.Next(0, 2));
                    break;
                case 5: nextShape = new LeftZ(color, random.Next(0, 2));
                    break;
                case 6: nextShape = new TShape(color, random.Next(0, 4));
                    break;
            }
            
                
            drawNextPanel();

        }

        private void moveBlock()
        {
            if (canMoveDown())
            {
                blockCurrentY++;
                putShape();
            }
            else
            {
                int index;
                
                for (index = 0; index < 4; index++)
                    permanentBlock[blockCurrentX + currentShape.blockX[index], blockCurrentY + currentShape.blockY[index]] = 
                        currentShape.color;
                clearCompletedRow();

                newShape();
                putShape();
                // Check if game is over
                for (index = 0; index < blockInRow; index++)
                    if (permanentBlock[index, 0] != 0)
                    {
                        timer1.Stop();
                        MessageBox.Show("GAME OVER");
                        if (score > (int)regkey.GetValue("HighestScore"))
                        {
                            regkey.SetValue("HighestScore", score);
                            highestScore.Text = (int)regkey.GetValue("HighestScore") + "";
                        }
                        break;
                    }          
              
            }

        }

        private void clearCompletedRow()
        {
            int row, column;
            bool completedRow = true;
            for (row = 0; row < (blockInRow * 2); row++)
            {
                
                if (mode == 0)
                {
                    completedRow = true;
                    for (column = 0; column < blockInRow; column++)
                        if (permanentBlock[column, row] == 0)
                            completedRow = false;
                }
                else if (mode == 1) // insane mode
                {
                    completedRow = false;
                    for (column = 0; column < blockInRow - 4; column++)
                        if (permanentBlock[column, row] != 0 &&
                            (permanentBlock[column, row] == permanentBlock[column + 1, row]) &&
                            (permanentBlock[column + 1, row] == permanentBlock[column + 2, row]) &&
                            (permanentBlock[column + 2, row] == permanentBlock[column + 3, row]) &&
                            (permanentBlock[column + 3, row] == permanentBlock[column + 4, row]))
                            completedRow = true;
                }

                if (completedRow)
                {
                    int rowAbove;
                    
                    for (rowAbove = row - 1; rowAbove >= 0; rowAbove--)
                        for (column = 0; column < blockInRow; column++)
                            permanentBlock[column, rowAbove + 1] = permanentBlock[column, rowAbove];
                    completedRows++;
                    updateScore();
                }
            }
            drawPanel();
        }

        private void updateScore()
        {
            score += (level * 10) /(mode + 1);
            scoreLabel.Text = "" + score;
            if ((completedRows % 20) == 0)
                level++;
            levelLabel.Text = "" + level;
            timer1.Interval = initialInterval / level;
        }

        private bool canMoveDown()
        {
            int index;
            for (index = 0; index < 4; index++)
                if ((blockCurrentY + currentShape.blockY[index] == (2 * blockInRow) - 1) ||
                    (permanentBlock[blockCurrentX + currentShape.blockX[index], blockCurrentY + currentShape.blockY[index] + 1] != 0))
                    
                    return false;

            return true;
        }

        private bool canMoveRight()
        {
            int index;
            for (index = 0; index < 4; index++)
                if ((blockCurrentX + currentShape.blockX[index] == (blockInRow) - 1) ||
                    (permanentBlock[blockCurrentX + currentShape.blockX[index] + 1, blockCurrentY + currentShape.blockY[index]] != 0))
                    
                    return false;

            return true;
        }

        private bool canMoveLeft()
        {
            int index;
            for (index = 0; index < 4; index++)
                if ((blockCurrentX + currentShape.blockX[index] == 0) ||
                    (permanentBlock[blockCurrentX + currentShape.blockX[index] - 1, blockCurrentY + currentShape.blockY[index]] != 0))

                    return false;

            return true;
        }

        private bool canRotate(int direction)
        {
            int index;
            Shape tempShape ;
            tempShape = new Square(1, 1);
            switch (currentType)
            {
                case 1: tempShape = new Line(1, 1);
                    break;
                case 2: tempShape = new RightL(1, 1);
                    break;
                case 3: tempShape = new LeftL(1, 1);
                    break;
                case 4: tempShape = new RightZ(1, 1);
                    break;
                case 5: tempShape = new LeftZ(1, 1);
                    break;
                case 6: tempShape = new TShape(1, 1);
                    break;
            }         

            for (index = 0; index < 4; index++)
            {
                tempShape.blockX[index] = currentShape.blockX[index];
                tempShape.blockY[index] = currentShape.blockY[index];
            }
            tempShape.direction = currentShape.direction;
            tempShape.Rotate(direction);
            for (index = 0; index < 4; index++)
                if ((blockCurrentX + tempShape.blockX[index] < 0) || (blockCurrentY + tempShape.blockY[index] > (blockInRow * 2) - 1) ||
                    (blockCurrentX + tempShape.blockX[index] > (blockInRow - 1)) ||
                    (permanentBlock[blockCurrentX + tempShape.blockX[index], blockCurrentY + tempShape.blockY[index]] != 0))

                    return false;

            return true;
        }

        private void clearMovingBlock()
        {
            int row, column;
            for (row = 0; row < blockInRow * 2; row++)
                for (column = 0; column < blockInRow; column++)
                    movingBlock[column, row] = 0;
        }

        private void clearPermanentBlock()
        {
            int row, column;
            for (row = 0; row < blockInRow * 2; row++)
                for (column = 0; column < blockInRow; column++)
                    permanentBlock[column, row] = 0;
        }

        private void clearGrid()
        {
            int row, column;
            for (row = 0; row < blockInRow * 2; row++)
                for (column = 0; column < blockInRow; column++)
                    Grid[column, row] = 0;
        }

     
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && canMoveLeft())
                blockCurrentX -= 1;
            else if (e.KeyCode == Keys.Right && canMoveRight())
                blockCurrentX += 1;
            else if (e.KeyCode == Keys.Down)
                timer1.Interval = 25;
            else if (e.KeyCode == Keys.Up && canRotate(1))
                rotateShape('R');
            else if (e.KeyCode == Keys.Space && canRotate(-1))
                rotateShape('L');
            else if (e.KeyCode == Keys.F2)
                newGame();
            else if (e.KeyCode == Keys.F3)
                pauseGame();
            else if (e.KeyCode == Keys.F4)
                changeMode();
            putShape();
            drawPanel();
        }

        private void changeMode()
        {
        
            mode = 1 - mode;
            if (mode == 1)
            {
                InsaneLabel1.Visible = true;
                InsaneLabel2.Visible = true;
            }
            else
            {
                InsaneLabel1.Visible = false;
                InsaneLabel2.Visible = false;
            }
           
            newGame();
        }

        private void pauseGame()
        {
            if (isPaused == true)
            {
                isPaused = false;
                timer1.Interval = initialInterval / level;
                timer1.Start();
                hidePanel.Visible = false;
                pausedLabel.Visible = false;
            }
            else
            {
                isPaused = true;
                timer1.Stop();
                hidePanel.Visible = true;
                pausedLabel.Visible = true;
            }
                       
        }

        private void newGame()
        {
            generateNextShape();
            blockCurrentX = blockStartX;
            blockCurrentY = blockStartY;
            level = 0;
            completedRows = 0;
            score = 0;
            updateScore();
            clearPermanentBlock();
            clearGrid();
            clearMovingBlock();
            newShape();
            drawPanel();
            timer1.Interval = initialInterval / level;
            isPaused = true;
            pauseGame();
            timer1.Start();
            highestScore.Text = (int)regkey.GetValue("HighestScore") + "";
            
        }

        private void rotateShape(char turn)
        {
            if (turn == 'R')
                currentShape.Rotate(1);
            else
                currentShape.Rotate(-1);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                timer1.Interval = initialInterval / level;
        }

        private void drawNextPanel()
        {
            int index;
            int blockSize = (panel2.Width / sizeOfNextPanel);
            int row, column;

            // Clear the next shape grid
            for (row = 0; row < sizeOfNextPanel; row++)
                for (column = 0; column < sizeOfNextPanel; column++)
                    nextGrid[column, row] = 0;

            // Put the shape into the grid
            for(index = 0; index < 4; index++)
                nextGrid[2 + nextShape.blockX[index], 1 + nextShape.blockY[index]] =
                    nextShape.color;
            nextPanel.Clear(panel2.BackColor);

            

            for (row = 0; row < sizeOfNextPanel; row++)
                for (column = 0; column < sizeOfNextPanel; column++)
                    if (nextGrid[column, row] > 0)
                    {
                        Color color;

                        if (nextGrid[column, row] == 1)
                            color = Color.DeepSkyBlue;
                        else if (nextGrid[column, row] == 2)
                            color = Color.Crimson;
                        else if (nextGrid[column, row] == 3)
                            color = Color.Green;
                        else if (nextGrid[column, row] == 4)
                            color = Color.DarkOrange;
                        else if (nextGrid[column, row] == 5)
                            color = Color.Yellow;
                        else if (nextGrid[column, row] == 6)
                            color = Color.Violet;
                        else
                            color = Color.Purple;
                        
                        Brush brush = new LinearGradientBrush(new Rectangle((column * blockSize) + 1, (row * blockSize) + 1, blockSize - 2, blockSize - 2), Color.WhiteSmoke, color, 60);
                        nextPanel.FillRectangle(brush, (column * blockSize) + 1, (row * blockSize) + 1, blockSize - 2, blockSize - 2);
                    }
        }

    }
}