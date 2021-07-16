using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
    [Serializable]
    public class TrajectoryData
    {
        /// <summary>
        /// The only editable value, is the starting value.
        /// </summary>
        public double startingSpeed;
        public double mass;
        public Double2 currentVelocity;
        public Double2 lastDir;
        public Double2 lastPos;
        public Double2 lastForce;
    }

    public class TrajectoryBody : SimulatedBody
    {
        //used for the calculation of force and for variables here to retain throughout calculation
        public TrajectoryData t_data = new TrajectoryData();

        //Trajectory calculation
        //cache
        int caclulationSecond = 0;

        public void InitCalculation(Double2 position, Double2 up)
        {
            caclulationSecond = 0;

            trajectory = new List<Double2>();

            t_data.lastDir = up;
            t_data.lastPos = position;
            t_data.lastForce = up * (t_data.startingSpeed * t_data.mass);
        }

        public void CalculateNext(SimulatedBody[] otherObjects)
        {
            if (caclulationSecond % pathResolution == 0)
                trajectory.Add(t_data.lastPos);

            var newForce = Double2.zero;
            newForce += GetGravity(t_data.mass, t_data.lastPos, otherObjects);

            t_data.lastForce = newForce;
            t_data.currentVelocity = t_data.lastForce / t_data.mass;
            t_data.lastPos += t_data.currentVelocity / SpaceSimulation.scale;

            caclulationSecond++;
        }

        //forces
        #region Forces

        Double2 GetGravity(double mass, Double2 pos, SimulatedBody[] gravityHolders)
        {
            Double2 force = Double2.zero;

            foreach (var body in gravityHolders)
            {
                var bodyPos = body.GetUnscaledPositionAtTime(caclulationSecond);

                double sqrdist = Math.Pow(RealDist(pos, bodyPos), 2);
                //Gravity equation
                double rawForce = SpaceSimulation.gconst * (t_data.mass * mass / sqrdist);

                Double2 dir = bodyPos - pos;
                force += dir.normalized * rawForce;
            }

            return force;
        }

        Double2 GetThrust(double mass, Double2 pos, CelestialBody gravityHolder)
        {
            // get gravity of body, if earth then this should return 9.81m/s^2            
            double grav = GetRawForce(pos, gravityHolder);

            // get total acceleration of craft.
            double accel = (SpaceSimulation.exhaustVelo / mass) * SpaceSimulation.fuelBurnRate - grav;

            Double2 dir = (gravityHolder.unscaledPos - pos) * -1; // going against the earth
            return dir.normalized * accel;
        }

        double GetRawForce(Double2 pos, CelestialBody body)
        {
            // this method returns the raw force of the planet on the body
            // universal gravity equation = gconst * (m1 * m2 / sqrdist)

            double sqrdist = Math.Pow(RealDist(pos, body.unscaledPos), 2);
            double rawForce = SpaceSimulation.gconst * (body.mass * t_data.mass / sqrdist);
            return rawForce;
        }
        #endregion
    }
}
