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

namespace Serveur
{
    public class ZoneJeu
    {
        private static List<Combinaison> lstCombinaisonsDeposees = new List<Combinaison>();
        private static List<Carte> lstCartesDeposees = new List<Carte>();

        #region Evénements

        // Evénements à l'attention de la zone de jeu
        public delegate void CombinaisonsChangedEvtHandler(bool historiser);
        public static event CombinaisonsChangedEvtHandler CombinaisonsChanged;

        public delegate void DoitDeposerCartesEvtHandler();
        public static event DoitDeposerCartesEvtHandler DoitDeposerCartes;

        public delegate void PiocheInterditeEvtHandler();
        public static event PiocheInterditeEvtHandler PiocheInterdite;

        public delegate void CombinaisonInvalideEvtHandler();
        public static event CombinaisonInvalideEvtHandler CombinaisonInvalide;

        public delegate void PasAssezPointsEvtHandler();
        public static event PasAssezPointsEvtHandler PasAssezPoints;

        public delegate void CartePiochee();
        public static event CartePiochee CartePiocheeEvtHandler; 

        public delegate void JoueurSuivantEvtHandler();
        public static event JoueurSuivantEvtHandler JoueurSuivant;

        public delegate void PartieTermineeEvtHandler();
        public static event PartieTermineeEvtHandler PartieTerminee;

        #endregion

        /// <summary>
        /// Renvoie la liste des combinaisons de cartes actuellement déposées sur la zone de jeu
        /// </summary>
        public static List<Combinaison> Combinaisons
        {
            get
            {
                return lstCombinaisonsDeposees.ToList();
            }
        }

        private static void AjouterCombinaison(Combinaison combinaison,
                                               bool leverEvenement = true)
        {
            combinaison.CombinaisonChanged += OnCombinaisonChanged;
            lstCombinaisonsDeposees.Add(combinaison);

            if (leverEvenement)
                OnCombinaisonModifiee();
        }

        protected internal static void ReinitialiserCombinaisons()
        {
            foreach (Combinaison objCombinaison in lstCombinaisonsDeposees)
            {
                objCombinaison.CombinaisonChanged -= OnCombinaisonChanged;
            }

            lstCombinaisonsDeposees.Clear();
            lstCartesDeposees.Clear();
        }

        private static void OnCombinaisonChanged()
        {
            OnCombinaisonModifiee();
        }

        /// <summary>
        /// Evénements à l'attention des différents clients connectés au serveur
        /// </summary>
        private static void OnCombinaisonModifiee(bool historiser = true)
        {
            if (CombinaisonsChanged != null)
                CombinaisonsChanged(historiser);
        }

        protected internal static List<Carte> CartesDeposees
        {
            get { return lstCartesDeposees.ToList(); }
            private set { }
        }

        #region Gestion de l'historique

        protected internal static bool ChargerContexteHistorique(List<Combinaison> combinaisonsJeu,
                                                                 List<Carte> cartesDeposees)
        {
            foreach (Combinaison objCombinaison in lstCombinaisonsDeposees)
            {
                objCombinaison.CombinaisonChanged -= OnCombinaisonChanged;
            }

            lstCombinaisonsDeposees.Clear();

            foreach (Combinaison objCombinaison in combinaisonsJeu.ToList())
                AjouterCombinaison(objCombinaison, false);

            lstCartesDeposees = cartesDeposees;

            OnCombinaisonModifiee(false);

            return true;
        }

        #endregion

        private static Combinaison NouvelleCombinaison(Carte carte)
        {
            try
            {
                /* L'ajout de la carte à la combinaison se fait lors de l'appel à la méthode "AjouterCarte"
                 * à la fin du traitement */
                carte.Supprimer();

                Combinaison objCombinaison = new Combinaison();

                AjouterCombinaison(objCombinaison, false);

                AjouterCarte(carte, objCombinaison);

                return objCombinaison;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return new Combinaison();
            }
        }

