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
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        public int buttonValue { get; set; }

        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void buttonPlayerVsPlayer_Click(object sender, EventArgs e)
        {
            buttonValue = 1;
            Close();
        }

        private void buttonPlayerVsPc_Click(object sender, EventArgs e)
        {
            buttonValue = 2;
            Close();
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            buttonValue = 3;
            Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            buttonValue = 4;
            Close();
        }
    }
}
