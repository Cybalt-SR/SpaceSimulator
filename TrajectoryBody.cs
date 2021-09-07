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
        public int trajectoryResolution = 3600;

        #region utils
        public double LerpKeyList(List<Double2> list, double t)
        {
            Double2 preKey = list[0];
            Double2 postKey = preKey;

            foreach (var item in list)
            {
                if (item.x < t)
                    preKey = item;
                else
                {
                    postKey = item;
                    break;
                }
            }

            double maxT = postKey.x - preKey.x;
            if (maxT > 0)
            {
                double normalizedT = (t - preKey.x) / maxT;
                return Double.Lerp(preKey.y, postKey.y, normalizedT);
            }
            else
            {
                return preKey.y;
            }
        }

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
        public Double2 GetVelocityAtTime(int timeSecond, bool clamped = false)
        {
            double t = GetInterpolatedT(timeSecond, out int index, out int nextIndex, clamped);
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
        public int startingSecond { get; protected set; }
        protected int calculationSecond => localSecond + startingSecond;

        public void InitCalculation(Double2 position, Double2 up, Double2 startingVelocity, int startingSecond)
        {	
			// constructs the class (js speak)
            localSecond = 0;
            this.startingSecond = startingSecond;

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

            var newForce = GetCurrentForces(otherObjects); // get total gravity exerted
            current_t_data.Force = newForce;
            current_t_data.Velocity += current_t_data.Force / current_t_data.mass;

            CelestialBody planetHit = null; // planet that body has intersected positions with
            Double2 intersection = Double2.zero;

            //check if newPosition is inside planet
            foreach (var planet in otherObjects)
            {
                var planetPos = planet.GetPositionAtTime(calculationSecond); // determine where planet would be at that time
                var r = planet.radius;
                var C = planetPos - current_t_data.Pos; // distance between planet and body
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
                current_t_data.Velocity = planetHit.GetVelocityAtTime(calculationSecond); // velocity of body matches velocity of planet that was hit
            }
            else
                current_t_data.Pos += current_t_data.Velocity; // no collision so continue moving

            localSecond++;
        }

        //forces
        #region Forces

		// shared between everything that inherits from TracjetoryBody
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
                // loop through gravityHolders then try to get summative total of their forces
				// determine position of body at given time
				var bodyPos = body.GetPositionAtTime(calculationSecond);

                double sqrdist = (pos - bodyPos).sqrmagnitude; // get distance of body to planet
				// how strong the attraction is between planet and body
                double rawForce = gconst * (current_t_data.mass * body.current_t_data.mass / sqrdist); //Gravity equation

				// get direction between body and planet through their positions
                Double2 dir = bodyPos - pos;
                var possibleNewForce = dir.normalized * rawForce; //combine data of direction and attraction force

                force += possibleNewForce; // add to summative force
            }

            return force;
        }
        #endregion
    }
}
