using System;
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

        private async void button1_Click(object sender, EventArgs e)
        {
            var poolSize = 9;
            var preservedSize = 4;
            var newGenotypeSize = poolSize - preservedSize;
            var bitSize = 31;
            var leftBorder = -4;
            var rightBorder = 2;
            var iterations = 200;
            var tournamentSize = 4;
            var alg = new Algorithm();
            var populations = alg.Simulation(poolSize, bitSize, iterations, leftBorder, rightBorder, preservedSize,
                tournamentSize, newGenotypeSize);

            var chart = new ChartHelper();
            foreach (var population in populations)
            {
                chart.GenerateChart(chart1, population);
                await Task.Delay(200);
            }
        }
    }
}
