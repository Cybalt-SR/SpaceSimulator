using System;

namespace SpaceSimulation
{
    [Serializable]
    public struct Double2
    {
        public double x;
        public double y;

        public static Double2 zero => new Double2(0, 0);
        public static Double2 one => new Double2(1, 1);
        public static Double2 perpendicular => new Double2(-1, 1);

        public Double2(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public static Double2 operator +(Double2 a, Double2 b) => new Double2(a.x + b.x, a.y + b.y);
        public static Double2 operator -(Double2 a, Double2 b) => new Double2(a.x - b.x, a.y - b.y);
        public static Double2 operator *(Double2 a, double b) => new Double2(a.x * b, a.y * b);
        public static Double2 operator /(Double2 a, double b) => new Double2(a.x / b, a.y / b);

        public double sqrmagnitude
        {
            get
            {
                return (x * x) + (y * y);
            }
        }
        public double magnitude
        {
            get
            {
			// distance between 2 points
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
        public static Double2 Lerp(Double2 a , Double2 b, double t)
        {
			// perform arithmetic sequence math between values of a and b
            var value = new Double2(Double.Lerp(a.x, b.x, t), Double.Lerp(a.y, b.y, t));
            return value;
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
            var value = a + ((b - a) * t); // arithmetic sequence, b - a is common difference, t is iterations, a is a0
            if (double.IsNaN(value))
                return a;
            else
                return value;
        }
    }
}
