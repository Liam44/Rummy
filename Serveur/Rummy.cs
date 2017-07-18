using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NoyauCommun;
using NoyauCommun.Cartes;
using NoyauCommun.Configuration;
using NoyauCommun.Enumeres;
using NoyauCommun.Erreurs;

namespace Serveur
{
    public class Rummy
    {
        public const int NB_CARTES = 14;
        public const int NB_POINTS_MINIMUM = 30;

        private static List<Carte> lstCartes;

        private static List<Joueur> lstJoueurs = new List<Joueur>();
        private static List<Joueur> lstJoueursHumains = new List<Joueur>();
        private static Joueur objJoueurCourant = null;

        private static Representation objRepresentation = new Representation();

        // Evénements à l'attention de l'interface graphique
        public delegate void PartieEnCoursEvtHandler();
        public static PartieEnCoursEvtHandler PartieEnCours;

        public delegate void PartiePeutCommencerEvtHandler();
        public static PartiePeutCommencerEvtHandler PartiePeutCommencer;

        public static Representation Representation
        {
            get
            {
                return objRepresentation;
            }
        }

        public static List<Joueur> Joueurs
        {
            get
            {
                return new List<Joueur>(lstJoueurs);
            }
        }

        public static Joueur JoueurCourant
        {
            get
            {
                return objJoueurCourant;
            }
        }

        /// <summary>
        /// Initialise la liste de cartes
        /// </summary>
        /// <remarks></remarks>
        private static void Initialiser()
        {
            GestionErreurs.SupprimerFichierLog();

            lstCartes = new List<Carte>();
            foreach (eValeur enumValeur in Enum.GetValues(typeof(eValeur)))
                if (enumValeur > eValeur.Indetermine)
                    foreach (eCouleur enumCouleur in Enum.GetValues(typeof(eCouleur)))
                        if (enumCouleur > eCouleur.Indetermine)
                        {
                            // On ajoute deux exemplaires de chaque carte à la liste initiale
                            lstCartes.Add(Carte(enumValeur, enumCouleur));
                            lstCartes.Add(Carte(enumValeur, enumCouleur));
                        }

            objJoueurCourant = null;

            ViderHistorique();

            ZoneJeu.CombinaisonsChanged -= OnCombinaisonsJeuModifiees;
        }

