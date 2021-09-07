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
		public double rocketLength = 0; // length of the rocket in meters
		// current_t_data is an inherited value from TrajectoryBody

		// override the GetCurrentForces method from TrajectoryBody to add thrust in the forces
        protected override Double2 GetCurrentForces(CelestialBody[] otherObjects)
        {
            var newForces = base.GetCurrentForces(otherObjects); // gets gravity of trajectoryBody
            newForces += GetThrust(otherObjects); // add thrust (special to ThrustBodies)
            return newForces;
        }

<<<<<<< HEAD
        // turning energy should be a negative value if the rocket is turning right
        // in trigonometry clockwise is negative
        double GetRotationalVelocity(double turningEnergy, double timespan)
        {
            // formula for getting the inertia of an object whose pivot is in the center
            // I = 1/12 * M * L^2
            double inertia = (Math.Pow(rocketLength, 2) * current_t_data.mass) / 12;
            double torque = turningEnergy * rocketLength / 2; // should be newton-meters
            double finalMomentum = torque * timespan; // n * m * s

            // initial momentum should be subtracted from this but the rocket has no angular momentum when the turn is started
            double angularMomentum = finalMomentum / inertia; // this should be in radians / second
            double result = (angularMomentum / SpaceSimulation.PI) * 180; // convert it to radians / second
            return result;
        }
=======
			// initial momentum should be subtracted from this but the rocket has no angular momentum when the turn is started
			Double angularMomentum = finalMomentum / inertia; // this should be in radians / second
			Double result = (angularMomentum / SpaceSimulation.PI) * 180; // convert it to degrees / second
			return result;
		}
>>>>>>> 749af4f038edb3065a6e3b70f3deee43b5390883

        Double2 GetThrust(CelestialBody[] otherObjects)
        {
            CelestialBody nearest = null; // the nearest planet
            double nearestDist = 0; // distance of the craft to the nearest planet

			// loop through each planet and try to find the one that is nearest
			// which is probably where the spacecraft would be launching from
            foreach (var item in otherObjects)
            {	
				// distance of spacecraft to item
                var newDist = (item.GetPositionAtTime(calculationSecond) - current_t_data.Pos).magnitude;

                if (nearest == null)
                {
					// set first item as default
                    nearest = item;
                    nearestDist = newDist;
                }
                else if (newDist < nearestDist)
                {	
					// if newDist is smaller, it is closer, so it becomes new closest planet
                    nearest = item;
                    nearestDist = newDist;
                }
            }

			// get force of the nearest planet to the craft
			// might be unused since gravity is computed independently of raw thrust
            double grav = GetRawForce(current_t_data.Pos, nearest);

            // get total acceleration of craft.
<<<<<<< HEAD
            double accel = SpaceSimulation.exhaustVelo * (SpaceSimulation.fuelBurnRate);
=======
            double accel = (SpaceSimulation.exhaustVelo * SpaceSimulation.fuelBurnRate) - grav;
>>>>>>> 749af4f038edb3065a6e3b70f3deee43b5390883

            Double2 dir = current_t_data.Dir; // going against the earth
            return dir.normalized * accel;
        }

        double GetRawForce(Double2 pos, CelestialBody body)
        {
            // this method returns the raw force of the planet on the body
            // universal gravity equation = gconst * (m1 * m2 / sqrdist)

            double sqrdist = Math.Pow((pos - body.GetPositionAtTime(calculationSecond)).magnitude, 2);
            double rawForce = SpaceSimulation.gconst * (body.current_t_data.mass * current_t_data.mass / sqrdist);
            return rawForce;
        }
    }
}
