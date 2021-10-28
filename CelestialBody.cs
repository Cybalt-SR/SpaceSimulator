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

        public readonly double radius;
    }
}
