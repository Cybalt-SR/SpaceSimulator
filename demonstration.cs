using System;
using System.Collections.Generic;
using SpaceSimulation;

namespace ConsoleApp1{
    class DemonstrationProgram {

#if !UNITY_EDITOR
        static void Main(string[] args){
            double[] thrustKeys = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };
            double[] angleKeys = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // double[] angleKeys = { -0.20, 0.20, -0.40, 0.40, -0.60, 0.60, -0.80, 0.80, -1, 1 };
			
			List<Double2> lerpableThrustKeys = convertSimpleKeysToLerpable(thrustKeys);
			List<Double2> lerpableAngleKeys = convertSimpleKeysToLerpable(angleKeys);

            Double2 startingPos = new Double2(0, 50); // position of the rocket
            TrajectoryData startingTrajectoryData = new TrajectoryData(10000, startingPos, Double2.Zero, 45, 0);
            ThrustBody rocket = new ThrustBody(startingTrajectoryData, lerpableThrustKeys, lerpableAngleKeys, 1, 1000, 1000);

			Double2 planetStartingPos = new Double2(0, -10000); // position of the planet
            TrajectoryData planetTData = new TrajectoryData(10000, planetStartingPos, Double2.Zero, 0, 0);
            CelestialBody earth = new CelestialBody(planetTData, 500);
			CelestialBody[] planets = { earth }; // create the planets array with only one planet as the item

            int maxSimTime = thrustKeys.Length * 1;
            for (int currentTime = 0; currentTime < maxSimTime + 1; currentTime++) {
                Console.WriteLine("Currently on iteration: " + currentTime);
            	
				TrajectoryData CurrentTrajectoryData = rocket.CurrentTrajectoryData;
				Console.WriteLine("Rocket velocity: " + CurrentTrajectoryData.Velocity);
            	Console.WriteLine("Rocket position: " + CurrentTrajectoryData.Pos);
				Console.WriteLine("");
				
                rocket.CalculateNext(planets);
            }
        }
# endif

        static List<Double2> convertSimpleKeysToLerpable(double[] simpleKeys) {
            List<Double2> response = new List<Double2>();

            for (int i = 0; i < simpleKeys.Length; i++) {
                Double2 key = new Double2(i, simpleKeys[i]);
                response.Add(key);
            }
            
            return response;
        }
    }
}
