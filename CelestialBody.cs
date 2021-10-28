using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
    [Serializable]
	// represents a planet
    public class CelestialBody : TrajectoryBody
    {
        public double radius = 0;
        public double eccentricity = 0;
        public double inclination = 0;
        public double Longitude_ascendingNode = 0;
        public double Longitude_periapsis = 0;
        public double periapsis = 0;
        public double apoapsis = 0;
    }
}
