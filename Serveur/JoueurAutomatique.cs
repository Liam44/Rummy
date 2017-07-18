using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NoyauCommun.Cartes;
using NoyauCommun.Enumeres;
using NoyauCommun.Erreurs;

namespace Serveur
{
    class JoueurAutomatique : Joueur
    {
        private eNiveauDifficulte enumNiveauDifficulte;
        private Combinaison objMainJoueur;
        private List<Carte> lstJokersMain;
        private List<Combinaison> lstCombinaisonsZoneJeu;

        private List<Combinaison> lstCombinaisonsPotentielles;

        public JoueurAutomatique(string nom,
                                 eNiveauDifficulte niveauDifficulte = eNiveauDifficulte.Facile)
            : base(nom, false)
        {
            enumNiveauDifficulte = niveauDifficulte;
        }

        public override void ATonTour()
        {
            base.ATonTour();
            lstCombinaisonsZoneJeu = ZoneJeu.Combinaisons;
            CalculerPossibilites();
        }

        public void TracerContexte(bool enTete, bool separateur)
        {
            try
            {
                if (enTete)
                    GestionErreurs.Tracer(Nom, true);
                else
                    GestionErreurs.Tracer(String.Empty, separateur);

                GestionErreurs.Tracer("Main du joueur : " + objMainJoueur);
                GestionErreurs.Tracer("Combinaisons déposées :");
                foreach (Combinaison objCombinaisonJeu in lstCombinaisonsZoneJeu)
                    GestionErreurs.Tracer(objCombinaisonJeu.ToString());
            }
            catch (Exception)
            {
            }
        }

        private void SupprimerCarteMain(Carte carte)
        {
            Carte objCarte = CarteMainJoueur(carte);
            if (objCarte != null)
                objCarte.Supprimer();
        }

        private void CalculerPossibilites()
        {
            try
            {
                // Duplication de la liste des cartes constituant la main du joueur
                Dictionary<eCouleur, List<eValeur>> dicCouleursValeurs = new Dictionary<eCouleur, List<eValeur>>();
                Dictionary<eValeur, List<eCouleur>> dicValeursCouleurs = new Dictionary<eValeur, List<eCouleur>>();

                // Obtention de la liste des jokers présents dans la main
                lstJokersMain = new List<Carte>();

                objMainJoueur = base.MainJoueur.Clone();

                // Traçage des opérations effectuées par le joueur automatique
                TracerContexte(true, true);

                lstCombinaisonsPotentielles = new List<Combinaison>();
                foreach (Carte objCarte in objMainJoueur.CartesCombinaison)
                {
                    // Création d'une liste de cartes triées par couleurs puis par valeurs décroissantes
                    if (!dicCouleursValeurs.ContainsKey(objCarte.Couleur))
                        dicCouleursValeurs.Add(objCarte.Couleur, new List<eValeur>());

                    if (objCarte.Valeur == eValeur.Joker)
                        lstJokersMain.Add(objCarte.Clone());
                    else
                    {
                        // On en profite pour créer une liste de cartes triées par valeurs puis par couleurs
                        if (!dicValeursCouleurs.ContainsKey(objCarte.Valeur))
                            dicValeursCouleurs.Add(objCarte.Valeur, new List<eCouleur>());

                        if (!dicValeursCouleurs[objCarte.Valeur].Contains(objCarte.Couleur))
                            dicValeursCouleurs[objCarte.Valeur].Add(objCarte.Couleur);

                        if (!dicCouleursValeurs[objCarte.Couleur].Contains(objCarte.Valeur))
                            dicCouleursValeurs[objCarte.Couleur].Add(objCarte.Valeur);

                        lstCombinaisonsPotentielles.Add(new Combinaison(new List<Carte> { objCarte }));
                    }
                }

                foreach (eCouleur enumCouleur in dicCouleursValeurs.Keys.ToList())
                    if (!ChercherSuitesValeurs(enumCouleur, dicCouleursValeurs[enumCouleur]))
                        break;

                foreach (eValeur enumValeur in dicValeursCouleurs.Keys.ToList())
                    if (!ChercherSuitesCouleurs(enumValeur, dicValeursCouleurs[enumValeur]))
                        break;

                lstCombinaisonsPotentielles.Sort(new CComparerCombinaisons());

                // Calcul du nombre de points requis pour déposer des cartes sur la zone de jeu
                int intNbPoints = 0;

                List<Combinaison> lstCombinaisonsAJouer = new List<Combinaison>();

                List<Carte> lstVide = null;

                foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles)
                    if (objCombinaisonTmp.NbCartes < 3)
                        /* Les combinaisons étant triées par nombre décroissant de cartes,
                         * on s'arrête dès que l'une d'elles n'a pas pu être complétée par un joker */
                        break;
                    else
                    {
                        // Vérification que la main contient bien toutes les cartes nécessaires à sa réalisation
                        Combinaison objCombinaison = CreerCombinaison(objCombinaisonTmp,
                                                                      true,
                                                                      ref lstVide);

                        if (objCombinaison.NbCartes > 0)
                        {
                            lstCombinaisonsAJouer.Add(objCombinaison);

                            // Calcul du nombre de points qu'il est possible de déposer
                            foreach (Carte objCarte in objCombinaison.CartesCombinaison)
                                if (objCarte.Valeur == eValeur.Joker)
                                    intNbPoints += (int)objCarte.ValeurRemplacee;
                                else
                                    intNbPoints += (int)objCarte.Valeur;
                        }
                    }

                if (boolADeposePointsMinimum || intNbPoints >= Rummy.NB_POINTS_MINIMUM)
                {
                    boolADeposePointsMinimum = true;

                    foreach (Combinaison objCombinaison in lstCombinaisonsAJouer)
                        Jouer(objCombinaison);

                    // La suite du traitement n'a de sens que si la zone de jeu est déjà remplie de combinaisons
                    if (lstCombinaisonsZoneJeu.Count > 0)
                    {
                        /* On peut également essayer de compléter certaines combinaisons avec les cartes présentes
                         * sur la zone de jeu */

                        List<Carte> lstJokersZoneJeu = new List<Carte>();
                        switch (enumNiveauDifficulte)
                        {
                            case eNiveauDifficulte.Debutant:
                                {
                                    CompleterNiveau0(ref lstJokersZoneJeu);
                                    break;
                                }
                            case eNiveauDifficulte.Facile:
                                {
                                    CompleterNiveau1(ref lstJokersZoneJeu);
                                    break;
                                }
                            case eNiveauDifficulte.Moyen:
                                {
                                    CompleterNiveau2(ref lstJokersZoneJeu);
                                    break;
                                }
                            case eNiveauDifficulte.Difficile:
                                {
                                    CompleterNiveau3(ref lstJokersZoneJeu);
                                    break;
                                }
                        }

                        /* Tentative de completion de combinaisons potentielles incomplètes avec les jokers
                         * éventuellement récoltés sur la zone de jeu */
                        int intNbJokersPrec = lstJokersZoneJeu.Count;
                        while (lstJokersZoneJeu.Count > 0)
                        {
                            bool boolSortirBoucleFor = false;

                            foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles.ToList())
                            {
                                /* Remplacement des cartes "temporaires" par les véritables instances de cartes
                                 * constituant la main du joueur */
                                Combinaison objCombinaison = CreerCombinaison(objCombinaisonTmp, false, ref lstVide);

                                switch (objCombinaison.NbCartes)
                                {
                                    case 0:
                                    case 1:
                                        {
                                            break;
                                        }
                                    case 2:
                                        {
                                            Combinaison objCombinaisonJoker = lstJokersZoneJeu.First().Combinaison;
                                            bool boolJokerAjoute = false;
                                            Carte objJoker = lstJokersZoneJeu.First();
                                            for (int intPosition = 0; intPosition <= objCombinaison.NbCartes; intPosition++)
                                            {
                                                objCombinaison.Ajouter(objJoker, intPosition);
                                                if (objCombinaison.Verifier())
                                                {
                                                    boolJokerAjoute = true;
                                                    break;
                                                }
                                                else
                                                    objCombinaison.Supprimer(objJoker);
                                            }

                                            if (boolJokerAjoute)
                                            {
                                                int intPosition = 0;
                                                foreach (Carte objCarteTmp in objCombinaison.CartesCombinaison)
                                                {
                                                    if (objCarteTmp.Valeur != eValeur.Joker)
                                                    {
                                                        Jouer(objCarteTmp, objCombinaisonJoker, intPosition);

                                                        SupprimerCarteMain(objCarteTmp);
                                                    }

                                                    intPosition++;
                                                }
                                                lstJokersZoneJeu.RemoveAt(0);
                                                lstCombinaisonsPotentielles.Remove(objCombinaison);

                                                boolSortirBoucleFor = true;
                                            }

                                            break;
                                        }
                                    default:
                                        {
                                            Jouer(objCombinaison);
                                            break;
                                        }
                                }

                                if (boolSortirBoucleFor)
                                    break;
                            }

                            // Si malgré cela il reste des jokers implacés, on essaye de les fourguer sur la zone de jeu
                            if (lstJokersZoneJeu.Count > 0)
                                foreach (Combinaison objCombinaisonTmp in lstCombinaisonsZoneJeu.ToList())
                                {
                                    Combinaison objCombinaison = objCombinaisonTmp;

                                    if (objCombinaison.NbCartes < 3)
                                        lstCombinaisonsZoneJeu.Remove(objCombinaison);
                                    else
                                    {
                                        if (!objCombinaison.Verifier())
                                            GestionErreurs.Tracer(objCombinaison.ToString(), true);

                                        if (objCombinaison.PositionJoker == -1 &&
                                           (objCombinaison.Type == eTypeCombinaison.suitecouleurs ||
                                            objCombinaison.NbCartes < 4))
                                        {
                                            Carte objJoker = lstJokersZoneJeu.First();
                                            if (objCombinaison.Type == eTypeCombinaison.suitevaleurs ||
                                                objCombinaison.CartesCombinaison.Last().Valeur < eValeur.Treize)
                                            {
                                                GestionErreurs.Tracer(String.Format("Débarras de {0} en fin de {1}",
                                                                                    objJoker,
                                                                                    objCombinaison));
                                                Jouer(objJoker, objCombinaison);
                                            }
                                            else
                                            {
                                                GestionErreurs.Tracer(String.Format("Débarras de {0} en début de {1}",
                                                                                    objJoker,
                                                                                    objCombinaison));
                                                Jouer(objJoker, objCombinaison, 0);
                                            }
                                            lstJokersZoneJeu.RemoveAt(0);

                                            if (!objCombinaison.Verifier())
                                                GestionErreurs.Tracer(objCombinaison.ToString(), true);

                                            break;
                                        }
                                    }
                                }

                            if (intNbJokersPrec == lstJokersZoneJeu.Count)
                            {
                                // Impossible de replacer l'intégralité des jokers pris dans le jeu
                                System.Windows.Forms.MessageBox.Show("Replacer jokers jeu impossible !");
                                break;
                            }
                            else
                                intNbJokersPrec = lstJokersZoneJeu.Count;
                        }
                    }
                }

