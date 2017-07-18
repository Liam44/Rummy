using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NoyauCommun.Cartes;
using NoyauCommun.Erreurs;

namespace Serveur
{
    public class Historique
    {
        private static List<EtapeHistorique> lstEtapesHistorique = new List<EtapeHistorique>();
        private static int intIndiceHistorique = -1;

        public delegate void HistorisationHandler();
        public static event HistorisationHandler HistoriqueChanged;

        private static void OnHistoriqueChanged()
        {
            if (HistoriqueChanged != null)
                HistoriqueChanged();
        }

        public static bool PeutRefaire
        {
            get
            {
                return intIndiceHistorique < lstEtapesHistorique.Count - 1;
            }
        }

        public static bool PeutAnnuler
        {
            get
            {
                return intIndiceHistorique > 0;
            }
        }

        public static EtapeHistorique RefaireAction()
        {
            EtapeHistorique objRes = null;

            try
            {
                if (PeutRefaire)
                {
                    intIndiceHistorique += 1;
                    objRes = lstEtapesHistorique[intIndiceHistorique];
                }

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
            finally
            {
                OnHistoriqueChanged();
            }
        }

        public static EtapeHistorique RefaireTout()
        {
            EtapeHistorique objRes = null;

            if (PeutRefaire)
            {
                objRes = lstEtapesHistorique.Last();
                intIndiceHistorique = lstEtapesHistorique.Count - 1;

                OnHistoriqueChanged();
            }

            return objRes;
        }

        public static EtapeHistorique AnnulerAction()
        {
            EtapeHistorique objRes = null;

            try
            {
                if (PeutAnnuler)
                {
                    intIndiceHistorique -= 1;
                    objRes = lstEtapesHistorique[intIndiceHistorique];
                }

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
            finally
            {
                OnHistoriqueChanged();
            }
        }

        public static EtapeHistorique AnnulerTout()
        {
            EtapeHistorique objRes = null;

            if (PeutAnnuler)
            {
                objRes = lstEtapesHistorique.First();
                intIndiceHistorique = 0;

                OnHistoriqueChanged();
            }

            return objRes;
        }

        protected internal static void Historiser()
        {
            try
            {
                /* Une nouvelle historisation signifie la création d'une nouvelle branche
                 * d'historisation des modifications, impliquant la perte de la branche précédente */
                while (intIndiceHistorique < lstEtapesHistorique.Count - 1)
                    lstEtapesHistorique.RemoveAt(intIndiceHistorique + 1);

                lstEtapesHistorique.Add(new EtapeHistorique(ZoneJeu.Combinaisons,
                                                            ZoneJeu.CartesDeposees,
                                                            Rummy.JoueurCourant));
                intIndiceHistorique += 1;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
            finally
            {
                OnHistoriqueChanged();
            }
        }

        public static void ViderHistorique()
        {
            lstEtapesHistorique.Clear();
            intIndiceHistorique = -1;
        }
    }

    public class EtapeHistorique
    {
        private List<Combinaison> lstCombinaisonsJeu = null;
        private Combinaison objMainJoueur = null;
        private List<Carte> lstCartesDeposees = null;
        private bool boolADeposePointsMinimum = false;

        public List<Combinaison> CombinaisonsJeu
        {
            get
            {
                return lstCombinaisonsJeu;
            }
        }

        public Combinaison MainJoueur
        {
            get
            {
                return objMainJoueur;
            }
        }

        public List<Carte> CartesDeposees
        {
            get { return lstCartesDeposees; }
            private set { }
        }

        public bool ADeposePointsMinimum
        {
            get
            {
                return boolADeposePointsMinimum;
            }
        }

        protected internal EtapeHistorique(List<Combinaison> combinaisonsJeu,
                                           List<Carte>cartesDeposees,
                                           Joueur joueur)
        {
            lstCombinaisonsJeu = new List<Combinaison>();

            foreach (Combinaison objCombinaison in combinaisonsJeu)
                lstCombinaisonsJeu.Add(objCombinaison.Clone());

            objMainJoueur = joueur.MainJoueur.Clone();

            lstCartesDeposees = cartesDeposees;

            boolADeposePointsMinimum = joueur.ADeposePointsMinimum;
        }
    }
}
