using System;
using System.Collections.Generic;

namespace SpaceSimulation
{
    [Serializable]
    public class SimulatedBody
    {
        public Double2 unscaledPos;
        public List<Double2> trajectory;
        
        protected int pathResolution => SpaceSimulation.pathResolution;

        #region utils
        public double RealDist(Double2 pos1, Double2 pos2)
        {
            return (pos1 - pos2).magnitude * SpaceSimulation.scale;
        }

        public Double2 GetUnscaledPositionAtTime(int timeSecond)
        {
            int index = timeSecond / pathResolution;
            return trajectory[index];
        }
        #endregion
    }
}
