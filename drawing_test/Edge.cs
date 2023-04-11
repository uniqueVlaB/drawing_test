using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drawing_test
{
    class Edge
    {
        public Planet from { get; }
        public Planet to { get; }
        public Edge(Planet from, Planet to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
