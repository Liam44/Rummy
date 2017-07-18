using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

using Client.MessageAttente;

using NoyauCommun.Cartes;
using NoyauCommun.Configuration;
using NoyauCommun.Enumeres;
using NoyauCommun.Erreurs;
using Serveur;

namespace Client
{
    public partial class Rummy : Form
    {
        private ToolStripStatusLabel tsslPioche = null;

        // Gestion du temps de jeu
        private int intNbSecondesRestant = 0;
        private ToolStripStatusLabel tsslTempsRestant = null;

        private Dictionary<Joueur, ZoneJoueur> dicZoneJoueurs = new Dictionary<Joueur, ZoneJoueur>();
        private Dictionary<ZoneJoueur, ToolStripStatusLabel> dicTSSLZoneJoueurs = new Dictionary<ZoneJoueur, ToolStripStatusLabel>();
        private ZoneJoueur objZoneJoueurCourant = null;
        private static ZoneJeu objZoneJeu = null;
        private static bool boolPartiePeutCommencer = false;

        private ScreenSaver.ScreenSaver objScreenSaver;

        protected internal static ZoneJeu ZoneJeu
        {
            get
            {
                return objZoneJeu;
            }
        }

        #region Gestion des événements sur le formulaire

        private void Rummy_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            // Mémorisation de l'état de la fenêtre à la fermeture
            Configuration.ParametresInterface.Etat = this.WindowState;
            Configuration.ParametresInterface.Dimensions = this.Size;
            Configuration.ParametresInterface.Location = this.Location;

            Configuration.Enregistrer();
        }

