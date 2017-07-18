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
        private ICombinaison objMainJoueur;
        private List<Carte> lstJokersMain;
        private List<ICombinaison> lstCombinaisonsZoneJeu;

        private List<ICombinaison> lstCombinaisonsPotentielles;

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

                GestionErreurs.Tracer("Main du joueur : " + objMainJoueur.ToString());
                GestionErreurs.Tracer("Combinaisons déposées :");
                foreach (Combinaison objCombinaisonJeu in lstCombinaisonsZoneJeu)
                    GestionErreurs.Tracer(objCombinaisonJeu.ToString());
            }
            catch (Exception)
            {
            }
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

                lstCombinaisonsPotentielles = new List<ICombinaison>();
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

                        dicValeursCouleurs[objCarte.Valeur].Add(objCarte.Couleur);
                        dicCouleursValeurs[objCarte.Couleur].Add(objCarte.Valeur);

                        lstCombinaisonsPotentielles.Add(new Combinaison(new List<Carte>(new Carte[] { objCarte })));
                    }
                }

                foreach (eCouleur enumCouleur in dicCouleursValeurs.Keys.ToList())
                    if (!ChercherSuitesCouleur(enumCouleur, dicCouleursValeurs[enumCouleur]))
                        break;

                foreach (eValeur enumValeur in dicValeursCouleurs.Keys.ToList())
                    lstCombinaisonsPotentielles.AddRange(ChercherSuitesValeur(enumValeur,
                                                                              dicValeursCouleurs[enumValeur]));

                lstCombinaisonsPotentielles.Sort(new CComparerCombinaisons());

                // Calcul du nombre de points requis pour déposer des cartes sur la zone de jeu
                int intNbPoints = 0;

                bool boolADeposeCartes = false;
                List<Combinaison> lstCombinaisonsAJouer = new List<Combinaison>();

                List<Carte> lstVide = null;

                foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles.ToList())
                    if (objCombinaisonTmp.NbCartes < 3)
                        /* Les combinaisons étant triées par nombre décroissant de cartes,
                         * on s'arrête dès que l'une d'elles n'a pas pu être complétée par un joker */
                        break;
                    else
                    {
                        // Vérification que la main contient bien toutes les cartes nécessaires à sa réalisation
                        Combinaison objCombinaison = CreerCombinaison(objCombinaisonTmp,
                                                                      false,
                                                                      ref lstVide);

                        int intPositionJoker = objCombinaison.PositionJoker;

                        if (objCombinaison.Verifier())
                        {
                            // Suppression des cartes utilisées et calcul du nombre de points qu'il est possible de déposer
                            foreach (Carte objCarte in objCombinaison.CartesCombinaison)
                            {
                                if (objCarte.Valeur == eValeur.Joker)
                                    intNbPoints += (int)objCarte.ValeurRemplacee;
                                else
                                    intNbPoints += (int)objCarte.Valeur;

                                objMainJoueur.Supprimer(objCarte);

                                lstCombinaisonsPotentielles.Remove(objCombinaison);
                            }
                        }
                        else if (intPositionJoker > -1)
                        {
                            Carte objJoker = objCombinaison.CartesCombinaison[intPositionJoker];

                            if (objJoker.Couleur != eCouleur.Indetermine)
                            {
                                // Rajout du joker dans la liste des cartes constituant la main du joueur
                                lstJokersMain.Add(objJoker);

                                // Réinitialisation de la couleur du joker présent dans la combinaison
                                objCombinaison.CartesCombinaison[intPositionJoker] = new Carte(eValeur.Joker,
                                                                                               eCouleur.Indetermine,
                                                                                               Rummy.Representation);
                            }
                        }
                    }

                bool boolZoneJeuCompletee = false;
                if (boolADeposePointsMinimum || intNbPoints >= Rummy.NB_POINTS_MINIMUM)
                {
                    boolADeposePointsMinimum = true;

                    foreach (Combinaison objCombinaison in lstCombinaisonsAJouer)
                    {
                        Jouer(objCombinaison);
                        boolADeposeCartes = true;
                    }

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
                                    boolZoneJeuCompletee = CompleterNiveau0(ref lstJokersZoneJeu);
                                    break;
                                }
                            case eNiveauDifficulte.Facile:
                                {
                                    boolZoneJeuCompletee = CompleterNiveau1(ref lstJokersZoneJeu);
                                    break;
                                }
                            case eNiveauDifficulte.Moyen:
                                {
                                    boolZoneJeuCompletee = CompleterNiveau2(ref lstJokersZoneJeu);
                                    break;
                                }
                            case eNiveauDifficulte.Difficile:
                                {
                                    boolZoneJeuCompletee = CompleterNiveau3(ref lstJokersZoneJeu);
                                    break;
                                }
                        }

                        /* Tentative de completion de combinaisons potentielles incomplètes avec les jokers
                         * éventuellement récoltés sur la zone de jeu */
                        int intNbJokersPrec = lstJokersZoneJeu.Count;
                        while (lstJokersZoneJeu.Count > 0)
                        {
                            foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles.ToList())
                            {
                                /* Remplacement des cartes "temporaires" par les véritables instances de cartes
                                 * constituant la main du joueur */
                                Combinaison objCombinaison = CreerCombinaison(objCombinaisonTmp, false, ref lstVide);

                                if (objCombinaison.NbCartes == 2)
                                {
                                    ICombinaison objCombinaisonJoker = lstJokersZoneJeu.First().Combinaison;
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
                                        foreach (Carte objCarte in objCombinaison.CartesCombinaison)
                                        {
                                            if (objCarte.Valeur != eValeur.Joker)
                                                Jouer(objCarte, objCombinaisonJoker, intPosition);

                                            intPosition++;
                                        }
                                        lstJokersZoneJeu.RemoveAt(0);
                                        lstCombinaisonsPotentielles.Remove(objCombinaison);

                                        boolZoneJeuCompletee = true;

                                        break;
                                    }
                                }
                                else
                                    break;
                            }

                            // Si malgré cela il reste des jokers implacés, on essaye de les fourguer sur la zone de jeu
                            if (lstJokersZoneJeu.Count > 0)
                                foreach (Combinaison objCombinaisonTmp in lstCombinaisonsZoneJeu.ToList())
                                {
                                    ICombinaison objCombinaison = objCombinaisonTmp;

                                    if (objCombinaison.NbCartes < 3)
                                        lstCombinaisonsZoneJeu.Remove(objCombinaison);
                                    else
                                    {
                                        if (!objCombinaison.Verifier())
                                            GestionErreurs.Tracer(objCombinaison.ToString(), true);

                                        if (objCombinaison.PositionJoker == -1 &&
                                           (objCombinaison.TypeCombinaison == eTypeCombinaison.SuiteCouleurs ||
                                            objCombinaison.NbCartes < 4))
                                        {
                                            Carte objJoker = lstJokersZoneJeu.First();
                                            if (objCombinaison.TypeCombinaison == eTypeCombinaison.SuiteValeurs ||
                                               objCombinaison.CartesCombinaison.Last().Valeur < eValeur.Treize)
                                            {
                                                GestionErreurs.Tracer(String.Format("Débarras de {0} en fin de {1}",
                                                                                    objJoker.ToString(),
                                                                                    objCombinaison.ToString()));
                                                Jouer(objJoker, objCombinaison);
                                            }
                                            else
                                            {
                                                GestionErreurs.Tracer(String.Format("Débarras de {0} en début de {1}",
                                                                                    objJoker.ToString(),
                                                                                    objCombinaison.ToString()));
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

                if (boolADeposeCartes || boolZoneJeuCompletee)
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

        private bool ChercherSuitesCouleur(eCouleur couleur, List<eValeur> valeurs)
        {
            try
            {
                Combinaison objCombinaison = null;
                eValeur enumValeurPrec = eValeur.Indetermine;

                foreach (eValeur enumValeur in valeurs)
                    if (enumValeurPrec == eValeur.Indetermine)
                    {
                        Carte objCarte = Rummy.Carte(enumValeur, couleur);
                        objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                        objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;
                        objCombinaison = new Combinaison(new List<Carte>(new Carte[] { objCarte }));

                        enumValeurPrec = enumValeur;
                    }
                    else if (enumValeur == enumValeurPrec + 1)
                    {
                        Carte objCarte = Rummy.Carte(enumValeur, couleur);
                        objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                        objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;
                        objCombinaison.Ajouter(objCarte);

                        enumValeurPrec = enumValeur;
                    }
                    else if (objCombinaison.PositionJoker == -1)
                    {
                        Carte objJoker = CreerJoker(enumValeurPrec + 1, couleur);

                        if (enumValeur == enumValeurPrec + 2)
                        {
                            objCombinaison.Ajouter(objJoker);
                            Carte objCarte = Rummy.Carte(enumValeur, couleur);
                            objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                            objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;
                            objCombinaison.Ajouter(objCarte);

                            enumValeurPrec = enumValeur;
                        }
                        else
                        {
                            if (objCombinaison.NbCartes == 2)
                            {
                                // Complétion de la combinaison avec un joker en fin de combinaison
                                if (enumValeurPrec < eValeur.Treize)
                                {
                                    ICombinaison objCombinaisonTmp = objCombinaison.Clone();
                                    objCombinaisonTmp.Ajouter(objJoker.Clone());

                                    AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaisonTmp);
                                }

                                // Complétion de la combinaison avec un joker en début de combinaison
                                Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();
                                if (objPremiereCarte.Valeur > eValeur.Un)
                                {
                                    ICombinaison objCombinaisonTmp = objCombinaison.Clone();
                                    objJoker.ReinitialiserCartesRemplacees();
                                    objJoker.AjouterCarteRemplacee(objPremiereCarte.Valeur - 1, couleur);
                                    objCombinaisonTmp.Ajouter(objJoker, 0);

                                    AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaisonTmp);
                                }
                            }
                            else
                                AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaison);

                            // Préparation pour une nouvelle suites de couleur
                            Carte objCarte = Rummy.Carte(enumValeur, couleur);
                            objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                            objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;
                            objCombinaison = new Combinaison(new List<Carte>(new Carte[] { objCarte }));

                            enumValeurPrec = enumValeur;
                        }
                    }
                    else
                    {
                        AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaison);

                        Carte objCarte = Rummy.Carte(enumValeur, couleur);
                        objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                        objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;
                        objCombinaison = new Combinaison(new List<Carte>(new Carte[] { objCarte }));

                        enumValeurPrec = enumValeur;
                    }

                if (objCombinaison.NbCartes == 2)
                {
                    // Ultime complétion de la combinaison avec un joker en fin de combinaison
                    Carte objDerniereCarte = objCombinaison.CartesCombinaison.Last();
                    if (objDerniereCarte.Valeur < eValeur.Treize)
                    {
                        ICombinaison objCombinaisonTmp = objCombinaison.Clone();
                        Carte objJoker = CreerJoker(objDerniereCarte.Valeur + 1, couleur);
                        objCombinaisonTmp.Ajouter(objJoker);

                        AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaisonTmp);
                    }

                    // Ultime complétion de la combinaison avec un joker en début de combinaison
                    Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();
                    if (objPremiereCarte.Valeur > eValeur.Un)
                    {
                        ICombinaison objCombinaisonTmp = objCombinaison.Clone();
                        Carte objJoker = CreerJoker(objPremiereCarte.Valeur - 1, couleur);
                        objCombinaisonTmp.Ajouter(objJoker, 0);

                        AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaisonTmp);
                    }
                }
                else
                    AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaison);


                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        private void AjouterSuiteCouleurCombinaisonsPotentielles(ICombinaison combinaison)
        {
            try
            {
                if (combinaison.NbCartes < 2)
                    // Aucune carte présente dans la combinaison
                    return;
                else
                {
                    ICombinaison objCombinaisonTmp = combinaison.Clone();
                    Combinaison objCombinaison = null;

                    foreach (Carte objCarte in objCombinaisonTmp.CartesCombinaison)
                    {
                        if (objCombinaison == null)
                            objCombinaison = new Combinaison(new List<Carte>(new Carte[] { objCarte.Clone() }));
                        else
                        {
                            objCombinaison.Ajouter(objCarte.Clone());

                            if (objCombinaison.NbCartes > 2)
                                lstCombinaisonsPotentielles.Add(objCombinaison);
                            else if (objCombinaison.PositionJoker == -1)
                            {
                                Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();
                                if (objPremiereCarte.Valeur > eValeur.Un)
                                {
                                    ICombinaison objCombinaisonTmp2 = objCombinaison.Clone();
                                    Carte objJoker = CreerJoker(objPremiereCarte.Valeur - 1, objPremiereCarte.Couleur);
                                    objCombinaisonTmp2.Ajouter(objJoker, 0);
                                    lstCombinaisonsPotentielles.Add(objCombinaisonTmp2);
                                }

                                Carte objDerniereCarte = objCombinaison.CartesCombinaison.Last();
                                if (objDerniereCarte.Valeur < eValeur.Treize)
                                {
                                    ICombinaison objCombinaisonTmp2 = objCombinaison.Clone();
                                    Carte objJoker = CreerJoker(objDerniereCarte.Valeur + 1, objDerniereCarte.Couleur);
                                    objCombinaisonTmp2.Ajouter(objJoker);
                                    lstCombinaisonsPotentielles.Add(objCombinaisonTmp2);
                                }
                            }
                        }

                        objCombinaisonTmp.Supprimer(objCarte);
                        if (objCombinaisonTmp.NbCartes > 2)
                            AjouterSuiteCouleurCombinaisonsPotentielles(objCombinaisonTmp);
                    }
                }
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        /// <summary>
        /// Réorganise les cartes de même valeur en suite de valeurs
        /// </summary>
        /// <param name="valeur">Valeur de la carte</param>
        /// <param name="couleurs">Liste des couleurs disponibles pour cette valeur</param>
        /// <returns>Renvoie la liste des combinaisons possibles</returns>
        private List<Combinaison> ChercherSuitesValeur(eValeur valeur, List<eCouleur> couleurs)
        {
            try
            {
                /* Etant donné que chaque carte existe en double dans le jeu, il est possible de
                 * réaliser des suites de valeur incluant deux fois la "même" carte */
                Combinaison objCombinaison1 = new Combinaison();
                Combinaison objCombinaison2 = new Combinaison();
                eCouleur enumCouleurPrec = eCouleur.Indetermine;

                foreach (eCouleur enumCouleur in couleurs)
                {
                    Carte objCarte = Rummy.Carte(valeur, enumCouleur);
                    objCarte.ControleParentActuel = eControleOrigine.zoneJoueur;
                    objCarte.ControleParentOrigine = eControleOrigine.zoneJoueur;

                    if (enumCouleurPrec == eCouleur.Indetermine)
                    {
                        objCombinaison1.Ajouter(objCarte);
                        enumCouleurPrec = enumCouleur;
                    }
                    else if (objCombinaison1.Carte(valeur, enumCouleur) != null)
                        objCombinaison2.Ajouter(objCarte);
                    else
                        objCombinaison1.Ajouter(objCarte);
                }

                if (objCombinaison1.NbCartes == 2)
                    objCombinaison1.Ajouter(CreerJoker());

                if (objCombinaison2.NbCartes == 2)
                    objCombinaison2.Ajouter(CreerJoker());

                List<Combinaison> lstRes = new List<Combinaison>();
                if (objCombinaison1.NbCartes > 0)
                {
                    objCombinaison1.Verifier(false);
                    lstRes.Add(objCombinaison1);
                }

                if (objCombinaison2.NbCartes > 0)
                {
                    objCombinaison2.Verifier(false);
                    lstRes.Add(objCombinaison2);
                }

                return lstRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return new List<Combinaison>();
            }
        }

        private Carte CreerJoker(eValeur valeurRemplacee = eValeur.Indetermine, eCouleur couleurRemplacee = eCouleur.Indetermine)
        {
            Carte objJoker = Rummy.Carte(eValeur.Joker, eCouleur.Indetermine);
            objJoker.ControleParentActuel = eControleOrigine.zoneJoueur;
            objJoker.ControleParentOrigine = eControleOrigine.zoneJoueur;

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
                Carte objCarteAbsente = null;

                GestionErreurs.Tracer("Création de " + combinaison.ToString() + ", avec :");
                GestionErreurs.Tracer(objMainJoueur.ToString());

                foreach (Carte objJoker in lstJokersMain)
                    GestionErreurs.Tracer(objJoker.ToString());

                if (jokers != null)
                    foreach (Carte objJoker in jokers)
                        GestionErreurs.Tracer(objJoker.ToString());

                foreach (Carte objCarte in combinaison.CartesCombinaison)
                {
                    Carte objCarteMain = CarteMainJoueur(objCarte);

                    if (objCarteMain != null)
                        objRes.Ajouter(objCarteMain.Clone());
                    else if (objCarte.Valeur == eValeur.Joker)
                        if (!(jokers == null || jokers.Count == 0))
                            objRes.Ajouter(jokers.First().Clone());
                        else if (lstJokersMain.Count > 0)
                            objRes.Ajouter(lstJokersMain.First().Clone());
                        else
                        {
                            objCarteAbsente = objCarte;

                            // Il n'y a pas de joker pour compléter la combinaison
                            break;
                        }
                    else
                    {
                        if (objRes.PositionJoker > -1)
                        {
                            // Reconstruction de la liste des jokers
                            Carte objJoker = objRes.CartesCombinaison[objRes.PositionJoker];
                            if (objJoker.VientDeZoneJoueur)
                                lstJokersMain.Insert(0, objJoker);
                            else
                                jokers.Insert(0, ReinitialiserJokerJeu(objJoker));
                        }

                        objCarteAbsente = objCarte;

                        // Il n'y a pas de joker pour compléter la combinaison
                        break;
                    }
                }

                if (!objRes.Verifier(verifierNbCartesCombinaison))
                {
                    if (combinaison.NbCartes > 2 || jokers.Count > 0)
                    {
                        GestionErreurs.Tracer("Echec de la construction de " + combinaison.ToString());
                        if (objCarteAbsente != null)
                            GestionErreurs.Tracer("Il manque : " + objCarteAbsente.ToString());
                    }

                    objRes = new Combinaison();
                }
                else
                    // Suppression de la main du joueur des cartes utilisées
                    foreach (Carte objCarteTmp in objRes.CartesCombinaison)
                        if (objCarteTmp.Valeur == eValeur.Joker)
                        {
                            // Deux possibilités de source pour le joker :
                            if (objCarteTmp.VientDeZoneJoueur)
                            {
                                objRes.CartesCombinaison[objCarteTmp.Position] = lstJokersMain.First();
                                lstJokersMain.RemoveAt(0);
                            }
                            else
                                jokers.RemoveAt(0);
                        }
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
            GestionErreurs.Tracer("Jouer (" + combinaison.ToString() + ")");

            // Dépose de toutes les cartes de la combinaison sur la zone de jeu
            ICombinaison objCombinaison = null;
            foreach (Carte objCarteTmp in combinaison.CartesCombinaison)
            {
                /* On s'assure que la carte qui est déposée sur la zone de jeu
                 * soit bien celle qui provient de la main du joueur et non de son clone */
                Carte objCarte = null;
                if (objCarteTmp.ControleParentOrigine == eControleOrigine.zoneJoueur)
                    objCarte = CarteMainJoueur(objCarteTmp, true);
                else
                    objCarte = objCarteTmp;

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
        private Carte Jouer(Carte carte, ICombinaison combinaison, int position = -1)
        {
            try
            {
                Carte objRes = null;

                GestionErreurs.Tracer(String.Format("Jouer ({0}, {1}, {2})",
                                                    carte.ToString(),
                                                    combinaison.ToString(),
                                                    position.ToString()));

                /* On s'assure que la carte qui est déposée sur la zone de jeu
                 * soit bien celle qui provient de la main du joueur et non de son clone */
                Carte objCarte = null;
                if (carte.ControleParentOrigine == eControleOrigine.zoneJoueur)
                    objCarte = CarteMainJoueur(carte, true);
                else
                    objCarte = carte;

                if (position == -1)
                    ZoneJeu.DeposerCarte(ref objCarte, combinaison);
                else
                {
                    Carte objCarteDest = combinaison.CartesCombinaison[position];
                    objRes = ZoneJeu.DeposerCarte(ref objCarte, objCarteDest);
                }

                // On supprime la carte du clone de la main du joueur
                objMainJoueur.Supprimer(carte);

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        private Carte UtiliserCarte(Carte carte)
        {
            try
            {
                GestionErreurs.Tracer(String.Format("Tentative d'utilisation de {0} ({1})",
                                                    carte.ToString(),
                                                    carte.Combinaison.ToString()));

                Carte objRes = null;

                ICombinaison objCombinaison = carte.Combinaison;

                if (objCombinaison == null)
                    return null;

                if (objCombinaison.NbCartes > 1 &&
                    objCombinaison.TypeCombinaison == eTypeCombinaison.SuiteCouleurs)
                    if (carte.Position == 0)
                        objCombinaison.Diviser(carte, false);
                    else if (carte.Position == carte.Combinaison.NbCartes - 1)
                        objCombinaison.Diviser(carte, true);
                    else
                    {
                        ICombinaison objCombinaisonDivisee = objCombinaison.Diviser(carte, false);
                        if (carte.Position == 0)
                            objCombinaison.Diviser(carte, false);
                        else
                            objCombinaison.Diviser(carte, true);
                    }

                objRes = carte;

                GestionErreurs.Tracer(String.Format("Résultat : {0} ({1})",
                                                    objRes.ToString(),
                                                    objCombinaison.ToString()));

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        private Carte DeplacerCarteJeu(Carte carte,
                                       ICombinaison combinaisonDest,
                                       int position = -1)
        {
            try
            {
                GestionErreurs.Tracer(String.Format("Tentative de déplacement de {0} vers {1} en position {2}",
                                                    carte.ToString(),
                                                    combinaisonDest.ToString(),
                                                    position.ToString()));

                Carte objRes = null;

                carte = UtiliserCarte(carte);

                if (carte != null)
                    return combinaisonDest.Ajouter(carte, position, true);

                GestionErreurs.Tracer("Résultat : " + combinaisonDest.ToString());

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
            if (carte.ControleParentOrigine == eControleOrigine.zoneJoueur)
                if (mainOriginale)
                    return base.MainJoueur.Carte(carte);
                else
                    return objMainJoueur.Carte(carte);
            else
                return carte;
        }

        /// <summary>
        /// Réinitialise les propriétés du joker, afin de pouvoir le réutiliser par la suite
        /// </summary>
        /// <param name="joker">Joker à réinitialiser</param>
        /// <returns>Renvoie le joker réinitialisé</returns>
        private Carte ReinitialiserJokerJeu(Carte joker)
        {
            joker.ReinitialiserCartesRemplacees();
            joker.Supprimer();

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
                        switch (objCombinaisonJeu.TypeCombinaison)
                        {
                            case eTypeCombinaison.SuiteValeurs:
                                {
                                    if (objCombinaisonJeu.NbCartes > 3)
                                        lstCartes.Add(objCarte);

                                    break;
                                }
                            case eTypeCombinaison.SuiteCouleurs:
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
                    ICombinaison objCombinaison1 = objCarte1.Combinaison;
                    ICombinaison objCombinaison2 = objCarte2.Combinaison;

                    if (objCombinaison1.TypeCombinaison == eTypeCombinaison.SuiteCouleurs &&
                        objCombinaison1.TypeCombinaison == objCombinaison2.TypeCombinaison)
                        if (objCarte1.Position == 0 && objCarte2.Position == objCombinaison2.NbCartes - 1)
                        {
                            GestionErreurs.Tracer("Division de " + objCombinaison1.ToString());
                            objRes = ZoneJeu.DiviserCombinaison(objCarte1, false).CartesCombinaison.First();
                            foreach (Carte objCarteTmp in objCombinaison1.CartesCombinaison)
                                DeplacerCarteJeu(objCarteTmp, objCombinaison2);

                            GestionErreurs.Tracer("Obtention de " + objCombinaison2.ToString());
                        }
                        else if (objCarte2.Position == 0 && objCarte1.Position == objCombinaison1.NbCartes - 1)
                        {
                            GestionErreurs.Tracer("Division de " + objCombinaison2.ToString());
                            objRes = ZoneJeu.DiviserCombinaison(objCarte2, false).CartesCombinaison.First();
                            foreach (Carte objCarteTmp in objCombinaison2.CartesCombinaison)
                                DeplacerCarteJeu(objCarteTmp, objCombinaison1);

                            GestionErreurs.Tracer("Obtention de " + objCombinaison1.ToString());
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

                foreach (Combinaison objCombinaisonTmp in lstCombinaisonsZoneJeu.ToList())
                {
                    ICombinaison objCombinaison = objCombinaisonTmp;

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
                                                                    objCarte.ToString(),
                                                                    objCombinaison.ToString(),
                                                                    intPositionJoker.ToString()));
                                objJoker = Jouer(objCarte, objCombinaison, intPositionJoker);
                                if (objJoker != null)
                                {
                                    GestionErreurs.Tracer("Obtention de " + objJoker.ToString());
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
                                                                        objCarteMain.ToString(),
                                                                        objCombinaison.ToString(),
                                                                        intPositionJoker.ToString()));
                                    objJoker = Jouer(objCarteMain, objCombinaison, intPositionJoker);
                                    objMainJoueur.Supprimer(objCarteMain);
                                    jokers.Add(ReinitialiserJokerJeu(objJoker));
                                    GestionErreurs.Tracer("Obtention de " + objJoker.ToString());
                                    boolRes = true;
                                    break;
                                }
                            }
                        }
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

        private bool CompleterNiveau1(ref List<Carte> jokers)
        {
            GestionErreurs.Tracer("Début de compléter Niveau 1");

            try
            {
                bool boolTourFini = true;

                bool boolRes = false;

                // - - - - - - PRIS DEPUIS LA VERSION VB.NET LA PLUS RECENTE ! - - - - - //

                //do
                //{
                //    boolTourFini = true;

                //    if (CompleterNiveau0(ref jokers))
                //        boolRes = true;

                //    if (jokers.Count > 0)
                //    {
                //        GestionErreurs.Tracer("Jokers obtenus :");
                //        foreach (Carte objJoker in jokers)
                //            GestionErreurs.Tracer(objJoker.ToString());

                //        /* Si l'on a réussi à récupérer des jokers de la zone de jeu, il faut voir
                //         * si l'on ne peut pas compléter des combinaisons de la main du joueur */
                //        foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles.ToList())
                //        {
                //            Combinaison objCombinaison = objCombinaisonTmp;

                //            if (objCombinaison.NbCartes == 1)
                //                // On ne peut décemment pas compléter de combinaisons ne possédant qu'une seule carte
                //                break;
                //            else if (objCombinaison.NbCartes == 2 && objCombinaison.PositionJoker == -1 && jokers.Count > 0)
                //                // Ajout d'un joker à la combinaison de cartes
                //                if (objCombinaison.TypeCombinaison == eTypeCombinaison.SuiteCouleurs &&
                //                   objCombinaison.CartesCombinaison[objCombinaison.NbCartes - 1].Valeur == eValeur.Treize)
                //                    /* Cas particulier d'une suite de couleur, dont la dernière carte est un 13 :
                //                     * il faut ajouter le joker en début de combinaison */
                //                    objCombinaison.Ajouter(jokers.First(), 0);
                //                else
                //                    // La position du joker dans tous les autres cas est sans importance
                //                    objCombinaison.Ajouter(jokers.First());

                //            GestionErreurs.Tracer("Tentative de complétion de " + objCombinaison.ToString() + " avec un joker");
                //            objCombinaison = CreerCombinaison(objCombinaison, true, ref jokers);

                //            if (objCombinaison != null && objCombinaison.NbCartes > 0)
                //            {
                //                GestionErreurs.Tracer("Combinaison obtenue : " + objCombinaison.ToString());

                //                if (objCombinaison.Verifier())
                //                {
                //                    GestionErreurs.Tracer("Placement sur le jeu de " + objCombinaison.ToString());
                //                    Jouer(objCombinaison);
                //                }
                //            }
                //        }
                //    }
                //} while (!boolTourFini);

                // - - - - - - PRIS DEPUIS LA VERSION VB.NET LA PLUS RECENTE ! - - - - - //

                do
                {
                    boolTourFini = true;

                    if (CompleterNiveau0(ref jokers))
                        boolRes = true;

                    if (jokers.Count > 0)
                    {
                        GestionErreurs.Tracer("Jokers obtenus :");
                        foreach (Carte objJoker in jokers)
                            GestionErreurs.Tracer(objJoker.ToString());

                        /* Si l'on a réussi à récupérer des jokers de la zone de jeu, il faut voir
                         * si l'on ne peut pas compléter des combinaisons de la main du joueur */
                        foreach (Combinaison objCombinaisonTmp in lstCombinaisonsPotentielles.ToList())
                        {
                            GestionErreurs.Tracer("Tentative de complétion de " + objCombinaisonTmp.ToString() + " avec un joker");
                            Combinaison objCombinaison = CreerCombinaison(objCombinaisonTmp, true, ref jokers);
                            if (objCombinaison.Verifier())
                            {
                                GestionErreurs.Tracer("Placement sur le jeu de " + objCombinaison.ToString());
                                Jouer(objCombinaison);
                            }
                            else
                                GestionErreurs.Tracer("Echec de la construction de la combinaison.");
                        }
                    }

                    foreach (Carte objCarte in objMainJoueur.CartesCombinaison.ToList())
                    {
                        // On se réserve quand même les jokers
                        if (objCarte.Valeur != eValeur.Joker)
                        {
                            bool boolSortirFor = false;

                            foreach (Combinaison objCombinaisonTmp in lstCombinaisonsZoneJeu)
                            {
                                ICombinaison objCombinaison = objCombinaisonTmp;

                                if (objCombinaison.NbCartes > 0)
                                {
                                    switch (objCombinaison.TypeCombinaison)
                                    {
                                        case eTypeCombinaison.SuiteValeurs:
                                            {
                                                // 1ère possibilité : suite de valeurs
                                                try
                                                {
                                                    if ((objCarte.Valeur == objCombinaison.CartesCombinaison.First().Valeur ||
                                                         objCarte.Valeur == objCombinaison.CartesCombinaison.Last().Valeur) &&
                                                        objCombinaison.Carte(objCarte) == null)
                                                    {
                                                        GestionErreurs.Tracer(String.Format("Ajout de {0} à la fin de {1}",
                                                                                            objCarte.ToString(),
                                                                                            objCombinaison.ToString()));
                                                        Jouer(objCarte, objCombinaison);
                                                        if (objCombinaison.Verifier())
                                                            objMainJoueur.Supprimer(objCarte);
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
                                        case eTypeCombinaison.SuiteCouleurs:
                                            {
                                                // 2nde possibilité : suite de couleurs
                                                try
                                                {
                                                    if (objCombinaison.CartesCombinaison.First().Valeur == eValeur.Joker)
                                                    {
                                                        Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();
                                                        if (objPremiereCarte.PeutRemplacer(objCarte.Valeur + 1, objCarte.Couleur))
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en début de {1}",
                                                                                                objCarte.ToString(),
                                                                                                objCombinaison.ToString()));
                                                            Jouer(objCarte, objCombinaison, 0);
                                                            if (objCombinaison.Verifier())
                                                                objMainJoueur.Supprimer(objCarte);
                                                            else
                                                                TracerContexte(false, true);

                                                            boolRes = true;
                                                            boolTourFini = false;
                                                            boolSortirFor = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Carte objPremiereCarte = objCombinaison.CartesCombinaison.First();
                                                        if (objCarte.Couleur == objPremiereCarte.Couleur &&
                                                            objCarte.Valeur == objPremiereCarte.Valeur - 1)
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en début de {1}",
                                                                                                objCarte.ToString(),
                                                                                                objCombinaison.ToString()));
                                                            Jouer(objCarte, objCombinaison, 0);
                                                            if (objCombinaison.Verifier())
                                                                objMainJoueur.Supprimer(objCarte);
                                                            else
                                                                TracerContexte(false, true);

                                                            boolRes = true;
                                                            boolTourFini = false;
                                                            boolSortirFor = true;
                                                        }
                                                    }

                                                    if (objCombinaison.CartesCombinaison.Last().Valeur == eValeur.Joker)
                                                    {
                                                        Carte objDerniereCarte = objCombinaison.CartesCombinaison.Last();

                                                        if (objDerniereCarte.PeutRemplacer(objCarte.Valeur - 1, objCarte.Couleur))
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en fin de {1}",
                                                                                                objCarte.ToString(),
                                                                                                objCombinaison.ToString()));
                                                            Jouer(objCarte, objCombinaison);
                                                            if (objCombinaison.Verifier())
                                                                objMainJoueur.Supprimer(objCarte);
                                                            else
                                                                TracerContexte(false, true);

                                                            boolRes = true;
                                                            boolTourFini = false;
                                                            boolSortirFor = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Carte objDerniereCarte = objCombinaison.CartesCombinaison.Last();
                                                        if (objCarte.Couleur == objDerniereCarte.Couleur &&
                                                            objCarte.Valeur == objDerniereCarte.Valeur + 1)
                                                        {
                                                            GestionErreurs.Tracer(String.Format("Ajout de {0} en fin de {1}",
                                                                                                objCarte.ToString(),
                                                                                                objCombinaison.ToString()));
                                                            Jouer(objCarte, objCombinaison);
                                                            if (objCombinaison.Verifier())
                                                                objMainJoueur.Supprimer(objCarte);
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

                //'' 2nde étape : parcours des combinaisons potentielles de la main afin de
                //'' déterminer si leurs cartes ne sont pas complémentaires de combinaisons du jeu
                //'foreach (objCombinaison As Combinaison in lstCombinaisonsPotentielles.ToList
                //'    objCombinaison = CreerCombinaison(objCombinaison, false, jokers)

                //'    if (objCombinaison.Verifier)
                //'        GestionErreurs.Tracer("Dépose de " & objCombinaison.ToString)
                //'        Jouer(objCombinaison)

                //'        CompleterNiveau2 = true
                //'    else
                //'        Select Case objCombinaison.TypeCombinaison
                //'            Case eTypeCombinaison.SuiteValeurs
                //'                ' Calcul des cartes manquantes
                //'                Dim enumValeur As eValeur = eValeur.Indetermine
                //'                Dim lstCartesManquantes As new List(Of eCouleur)

                //'                ' Si la combinaison possède un joker, on se base sur les cartes
                //'                ' qu'il peut remplacer
                //'                ' Sinon, il faut procéder par élimination
                //'                Dim intPositionJoker As Integer = objCombinaison.PositionJoker
                //'                if (intPositionJoker = -1)
                //'                    With objCombinaison.Cartes.First
                //'                        foreach (enumCouleur As eCouleur in [Enum].GetValues(GetType(eCouleur))
                //'                            if (enumCouleur > eCouleur.Indetermine)
                //'                                lstCartesManquantes.Add(enumCouleur)
                //'                            End If
                //'                        Next

                //'                        enumValeur = .Valeur
                //'                    End With

                //'                    foreach (objCarte As Carte in objCombinaison.Cartes
                //'                        lstCartesManquantes.Remove(objCarte.Couleur)
                //'                    Next
                //'                else
                //'                    With objCombinaison.Cartes(intPositionJoker)
                //'                        foreach (objCarte As Carte in .ValeursRemplacees
                //'                            lstCartesManquantes.Add(objCarte.Couleur)
                //'                        Next

                //'                        enumValeur = .ValeurRemplacee
                //'                    End With
                //'                End If

                //'                ' Parcours des combinaisons du jeu à la rechercher de cartes
                //'                ' pouvant compléter la combinaison
                //'                foreach (enumCouleur As eCouleur in lstCartesManquantes
                //'                    Dim objCarte As Carte = RechercherCarte(Rummy.Carte(enumValeur, enumCouleur))
                //'                    if (objCarte != null)
                //'                        GestionErreurs.Tracer("Utilisation de " & objCarte.ToString)
                //'                        objCarte = UtiliserCarte(objCarte)
                //'                        objCombinaison.Ajouter(objCarte)
                //'                        if (! objCombinaison.Verifier)
                //'                            TracerContexte(false, true)
                //'                        End If
                //'                    End If
                //'                Next

                //'                if (objCombinaison.Verifier)
                //'                    GestionErreurs.Tracer("Dépose de " & objCombinaison.ToString)
                //'                    Jouer(objCombinaison)
                //'                    CompleterNiveau2 = true
                //'                End If
                //'            Case eTypeCombinaison.SuiteCouleurs
                //'                ' Calcul des cartes manquantes
                //'                Dim lstCartesManquantes As new List(Of Carte)
                //'                Dim dicPositionCarte As new Dictionary(Of eValeur, Integer)

                //'                ' Carte précédant la première carte de la combinaison, si celle-ci n'est pas un as
                //'                With objCombinaison.Cartes.First
                //'                    Dim enumValeur As eValeur = eValeur.Indetermine
                //'                    if (.Valeur = eValeur.Joker)
                //'                        enumValeur = .ValeurRemplacee
                //'                        lstCartesManquantes.Add(Rummy.Carte(enumValeur, .ValeursRemplacees.First.Couleur))
                //'                        dicPositionCarte.Add(enumValeur, 0)
                //'                    else
                //'                        enumValeur = .Valeur
                //'                    End If

                //'                    if (enumValeur > eValeur.Un)
                //'                        enumValeur = CType(enumValeur - 1, eValeur)
                //'                        lstCartesManquantes.Add(Rummy.Carte(enumValeur, .Couleur))
                //'                        dicPositionCarte.Add(enumValeur, 0)
                //'                    End If
                //'                End With

                //'                ' Carte suivant la dernière carte de la combinaison, si celle-ci n'est pas un 13
                //'                With objCombinaison.Cartes.Last
                //'                    Dim enumValeur As eValeur = eValeur.Indetermine
                //'                    if (.Valeur = eValeur.Joker)
                //'                        enumValeur = .ValeurRemplacee
                //'                        lstCartesManquantes.Add(Rummy.Carte(enumValeur, .ValeursRemplacees.First.Couleur))
                //'                        dicPositionCarte.Add(enumValeur, .Position)
                //'                    else
                //'                        enumValeur = .Valeur
                //'                    End If

                //'                    if (enumValeur < eValeur.Treize)
                //'                        enumValeur = CType(enumValeur + 1, eValeur)
                //'                        lstCartesManquantes.Add(Rummy.Carte(enumValeur, .Couleur))
                //'                        dicPositionCarte.Add(enumValeur, .Position + 1)
                //'                    End If
                //'                End With

                //'                Dim intNbCartes As Integer = 0
                //'                foreach (objCarte As Carte in lstCartesManquantes
                //'                    objCarte = RechercherCarte(objCarte)
                //'                    if (objCarte != null)
                //'                        GestionErreurs.Tracer("Utilisation de " & objCarte.ToString)
                //'                        objCarte = UtiliserCarte(objCarte)
                //'                        Dim objJoker As Carte = objCombinaison.Ajouter(objCarte, _
                //'                                                                       dicPositionCarte(objCarte.Valeur) + intNbCartes, _
                //'                                                                       true)

                //'                        if (objJoker != null)
                //'                            ' On a même réussi à libérer un joker
                //'                            jokers.Add(ReinitialiserJokerJeu(objJoker))
                //'                        End If

                //'                        intNbCartes += 1
                //'                    End If
                //'                Next

                //'                if (objCombinaison.Verifier)
                //'                    GestionErreurs.Tracer("Dépose de " & objCombinaison.ToString)
                //'                    Jouer(objCombinaison)

                //'                    CompleterNiveau2 = true
                //'                End If
                //'            Case else
                //'                if (objCombinaison.NbCartes = 0)
                //'                    lstCombinaisonsPotentielles.Remove(objCombinaison)
                //'                    Exit For
                //'                End If

                //'                ' Le type de la combinaison n'a pas pu être déterminé
                //'                ' Cela peut signifier deux choses :
                //'                ' - la combinaison n'est constituée que d'une seule carte
                //'                ' - la combinaison contient une carte plus un joker
                //'                Dim objCarteMain As Carte = null
                //'                Dim objJoker As Carte = null

                //'                foreach (objCarte As Carte in objCombinaison.Cartes
                //'                    if (objCarte.Valeur = eValeur.Joker)
                //'                        objJoker = objCarte
                //'                    else
                //'                        objCarteMain = objCarte
                //'                    End If
                //'                Next

                //'                ' Calcul des combinaisons possibles avec la/les carte/s
                //'                ' courante/s
                //'                Dim lstCombinaisonsPossibles As new List(Of Combinaison)
                //'                lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objCarteMain})))

                //'                if (objJoker != null)
                //'                    foreach (enumCouleur As eCouleur in [Enum].GetValues(GetType(eCouleur))
                //'                        if (enumCouleur != eCouleur.Indetermine)
                //'                            if (enumCouleur = objCarteMain.Couleur)
                //'                                Select Case objCarteMain.Valeur
                //'                                    Case eValeur.Un
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objCarteMain, _
                //'                                                                                                         objJoker, _
                //'                                                                                                         Rummy.Carte(CType(objCarteMain.Valeur + 2, eValeur), enumCouleur)})))
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objCarteMain, _
                //'                                                                                                         Rummy.Carte(CType(objCarteMain.Valeur + 1, eValeur), enumCouleur), _
                //'                                                                                                         objJoker})))
                //'                                    Case eValeur.Treize
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objJoker, _
                //'                                                                                                         Rummy.Carte(CType(objCarteMain.Valeur - 2, eValeur), enumCouleur), _
                //'                                                                                                         objCarteMain})))
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({Rummy.Carte(CType(objCarteMain.Valeur - 1, eValeur), enumCouleur), _
                //'                                                                                                         objJoker, _
                //'                                                                                                         objCarteMain})))
                //'                                    Case else
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({Rummy.Carte(CType(objCarteMain.Valeur - 1, eValeur), enumCouleur), _
                //'                                                                                                         objCarteMain, _
                //'                                                                                                         objJoker})))
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objJoker, _
                //'                                                                                                         objCarteMain, _
                //'                                                                                                         Rummy.Carte(CType(objCarteMain.Valeur + 1, eValeur), enumCouleur)})))
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objCarteMain, _
                //'                                                                                                         objJoker, _
                //'                                                                                                         Rummy.Carte(CType(objCarteMain.Valeur + 2, eValeur), enumCouleur)})))
                //'                                        lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objCarteMain, _
                //'                                                                                                         Rummy.Carte(CType(objCarteMain.Valeur + 1, eValeur), enumCouleur), _
                //'                                                                                                         objJoker})))
                //'                                End Select
                //'                            End If
                //'                        else
                //'                            lstCombinaisonsPossibles.Add(new Combinaison(new List(Of Carte)({objCarteMain, _
                //'                                                                                             Rummy.Carte(objCarteMain.Valeur, enumCouleur), _
                //'                                                                                             objJoker})))
                //'                        End If
                //'                    Next
                //'                End If

                //'                foreach (objCombinaisonPossible As Combinaison in lstCombinaisonsPossibles.ToList
                //'                    foreach (objCarte As Carte in objCombinaisonPossible.Cartes
                //'                        ' Si la carte ne provient pas de la main du joueur
                //'                        if (objCarte.ControleParentOrigine != null)
                //'                            objCarte = RechercherCarte(objCarte)
                //'                            if (objCarte != null)
                //'                                ' On a trouvé une carte correspondant à la carte présente
                //'                                ' dans la main du joueur
                //'                                ' On va donc tenter de diviser la combinaison de la zone de jeu
                //'                                ' afin de pouvoir insérer une seconde carte de même valeur,
                //'                                ' à condition qu'il s'agisse bien d'une suite de couleur de plus
                //'                                ' de 5 cartes
                //'                                if (objCombinaison.NbCartes = 1)
                //'                                    if (objCarte.Position > 2 &&
                //'                                       objCarte.Position < objCarte.Combinaison.NbCartes - 3 &&
                //'                                        objCarte.Combinaison.TypeCombinaison = eTypeCombinaison.SuiteCouleurs)
                //'                                        Dim objCombinaisonJeu As ICombinaison = objCarte.Combinaison
                //'                                        GestionErreurs.Tracer("Division de " & objCombinaisonJeu.ToString)
                //'                                        Dim objNouvelleCombinaison As ICombinaison = DiviserCombinaison(objCarte, false)
                //'                                        if (objNouvelleCombinaison != null)
                //'                                            GestionErreurs.Tracer("Résultat : " & objNouvelleCombinaison.ToString)
                //'                                            ' Il faut déterminer dans quelle combinaison se trouve la carte recherchée
                //'                                            if (objCombinaisonJeu.Carte(objCarte) == null)
                //'                                                Dim intPosition As Integer = -1
                //'                                                if (objCarteMain.Valeur < objCombinaisonJeu.Cartes.First.Valeur)
                //'                                                    intPosition = 0
                //'                                                End If
                //'                                                GestionErreurs.Tracer("Ajout de " & objCarteMain.ToString & " à " & objCombinaisonJeu.ToString & " à la position " & intPosition)
                //'                                                Jouer(objCarteMain, objCombinaisonJeu, intPosition)
                //'                                                if (! objCombinaisonJeu.Verifier)
                //'                                                    TracerContexte(false, true)
                //'                                                End If
                //'                                            else
                //'                                                GestionErreurs.Tracer("Ajout de " & objCarteMain.ToString & " en début de " & objNouvelleCombinaison.ToString)
                //'                                                Jouer(objCarteMain, objNouvelleCombinaison, 0)
                //'                                                if (! objNouvelleCombinaison.Verifier)
                //'                                                    TracerContexte(false, true)
                //'                                                End If
                //'                                            End If

                //'                                            CompleterNiveau2 = true
                //'                                        End If
                //'                                    End If
                //'                                else
                //'                                    GestionErreurs.Tracer("Utilisation de " & objCarte.ToString)
                //'                                    objCarte = UtiliserCarte(objCarte)
                //'                                    Dim intPosition As Integer = 0
                //'                                    foreach (objCarteCombinaison As Carte in objCombinaisonPossible.Cartes
                //'                                        if (objCarteCombinaison.ControleParentOrigine == null)
                //'                                            objCombinaisonPossible.Supprimer(objCarteCombinaison)
                //'                                            objCombinaisonPossible.Ajouter(objCarte, intPosition)
                //'                                        End If
                //'                                    Next

                //'                                    GestionErreurs.Tracer("Dépose de " & objCombinaisonPossible.ToString)
                //'                                    Jouer(objCombinaisonPossible)
                //'                                    if (! objCombinaisonPossible.Verifier)
                //'                                        TracerContexte(false, true)
                //'                                    End If
                //'                                    CompleterNiveau2 = true
                //'                                End If
                //'                            End If
                //'                        End If
                //'                    Next
                //'                Next
                //'        End Select
                //'    End If
                //'Next

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

        private class CComparerCombinaisons : IComparer<ICombinaison>
        {
            public int Compare(ICombinaison c1, ICombinaison c2)
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
