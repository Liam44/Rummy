using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NoyauCommun.Cartes;
using NoyauCommun.Enumeres;
using NoyauCommun.Erreurs;
using Serveur;

namespace Client
{
    public partial class ZoneJoueur : UserControl
    {
        private Joueur objJoueur;
        private ePosition enumPosition;

        private Carte objCarteSource = null;

        private bool boolDragDropInterdit = true;
        private Color colorRectangle;

        public delegate void NbCartesChangedEvtHandler();
        public event NbCartesChangedEvtHandler NbCartesChanged;

        // Gestion des tours de jeu
        public delegate void TourTermineEvtHandler(bool aDeposeCartes);
        public event TourTermineEvtHandler TourTermine;
        public delegate void AMonTourEvtHandler(ZoneJoueur sender);
        public event AMonTourEvtHandler AMonTour;

        protected internal Joueur Joueur
        {
            get
            {
                return objJoueur;
            }
        }

        public string Nom
        {
            get
            {
                return objJoueur.Nom;
            }
        }

        public ePosition Position
        {
            get
            {
                return enumPosition;
            }
        }

        public int NbCartes
        {
            get
            {
                return objJoueur.NbCartes;
            }
        }

        public int PointsRestants
        {
            get
            {
                return objJoueur.PointsRestants;
            }
        }

        public int NbPoints
        {
            get
            {
                return objJoueur.NbPoints;
            }
        }

        public Carte CarteSource
        {
            set
            {
                objCarteSource = value;
            }
        }

        #region Gestion des événements

        #region    Gestion des événements liés au contrôle utilisateur

        private void ZoneJoueur_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Carte_DragDrop();
        }

