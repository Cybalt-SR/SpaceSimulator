using System;

namespace SpaceSimulation
{
    // "universal constants"
    public static class SpaceSimulation
    {
        public static double gconst = 0.000000000066743;
        public static double EarthMass = 5.9722 * Math.Pow(10, 24); // in kg
        public static double EarthRadius = 6371000;	// in meters
        public static double SaturnVRocketLength = 111; // in meters
        public const double SaturnVMass = 2812272.69; // in kg
    }
}