        private void Rummy_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                Rummy_Resize(this, new System.EventArgs());
        }

        private void Rummy_Load(object sender, System.EventArgs e)
        {
            Configuration.Lire();
            Configuration.LangueChangee += AppliquerLangue;
            Configuration.CouleurFondChangee += AppliquerCouleurFond;

            this.BackColor = Configuration.CouleurFond;

            InitialiserJeu();

            AppliquerCouleurFond();

            // Restitution de l'état de la fenêtre tel qu'enregistré lors de la dernière fermeture
            FormatInterface objFI = Configuration.ParametresInterface;

            if (objFI.Dimensions.Equals(new System.Drawing.Size(0, 0)))
                this.WindowState = FormWindowState.Maximized;
            else
            {
                this.WindowState = objFI.Etat;
                if (objFI.Etat == FormWindowState.Normal)
                {
                    this.Size = objFI.Dimensions;
                    this.Location = objFI.Location;
                }
            }
        }

        private void Rummy_Resize(object sender, System.EventArgs e)
        {
            if (objZoneJeu != null)
            {
                int intOffset = 2 * (Serveur.Rummy.Representation.Dimensions.Height + 1);

                objZoneJeu.Left = intOffset;
                objZoneJeu.Top = intOffset + MenuStrip1.Height;

                objZoneJeu.Width = this.ClientSize.Width - 2 * intOffset;
                objZoneJeu.Height = this.ClientSize.Height - MenuStrip1.Height - StatusStrip1.Height - 2 * intOffset;
            }

            if (dicZoneJoueurs != null)
                foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
                {
                    switch (objZoneJoueur.Position)
                    {
                        case ePosition.Bas:
                            {
                                int height = 2 * (Serveur.Rummy.Representation.Dimensions.Height + 1);
                                int width = this.ClientSize.Width - 2 * height;

                                objZoneJoueur.Size = new Size(width, height);
                                objZoneJoueur.Top = this.ClientSize.Height - StatusStrip1.Height - height;
                                objZoneJoueur.Left = (this.ClientSize.Width - width) / 2;

                                break;
                            }
                        case ePosition.Haut:
                            {
                                int height = 2 * (Serveur.Rummy.Representation.Dimensions.Height + 1);
                                int width = this.ClientSize.Width - 2 * height;

                                objZoneJoueur.Size = new Size(width, height);
                                objZoneJoueur.Top = MenuStrip1.Height;
                                objZoneJoueur.Left = (this.ClientSize.Width - width) / 2;

                                break;
                            }
                        case ePosition.Gauche:
                            {
                                int width = 2 * (Serveur.Rummy.Representation.Dimensions.Height + 1);
                                int height = this.ClientSize.Height - MenuStrip1.Height - StatusStrip1.Height - 2 * width;

                                objZoneJoueur.Size = new Size(width, height);
                                objZoneJoueur.Top = width + MenuStrip1.Height;
                                objZoneJoueur.Left = 0;

                                break;
                            }
                        case ePosition.Droite:
                            {
                                int width = 2 * (Serveur.Rummy.Representation.Dimensions.Height + 1);
                                int height = this.ClientSize.Height - MenuStrip1.Height - StatusStrip1.Height - 2 * width;

                                objZoneJoueur.Size = new Size(width, height);
                                objZoneJoueur.Top = width + MenuStrip1.Height;
                                objZoneJoueur.Left = this.ClientSize.Width - width;

                                break;
                            }
                    }

                    objZoneJoueur.ActualiserAffichage();
                }
        }

        #endregion

        private void InitialiserJeu(bool reinitialiser = true)
        {
            Cursor.Current = Cursors.WaitCursor;

            FermerScreenSaver();

            Serveur.Rummy.NouvellePartie(reinitialiser);
            ReactualiserMenusAction();

            if (reinitialiser)
            {
                dicZoneJoueurs = new Dictionary<Joueur, ZoneJoueur>();
                dicTSSLZoneJoueurs = new Dictionary<ZoneJoueur, ToolStripStatusLabel>();

                List<ePosition> lstPositions;

                if (Configuration.Joueurs.Count == 2)
                    lstPositions = new List<ePosition> { ePosition.Bas, ePosition.Haut };
                else
                {
                    lstPositions = new List<ePosition>();
                    foreach (ePosition enumPosition in Enum.GetValues(typeof(ePosition)))
                        lstPositions.Add(enumPosition);
                }

                foreach (Joueur objJoueurTmp in Serveur.Rummy.Joueurs)
                    AjouterZoneJoueur(objJoueurTmp, lstPositions[dicZoneJoueurs.Count]);

                Serveur.Historique.HistoriqueChanged += OnHistorisation;

                tsslPioche = new ToolStripStatusLabel();
                tsslPioche.Spring = true;
                StatusStrip1.Items.Add(tsslPioche);
            }
            else
                foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
                {
                    objZoneJoueur.Reinitialiser();
                    ActualiserNbCartes(objZoneJoueur);
                }

            // On attend que tous les joueurs humains se soient connectés au serveur
            boolPartiePeutCommencer = false;

            Joueur objJoueur = Serveur.Rummy.ObtenirJoueur();

            if (objJoueur != null)
            {
                objZoneJeu = new ZoneJeu(ZoneJoueur(objJoueur));
                this.Controls.Add(objZoneJeu);

                objZoneJeu.ActualiserNbCartesPioche += ZoneJeu_ActualiserNbCartesPioche;
                objZoneJeu.PartieTerminee += ZoneJeu_PartieTerminee;

                objZoneJoueurCourant = dicZoneJoueurs[Serveur.Rummy.JoueurCourant];
                objZoneJeu.ADeposePointsMinimum = objZoneJoueurCourant.Joueur.ADeposePointsMinimum;

                Rummy_Resize(this, new System.EventArgs());

                objZoneJoueurCourant.Joueur.ATonTour();

                MO_Scores.Enabled = true;
                MJ_Rafraichir.Enabled = true;
                MenuActions.Enabled = true;

                AppliquerLangue();
            }

            if (Configuration.TempsJeuLimite)
            {
                intNbSecondesRestant = Configuration.NbSecondesReflexion;

                if (tsslTempsRestant == null)
                {
                    tsslTempsRestant = new ToolStripStatusLabel();
                    tsslTempsRestant.Spring = true;
                    StatusStrip1.Items.Add(tsslTempsRestant);
                }
            }

            MessageAttente.MessageAttente objMessage = new MessageAttente.MessageAttente(Configuration.ObtenirTexte("EnAttenteTexte"),
                                                                                         Configuration.ObtenirTexte("EnAttenteTitre"),
                                                                                         true);

            while (!boolPartiePeutCommencer)
            {
                if (objMessage.Controller.Canceled)
                    break;
                Application.DoEvents();
            }

            objMessage.Fermer();

            Cursor.Current = Cursors.Default;
        }

        private void AjouterZoneJoueur(Joueur joueur, ePosition position)
        {
            ToolStripStatusLabel tssl = new ToolStripStatusLabel();
            tssl.Spring = true;
            StatusStrip1.Items.Add(tssl);

            ZoneJoueur objZoneJoueur = new ZoneJoueur();
            this.Controls.Add(objZoneJoueur);

            objZoneJoueur.Initialiser(joueur, position);
            objZoneJoueur.AMonTour += ZoneJoueur_AMonTour;
            objZoneJoueur.NbCartesChanged += ZoneJoueur_NbCartesChanged;

            dicZoneJoueurs.Add(joueur, objZoneJoueur);
            dicTSSLZoneJoueurs.Add(objZoneJoueur, tssl);
        }

        private ZoneJoueur ZoneJoueur(Joueur joueur)
        {
            ZoneJoueur objRes = null;

            foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
            {
                if (dicZoneJoueurs[joueur] == objZoneJoueur)
                {
                    objRes = objZoneJoueur;
                    break;
                }
            }

            return objRes;
        }

        private void FinirPartie(bool rejouer)
        {
            ViderInterface();

            objScreenSaver = new ScreenSaver.ScreenSaver(this.BackColor, this.ClientSize);
            this.Controls.Add(objScreenSaver);
            this.Activate();

            if (rejouer)
                InitialiserJeu(false);
        }

        private void FermerScreenSaver()
        {
            if (objScreenSaver != null)
            {
                this.Controls.Remove(objScreenSaver);
                objScreenSaver.Dispose();
            }
        }

        private void ViderInterface()
        {
            if (dicZoneJoueurs != null)
            {
                foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
                {
                    objZoneJoueur.PartieTerminee();
                    objZoneJoueur.NbCartesChanged -= ZoneJoueur_NbCartesChanged;
                    this.Controls.Remove(objZoneJoueur);
                }

                dicZoneJoueurs.Clear();
                dicZoneJoueurs = null;
            }

            if (dicTSSLZoneJoueurs != null)
            {
                dicTSSLZoneJoueurs.Clear();
                dicTSSLZoneJoueurs = null;
            }

            if (objZoneJeu != null)
            {
                this.Controls.Remove(objZoneJeu);

                objZoneJeu.ActualiserNbCartesPioche -= ZoneJeu_ActualiserNbCartesPioche;
                objZoneJeu.PartieTerminee -= ZoneJeu_PartieTerminee;
                objZoneJeu.Dispose();
            }

            StatusStrip1.Items.Clear();
            tsslPioche = null;
            tsslTempsRestant = null;

            Serveur.Historique.HistoriqueChanged -= OnHistorisation;

            MO_Scores.Enabled = false;
            MJ_Rafraichir.Enabled = false;
            MenuActions.Enabled = false;
        }

        #region Action sur la barre de menus

        private void OnHistorisation()
        {
            ReactualiserMenusAction();
        }

        private void ReactualiserMenusAction()
        {
            try
            {
                MA_Annuler.Enabled = Historique.PeutAnnuler;
                MA_AnnulerTout.Enabled = MA_Annuler.Enabled;

                MA_Refaire.Enabled = Historique.PeutRefaire;
                MA_RefaireTout.Enabled = MA_Refaire.Enabled;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        private void MA_Annuler_Click(object sender, System.EventArgs e)
        {
            Serveur.Rummy.AnnulerAction();
        }

        private void MA_AnnulerTout_Click(object sender, System.EventArgs e)
        {
            Serveur.Rummy.AnnulerTout();
        }

        private void MA_Refaire_Click(object sender, System.EventArgs e)
        {
            Serveur.Rummy.RefaireAction();
        }

        private void MA_RefaireTout_Click(object sender, System.EventArgs e)
        {
            Serveur.Rummy.RefaireTout();
        }

        private void MJ_Nouveau_Click(object sender, System.EventArgs e)
        {
            if (dicZoneJoueurs != null)
            {
                if (MessageBox.Show(Configuration.ObtenirTexte("QuitterPartieEnCours"),
                                    Configuration.ObtenirTexte("NouvellePartie"),
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information) == DialogResult.No)
                    return;

                FinirPartie(false);
            }

            InitialiserJeu();
        }

        private void MJ_Rafraichir_Click(object sender, System.EventArgs e)
        {
            objZoneJeu.ChargerContexte();
        }

        private void MJ_Quitter_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void MO_Configuration_Click(object sender, System.EventArgs e)
        {
            frmConfiguration frm = new frmConfiguration();

            frm.Initialiser(dicZoneJoueurs != null);
            if (frm.ConfigurationJoueursModifiee)
                FinirPartie(true);

            frm.Close();
        }

        private void MO_Scores_Click(object sender, System.EventArgs e)
        {
            if (dicZoneJoueurs == null || dicZoneJoueurs.Count == 0)
                return;

            string strTexte = String.Empty;
            foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
            {
                if (strTexte.Length > 0)
                    strTexte += Environment.NewLine;

                String strPlurielPoints = String.Empty;
                if (objZoneJoueur.NbPoints > 1)
                    strPlurielPoints = Configuration.ObtenirTexte("PlurielPoints");

                strTexte += String.Format("{0}{1}: {2} {3}{4}",
                                          objZoneJoueur.Nom,
                                          Configuration.ObtenirTexte("EspaceJoueur"),
                                          objZoneJoueur.NbPoints,
                                          Configuration.ObtenirTexte("Point"),
                                          strPlurielPoints);
            }

            MessageBox.Show(strTexte,
                            MO_Scores.Text.Replace("&", String.Empty),
                            MessageBoxButtons.OK);
        }

        #endregion

        #region Gestion des événements lancés suite aux changements de configuration

        private void AppliquerLangue()
        {
            MenuJeu.Text = Configuration.ObtenirTexte("MenuJeu");
            MJ_Nouveau.Text = Configuration.ObtenirTexte("MJ_Nouveau");
            MJ_Rafraichir.Text = Configuration.ObtenirTexte("MJ_Rafraichir");
            MJ_Quitter.Text = Configuration.ObtenirTexte("MJ_Quitter");
            MenuOptions.Text = Configuration.ObtenirTexte("MenuOptions");
            MO_Scores.Text = Configuration.ObtenirTexte("MO_Scores");

            // Actualisation du nombre de cartes restantes pour chaque joueur
            foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
                ActualiserNbCartes(objZoneJoueur);

            // Actualisation du nombre de cartes restantes dans la pioche
            objZoneJeu.LancerEvtActualiserNbCartesPioche();

            // Modification du texte du menu contextuel de la zone de jeu
            objZoneJeu.AffecterTextesMenuContextuel(new List<string>{ Configuration.ObtenirTexte("MC_ZoneJeu"), 
                                                                      Configuration.ObtenirTexte("MCD_Avant"), 
                                                                      Configuration.ObtenirTexte("MCD_Apres") });
        }

        private void AppliquerCouleurFond()
        {
            this.BackColor = Configuration.CouleurFond;

            // Répercution sur les zones joueur et la zone de jeu
            objZoneJeu.CouleurFond = Configuration.CouleurFond;
            foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
                objZoneJoueur.CouleurFond = Configuration.CouleurFond;
        }

        private void ActualiserNbCartes(ZoneJoueur zoneJoueur)
        {
            string strPlurielCartes = String.Empty;
            string strPlurielRestant = String.Empty;

            if (zoneJoueur.NbCartes > 1)
            {
                strPlurielCartes = Configuration.ObtenirTexte("PlurielCartes");
                strPlurielRestant = Configuration.ObtenirTexte("PlurielRestant");
            }

            string strTexte = String.Format("{0}{1}: {2} {3}{4} {5}{6}",
                                            zoneJoueur.Nom,
                                            Configuration.ObtenirTexte("EspaceJoueur"),
                                            zoneJoueur.NbCartes,
                                            Configuration.ObtenirTexte("Carte"),
                                            strPlurielCartes,
                                            Configuration.ObtenirTexte("CartesRestantes"),
                                            strPlurielRestant);


            dicTSSLZoneJoueurs[zoneJoueur].Text = strTexte;
        }

        #endregion

        #region Gestion des événements lancés par la zone joueur courant

        private void ZoneJoueur_NbCartesChanged()
        {
            ActualiserNbCartes(objZoneJoueurCourant);
        }

        private void ZoneJoueur_AMonTour(ZoneJoueur sender)
        {
            try
            {
                objZoneJoueurCourant = sender;
                objZoneJeu.ADeposePointsMinimum = objZoneJoueurCourant.Joueur.ADeposePointsMinimum;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        #endregion

        #region Gestion des événements lancés par la zone de jeu

        private void ZoneJeu_ActualiserNbCartesPioche(int nbCartes)
        {
            string strPlurielCartes = String.Empty;
            string strPlurielRestant = String.Empty;
            if (nbCartes > 1)
            {
                strPlurielCartes = Configuration.ObtenirTexte("PlurielCartes");
                strPlurielRestant = Configuration.ObtenirTexte("PlurielRestant");
            }

            tsslPioche.Text = String.Format("{0}{1}: {2} {3}{4} {5}{6}",
                                            Configuration.ObtenirTexte("Pioche"),
                                            Configuration.ObtenirTexte("EspaceJoueur"),
                                            nbCartes.ToString(),
                                            Configuration.ObtenirTexte("Carte"),
                                            strPlurielCartes,
                                            Configuration.ObtenirTexte("CartesRestantes"),
                                            strPlurielRestant);
        }

        private void ZoneJeu_PartieTerminee()
        {
            try
            {
                string strTexte = String.Format("{0} {1} {2}!",
                                                Configuration.ObtenirTexte("LeJoueur"),
                                                objZoneJoueurCourant.Nom,
                                                Configuration.ObtenirTexte("Gagne"));
                foreach (ZoneJoueur objZoneJoueur in dicZoneJoueurs.Values)
                    if (objZoneJoueur != objZoneJoueurCourant)
                    {
                        string s = String.Empty;
                        int intNbPoints = objZoneJoueur.PointsRestants;
                        if (intNbPoints > 1)
                            s = Configuration.ObtenirTexte("PlurielPoints");

                        strTexte += String.Format("{0}{1} {2}{3}: {4} {5}{6}",
                                                  Environment.NewLine,
                                                  Configuration.ObtenirTexte("Penalite"),
                                                  objZoneJoueur.Nom,
                                                  Configuration.ObtenirTexte("EspaceJoueur"),
                                                  intNbPoints.ToString(),
                                                  Configuration.ObtenirTexte("Point"),
                                                  s);
                    }

                MessageBox.Show(strTexte);

                strTexte = String.Format("{0}?", Configuration.ObtenirTexte("Rejouer"));

                if (MessageBox.Show(strTexte,
                                    Configuration.ObtenirTexte("PartieTerminee"),
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information) == DialogResult.Yes)
                    FinirPartie(true);
                else
                    FinirPartie(false);
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        #endregion

        #region Gestion des événements lancés par le serveur

        private void OnPartieEnCours()
        {
        }

        private void OnPartiePeutCommencer()
        {
            boolPartiePeutCommencer = true;
        }

        #endregion

        public Rummy()
        {
            InitializeComponent();

            Serveur.Rummy.PartieEnCours += OnPartieEnCours;
            Serveur.Rummy.PartiePeutCommencer += OnPartiePeutCommencer;
        }
    }
}
