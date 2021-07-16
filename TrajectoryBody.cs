using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
    [Serializable]
    public class ForceInfo
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
        public ForceInfo forceInfo = new ForceInfo();

        //Trajectory calculation
        //cache
        int caclulationSecond = 0;

        public void InitCalculation(Double2 position, Double2 up)
        {
            caclulationSecond = 0;

            trajectory = new List<Double2>();

            forceInfo.lastDir = up;
            forceInfo.lastPos = position;
            forceInfo.lastForce = up * (forceInfo.startingSpeed * forceInfo.mass);
        }

        public void CalculateNext(SimulatedBody[] otherObjects)
        {
            if (caclulationSecond % pathResolution == 0)
                trajectory.Add(forceInfo.lastPos);

            var newForce = Double2.zero;
            newForce += GetGravity(forceInfo.mass, forceInfo.lastPos, otherObjects);

            forceInfo.lastForce = newForce;
            forceInfo.currentVelocity = forceInfo.lastForce / forceInfo.mass;
            forceInfo.lastPos += forceInfo.currentVelocity / SpaceSimulation.scale;

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
                double rawForce = SpaceSimulation.gconst * (forceInfo.mass * mass / sqrdist);

                Double2 dir = bodyPos - pos;
                force += dir.nomalized * rawForce;
            }

            return force;
        }

        #endregion
    }
}
