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
        public GameScreen()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
            using (Graphics g = this.tableLayoutPanel2.CreateGraphics())
            {
                Pen pen = new Pen(Color.Black, 2);
                Brush brush = new SolidBrush(this.tableLayoutPanel2.BackColor);

                int boardSize = 10; //TODO: get size from rule book

                int firstColumnWidth = tableLayoutPanel2.GetColumnWidths()[0];
                int secondColumnWidth = tableLayoutPanel2.GetColumnWidths()[1];
                int tableLayoutHeight = tableLayoutPanel2.GetRowHeights()[0];
                int leftRightOffset = secondColumnWidth / 10;
                int topOffset = tableLayoutHeight / 10;

                int cellWidth;
                if (secondColumnWidth < tableLayoutHeight)
                    cellWidth = (secondColumnWidth-(leftRightOffset*2))/boardSize;
                else
                    cellWidth = (tableLayoutHeight - (topOffset * 2)) / boardSize;

                //board 1 (left)
                int startX = firstColumnWidth + leftRightOffset;
                int startY = topOffset;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        g.DrawRectangle(pen, startX + (cellWidth*x), startY+(cellWidth*y), cellWidth, cellWidth);
                    }
                }

                //board 2 (right)
                startX = firstColumnWidth + secondColumnWidth + leftRightOffset;
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        g.DrawRectangle(pen, startX + (cellWidth * x), startY + (cellWidth * y), cellWidth, cellWidth);
                    }
                }
                pen.Dispose();
            }
        }
    }
}
