using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    class ComputeLogic
    {
        private uint trainColor;
        private Map map;
        private Paths paths;

        public ComputeLogic(Map currentMap, uint color)
        {
            trainColor = color;
            map = currentMap;
        }

        public Direction nextStep()
        {
            paths = new Paths(map, map.BestTrainEver);

            paths.createPaths();
            paths.PathsHeuristic();

            return paths.getByPriority();
        }
    }
}
