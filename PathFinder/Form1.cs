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


        private const int GRID_SPACING = 20;
        private const string defaultStatus = "Use the buttons above to add obstacles and path points";

        private Point lastPoint;
        private MapPoint StartPoint;
        private MapPoint EndPoint;

        Queue<Rect> rects;
        List<MapPoint> points;
        LinkedList<GraphNode> nodes;

        public Form1()
        {
            InitializeComponent();
            rects = new Queue<Rect>();
            points = new List<MapPoint>();
            toolStripStatusLabel1.Text = defaultStatus;
            appStatus = 0;
           
        }

        private Point getGridPoint(MouseEventArgs e)
        {
            int x = (e.X / GRID_SPACING) * GRID_SPACING;
            int y = (e.Y / GRID_SPACING) * GRID_SPACING;
            Point p = new Point(x, y);
            return p;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            // Downcast eventargs
            MouseEventArgs mouseClick = (MouseEventArgs)e;

            switch (appStatus)
            {
                case ADD_RECT_START:
                    lastPoint = getGridPoint(mouseClick);
                    appStatus = ADD_RECT_END;
                    toolStripStatusLabel1.Text = "Click to finish obstacle"; 
                    break;
                case ADD_RECT_END:
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
                    if (StartPoint == null)
                    {
                        StartPoint = new MapPoint(getGridPoint(mouseClick), false, Color.Blue);
                        points.Add(StartPoint);
                        appStatus = IDLE;
                        toolStripStatusLabel1.Text = defaultStatus;
                    }
                    break;
                case ADD_END:
                    if (EndPoint == null)
                    {
                        EndPoint = new MapPoint(getGridPoint(mouseClick), false, Color.Red);
                        points.Add(EndPoint);
                        appStatus = IDLE;
                        toolStripStatusLabel1.Text = defaultStatus;
                    }
                    break;
                    

            }
            pictureBox1.Invalidate();
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            SolidBrush greenBrush = new SolidBrush(Color.Green);
            Pen blackPen = new Pen(Color.Black, 1);
            // Draw grid
            for (int x = 0; x < pictureBox1.Width; x += GRID_SPACING)
            {
                g.DrawLine(blackPen, x, 0, x, pictureBox1.Height);
            }
            for (int y = 0; y < pictureBox1.Height; y += GRID_SPACING)
            {
                g.DrawLine(blackPen, 0, y, pictureBox1.Width, y);
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
            // Draw weights
            if (nodes != null)
            {
                Font drawFont = new Font("Arial", 12);
                SolidBrush drawBrush = new SolidBrush(Color.Black);

                foreach (GraphNode node in nodes)
                {

                    g.DrawString(node.Weight.ToString(), drawFont, drawBrush, node.X, node.Y);
                }
            }
        }

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

        private void FindPath()
        {
            Mesh mesh = new Mesh(pictureBox1.Width, pictureBox1.Height, GRID_SPACING, rects);

            nodes = mesh.WeighGraph(StartPoint, EndPoint);

            if (nodes != null)
            {
                points.AddRange(mesh.FindPath());
            }

            pictureBox1.Invalidate();

        }
    }
}
