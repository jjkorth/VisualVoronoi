using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualVoronoi
{
    public class Center
    {
        public int index;
        public Point loc;
        public List<Corner> corners = new List<Corner>();
        public List<Center> neighbors = new List<Center>();
        public List<DEdge> borders = new List<DEdge>();
        public bool border;
        public bool ocean;
        public bool water;
        public bool coast;
        public double elevation;
        public double moisture;
        public Enum biome;
        public double area;
        public Center()
        {

        }

        public Center(Point loc)
        {
            this.loc = loc;
        }
    }
}
