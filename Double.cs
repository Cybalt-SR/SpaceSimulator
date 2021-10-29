using System;

namespace SpaceSimulation
{
    [Serializable]
    public struct Double2
    {
        public double x;
        public double y;

        public static Double2 Zero => new Double2(0, 0);
        public static Double2 One => new Double2(1, 1);
        public static Double2 Perpendicular => new Double2(-1, 1);

        public Double2(double _x, double _y){
            x = _x;
            y = _y;
        }

        public static Double2 operator +(Double2 a, Double2 b) => new Double2(a.x + b.x, a.y + b.y);
        public static Double2 operator -(Double2 a, Double2 b) => new Double2(a.x - b.x, a.y - b.y);
        public static Double2 operator *(Double2 a, double b) => new Double2(a.x * b, a.y * b);
        public static Double2 operator /(Double2 a, double b) => new Double2(a.x / b, a.y / b);

        public double SquareMagnitude{
            get{
                return (x * x) + (y * y);
            }
        }
        
        /// <summary>
        /// The distance of the point from the origin if plotted on a Cartesian plane
        /// </summary>
        public double Magnitude {
            get {
                return Math.Sqrt(SquareMagnitude);
            }
        }

        public Double2 Normalized {
            get {
                return new Double2(x, y) / Magnitude;
            }
        }

		// applies linear interpolation to the components two Double2 instances
        public static Double2 Lerp(Double2 a , Double2 b, double t){
			// perform arithmetic sequence math between values of a and b
            var value = new Double2(Double.Lerp(a.x, b.x, t), Double.Lerp(a.y, b.y, t));
            return value;
        }

		// creates a Double2 whose components are between 0 and 1
		// that has a specific angle from the positive x-axis 
		// when a line is drawn from the origin to its components
        public static Double2 DirFromAngle(double angle){
            return new Double2(Math.Cos(angle), Math.Sin(angle));
        }

		// shows the x and y components in the format of [x, y] fo console debugging
        public override string ToString(){
            return "[" + x + ", " + y + "]";
        }
    }

    public struct Double{
		// Applies linear interpolation to two values where t is the percentile between the two values
        public static double Lerp(double a, double b, double t){
			// this is just nice to know, but...
			// arithmetic sequence, b - a is common difference, t is iterations, a is a0
            var value = a + ((b - a) * t);
            if (double.IsNaN(value))
                return a;
            else
                return value;
        }
    }
}