        private static Carte AjouterCarte(Carte carte,
                                          Combinaison combinaison,
                                          int position = -1,
                                          bool remplacerCarte = false)
        {
            try
            {
                carte.Afficher(true);

                Carte objCarte = combinaison.Ajouter(carte, position, remplacerCarte);

                if (objCarte != null)
                    NouvelleCombinaison(objCarte);

                return objCarte;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        protected internal static Carte SupprimerCarte(Carte carte)
        {
            try
            {
                Combinaison objCombinaison = carte.Combinaison;

                /* Il n'y a plus qu'une seule carte dans la combinaison, et que l'on s'apprête
                 * à supprimer.
                 * Il faut donc également supprimer la combinaison de la liste des combinaisons déposées */
                if (objCombinaison != null && objCombinaison.NbCartes == 1)
                    lstCombinaisonsDeposees.Remove(objCombinaison);

                Carte objRes = carte.Supprimer();

                if (carte.ControleParentOrigine == eControleOrigine.zoneJoueur)
                    RetirerCarte(carte);

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return carte;
            }
        }

        #region Méthodes appelées à la fois par le joueur humain et le joueur automatique

        /// <summary>
        /// Permet au joueur de déposer une carte de sa main sur la zone de jeu
        /// </summary>
        /// <param name="carte">Carte que le joueur est sur le point de déposer</param>
        /// <param name="objetDest">Objet recevant la carte sur le point d'être déposée. Peut être :
        /// - une carte de la zone de jeu (la carte déposée est alors insérée avant la carte de destination)
        /// - une combinaison de la zone de jeu (la carte déposée est alors ajoutée à la fin de la combinaison)
        /// - l'objet "null", indiquant que la carte a été déposée directement sur la zone de jeu en vue de la création d'une nouvelle combinaison</param>
        public static Carte DeposerCarte(ref Carte carte, object objetDest)
        {
            Carte objRes = null;

            // Acquittement du déplacement
            Rummy.JoueurCourant.CarteJoueurAcceptee(ref carte);

            // La carte est déposée sur ...
            //   ... la zone de jeu
            if (objetDest == null)
            {
                //       => créer une nouvelle combinaison
                NouvelleCombinaison(carte);
            }
            else if (objetDest is Combinaison)
            {
                /*   ... une combinaison existante
                 *       => ajouter la carte à la fin de la combinaison */
                AjouterCarte(carte, (Combinaison)objetDest);
            }
            else
            {
                /*   ... une carte de la zone de jeu
                 *       => la carte est un joker pouvant être remplacé par la carte déplacée
                 *          ==> ajouter carte déplacée à la combinaison de destination
                 *          ==> retirer le joker de la combinaison
                 *          ==> créer une nouvelle combinaison
                 *       => sinon
                 *          ==> insérer la carte déplacée à la place de la carte de destination */
                Carte objCarteDest = (Carte)objetDest;
                objRes = AjouterCarte(carte,
                                      objCarteDest.Combinaison,
                                      objCarteDest.Position,
                                      remplacerCarte: (objCarteDest.ControleParentOrigine == eControleOrigine.zoneJeu));
            }

            // Mémorisation qu'une nouvelle carte a été déposée sur la zone de jeu
            if (carte.ControleParentOrigine == eControleOrigine.zoneJoueur && !lstCartesDeposees.Contains(carte))
                lstCartesDeposees.Add(carte);

            return objRes;
        }

        /// <summary>
        /// Permet au joueur de déplacer une carte déjà présente sur la zone de jeu
        /// </summary>
        /// <param name="carte">Carte à déplacer</param>
        /// <param name="objetDest">Objet sur lequel est déplacée la carte de la zone de jeu. Peut être :
        /// - une autre carte de la zone de jeu (la carte d'origine est alors insérée avant la carte de destination)
        /// - une combinaison de la zone de jeu (la carte d'origine est alors ajoutée en fin de combinaison)
        /// - l'objet "null", indiquant que la carte est déplacée sur la zone de jeu en vue de la création d'une nouvelle combinaison</param>
        public static void DeplacerCarte(Carte carte, object objetDest)
        {
            // La carte a été déposée à la même place
            if (carte == objetDest)
            {
                // Il n'y a donc rien de plus à faire
                carte.Desurligner();
                return;
            }

            Combinaison objCombinaisonSource = carte.Combinaison;
            int intPositionSource = carte.Position;

            Carte objCarte = carte.Supprimer();

            // Vérification qu'aucun problème n'est survenu lors de la suppression de la carte
            if (objCarte == null)
                return;

            /* - la carte provient d'une combinaison de la zone de jeu et atterrit sur ...
             *   ... la zone de jeu */
            if (objetDest == null)
            {
                /*       => supprimer la carte de la combinaison d'origine
                 *       => créer une nouvelle combinaison */
                SupprimerCarte(carte);
                NouvelleCombinaison(carte);
            }
            else if (objetDest is Combinaison)
                //   ... la même combinaison
                if (objCombinaisonSource == (Combinaison)objetDest)
                    //       => ajouter la carte à la fin de la combinaison
                    ((Combinaison)objetDest).Ajouter(carte);
                else
                {
                    /*   ... une autre combinaison
                     *       => supprimer la carte de la combinaison source
                     *       => ajouter la carte à la fin de la combinaison de destination */
                    SupprimerCarte(carte);
                    AjouterCarte(carte, (Combinaison)objetDest);
                }
            else if (objCombinaisonSource == ((Carte)objetDest).Combinaison)
            {
                /*   ... une carte de la même combinaison
                 *       => ajouter la carte à la position de la carte de destination */
                Carte objCarteDest = (Carte)objetDest;
                objCarteDest.Combinaison.Ajouter(carte, objCarteDest.Position);
            }
            else
            {
                /*   ... une carte d'une autre combinaison
                 *       => le contrôle d'origine de la carte correspond à la combinaison de destination
                 *          ==> ajouter la carte à la position de la carte de destination
                 *       => sinon, la carte est un joker pouvant être remplacé par la carte déplacée
                 *          ==> ajouter carte déplacée à la combinaison
                 *          ==> retirer le joker de la combinaison
                 *          ==> créer une nouvelle combinaison
                 *       => sinon
                 *       ==> insérer la carte déplacée avant la carte destination */
                Carte objCarteDest = (Carte)objetDest;
                SupprimerCarte(carte);
                AjouterCarte(carte,
                             objCarteDest.Combinaison,
                             objCarteDest.Position,
                             remplacerCarte: true);
            }
        }

        /// <summary>
        /// Gère la pioche d'une carte par le joueur courant
        /// </summary>
        /// <param name="trier">Indique si la main du joueur doit ou non être triée après l'ajout de la carte piochée</param>
        /// <returns></returns>
        public static bool Piocher(bool trier = true)
        {
            try
            {
                if (lstCartesDeposees.Count > 0)
                {
                    PiocheInterdite();
                    return false;
                }

                Rummy.JoueurCourant.Piocher(trier);

                if (CartePiocheeEvtHandler != null)
                    CartePiocheeEvtHandler();

                // Si le joueur tire une carte, il ne peut plus jouer
                if (!TerminerTourJeu(true))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        public static bool PointsDeposesSuffisants()
        {
            int intNbPointsDeposes = 0;

            List<Combinaison> lstCombinaisons = new List<Combinaison>();

            foreach (Carte objCarte in lstCartesDeposees)
                if (objCarte.Combinaison != null &&
                    (lstCombinaisons.Contains(objCarte.Combinaison) ||
                     objCarte.Combinaison.Verifier()))
                {
                    if (objCarte.Valeur == eValeur.Joker)
                        intNbPointsDeposes += (int)objCarte.ValeurRemplacee;
                    else
                        intNbPointsDeposes += (int)objCarte.Valeur;

                    lstCombinaisons.Add(objCarte.Combinaison);
                }

            return intNbPointsDeposes >= Serveur.Rummy.NB_POINTS_MINIMUM;
        }

        /// <summary>
        /// Indique que le joueur a fini son tour de jeu
        /// </summary>
        /// <param name="aPioche">Indique si le joueur a ou non pioché une carte</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool TerminerTourJeu(bool aPioche)
        {
            try
            {
                if (!aPioche)
                {
                    // Il faut que le joueur ait déposé des cartes avant de pouvoir finir son tour de jeu
                    if (lstCartesDeposees.Count == 0)
                    {
                        DoitDeposerCartes();
                        return false;
                    }
                    else
                    {
                        // Affectation des cartes aux Panels auxquels ils sont rattachés
                        foreach (Combinaison objCombinaison in lstCombinaisonsDeposees.ToList())
                        {
                            // Vérification que toutes les combinaisons sont bien conformes aux attentes
                            if (!objCombinaison.Verifier())
                                if (objCombinaison.NbCartes == 0)
                                    lstCombinaisonsDeposees.Remove(objCombinaison);
                                else
                                {
                                    CombinaisonInvalide();
                                    return false;
                                }

                            // Vérification que le joueur a bien déposé le nombre de points minimal
                            if (!(Rummy.JoueurCourant.ADeposePointsMinimum || PointsDeposesSuffisants()))
                            {
                                PasAssezPoints();
                                return false;
                            }

                            foreach (Carte objCarte in objCombinaison.CartesCombinaison)
                                objCarte.ControleParentOrigine = eControleOrigine.zoneJeu;
                        }
                    }

                    // Mise en évidence des cartes déposées
                    foreach (Carte objCarte in lstCartesDeposees)
                        objCarte.Surligner(Color.DarkTurquoise);

                    lstCartesDeposees.Clear();
                }

                if (Rummy.JoueurCourant.NbCartes == 0)
                    PartieTerminee();
                else
                {
                    Rummy.TerminerTourJeu(!aPioche);
                    if (JoueurSuivant != null)
                        JoueurSuivant();
                }

                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        public static Combinaison DiviserCombinaison(Carte carte, bool avant)
        {
            try
            {
                Combinaison objCombinaison = carte.Combinaison;

                if (objCombinaison == null)
                    return null;

                Combinaison objRes = objCombinaison.Diviser(carte, avant);

                if (objRes == null)
                    return objRes;

                AjouterCombinaison(objRes);

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        #endregion

        #region Méthodes appelées uniquement par le joueur humain

        public static void RetirerCarte(Carte carte)
        {
            lstCartesDeposees.Remove(carte);
        }

        public static Carte CarteJeuAcceptee(Carte carte)
        {
            return SupprimerCarte(carte);
        }

        #endregion

        #region Méthodes appelées par le joueur automatique

        protected internal static bool ADeposeCartes
        {
            get { return lstCartesDeposees.Count > 0; }
            private set { }
        }

        //protected internal static Carte UtiliserCarte(Carte carte)
        //{
        //    try
        //    {
        //        Carte objRes = null;

        //        if (carte.Combinaison.NbCartes > 1 &&
        //            carte.Combinaison.TypeCombinaison == eTypeCombinaison.SuiteCouleurs)
        //            if (carte.Position == 0)
        //                DiviserCombinaison(carte, false);
        //            else if (carte.Position == carte.Combinaison.NbCartes - 1)
        //                DiviserCombinaison(carte, true);
        //            else
        //            {
        //                Combinaison objCombinaison = DiviserCombinaison(carte, false);
        //                if (carte.Position == 0)
        //                    DiviserCombinaison(carte, false);
        //                else
        //                    DiviserCombinaison(carte, true);
        //            }

        //        if (SupprimerCarte(carte) != null)
        //            objRes = carte;

        //        return objRes;
        //    }
        //    catch (Exception ex)
        //    {
        //        GestionErreurs.EnregistrerErreur(ex);
        //        return null;
        //    }
        //}

        #endregion
    }
}
