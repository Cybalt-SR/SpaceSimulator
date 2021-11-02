using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace SpaceSimulation
{
    [Serializable]

    public struct TrajectoryData
    {

#if UNITY_EDITOR
        [SerializeField]
#endif
        private double mass;
        public double GetMass() => mass;

        public Double2 Pos;
        public Double2 Velocity;
        public Double2 Force;

        public double Angle;
        public double AngularVelocity;
        public double Torque;

        /// <summary>
        /// Represents a snapshot of a body's trajectory, such as it's position, velocity
        /// </summary>
        /// <param name="position"> Position of the object </param>
        /// <param name="velocity"> Velocity of the object </param>
        /// <param name="angle"> Angle of the object </param>
        /// <param name="angularVelocity"> Angular velocity of the object </param>
        /// <param name="Mass"> Mass of the object </param>
        public TrajectoryData(double Mass, Double2 position, Double2 velocity, double angle, double angularVelocity)
        {

            mass = Mass;

            Pos = position;
            Velocity = velocity;
            Force = Double2.Zero;

            Angle = angle;
            AngularVelocity = angularVelocity;
            Torque = 0;
        }
    }

    public abstract class TrajectoryBody
    {
        public string name;
        public double percentOffset;

#if UNITY_EDITOR
        [SerializeField]
#endif
        private int SnapshotInterval = 1; // set this to 1 if in CMD, 3600 in unity
        public int GetSnapshotInterval() => SnapshotInterval;

#if UNITY_EDITOR
        [SerializeField]
#endif
        private List<TrajectoryData> TrajectoryList;
        public List<TrajectoryData> GetTrajectoryList() => TrajectoryList;

        // prevent current_t_data from being rewritten outside of TrajectoryBodies / classes that derive from it
        protected TrajectoryData current_t_data = new TrajectoryData();
        public TrajectoryData CurrentTrajectoryData
        {
            get => current_t_data;
        }

        /// <summary>
        /// Makes a new trajectory body and initiates it
        /// </summary>
        /// <param name="startingTrajectoryData"> The initial trajectory data of the trajectoryBody </param>
        public TrajectoryBody(TrajectoryData startingTrajectoryData, List<TrajectoryData> Trajectory = null)
        {
            if (Trajectory != null)
                TrajectoryList = Trajectory;
            else
                InitCalculation(startingTrajectoryData);
        }

        //this will still exist for reusing of class
        /// <summary>
        /// Initiates a trajectoryBody
        /// </summary>
        /// <param name="staringTrajectoryData"> The initial trajectory data of the trajectoryBody </param>
        public void InitCalculation(TrajectoryData staringTrajectoryData)
        {
            localSecond = 0;

            current_t_data = staringTrajectoryData;
            TrajectoryList = new List<TrajectoryData>()
            {
                staringTrajectoryData
            };
        }

        #region utils

        /// <summary>
        /// Determines the lerp percentage and trajectory indexes for a given second in the simulation
        /// </summary>
        /// <param name="time">Simulation time in seconds that will be used to calculate indexes</param>
        /// <param name="data">Output: TrajectoryList element before the specified time</param>
        /// <param name="nextdata">Output: TrajectoryList element after the specified time</param>
        /// <param name="clamped">Optional: if true, allows method to sometimes return first and last index</param>
        /// <returns>Lerp percentage</returns>
        public double GetInterpolatedT(int time, out TrajectoryData data, out TrajectoryData nextdata, bool clamped = false)
        {

            if (GetTrajectoryList().Count > 0)
            {
                //indexes to find the trajectory data in the TrajectoryList
                int index;
                int nextIndex;

                //local function to easily assign TrajectoryData from the given indexes
                void AssignOutputDatas(out TrajectoryData _data, out TrajectoryData _nextdata)
                {
                    try
                    {
                        _data = GetTrajectoryList()[index];
                        _nextdata = GetTrajectoryList()[nextIndex];
                    }
                    catch
                    {
                        throw new Exception("invalid indexes " + index + " | " + nextIndex + " for " + name + " (ListCount: " + TrajectoryList.Count + ")");
                    }
                }

                // takes into account SnapshotInterval, 
                double trueIndex = ((double)time / SnapshotInterval);

                trueIndex += (GetTrajectoryList().Count * percentOffset);
                // is true if trueIndex is between the start and last indices of trajectory
                bool withinClamp = trueIndex < GetTrajectoryList().Count - 1 && trueIndex > 0;

                if (clamped == false || withinClamp == true)
                {
                    // this block always gets executed if the user didn't provide clamped parameter / it's false 
                    // or if it's true but trueIndex was within the start and last indices of trajectory

                    trueIndex %= GetTrajectoryList().Count; // gets the modulo of trueIndex
                    index = (int)Math.Floor(trueIndex); // round and cast to int
                    nextIndex = (index + 1) % GetTrajectoryList().Count; // if indexNext is the length of trajectoryArray then loop around to first

                    double t = trueIndex - index; // percentage / lerp value

                    AssignOutputDatas(out data, out nextdata);
                    return t;
                }
                else
                {
                    // this block is executed if clamped == true and trueIndex is 0 / greater than or equal to the last indice

                    if (trueIndex > GetTrajectoryList().Count - 1)
                    {
                        // executed if trueIndex is greater than the last index of trajectoryCount
                        index = GetTrajectoryList().Count - 1; // simply returns the last index
                        nextIndex = GetTrajectoryList().Count - 1;

                        AssignOutputDatas(out data, out nextdata);
                        return 1; // the y value of the last index
                    }
                    else
                    {
                        // executed if trueIndex is 0 of if it is or equal to the last index;
                        index = 0;
                        nextIndex = 0;

                        AssignOutputDatas(out data, out nextdata);
                        return 0; // the y value of the first index
                    }
                }
            }
            else
            {
                throw new Exception("Unitialized Trajectory");
            }
        }

        /// <summary>
        /// Get the position of the rocket at a given second.
        /// Smoothing / linear interpolation is applied if the given second is between the available TrajectoryData
        /// </summary>
        /// <param name="timeSecond">Simulation time in seconds</param>
        /// <returns>The position of the rocket at a given second.</returns>
        public Double2 GetPositionAtTime(int timeSecond, bool clamped = false)
        {
            double t = GetInterpolatedT(timeSecond, out TrajectoryData data, out TrajectoryData nextdata, clamped);

            return Double2.Lerp(data.Pos, nextdata.Pos, t);
        }

        /// <summary>
        /// Get the velocity of the rocket at a given second.
        /// Smoothing / linear interpolation is applied if the given second is between the available TrajectoryData
        /// </summary>
        /// <param name="timeSecond">Simulation time in seconds</param>
        /// <returns>The velocity of the rocket at a given second.</returns>
        public Double2 GetVelocityAtTime(int timeSecond, bool clamped = false)
        {
            double t = GetInterpolatedT(timeSecond, out TrajectoryData data, out TrajectoryData nextdata, clamped);

            return Double2.Lerp(data.Velocity, nextdata.Velocity, t);
        }

        public double GetAngleAtTime(int timeSecond, bool clamped = false)
        {
            double t = GetInterpolatedT(timeSecond, out TrajectoryData data, out TrajectoryData nextdata, clamped);

            return Double.Lerp(data.Angle, nextdata.Angle, t);
        }
        public double GetAnglularVelocityAtTime(int timeSecond, bool clamped = false)
        {
            double t = GetInterpolatedT(timeSecond, out TrajectoryData data, out TrajectoryData nextdata, clamped);

            return Double.Lerp(data.AngularVelocity, nextdata.AngularVelocity, t);
        }
        #endregion

        //Trajectory calculation
        protected int localSecond = 0;

        /// <summary>
        /// Calculates the next TrajectoryData and appends it to TrajectoryList array
        /// It also performs collision detection to see if the rocket has collided with any planets
        /// </summary>
        /// <param name="otherObjects">Array of celestial objects that will be taken into account</param>
        public void CalculateNext(CelestialBody[] otherObjects)
        {
            if (localSecond % SnapshotInterval == 0) GetTrajectoryList().Add(current_t_data);

            if(current_t_data.GetMass() == 0)
                throw new Exception("Zero mass exception for " + name);

            //positional physics
            var newForce = GetCurrentForces(otherObjects); // get total forces exerted on the object
            current_t_data.Force = newForce;
            current_t_data.Velocity += current_t_data.Force / current_t_data.GetMass();

            //rotational physics
            current_t_data.Torque = GetCurrentTorques(otherObjects);
            current_t_data.AngularVelocity += ConvertTorqueToDegPerSec(current_t_data.Torque, GetLength());

            CelestialBody planetHit = null; // planet that body has intersected positions with
            Double2 intersection = Double2.Zero; // coordinates relative to the current position to the rocket where an intersection takes place

            //check if newPosition is inside planet
            foreach (var planet in otherObjects)
            {
                var absolutePlanetPos = planet.GetPositionAtTime(localSecond); // absolute position of the planet
                var relativePlanetPos = absolutePlanetPos - current_t_data.Pos; // position of planet relative to rocket
                var rocketVelocity = current_t_data.Velocity; // velocity of the rocket

                // How this works is you have a system of equations of a line and a circle
                // if you solve them together you have a quadratic equation, 
                // and you can determine through the discriminant if there is an intersection

                // (x - d)^2 + (y - e)^2 = r^2    circle
                // d, e are the coordinates of the center of the planet

                // (x - h)^2 + (y - k)^2 = r^2       circle
                // (h, k) are the coordinates of the center of the planet
                // r is the radius of the planet

                // slope intercept of a line 
                // as the planet's position was computed to be relative to the rocket, the y-intercept can be ignored
                // y = mx      m is the slope of the line, which can be determined through the velocity of the rocket

                // (x - h)^2 + (mx - k)^2 = r^2                      			substitution
                // (x^2 - 2hx + h^2) + (m^2 * x^2 - 2mkx + k^2) = r^2    		expansion
                // (m^2 * x^2 + x^2) + (-2hx - 2mkx) + (h^2 + k^2 - r^2) = 0  	let's go ahead and clean that up
                // (m^2 + 1) * x^2 + (-2h - 2mk)x + (h^2 + k^2 - r^2) = 0		now it's clear that it's a quadratic equation

                // a = m^2 + 1 			   	m is the slope of the line
                // b = -2h -2mk   			h is x coordinate of circle, k is y coordinate of circle
                // c = h^2 + k^2 - r^2		r is the radius

                // this is the slope of the line because the velocity represents the change in position over time
                var slope = rocketVelocity.y / rocketVelocity.x;

                var a = Math.Pow(slope, 2) + 1;
                var b = (-2.0 * relativePlanetPos.x) + (-2.0 * slope * relativePlanetPos.y);
                var c = Math.Pow(relativePlanetPos.x, 2) + Math.Pow(relativePlanetPos.y, 2) - Math.Pow(planet.Radius, 2);

                var discriminant = (b * b) - (4 * a * c);

                // discriminant = 0 then there is 1 collision
                // discriminant > 0 then there is 2 collisions
                if (discriminant >= 0)
                {
                    // x sign is used for the directionality of travel of the ship along the slope
                    // it allows the code to get the closer intersection
                    double xsign = Math.Sign(rocketVelocity.x); // gets the sign of a number, either 1 or -1
                    var root = (-b - (xsign * Math.Sqrt(discriminant))) / (a * 2.0); // relative x coordinate of where collision takes place

                    // If the root is between the initial position and the final position, then there has been a collision
                    // current position does not need to be checked since this is relative to the origin, 0 < root < finalPosition
                    if (0 < xsign * root && xsign * root < Math.Abs(rocketVelocity.x))
                    {
                        planetHit = planet; // set planetHit to the current planet
                        intersection = new Double2(root, root * slope); // relative coordinates where the collision takes place
                        break; // exit from running the for loop
                    }
                }
            }

            if (planetHit != null)
            {
                // position of body now matches with the planet (intersection is relative from rocket)
                current_t_data.Pos += intersection;
                // velocity of body matches velocity of planet that was hit due to sticky collision
                current_t_data.Velocity = planetHit.GetVelocityAtTime(localSecond);

                if (planetHit != null)
                {
                    // position of body now matches with the planet (intersection is relative from rocket)
                    current_t_data.Pos += intersection;
                    // velocity of body matches velocity of planet that was hit due to sticky collision
                    current_t_data.Velocity = planetHit.GetVelocityAtTime(localSecond);
                }
                else current_t_data.Pos += current_t_data.Velocity; // no collision so continue moving

                // only add current_t_data on the interval specified by the user
                if (localSecond % SnapshotInterval == 0) GetTrajectoryList().Add(current_t_data);
                localSecond++;
            }
            else current_t_data.Pos += current_t_data.Velocity; // no collision so continue moving

            current_t_data.Angle += current_t_data.AngularVelocity; // apply rotation
            localSecond++;
        }

        //forces
        #region Forces

        /// <summary>
        /// Calculates the forces being applied on the TrajectoryBody with planets being taken into account
        /// </summary>
        /// <param name="otherObjects"> Array of celestial objects that will be taken into account </param>
        /// <returns>The forces being applied on the rocket on the horizontal and vertical axis</returns>
        protected virtual Double2 GetCurrentForces(CelestialBody[] otherObjects)
        {
            var newForce = Double2.Zero;
            newForce += GetGravity(otherObjects); // everything is affected by gravity

            return newForce;
        }

        /// <summary>
        /// Determines the forces of gravity acting on the trajectory body on the horizontal and vertical axis
        /// </summary>
        /// <param name="gravityHolders"> Array of celestial objects that will be taken into account </param>
        /// <returns>The forces of gravity being applied on the rocket on the hroziontal and vertical axi</returns>
        public Double2 GetGravity(CelestialBody[] gravityHolders)
        {
            Double2 force = Double2.Zero;

            foreach (var body in gravityHolders)
            {
                // loop through gravityHolders then try to get summative total of their forces
                // determine position of body at given time
                var bodyPos = body.GetPositionAtTime(localSecond);
                double rawForce = GetRawForceOfGravity(body, bodyPos);  // how strong the attraction is between planet and body 

                // get direction between body and planet through their positions
                Double2 dir = bodyPos - current_t_data.Pos;
                var possibleNewForce = dir.Normalized * rawForce; // combine data of direction and attraction force

                force += possibleNewForce; // add to summative force
            }

            return force;
        }

        /// <summary>
        /// Calculates the raw force of gravity between two bodies
        /// Does not take into account the direction of force between the bodies
        /// </summary>
        /// <param name="body">A planet</param>
        /// <param name="bodyPos">The position of the rocket</param>
        /// <returns></returns>
        public double GetRawForceOfGravity(CelestialBody body, Double2 bodyPos)
        {
            // this method returns the raw force of the planet on the body
            // universal gravity equation = gconst * (m1 * m2 / sqrdist
            double sqrdist = (current_t_data.Pos - bodyPos).SquareMagnitude;
            double rawForce = SpaceSimulation.gconst * (body.current_t_data.GetMass() * current_t_data.GetMass() / sqrdist);
            return rawForce;
        }
        #endregion

        //forces
        #region Torques
        protected abstract double GetLength();

        // shared between everything that inherits from TracjetoryBody
        protected virtual double GetCurrentTorques(CelestialBody[] otherObjects)
        {
            return 0;
        }
        public double ConvertTorqueToDegPerSec(double torque, double length)
        {
            // formula for getting the inertia of an object whose pivot is in the center
            // I = 1/12 * M * L^2
            double inertia = (Math.Pow(length, 2) * current_t_data.GetMass()) / 12;
            double angularMomentum = torque / inertia; // this should be in radians / second
            double result = (angularMomentum / Math.PI) * 180; // convert it to degrees / second

            return result;
        }
        #endregion
    }
}
