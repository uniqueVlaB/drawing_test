using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drawing_test
{
     static class Map
    {
        public static List<Planet> planets { get; set; }
        public static List<Edge> edges { get; set; }
        public static int height { get; set; }
        public static int width { get; set; }
        public static int minDistanceBetweenPlanets { get; set; }
        public static int minDistanceFromPlanetToEdge { get; set; }
        public static int numPlanets { get; set; }
    }
}
