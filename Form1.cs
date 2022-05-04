using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace LabDiffIvlev1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private double FuncKoshi(double x, double y)
        {
            return (3 * y - (5 * x * x + 3) * y * y * y) / (2 * x);
        }
        private double FuncAnalytic(double x)
        {
            return Math.Sqrt(1 / (x * x + 1));
        }
        private double[,] PlotAnalytic(double h)
        {
            double a = 1, b = 2;

            double x = a;
            int n = (int)((b - a) / h);
            double[,] array = new double[n, 2];
            for (int j = 0; j < n; j++)
            {
                double y = FuncAnalytic(x);
                
                
                array[j, 0] = x;
                array[j, 1] = y;
                chart1.Series[0].Points.AddXY(x, y);
                x += h;
            }
            return array;
        }

        private double[,] PlotRungeKutt(double h)
        {
            double a, b, x, y, g, k1, k2, k3, k4;
            a = 1; b = 2;
            x = a;
            y = 1 / Math.Sqrt(2);
            int n = (int)((b - a) / h);
            double[,] array = new double[n, 2];
            for (int j = 0; j < n; j++)
            {
                k1 = FuncKoshi(x, y);
                k2 = FuncKoshi(x + h / 2, y + (h * k1 / 2));
                k3 = FuncKoshi(x + h / 2, y + (h * k2 / 2));
                k4 = FuncKoshi(x + h, y + (h * k3));
                g = (h / 6) * (k1 + 2 * k2 + 2 * k3 + k4);
                y = (g + y);

                x = x + h;
                chart1.Series[2].Points.AddXY(x, y);
                array[j, 0] = x;
                array[j, 1] = y;
            }
            return array;
        }
        private double[,] PlotEuler(double h)
        {
            double a = 1, b = 2;
            double x = a;
            double y = 1 / Math.Sqrt(2);


            int n = (int)((b - a) / h);
            double[,] array = new double[n, 2];
            for (int j = 0; j < n; j++)
            {
                y = y + h * FuncKoshi(x, y);
                x = x + h;

                chart1.Series[1].Points.AddXY(x, y);
                array[j, 0] = x;
                array[j, 1] = y;
            }
            return array;
        }
        private static double maxDiff(double[,] one, double[,] two)
        {
            double maxDiff = 0;
            for(int i = 0; i < one.Length/2; i++)
            {
                if(Math.Abs(one[i,1] - two[i,1]) > maxDiff)
                    maxDiff = Math.Abs(one[i,1] - two[i,1]);
            }
            return maxDiff;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            double[,] Euler, RungeKutt, Analytic;
            double E = 0.0001;
            double h = 0.01;
            double maxDifference;
            do
            {
                h = 0.9 * h;
                Analytic = PlotAnalytic(h);
                RungeKutt = PlotRungeKutt(h);
                maxDifference = maxDiff(Analytic, RungeKutt);
                
             } while (maxDifference > E);


            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            Analytic = PlotAnalytic(h);
            RungeKutt = PlotRungeKutt(h);
            Euler = PlotEuler(h);

            DataTable table = new DataTable();
            table.Columns.Add("X", typeof(double));
            table.Columns.Add("Y", typeof(double));
            table.Columns.Add("Y Euler", typeof(double));
            table.Columns.Add("Dif Euler", typeof(double));
            table.Columns.Add("Y Runge", typeof(double));
            table.Columns.Add("Dif Runge", typeof(double));
            for (int i = 0; i < Analytic.Length/2; i++)
            {
                table.Rows.Add(Analytic[i,0], Analytic[i, 1], Euler[i,1], Math.Abs(Euler[i, 1] - Analytic[i, 1]), RungeKutt[i,1], Math.Abs(RungeKutt[i, 1] - Analytic[i, 1]));
            }
            dataGridView1.DataSource = table;
        }
    }
}
