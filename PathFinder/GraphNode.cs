using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PathFinder
{
    class GraphNode : MapPoint
    {
        public List<GraphNode> Neighbors;
        private int weight;
        public int Weight
        {
            get { return weight; }
            set { weight = value; visited = true; }
        }
        public bool visited;

        public GraphNode(int x, int y, bool solid, Color color) : base(x, y, solid, color)
        {
            this.visited = false;
            Neighbors = new List<GraphNode>();
        }

        public GraphNode GetLightestNeighbor()
        {

            // Return the lightest neighboring node
            GraphNode lightest = this;

            foreach (GraphNode neighbor in Neighbors)
            {
                if ((neighbor.Weight < lightest.Weight) && neighbor.visited)
                    lightest = neighbor;
            }

            if (lightest == this)
                return null;

            return lightest;
        }

    }
}
