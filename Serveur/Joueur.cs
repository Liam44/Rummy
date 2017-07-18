using System;

using NoyauCommun.Cartes;
using NoyauCommun.Enumeres;
using NoyauCommun.Erreurs;

namespace Serveur
{
    public class Joueur
    {
        private bool boolCartesVisibles = false;
        private int intNbCartesInitial = Rummy.NB_CARTES;
        private string strNom;

        protected bool boolADeposePointsMinimum = false;

        // Indique le nombre de points cumulés au cours des parties
        private int intNbPoints = 0;

        private Combinaison objMainJoueur = null;

        // Evénements lancés à l'intention des zone du joueur
        public delegate void TourTermineEvtHandler(bool aDeposeCartes);
        public event TourTermineEvtHandler TourTermine;
        public delegate void AMonTourEvtHandler();
        public event AMonTourEvtHandler AMonTour;

        public event CombinaisonChangedEvtHandler MainJoueurChanged;

        public string Nom
        {
            get
            {
                return strNom;
            }
            set
            {
                strNom = value;
            }
        }

        public bool CartesVisibles
        {
            get
            {
                return boolCartesVisibles;
            }
        }

        public Combinaison MainJoueur
        {
            get
            {
                return objMainJoueur;
            }
        }

        public int NbCartes
        {
            get
            {
                return objMainJoueur.NbCartes;
            }
        }

        public bool ADeposePointsMinimum
        {
            get
            {
                return boolADeposePointsMinimum;
            }
        }

        public int PointsRestants
        {
            get
            {
                intNbPoints += objMainJoueur.PointsRestants;

                return objMainJoueur.PointsRestants;
            }
        }

        public int NbPoints
        {
            get
            {
                return intNbPoints;
            }
        }

        #region Gestion des événements lancés par la zone de jeu

        public void TerminerTourJeu(bool aDeposePointsMinimum)
        {
            foreach (Carte objCarte in objMainJoueur.CartesCombinaison)
                objCarte.Desurligner();

            if (!boolADeposePointsMinimum)
                boolADeposePointsMinimum = aDeposePointsMinimum;

            if (TourTermine != null)
                TourTermine(boolADeposePointsMinimum);
        }

        public void Piocher(bool trier)
        {
            AjouterCarte(Rummy.Piocher(boolCartesVisibles), trier: trier);
        }

        /// <summary>
        /// Ajoute une carte à la main du joueur
        /// </summary>
        /// <param name="carte">Carte ajoutée</param>
        /// <param name="position">Position à laquelle la carte est insérée dans la main
        /// (-1 indique que la carte est ajoutée en dernier)</param>
        /// <param name="trier">Indique si la main du joueur doit ou non être triée après ajout de la carte</param>
        public void AjouterCarte(Carte carte,
                                 int position = -1,
                                 bool trier = false)
        {
            objMainJoueur.Ajouter(carte, position);
            if (trier)
                objMainJoueur.Trier();
        }

        /// <summary>
        /// Fonction vide, implémentée uniquement lors de l'instanciation du joueur automatique
        /// </summary>
        /// <remarks></remarks>
        public virtual void ATonTour()
        {
            if (AMonTour != null)
                AMonTour();

            // Historisation de la situation initiale
            Historique.Historiser();
        }

        #endregion

        #region Evénements lancés à l'attention de la zone du joueur

        public void CarteJoueurAcceptee(ref Carte carte)
        {
            try
            {
                if (carte.ControleParentOrigine != eControleOrigine.zoneJoueur)
                    return;

                Carte objCarte = objMainJoueur.Carte(carte);

                if (objCarte != null)
                {
                    objCarte.Supprimer();
                    carte = objCarte;
                }
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        private void OnMainJoueurModifiee()
        {
            if (MainJoueurChanged != null)
                MainJoueurChanged();
        }

        #endregion

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="nom">Nom du joueur</param>
        /// <param name="visible">Indique si les cartes du joueur sont visibles ou non</param>
        /// <param name="nbCartes">Indique le nombre de cartes que doit contenir la main du joueur par défaut</param>
        public Joueur(string nom, bool visible = false, int nbCartes = Rummy.NB_CARTES)
        {
            strNom = nom;
            boolCartesVisibles = visible;
            intNbCartesInitial = nbCartes;

            Reinitialiser();
        }

        /// <summary>
        /// Réinitialise la main du joueur
        /// </summary>
        /// <param name="contexteHistorique">Permet de réinitialiser la main du joueur avec un jeu de cartes prédéterminé</param>
        /// <param name="aDeposePointsMinimum">Indique si, conséquemment à la réinitialisation de la main du joueur, celui-ci
        /// préserve les points minimum nécessaire à la validation du tour de jeu</param>
        /// <remarks></remarks>
        public void Reinitialiser(Combinaison contexteHistorique = null,
                                  bool aDeposePointsMinimum = false)
        {
            //Evt_Reinitialiser(contexteHistorique, aDeposePointsMinimum);
            if (objMainJoueur != null)
                objMainJoueur.CombinaisonChanged -= OnMainJoueurModifiee;

            if (contexteHistorique == null)
            {
                boolADeposePointsMinimum = false;

                objMainJoueur = Rummy.Distribuer(intNbCartesInitial, boolCartesVisibles);
                objMainJoueur.Trier();
                objMainJoueur.CombinaisonChanged += OnMainJoueurModifiee;
            }
            else
            {
                boolADeposePointsMinimum = aDeposePointsMinimum;

                objMainJoueur = (Combinaison)contexteHistorique.Clone();
                objMainJoueur.CombinaisonChanged += OnMainJoueurModifiee;
            }

            OnMainJoueurModifiee();
        }

    }
}
