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
        private List<Ship> shipListOne;
        private List<Ship> shipListTwo;
        private int turn;
        private int phase; //int?
        private int shipSelected = -1; //no ship selected

        public GameScreen()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(Form_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(Form_MouseClick);

            //load ship lists
        }

        private void Form_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            * ship list click
            * left board click
            * right board click
            */
            if(posX < listWindowWidth && posY < boardWindowHeight)
            {

            }else if (posX > (listWindowWidth+boardOffset) && posX < (listWindowWidth + boardOffset+cellWidth*boardSize) && posY > boardOffset && posY < (boardOffset+cellWidth*boardSize))
            {

            }
            else if (posX > (listWindowWidth + boardWindowWidth + boardOffset) && posX < (listWindowWidth + boardWindowWidth + boardOffset + cellWidth*boardSize) && posY > boardOffset && posY < (boardOffset + cellWidth * boardSize))
            {

            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            posX = e.X;
            posY = e.Y;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            System.Drawing.Pen black = new System.Drawing.Pen(Color.Black, 2);
            System.Drawing.Pen green = new System.Drawing.Pen(Color.LightGreen, 3);
            System.Drawing.SolidBrush fillBlue = new System.Drawing.SolidBrush(System.Drawing.Color.LightSkyBlue);

            //draw windows
            pe.Graphics.DrawRectangle(black, 0, 0, listWindowWidth, boardWindowHeight);
            pe.Graphics.DrawRectangle(black, listWindowWidth, 0, boardWindowWidth, boardWindowHeight);
            pe.Graphics.DrawRectangle(black, listWindowWidth+boardWindowWidth, 0, boardWindowWidth-18, boardWindowHeight);

            int leftBoardStartX = listWindowWidth + boardOffset;
            int rightBoardStartX = listWindowWidth + boardWindowWidth + boardOffset;

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
            //if(phase != placeShip)
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
