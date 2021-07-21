using System;
using UnityEngine;
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
        public int trajectoryResolution = 3600;

        #region utils
        public double GetInterpolatedT(int time, out int index, out int indexnext, bool clamped = false)
        {
            double accurateindex = ((double)time / trajectoryResolution);
            bool withinClamp = accurateindex < trajectory.Count - 1 && accurateindex > 0;

            if (clamped == false || withinClamp == true)
            {
                accurateindex %= trajectory.Count;
                index = (int)Math.Floor(accurateindex);
                indexnext = (index + 1) % trajectory.Count;

                double t = accurateindex - index;
                return t;
            }
            else
            {
                if(accurateindex > trajectory.Count - 1)
                {
                    index = trajectory.Count - 1;
                    indexnext = trajectory.Count - 1;
                    return 1;
                }
                else
                {
                    index = 0;
                    indexnext = 0;
                    return 0;
                }
            }
        }

        public Double2 GetPositionAtTime(int timeSecond, bool clamped = false)
        {
            double t = GetInterpolatedT(timeSecond, out int index, out int nextIndex, clamped);
            Double2 a = trajectory[index].Pos;
            Double2 b = trajectory[nextIndex].Pos;

            return Double2.Lerp(a, b, t);
        }
        public Double2 GetVelocityAtTime(int timeSecond, bool clamed = false)
        {
            double t = GetInterpolatedT(timeSecond, out int index, out int nextIndex, clamed);
            Double2 a = trajectory[index].Velocity;
            Double2 b = trajectory[nextIndex].Velocity;

            return Double2.Lerp(a, b, t);
        }
        #endregion

        //used for the calculation of force and for variables here to retain throughout calculation
        public TrajectoryData current_t_data = new TrajectoryData();

        //Trajectory calculation
        //cache
        protected int localSecond = 0;
        protected int calculationSecond => localSecond + globalSecond;

        public void InitCalculation(Double2 position, Double2 up, Double2 startingVelocity)
        {
            localSecond = 0;

            trajectory = new List<TrajectoryData>();

            current_t_data.Dir = up;
            current_t_data.Pos = position;
            current_t_data.Velocity = startingVelocity;
            current_t_data.Force = Double2.zero;
        }

        public void CalculateNext(CelestialBody[] otherObjects)
        {
            if (localSecond % trajectoryResolution == 0)
                trajectory.Add(current_t_data);

            var newForce = GetCurrentForces(otherObjects);
            current_t_data.Force = newForce;
            current_t_data.Velocity += current_t_data.Force / current_t_data.mass;

            CelestialBody planetHit = null;
            Double2 intersection = Double2.zero;
            //check if newPosition is inside planet
            foreach (var planet in otherObjects)
            {
                var planetPos = planet.GetPositionAtTime(calculationSecond + globalSecond);
                var r = planet.radius;
                var C = planetPos - current_t_data.Pos;
                var V = current_t_data.Velocity;

                var a = 1 + ((V.y * V.y) / (V.x * V.x));
                var b = (-2.0 * (V.y / V.x) * C.y) - (2.0 * C.x);
                var c = (C.x * C.x) + (C.y * C.y) - (r * r);

                var discriminant = (b * b) - (4 * a * c);

                if (discriminant >= 0)
                {
                    double xsign = Math.Sign(V.x);
                    var X = (-b - (xsign * Math.Sqrt(discriminant))) / (a * 2.0);

                    if (Math.Abs(X) < Math.Abs(V.x))
                    {
                        planetHit = planet;
                        intersection = new Double2(X, X * (V.y / V.x));
                        break;
                    }
                }
            }

            if (planetHit != null)
            {
                current_t_data.Pos += intersection;
                current_t_data.Velocity = planetHit.GetVelocityAtTime(calculationSecond + globalSecond);
            }
            else
                current_t_data.Pos += current_t_data.Velocity;

            localSecond++;
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
                var bodyPos = body.GetPositionAtTime(calculationSecond + globalSecond);

                double sqrdist = (pos - bodyPos).sqrmagnitude;
                //Gravity equation
                double rawForce = gconst * (current_t_data.mass * body.current_t_data.mass / sqrdist);

                Double2 dir = bodyPos - pos;
                var possibleNewForce = dir.normalized * rawForce;

                force += possibleNewForce;
            }

            return force;
        }
        #endregion
    }
}
