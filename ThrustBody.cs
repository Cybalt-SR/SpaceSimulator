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
        protected override Double2 GetCurrentForces(CelestialBody[] otherObjects)
        {
            var newForces = base.GetCurrentForces(otherObjects);
            newForces += GetThrust(otherObjects);

            return newForces;
        }

        Double2 GetThrust(CelestialBody[] otherObjects)
        {
            CelestialBody nearest = null;
            double nearestDist = 0;

            foreach (var item in otherObjects)
            {
                var newDist = (item.GetPositionAtTime(calculationSecond) - current_t_data.Pos).magnitude;

                if (nearest == null)
                {
                    nearest = item;
                    nearestDist = newDist;
                }
                else if (newDist < nearestDist)
                {
                    nearest = item;
                    nearestDist = newDist;
                }
            }

            // get gravity of body, if earth then this should return 9.81m/s^2            
            double grav = GetRawForce(current_t_data.Pos, nearest);

            // get total acceleration of craft.
            double accel = SpaceSimulation.exhaustVelo * (SpaceSimulation.fuelBurnRate);

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
