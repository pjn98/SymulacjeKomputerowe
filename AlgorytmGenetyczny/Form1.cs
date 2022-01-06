using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgorytmGenetyczny
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var poolSize = 8;
            var bitSize = 32;
            var leftBorder = -4;
            var rightBorder = 2;
            var iterations = 10;
            var tournamentSize = 4;
            var preservedSize = 4;
            var alg = new Algorithm();
            var population = alg.Simulation(poolSize, bitSize, iterations, leftBorder, rightBorder, preservedSize,
                tournamentSize);
        }
    }
}
