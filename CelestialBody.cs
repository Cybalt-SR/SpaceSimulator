<<<<<<< HEAD
﻿using System;
=======
<<<<<<< HEAD
using System;
>>>>>>> restructure
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSimulation
{
	// represents a planet
    [Serializable]
<<<<<<< HEAD
	// represents a planet
    public class CelestialBody : TrajectoryBody
    {
=======
    public class CelestialBody : TrajectoryBody
    {
        public readonly double radius;

>>>>>>> restructure
        protected override double GetLength()
        {
            return radius * 2;
        }
<<<<<<< HEAD
=======

>>>>>>> restructure
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
=======
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
>>>>>>> 12902e6f438decf90e77f0d8a87dc14b596b52c9
