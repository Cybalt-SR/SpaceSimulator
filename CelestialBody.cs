using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
    [Serializable]
    public class CelestialBody : TrajectoryBody
    {
        public double Radius { get; private set; }

        /// <summary>
        /// Represents a planet
        /// </summary>
        /// <param name="startingTrajectoryData"> Starting trajectory data of the planet </param>
        /// <param name="radius"> The radius of the planet </param>
        public CelestialBody(TrajectoryData startingTrajectoryData, double radius, List<TrajectoryData> Trajectory = null, int interval = 1) : base(startingTrajectoryData, Trajectory, interval)
        {
            Radius = radius;
        }

        protected override double GetLength()
        {
            return Radius * 2;
        }

        public void ChangeRadius(double newRadius)
        {
            Radius = newRadius;
        }
    }
}
