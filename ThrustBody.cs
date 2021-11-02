using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace SpaceSimulation
{
    [Serializable]
    public class ThrustBody : TrajectoryBody
    {
#if UNITY_EDITOR
        [SerializeField]
#endif
        private List<Double2> angularthrustKeys;
        public List<Double2> GetAngularthrustKeys() => angularthrustKeys;

#if UNITY_EDITOR
        [SerializeField]
#endif
        private List<Double2> thrustKeys;
        public List<Double2> GetThrustKeys() => thrustKeys;

        public readonly double rocketLength; // length of the rocket in meters
        public readonly double exhaustVelo; // Saturn V exhaust velocity 2.40 * 10^3 m/s (Make a method for this later?)
        public readonly double fuelBurnRate; // Saturn V Fuel burn rate is 1.40 * 10^4 kg/s (This should be a constant since it doesn't change for the entire flight duration)

        /// <summary>
        /// Represents a rocket
        /// </summary>
        /// <param name="tdata"> Starting trajectoryData of the rocket </param>
        /// <param name="ThrustKeys"> List that contain keys that represent the rocket's thrust levels over time </param>
		/// <param name="AngleKeys"> List that contain keys that represent the rocket's angular thrust levels over time </param>
        /// <param name="RocketLength"> Length of the rocket </param>
        /// <param name="ExhaustVelo"> Optional: The velocity of the rocket's exhaust gases </param>
        /// <param name="FuelBurnRate"> Optional: The rocket's fuel burn rate</param>
		/// <param name="Trajectory"> Optional: A list of the rocket's trajectoryData </param>
		public ThrustBody(
            TrajectoryData startingTrajectoryData, 
            List<Double2> ThrustKeys,
            List<Double2> AngleKeys,
            double RocketLength, 
            double ExhaustVelo = 2400, 
            double FuelBurnRate = 14000,
            List<TrajectoryData> Trajectory = null
            ) : base(startingTrajectoryData, Trajectory)
        {
            thrustKeys = ThrustKeys;
            angularthrustKeys = AngleKeys;
            rocketLength = RocketLength;
            exhaustVelo = ExhaustVelo;
            fuelBurnRate = FuelBurnRate;
        }

        // current_t_data is an inherited value from TrajectoryBody
        // override the GetCurrentForces method from TrajectoryBody to add thrust in the forces
        protected override Double2 GetCurrentForces(CelestialBody[] otherObjects)
        {
            var newForces = base.GetCurrentForces(otherObjects); // gets gravity of trajectoryBody
            newForces += GetThrust(Double2.LerpKeyList(thrustKeys, localSecond)); // add thrust (special to ThrustBodies)
            return newForces;
        }

        protected override double GetCurrentTorques(CelestialBody[] otherObjects)
        {
            var newForces = base.GetCurrentTorques(otherObjects);
            newForces += GetTorque(Double2.LerpKeyList(angularthrustKeys, localSecond));

            if (double.IsNaN(newForces))
                throw new Exception("NaN torque");

            return newForces;
        }

        protected override double GetLength()
        {
            if (rocketLength == 0)
                throw new Exception("Zero length exception for " + name);

            return rocketLength;
        }

        // turning energy should be a negative value if the rocket is turning right
        // in trigonometry clockwise is negative
        double GetTorque(double percentage)
        {
            double torque = (SpaceSimulation.maxTorque * percentage) * rocketLength / 2; // should be newton-meters
            return torque;
        }

        /// <summary>
        /// Compute for the thrust of the rocket
        /// </summary>
        /// <param name="percentage">The thrust level of the rocket</param>
        /// <returns> The change in position of the rocket with its angle into account</returns>
        Double2 GetThrust(double percentage)
        {
            double accel = exhaustVelo * fuelBurnRate * percentage;
            Double2 dir = Double2.DirFromAngle(current_t_data.Angle); //going towards ship direction
            return dir * accel;
        }
    }
}