                if (ZoneJeu.ADeposeCartes)
                {
                    TracerContexte(false, true);
                    if (!ZoneJeu.TerminerTourJeu(false))
                        System.Windows.Forms.MessageBox.Show("Terminer tour jeu impossible !");
                }
                else if (!ZoneJeu.Piocher())
                    System.Windows.Forms.MessageBox.Show("Piocher carte impossible !");
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
            finally
            {
                lstCombinaisonsPotentielles.Clear();
                lstCombinaisonsZoneJeu.Clear();
                lstJokersMain.Clear();
            }
        }

        /// <summary>
        /// Construit toutes les suites de valeurs possibles pour une couleur donnée,
        /// à partir des cartes disponibles dans la main du joueur
        /// </summary>
        /// <param name="couleur">Couleur de la suite de valeurs</param>
        /// <param name="valeurs">Liste des valeurs de carte disponibles dans la main du joueur</param>
        /// <returns></returns>
        private bool ChercherSuitesValeurs(eCouleur couleur, List<eValeur> valeurs)
        {
            try
            {
                Combinaison objCombinaison = null;
                eValeur enumValeurPrec = eValeur.Indetermine;

                bool boolCombinaisonAjoutee = false;

                foreach (eValeur enumValeur in valeurs)
                    if (objCombinaison == null)
                    {
                        objCombinaison = new Combinaison(new List<Carte> { Carte(enumValeur, couleur) });

                        enumValeurPrec = enumValeur;
                    }
                    else if (enumValeur == enumValeurPrec + 1)
                    {
                        objCombinaison.Ajouter(Carte(enumValeur, couleur));

                        enumValeurPrec = enumValeur;
                    }
                    else if (enumValeur == enumValeurPrec + 2)
                    {
                        int intPositionJoker = objCombinaison.PositionJoker;

                        if (intPositionJoker == -1)
                        {
                            objCombinaison.Ajouter(Joker(enumValeur - 1, couleur));
                            objCombinaison.Ajouter(Carte(enumValeur, couleur));
                        }
                        else
                        {
                            CreerSousCombinaisons(objCombinaison);
                            boolCombinaisonAjoutee = true;

                            if (intPositionJoker == objCombinaison.NbCartes)
                                objCombinaison = new Combinaison(new List<Carte> { Carte(enumValeur, couleur) });
                            else
                            {
                                objCombinaison = objCombinaison.Diviser(objCombinaison.CartesCombinaison[intPositionJoker], false);
                                objCombinaison.Ajouter(Joker(enumValeur - 1, couleur));
                                objCombinaison.Ajouter(Carte(enumValeur, couleur));
                            }
                        }

                        enumValeurPrec = enumValeur;
                    }
                    else
                    {
                        CreerSousCombinaisons(objCombinaison);

                        objCombinaison = new Combinaison(new List<Carte> { Carte(enumValeur, couleur) });

                        enumValeurPrec = enumValeur;

                        boolCombinaisonAjoutee = false;
                    }

                if (!boolCombinaisonAjoutee)
                    CreerSousCombinaisons(objCombinaison);

                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        /// <summary>
        /// Assure l'ajout de la suite de valeurs à la liste de combinaisons potentielles
        /// </summary>
        /// <param name="combinaison">Suite de valeurs</param>
        private void CreerSousCombinaisons(Combinaison combinaison)
        {
            try
            {
                switch (combinaison.NbCartes)
                {
                    case 0:
                    case 1:
                        {
                            return;
                        }
                    case 2:
                        {
                            if (combinaison.PositionJoker == -1)
                            {
                                Combinaison objCombinaison = combinaison.Clone();

                                Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();
                                if (objPremiereCarte.Valeur > eValeur.Un)
                                {
                                    objCombinaison.Ajouter(Joker(objPremiereCarte.Valeur - 1, objPremiereCarte.Couleur), 0);
                                    lstCombinaisonsPotentielles.Add(objCombinaison);
                                }

                                objCombinaison = combinaison.Clone();
                                Carte objDerniereCarte = objCombinaison.CartesCombinaison.Last();
                                if (objDerniereCarte.Valeur < eValeur.Treize)
                                {
                                    objCombinaison.Ajouter(Joker(objDerniereCarte.Valeur + 1, objDerniereCarte.Couleur));
                                    lstCombinaisonsPotentielles.Add(objCombinaison);
                                }
                            }

                            break;
                        }
                    case 3:
                        {
                            lstCombinaisonsPotentielles.Add(combinaison);

                            if (combinaison.PositionJoker == -1)
                            {
                                Combinaison objClone = combinaison.Clone();
                                Combinaison objCombinaisonAvant = objClone.Diviser(objClone.CartesCombinaison.Last(), true);
                                CreerSousCombinaisons(objClone);

                                objClone = combinaison.Clone();
                                Combinaison objCombinaisonApres = objClone.Diviser(objClone.CartesCombinaison.First(), false);
                                CreerSousCombinaisons(objCombinaisonApres);
                            }

                            break;
                        }
                    default:
                        {
                            lstCombinaisonsPotentielles.Add(combinaison);

                            foreach (Carte objCarte in combinaison.CartesCombinaison)
                            {
                                Combinaison objClone = combinaison.Clone();
                                Combinaison objCombinaisonAvant = objClone.Diviser(objCarte, true);
                                if (objCombinaisonAvant != null && objClone.NbCartes > 0)
                                {
                                    CreerSousCombinaisons(objCombinaisonAvant);

                                    CreerSousCombinaisons(objClone);
                                }
                            }

                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        /// <summary>
        /// Réorganise les cartes de même valeur en suite de couleurs
        /// </summary>
        /// <param name="valeur">Valeur de la carte</param>
        /// <param name="couleurs">Liste des couleurs disponibles pour cette valeur</param>
        /// <returns>Renvoie la liste des combinaisons possibles</returns>
        private bool ChercherSuitesCouleurs(eValeur valeur, List<eCouleur> couleurs)
        {
            try
            {
                switch (couleurs.Count)
                {
                    case 4:
                    case 3:
                        {
                            /* Construction d'une nouvelle combinaison comprenant toutes les
                             * valeurs disponibles */
                            Combinaison objCombinaison = new Combinaison();
                            foreach (eCouleur enumCouleur in couleurs)
                            {
                                // Construction d'une combinaison possédant une couleur de moins
                                List<eCouleur> lstCouleurs = new List<eCouleur>(couleurs);
                                lstCouleurs.Remove(enumCouleur);

                                if (!ChercherSuitesCouleurs(valeur, lstCouleurs))
                                    return false;

                                Carte objCarte = Rummy.Carte(valeur, enumCouleur);
                                objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                                objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;

                                objCombinaison.Ajouter(objCarte);
                            }

                            // Ajout de cette combinaison à la liste des combinaisons potentielles
                            lstCombinaisonsPotentielles.Add(objCombinaison);

                            break;
                        }
                    case 2:
                        {
                            /* Construction d'une nouvelle combinaison comprenant toutes les
                             * valeurs disponibles, complétée par un joker */
                            Combinaison objCombinaison = new Combinaison();
                            foreach (eCouleur enumCouleur in Enum.GetValues(typeof(eCouleur)))
                            {
                                if (couleurs.Contains(enumCouleur))
                                {
                                    Carte objCarte = Rummy.Carte(valeur, enumCouleur);
                                    objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                                    objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;

                                    objCombinaison.Ajouter(objCarte);
                                }
                                else if (objCombinaison.PositionJoker == -1)
                                    objCombinaison.Ajouter(Joker(valeur, enumCouleur));
                                else
                                    // Le joker peut être remplacé par une carte supplémentaire
                                    objCombinaison.Carte(eValeur.Joker, eCouleur.Indetermine).AjouterCarteRemplacee(valeur,
                                                                                                                    enumCouleur);
                            }

                            // Ajout de cette combinaison à la liste des combinaisons potentielles
                            lstCombinaisonsPotentielles.Add(objCombinaison);

                            break;
                        }
                    default:
                        {
                            /* Pas assez de cartes de cette valeur disponibles : on ne peut
                             * donc même pas tenter de compléter la combinaison avec un joker */
                            break;
                        }
                }

                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        private Carte Carte(eValeur valeur, eCouleur couleur)
        {
            Carte objRes = Rummy.Carte(valeur, couleur);
            objRes.ControleParentActuel = eControleOrigine.zoneJoueur;
            objRes.ControleParentOrigine = eControleOrigine.zoneJoueur;

            return objRes;
        }

        /// <summary>
        /// Renvoie un nouveau joker, éventuellement initialisé avec une valeur à remplacer
        /// </summary>
        /// <param name="valeurRemplacee">Valeur de la carte que le joker peut remplacer</param>
        /// <param name="couleurRemplacee">Couleur de la carte que le joker peut remplacer</param>
        /// <returns></returns>
        private Carte Joker(eValeur valeurRemplacee = eValeur.Indetermine, eCouleur couleurRemplacee = eCouleur.Indetermine)
        {
            Carte objJoker = Carte(eValeur.Joker, eCouleur.Indetermine);

            if (valeurRemplacee != eValeur.Indetermine && couleurRemplacee != eCouleur.Indetermine)
                objJoker.AjouterCarteRemplacee(valeurRemplacee, couleurRemplacee);

            return objJoker;
        }

        /// <summary>
        /// Vérifie que toutes les cartes nécessaires à la création de la combinaison se trouvent toujours dans la main du joueur
        /// </summary>
        /// <param name="combinaison">Combinaison à créer</param>
        /// <param name="verifierNbCartesCombinaison">Indique si, après sa création, la combinaison doit ou non être vérifiée</param>
        /// <param name="jokers">Liste des jokers de la zone de jeu actuellement disponibles - si "null",
        /// les jokers de la main du joueur seront utilisés (si possible)</param>
        /// <returns></returns>
        private Combinaison CreerCombinaison(Combinaison combinaison,
                                             bool verifierNbCartesCombinaison,
                                             ref List<Carte> jokers)
        {
            try
            {
                Combinaison objRes = new Combinaison();

                GestionErreurs.Tracer("Création de " + combinaison + ", avec :");
                GestionErreurs.Tracer(objMainJoueur.ToString());

                /* Test préliminaire, afin de voir si l'on possède déjà assez de jokers
                 * pour créer la combinaison, si elle contient un quelconque joker */
                if (combinaison.PositionJoker != -1 &&
                    lstJokersMain.Count == 0 &&
                    (jokers == null || jokers.Count == 0))
                {
                    GestionErreurs.Tracer("Pas de joker disponible !");
                    return objRes;
                }

                Carte objCarteAbsente = null;
                Carte objJoker = null;

                foreach (Carte objJokerTmp in lstJokersMain)
                    GestionErreurs.Tracer(objJokerTmp.ToString());

                if (jokers != null)
                    foreach (Carte objJokerTmp in jokers)
                        GestionErreurs.Tracer(objJokerTmp.ToString());

                foreach (Carte objCarte in combinaison.CartesCombinaison)
                {
                    Carte objCarteMain = CarteMainJoueur(objCarte);

                    if (objCarteMain != null)
                        objRes.Ajouter(objCarteMain.Clone());
                    else if (objCarte.Valeur == eValeur.Joker)
                    {
                        if (!(jokers == null || jokers.Count == 0))
                            objJoker = jokers.First().Clone();
                        else if (lstJokersMain.Count > 0)
                            objJoker = lstJokersMain.First().Clone();
                        else
                        {
                            objCarteAbsente = objCarte;

                            // Il n'y a pas de joker pour compléter la combinaison
                            break;
                        }

                        objRes.Ajouter(objJoker);
                    }
                    else
                    {
                        objCarteAbsente = objCarte;

                        // Il n'y a pas de joker pour compléter la combinaison
                        break;
                    }
                }

                if (!objRes.Verifier(verifierNbCartesCombinaison))
                {
                    if (combinaison.NbCartes > 2 || jokers.Count > 0)
                    {
                        GestionErreurs.Tracer("Echec de la construction de " + combinaison);
                        if (objCarteAbsente != null)
                            GestionErreurs.Tracer("Il manque : " + objCarteAbsente);
                    }

                    objRes = new Combinaison();
                }
                else if (verifierNbCartesCombinaison)
                    // Suppression de la main du joueur des cartes utilisées
                    foreach (Carte objCarteTmp in objRes.CartesCombinaison)
                        if (objCarteTmp.Valeur == eValeur.Joker)
                            // Deux possibilités de source pour le joker :
                            if (objCarteTmp.VientDeZoneJoueur)
                                lstJokersMain.RemoveAt(0);
                            else
                                jokers.RemoveAt(0);
                        else
                            SupprimerCarteMain(objCarteTmp);

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return new Combinaison();
            }
        }

        /// <summary>
        /// Dépose l'intégralité des cartes de la combinaison sur la zone de jeu
        /// </summary>
        /// <param name="combinaison">Combinaison à déposer sur la zone de jeu</param>
        private bool Jouer(Combinaison combinaison)
        {
            GestionErreurs.Tracer("Jouer (" + combinaison + ")");

            // Dépose de toutes les cartes de la combinaison sur la zone de jeu
            Combinaison objCombinaison = null;
            foreach (Carte objCarteTmp in combinaison.CartesCombinaison)
            {
                /* On s'assure que la carte qui est déposée sur la zone de jeu
                 * soit bien celle qui provient de la main du joueur et non de son clone */
                Carte objCarte = null;
                if (objCarteTmp.ControleParentOrigine == eControleOrigine.zoneJoueur)
                    objCarte = CarteMainJoueur(objCarteTmp, true);
                else
                    objCarte = ZoneJeu.SupprimerCarte(objCarteTmp);

                if (objCarte == null)
                    return false;

                ZoneJeu.DeposerCarte(ref objCarte, objCombinaison);

                if (objCombinaison == null)
                    objCombinaison = objCarte.Combinaison;
            }

            return true;
        }

        /// <summary>
        /// Dépose la carte sur la combinaison passée en paramètre, à la position donnée
        /// </summary>
        /// <param name="carte">Carte de la main du joueur à déposer sur la zone de jeu</param>
        /// <param name="combinaison">Combinaison de la zone de jeu à compléter</param>
        /// <param name="position">Position à laquelle la carte est insérée dans la combinaison
        /// (-1 pour un ajout à la fin de la combinaison)</param>
        /// <returns>Renvoie le joker qui vient éventuellement d'être remplacer - "null" sinon</returns>
        private Carte Jouer(Carte carte, Combinaison combinaison, int position = -1)
        {
            try
            {
                Carte objRes = null;

                GestionErreurs.Tracer(String.Format("Jouer ({0}, {1}, {2})",
                                                    carte,
                                                    combinaison,
                                                    position));

                /* On s'assure que la carte qui est déposée sur la zone de jeu
                 * soit bien celle qui provient de la main du joueur et non de son clone */
                Carte objCarte = null;
                if (carte.ControleParentOrigine == eControleOrigine.zoneJoueur)
                    objCarte = CarteMainJoueur(carte, true);
                else
                    objCarte = ZoneJeu.SupprimerCarte(carte);

                if (position == -1)
                    ZoneJeu.DeposerCarte(ref objCarte, combinaison);
                else
                {
                    Carte objCarteDest = combinaison.CartesCombinaison[position];
                    objRes = ZoneJeu.DeposerCarte(ref objCarte, objCarteDest);
                }

                // On supprime la carte du clone de la main du joueur
                if (carte.ControleParentOrigine == eControleOrigine.zoneJoueur)
                    SupprimerCarteMain(carte);

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        #region Méthodes de complétion des combinaisons potentielles, en fonction du niveau de difficulté

        /// <summary>
        /// Renvoie la carte de la main du joueur correspondant à la carte passée en paramètre - "null" si la carte
        /// est absente de la main du joueur
        /// </summary>
        /// <param name="carte">Carte recherchée</param>
        /// <param name="mainOriginale">Indique si la carte doit être recherchée dans la main originale du joueur
        /// ou bien dans le clone local</param>
        /// <returns></returns>
        private Carte CarteMainJoueur(Carte carte, bool mainOriginale = false)
        {
            if (carte.ControleParentOrigine == eControleOrigine.zoneJeu)
                return carte;
            else if (mainOriginale)
                return base.MainJoueur.Carte(carte);
            else
                return objMainJoueur.Carte(carte);
        }

        /// <summary>
        /// Réinitialise les propriétés du joker, afin de pouvoir le réutiliser par la suite
        /// </summary>
        /// <param name="joker">Joker à réinitialiser</param>
        /// <returns>Renvoie le joker réinitialisé</returns>
        private Carte ReinitialiserJokerJeu(Carte joker)
        {
            joker.ReinitialiserCartesRemplacees();
            joker = ZoneJeu.SupprimerCarte(joker);

            return joker;
        }

        /// <summary>
        /// Indique si la carte recherchée se trouve sur la zone de jeu et peut "aisément" être utilisée
        /// </summary>
        /// <param name="carte">Carte recherchée</param>
        /// <returns></returns>
        private Carte RechercherCarte(Carte carte)
        {
            try
            {
                Carte objRes = null;

                List<Carte> lstCartes = new List<Carte>();

                foreach (Combinaison objCombinaisonJeu in lstCombinaisonsZoneJeu)
                {
                    Carte objCarte = objCombinaisonJeu.Carte(carte);
                    if (objCarte != null)
                        switch (objCombinaisonJeu.Type)
                        {
                            case eTypeCombinaison.suitevaleurs:
                                {
                                    if (objCombinaisonJeu.NbCartes > 3)
                                        lstCartes.Add(objCarte);

                                    break;
                                }
                            case eTypeCombinaison.suitecouleurs:
                                {
                                    if (objCarte.Position == 0 || objCarte.Position == objCombinaisonJeu.NbCartes - 1)
                                        lstCartes.Add(objCarte);
                                    else if (objCarte.Position >= 3 && objCarte.Position < objCombinaisonJeu.NbCartes - 3)
                                        lstCartes.Add(objCarte);

                                    break;
                                }
                            default: break;
                        }
                }

                if (lstCartes.Count == 2)
                {
                    // Est-il possible de fusionner les deux combinaisons ?
                    Carte objCarte1 = lstCartes.First();
                    Carte objCarte2 = lstCartes.Last();
                    Combinaison objCombinaison1 = objCarte1.Combinaison;
                    Combinaison objCombinaison2 = objCarte2.Combinaison;

                    if (objCombinaison1.Type == eTypeCombinaison.suitevaleurs &&
                        objCombinaison1.Type == objCombinaison2.Type)
                        if (objCarte1.Position == 0 && objCarte2.Position == objCombinaison2.NbCartes - 1)
                        {
                            GestionErreurs.Tracer("Division de " + objCombinaison1);
                            objRes = ZoneJeu.DiviserCombinaison(objCarte1, false).CartesCombinaison.First();
                            foreach (Carte objCarteTmp in objCombinaison1.CartesCombinaison)
                                ZoneJeu.DeplacerCarte(objCarteTmp, objCombinaison2);

                            GestionErreurs.Tracer("Obtention de " + objCombinaison2);
                        }
                        else if (objCarte2.Position == 0 && objCarte1.Position == objCombinaison1.NbCartes - 1)
                        {
                            GestionErreurs.Tracer("Division de " + objCombinaison2);
                            objRes = ZoneJeu.DiviserCombinaison(objCarte2, false).CartesCombinaison.First();
                            foreach (Carte objCarteTmp in objCombinaison2.CartesCombinaison)
                                ZoneJeu.DeplacerCarte(objCarteTmp, objCombinaison1);

                            GestionErreurs.Tracer("Obtention de " + objCombinaison1);
                        }

                    if (objRes == null)
                        if (objCombinaison1.NbCartes > 3)
                            objRes = objCarte1;
                        else if (objCombinaison2.NbCartes > 3)
                            objRes = objCarte2;
                }
                else if (lstCartes.Count > 0 && lstCartes.First().Combinaison.NbCartes > 3)
                    objRes = lstCartes.First();

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        /// <summary>
        /// Parcourt les combinaisons de la zone de jeu à la recherche de jokers pouvant être remplacés
        /// soit par des cartes de la zone de jeu, soit par des cartes de la main du joueur
        /// </summary>
        /// <param name="jokers">Liste des jokers de la zone de jeu ainsi obtenus</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CompleterNiveau0(ref List<Carte> jokers)
        {
            GestionErreurs.Tracer("Début de compléter Niveau 0");

            try
            {
                bool boolRes = false;

                foreach (Combinaison objCombinaison in lstCombinaisonsZoneJeu)
                {
                    int intPositionJoker = objCombinaison.PositionJoker;
                    if (intPositionJoker > -1)
                    {
                        Carte objJoker = objCombinaison.CartesCombinaison[intPositionJoker];
                        foreach (Carte objCarteManquante in objJoker.ValeursRemplacees)
                        {
                            Carte objCarte = RechercherCarte(objCarteManquante);
                            /* 1ère étape : recherche des jokers du jeu pouvant être remplacés par d'autres
                             * cartes du jeu */
                            if (objCarte != null)
                            {
                                GestionErreurs.Tracer(String.Format("Déplacement de {0} vers {1} à la position {2} (carte jeu)",
                                                                    objCarte,
                                                                    objCombinaison,
                                                                    intPositionJoker));
                                objJoker = Jouer(objCarte, objCombinaison, intPositionJoker);
                                if (objJoker != null)
                                {
                                    GestionErreurs.Tracer("Obtention de " + objJoker);
                                    jokers.Add(ReinitialiserJokerJeu(objJoker));
                                    boolRes = true;

                                    break;
                                }
                                else
                                    GestionErreurs.Tracer("Echec de l'obtention du joker");
                            }
                            else
                            {
                                /* 2ème étape : vérification que le joker en question
                                 * ne peut pas être remplacé par une carte de la main du joueur /*/
                                Carte objCarteMain = CarteMainJoueur(objCarteManquante, true);
                                if (objCarteMain != null)
                                {
                                    GestionErreurs.Tracer(String.Format("Déplacement de {0} vers {1} à la position {2} (carte main)",
                                                                        objCarteMain,
                                                                        objCombinaison,
                                                                        intPositionJoker));
                                    objJoker = Jouer(objCarteMain, objCombinaison, intPositionJoker);
                                    GestionErreurs.Tracer("Obtention de " + objJoker);
                                    jokers.Add(ReinitialiserJokerJeu(objJoker));
                                    boolRes = true;

                                    break;
                                }
                            }
                        }
                    }
                }

                if (boolRes)
                {
                    GestionErreurs.Tracer("Jokers obtenus :");
                    foreach (Carte objJoker in jokers)
                        GestionErreurs.Tracer(objJoker.ToString());

                    /* Si l'on a réussi à récupérer des jokers de la zone de jeu, il faut voir
                     * si l'on ne peut pas compléter des combinaisons de la main du joueur */
                    foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles)
                    {
                        GestionErreurs.Tracer(String.Format("Tentative de complétion de {0} avec un joker de la zone de jeu",
                                                            objCombinaisonTmp));
                        Combinaison objCombinaison = CreerCombinaison(objCombinaisonTmp, true, ref jokers);
                        if (objCombinaison.NbCartes > 0)
                        {
                            GestionErreurs.Tracer("Placement sur le jeu de " + objCombinaison);
                            if (!Jouer(objCombinaison))
                                break;
                        }

                        if (jokers.Count == 0)
                            break;
                    }
                }

                return boolRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
            finally
            {
                foreach (Carte objJoker in jokers)
                    GestionErreurs.Tracer(objJoker.ToString());

                GestionErreurs.Tracer("Fin de compléter Niveau 0");
            }
        }

        /// <summary>
        /// Parcourt la zone de jeu à la recherche de combinaisons pouvant "aisément" être complétées
        /// </summary>
        /// <param name="jokers"></param>
        /// <returns></returns>
        private bool CompleterNiveau1(ref List<Carte> jokers)
        {
            GestionErreurs.Tracer("Début de compléter Niveau 1");

            try
            {
                bool boolTourFini = true;

                bool boolRes = false;

                do
                {
                    boolTourFini = true;

                    if (CompleterNiveau0(ref jokers))
                        boolRes = true;

                    foreach (Carte objCarte in objMainJoueur.CartesCombinaison.ToList())
                    {
                        // On se réserve quand même les jokers
                        if (objCarte.Valeur != eValeur.Joker)
                        {
                            bool boolSortirFor = false;

                            foreach (Combinaison objCombinaison in lstCombinaisonsZoneJeu)
                            {
                                if (objCombinaison.NbCartes > 0)
                                {
                                    switch (objCombinaison.Type)
                                    {
                                        case eTypeCombinaison.suitevaleurs:
                                            {
                                                // 1ère possibilité : suite de valeurs
                                                try
                                                {
                                                    if ((objCarte.Valeur == objCombinaison.CartesCombinaison.First().Valeur ||
                                                         objCarte.Valeur == objCombinaison.CartesCombinaison.Last().Valeur) &&
                                                        objCombinaison.Carte(objCarte) == null)
                                                    {
                                                        GestionErreurs.Tracer(String.Format("Ajout de {0} à la fin de {1}",
                                                                                            objCarte,
                                                                                            objCombinaison));
                                                        Jouer(objCarte, objCombinaison);
                                                        if (objCombinaison.Verifier())
                                                            SupprimerCarteMain(objCarte);
                                                        else
                                                            TracerContexte(false, true);

                                                        boolRes = true;
                                                        boolTourFini = false;
                                                        boolSortirFor = true;
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    // Juste pour le point d'arrêt
                                                    Carte objCarteTmp = objCarte;
                                                }

                                                break;
                                            }
                                        case eTypeCombinaison.suitecouleurs:
                                            {
                                                // 2nde possibilité : suite de couleurs
                                                try
                                                {
                                                    Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();
                                                    if (objCombinaison.CartesCombinaison.First().Valeur == eValeur.Joker)
                                                    {
                                                        if (objPremiereCarte.PeutRemplacer(objCarte.Valeur + 1, objCarte.Couleur))
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en début de {1}",
                                                                                                objCarte,
                                                                                                objCombinaison));
                                                            Jouer(objCarte, objCombinaison, 0);
                                                            if (objCombinaison.Verifier())
                                                                SupprimerCarteMain(objCarte);
                                                            else
                                                                TracerContexte(false, true);

                                                            boolRes = true;
                                                            boolTourFini = false;
                                                            boolSortirFor = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (objCarte.Couleur == objPremiereCarte.Couleur &&
                                                            objCarte.Valeur == objPremiereCarte.Valeur - 1)
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en début de {1}",
                                                                                                objCarte,
                                                                                                objCombinaison));
                                                            Jouer(objCarte, objCombinaison, 0);
                                                            if (objCombinaison.Verifier())
                                                                SupprimerCarteMain(objCarte);
                                                            else
                                                                TracerContexte(false, true);

                                                            boolRes = true;
                                                            boolTourFini = false;
                                                            boolSortirFor = true;
                                                        }
                                                    }

                                                    Carte objDerniereCarte = objCombinaison.CartesCombinaison.Last();
                                                    if (objCombinaison.CartesCombinaison.Last().Valeur == eValeur.Joker)
                                                    {

                                                        if (objDerniereCarte.PeutRemplacer(objCarte.Valeur - 1, objCarte.Couleur))
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en fin de {1}",
                                                                                                objCarte,
                                                                                                objCombinaison));
                                                            Jouer(objCarte, objCombinaison);
                                                            if (objCombinaison.Verifier())
                                                                SupprimerCarteMain(objCarte);
                                                            else
                                                                TracerContexte(false, true);

                                                            boolRes = true;
                                                            boolTourFini = false;
                                                            boolSortirFor = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (objCarte.Couleur == objDerniereCarte.Couleur &&
                                                            objCarte.Valeur == objDerniereCarte.Valeur + 1)
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en fin de {1}",
                                                                                                objCarte,
                                                                                                objCombinaison));
                                                            Jouer(objCarte, objCombinaison);
                                                            if (objCombinaison.Verifier())
                                                                SupprimerCarteMain(objCarte);
                                                            else
                                                                TracerContexte(false, true);

                                                            boolRes = true;
                                                            boolTourFini = false;
                                                            boolSortirFor = true;
                                                        }
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    // Juste pour le point d'arrêt
                                                    Carte objCarteTmp = objCarte;
                                                }

                                                break;
                                            }
                                        default:
                                            {
                                                // Juste pour le point d'arrêt
                                                Carte objCarteTmp = objCarte;

                                                break;
                                            }
                                    }
                                }

                                if (boolSortirFor)
                                    break;
                            }
                        }

                        /* Si on a réussi à déposer une carte, on refait un tour, voir si on ne peut
                         * pas en déposer davantage */
                        if (!boolTourFini)
                            break;
                    }
                } while (!boolTourFini);

                return boolRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
            finally
            {
                GestionErreurs.Tracer("Fin de compléter Niveau 1");
            }
        }

        private bool CompleterNiveau2(ref List<Carte> jokers)
        {
            GestionErreurs.Tracer("Début de compléter Niveau 2");

            try
            {
                bool boolRes = CompleterNiveau1(ref jokers);

                bool boolSortirFor = false;
                eValeur enumValeur = eValeur.Indetermine;
                Carte objJoker = null;

                // 2nde étape : parcours des combinaisons potentielles de la main afin de
                // déterminer si leurs cartes ne sont pas complémentaires de combinaisons du jeu
                foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles.ToList())
                {
                    Combinaison objCombinaison = CreerCombinaison(objCombinaisonTmp, false, ref jokers);

                    if (objCombinaison.Verifier())
                    {
                        GestionErreurs.Tracer("Dépose de " + objCombinaison);
                        Jouer(objCombinaison);

                        return true;
                    }
                    else
                        switch (objCombinaison.Type)
                        {
                            case eTypeCombinaison.suitevaleurs:
                                // Calcul des cartes manquantes
                                enumValeur = eValeur.Indetermine;
                                List<eCouleur> lstCouleursManquantes = new List<eCouleur>();

                                // Si la combinaison possède un joker, on se base sur les cartes
                                // qu'il peut remplacer
                                // Sinon, il faut procéder par élimination
                                int intPositionJoker = objCombinaison.PositionJoker;
                                if (intPositionJoker == -1)
                                {
                                    foreach (eCouleur enumCouleur in Enum.GetValues(typeof(eCouleur)))
                                        if (enumCouleur > eCouleur.Indetermine)
                                            lstCouleursManquantes.Add(enumCouleur);

                                    enumValeur = objCombinaison.CartesCombinaison.First().Valeur;

                                    foreach (Carte objCarte in objCombinaison.CartesCombinaison)
                                        lstCouleursManquantes.Remove(objCarte.Couleur);
                                }
                                else
                                {
                                    objJoker = objCombinaison.CartesCombinaison[intPositionJoker];

                                    foreach (Carte objCarte in objJoker.ValeursRemplacees)
                                        lstCouleursManquantes.Add(objCarte.Couleur);

                                    enumValeur = objJoker.ValeurRemplacee;
                                }

                                // Parcours des combinaisons du jeu à la rechercher de cartes
                                // pouvant compléter la combinaison
                                foreach (eCouleur enumCouleur in lstCouleursManquantes)
                                {
                                    Carte objCarte = RechercherCarte(Rummy.Carte(enumValeur, enumCouleur));
                                    if (objCarte != null)
                                    {
                                        GestionErreurs.Tracer("Utilisation de " + objCarte);
                                        objCarte = ZoneJeu.SupprimerCarte(objCarte);
                                        objCombinaison.Ajouter(objCarte);
                                        if (!objCombinaison.Verifier())
                                            TracerContexte(false, true);
                                    }
                                }

                                if (objCombinaison.Verifier())
                                {
                                    GestionErreurs.Tracer("Dépose de " + objCombinaison);
                                    Jouer(objCombinaison);
                                    boolRes = true;
                                }

                                break;
                            case eTypeCombinaison.suitecouleurs:
                                // Calcul des cartes manquantes
                                List<Carte> lstCartesManquantes = new List<Carte>();
                                Dictionary<eValeur, int> dicPositionCarte = new Dictionary<eValeur, int>();

                                enumValeur = eValeur.Indetermine;

                                // Carte précédant la première carte de la combinaison, si celle-ci n'est pas un as
                                Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();

                                if (objPremiereCarte.Valeur == eValeur.Joker)
                                {
                                    enumValeur = objPremiereCarte.ValeurRemplacee;
                                    lstCartesManquantes.Add(Rummy.Carte(enumValeur, objPremiereCarte.ValeursRemplacees.First().Couleur));
                                    dicPositionCarte.Add(enumValeur, 0);
                                }
                                else
                                    enumValeur = objPremiereCarte.Valeur;

                                if (enumValeur > eValeur.Un)
                                {
                                    enumValeur -= 1;
                                    lstCartesManquantes.Add(Rummy.Carte(enumValeur, objPremiereCarte.Couleur));
                                    dicPositionCarte.Add(enumValeur, 0);
                                }

                                // Carte suivant la dernière carte de la combinaison, si celle-ci n'est pas un 13
                                Carte objDerniereCarte = objCombinaison.CartesCombinaison.Last();
                                enumValeur = eValeur.Indetermine;
                                if (objDerniereCarte.Valeur == eValeur.Joker)
                                {
                                    enumValeur = objDerniereCarte.ValeurRemplacee;
                                    lstCartesManquantes.Add(Rummy.Carte(enumValeur, objDerniereCarte.ValeursRemplacees.First().Couleur));
                                    dicPositionCarte.Add(enumValeur, objDerniereCarte.Position);
                                }
                                else
                                    enumValeur = objDerniereCarte.Valeur;

                                if (enumValeur < eValeur.Treize)
                                {
                                    enumValeur += 1;
                                    lstCartesManquantes.Add(Rummy.Carte(enumValeur, objDerniereCarte.Couleur));
                                    dicPositionCarte.Add(enumValeur, objDerniereCarte.Position + 1);
                                }

                                int intNbCartes = 0;
                                foreach (Carte objCarteTmp in lstCartesManquantes)
                                {
                                    Carte objCarte = RechercherCarte(objCarteTmp);
                                    if (objCarte != null)
                                    {
                                        GestionErreurs.Tracer("Utilisation de " + objCarte);
                                        objCarte = ZoneJeu.SupprimerCarte(objCarte);
                                        objJoker = objCombinaison.Ajouter(objCarte,
                                                                          dicPositionCarte[objCarte.Valeur] + intNbCartes,
                                                                          true);

                                        if (objJoker != null)
                                            // On a même réussi à libérer un joker
                                            jokers.Add(ReinitialiserJokerJeu(objJoker));

                                        intNbCartes += 1;
                                    }
                                }

                                if (objCombinaison.Verifier())
                                {
                                    GestionErreurs.Tracer("Dépose de " + objCombinaison);
                                    Jouer(objCombinaison);

                                    boolRes = true;
                                }

                                break;
                            default:
                                if (objCombinaison.NbCartes == 0)
                                {
                                    lstCombinaisonsPotentielles.Remove(objCombinaison);
                                    boolSortirFor = true;
                                }

                                // Le type de la combinaison n'a pas pu être déterminé
                                // Cela peut signifier deux choses :
                                // - la combinaison n'est constituée que d'une seule carte
                                // - la combinaison contient une carte plus un joker
                                Carte objCarteMain = null;
                                objJoker = null;

                                foreach (Carte objCarte in objCombinaison.CartesCombinaison)
                                    if (objCarte.Valeur == eValeur.Joker)
                                        objJoker = objCarte;
                                    else
                                        objCarteMain = objCarte;

                                // Calcul des combinaisons possibles avec la/les carte/s
                                // courante/s
                                List<Combinaison> lstCombinaisonsPossibles = new List<Combinaison> { new Combinaison(new List<Carte> { objCarteMain }) };

                                if (objJoker != null)
                                    foreach (eCouleur enumCouleur in Enum.GetValues(typeof(eCouleur)))
                                        if (enumCouleur != eCouleur.Indetermine)
                                            if (enumCouleur == objCarteMain.Couleur)
                                                switch (objCarteMain.Valeur)
                                                {
                                                    case eValeur.Un:
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte>{ objCarteMain,
                                                                                                                      objJoker,
                                                                                                                      Rummy.Carte(objCarteMain.Valeur + 2, enumCouleur) }));
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte>{ objCarteMain,
                                                                                                                      Rummy.Carte(objCarteMain.Valeur + 1, enumCouleur),
                                                                                                                      objJoker }));

                                                        break;
                                                    case eValeur.Treize:
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte> { objJoker,
                                                                                                                       Rummy.Carte(objCarteMain.Valeur - 2, enumCouleur),
                                                                                                                       objCarteMain }));
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte> { Rummy.Carte(objCarteMain.Valeur - 1, enumCouleur),
                                                                                                                       objJoker,
                                                                                                                       objCarteMain }));

                                                        break;
                                                    default:
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte> { Rummy.Carte(objCarteMain.Valeur - 1, enumCouleur),
                                                                                                                       objCarteMain,
                                                                                                                       objJoker }));
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte> { objJoker,
                                                                                                                       objCarteMain,
                                                                                                                       Rummy.Carte(objCarteMain.Valeur + 1, enumCouleur) }));
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte> { objCarteMain,
                                                                                                                       objJoker,
                                                                                                                       Rummy.Carte(objCarteMain.Valeur + 2, enumCouleur) }));
                                                        lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte> { objCarteMain,
                                                                                                                       Rummy.Carte(objCarteMain.Valeur + 1, enumCouleur),
                                                                                                                       objJoker }));

                                                        break;
                                                }
                                            else
                                                lstCombinaisonsPossibles.Add(new Combinaison(new List<Carte>{ objCarteMain,
                                                                                                              Rummy.Carte(objCarteMain.Valeur, enumCouleur), 
                                                                                                              objJoker}));

                                foreach (Combinaison objCombinaisonPossible in lstCombinaisonsPossibles.ToList())
                                    foreach (Carte objCarteTmp in objCombinaisonPossible.CartesCombinaison)
                                        // Si la carte ne provient pas de la main du joueur
                                        if (objCarteTmp.ControleParentOrigine != eControleOrigine.zoneJoueur)
                                        {
                                            Carte objCarte = RechercherCarte(objCarteTmp);
                                            if (objCarte != null)
                                                // On a trouvé une carte correspondant à la carte présente
                                                // dans la main du joueur
                                                // On va donc tenter de diviser la combinaison de la zone de jeu
                                                // afin de pouvoir insérer une seconde carte de même valeur,
                                                // à condition qu'il s'agisse bien d'une suite de couleur de plus
                                                // de 5 cartes
                                                if (objCombinaison.NbCartes == 1)
                                                {
                                                    if (objCarte.Position > 2 &&
                                                       objCarte.Position < objCarte.Combinaison.NbCartes - 3 &&
                                                        objCarte.Combinaison.Type == eTypeCombinaison.suitecouleurs)
                                                    {
                                                        Combinaison objCombinaisonJeu = objCarte.Combinaison;
                                                        GestionErreurs.Tracer("Division de " + objCombinaisonJeu);
                                                        Combinaison objNouvelleCombinaison = ZoneJeu.DiviserCombinaison(objCarte, false);
                                                        if (objNouvelleCombinaison != null)
                                                        {
                                                            GestionErreurs.Tracer("Résultat : " + objNouvelleCombinaison);

                                                            // Il faut déterminer dans quelle combinaison se trouve la carte recherchée
                                                            if (objCombinaisonJeu.Carte(objCarte) == null)
                                                            {
                                                                int intPosition = -1;
                                                                if (objCarteMain.Valeur < objCombinaisonJeu.CartesCombinaison.First().Valeur)
                                                                    intPosition = 0;

                                                                GestionErreurs.Tracer("Ajout de " + objCarteMain + " à " + objCombinaisonJeu + " à la position " + intPosition);
                                                                Jouer(objCarteMain, objCombinaisonJeu, intPosition);
                                                                if (!objCombinaisonJeu.Verifier())
                                                                    TracerContexte(false, true);
                                                            }
                                                            else
                                                            {
                                                                GestionErreurs.Tracer("Ajout de " + objCarteMain + " en début de " + objNouvelleCombinaison);
                                                                Jouer(objCarteMain, objNouvelleCombinaison, 0);
                                                                if (!objNouvelleCombinaison.Verifier())
                                                                    TracerContexte(false, true);
                                                            }

                                                            boolRes = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    GestionErreurs.Tracer("Utilisation de " + objCarte);
                                                    objCarte = ZoneJeu.SupprimerCarte(objCarte);
                                                    int intPosition = 0;
                                                    foreach (Carte objCarteCombinaison in objCombinaisonPossible.CartesCombinaison)
                                                        if (objCarteCombinaison.ControleParentOrigine == eControleOrigine.zoneJoueur)
                                                        {
                                                            objCombinaisonPossible.Supprimer(objCarteCombinaison);
                                                            objCombinaisonPossible.Ajouter(objCarte, intPosition);
                                                        }

                                                    GestionErreurs.Tracer("Dépose de " + objCombinaisonPossible);
                                                    Jouer(objCombinaisonPossible);
                                                    if (!objCombinaisonPossible.Verifier())
                                                        TracerContexte(false, true);

                                                    boolRes = true;
                                                }
                                        }

                                break;
                        }

                    if (boolSortirFor)
                        break;
                }

                return boolRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
            finally
            {
                GestionErreurs.Tracer("Fin de compléter Niveau 2");
            }
        }

        private bool CompleterNiveau3(ref List<Carte> jokers)
        {
            try
            {
                bool boolRes = CompleterNiveau2(ref jokers);

                //'Dim enumValeur1 As eValeur = eValeur.Indetermine
                //'Dim enumValeur2 As eValeur = eValeur.Indetermine
                //'Dim enumCouleur As eCouleur = eCouleur.Indetermine

                //'With objCombinaison.Cartes.First
                //'    if (.Valeur = eValeur.Joker)
                //'        With .ValeursRemplacees.First
                //'            enumValeur1 = .Valeur
                //'            enumCouleur = .Couleur
                //'        End With
                //'    else
                //'        enumValeur1 = .Valeur
                //'        enumCouleur = .Couleur
                //'    End If
                //'End With

                //'With objCombinaison.Cartes.Last
                //'    if (.Valeur = eValeur.Joker)
                //'        With .ValeursRemplacees.First
                //'            enumValeur2 = .Valeur
                //'        End With
                //'    else
                //'        enumValeur2 = .Valeur
                //'    End If
                //'End With

                //'if (enumValeur1 > eValeur.Un && enumValeur2 < eValeur.Treize)
                //'    Dim objCombinaisonTmp2 As Combinaison = objCombinaison.Clone
                //'    objCombinaisonTmp2.CartesManquantes = new List(Of Carte)({Rummy.Carte(CType(enumValeur1 - 1, eValeur), enumCouleur), _
                //'                                                              Rummy.Carte(CType(enumValeur2 + 1, eValeur), enumCouleur)})

                //'    lstCombinaisonsPotentielles.Add(objCombinaisonTmp2)
                //'End If

                //'if (enumValeur1 > eValeur.Deux)
                //'    Dim objCombinaisonTmp2 As Combinaison = objCombinaison.Clone
                //'    objCombinaisonTmp2.CartesManquantes = new List(Of Carte)({Rummy.Carte(CType(enumValeur1 - 2, eValeur), enumCouleur), _
                //'                                                              Rummy.Carte(CType(enumValeur1 - 1, eValeur), enumCouleur)})

                //'    lstCombinaisonsPotentielles.Add(objCombinaisonTmp2)
                //'End If

                //'if (enumValeur2 < eValeur.Douze)
                //'    Dim objCombinaisonTmp2 As Combinaison = objCombinaison.Clone
                //'    objCombinaisonTmp2.CartesManquantes = new List(Of Carte)({Rummy.Carte(CType(enumValeur2 + 1, eValeur), enumCouleur), _
                //'                                                              Rummy.Carte(CType(enumValeur2 + 2, eValeur), enumCouleur)})

                //'    lstCombinaisonsPotentielles.Add(objCombinaisonTmp2)
                //'End If

                return boolRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        #endregion

        #region Classe assurant le tri par ordre décroissant de valeur

        private class CComparerValeurs : IComparer<eValeur>
        {
            public int Compare(eValeur v1, eValeur v2)
            {
                return v2 - v1;
            }
        }

        private class CComparerCombinaisons : IComparer<Combinaison>
        {
            public int Compare(Combinaison c1, Combinaison c2)
            {
                int intTmp = c1.NbCartes - c2.NbCartes;

                if (intTmp < 0)
                    return 1;
                else if (intTmp > 0)
                    return -1;
                else
                    return c2.NbPoints - c1.NbPoints;
            }
        }

        #endregion

    }
}
