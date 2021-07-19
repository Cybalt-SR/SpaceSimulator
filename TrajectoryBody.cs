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
        public double GetInterpolatedT(int time, out int index, out int indexnext, bool clamped = false)
        {
            double accurateindex = ((double)time / pathResolution);
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
            if (localSecond % pathResolution == 0)
                trajectory.Add(current_t_data);

            var newForce = GetCurrentForces(otherObjects);

            current_t_data.Force = newForce;
            current_t_data.Velocity += newForce / current_t_data.mass;

            CelestialBody planetHit = null;
            Double2 intersection = Double2.zero;
            //check if newPosition is inside planet
            foreach (var planet in otherObjects)
            {
                var planetPos = planet.GetPositionAtTime(calculationSecond + globalSecond);
                var r = planet.radius;
                var possiblePos = current_t_data.Pos + current_t_data.Velocity;
                var dirToPlanet = planetPos - current_t_data.Pos;
                var dirToPos = current_t_data.Velocity;

                var possiblePosSqrDistToPlanet = (planetPos - possiblePos).sqrmagnitude;

                bool collision = false;
                if(possiblePosSqrDistToPlanet < r * r)
                {
                    collision = true;
                }
                else if(dirToPlanet.sqrmagnitude < dirToPos.sqrmagnitude)
                {
                    var a = dirToPos;
                    var b = dirToPlanet;
                    var angle = Math.Acos(((a.x * b.x) + (a.y * b.y)) / (Math.Sqrt((a.x * a.x) + (a.y * a.y)) * Math.Sqrt((b.x * b.x) + (b.y * b.y))));
                    var interceptDist = Math.Sin(angle) * dirToPlanet.magnitude;

                    if (interceptDist <= planet.radius)
                        collision = true;
                }

                if (collision == true)
                {
                    planetHit = planet;
                    intersection = planetPos;
                }
            }

            if (planetHit != null)
            {
                current_t_data.Velocity = planetHit.GetVelocityAtTime(calculationSecond + globalSecond);
                current_t_data.Pos = intersection;
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