        private void ZoneJoueur_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Carte_DragEnter(sender, e);
        }

        private void ZoneJoueur_DragLeave(object sender, System.EventArgs e)
        {
            if (!boolDragDropInterdit)
                Carte_DragLeave(sender, e);
        }

        private void ZoneJoueur_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (this.HScroll)
            {
                // Autoscroll de le zone du joueur lors d'un Drag&Drop
                Point pt = this.PointToClient(new Point(e.X, e.Y));
                int delta = this.Width - pt.X;
                if (delta < this.Width / 2 && delta > 0)
                    this.HorizontalScroll.Value = Math.Min(this.HorizontalScroll.Maximum, this.HorizontalScroll.Value + 3);

                if (delta > this.Width / 2 && delta < this.Width)
                    this.HorizontalScroll.Value = Math.Max(0, this.HorizontalScroll.Value - 3);
            }
        }

        private void ZoneJoueur_MouseUp(object sender, MouseEventArgs e)
        {
            Carte_MouseUp(sender, e);
        }

        private void ZoneJoueur_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DessinerContours();
        }

        #endregion

        #region    Gestion des événements liés aux cartes constituant la main du joueur

        private void Carte_Click(object sender, System.EventArgs e)
        {
            ((Carte)sender).Afficher(true);
            objCarteSource = null;
        }

        private void Carte_DragDrop(Carte carteDest = null)
        {
            try
            {
                // Par défaut, la carte est déplacée à la fin de la zone joueur
                int intPositionDest = -1;
                if (carteDest != null)
                {
                    // Sinon, la carte est déplacée à l'endroit de la carte destination
                    intPositionDest = carteDest.Position;

                    if (carteDest == objCarteSource)
                    {
                        Carte_Click(objCarteSource, new System.EventArgs());
                        return;
                    }
                }

                int intPositionSource = objCarteSource.Position;

                if (objCarteSource.ControleParentActuel == eControleOrigine.zoneJoueur)
                {
                    // La carte est déplacée au sein de la zone joueur
                    if (intPositionDest == intPositionSource)
                        intPositionDest++;

                    objJoueur.MainJoueur.Ajouter(objCarteSource, intPositionDest);
                }
                else
                {
                    Carte objCarte = Serveur.ZoneJeu.CarteJeuAcceptee(objCarteSource);
                    objCarteSource.ControleParentActuel = eControleOrigine.zoneJoueur;

                    if (objCarte == null)
                        // Une erreur est survenue lors de la suppression de la carte
                        return;

                    /* La carte provient de la zone de jeu : il faut donc ajouter le contrôle "Carte"
                                * et gérer les événements qui lui sont associés */
                    objJoueur.AjouterCarte(objCarteSource, intPositionDest);
                }

                Afficher();
            }
            catch (Exception)
            {
            }
            finally
            {
                objCarteSource = null;
            }
        }

        private void Carte_MouseDown(object sender, MouseEventArgs e)
        {
            objCarteSource = null;

            if (e.Button == MouseButtons.Left)
            {
                objCarteSource = (Carte)sender;
            }
        }

        private void Carte_MouseMove(object sender, MouseEventArgs e)
        {
            if (objCarteSource != null)
                objCarteSource.DoDragDrop(objCarteSource, DragDropEffects.Move);
        }

        private void Carte_MouseUp(object sender, MouseEventArgs e)
        {
            objCarteSource = null;
        }

        private void Carte_DragEnter(object sender, DragEventArgs e)
        {
            if (objCarteSource != null &&
                e.Data.GetDataPresent(typeof(Carte)) &&
                (e.AllowedEffect & DragDropEffects.Move) != 0)
                // Mouvement autorisé
                e.Effect = DragDropEffects.Move;
            else
                // Mouvement interdit
                e.Effect = DragDropEffects.None;
        }

        private void Carte_DragDrop(object sender, DragEventArgs e)
        {
            Carte_DragDrop((Carte)sender);
        }

        private void Carte_DragLeave(object sender, System.EventArgs e)
        {
            if (objCarteSource != null &&
                objCarteSource.ControleParentOrigine == eControleOrigine.zoneJoueur &&
                !boolDragDropInterdit)
                Rummy.ZoneJeu.CarteSource = objCarteSource;
        }

        #endregion

        #endregion

        #region Méthode d'initialisation de la zone du joueur

        public void Initialiser(Joueur joueur, ePosition position)
        {
            this.BackColor = Serveur.Rummy.Representation.CouleurFond;
            colorRectangle = this.BackColor;

            objJoueur = joueur;
            enumPosition = position;

            objJoueur.TourTermine += OnTourJeuTermine;
            objJoueur.AMonTour += OnAMonTour;

            objJoueur.MainJoueurChanged += OnMainJoueurChanged;

            Afficher();
        }

        public void Reinitialiser()
        {
            colorRectangle = this.BackColor;

            objJoueur.Reinitialiser();

            Afficher();
        }

        #endregion

        #region Interaction avec l'interface

        public void PartieTerminee()
        {
            Parent.Controls.Remove(this);
            this.Dispose();
        }

        public Color CouleurFond
        {
            set
            {
                this.BackColor = value;

                // Répercution sur les Cartes contenues sur la zone joueur
                foreach (Carte objCarte in this.Controls.OfType<Carte>())
                    objCarte.BackColor = value;
            }
        }

        private void OnTourJeuTermine(bool aDeposeCartes)
        {
            boolDragDropInterdit = true;
            colorRectangle = this.BackColor;
            DessinerContours();

            this.Enabled = false;

            if (TourTermine != null)
                TourTermine(aDeposeCartes);
        }

        private void OnAMonTour()
        {
            boolDragDropInterdit = false;
            int intAlpha = this.BackColor.A;
            int intRed = this.BackColor.R;
            int intGreen = this.BackColor.G;
            int intBlue = this.BackColor.B;
            colorRectangle = Color.FromArgb(intAlpha, 255 - intRed, 255 - intGreen, 255 - intBlue);
            if (colorRectangle == this.BackColor)
                // Ceci peut arriver si la couleur choisie pour le fond est gris (128, 128, 128)
                colorRectangle = Color.Black;

            DessinerContours();

            this.Enabled = true;

            if (AMonTour != null)
                AMonTour(this);
        }

        public void DeplacerCarteZoneJeu(Carte carte)
        {
            objCarteSource = carte;
        }

        public void CarteJoueurAcceptee(Carte carte = null)
        {
            if (carte == null)
                carte = objCarteSource;

            if (carte != null)
            {
                carte.CardClick -= Carte_Click;
                carte.CardMouseDown -= Carte_MouseDown;
                carte.CardMouseMove -= Carte_MouseMove;
                carte.CardMouseUp -= Carte_MouseUp;
                carte.CardDragDrop -= Carte_DragDrop;
                carte.CardDragEnter -= Carte_DragEnter;
                carte.CardDragLeave -= Carte_DragLeave;

                this.Controls.Remove(carte);

                objJoueur.MainJoueur.Supprimer(carte);
            }

            Afficher();
        }

        #endregion

        private void DessinerContours()
        {
            // dimension variables of local scope
            Graphics myGraphics = Graphics.FromHwnd(this.Handle);

            // draw rectangle from pen and rectangle objects
            myGraphics.DrawRectangle(new Pen(colorRectangle), new Rectangle(0, 0, this.Width - 1, this.Height - 1));
        }

        protected internal void ActualiserAffichage()
        {
            DessinerContours();

            int intValue = 0;
            if (this.HScroll)
            {
                intValue = this.HorizontalScroll.Value;
                this.HorizontalScroll.Value = 0;
            }

            int x = 1;
            int top = 1;

            foreach (Carte objCarte in this.Controls)
            {
                objCarte.Desurligner();
                objCarte.Left = x;
                objCarte.Top = top;
                x += objCarte.Width;
                if (x >= this.Width - objCarte.Width - 2)
                {
                    // Passage à la ligne suivante
                    x = 1;
                    top += objCarte.Height;
                }
            }

            this.HorizontalScroll.Value = Math.Min(0, intValue);
        }

        private void Afficher()
        {
            // Réinitialisation de la zone du joueur
            foreach (Carte objCarte in this.Controls.OfType<Carte>().ToList())
            {
                objCarte.CardClick -= Carte_Click;
                objCarte.CardMouseDown -= Carte_MouseDown;
                objCarte.CardMouseMove -= Carte_MouseMove;
                objCarte.CardMouseUp -= Carte_MouseUp;
                objCarte.CardDragDrop -= Carte_DragDrop;
                objCarte.CardDragEnter -= Carte_DragEnter;
                objCarte.CardDragLeave -= Carte_DragLeave;

                this.Controls.Remove(objCarte);
            }

            foreach (Carte objCarte in objJoueur.MainJoueur.CartesCombinaison)
            {
                this.Controls.Add(objCarte);
                objCarte.Afficher(objJoueur.CartesVisibles);
                objCarte.CardClick += Carte_Click;
                objCarte.CardMouseDown += Carte_MouseDown;
                objCarte.CardMouseMove += Carte_MouseMove;
                objCarte.CardMouseUp += Carte_MouseUp;
                objCarte.CardDragDrop += Carte_DragDrop;
                objCarte.CardDragEnter += Carte_DragEnter;
                objCarte.CardDragLeave += Carte_DragLeave;
            }

            ActualiserAffichage();
            if (NbCartesChanged != null)
                NbCartesChanged();
        }

        private void OnMainJoueurChanged()
        {
            Afficher();
        }

        public ZoneJoueur()
        {
            InitializeComponent();
        }
    }
}
