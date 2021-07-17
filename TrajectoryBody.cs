using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceSimulation.SpaceSimulation;

namespace SpaceSimulation
{
    [Serializable]
    public class TrajectoryData
    {
        public double mass;
        public Double2 currentVelocity;
        public Double2 lastDir;
        public Double2 lastPos;
        public Double2 lastForce;
    }

    public class TrajectoryBody : SimulatedBody
    {
        public List<Double2> trajectory;

        #region utils
        public Double2 GetPositionAtTime(int timeSecond)
        {
            int index = (timeSecond / pathResolution) % trajectory.Count;
            return trajectory[index];
        }
        #endregion

        //used for the calculation of force and for variables here to retain throughout calculation
        public TrajectoryData t_data = new TrajectoryData();

        //Trajectory calculation
        //cache
        int calculationSecond = 0;

        public void InitCalculation(Double2 position, Double2 up, double startRealSpeed)
        {
            calculationSecond = 0;

            trajectory = new List<Double2>();

            t_data.lastDir = up;
            t_data.lastPos = position;
            t_data.currentVelocity = up * startRealSpeed;
            t_data.lastForce = Double2.zero;
        }

        public void CalculateNext(CelestialBody[] otherObjects)
        {
            if (calculationSecond % pathResolution == 0)
                trajectory.Add(t_data.lastPos);

            var newForce = Double2.zero;
            newForce += GetGravity(t_data.lastPos, otherObjects);

            t_data.lastForce = newForce;
            t_data.currentVelocity += newForce / t_data.mass;
            t_data.lastPos += t_data.currentVelocity;

            calculationSecond++;
        }

        //forces
        #region Forces

        Double2 GetGravity(Double2 pos, CelestialBody[] gravityHolders)
        {
            Double2 force = Double2.zero;

            foreach (var body in gravityHolders)
            {
                var bodyPos = body.GetPositionAtTime(calculationSecond);

                double sqrdist = Math.Pow((pos - bodyPos).magnitude, 2);
                //Gravity equation
                double rawForce = gconst * (t_data.mass * body.t_data.mass / sqrdist);

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

            Double2 dir = (gravityHolder.GetPositionAtTime(calculationSecond) - pos) * -1; // going against the earth
            return dir.normalized * accel;
        }

        double GetRawForce(Double2 pos, CelestialBody body)
        {
            // this method returns the raw force of the planet on the body
            // universal gravity equation = gconst * (m1 * m2 / sqrdist)

            double sqrdist = Math.Pow((pos - body.GetPositionAtTime(calculationSecond)).magnitude, 2);
            double rawForce = SpaceSimulation.gconst * (body.t_data.mass * t_data.mass / sqrdist);
            return rawForce;
        }
        #endregion
    }
}
