using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathFinder
{
    
    public partial class Form1 : Form
    {
        private int appStatus;

        private const int IDLE = 0;
        private const int ADD_RECT_START = 1;
        private const int ADD_RECT_END = 2;
        private const int ADD_START = 3;
        private const int ADD_END = 4;
        private const int PATH_FINDING = 5;

        private const int MIN_WIDTH = 150;
        private const int MIN_FOR_SHOW_WEIGHT = 20;
        private const int MIN_FOR_SHOW_GRID = 10;
        private const string defaultStatus = "Use the buttons above to add obstacles and path points";

        private Point lastPoint; // stores the first mouse click when creating an obstacle
        private MapPoint StartPoint;
        private MapPoint EndPoint;
        private int gridSpacing;
        /// <summary>
        /// List of rectangles that represent obstacles.
        /// </summary>
        Queue<Rect> rects;
        /// <summary>
        /// List of map points to be rendered.
        /// </summary>
        List<MapPoint> points;
   
        // TODO: Handle below more appropriately
        LinkedList<GraphNode> nodes;

        public Form1(int width, int height, int resolution)
        {
            
            InitializeComponent();
            
            // reset picture + form size based on inputs
            this.pictureBox1.Size = new System.Drawing.Size(width, height);
            if (width < MIN_WIDTH)
                width = MIN_WIDTH;
            this.ClientSize = new System.Drawing.Size(width+160, height+40);
            gridSpacing = resolution;

            rects = new Queue<Rect>();
            points = new List<MapPoint>();
            toolStripStatusLabel1.Text = defaultStatus;
            appStatus = IDLE;
           
        }

        /// <summary>
        /// Snaps a mouse event to the grid and returns the grid point.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Point getGridPoint(MouseEventArgs e)
        {
            int x = (e.X / gridSpacing) * gridSpacing;
            int y = (e.Y / gridSpacing) * gridSpacing;
            Point p = new Point(x, y);
            return p;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            // Downcast eventargs
            MouseEventArgs mouseClick = (MouseEventArgs)e;
            // Route mouse event based on current application status
            switch (appStatus)
            {
                case ADD_RECT_START:
                    // Create first point of new obstacle
                    lastPoint = getGridPoint(mouseClick);
                    appStatus = ADD_RECT_END;
                    toolStripStatusLabel1.Text = "Click to finish obstacle"; 
                    break;
                case ADD_RECT_END:
                    // Finish new obstacle
                    Point p = getGridPoint(mouseClick);
                    int x = p.X;
                    int y = p.Y;
                    int width, height, startX, startY;
                    if (x > lastPoint.X)
                    {
                        width = x - lastPoint.X;
                        startX = lastPoint.X;
                    }
                    else
                    {
                        width = lastPoint.X - x;
                        startX = x;
                    }
                    if (y > lastPoint.Y)
                    {
                        height = y - lastPoint.Y;
                        startY = lastPoint.Y;
                    }
                    else
                    {
                        height = lastPoint.Y - y;
                        startY = y;
                    }
                    Point startPoint = new Point(startX, startY);
                    Rect rect = new Rect(startPoint, width, height);
                    rects.Enqueue(rect);
                    appStatus = IDLE;
                    toolStripStatusLabel1.Text = defaultStatus;
                    break;
                case ADD_START:
                    // Add path start point
                    if (StartPoint == null)
                    {
                        StartPoint = new MapPoint(getGridPoint(mouseClick), false, Color.Blue);
                        points.Add(StartPoint);
                        appStatus = IDLE;
                        toolStripStatusLabel1.Text = defaultStatus;
                    }
                    break;
                case ADD_END:
                    // Add path end point
                    if (EndPoint == null)
                    {
                        EndPoint = new MapPoint(getGridPoint(mouseClick), false, Color.Red);
                        points.Add(EndPoint);
                        appStatus = IDLE;
                        toolStripStatusLabel1.Text = defaultStatus;
                    }
                    break;
                    

            }
            // Redraw the pictureBox
            pictureBox1.Invalidate();
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            SolidBrush greenBrush = new SolidBrush(Color.Green);
            Pen blackPen = new Pen(Color.Black, 1);
            if (this.gridSpacing > MIN_FOR_SHOW_GRID)
            {
                // Draw grid if the spacing is large enough it won't be too
                // crowded.
                for (int x = 0; x < pictureBox1.Width; x += gridSpacing)
                {
                    g.DrawLine(blackPen, x, 0, x, pictureBox1.Height);
                }
                for (int y = 0; y < pictureBox1.Height; y += gridSpacing)
                {
                    g.DrawLine(blackPen, 0, y, pictureBox1.Width, y);
                }
            }
            // Draw obstacles
            foreach (Rect rect in rects)
            {
                g.FillRectangle(greenBrush, rect.GetRectangle());
            }
            // Draw points
            foreach (MapPoint point in points)
            {
                DrawMapPoint(point, g);
            }
            if ((nodes != null) && (gridSpacing > MIN_FOR_SHOW_WEIGHT))
            {
                // Draw nodes' weight numbers if the spacing is large enough
                Font drawFont = new Font("Arial", 12);
                SolidBrush drawBrush = new SolidBrush(Color.Black);

                foreach (GraphNode node in nodes)
                {

                    g.DrawString(node.Weight.ToString(), drawFont, drawBrush, node.X, node.Y);
                }
            }
        }
        /// <summary>
        /// Draws a single MapPoint point in Graphics component g.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="g"></param>
        private void DrawMapPoint(MapPoint point, Graphics g)
        {
            SolidBrush pointBrush = new SolidBrush(point.MyColor);
            Rectangle rect = new Rectangle(point.X - 5, point.Y - 5, 10, 10);
            g.FillEllipse(pointBrush, rect);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            appStatus = ADD_RECT_START;
            toolStripStatusLabel1.Text = "Click to select start point of obstacle";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Click to select path start point";
            appStatus = ADD_START;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Click to select path end point";
            appStatus = ADD_END;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if ((StartPoint != null) && (EndPoint != null))
            {
                FindPath();
            }
        }
        /// <summary>
        /// Creates a mesh from the current grid and finds the path.
        /// </summary>
        private void FindPath()
        {
            Mesh mesh = new Mesh(pictureBox1.Width, pictureBox1.Height, gridSpacing, rects);
            // Generate the mesh and weigh the nodes
            nodes = mesh.WeighGraph(StartPoint, EndPoint);
            // If the mesh creation was succesful, find the path and add
            // it's point to the point list.
            if (nodes != null)
            {
                points.AddRange(mesh.FindPath());
            }
            // Redraw
            pictureBox1.Invalidate();

        }
    }
}
