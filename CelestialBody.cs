using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
    [Serializable]

    public class CelestialBody : TrajectoryBody {
        /// <summary>
        /// Represents a planet
        /// </summary>
        /// <param name="startingTrajectoryData"> Starting trajectory data of the planet </param>
        /// <param name="radius"> The radius of the planet </param>
        public CelestialBody(TrajectoryData startingTrajectoryData, double radius) : base(startingTrajectoryData) {
            Radius = radius;
        }

        public readonly double Radius;
    }
}
