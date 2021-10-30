using System;
using System.Collections.Generic;
using static SpaceSimulation.SpaceSimulation;

namespace SpaceSimulation
{
    [Serializable]

    public struct TrajectoryData {
        public Double2 Force;

        public readonly double mass;
        public Double2 Pos;
        public Double2 Velocity;

        public double Angle;
        public double AngularVelocity;

        /// <summary>
        /// Represents a snapshot of a body's trajectory, such as it's position, velocity
        /// </summary>
        /// <param name="position"> Position of the object </param>
        /// <param name="velocity"> Velocity of the object </param>
        /// <param name="angle"> Angle of the object </param>
        /// <param name="angularVelocity"> Angular velocity of the object </param>
        /// <param name="Mass"> Mass of the object </param>
        public TrajectoryData(Double2 position, Double2 velocity, double angle, double angularVelocity, double Mass) {
            Force = Double2.Zero;

            mass = Mass;
            Pos = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
        }
    }

    public class TrajectoryBody {
        protected int localSecond = 0;
        public readonly int trajectoryResolution; // set this to 1 if in CMD, 3600 in unity
        public readonly List<TrajectoryData> TrajectoryList = new List<TrajectoryData>();
           
        // prevent current_t_data from being rewritten outside of TrajectoryBodies / classes that derive from it
        protected TrajectoryData current_t_data;
        public TrajectoryData CurrentTrajectoryData {
            get => current_t_data;
        }

		// InitCalculation was replaced with a class constructor that takes in the starting trajectory data
        /// <summary>
        /// Initiates a trajectoryBody
        /// </summary>
        /// <param name="staringTrajectoryData">The initial trajectory data of the rocket </param>
        /// <param name="TrajectoryResolution">Optional: the interval of the trajectoryData snapshots </param>
        public TrajectoryBody(TrajectoryData staringTrajectoryData, int TrajectoryResolution = 1) {
            trajectoryResolution = TrajectoryResolution;
            current_t_data = staringTrajectoryData;
            TrajectoryList.Add(staringTrajectoryData);
        }

        #region utils

        /// <summary>
        /// Determine the value of angular / linear thrust at a given seocnd
        /// Smoothing / linear interpolation is applied between closest keys if exact time is not available
        /// </summary>
        /// <param name="list"> List of Double2, whose x property is the second and y property is the thrust value </param>
        /// <param name="t"> The index that will be used to determine the thrust from the list of keys </param>
        /// <returns> The thrust at a given second </returns>
        public static double LerpKeyList(List<Double2> list, double t){
            // init the function with pre and post keys as the first item
			Double2 preKey = list[0];
            Double2 postKey = preKey;

			// start looping through the list double2
            foreach (var item in list){
				if (item.x == t){
					// if the item's second is exactly what the user requested then return the thrust
					return item.y;
				}else if(item.x < t){
					// if the item is before the specified time then set it as preKey
					preKey = item;
				}else {
                    postKey = item; // postkey is now the first item whose second is after the requested
					break; // break out of the foreach after the item's second is past the requested
				}
            }
			
			// Example:
			// keys [5, 0.40], [15, 0.60], user wants thrust at 13 seconds
			// t = 13, preKey.x = 5,  maxT = 10; normalizedT = 0.8;
			// Double.Lerp(0.40, 0.60, 0.8); // returns 0.56

			// percentile of requested time between closest available keys
			double normalizedT = (t - preKey.x) / (postKey.x - preKey.x);
			return Double.Lerp(preKey.y, postKey.y, normalizedT);
        }

        /// <summary>
        /// Determines the lerp percentage and trajectory indexes for a given second in the simulation
        /// </summary>
        /// <param name="time">Simulation time in seconds that will be used to calculate indexes</param>
        /// <param name="index">Output: TrajectoryList index before the specified time</param>
        /// <param name="indexnext">Output: TrajectoryList index after the specified time</param>
        /// <param name="clamped">Optional: if true, allows method to sometimes return first and last index</param>
        /// <returns>Lerp percentage</returns>
        public double GetInterpolatedT(int time, out int index, out int indexnext, bool clamped = false){
            //time += (int)Math.Round(percentOffset * trajectory.Count * trajectoryResolution);

			// takes into account trajectoryResolution, 
            double accurateindex = ((double)time / trajectoryResolution);

			// is true if accurateIndex is between the start and last indices of trajectory
            bool withinClamp = accurateindex < TrajectoryList.Count - 1 && accurateindex > 0;

            if (clamped == false || withinClamp == true){
				// this block always gets executed if the user didn't provide clamped parameter / it's false 
				// or if it's true but accurateIndex was within the start and last indices of trajectory

                accurateindex %= TrajectoryList.Count; // gets the modulo of accurateIndex
                index = (int)Math.Floor(accurateindex); // round and cast to int
                indexnext = (index + 1) % TrajectoryList.Count; // if indexNext is the length of trajectoryArray then loop around to first

                double t = accurateindex - index; // percentage / lerp value
                return t;
            } else {
				// this block is executed if clamped == true and accurateIndex is 0 / greater than or equal to the last indice
				
				if(accurateindex > TrajectoryList.Count - 1){
					// executed if accurateIndex is greater than the last index of trajectoryCount
                    index = TrajectoryList.Count - 1; // simply returns the last index
                    indexnext = TrajectoryList.Count - 1;
                    return 1; // the y value of the last index
                } else {
					// executed if accurateIndex is 0 of if it is or equal to the last index;
                    index = 0;
                    indexnext = 0;
                    return 0; // the y value of the first index
                }
            }
        }

