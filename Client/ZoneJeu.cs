using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NoyauCommun;
using NoyauCommun.Cartes;
using NoyauCommun.Configuration;
using NoyauCommun.Enumeres;
using NoyauCommun.Erreurs;
using Serveur;

namespace Client
{
    public partial class ZoneJeu : UserControl
    {
        private Panel pnSource;
        private Carte objCarteSource = null;
        private Carte objCarteDivision = null;
        private bool boolBoutonEnfonce = false;

        // Zone du joueur rattaché au client
        private ZoneJoueur objZoneJoueur;

        private Panel pnPioche = null;

        private int intNbPanels = 0;  // Utilisé lors de l'affectation du nom du nouveau Panel
        private Dictionary<Combinaison, Panel> dicPanels = new Dictionary<Combinaison, Panel>();

        // Contraintes liées au jeu
        private bool boolADeposePointsMinimum;

        #region Evénements

        public delegate void ActualiserNbCartesPiocheEvtHandler(int nbCartes);
        public event ActualiserNbCartesPiocheEvtHandler ActualiserNbCartesPioche;

        public delegate void PartieTermineeEvtHandler();
        public event PartieTermineeEvtHandler PartieTerminee;

        #endregion

        private void Tracer()
        {
            string str = "La zone de jeu contient :";

            foreach (Combinaison objCombinaison in dicPanels.Keys)
            {
                str += Environment.NewLine + objCombinaison.ToString();
            }

            GestionErreurs.Tracer(str, true);
        }

        /// <summary>
        /// Indique la carte présentement déplacée depuis la zone du joueur vers la zone de jeu
        /// </summary>
        protected internal Carte CarteSource
        {
            set
            {
                objCarteSource = value;
            }
        }

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="zoneJoueur">Zone du joueur associé au client.
        /// Est utilisée dans la gestion du drag&drop des cartes de la main du joueur vers la zone de jeu
        /// et réciproquement.</param>
        public ZoneJeu(ZoneJoueur zoneJoueur)
        {
            InitializeComponent();

            objZoneJoueur = zoneJoueur;

            this.BackColor = Configuration.CouleurFond;
            CreerPioche();
            Serveur.ZoneJeu.CombinaisonsChanged += OnCombinaisonsModifiees;
            Serveur.ZoneJeu.DoitDeposerCartes += OnDoitDeposerCartes;
            Serveur.ZoneJeu.PiocheInterdite += OnPiocheInterdite;
            Serveur.ZoneJeu.CombinaisonInvalide += OnCombinaisonInvalide;
            Serveur.ZoneJeu.PasAssezPoints += OnPointsInsuffisants;
            Serveur.ZoneJeu.JoueurSuivant += OnJoueurSuivant;
            Serveur.ZoneJeu.PartieTerminee += OnPartieTerminee;
        }

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            Serveur.ZoneJeu.CombinaisonsChanged -= OnCombinaisonsModifiees;
            Serveur.ZoneJeu.DoitDeposerCartes -= OnDoitDeposerCartes;
            Serveur.ZoneJeu.PiocheInterdite -= OnPiocheInterdite;
            Serveur.ZoneJeu.CombinaisonInvalide -= OnCombinaisonInvalide;
            Serveur.ZoneJeu.PasAssezPoints -= OnPointsInsuffisants;
            Serveur.ZoneJeu.JoueurSuivant -= OnJoueurSuivant;
            Serveur.ZoneJeu.PartieTerminee -= OnPartieTerminee;

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Indique si le joueur a déjà passé un tour en posant des cartes sur la table
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        protected internal bool ADeposePointsMinimum
        {
            set
            {
                boolADeposePointsMinimum = value;
            }
        }

        /// <summary>
        /// Renvoie la liste des combinaisons actuellement déposées sur la zone de jeu
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        protected internal List<Combinaison> Combinaisons
        {
            get
            {
                // return dicPanels.Keys.ToList();

                List<Combinaison> lstRes = new List<Combinaison>();

                foreach (Combinaison objCombinaison in dicPanels.Keys)
                {
                    Combinaison objClone = objCombinaison.Clone();
                    lstRes.Add(objClone);
                }

                return lstRes;
            }
        }

