using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenSaver
{
    public partial class ScreenSaver : UserControl, IDisposable
    {

        private List<Serpent> lstSerpents = new List<Serpent>();

        public ScreenSaver()
        {
            InitializeComponent();
        }

        public ScreenSaver(Color couleurFond, Size dimensions)
        {
            InitializeComponent();
            this.BackColor = couleurFond;
            this.Size = dimensions;

            lstSerpents.Add(new Serpent(this));

            timer1.Interval = 5000; // Intervalle de 5s entre la création de chaque serpent
            timer1.Enabled = true;
            timer1.Start();
        }

        protected virtual void timer1_Tick(object sender, System.EventArgs e)
        {
            lstSerpents.Add(new Serpent(this));
        }

        void IDisposable.Dispose()
        {
            base.Dispose();

            foreach (Serpent objSerpent in lstSerpents)
                objSerpent.Supprimer();
        }

        private class Serpent : IDisposable
        {
            private ScreenSaver objParent;
            private Color cCouleur;
            private Point ptPoint;
            private int intHeight;
            private int intWidth;
            private int intNbX;
            private int intNbY;
            private int intOffsetX;
            private int intOffsetY;
            private int intOffsetPrecX = -2;
            private int intOffsetPrecY = -2;

            private Timer tmIntervalle = new Timer();

            private Dictionary<int, List<int>> dicPoints = new Dictionary<int, List<int>>();

            protected internal Serpent(ScreenSaver parent)
            {
                objParent = parent;
                intNbX = 0;
                intNbY = 0;
                intOffsetPrecX = -2;
                intOffsetPrecY = -2;

                Random r = new Random((int)(DateTime.Now.Ticks & 0xFFFF));

                cCouleur = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));

                // Détermination des dimensions de l'élipse
                intHeight = r.Next(objParent.Height / 15, objParent.Height / 5 + 1);
                intWidth = r.Next(objParent.Width / 15, objParent.Width / 5 + 1);

                // Détermination du point de départ
                r = new Random((int)(DateTime.Now.Ticks & 0xFFFF));
                ptPoint = new Point(r.Next(0, objParent.Width + 1), r.Next(0, objParent.Height + 1));

                tmIntervalle.Tick += new EventHandler(tmIntervalle_Tick);

                tmIntervalle.Interval = 800;  // Intervalle de 800ms entre le dessin des élipses
                tmIntervalle.Tick += tmIntervalle_Tick;
                tmIntervalle.Enabled = true;
                tmIntervalle.Start();
            }

            protected internal void Supprimer()
            {
                tmIntervalle.Stop();
            }

            private void tmIntervalle_Tick(object sender, System.EventArgs e)
            {
                DessinerElipse();
            }

            private void DessinerElipse()
            {
                try
                {
                    // Détermination des nouvelles coordonnées
                    do
                    {
                        if (intNbX == 0)
                        {
                            Random r = new Random((int)(DateTime.Now.Ticks & 0xFFFF));
                            intOffsetX = r.Next(-1, 2);
                            intNbX = r.Next(1, 31);
                        }
                        else
                            intNbX--;

                        if (intNbY == 0)
                        {
                            Random r = new Random((int)(DateTime.Now.Ticks & 0xFFFF));
                            intOffsetY = r.Next(-1, 2);
                            intNbY = r.Next(1, 31);
                        }
                        else
                            intNbY--;

                        // On s'assure que l'élipse ne reste pas sur place
                        if ((intOffsetX == 0 && intOffsetY == 0) ||
                            (intOffsetPrecX == -intOffsetX && intOffsetPrecY == -intOffsetY))
                        {
                            intNbX = 0;
                            intNbY = 0;
                        }
                    } while (intNbX == 0 && intNbY == 0);

                    intOffsetPrecX = intOffsetX;
                    intOffsetPrecY = intOffsetY;

                    ptPoint = new Point(ptPoint.X + (intOffsetX * 5), ptPoint.Y + (intOffsetY * 5));

                    if (!dicPoints.ContainsKey(ptPoint.X))
                        dicPoints.Add(ptPoint.X, new List<int>());

                    dicPoints[ptPoint.X].Add(ptPoint.Y);

                    if (ptPoint.X < -intHeight || ptPoint.Y < -intWidth ||
                        ptPoint.X > objParent.Width || ptPoint.Y > objParent.Height)
                    {
                        // On arrête le timer si l'on est sorti des limites du contrôle
                        tmIntervalle.Stop();
                    }

                    // dimension variables of local scope
                    Graphics myGraphics = Graphics.FromHwnd(objParent.Handle);

                    // Draw rectangle from pen and rectangle objects
                    Rectangle rect = new Rectangle(ptPoint, new Size(intWidth, intHeight));
                    myGraphics.FillEllipse(new SolidBrush(cCouleur), rect);
                    myGraphics.DrawEllipse(new Pen(Brushes.Black), rect);
                }
                catch (Exception)
                {
                    tmIntervalle.Stop();
                }
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    tmIntervalle.Dispose();
                    dicPoints = null;
                }
            }
        }
    }
}
