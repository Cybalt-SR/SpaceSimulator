using System;
using System.Collections.Generic;
using SpaceSimulation;

namespace ConsoleApp1{
    class Program {
        static void Main(string[] args){
            double[] keys = { 0.1, 0, 0, 0 };
            Double2 startingPos = new Double2(0, 50);
            TrajectoryData startingTrajectoryData = new TrajectoryData(startingPos, Double2.Zero, 45, 0, 10000000000);
            ThrustBody rocket = new ThrustBody(startingTrajectoryData, 100, convertSimpleKeysToLerpable(keys));

            double earthMass = 5.972 * Math.Pow(10, 24);
            TrajectoryData planetTData = new TrajectoryData(Double2.Zero, Double2.Zero, 0, 0, earthMass);
            CelestialBody earth = new CelestialBody(planetTData, 6371000);
            CelestialBody[] planets = { earth };

            int maxSimTime = keys.Length;
            for (int currentTime = 0; currentTime < maxSimTime; currentTime++) {
                Console.WriteLine("Currently on iteration: " + currentTime);
                rocket.CalculateNext(planets);
            }

            Console.WriteLine("Done performing calculations, here is the rocket velocity " + rocket.current_t_data.Velocity);
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

