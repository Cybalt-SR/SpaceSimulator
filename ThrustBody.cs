using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
    [Serializable]
    public class ThrustBody : TrajectoryBody
    {
<<<<<<< HEAD
        public List<Double2> thrustKeys;
        public List<Double2> angularthrustKeys;
		public double rocketLength = 0; // length of the rocket in meters

        public ThrustBody(
            TrajectoryData startingData,
            List<Double2> thrustKeys,
            List<Double2> angularthrustKeys,
            double rocketLength
            ) : base(startingData)
        {
            this.thrustKeys = thrustKeys;
            this.angularthrustKeys = angularthrustKeys;
            this.rocketLength = rocketLength;
        }

        // current_t_data is an inherited value from TrajectoryBody

        // override the GetCurrentForces method from TrajectoryBody to add thrust in the forces
        protected override Double2 GetCurrentForces(CelestialBody[] otherObjects)
        {
            var newForces = base.GetCurrentForces(otherObjects); // gets gravity of trajectoryBody
            newForces += GetThrust(otherObjects, LerpKeyList(thrustKeys, localSecond)); // add thrust (special to ThrustBodies)

            return newForces;
        }

        protected override double GetCurrentTorques(CelestialBody[] otherObjects)
        {
            var newForces = base.GetCurrentTorques(otherObjects);
            newForces += GetTorque(LerpKeyList(angularthrustKeys, localSecond));

            return newForces;
        }

        protected override double GetLength()
        {
            return rocketLength;
        }

        // turning energy should be a negative value if the rocket is turning right
        // in trigonometry clockwise is negative
        double GetTorque(double percentage)
        {
            double torque = (SpaceSimulation.maxTorque * percentage) * rocketLength / 2; // should be newton-meters
            return torque;
=======
        public readonly List<Double2> thrustKeys;
		public readonly double rocketLength; // length of the rocket in meters
		public readonly double exhaustVelo; // Saturn V exhaust velocity 2.40 * 10^3 m/s (Make a method for this later?)
		public readonly double fuelBurnRate; // Saturn V Fuel burn rate is 1.40 * 10^4 kg/s (This should be a constant since it doesn't change for the entire flight duration)

        /// <summary>
        /// Represents a rocket
        /// </summary>
        /// <param name="tdata"> Starting trajectoryData of the rocket </param>
        /// <param name="ThrustKeys"> List that contain keys that represent the rocket's thrust levels over time </param>
        /// <param name="RocketLength"> Length of the rocket </param>
        /// <param name="ExhaustVelo"> Optional: The velocity of the rocket's exhaust gases </param>
        /// <param name="FuelBurnRate"> Optional: The rocket's fuel burn rate</param>
		public ThrustBody(TrajectoryData startingTrajectoryData, List<Double2> ThrustKeys, double RocketLength, double ExhaustVelo = 2400, double FuelBurnRate = 14000): base(startingTrajectoryData) {
            thrustKeys = ThrustKeys;
            rocketLength = RocketLength;
			exhaustVelo = ExhaustVelo;
			fuelBurnRate = FuelBurnRate;
        }

        // current_t_data is an inherited value from TrajectoryBody
		// override the GetCurrentForces method from TrajectoryBody to add thrust in the forces
        protected override Double2 GetCurrentForces(CelestialBody[] otherObjects)
        {
            var newForces = base.GetCurrentForces(otherObjects); // gets gravity of trajectoryBody
            newForces += GetThrust(otherObjects, Double2.LerpKeyList(thrustKeys, localSecond)); // add thrust (special to ThrustBodies)
            return newForces;
        }

        /// <summary>
        /// Compute the rotational velocity of a rocket initiating a turn
        /// </summary>
        /// <param name="turningEnergy"> The amonut of force exerted on the side of a rocket. Negative values represent a right turn while positive values represent a left turn. </param>
        /// <param name="timespan"> The amount of time that a turn takes place </param>
        /// <returns></returns>
        double GetRotationalVelocity(double turningEnergy, double timespan)
        {
            // formula for getting the inertia of an object whose pivot is in the center
            // I = 1/12 * M * L^2
            double inertia = (Math.Pow(rocketLength, 2) * current_t_data.mass) / 12;
            double torque = turningEnergy * rocketLength / 2; // should be newton-meters
            double finalMomentum = torque * timespan; // n * m * s

            // initial momentum should be subtracted from this but the rocket has no angular momentum when the turn is started
            double angularMomentum = finalMomentum / inertia; // this should be in radians / second
            double result = (angularMomentum / Math.PI) * 180; // convert it to degrees / second
            return result;
>>>>>>> restructure
		}

        /// <summary>
        /// Compute for the thrust of the rocket
        /// </summary>
        /// <param name="otherObjects">Array of planets, used to determine where the rocket launching from</param>
        /// <param name="percentage">The thrust level of the rocket</param>
        /// <returns> The change in position of the rocket with its angle into account</returns>
        Double2 GetThrust(CelestialBody[] otherObjects, double percentage){
            CelestialBody nearest = null; // the nearest planet
            double nearestDist = 0; // distance of the craft to the nearest planet

	  		// loop through each planet and try to find the one that is nearest
	    	// which is probably where the spacecraft would be launching from
            foreach (var item in otherObjects){	
				// distance of spacecraft to item
                var newDist = (item.GetPositionAtTime(localSecond) - current_t_data.Pos).Magnitude;

                if (nearest == null){
		   			// set first item as default
                    nearest = item;
                    nearestDist = newDist;
                }else if (newDist < nearestDist){	
		    		// if newDist is smaller, it is closer, so it becomes new closest planet
                    nearest = item;
                    nearestDist = newDist;
                }
            }

            // get total acceleration of craft.
<<<<<<< HEAD
            double accel = SpaceSimulation.exhaustVelo * (SpaceSimulation.fuelBurnRate) * percentage;
            Double2 dir = Double2.DirFromAngle(current_t_data.Angle); //going towards ship direction
            return dir.Normalized * accel;
        }

        double GetRawForce(Double2 pos, CelestialBody body)
        {
            // this method returns the raw force of the planet on the body
            // universal gravity equation = gconst * (m1 * m2 / sqrdist)

            double sqrdist = Math.Pow((pos - body.GetPositionAtTime(localSecond)).Magnitude, 2);
            double rawForce = SpaceSimulation.gconst * (body.current_t_data.mass * current_t_data.mass / sqrdist);
            return rawForce;
=======
            double accel = exhaustVelo * fuelBurnRate * percentage;
            Double2 dir = Double2.DirFromAngle(current_t_data.Angle); // take into consideration the current angle of the rocket
            return dir * accel;
>>>>>>> restructure
        }
    }
}
