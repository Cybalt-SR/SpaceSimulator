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
        /// <param name="tdata"> Starting trajectory data of the planet </param>
        /// <param name="Radius"> The radius of the planet </param>
        public CelestialBody(TrajectoryData tdata, double Radius) : base(tdata) {
            radius = Radius;
        }

        public readonly double radius;
    }
}
