using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PathFinder
{
    class Mesh
    {
        private GraphNode[,] mesh;
        private LinkedList<GraphNode> nodeQueue;
        private GraphNode startNode, endNode;
        private int spacing;

        public GraphNode[,] GetMesh()
        {
            return mesh;
        }

        public Mesh(int width, int height, int spacing, Queue<Rect> rects)
        {

            // find smallest rectangle side
            int minLength = 1000;
            foreach (Rect rect in rects)
            {
                if (rect.Width < minLength)
                {
                    minLength = rect.Width;
                }
                if (rect.Height < minLength)
                {
                    minLength = rect.Height;
                }
            }
            // set our spacing to shorter than that to avoid
            // "skipping" over a obstacle
            this.spacing = minLength - 1;
            this.spacing = spacing;
            // Generate node mesh
            int meshWidth = width / spacing;
            int meshHeight = height / spacing;
            mesh = new GraphNode[meshWidth, meshHeight];

            for (int i_y = 0; i_y < meshHeight; i_y++)
            {
                for (int i_x = 0; i_x < meshWidth; i_x++)
                {
                    int x = i_x * spacing;
                    int y = i_y * spacing;
                    GraphNode p = new GraphNode(x, y, false, Color.Black);
                    mesh[i_x, i_y] = p;
                    // Set as solid all points in a rectangle or on the border
                    if ((i_x == meshWidth) || (i_y == meshHeight) || (i_x == 0) || (i_y == 0) || (InRect(p, rects)))
                        mesh[i_x, i_y].setSolid(true);

                }
            }

            // Link mesh, removing solids
            for (int i_y = 1; i_y < (meshHeight - 1); i_y++)
            {
                for (int i_x = 1; i_x < (meshWidth - 1); i_x++)
                {
                    // left neighbor
                    if (!mesh[(i_x - 1), i_y].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[(i_x-1), i_y]);
                    // right neighbor
                    if (!mesh[(i_x + 1), i_y].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[(i_x+1), i_y]);
                    // top neighbor
                    if (!mesh[i_x, i_y - 1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x, i_y - 1]);
                    // bottom neighbor
                    if (!mesh[i_x, i_y + 1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x, i_y+1]);

                }
            }

        }

        public LinkedList<GraphNode> WeighGraph(Point startPoint, Point endPoint)
        {
            // Identify (approx) starting and ending nodes
            int startXIndex = startPoint.X / this.spacing;
            int startYIndex = startPoint.Y / this.spacing;
            int endXIndex = endPoint.X / this.spacing;
            int endYIndex = endPoint.Y / this.spacing;
            startNode = mesh[startXIndex, startYIndex];
            endNode = mesh[endXIndex, endYIndex];
            endNode.Weight = 0;
            // Calculate path weights
            nodeQueue = new LinkedList<GraphNode>();
            nodeQueue.AddFirst(endNode);
            LinkedListNode<GraphNode> currentNode = nodeQueue.First;
            do
            {
                if (currentNode.Value == startNode)
                    break;
                Traverse(currentNode.Value);
                currentNode = currentNode.Next;

            } while (currentNode != null);

            // Check for problems
            if (!nodeQueue.Contains(startNode))
                return null; // An error has occured

            return nodeQueue;

        }

        public Queue<MapPoint> FindPath()
        {
            // Generate travel path
            Queue<MapPoint> pathQueue = new Queue<MapPoint>();
            Travel(startNode, pathQueue);

            return pathQueue;

        }

        public void Travel(GraphNode node, Queue<MapPoint> pQ)
        {
            if ((node == endNode) || (node == null))
                return;
            pQ.Enqueue(node);
            Travel(node.GetLightestNeighbor(), pQ);
        }

        private void Traverse(GraphNode node)
        {
            foreach (GraphNode neighbor in node.Neighbors) {

                if (!nodeQueue.Contains(neighbor))
                {
                    neighbor.Weight = node.Weight + 1;
                    nodeQueue.AddLast(neighbor);
                }
                else if (neighbor.Weight > (node.Weight + 1))
                {
                    neighbor.Weight = node.Weight + 1;
                    // Remove neighbor and send to end of list
                    nodeQueue.Remove(neighbor);
                    nodeQueue.AddLast(neighbor);
                }
                
            }
            return;
        }



        private bool InRect(Point p, Queue<Rect> rects)
        {
            foreach (Rect rect in rects)
            {
                if (rect.contains(p))
                    return true;
            }
            return false;
        }

    }
}