        #region Affectation des textes des éléments du menu contextuel

        protected internal void AffecterTextesMenuContextuel(List<string> textes)
        {
            MCDiviserCombinaison.Text = textes.First();
            MCD_Avant.Text = textes[1];
            MCD_Apres.Text = textes.Last();

        }

        #endregion

        #region Gestion des événements

        /// <summary>
        /// Assure la gestion du drag&drop d'une carte vers un objet de la zone de jeu. Cela peut être :
        /// - une carte (la carte déplacée est alors insérée avant la carte de destination)
        /// - un panel (la carte est alors déplacéee en fin de la combinaison associée au panel)
        /// - la zone de jeu elle-même (la carte va alors constituer une nouvelle combinaison)
        /// </summary>
        /// <param name="objetDest">Objet sur laquelle la carte en cours de déplacement est déposée</param>
        private void Carte_DragDrop(object objetDest)
        {
            if (objCarteSource == null)
                return;

            if (objetDest == this)
                objetDest = null;
            else if (objetDest is Panel)
                objetDest = (Combinaison)(((Panel)objetDest).Tag);

            if (objCarteSource.ControleParentActuel == eControleOrigine.zoneJoueur)
                // La carte provient de la zone joueur et est déplacée vers la zone de jeu
                Serveur.ZoneJeu.DeposerCarte(ref objCarteSource, objetDest);
            else
                // La carte est déplacée au sein de la zone de jeu
                Serveur.ZoneJeu.DeplacerCarte(objCarteSource, objetDest);

            objCarteSource = null;
            pnSource = null;
            boolBoutonEnfonce = false;
        }

        #region    Evénements sur Carte

        /// <summary>
        /// La seule carte possédant l'événement "Click" est la carte représentant la pioche
        /// sur la zone de jeu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Carte_Click(object sender, System.EventArgs e)
        {

            // Le joueur ne peut piocher de carte que s'il n'en a déposé aucune sur la zone de jeu
            Serveur.ZoneJeu.Piocher(false);
        }

        private void Carte_DragEnter(object sender, DragEventArgs e)
        {
            // Vérifie qu'il s'agit bien d'un déplacement de carte
            if (objCarteSource != null &&
               e.Data.GetDataPresent(typeof(Carte)) &&
               (e.AllowedEffect & DragDropEffects.Move) != 0)
            {
                if (!boolADeposePointsMinimum &&
                   sender is Carte &&
                   !((Carte)sender).Combinaison.PossedeCarteJoueur() &&
                   !Serveur.ZoneJeu.PointsDeposesSuffisants())
                    /* Au premier tour de jeu, le joueur ne peut pas compléter de combinaisons qu'il n'a pas lui-même créée
                     * A moins d'avoir déjà déposé les 30 points requis */

                    // Movement interdit
                    e.Effect = DragDropEffects.None;
                else
                {
                    // Mise en avant de la carte destination
                    if (sender is Carte)
                        ((Carte)sender).Surligner();

                    // Movement autorisé
                    e.Effect = DragDropEffects.Move;
                }
            }
            else
                // Movement interdit
                e.Effect = DragDropEffects.None;
        }

        private void Carte_DragDrop(object sender, DragEventArgs e)
        {
            Carte_DragDrop(sender);
        }

        private void Carte_DragLeave(object sender, System.EventArgs e)
        {
            ((Carte)sender).Desurligner();

        }

        private void Carte_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        objCarteSource = (Carte)sender;
                        if (objCarteSource.Parent is Panel)
                            pnSource = (Panel)objCarteSource.Parent;
                        else
                            pnSource = null;

                        boolBoutonEnfonce = true;

