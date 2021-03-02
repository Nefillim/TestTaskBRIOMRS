using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static TestTaskMoskvin.CalculationsManager;

namespace TestTaskMoskvin
{
    public class Locator
    {
        public Locator(double X, double Y)
        {
            x = X;
            y = Y;
        }
        public Point point { get { return P; } }
        private Point P = new Point();
        public double x { get => P.x; set => P.x = value; }
        public double y { get => P.y; set => P.y = value; }

        public double timeRange { get => TimeRange; set => TimeRange = value; }
        private double TimeRange;


    }
}
