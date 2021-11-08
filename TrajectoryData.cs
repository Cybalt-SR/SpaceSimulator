using System;
#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
#endif

namespace SpaceSimulation
{
    [Serializable]
    public struct TrajectoryData
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        [SerializeField]
#endif
        private double mass;
        public double GetMass()
        {
            if (mass <= 0)
                throw new Exception("Zero mass exception.");

            return mass;
        }

        public Double2 Pos;
        public Double2 Velocity;
        public Double2 Force;

        public double Angle;
        public double AngularVelocity;
        public double Torque;

        /// <summary>
        /// Represents a snapshot of a body's trajectory, such as it's position, velocity
        /// </summary>
        /// <param name="Mass"> Mass of the object </param>
        /// <param name="position"> Position of the object </param>
        /// <param name="velocity"> Velocity of the object </param>
        /// <param name="angle"> Angle of the object </param>
        /// <param name="angularVelocity"> Angular velocity of the object </param>
        public TrajectoryData(double Mass, Double2 position, Double2 velocity, double angle, double angularVelocity)
        {
            mass = Mass;

            Pos = position;
            Velocity = velocity;
            Force = Double2.Zero;

            Angle = angle;
            AngularVelocity = angularVelocity;
            Torque = 0;
        }

#if !(UNITY_EDITOR || UNITY_STANDALONE)
		public void PrintToConsole(){
			Console.WriteLine("Velocity: " + Velocity);
            Console.WriteLine("Position: " + Pos);
			Console.WriteLine("Angle: " + Angle);
			Console.WriteLine("AngularVelocity: " + AngularVelocity);
			Console.WriteLine("");
		}
#endif

    }
}