        /// <summary>
        /// Get the position of the rocket at a given second.
        /// Smoothing / linear interpolation is applied if the given second is between the available TrajectoryData
        /// </summary>
        /// <param name="timeSecond">Simulation time in seconds</param>
        /// <returns>The position of the rocket at a given second.</returns>
        public Double2 GetPositionAtTime(int timeSecond, bool clamped = false){
            double t = GetInterpolatedT(timeSecond, out int index, out int nextIndex, clamped);
            Double2 a = TrajectoryList[index].Pos;
            Double2 b = TrajectoryList[nextIndex].Pos;

            return Double2.Lerp(a, b, t);
        }

        /// <summary>
        /// Get the velocity of the rocket at a given second.
        /// Smoothing / linear interpolation is applied if the given second is between the available TrajectoryData
        /// </summary>
        /// <param name="timeSecond">Simulation time in seconds</param>
        /// <returns>The velocity of the rocket at a given second.</returns>
        public Double2 GetVelocityAtTime(int timeSecond, bool clamped = false){
            double t = GetInterpolatedT(timeSecond, out int index, out int nextIndex, clamped);
            Double2 a = TrajectoryList[index].Velocity;
            Double2 b = TrajectoryList[nextIndex].Velocity;

            return Double2.Lerp(a, b, t);
        }
        #endregion

        /// <summary>
        /// Calculates the next TrajectoryData and appends it to trajectories arra
        /// It also performs collision detection to see if the rocket has collided with any planets
        /// </summary>
        /// <param name="otherObjects">Array of celestial objects that will be taken into account</param>
        public void CalculateNext(CelestialBody[] otherObjects){
            var newForce = GetCurrentForces(otherObjects); // get total forces exerted on the object
            current_t_data.Force = newForce;
            current_t_data.Velocity += current_t_data.Force / current_t_data.mass;

            CelestialBody planetHit = null; // planet that body has intersected positions with
            Double2 intersection = Double2.Zero; // coordinates relative to the current position to the rocket where an intersection takes place

            //check if newPosition is inside planet
            foreach (var planet in otherObjects){
                var absolutePlanetPos = planet.GetPositionAtTime(localSecond); // absolute position of the planet
                var planetRadius = planet.radius; // radius of the planet
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
                var c = Math.Pow(relativePlanetPos.x, 2) + Math.Pow(relativePlanetPos.y, 2) - Math.Pow(planetRadius, 2);
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
                    if (Math.Abs(root) < Math.Abs(rocketVelocity.x))
                    {
                        planetHit = planet; // set planetHit to the current planet
                        intersection = new Double2(root, root * slope); // relative coordinates where the collision takes place
                        break; // exit from running the for loop
                    }
                }

                if (planetHit != null)
                {
                    // position of body now matches with the planet (intersection is relative from rocket)
                    current_t_data.Pos += intersection;
                    // velocity of body matches velocity of planet that was hit due to sticky collision
                    current_t_data.Velocity = planetHit.GetVelocityAtTime(localSecond);
                }
                else current_t_data.Pos += current_t_data.Velocity; // no collision so continue moving

                if (localSecond % trajectoryResolution == 0) TrajectoryList.Add(current_t_data);
                localSecond++;
            }
        }

        //forces
        #region Forces

        /// <summary>
        /// Calculates the forces being applied on the TrajectoryBody with planets being taken into account
        /// </summary>
        /// <param name="otherObjects"> Array of celestial objects that will be taken into account </param>
        /// <returns>The forces being applied on the rocket on the horizontal and vertical axis</returns>
        protected virtual Double2 GetCurrentForces(CelestialBody[] otherObjects){
            var newForce = Double2.Zero;
            newForce += GetGravity(otherObjects); // everything is affected by gravity

            return newForce;
        }

        /// <summary>
        /// Determines the forces of gravity acting on the trajectory body on the horizontal and vertical axis
        /// </summary>
        /// <param name="gravityHolders"> Array of celestial objects that will be taken into account </param>
        /// <returns>The forces of gravity being applied on the rocket on the hroziontal and vertical axi</returns>
        public Double2 GetGravity(CelestialBody[] gravityHolders){
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
        public double GetRawForceOfGravity(CelestialBody body, Double2 bodyPos){
            // this method returns the raw force of the planet on the body
            // universal gravity equation = gconst * (m1 * m2 / sqrdist
            double sqrdist = (current_t_data.Pos - bodyPos).SquareMagnitude;
            double rawForce = SpaceSimulation.gconst * (body.current_t_data.mass * current_t_data.mass / sqrdist);
            return rawForce;
        }
        #endregion
    }
}
