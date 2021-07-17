using System;
using System.Collections.Generic;
using static SpaceSimulation.SpaceSimulation;

namespace SpaceSimulation
{
    [Serializable]
    public struct TrajectoryData
    {
        public double mass;
        public Double2 Velocity;
        public Double2 Dir;
        public Double2 Pos;
        public Double2 Force;
    }

    public class TrajectoryBody
    {
        public List<TrajectoryData> trajectory;

        #region utils
        Double2 GetLerpedValue(List<Double2> values, int time, int timeResolution)
        {
            var normalValue = ((double)time / timeResolution) % values.Count;

            int valueBeforeIndex = (int)Math.Floor(normalValue);
            Double2 a = values[valueBeforeIndex];
            Double2 b = values[valueBeforeIndex + 1];

            double t = normalValue - valueBeforeIndex;

            return Double2.Lerp(a, b, t);
        }

        public Double2 GetPositionAtTime(int timeSecond)
        {
            timeSecond += globalSecond;

            int index = (timeSecond) % trajectory.Count;
            return trajectory[index].Pos;
        }
        public Double2 GetVelocityAtTime(int timeSecond)
        {
            timeSecond += globalSecond;

            int index = (timeSecond) % trajectory.Count;
            return trajectory[index].Velocity;
        }
        #endregion

        //used for the calculation of force and for variables here to retain throughout calculation
        public TrajectoryData current_t_data = new TrajectoryData();

        //Trajectory calculation
        //cache
        protected int calculationSecond = 0;

        public void InitCalculation(Double2 position, Double2 up, Double2 startingVelocity)
        {
            calculationSecond = 0;

            trajectory = new List<TrajectoryData>();

            current_t_data.Dir = up;
            current_t_data.Pos = position;
            current_t_data.Velocity = startingVelocity;
            current_t_data.Force = Double2.zero;
        }

        public void CalculateNext(CelestialBody[] otherObjects)
        {
            if (calculationSecond % pathResolution == 0)
                trajectory.Add(current_t_data);

            var newForce = GetCurrentForces(otherObjects);

            current_t_data.Force = newForce;
            current_t_data.Velocity += newForce / current_t_data.mass;

            CelestialBody planetHit = null;
            //check if newPosition is inside planet
            foreach (var planet in otherObjects)
            {
                var planetPos = planet.GetPositionAtTime(calculationSecond);

                var X = current_t_data.Pos.x - planetPos.x;
                var Y = current_t_data.Pos.x - planetPos.y;

                var m = current_t_data.Velocity.y / current_t_data.Velocity.x;
                var c = Y - (m * X);
                var r = planet.radius;

                var rSqr = Math.Pow(r, 2);
                var mSqr = Math.Pow(m, 2);
                var discriminant = rSqr + (mSqr * rSqr) - Math.Pow(c, 2);

                if (discriminant > 0)
                {
                    var dSqrt = Math.Sqrt(discriminant);

                    planetHit = planet;

                    var x1 = ((-m * c) + dSqrt) / (1 + mSqr);
                    var x2 = ((-m * c) - dSqrt) / (1 + mSqr);

                    var point1 = new Double2(x1, (m * x1) + c);
                    var point2 = new Double2(x2, (m * x2) + c);

                    var sqrdist1 = (point1 - current_t_data.Pos).sqrmagnitude;
                    var sqrdist2 = (point2 - current_t_data.Pos).sqrmagnitude;

                    if (sqrdist1 < sqrdist2)
                        current_t_data.Pos = point1;
                    else
                        current_t_data.Pos = point2;

                    break;
                }
            }

            if (planetHit != null)
                current_t_data.Velocity = planetHit.GetVelocityAtTime(calculationSecond);
            else
                current_t_data.Pos += current_t_data.Velocity;

            calculationSecond++;
        }

        //forces
        #region Forces

        protected virtual Double2 GetCurrentForces(CelestialBody[] otherObjects)
        {
            var newForce = Double2.zero;
            newForce += GetGravity(current_t_data.Pos, otherObjects);

            return newForce;
        }

        Double2 GetGravity(Double2 pos, CelestialBody[] gravityHolders)
        {
            Double2 force = Double2.zero;

            foreach (var body in gravityHolders)
            {
                var bodyPos = body.GetPositionAtTime(calculationSecond);

                double sqrdist = Math.Pow((pos - bodyPos).magnitude, 2);
                //Gravity equation
                double rawForce = gconst * (current_t_data.mass * body.current_t_data.mass / sqrdist);

                Double2 dir = bodyPos - pos;
                force += dir.normalized * rawForce;
            }

            return force;
        }
        #endregion
    }
}
