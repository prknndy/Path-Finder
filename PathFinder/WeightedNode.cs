using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    class WeightedNode
    {
        public GraphNode GNode { get; set; }
        public int Weight { get; set; }

        public WeightedNode(GraphNode GNode, int weight)
        {
            this.GNode = GNode;
            this.Weight = weight;
        }
    }
}
