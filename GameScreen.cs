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
        GameEngine GE;
        Board p1;
        Board p2;
        List<Ship> p1ShipList;
        List<Ship> p2ShipList;

        public GameScreen(GameEngine GE, Board p1, Board p2)
        {
            this.GE = GE;
            this.p1 = p1;
            this.p2 = p2;
            p1ShipList = p1.rulebook.getShipList();
            p2ShipList = p2.rulebook.getShipList();
            InitializeComponent();
            //shipList = 
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
            int tableLayoutWidth = tableLayoutPanel2.GetColumnWidths()[0];
            int leftRightOffset = tableLayoutWidth / 10;
            int cellWidth = (tableLayoutWidth - (leftRightOffset * 3))/6;
            int rowSpacing = leftRightOffset;

            using (Graphics g = this.tableLayoutPanel2.CreateGraphics())
            {
                Pen pen = new Pen(Color.Black, 2);
                Brush brush = new SolidBrush(this.tableLayoutPanel2.BackColor);

                int shipSize = 0;
                int row = 0;
                int copies = 0;
                for (int i = 0; i < p1ShipList.Count; i++)
                {
                    if (shipSize != p1ShipList.ElementAt(i).getSize())
                    {
                        if(copies != 0)
                            Controls.Add(new Label { Location = new Point(100, 100), AutoSize = true, Text = copies + "x" });

                        copies = 1;
                        shipSize = p1ShipList.ElementAt(i).getSize();
                        for (int x = 0; x < shipSize; x++)
                        {
                            g.DrawRectangle(pen, (leftRightOffset*2)+(cellWidth*x), rowSpacing+((rowSpacing+cellWidth)*row), cellWidth, cellWidth);
                        }
                        row++;
                    }
                    else
                    {
                        copies++;
                    }
                    shipSize = p1ShipList.ElementAt(i).getSize();
                }
                pen.Dispose();
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {
            //board 1 (left)
            using (Graphics g = this.tableLayoutPanel3.CreateGraphics())
            {
                Pen pen = new Pen(Color.Black, 2);
                Brush brush = new SolidBrush(this.tableLayoutPanel3.BackColor);

                int boardSize = 10; //TODO: get size from rule book

                int tableLayoutWidth = tableLayoutPanel3.GetColumnWidths()[0];
                int firstRowHeight = tableLayoutPanel3.GetRowHeights()[0];
                int secondRowHeight = tableLayoutPanel3.GetRowHeights()[1];
                int leftRightOffset = tableLayoutWidth / 10;
                int topOffset = secondRowHeight / 15;

                int cellWidth;
                if (tableLayoutWidth < secondRowHeight)
                    cellWidth = (tableLayoutWidth - (leftRightOffset * 2)) / boardSize;
                else
                    cellWidth = (secondRowHeight - (topOffset * 2)) / boardSize;

                int startX = leftRightOffset;
                int startY = firstRowHeight;

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

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {
            //board 2 (right)
            using (Graphics g = this.tableLayoutPanel4.CreateGraphics())
            {
                Pen pen = new Pen(Color.Black, 2);
                Brush brush = new SolidBrush(this.tableLayoutPanel4.BackColor);

                int boardSize = 10; //TODO: get size from rule book

                int tableLayoutWidth = tableLayoutPanel4.GetColumnWidths()[0];
                int firstRowHeight = tableLayoutPanel4.GetRowHeights()[0];
                int secondRowHeight = tableLayoutPanel4.GetRowHeights()[1];
                int leftRightOffset = tableLayoutWidth/10;
                int topOffset = secondRowHeight / 15;

                int cellWidth;
                if (tableLayoutWidth < secondRowHeight)
                    cellWidth = (tableLayoutWidth - (leftRightOffset * 2)) / boardSize;
                else
                    cellWidth = (secondRowHeight - (topOffset * 2)) / boardSize;

                int startX = leftRightOffset;
                int startY = firstRowHeight;

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
