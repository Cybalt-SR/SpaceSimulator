﻿using System;
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

        public void CalculateNext(CelestialBody[] otherObjects){
            if (localSecond % trajectoryResolution == 0) trajectory.Add(current_t_data);
            var newForce = GetCurrentForces(otherObjects); // get total gravity exerted
            current_t_data.Force = newForce;
            current_t_data.Velocity += current_t_data.Force / current_t_data.mass;

            CelestialBody planetHit = null; // planet that body has intersected positions with
            Double2 intersection = Double2.zero; // coordinates relative to the current position to the rocket where an intersection takes place

            //check if newPosition is inside planet
            foreach (var planet in otherObjects) {
                var absolutePlanetPos = planet.GetPositionAtTime(calculationSecond); // absolute position of the planet
                var planetRadius = planet.radius; // radius of the planet
                var relativePlanetPos = planetPos - current_t_data.Pos; // position of planet relative to rocket
                var rocketVelocity = current_t_data.Velocity; // velocity of the rocket

				// How this works is you have a system of equations of a line and a circle
				// if you solve them together you have a quadratic equation, 
				// and you can determine through the discriminant if there is an intersection
				
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
                
				var a = Math.pow(slope, 2) + 1;
				var b = (-2.0 * relativePlanetPos.x) + (-2.0 * slope * relativePlanetPos.y);
				var c = Math.pow(relativePlanetPos.x, 2) + Math.pow(relativePlanetPos.y, 2) - Math.pow(planetRadius, 2);
                var discriminant = (b * b) - (4 * a * c);

				// discriminant = 0 then there is 1 collision
				// discriminant > 0 then there is 2 collisions
                if (discriminant >= 0){
					// x sign is the directionality of travel of the ship along the slope
					// it allows the code to get the closer intersection
                    double xsign = Math.Sign(rocketVelocity.x); // gets the sign of a number, either 1 or -1
                    var root = (-b - (xsign * Math.Sqrt(discriminant))) / (a * 2.0); // relative x coordinate of where collision takes place

					// If the root is between the initial position and the final position, then there has been a collision
					// current position does not need to be checked since this is relative to the origin, 0 < root < finalPosition
                    if (Math.Abs(root) < Math.Abs(rocketVelocity.x)){
                        planetHit = planet; // set planetHit to the current planet
                        intersection = new Double2(root, root * slope); // relative coordinates where the collision takes place
                        break; // exit from running the for loop
                    }
                }
            }

            if (planetHit != null){	
				// position of body now matches with the planet (intersection is relative from rocket)
                current_t_data.Pos += intersection;

				// velocity of body matches velocity of planet that was hit due to sticky collision
                current_t_data.Velocity = planetHit.GetVelocityAtTime(calculationSecond);
            }else current_t_data.Pos += current_t_data.Velocity; // no collision so continue moving

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
