using System;
using SpaceSimulation;

namespace MyDemonstrationProgram
{
    class Program
    {
#if !(UNITY_EDITOR || UNITY_STANDALONE)
        static void Main(string[] args){
            double[] thrustKeys = { 0, 0, 0, 0, 0 };
            double[] angularThrustKeys = { 0.1, 0.0, -0.2, 0.0, 0.1 };

            Double2 startingPos = new Double2(0, 0); // position of the rocket
            TrajectoryData startingTrajectoryData = new TrajectoryData(10000, startingPos, Double2.Zero, 45, 0);
            ThrustBody rocket = new ThrustBody(startingTrajectoryData, thrustKeys, angularThrustKeys, 1, 1000, 1000);

			Double2 planetStartingPos = new Double2(0, -10000); // position of the planet
            TrajectoryData planetTData = new TrajectoryData(7.5 * Math.Pow(10, 20), planetStartingPos, Double2.Zero, 0, 0);
            CelestialBody earth = new CelestialBody(planetTData, 4000);
			CelestialBody[] planets = { earth }; // create the planets array with only one planet as the item

			Console.WriteLine("Initial TrajectoryData: ");
			startingTrajectoryData.PrintToConsole();

            for (int currentTime = 0; currentTime < thrustKeys.Length; currentTime++) {
                Console.WriteLine("Currently on iteration: " + currentTime);
                rocket.CalculateNext(planets);
				rocket.CurrentTrajectoryData.PrintToConsole();
            }
        }
# endif
    }
}
