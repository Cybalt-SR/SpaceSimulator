using System;
using System.Collections.Generic;
using SpaceSimulation;

namespace ConsoleApp1{
    class Program {
        static void Main(string[] args){
            double[] keys = { 1, 1, 1, 1, 1, 1, 1 };
			List<Double2> lerpable = convertSimpleKeysToLerpable(keys);

            Double2 startingPos = new Double2(0, 50); // position of the rocket
            TrajectoryData startingTrajectoryData = new TrajectoryData(startingPos, Double2.Zero, 45, 0, 10000);
            ThrustBody rocket = new ThrustBody(startingTrajectoryData, lerpable, 1, 1000, 1000);

			Double2 planetStartingPos = new Double2(0, -10000); // position of the planet
            TrajectoryData planetTData = new TrajectoryData(planetStartingPos, Double2.Zero, 0, 0, 10000);
            CelestialBody earth = new CelestialBody(planetTData, 500);
			CelestialBody[] planets = { earth }; // create the planets array with only one planet as the item

            int maxSimTime = keys.Length * 1;
            for (int currentTime = 0; currentTime < maxSimTime; currentTime++) {
                Console.WriteLine("Currently on iteration: " + currentTime);
                rocket.CalculateNext(planets);
            }

            TrajectoryData CurrentTrajectoryData = rocket.CurrentTrajectoryData;
            Console.WriteLine("Done performing calculations, here is the rocket position: " + CurrentTrajectoryData.Pos);
			Console.WriteLine("Done performing calculations, here is the rocket velocity: " + CurrentTrajectoryData.Velocity);
        }

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

