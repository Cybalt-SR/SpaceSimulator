using System;
using System.Collections.Generic;

namespace SpaceSimulation
{
    [Serializable]
    public class SimulatedBody
    {
        public List<Double2> trajectory;
        
        protected int pathResolution => SpaceSimulation.pathResolution;

        #region utils
        public Double2 GetPositionAtTime(int timeSecond)
        {
            int index = (timeSecond / pathResolution) % trajectory.Count;
            return trajectory[index];
        }
        #endregion
    }
}
