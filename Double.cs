﻿using System;

namespace SpaceSimulation
{
    [Serializable]
    public struct Double2
    {
        public double x;
        public double y;

        public static Double2 zero => new Double2(0, 0);

        public Double2(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public static Double2 operator +(Double2 a, Double2 b) => new Double2(a.x + b.x, a.y + b.y);
        public static Double2 operator -(Double2 a, Double2 b) => new Double2(a.x - b.x, a.y - b.y);
        public static Double2 operator *(Double2 a, double b) => new Double2(a.x * b, a.y * b);
        public static Double2 operator /(Double2 a, double b) => new Double2(a.x / b, a.y / b);

        public double magnitude
        {
            get
            {
                return Math.Sqrt((x * x) + (y * y));
            }
        }
        public Double2 normalized
        {
            get
            {
                var mag = magnitude;

                return new Double2(x, y) / mag;
            }
        }

        public override string ToString()
        {
            return x + ", " + y;
        }
    }

    public struct Double
    {
        public static double Lerp(double a, double b, double t)
        {
            var value = a + ((b - a) / t);

            if (double.IsNaN(value))
                return a;
            else
                return value;
        }
    }
}