                        break;
                    }
                case MouseButtons.Right:
                    {
                        objCarteDivision = (Carte)sender;

                        Size d = Serveur.Rummy.Representation.Dimensions;
                        MenuContextuel.Show(objCarteDivision, d.Width / 2, d.Height / 2);

                        break;
                    }
            }
        }

        private void Carte_MouseMove(object sender, MouseEventArgs e)
        {
            if (objCarteSource == null || !boolBoutonEnfonce)
                return;
            else if (objCarteSource.Combinaison == null ||
                     !(boolADeposePointsMinimum ||
                       objCarteSource.Combinaison.PossedeCarteJoueur() ||
                       Serveur.ZoneJeu.PointsDeposesSuffisants()))
                /* Au premier tour de jeu, le joueur ne peut pas déplacer de carte qu'il n'a pas lui-même déposée
                 * A moins d'avoir déjà déposé les 30 points requis */
                return;
            else if (objCarteSource.Valeur == eValeur.Joker &&
                   objCarteSource.ControleParentOrigine == eControleOrigine.zoneJoueur &&
                   objCarteSource.ControleParentOrigine == objCarteSource.ControleParentActuel)
                /* Dans le cas de tentative de déplacement d'un joker, cela n'est possible que si :
                 * - la carte provient de la main du joueur
                 * - la carte provient d'un autre Panel que celui auquel elle est actuellement rattachée
                 * Dans les autres cas, la seule façon de déplacer un joker est de le remplacer par une carte équivalente */
                return;
            else if (!(objCarteSource.ControleParentOrigine == eControleOrigine.zoneJoueur) &&
                   objCarteSource.ControleParentOrigine == objCarteSource.ControleParentActuel &&
                   objCarteSource.Combinaison.PossedeJokerDejaDepose() &&
                   objCarteSource.Combinaison.NbCartes == 3)
                /* Une carte inclue dans une combinaison possédant un joker déjà déposé à un tour précédent
                 * ne peut être déplacée que s'il reste au moins encore 3 cartes dans la combinaison */
                return;

            objCarteSource.DoDragDrop(objCarteSource, DragDropEffects.Move);
            boolBoutonEnfonce = false;
        }

        private void Carte_MouseUp(object sender, MouseEventArgs e)
        {
            objCarteSource = null;
            pnSource = null;
            boolBoutonEnfonce = false;
        }

        #endregion

        #region    Evénements sur Panel

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            Carte_DragDrop(sender);
        }

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (!(boolADeposePointsMinimum ||
                ((Combinaison)((Panel)sender).Tag).PossedeCarteJoueur() ||
                Serveur.ZoneJeu.PointsDeposesSuffisants()))
                /* Au premier tour de jeu, le joueur ne peut pas déplacer de carte qu'il n'a pas lui-même déposée
                 * A moins d'avoir déjà déposé les 30 points requis */
                return;

            // Mise en évidence de la suite de cartes impactée
            foreach (Carte objCarte in ((Panel)sender).Controls)
                objCarte.Surligner();

            Carte_DragEnter(sender, e);
        }

        private void Panel_DragLeave(object sender, System.EventArgs e)
        {
            // Mise en évidence de la suite de cartes impactée
            foreach (Carte objCarte in ((Panel)sender).Controls)
                objCarte.Desurligner();
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            ZoneJeu_MouseDown(sender, e);
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            Carte_MouseUp(sender, e);
        }

        private void Panel_Resize(object sender, System.EventArgs e)
        {
            AfficherCartes();
        }

        #endregion

        #region    Evénements sur Zone de jeu

        private void ZoneJeu_DragDrop(object sender, DragEventArgs e)
        {
            Carte_DragDrop(sender);
        }

        private void ZoneJeu_DragEnter(object sender, DragEventArgs e)
        {
            Carte_DragEnter(sender, e);
        }

        private void ZoneJeu_DragLeave(object sender, System.EventArgs e)
        {
            if (objCarteSource != null &&
                objCarteSource.ControleParentOrigine == eControleOrigine.zoneJoueur)
                objZoneJoueur.DeplacerCarteZoneJeu(objCarteSource);
        }

        private void ZoneJeu_DragOver(object sender, DragEventArgs e)
        {
            if (this.HScroll)
            {
                // Autoscroll de le zone de jeu lors d'un Drag&Drop
                Point pt = this.PointToClient(new Point(e.X, e.Y));
                int delta = this.Width - pt.X;
                if (delta < this.Width / 2 && delta > 0)
                    this.HorizontalScroll.Value = Math.Min(this.HorizontalScroll.Maximum, this.HorizontalScroll.Value + 3);

                if (delta > this.Width / 2 && delta < this.Width)
                    this.HorizontalScroll.Value = Math.Max(0, this.HorizontalScroll.Value - 3);
            }
        }

        private void ZoneJeu_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                    // On acquitte la fin du tour par un clic droit sur la zone de jeu
                    Serveur.ZoneJeu.TerminerTourJeu(false);
            }
            catch
            {
                // Répercution sur la méthode appelante
                throw;
            }

        }

        private void ZoneJeu_MouseUp(object sender, MouseEventArgs e)
        {
            Carte_MouseUp(sender, e);
        }

        private void ZoneJeu_Resize(object sender, System.EventArgs e)
        {
            AfficherPioche();
            AfficherCartes();
        }

        #region    Menu contextuel

        private void MCDiviserCombinaison_MouseHover(object sender, System.EventArgs e)
        {
            if (objCarteDivision != null)
                if (objCarteDivision.Position == 0)
                {
                    MCD_Avant.Enabled = false;
                    MCD_Apres.Enabled = true;
                }
                else if (objCarteDivision.Position == objCarteDivision.Combinaison.NbCartes - 1)
                {
                    MCD_Avant.Enabled = true;
                    MCD_Apres.Enabled = false;
                }
                else
                {
                    MCD_Avant.Enabled = true;
                    MCD_Apres.Enabled = true;
                }
        }

        private void MCD_Apres_Click(object sender, System.EventArgs e)
        {
            if (objCarteDivision != null)
                Serveur.ZoneJeu.DiviserCombinaison(objCarteDivision, false);

            objCarteDivision = null;
        }

        private void MCD_Avant_Click(object sender, System.EventArgs e)
        {
            if (objCarteDivision != null)
                Serveur.ZoneJeu.DiviserCombinaison(objCarteDivision, true);

            objCarteDivision = null;
        }

        #endregion

        #endregion

        #endregion

        #region "Dialogue avec la classe ZoneJeu"

        protected internal Color CouleurFond
        {
            set
            {
                this.BackColor = value;

                // Répercution sur les Panels et les Cartes qu'ils contiennent
                foreach (Panel pnCartes in this.Controls.OfType<Panel>())
                {
                    pnCartes.BackColor = value;

                    foreach (Carte objCarte in pnCartes.Controls)
                        objCarte.BackColor = value;
                }
            }
        }

        #endregion

        #region Gestion de la pioche

        private void CreerPioche()
        {
            pnPioche = new Panel();
            pnPioche.Size = Serveur.Rummy.Representation.Dimensions;
            this.Controls.Add(pnPioche);
            Carte objCarte = Serveur.Rummy.Carte(eValeur.Indetermine, eCouleur.Indetermine);
            objCarte.CardClick += Carte_Click;
            pnPioche.Controls.Add(objCarte);
            objCarte.Top = 0;
            objCarte.Left = 0;
            objCarte.Tag = pnPioche;

            AfficherPioche();

            Serveur.ZoneJeu.CartePiocheeEvtHandler += LancerEvtActualiserNbCartesPioche;

            LancerEvtActualiserNbCartesPioche();
        }

        private void AfficherPioche(int offset = 0)
        {
            if (pnPioche != null)
            {
                pnPioche.Top = (this.ClientSize.Height - pnPioche.Height) / 2;
                pnPioche.Left = Math.Max(this.ClientSize.Width - pnPioche.Width, offset);
            }
        }

        #endregion

        #region Gestion de l'historique

        protected internal bool ChargerContexte()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // Evite de faire "flasher" la zone de jeu
                this.Visible = false;

                // Suppression des contrôles graphiques
                foreach (Combinaison objCombinaison in dicPanels.Keys.ToList())
                    SupprimerCartes(objCombinaison);

                dicPanels.Clear();
                intNbPanels = 0;

                foreach (Combinaison objCombinaison in Serveur.ZoneJeu.Combinaisons)
                {
                    Panel pnCartes = null;
                    foreach (Carte objCarte in objCombinaison.CartesCombinaison)
                    {
                        objCarte.Afficher(true);

                        if (objCarte.VientDeZoneJoueur)
                            objCarte.Surligner(Color.DarkTurquoise);

                        if (pnCartes == null)
                            pnCartes = NouvelleCombinaison(objCarte);
                        else
                            AjouterCarte(objCarte, pnCartes);
                    }
                }

                AfficherCartes();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                // Renvoi à la méthode appelante
                throw ex;
            }
            finally
            {
                Cursor.Current = Cursors.Default;

                // Restitution de l'attribut de visibilité
                this.Visible = true;
            }
        }

        #endregion

        #region Interception des événements lancés par la classe "Rummy" du serveur

        private void OnCombinaisonsModifiees(bool historiser)
        {
            ChargerContexte();
        }

        private void OnJoueurSuivant()
        {
            Joueur objJoueurCourant = Serveur.Rummy.JoueurCourant;
            ADeposePointsMinimum = objJoueurCourant.ADeposePointsMinimum;
            this.Enabled = objZoneJoueur.Joueur == objJoueurCourant;
        }

        private void OnPartieTerminee()
        {
            if (PartieTerminee != null)
                PartieTerminee();
            else
                MessageBox.Show("Partie terminée non défine !");
        }

        private void OnPiocheInterdite()
        {
            MessageBox.Show(Configuration.ObtenirTexte("PiocheInterdite"),
                            "Tour de jeu invalide",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void OnDoitDeposerCartes()
        {
            MessageBox.Show(Configuration.ObtenirTexte("DoitDeposerCartes").Replace("\\n", Environment.NewLine),
                            "Tour de jeu invalide",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void OnCombinaisonInvalide()
        {
            MessageBox.Show(Configuration.ObtenirTexte("CombinaisonInvalide").Replace("\\n", Environment.NewLine),
                            "Tour de jeu invalide",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void OnPointsInsuffisants()
        {
            MessageBox.Show(Configuration.ObtenirTexte("PasAssezDePoints").Replace("%",
                                                                                   Serveur.Rummy.NB_POINTS_MINIMUM.ToString()),
                            "Tour de jeu invalide",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        #endregion

        public void AfficherCartes()
        {
            int intTop = 0;
            int intIndice = 0;
            int intLeft = 0;
            int intLargeurColonnePrec = 0;

            /* On s'assure que si la barre de défilement horizontal est visible,
             * la position de celle-ci soit bien calée à gauche */
            int intHScroll = 0;
            if (this.HScroll)
            {
                intHScroll = this.HorizontalScroll.Value;
                this.HorizontalScroll.Value = 0;
            }

            // Positionnement du panel sur le contrôle, en fonction de la place restante
            foreach (Panel pnCarte in dicPanels.Values)
            {
                // La hauteur restante n'est pas suffisante pour afficher la carte
                if (this.ClientSize.Height - intTop - pnCarte.Height < 0)
                {
                    // On donc passe à la colonne suivante
                    intTop = 0;
                    intLeft += intLargeurColonnePrec;
                    intLargeurColonnePrec = 0;
                }

                pnCarte.Top = intTop;
                pnCarte.Left = intLeft;

                if (pnCarte.Width > intLargeurColonnePrec)
                    intLargeurColonnePrec = pnCarte.Width;

                intTop += pnCarte.Height;
                intIndice += 1;

                Afficher(pnCarte);
            }

            // Restitution de la valeur précédente
            if (this.HScroll)
                this.HorizontalScroll.Value = Math.Min(intHScroll, this.HorizontalScroll.Maximum);

            AfficherPioche(intLeft + intLargeurColonnePrec);
        }

        private void Afficher(Panel panel)
        {
            int x = 0;

            foreach (Carte objCarte in ((Combinaison)(panel.Tag)).CartesCombinaison)
            {
                objCarte.Left = x;
                objCarte.Top = 0;
                objCarte.Desurligner();
                x += objCarte.Width;
            }

            panel.Width = x + Serveur.Rummy.Representation.Dimensions.Width;

        }

        private Panel NouvelleCombinaison(Carte carte)
        {
            try
            {
                Panel pnCarte = new Panel();
                pnCarte.Name = intNbPanels.ToString();
                pnCarte.Size = new Size(2 * carte.Width, carte.Height);
                pnCarte.DragEnter += Panel_DragEnter;
                pnCarte.DragDrop += Panel_DragDrop;
                pnCarte.DragLeave += Panel_DragLeave;
                pnCarte.MouseDown += Panel_MouseDown;
                pnCarte.MouseUp += Panel_MouseUp;
                pnCarte.Resize += Panel_Resize;
                pnCarte.Tag = carte.Combinaison;
                pnCarte.BackColor = this.BackColor;
                pnCarte.AllowDrop = true;

                carte.Top = 0;
                carte.Left = 0;

                this.Controls.Add(pnCarte);

                AjouterCarte(carte, pnCarte, redimensionnerPanel: false);
                dicPanels.Add(carte.Combinaison, pnCarte);
                intNbPanels += 1;

                return pnCarte;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return new Panel();
            }
        }

        private bool AjouterCarte(Carte carte,
                                  Panel pnDest,
                                  int position = -1,
                                  bool redimensionnerPanel = true,
                                  bool remplacerCarte = false)
        {
            try
            {
                carte.Afficher(true);

                // Affectation d'un nouveau contrôle parent
                pnDest.Controls.Add(carte);
                carte.ControleParentActuel = eControleOrigine.zoneJeu;
                carte.Tag = pnDest;

                carte.CardDragEnter += Carte_DragEnter;
                carte.CardDragDrop += Carte_DragDrop;
                carte.CardDragLeave += Carte_DragLeave;
                carte.CardMouseDown += Carte_MouseDown;
                carte.CardMouseMove += Carte_MouseMove;
                carte.CardMouseUp += Carte_MouseUp;

                if (redimensionnerPanel)
                    // Elargissement du Panel, si nécessaire
                    pnDest.Width += carte.Width;

                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        private bool SupprimerCartes(Combinaison combinaison)
        {
            try
            {
                Panel pnCartes = dicPanels[combinaison];

                if (pnCartes == null)
                    return false;

                while (pnCartes.Controls.Count > 0)
                {
                    Carte objCarte = pnCartes.Controls.OfType<Carte>().First();

                    // Suppression de la relation avec le contrôle parent
                    objCarte.Tag = null;

                    objCarte.CardDragEnter -= Carte_DragEnter;
                    objCarte.CardDragDrop -= Carte_DragDrop;
                    objCarte.CardDragLeave -= Carte_DragLeave;
                    objCarte.CardMouseDown -= Carte_MouseDown;
                    objCarte.CardMouseMove -= Carte_MouseMove;
                    objCarte.CardMouseUp -= Carte_MouseUp;

                    pnCartes.Controls.Remove(objCarte);
                }

                pnCartes.DragEnter -= Panel_DragEnter;
                pnCartes.DragDrop -= Panel_DragDrop;
                pnCartes.DragLeave -= Panel_DragLeave;
                pnCartes.MouseDown -= Panel_MouseDown;
                pnCartes.MouseUp -= Panel_MouseUp;
                pnCartes.Resize -= Panel_Resize;

                this.Controls.Remove(pnCartes);

                dicPanels.Remove(combinaison);

                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        public void LancerEvtActualiserNbCartesPioche()
        {
            if (ActualiserNbCartesPioche != null)
                ActualiserNbCartesPioche(Serveur.Rummy.Pioche.NbCartes);
        }
    }
}
