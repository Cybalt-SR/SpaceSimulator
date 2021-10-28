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
        public List<Double2> thrustKeys;
        public List<Double2> angularthrustKeys;
		public double rocketLength = 0; // length of the rocket in meters
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
		}

        Double2 GetThrust(CelestialBody[] otherObjects, double percentage)
        {
            CelestialBody nearest = null; // the nearest planet
            double nearestDist = 0; // distance of the craft to the nearest planet

			// loop through each planet and try to find the one that is nearest
			// which is probably where the spacecraft would be launching from
            foreach (var item in otherObjects)
            {	
				// distance of spacecraft to item
                var newDist = (item.GetPositionAtTime(localSecond) - current_t_data.Pos).magnitude;

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
            double accel = SpaceSimulation.exhaustVelo * (SpaceSimulation.fuelBurnRate) * percentage;
            Double2 dir = Double2.DirFromAngle(current_t_data.Angle); //going towards ship direction
            return dir.normalized * accel;
        }

        double GetRawForce(Double2 pos, CelestialBody body)
        {
            // this method returns the raw force of the planet on the body
            // universal gravity equation = gconst * (m1 * m2 / sqrdist)

            double sqrdist = Math.Pow((pos - body.GetPositionAtTime(localSecond)).magnitude, 2);
            double rawForce = SpaceSimulation.gconst * (body.current_t_data.mass * current_t_data.mass / sqrdist);
            return rawForce;
        }
    }
}
