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

        public string buttonEvent { get; set; }

        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void buttonLoadGame_Click(object sender, EventArgs e)
        {
            buttonEvent = "LOAD_SAVED_GAME";
            Close();
        }

        private void buttonPlayerVsPlayer_Click(object sender, EventArgs e)
        {
            buttonEvent = "PLAYER_VS_PLAYER";
            Close();
        }

        private void buttonPlayerVsPc_Click(object sender, EventArgs e)
        {
            buttonEvent = "PLAYER_VS_PC";
            Close();
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            buttonEvent = "ABOUT";
            Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            buttonEvent = "EXIT";
            Close();
        }
    }
}
