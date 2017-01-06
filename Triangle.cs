using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualVoronoi
{
    class Triangle
    {
        private List<Site> sites;

        public List<Site> GetSites()
        {
            return sites;
        }

        public Triangle(Site a, Site b, Site c)
        {
            sites = new List<Site>();
            sites.Add(a);
            sites.Add(b);
            sites.Add(c);
        }

        public void Dispose()
        {
            sites.Clear();
            sites = null;
        }
    }
}
