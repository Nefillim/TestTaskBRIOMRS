using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTaskMoskvin
{
    public partial class Form1 : Form
    {
        public string text = String.Empty;
        public List<CalculationsManager.Point> trajectory = new List<CalculationsManager.Point>();
        public CalculationsManager.Point lastPoint = new CalculationsManager.Point();

        public Locator[] locators = new Locator[3];

        Queue<double[]> timeRanges = new Queue<double[]>();

        public Form1()
        {
            InitializeComponent();
            Init();
            StartTracking();
            timer1.Interval = 1000;
            timer1.Start();
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            CalculationsManager.Point point = new CalculationsManager.Point();
            double temp = locators[0].point.x;
            if (!Double.TryParse(textBox2.Text, out locators[0].point.x)) locators[0].point.x = temp;
            temp = locators[0].point.y;
            if (!Double.TryParse(textBox3.Text, out locators[0].point.y)) locators[0].point.y = temp;
            temp = locators[1].point.x;
            if (!Double.TryParse(textBox4.Text, out locators[1].point.x)) locators[1].point.x = temp;
            temp = locators[1].point.y;
            if (!Double.TryParse(textBox5.Text, out locators[1].point.y)) locators[1].point.y = temp;
            temp = locators[2].point.x;
            if (!Double.TryParse(textBox6.Text, out locators[2].point.x)) locators[2].point.x = temp;
            temp = locators[2].point.y;
            if (!Double.TryParse(textBox7.Text, out locators[2].point.y)) locators[2].point.y = temp;
            temp = lastPoint.x;
            if (!Double.TryParse(textBox8.Text, out point.x)) point.x = temp;
            temp = lastPoint.y;
            if (!Double.TryParse(textBox9.Text, out point.y)) point.y = temp;

            lock (timeRanges) {
                timeRanges.Clear();
                double[] newTimeRanges = new double[3];
                for (int i = 0; i < newTimeRanges.Length; i++)
                {
                    newTimeRanges[i] = CalculationsManager.GetDistanceBetweenPoints(locators[i].point, point);
                }
                timeRanges.Enqueue(newTimeRanges);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox1.AppendText(text);
            DrawGrid();
            DrawLocators();
            DrawTrajectory();
        }


        private void DrawGrid()
        {
            Graphics g = pictureBox1.CreateGraphics();
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(255, 0, 0, 0)))
            {
                g.Clear(Color.White);
                for (int i = 0; i < 30; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        g.FillRectangle(sb, i * 15, j * 15, 1, 1);
                    }
                }
                g.DrawLine(new Pen(sb), new Point(225, 0), new Point(225, 450));
                g.DrawLine(new Pen(sb), new Point(0, 225), new Point(450, 225));
            }
        }

        private void DrawLocators()
        {
            Graphics g = pictureBox1.CreateGraphics();
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(255, 250, 50, 20)))
            {
                for (int i = 0; i < locators.Length; i++)
                {
                    g.FillRectangle(sb, (int)(locators[i].x * 15.0) + 225, -(int)(locators[i].y * 15.0) + 225, 5, 5);
                }
            }
        }

        private void DrawTrajectory()
        {
            Graphics g = pictureBox1.CreateGraphics();
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(255, 50, 50, 200)))
            {
                foreach (var item in trajectory)
                {
                    g.FillRectangle(sb, (int)(item.x * 15.0) + 225, -(int)(item.y * 15) + 225, 3, 3);
                }

            }
        }



        readonly string input = @"..\..\..\input.txt";
        readonly string customOutput = @"..\..\..\customOutput.txt";
        readonly string output = @"..\..\..\output.txt";



        void Init()
        {
            try
            {
                using (var reader = new StreamReader(new FileStream(input, FileMode.Open, FileAccess.Read)))
                {
                    string[] points = reader.ReadLine().Split(",");
                    double[] parsedPoints = new double[points.Length];
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = points[i].Replace('.', ',');
                        Double.TryParse(points[i], out parsedPoints[i]);
                        if (i % 2 == 1)
                        {
                            locators[i / 2] = new Locator(parsedPoints[i - 1], parsedPoints[i]);
                        }
                    }
                    while (!reader.EndOfStream)
                    {
                        string[] timeranges = reader.ReadLine().Split(",");
                        double[] parsedTimeranges = new double[timeranges.Length];
                        for (int i = 0; i < timeranges.Length; i++)
                        {
                            timeranges[i] = timeranges[i].Replace('.', ',');
                            Double.TryParse(timeranges[i], out parsedTimeranges[i]);
                            parsedTimeranges[i] *= 1000000.00d;
                        }
                        timeRanges.Enqueue(parsedTimeranges);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }


        void StartTracking()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        CalculationsManager.Point position = new CalculationsManager.Point();
                        if (timeRanges.Count > 0)
                        {
                            double[] points = timeRanges.Dequeue();
                            /*for (int i = 0; i < 3; i++)
                            {
                                text += points[i] + ";   " + '\r' + '\n';
                            }*/
                            position = CalculationsManager.FindTarget(points, locators);
                            position.Write(this);
                            trajectory.Add(position);
                            lastPoint = position;
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var writer = new StreamWriter(new FileStream(customOutput, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                string locatorsCoords = ((decimal)locators[0].x).ToString().Replace(',', '.') + ',' +
                    ((decimal)locators[0].y).ToString().Replace(',', '.') + ',' +
                    ((decimal)locators[1].x).ToString().Replace(',', '.') + ',' +
                    ((decimal)locators[1].y).ToString().Replace(',', '.') + ',' +
                    ((decimal)locators[2].x).ToString().Replace(',', '.') + ',' +
                    ((decimal)locators[2].y).ToString().Replace(',', '.');
                writer.WriteLine(locatorsCoords);
                foreach (var item in trajectory)
                {
                    writer.WriteLine((CalculationsManager.GetDistanceBetweenPoints(item, locators[0].point) / 1_000_000).ToString().Replace(',', '.') + "," +
                        (CalculationsManager.GetDistanceBetweenPoints(item, locators[1].point) / 1_000_000).ToString().Replace(',', '.') + "," +
                        (CalculationsManager.GetDistanceBetweenPoints(item, locators[2].point) / 1_000_000).ToString().Replace(',', '.'));
                }
            }
            using (var writer = new StreamWriter(new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                foreach (var item in trajectory)
                {
                    writer.WriteLine((item.x).ToString().Replace(',', '.') + "," +
                        (item.y).ToString().Replace(',', '.'));
                }
            }
        }
    }
}
