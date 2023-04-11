using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drawing_test
{
    class Planet
    {
        public PointF Position { get; set; }
        public List<Planet> Connections { get; set; }

        public Planet(PointF position)
        {
            Position = position;
            Connections = new List<Planet>();
        }

    }
}
