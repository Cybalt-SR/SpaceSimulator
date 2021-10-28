using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
	// represents a planet
    [Serializable]
    public class CelestialBody : TrajectoryBody
    {
        public readonly double radius;

        protected override double GetLength()
        {
            return radius * 2;
        }

        /// <summary>
        /// Represents a planet
        /// </summary>
        /// <param name="tdata"></param>
        /// <param name="Radius"></param>
        public CelestialBody(TrajectoryData tdata, double Radius) : base(tdata) {
            radius = Radius;
        }
    }
}
