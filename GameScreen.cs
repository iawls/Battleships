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
        private bool hidePlayerBoards = false;
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
            turn = ge.getTurn();
            phase = ge.getPhase();
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
            * bottom window click
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
                if(shipSelected != -1)
                {
                    int shipSize;
                    if (turn == 1)
                        shipSize = shipListOne.ElementAt(shipSelected).getSize();
                    else
                        shipSize = shipListTwo.ElementAt(shipSelected).getSize();

                    int boardCol = (posX - (listWindowWidth + boardOffset)) / cellWidth;
                    int boardRow = (posY - boardOffset) / cellWidth;

                    if (boardCol + shipSize > boardSize && shipRotation == 0)
                        boardCol = boardCol + boardSize - (boardCol + shipSize);
                    else if (boardRow + shipSize > boardSize && shipRotation == 1)
                        boardRow = boardRow + boardSize - (boardRow + shipSize);

                    Tuple<int, int> startPos = new Tuple<int, int>(boardCol, boardRow);
                    Tuple<int, int> endPos;
                    if (shipRotation == 0)
                        endPos = new Tuple<int, int>(boardCol+shipSize-1, boardRow);
                    else
                       endPos = new Tuple<int, int>(boardCol, boardRow+shipSize-1);

                    if (turn == 1)
                    {
                        if(p1.placeShip(startPos, endPos, shipListOne.ElementAt(shipSelected)))
                        {
                            if (shipSelected < shipListOne.Count - 1)
                                if (shipListOne.ElementAt(shipSelected).getSize() == shipListOne.ElementAt(shipSelected + 1).getSize())
                                    shipSelected++;
                                else
                                    shipSelected = -1;
                            else
                                shipSelected = -1;
                        }
                    }
                    else
                    {
                        if (p2.placeShip(startPos, endPos, shipListTwo.ElementAt(shipSelected)))
                        {
                            if (shipSelected < shipListTwo.Count - 1)
                                if (shipListTwo.ElementAt(shipSelected).getSize() == shipListTwo.ElementAt(shipSelected + 1).getSize())
                                    shipSelected++;
                                else
                                    shipSelected = -1;
                            else
                                shipSelected = -1;
                        }
                    }
                }
            }
            else if (posX > (listWindowWidth + boardWindowWidth + boardOffset) && posX < (listWindowWidth + boardWindowWidth + boardOffset + cellWidth * boardSize) && posY > boardOffset && posY < (boardOffset + cellWidth * boardSize))
            {
                if (phase == 2)
                {
                    int boardCol = (posX - (listWindowWidth + boardWindowWidth + boardOffset)) / cellWidth;
                    int boardRow = (posY - boardOffset) / cellWidth;

                    if (turn == 1)
                        p2.fire(boardCol, boardRow);
                    else
                        p1.fire(boardCol, boardRow);
                }
            }
            else if (posY > boardWindowHeight)
            {
                int buttonSize = (int)((this.Height - boardWindowHeight) * 0.5);
                if(posX > this.Width - buttonSize - buttonSize / 2 && posY > boardWindowHeight + buttonSize / 2)
                {
                    if(hidePlayerBoards == false)
                    {
                        ge.nextTurn();
                        if(ge.getWin() != 0)
                        {
                            System.Windows.Forms.MessageBox.Show("Player " + ge.getWin() + " wins!");
                            Close();
                        }
                        else
                        {
                            if (turn != ge.getTurn())
                                hidePlayerBoards = true;
                            else
                                phase = ge.getPhase();
                        }
                    }
                    else
                    {
                        hidePlayerBoards = false;
                        turn = ge.getTurn();
                        phase = ge.getPhase();
                    }
                }
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
            if (!hidePlayerBoards)
            {
                System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
                System.Drawing.SolidBrush fillBlack = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

                int shipSize = 0;
                int row = 0;
                int copies = 0;
                int shipListOffset = listWindowWidth / 10;
                int shipWidth = (listWindowWidth - shipListOffset * 2) / 6;
                bool last = true;


                RectangleF textRect = new RectangleF(0, 0, listWindowWidth, boardOffset);
                Font textFont = new Font("Arial", 20);
                try
                {
                    textFont = new Font("Arial", shipListOffset / 2);
                }
                catch (ArgumentException e)
                {
                    textFont = new Font("Arial", 20);
                }
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                if (phase == 1)
                    pe.Graphics.DrawString("Your undeployed ships", textFont, fillBlack, textRect, stringFormat);
                else
                    pe.Graphics.DrawString("Enemy ships on the battlefield", textFont, fillBlack, textRect, stringFormat);


                if (phase == 1)
                {
                    if (turn == 1)
                    {
                        for (int i = 0; i < shipListOne.Count; i++)
                        {
                            if (shipListOne.ElementAt(i).getPlaced() == false)
                            {
                                if (shipSize != shipListOne.ElementAt(i).getSize())
                                {
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

                                if (i < shipListOne.Count - 1)
                                {
                                    if (shipSize != shipListOne.ElementAt(i + 1).getSize())
                                    {
                                        textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                        float textSize = shipListOffset / 2;
                                        try
                                        {
                                            textFont = new Font("Arial", textSize);
                                        }
                                        catch (ArgumentException e)
                                        {
                                            textFont = new Font("Arial", 20);
                                        }
                                        stringFormat = new StringFormat();
                                        stringFormat.Alignment = StringAlignment.Center;
                                        stringFormat.LineAlignment = StringAlignment.Center;
                                        pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                    }
                                }
                                else
                                {
                                    textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                    float textSize = shipListOffset;
                                    try
                                    {
                                        textFont = new Font("Arial", textSize / 2);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        textFont = new Font("Arial", 20);
                                    }
                                    stringFormat = new StringFormat();
                                    stringFormat.Alignment = StringAlignment.Center;
                                    stringFormat.LineAlignment = StringAlignment.Center;
                                    pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                }

                                shipSize = shipListOne.ElementAt(i).getSize();

                            }
                            shipModels = row + 1;
                        }

                    }
                    else
                    {
                        for (int i = 0; i < shipListTwo.Count; i++)
                        {
                            if (shipListTwo.ElementAt(i).getPlaced() == false)
                            {

                                if (shipSize != shipListTwo.ElementAt(i).getSize())
                                {

                                    if (shipListTwo.ElementAt(i).getPlaced() == false)
                                    {
                                        copies = 1;
                                        shipSize = shipListTwo.ElementAt(i).getSize();
                                        for (int x = 0; x < shipSize; x++)
                                        {
                                            pe.Graphics.DrawRectangle(black, shipListOffset + shipWidth * x, boardOffset + (shipWidth * 2 * row), shipWidth, shipWidth);
                                        }
                                        shipSizes[row] = shipSize;
                                        row++;
                                    }
                                }
                                else
                                {
                                    copies++;
                                }


                                if (i < shipListTwo.Count - 1)
                                {
                                    if (shipSize != shipListTwo.ElementAt(i + 1).getSize())
                                    {
                                        textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                        float textSize = shipListOffset / 2;
                                        try
                                        {
                                            textFont = new Font("Arial", textSize);
                                        }
                                        catch (ArgumentException e)
                                        {
                                            textFont = new Font("Arial", 20);
                                        }
                                        stringFormat = new StringFormat();
                                        stringFormat.Alignment = StringAlignment.Center;
                                        stringFormat.LineAlignment = StringAlignment.Center;
                                        pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                    }
                                }
                                else
                                {
                                    textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                    float textSize = shipListOffset;
                                    try
                                    {
                                        textFont = new Font("Arial", textSize / 2);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        textFont = new Font("Arial", 20);
                                    }
                                    stringFormat = new StringFormat();
                                    stringFormat.Alignment = StringAlignment.Center;
                                    stringFormat.LineAlignment = StringAlignment.Center;
                                    pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                }

                                shipSize = shipListTwo.ElementAt(i).getSize();
                            }
                            shipModels = row + 1;
                        }
                    }
                }
                else
                {
                    if (turn == 1)
                    {
                        for (int i = 0; i < shipListTwo.Count; i++)
                        {
                            if (shipListTwo.ElementAt(i).getDead() == false)
                            {
                                if (shipSize != shipListTwo.ElementAt(i).getSize())
                                {
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

                                if (i < shipListTwo.Count - 1)
                                {
                                    for (int j = i + 1; j < shipListTwo.Count; j++)
                                    {
                                        if (shipListTwo.ElementAt(j).getSize() == shipSize && shipListTwo.ElementAt(j).getDead() == false)
                                            last = false;
                                    }
                                    if (last)
                                    {
                                        textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                        float textSize = shipListOffset / 2;
                                        try
                                        {
                                            textFont = new Font("Arial", textSize);
                                        }
                                        catch (ArgumentException e)
                                        {
                                            textFont = new Font("Arial", 20);
                                        }
                                        stringFormat = new StringFormat();
                                        stringFormat.Alignment = StringAlignment.Center;
                                        stringFormat.LineAlignment = StringAlignment.Center;
                                        pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                    }
                                    last = true;
                                }
                                else
                                {
                                    textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                    float textSize = shipListOffset;
                                    try
                                    {
                                        textFont = new Font("Arial", textSize / 2);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        textFont = new Font("Arial", 20);
                                    }
                                    stringFormat = new StringFormat();
                                    stringFormat.Alignment = StringAlignment.Center;
                                    stringFormat.LineAlignment = StringAlignment.Center;
                                    pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                }

                                shipSize = shipListTwo.ElementAt(i).getSize();

                            }
                            shipModels = row + 1;
                        }

                    }
                    else
                    {
                        for (int i = 0; i < shipListOne.Count; i++)
                        {
                            if (shipListOne.ElementAt(i).getDead() == false)
                            {

                                if (shipSize != shipListOne.ElementAt(i).getSize())
                                {
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


                                if (i < shipListOne.Count - 1)
                                {
                                    if (shipSize != shipListOne.ElementAt(i + 1).getSize())
                                    {
                                        textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                        float textSize = shipListOffset / 2;
                                        try
                                        {
                                            textFont = new Font("Arial", textSize);
                                        }
                                        catch (ArgumentException e)
                                        {
                                            textFont = new Font("Arial", 20);
                                        }
                                        stringFormat = new StringFormat();
                                        stringFormat.Alignment = StringAlignment.Center;
                                        stringFormat.LineAlignment = StringAlignment.Center;
                                        pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                    }
                                }
                                else
                                {
                                    textRect = new RectangleF(0, boardOffset + (shipWidth * 2 * (row - 1)), shipListOffset, shipWidth);
                                    float textSize = shipListOffset;
                                    try
                                    {
                                        textFont = new Font("Arial", textSize / 2);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        textFont = new Font("Arial", 20);
                                    }
                                    stringFormat = new StringFormat();
                                    stringFormat.Alignment = StringAlignment.Center;
                                    stringFormat.LineAlignment = StringAlignment.Center;
                                    pe.Graphics.DrawString(copies + "x", textFont, fillBlack, textRect, stringFormat);
                                }

                                shipSize = shipListOne.ElementAt(i).getSize();
                            }
                            shipModels = row + 1;
                        }
                    }
                }
            }
        }

        private void draw_frames(PaintEventArgs pe)
        {
            System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
            pe.Graphics.DrawRectangle(black, 0, 0, listWindowWidth, boardWindowHeight);
            pe.Graphics.DrawRectangle(black, listWindowWidth, 0, boardWindowWidth, boardWindowHeight);
            pe.Graphics.DrawRectangle(black, listWindowWidth + boardWindowWidth, 0, boardWindowWidth - 18, boardWindowHeight);
        }

        private void draw_turn_button(PaintEventArgs pe)
        {
            System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
            System.Drawing.SolidBrush fillGray = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray);
            System.Drawing.SolidBrush fillBlack = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            int buttonSize = (int)((this.Height - boardWindowHeight) * 0.5);
            RectangleF textRect = new RectangleF(this.Width - buttonSize - buttonSize / 2, boardWindowHeight + buttonSize / 2, buttonSize, buttonSize);
            Rectangle buttonRect = new Rectangle(this.Width - buttonSize - buttonSize / 2, boardWindowHeight + buttonSize / 2, buttonSize, buttonSize);
            pe.Graphics.DrawRectangle(black, buttonRect);
            pe.Graphics.FillRectangle(fillGray, buttonRect);
            Font textFont = new Font("Arial", 20);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            if (hidePlayerBoards == false)
                pe.Graphics.DrawString("Turn done", textFont, fillBlack, textRect, stringFormat);
            else
                pe.Graphics.DrawString("Enter turn", textFont, fillBlack, textRect, stringFormat);
        }

        private void draw_boards(PaintEventArgs pe)
        {
            System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
            System.Drawing.Pen red = new System.Drawing.Pen(Color.Red, 2);
            System.Drawing.SolidBrush fillBlack = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.SolidBrush fillBlue = new System.Drawing.SolidBrush(System.Drawing.Color.LightSkyBlue);

            int leftBoardStartX = listWindowWidth + boardOffset;
            int rightBoardStartX = listWindowWidth + boardWindowWidth + boardOffset;

            //board to the left
            RectangleF textRect = new RectangleF(listWindowWidth, 0, boardWindowWidth, boardOffset);
            Font textFont = new Font("Arial", 20);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            try
            {
                textFont = new Font("Arial", boardOffset / 2);
            }
            catch (ArgumentException e)
            {
                textFont = new Font("Arial", 20);
            }
            if (turn == 1)
                if (hidePlayerBoards)
                    pe.Graphics.DrawString("Player 2", textFont, fillBlack, textRect, stringFormat);
                else
                    pe.Graphics.DrawString("Player 1", textFont, fillBlack, textRect, stringFormat);
            else
                if (hidePlayerBoards)
                pe.Graphics.DrawString("Player 1", textFont, fillBlack, textRect, stringFormat);
            else
                pe.Graphics.DrawString("Player 2", textFont, fillBlack, textRect, stringFormat);

            pe.Graphics.FillRectangle(fillBlue, leftBoardStartX, boardOffset, cellWidth * 10, cellWidth * 10);
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    pe.Graphics.DrawRectangle(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                    if (turn == 1 && hidePlayerBoards == false)
                    {
                        if (p1.isHit(x, y) == true && p1.isShip(x, y) == true)
                        {
                            pe.Graphics.DrawEllipse(red, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                        }
                        else if (p1.isHit(x, y) == true && p1.isShip(x, y) == false)
                        {
                            pe.Graphics.DrawLine(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), leftBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y) + cellWidth);
                            pe.Graphics.DrawLine(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y) + cellWidth, leftBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y));
                        }
                        else if (p1.isShip(x, y) == true)
                        {
                            pe.Graphics.DrawEllipse(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                        }
                    }
                    else if (turn == 2 && hidePlayerBoards == false)
                    {
                        if (p2.isHit(x, y) == true && p2.isShip(x, y) == true)
                        {
                            pe.Graphics.DrawEllipse(red, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                        }
                        else if (p2.isHit(x, y) == true && p2.isShip(x, y) == false)
                        {
                            pe.Graphics.DrawLine(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), leftBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y) + cellWidth);
                            pe.Graphics.DrawLine(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y) + cellWidth, leftBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y));
                        }
                        else if (p2.isShip(x, y) == true)
                        {
                            pe.Graphics.DrawEllipse(black, leftBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                        }
                    }
                }
            }

            //board to the right
            textRect = new RectangleF(listWindowWidth + boardWindowWidth, 0, boardWindowWidth, boardOffset);
            pe.Graphics.DrawString("Enemy", textFont, fillBlack, textRect, stringFormat);

            pe.Graphics.FillRectangle(fillBlue, leftBoardStartX + boardWindowWidth, boardOffset, cellWidth * 10, cellWidth * 10);
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    pe.Graphics.DrawRectangle(black, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                    if (turn == 1 && hidePlayerBoards == false)
                    {
                        if (p2.isHit(x, y) == true && p2.isShip(x, y) == true)
                        {
                            pe.Graphics.DrawEllipse(red, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                        }
                        else if (p2.isHit(x, y) == true && p2.isShip(x, y) == false)
                        {
                            pe.Graphics.DrawLine(black, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), rightBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y) + cellWidth);
                            pe.Graphics.DrawLine(black, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y) + cellWidth, rightBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y));
                        }
                    }
                    else if (turn == 2 && hidePlayerBoards == false)
                    {
                        if (p1.isHit(x, y) == true && p1.isShip(x, y) == true)
                        {
                            pe.Graphics.DrawEllipse(red, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), cellWidth, cellWidth);
                        }
                        else if (p1.isHit(x, y) == true && p1.isShip(x, y) == false)
                        {
                            pe.Graphics.DrawLine(black, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y), rightBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y) + cellWidth);
                            pe.Graphics.DrawLine(black, rightBoardStartX + (cellWidth * x), boardOffset + (cellWidth * y) + cellWidth, rightBoardStartX + (cellWidth * x) + cellWidth, boardOffset + (cellWidth * y));
                        }
                    }
                }
            }
        }

        private void draw_crosshair(PaintEventArgs pe)
        {
            System.Drawing.Pen green = new System.Drawing.Pen(Color.LightGreen, 3);

            if (phase == 2)
            {
                if (posX > listWindowWidth + boardWindowWidth && posX < listWindowWidth + boardWindowWidth * 2 && posY > 0 && posY < boardWindowHeight)
                {
                    if (cursorVisible)
                    {
                        Cursor.Hide();
                        cursorVisible = false;
                    }

                    pe.Graphics.DrawEllipse(green, posX - (cellWidth), posY - (cellWidth), cellWidth * 2, cellWidth * 2);
                    pe.Graphics.DrawLine(green, listWindowWidth + boardWindowWidth, posY, listWindowWidth + boardWindowWidth * 2, posY);
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
        }

        private void draw_selected_ship(PaintEventArgs pe)
        {
            System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
            System.Drawing.SolidBrush fillGray = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray);

            int leftBoardStartX = listWindowWidth + boardOffset;
            int rightBoardStartX = listWindowWidth + boardWindowWidth + boardOffset;

            if (shipSelected != -1)
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

                    if (boardCol + shipSize > boardSize && shipRotation == 0)
                        boardCol = boardCol + boardSize - (boardCol + shipSize);
                    else if (boardRow + shipSize > boardSize && shipRotation == 1)
                        boardRow = boardRow + boardSize - (boardRow + shipSize);

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
                        if (shipRotation == 0)
                        {
                            pe.Graphics.DrawRectangle(black, posX + cellWidth * x, posY, cellWidth, cellWidth);
                            pe.Graphics.FillRectangle(fillGray, posX + cellWidth * x, posY, cellWidth, cellWidth);
                        }
                        else
                        {
                            pe.Graphics.DrawRectangle(black, posX, posY + cellWidth * x, cellWidth, cellWidth);
                            pe.Graphics.FillRectangle(fillGray, posX, posY + cellWidth * x, cellWidth, cellWidth);
                        }
                    }
                }
            }
        }

        private void draw_info_box(PaintEventArgs pe)
        {
            System.Drawing.SolidBrush fillGray = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray);
            System.Drawing.SolidBrush fillBlack = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            Rectangle rect = new Rectangle(5, boardWindowHeight + 5, listWindowWidth -5, (this.Height - boardWindowHeight)-50);
            RectangleF textRect = new RectangleF(listWindowWidth / 20 + 5, boardWindowHeight + listWindowWidth / 8, listWindowWidth - listWindowWidth / 20 - 5, this.Height - boardWindowHeight);
            pe.Graphics.FillRectangle(fillGray, rect);
            Font textFont = new Font("Arial", 20);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Near;
            try
            {
                textFont = new Font("Arial", listWindowWidth / 20);
            }
            catch (ArgumentException e)
            {
                textFont = new Font("Arial", 20);
            }

            if (phase == 1)
            {
                if (!hidePlayerBoards)
                    pe.Graphics.DrawString("Player " + turn + "'s turn:\r\n\r\nPlace your undeployed ships on your board. Right click to rotate a ship.", textFont, fillBlack, textRect, stringFormat);
                else
                {
                    if (turn == 1)
                        pe.Graphics.DrawString("Player 2's turn:\r\n\r\nPlease enter your turn by clicking on the 'Enter turn' button.", textFont, fillBlack, textRect, stringFormat);
                    else
                        pe.Graphics.DrawString("Player 1's turn:\r\n\r\nPlease enter your turn by clicking on the 'Enter turn' button.", textFont, fillBlack, textRect, stringFormat);
                }
            }
            else
            {
                if (!hidePlayerBoards)
                    pe.Graphics.DrawString("Player " + turn + "'s turn:\r\n\r\nAttack enemy board. Let no ship survive!", textFont, fillBlack, textRect, stringFormat);
                else
                {
                    if (turn == 1)
                        pe.Graphics.DrawString("Player 2's turn:\r\n\r\nPlease enter your turn by clicking on the 'Enter turn' button.", textFont, fillBlack, textRect, stringFormat);
                    else
                        pe.Graphics.DrawString("Player 1's turn:\r\n\r\nPlease enter your turn by clicking on the 'Enter turn' button.", textFont, fillBlack, textRect, stringFormat);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            draw_frames(pe);
            draw_ship_list(pe);
            draw_turn_button(pe);
            draw_boards(pe);
            draw_crosshair(pe);
            draw_selected_ship(pe);
            draw_info_box(pe);

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
