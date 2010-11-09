using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PathFinder
{
    class Rect : IMaterial
    {
        public Point StartPoint { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private bool solid;

        public bool Solid()
        {
            return solid; 
        }

        public bool contains(Point p)
        {
            if ((p.X > StartPoint.X) && (p.X < EndPoint.X)) {
                if ((p.Y > StartPoint.Y) && (p.Y < EndPoint.Y))
                {
                    return true;
                }
            }

            return false;
        }

        public Point EndPoint
        {
            get
            {
                Point endPoint = new Point((StartPoint.X + this.Width), (StartPoint.Y + this.Height));
                return endPoint;
            }
        }

        public Rect()
        {
            this.solid = true;

        }
        public Rect (Point startPoint, int width, int height)
        {
            this.StartPoint = startPoint;
            this.Width = width;
            this.Height = height;

            this.solid = true;
        }

        public Rectangle GetRectangle()
        {
            Rectangle rect = new Rectangle(this.StartPoint.X, this.StartPoint.Y, this.Width, this.Height);
            return rect;
        }
    }
}