        public static void NouvellePartie(bool reinitialiserJeu = true)
        {
            try
            {
                Initialiser();

                if (reinitialiserJeu)
                {
                    lstJoueurs = new List<Joueur>();

                    List<ePosition> lstPositions;

                    if (Configuration.Joueurs.Count == 2)
                        lstPositions = new List<ePosition> { ePosition.Bas, ePosition.Haut };
                    else
                    {
                        lstPositions = new List<ePosition>();
                        foreach (ePosition enumPosition in Enum.GetValues(typeof(ePosition)))
                            lstPositions.Add(enumPosition);
                    }

                    foreach (ConfigJoueur objJoueur in Configuration.Joueurs)
                        AjouterJoueur(objJoueur.Humain, objJoueur.Nom, lstPositions[lstJoueurs.Count], objJoueur.NiveauDifficulte);

                    ZoneJeu.ReinitialiserCombinaisons();

                    ZoneJeu.CombinaisonsChanged += OnCombinaisonsJeuModifiees;
                }

                JoueurSuivant();
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        private static void AjouterJoueur(bool humain,
                                          string nom,
                                          ePosition position,
                                          eNiveauDifficulte niveau = eNiveauDifficulte.Indetermine)
        {
            Joueur objJoueur = null;

            if (humain)
            {
                objJoueur = new Joueur(nom, true);
                lstJoueursHumains.Add(objJoueur);
            }
            else
                objJoueur = new JoueurAutomatique(nom, niveau);

            lstJoueurs.Add(objJoueur);
        }

        public static Joueur ObtenirJoueur()
        {
            try
            {
                if (lstJoueursHumains.Count == 0)
                {
                    /* Une nouvelle demande de connexion au serveur était inattendue :
                     * une partie doit déjà être en cours */
                    PartieEnCours();
                    return null;
                }
                else
                {
                    // On renvoie la premier joueur humain de la liste
                    Joueur objJoueur = lstJoueursHumains.First();
                    lstJoueursHumains.RemoveAt(0);

                    /* Si on n'attend plus de demande de connexion après celle-ci,
                     * cela signifie que la partie peut enfin commencer */
                    if (lstJoueursHumains.Count == 0)
                        PartiePeutCommencer();

                    return objJoueur;
                }
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        public static Carte Carte(eValeur valeur, eCouleur couleur)
        {
            return new Carte(valeur, couleur, objRepresentation);
        }

        /// <summary>
        /// Permet de créer un clone de la carte passée en paramètre
        /// </summary>
        /// <param name="carteParam">Carte à cloner</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected internal static Carte Carte(Carte carteParam)
        {
            return new Carte(carteParam.Valeur, carteParam.Couleur, Representation);
        }

        /// <summary>
        /// Retourne un jeu de main complet
        /// </summary>
        /// <param name="nbCartes">Indique le nombre de cartes que doit contenir la main</param>
        /// <param name="visible">Indique si la valeur des cartes doit ou non être visible par le joueur</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected internal static Combinaison Distribuer(int nbCartes = NB_CARTES, bool visible = false)
        {
            try
            {
                Combinaison objRes = new Combinaison();
                while (nbCartes > 0)
                {
                    Carte objCartePiochee = Piocher(visible);
                    objRes.Ajouter(objCartePiochee);
                    nbCartes -= 1;
                }

                objRes.Trier();

                return objRes;
            }
            catch (Exception)
            {
                return new Combinaison();
            }
        }

        /// <summary>
        /// Renvoie la liste des cartes restantes après distribution
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Combinaison Pioche
        {
            get
            {
                return new Combinaison(lstCartes);
            }
        }

        /// <summary>
        /// Fonction de pioche d'une carte dans le talon
        /// </summary>
        /// <param name="visible">Indique si la valeur de la carte doit ou non être visible par le joueur</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected internal static Carte Piocher(bool visible = false)
        {
            try
            {
                Random rd = new Random((int)(DateTime.Now.Ticks & 0xFFFF));
                int intPosition = rd.Next(0, lstCartes.Count - 1);

                Carte objRes = lstCartes[intPosition];
                objRes.Afficher(visible);
                // Si une carte est piochée, elle est nécessairement ajoutée à la zone du joueur courant
                objRes.ControleParentActuel = eControleOrigine.zoneJoueur;
                objRes.ControleParentOrigine = eControleOrigine.zoneJoueur;
                lstCartes.Remove(objRes);

                return objRes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void JoueurSuivant()
        {
            if (objJoueurCourant == null)
                objJoueurCourant = lstJoueurs.First();
            else
            {
                int intPosition = lstJoueurs.IndexOf(objJoueurCourant) + 1;
                if (intPosition == lstJoueurs.Count)
                    intPosition = 0;

                objJoueurCourant = lstJoueurs[intPosition];
            }
        }

        public static bool TerminerTourJeu(bool aDeposePointsMinimum)
        {
            ViderHistorique();

            objJoueurCourant.TerminerTourJeu(aDeposePointsMinimum);

            JoueurSuivant();

            objJoueurCourant.ATonTour();

            return true;
        }

        #region Gestion de l'historique

        private static void OnCombinaisonsJeuModifiees(bool historiser)
        {
            if (historiser)
                Historique.Historiser();
        }

        public static void AnnulerAction()
        {
            EtapeHistorique objAnnuler = Historique.AnnulerAction();

            if (objAnnuler != null)
            {
                objJoueurCourant.Reinitialiser(objAnnuler.MainJoueur, objAnnuler.ADeposePointsMinimum);
                ZoneJeu.ChargerContexteHistorique(objAnnuler.CombinaisonsJeu, objAnnuler.CartesDeposees);
            }
        }

        public static void AnnulerTout()
        {
            EtapeHistorique objAnnuler = Historique.AnnulerTout();

            if (objAnnuler != null)
            {
                objJoueurCourant.Reinitialiser(objAnnuler.MainJoueur, objAnnuler.ADeposePointsMinimum);
                ZoneJeu.ChargerContexteHistorique(objAnnuler.CombinaisonsJeu, objAnnuler.CartesDeposees);
            }
        }

        public static void RefaireAction()
        {
            EtapeHistorique objRefaire = Historique.RefaireAction();

            if (objRefaire != null)
            {
                objJoueurCourant.Reinitialiser(objRefaire.MainJoueur, objRefaire.ADeposePointsMinimum);
                ZoneJeu.ChargerContexteHistorique(objRefaire.CombinaisonsJeu, objRefaire.CartesDeposees);
            }
        }

        public static void RefaireTout()
        {
            EtapeHistorique objRefaire = Historique.RefaireTout();

            if (objRefaire != null)
            {
                objJoueurCourant.Reinitialiser(objRefaire.MainJoueur, objRefaire.ADeposePointsMinimum);
                ZoneJeu.ChargerContexteHistorique(objRefaire.CombinaisonsJeu, objRefaire.CartesDeposees);
            }
        }

        public static void ViderHistorique()
        {
            Historique.ViderHistorique();
        }

        #endregion
    }
}
