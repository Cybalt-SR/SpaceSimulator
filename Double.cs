using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Creates a Double2, a datatype that stores horizontal and vertical information
        /// </summary>
        /// <param name="_x"> The x component of the Double2 </param>
        /// <param name="_y"> The y component of the Double2 </param>
        public Double2(double _x, double _y){
            x = _x;
            y = _y;
        }

        public static Double2 operator +(Double2 a, Double2 b) => new Double2(a.x + b.x, a.y + b.y);
        public static Double2 operator -(Double2 a, Double2 b) => new Double2(a.x - b.x, a.y - b.y);
        public static Double2 operator *(Double2 a, double b) => new Double2(a.x * b, a.y * b);
        public static Double2 operator /(Double2 a, double b) => new Double2(a.x / b, a.y / b);

        /// <summary>
        /// The sum of the squares of the components
        /// </summary>
        public double SquareMagnitude{
            get{
                return (x * x) + (y * y);
            }
        }
        
        /// <summary>
        /// Total displacement of the horizontal and vertical components
        /// The square root of the sum of the squares of the components
        /// </summary>
        public double Magnitude {
            get {
                return Math.Sqrt(SquareMagnitude);
            }
        }

		/// <summary>
        /// Divides the components of the Double2 by their magnitude
        /// </summary>
        public Double2 Normalized {
            get {
                return new Double2(x, y) / Magnitude;
            }
        }

        /// <summary>
        /// Applies linear interpolation to the components of two Double2 instances
        /// </summary>
        /// <param name="a"> First Double2 instance </param>
        /// <param name="b"> Second Double2 instance </param>
        /// <param name="t"> double between 0-1 that represents the distance between the components of a and b </param>
        /// <returns> A new double2 instance whose components are lerped from the inputs </returns>
        public static Double2 Lerp(Double2 a , Double2 b, double t){
			// perform arithmetic sequence math between values of a and b
            var value = new Double2(Double.Lerp(a.x, b.x, t), Double.Lerp(a.y, b.y, t));
            return value;
        }
<<<<<<< HEAD
        public static Double2 DirFromAngle(double angle)
        {
            var rad = (angle * Math.PI) / 180.0;
            return new Double2(Math.Cos(rad), Math.Sin(rad));
=======

        /// <summary>
        /// Converts an angle to a Double2 representing the x and y directions whose magnitude is 1
        /// </summary>
        /// <param name="angle"> The angle in degrees that the point should be at from the + x-axis </param>
        /// <returns> A Double2 that represents a point that is theta degrees away from the + x-axis </returns>
        public static Double2 DirFromAngle(double angle){
            return new Double2(Math.Cos(angle), Math.Sin(angle));
>>>>>>> restructure
        }

		/// <summary>
        /// Determine the value of angular / linear thrust at a given seocnd
        /// Smoothing / linear interpolation is applied between closest keys if exact time is not available
        /// </summary>
        /// <param name="list"> List of Double2, whose x property is the second and y property is the thrust value </param>
        /// <param name="time"> The index that will be used to determine the thrust from the list of keys </param>
        /// <returns> The thrust at a given second </returns>
        public static double LerpKeyList(List<Double2> list, double time){
            // init the function with pre and post keys as the first item
			Double2 preKey = list[0];
            Double2 postKey = preKey;

			// start looping through the list double2
            foreach (var item in list){
				if (item.x == time){
					// if the item's second is exactly what the user requested then return the thrust
					return item.y;
				}else if(item.x < time){
					// if the item is before the specified time then set it as preKey
					preKey = item;
				}else {
                    postKey = item; // postkey is now the first item whose second is after the requested
					break; // break out of the foreach after the item's second is past the requested
				}
            }
			
			// Example:
			// keys [5, 0.40], [15, 0.60], user wants thrust at 13 seconds
			// t = 13, preKey.x = 5,  maxT = 10; normalizedT = 0.8;
			// Double.Lerp(0.40, 0.60, 0.8); // returns 0.56

			// percentile of requested time between closest available keys
			double normalizedT = (time - preKey.x) / (postKey.x - preKey.x);
			return Double.Lerp(preKey.y, postKey.y, normalizedT);
        }

        /// <summary>
        /// Shows the x and y components in a better format for console debugging
        /// </summary>
        /// <returns> "[x, y]" </returns>
        public override string ToString(){
            return "[" + x + ", " + y + "]";
        }
    }

    public struct Double{
        /// <summary>
        /// Applies linear interpolation between two doubles
        /// </summary>
        /// <param name="a"> First double </param>
        /// <param name="b"> Second double </param>
        /// <param name="t"> double between 0-1 that represents the distance between a and b </param>
        /// <returns> double that is t% of the way between a and b </returns>
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
