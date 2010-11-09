using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PathFinder
{
    class MapPoint : Point, IMaterial
    {
        private bool solid;

        public Color MyColor { get; set; }

        public bool Solid()
        {
            return this.solid;
        }

        public void setSolid(bool s)
        {
            this.solid = s;
        }

        public MapPoint(Point p, bool solid, Color color) : base (p.X, p.Y)
        {
            this.solid = solid;
            this.MyColor = color;
        }

        public MapPoint(int x, int y, bool solid, Color color) : base(x, y)
        {
            this.solid = solid;
            this.MyColor = color;

        }
    }
}
