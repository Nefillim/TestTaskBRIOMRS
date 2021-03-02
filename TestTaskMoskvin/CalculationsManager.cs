using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TestTaskMoskvin
{
    public static class CalculationsManager
    {
        public class Point
        {
            public Point() { }
            public void Write(Form1 form)
            {
                form.text += "x : " + x + "; " + "y : " + y + ";";
                form.text += '\r';
                form.text += '\n';
            }
            public Point(double X, double Y) => (x, y) = (X, Y);
            public double x;
            public double y;
        }

        public static double GetDistanceBetweenPoints(Point first,Point second)
        {
            return Math.Sqrt((first.x - second.x) * (first.x - second.x) + (first.y - second.y) * (first.y - second.y));
        }

        public static double GetRadius(Point[] points)
        {
            double sideA = GetDistanceBetweenPoints(points[0],points[1]);
            double sideB = GetDistanceBetweenPoints(points[2], points[1]);
            double sideC = GetDistanceBetweenPoints(points[2], points[0]);
            return sideA * sideB * sideC / Math.Sqrt((sideA + sideB + sideC) * (-sideA + sideB + sideC) * (sideA - sideB + sideC) * (sideA + sideB - sideC));
        }

        public static bool IsSamePoint(Point[] points)
        {
            return GetDistanceBetweenPoints(points[0],points[1]) < 0.05d && GetDistanceBetweenPoints(points[1],points[2]) < 0.05d && GetDistanceBetweenPoints(points[2], points[0]) < 0.05d;
        }


        public static Point GetCircleCenter(Point[] points)
        {
            if (IsSamePoint(points))
                return points[0];

            Point result = new Point();
            result.x = ((points[0].x * points[0].x + points[0].y * points[0].y) * (points[1].y - points[2].y)
                + (points[1].x * points[1].x + points[1].y * points[1].y) * (points[2].y - points[0].y)
                + (points[2].x * points[2].x + points[2].y * points[2].y) * (points[0].y - points[1].y)) / 
                (2.00d * (points[0].x * (points[1].y - points[2].y) + points[1].x * (points[2].y - points[0].y) + points[2].x * (points[0].y - points[1].y)));
            
            result.y = ((points[0].x * points[0].x + points[0].y * points[0].y) * (points[2].x - points[1].x)
                + (points[1].x * points[1].x + points[1].y * points[1].y) * (points[0].x - points[2].x)
                + (points[2].x * points[2].x + points[2].y * points[2].y) * (points[1].x - points[0].x)) /
                (2.00d * (points[0].x * (points[1].y - points[2].y) + points[1].x * (points[2].y - points[0].y) + points[2].x * (points[0].y - points[1].y)));

            return result;
        }
        public static Point FindTarget(double[] ranges, Locator[] locators)
        {
            Point[] intersectionPoints = new Point[6];
            double[] heights = new double[3];
            for (int i = 0; i < ranges.Length; i++)
            {
                intersectionPoints[2 * i] = new Point();
                intersectionPoints[2 * i + 1] = new Point();

                int j = i + 1;
                if (j == locators.Length)
                    j = 0;

                double distanceBetweenLocators = GetDistanceBetweenPoints(locators[i].point, locators[j].point);

                if (distanceBetweenLocators > ranges[i] + ranges[j])
                {
                    ranges[i] += (distanceBetweenLocators - ranges[i] - ranges[j]) / 2.00d;
                    ranges[j] += (distanceBetweenLocators - ranges[i] - ranges[j]) / 2.00d;
                }

                double rangeToHeight = (ranges[i] * ranges[i] - ranges[j] * ranges[j] + distanceBetweenLocators * distanceBetweenLocators) / (2.00d * distanceBetweenLocators);

                if (rangeToHeight < ranges[i])
                    heights[i] = Math.Sqrt(ranges[i] * ranges[i] - rangeToHeight * rangeToHeight);
                else
                    heights[i] = 0.00d;

                Point heightPoint = new Point();
                heightPoint.x = locators[i].x + (rangeToHeight / distanceBetweenLocators) * (locators[j].x - locators[i].x);
                heightPoint.y = locators[i].y + (rangeToHeight / distanceBetweenLocators) * (locators[j].y - locators[i].y);
                

                intersectionPoints[2 * i].x = heightPoint.x + ((locators[j].y - locators[i].y) * heights[i] / distanceBetweenLocators);
                intersectionPoints[2 * i].y = heightPoint.y - (locators[j].x - locators[i].x) * heights[i] / distanceBetweenLocators;

                intersectionPoints[2 * i + 1].x = heightPoint.x - (locators[j].y - locators[i].y) * heights[i] / distanceBetweenLocators;
                intersectionPoints[2 * i + 1].y = heightPoint.y + (locators[j].x - locators[i].x) * heights[i] / distanceBetweenLocators;
            }

            Point[][] resultPoints = new Point[][] {
                new Point[3],
                new Point[3]
                };


            for (int i = 0; i < 2; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    resultPoints[i][0] = intersectionPoints[i];
                    resultPoints[i][j] = GetDistanceBetweenPoints(intersectionPoints[j * 2], intersectionPoints[i]) < 
                        GetDistanceBetweenPoints(intersectionPoints[j * 2 + 1], intersectionPoints[i]) ?
                        intersectionPoints[j * 2] : intersectionPoints[j * 2 + 1];
                }
            }

            double firstRadius = GetRadius(resultPoints[0]);
            double secondRadius = GetRadius(resultPoints[1]);

            Point firstTarget = new Point();
            firstTarget = GetCircleCenter(resultPoints[0]);

            Point secondTarget = new Point();
            secondTarget = GetCircleCenter(resultPoints[1]);

            return firstRadius < secondRadius ? firstTarget : secondTarget;
            
        }
    }
}
