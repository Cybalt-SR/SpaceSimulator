using System;
using System.Collections.Generic;

namespace SpaceSimulation
{	
	// "universal constants"
    public static class SpaceSimulation
    {
        public static double scale;
        public const double gconst = 0.000000000066743;
		public const double PI = 3.1415926535897931;
        public const double exhaustVelo = 2400; // Saturn V exhaust velocity 2.40 * 10^3 m/s (Make a method for this later?)
        public const double fuelBurnRate = 14000; // Saturn V Fuel burn rate is 1.40 * 10^4 kg/s (This should be a constant since it doesn't change for the entire flight duration)
        public const double maxTorque = 200000; // theoretical max turning force of a rocket
    }
}
