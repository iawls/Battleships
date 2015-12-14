using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleships
{
    public partial class GameScreen : Form
    {
        private int listWindowWidth;
        private int boardWindowWidth;
        private int boardWindowHeight;
        private int boardOffset;
        private int cellWidth;
        private int boardSize = 10;
        private int posX = 0;
        private int posY = 0;
        private bool cursorVisible = true;
        private int turn;
        private int phase;
        private int shipSelected = -1; //no ship selected
        private int shipRotation = 0; //Horizontal = 0, Vertical = 1
        private int[] shipSizes = new int[10];
        private int shipModels;
        private List<Ship> shipListOne, shipListTwo;
        private GameEngine ge;
        private Board p1, p2;

        public GameScreen(GameEngine ge, Board p1, Board p2)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(Form_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(Form_MouseClick);

            this.ge = ge;
            this.p1 = p1;
            this.p2 = p2;
            boardSize = p1.getSize();
            shipListOne = p1.getShipList();
            shipListTwo = p2.getShipList();
            turn = 1;//ge.getTurn();
            phase = 1;
        }

        private int get_ship_size_from_click(int shipWidth)
        {
            if(posY > boardOffset && posY < (boardOffset + shipWidth))
            {
                return shipSizes[0]; 
            }else if (posY > (boardOffset+shipWidth*2) && posY < (boardOffset + shipWidth * 3))
            {
                return shipSizes[1];
            }
            else if (posY > (boardOffset + shipWidth * 4) && posY < (boardOffset + shipWidth * 5))
            {
                return shipSizes[2];
            }
            else if (posY > (boardOffset + shipWidth * 6) && posY < (boardOffset + shipWidth * 7))
            {
                return shipSizes[3];
            }
            else if (posY > (boardOffset + shipWidth * 8) && posY < (boardOffset + shipWidth * 9))
            {
                return shipSizes[4];
            }

            return 0;
        }

        private void right_click()
        {
            if (shipRotation == 0)
                shipRotation = 1;
            else
                shipRotation = 0;
        }

        private void left_click()
        {
            int shipListOffset = listWindowWidth / 10;
            int shipWidth = (listWindowWidth - shipListOffset * 2) / 6;
            /*
            * ship list click
            * left board click
            * right board click
            */
            if (posX < listWindowWidth && posY < boardWindowHeight)
            {
                if (phase == 1)
                {
                    if (posY < (boardOffset + (shipWidth * 2 * (shipModels - 1))) && posY > shipWidth)
                    {

                        int shipSize = get_ship_size_from_click(shipWidth);
                        if (turn == 1)
                        {
                            for (int i = 0; i < shipListOne.Count; i++)
                            {
                                if (shipListOne.ElementAt(i).getSize() == shipSize && shipListOne.ElementAt(i).getPlaced() == false)
                                {
                                    shipSelected = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < shipListTwo.Count; i++)
                            {
                                if (shipListTwo.ElementAt(i).getSize() == shipSize && shipListTwo.ElementAt(i).getPlaced() == false)
                                {
                                    shipSelected = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (posX > (listWindowWidth + boardOffset) && posX < (listWindowWidth + boardOffset + cellWidth * boardSize) && posY > boardOffset && posY < (boardOffset + cellWidth * boardSize))
            {

            }
            else if (posX > (listWindowWidth + boardWindowWidth + boardOffset) && posX < (listWindowWidth + boardWindowWidth + boardOffset + cellWidth * boardSize) && posY > boardOffset && posY < (boardOffset + cellWidth * boardSize))
            {

            }
        }

        private void Form_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                left_click();
            else
                right_click();
            
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            posX = e.X;
            posY = e.Y;
        }

        private void draw_ship_list(PaintEventArgs pe)
        {
            System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
            int shipSize = 0;
            int row = 0;
            int copies = 0;
            int shipListOffset = listWindowWidth / 10;
            int shipWidth = (listWindowWidth - shipListOffset*2)/6;
   
            if (turn == 1)
            {
                for (int i = 0; i < shipListOne.Count; i++)
                {
                    if (shipSize != shipListOne.ElementAt(i).getSize())
                    {
                        if (copies != 0)
                            //Controls.Add(new Label { Location = new Point(100, 100), AutoSize = true, Text = copies + "x" });

                        copies = 1;
                        shipSize = shipListOne.ElementAt(i).getSize();
                        for (int x = 0; x < shipSize; x++)
                        {
                            pe.Graphics.DrawRectangle(black, shipListOffset + shipWidth * x, boardOffset + (shipWidth * 2 * row), shipWidth, shipWidth);
                        }
                        shipSizes[row] = shipSize;
                        row++;
                    }
                    else
                    {
                        copies++;
                    }
                    shipSize = shipListOne.ElementAt(i).getSize();
                }
                shipModels = row + 1;
            }
            else
            {
                for (int i = 0; i < shipListTwo.Count; i++)
                {
                    if (shipSize != shipListOne.ElementAt(i).getSize())
                    {
                        if (copies != 0)
                            //Controls.Add(new Label { Location = new Point(100, 100), AutoSize = true, Text = copies + "x" });

                        copies = 1;
                        shipSize = shipListTwo.ElementAt(i).getSize();
                        for (int x = 0; x < shipSize; x++)
                        {
                            pe.Graphics.DrawRectangle(black, shipListOffset + shipWidth * x, boardOffset + (shipWidth * 2 * row), shipWidth, shipWidth);
                        }
                        shipSizes[row] = shipSize;
                        row++;
                    }
                    else
                    {
                        copies++;
                    }
                    shipSize = shipListTwo.ElementAt(i).getSize();
                }
            }
            shipModels = row + 1;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
            System.Drawing.Pen green = new System.Drawing.Pen(Color.LightGreen, 3);
            System.Drawing.SolidBrush fillBlue = new System.Drawing.SolidBrush(System.Drawing.Color.LightSkyBlue);
            System.Drawing.SolidBrush fillGray = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray);

            //draw windows
            pe.Graphics.DrawRectangle(black, 0, 0, listWindowWidth, boardWindowHeight);
            pe.Graphics.DrawRectangle(black, listWindowWidth, 0, boardWindowWidth, boardWindowHeight);
            pe.Graphics.DrawRectangle(black, listWindowWidth+boardWindowWidth, 0, boardWindowWidth-18, boardWindowHeight);

            int leftBoardStartX = listWindowWidth + boardOffset;
            int rightBoardStartX = listWindowWidth + boardWindowWidth + boardOffset;

            draw_ship_list(pe);

            //board to the left
            pe.Graphics.FillRectangle(fillBlue, leftBoardStartX, boardOffset, cellWidth*10, cellWidth * 10);
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    pe.Graphics.DrawRectangle(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                }
            }

            //board to the right
            pe.Graphics.FillRectangle(fillBlue, leftBoardStartX+boardWindowWidth, boardOffset, cellWidth * 10, cellWidth * 10);
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    pe.Graphics.DrawRectangle(black, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                }
            }

            //draw crosshair
            if(phase != 1)
            {
                if (posX > listWindowWidth && posX < listWindowWidth + boardWindowWidth && posY > 0 && posY < boardWindowHeight)
                {
                    if (cursorVisible)
                    {
                        Cursor.Hide();
                        cursorVisible = false;
                    }

                    pe.Graphics.DrawEllipse(green, posX - (cellWidth), posY - (cellWidth), cellWidth * 2, cellWidth * 2);
                    pe.Graphics.DrawLine(green, listWindowWidth, posY, listWindowWidth + boardWindowWidth, posY);
                    pe.Graphics.DrawLine(green, posX, 0, posX, boardWindowHeight);
                }
                else
                {
                    if (!cursorVisible)
                    {
                        Cursor.Show();
                        cursorVisible = true;
                    }
                }
            }
            else if(shipSelected != -1)
            {
                int shipSize;
                if (turn == 1)
                    shipSize = shipListOne.ElementAt(shipSelected).getSize();
                else
                    shipSize = shipListTwo.ElementAt(shipSelected).getSize();

                if (posX > (listWindowWidth + boardOffset) && posX < (listWindowWidth + boardOffset + cellWidth * boardSize) && posY > boardOffset && posY < (boardOffset + cellWidth * boardSize))
                {
                    int boardCol = (posX - (listWindowWidth + boardOffset)) / cellWidth;
                    int boardRow = (posY - boardOffset) / cellWidth;

                    for (int x = 0; x < shipSize; x++)
                    {
                        if (shipRotation == 0)
                        {
                            pe.Graphics.DrawRectangle(black, leftBoardStartX + (cellWidth * boardCol) + (cellWidth * x), boardOffset + (cellWidth * boardRow), cellWidth, cellWidth);
                            pe.Graphics.FillRectangle(fillGray, leftBoardStartX + (cellWidth * boardCol) + (cellWidth * x), boardOffset + (cellWidth * boardRow), cellWidth, cellWidth);
                        }
                        else
                        {
                            pe.Graphics.DrawRectangle(black, leftBoardStartX + (cellWidth * boardCol), boardOffset + (cellWidth * boardRow) + (cellWidth * x), cellWidth, cellWidth);
                            pe.Graphics.FillRectangle(fillGray, leftBoardStartX + (cellWidth * boardCol), boardOffset + (cellWidth * boardRow) + (cellWidth * x), cellWidth, cellWidth);
                        }
 
                    }
                }
                else
                {
                    for (int x = 0; x < shipSize; x++)
                    {
                        if(shipRotation == 0)
                        {
                            pe.Graphics.DrawRectangle(black, posX + cellWidth * x, posY, cellWidth, cellWidth);
                            pe.Graphics.FillRectangle(fillGray, posX + cellWidth * x, posY, cellWidth, cellWidth);
                        }
                        else
                        {
                            pe.Graphics.DrawRectangle(black, posX, posY + cellWidth*x, cellWidth, cellWidth);
                            pe.Graphics.FillRectangle(fillGray, posX, posY + cellWidth*x, cellWidth, cellWidth);
                        }
                    }
                }
            }

        }

        private void paint_timer_Tick(object sender, EventArgs e)
        {
            listWindowWidth = (int)(this.Width * 0.2);
            boardWindowWidth = (this.Width - listWindowWidth) / 2;
            boardWindowHeight = (this.Height / 100) * 75;
            if (boardWindowWidth > boardWindowHeight)
            {
                boardOffset = boardWindowHeight / boardSize;
                cellWidth = (boardWindowHeight - boardOffset * 2) / boardSize;
            }
            else
            {
                boardOffset = boardWindowWidth / boardSize;
                cellWidth = (boardWindowWidth - boardOffset * 2) / boardSize;
            }
            this.Refresh();
        }
    }
}
