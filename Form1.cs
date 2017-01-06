using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualVoronoi
{
    public partial class Form1 : Form
    {
        #region Drawing Variables
        Graphics g;
        bool drawBiomes = true;
        bool drawRivers = true;
        bool drawSites = false;
        bool drawCorners = false;
        bool drawDelaunay = false;
        bool isRandom = true;
        bool overlay = true;
        bool lighting = true;
        bool noisyEdges = true;
        bool smoothBlending = true;
        #endregion Drawing Variables

        #region Voronoi Variables 

        static bool moreRandom = false;
        int numSites = 6000;
        int numLloyd = 2;
        int bounds = 850;
        int seed = 0;
        VoronoiGraph curr;
        Random seedGen;
        Bitmap img;
        Bitmap img2;
        Bitmap fImg;

        #endregion Voronoi Variables

        public Form1()
        {
            InitializeComponent();
            g = this.MapPanel.CreateGraphics();
            
            seedGen = new Random();
        }
        public static VoronoiGraph CreateVoronoiGraph(int bounds, int numSites, int numLloyd, int seed)
        {
            Random r = new Random(seed);
            Voronoi v = new Voronoi(numSites, bounds, bounds, r);
            GraphImplementation graph = new GraphImplementation(v, numLloyd, r, moreRandom);
            return graph;
        }

        #region CreateButton

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (nSitesBox.Text != "")
            {
                try
                {
                    numSites = int.Parse(nSitesBox.Text);
                }
                catch (Exception)
                {
                    numSites = 6000;
                }
            }
            else
            {
                numSites = 6000;
            }
            if (nLloydBox.Text != "")
            {
                try
                {
                    numLloyd = int.Parse(nLloydBox.Text);
                }
                catch (Exception)
                {
                    numLloyd = 2;
                }
            }
            else
            {
                numLloyd = 2;
            }
            if (BoundsBox.Text != "")
            {
                try
                {
                    bounds = int.Parse(BoundsBox.Text);
                }
                catch (Exception)
                {
                    bounds = 850;
                }
            }
            else
            {
                bounds = 850;
            }
            if (isRandom)
            {
                seed = seedGen.Next();
                seedBox.Text = seed + "";
            }
            else if (seedBox.Text != "")
            {
                try
                {
                    seed = int.Parse(seedBox.Text);
                }
                catch (Exception)
                {
                    seed = seedGen.Next();
                    seedBox.Text = seed + "";
                }
            }
            else
            {
                seed = seedGen.Next();
                seedBox.Text = seed + "";
            }

            curr = CreateVoronoiGraph(bounds, numSites, numLloyd, seed);
            img = curr.CreateMap(g, drawBiomes, drawRivers, drawSites, drawCorners, drawDelaunay, noisyEdges, smoothBlending, lighting);

            fImg = new Bitmap(bounds, bounds);
            Graphics tG = Graphics.FromImage(fImg);
            tG.DrawImage(img, new PointF(0, 0));

            // EXPERIMENTAL Noise Application (Gives map "texture")
            if (overlay)
            {
                if (!File.Exists(@"gscale.png"))
                {
                    SimplexNoise s = new SimplexNoise(16, 0.25, 555);
                    img2 = s.GenerateGreyScale(bounds, bounds);
                    img2.Save("gscale.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                // Read noise in from file, much faster, looks worse at higher res
                img2 = new Bitmap(@"gscale.png");
                img2 = new Bitmap(img2, bounds, bounds);
                tG.DrawImage(img2, new PointF(0, 0));
            }

            MapImageBox.Image = fImg;
            MapPanel.Refresh();
        }

        #endregion CreateButton

        #region RefreshButton

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            curr.Paint(System.Drawing.Graphics.FromImage(img), drawBiomes, drawRivers, drawSites, drawCorners, drawDelaunay, false, noisyEdges, smoothBlending, lighting);
            fImg = new Bitmap(bounds, bounds);
            Graphics tG = Graphics.FromImage(fImg);
            tG.DrawImage(img, new PointF(0, 0));

            // EXPERIMENTAL Noise Application (Gives map "texture")
            if (overlay)
            {
                if (img2 == null)
                {
                    if (!File.Exists(@"gscale.png"))
                    {
                        SimplexNoise s = new SimplexNoise(16, 0.25, 555);
                        img2 = s.GenerateGreyScale(bounds, bounds);
                        img2.Save("gscale.png", System.Drawing.Imaging.ImageFormat.Png);
                    }

                    // Read noise in from file instead, much faster, looks worse at higher res.
                    img2 = new Bitmap("gscale.png");
                    img2 = new Bitmap(img2, bounds, bounds);
                }
                tG.DrawImage(img2, new PointF(0, 0));
            }
            

            MapImageBox.Image = fImg;
            MapPanel.Refresh();
            this.MapImageBox.Refresh();
        }

        #endregion RefreshButton

        #region Map Dragging

        private System.Drawing.Point StartPoint = new System.Drawing.Point();
        private bool isDragged = false;

        private void MapImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {
                System.Drawing.Point cPoint = new System.Drawing.Point(e.Location.X - StartPoint.X, e.Location.Y - StartPoint.Y);
                MapPanel.AutoScrollPosition = new System.Drawing.Point(-MapPanel.AutoScrollPosition.X - cPoint.X, -MapPanel.AutoScrollPosition.Y - cPoint.Y);
            }
        }

        private void MapImageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                StartPoint = e.Location;
            isDragged = true;
        }

        private void MapImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDragged = false;
        }

        #endregion Map Dragging

        #region Map Zoom

        private void Zoom(object sender, MouseEventArgs e)
        {

            if (e.Delta < 0)
            {
                MapImageBox.Width = Convert.ToInt32(MapImageBox.Width / 1.2);
                MapImageBox.Height = Convert.ToInt32(MapImageBox.Height / 1.2);
                MapImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                MapImageBox.Width = Convert.ToInt32(MapImageBox.Width * 1.2);
                MapImageBox.Height = Convert.ToInt32(MapImageBox.Height * 1.2);
                MapImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void MapPanel_MouseHover(object sender, EventArgs e)
        {
            MapImageBox.Focus();
        }

        private void MapImageBox_MouseHover(object sender, EventArgs e)
        {
            MapImageBox.Focus();
        }

        #endregion Map Zoom

        #region CheckBoxes

        private void BiomeCheck_CheckedChanged(object sender, EventArgs e)
        {
            drawBiomes = BiomeCheck.Checked;
        }

        private void RiverCheck_CheckedChanged(object sender, EventArgs e)
        {
            drawRivers = RiverCheck.Checked;
        }

        private void SitesCheck_CheckedChanged(object sender, EventArgs e)
        {
            drawSites = SitesCheck.Checked;
        }

        private void CornerCheck_CheckedChanged(object sender, EventArgs e)
        {
            drawCorners = CornerCheck.Checked;
        }

        private void DelaunayCheck_CheckedChanged(object sender, EventArgs e)
        {
            drawDelaunay = DelaunayCheck.Checked;
        }

        private void RandomCheck_CheckedChanged(object sender, EventArgs e)
        {
            isRandom = RandomCheck.Checked;
        }

        private void RHeightCheck_CheckedChanged(object sender, EventArgs e)
        {
            moreRandom = RHeightCheck.Checked;
        }

        private void NoisyCheck_CheckedChanged(object sender, EventArgs e)
        {
            noisyEdges = NoisyCheck.Checked;
        }

        private void OverlayCheck_CheckedChanged(object sender, EventArgs e)
        {
            overlay = OverlayCheck.Checked;   
        }

        private void BlendCheck_CheckedChanged(object sender, EventArgs e)
        {
            smoothBlending = BlendCheck.Checked;
        }

        private void LightingCheck_CheckedChanged(object sender, EventArgs e)
        {
            lighting = LightingCheck.Checked;
        }

        #endregion CheckBoxes

        private void SaveButton_Click(object sender, EventArgs e)
        {
            fImg.Save(seed + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
