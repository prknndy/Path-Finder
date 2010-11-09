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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">width of mesh in pixels</param>
        /// <param name="height">height of mesh in pixels</param>
        /// <param name="spacing">spacing between nodes in pixels</param>
        /// <param name="rects">list of obstacles</param>
        public Mesh(int width, int height, int spacing, Queue<Rect> rects)
        {
            // Initialize node mesh
            this.spacing = spacing;
            int meshWidth = width / spacing;
            int meshHeight = height / spacing;
            mesh = new GraphNode[meshWidth, meshHeight];
            // Create node mesh
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

            // Link mesh, removing solid (untraversable) nodes
            // TODO: Move linking code to the above loop
            // and automated reverse linking.
            for (int i_y = 1; i_y < (meshHeight - 1); i_y++)
            {
                for (int i_x = 1; i_x < (meshWidth - 1); i_x++)
                {
                    // left neighbor
                    if (!mesh[(i_x-1), i_y].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[(i_x-1), i_y]);
                    // left-top neighbor
                    if (!mesh[i_x-1, i_y-1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x - 1, i_y-1]);
                    // left-bottom neighbor
                    if (!mesh[i_x-1, i_y+1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x-1, i_y+1]);
                    // right neighbor
                    if (!mesh[i_x + 1, i_y].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x+1, i_y]);
                    // right-top neighbor
                    if (!mesh[i_x+1, i_y-1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x+1, i_y-1]);
                    // right-bottom neighbor
                    if (!mesh[i_x + 1, i_y+1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x + 1, i_y+1]);
                    // top neighbor
                    if (!mesh[i_x, i_y - 1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x, i_y - 1]);
                    // bottom neighbor
                    if (!mesh[i_x, i_y + 1].Solid())
                        mesh[i_x, i_y].Neighbors.Add(mesh[i_x, i_y+1]);

                }
            }

        }

        /// <summary>
        /// Weighs each mesh node for a given start and end destination.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the path for the weighted mesh.
        /// TODO: This should only be called if the 
        /// mesh has been weighed. Fix.
        /// </summary>
        /// <returns></returns>
        public Queue<MapPoint> FindPath()
        {
            // Generate travel path
            Queue<MapPoint> pathQueue = new Queue<MapPoint>();
            Travel(startNode, pathQueue);

            return pathQueue;

        }

        /// <summary>
        /// Recursively finds the path from the starting node
        /// to the ending node, adding each path node to
        /// queue pQ.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pQ"></param>
        public void Travel(GraphNode node, Queue<MapPoint> pQ)
        {
            if ((node == endNode) || (node == null))
                return;
            pQ.Enqueue(node);
            Travel(node.GetLightestNeighbor(), pQ);
        }

        /// <summary>
        /// Visits a node, weighs it, and add its
        /// neighbors to the queue to be visited.
        /// </summary>
        /// <param name="node"></param>
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


        /// <summary>
        /// Returns true if p is located in rect.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rects"></param>
        /// <returns></returns>
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
