using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NoyauCommun.Enumeres;
using NoyauCommun.Erreurs;

namespace NoyauCommun.Cartes
{
    public enum eTypeCombinaison
    {
        indetermine,
        suitecouleurs,
        suitevaleurs
    }

    public delegate void CombinaisonChangedEvtHandler();

    public class Combinaison// : ICombinaison
    {
        public event CombinaisonChangedEvtHandler CombinaisonChanged;

        private eTypeCombinaison enumTypeCombinaison = eTypeCombinaison.indetermine;
        private List<Carte> lstCartes;
        private List<Carte> lstCartesManquantes;

        // Indique si la combinaison a ou non été modifiée depuis la dernière vérification
        private bool boolModifiee = true;

        public Combinaison()
        {
            lstCartes = new List<Carte>();
            lstCartesManquantes = new List<Carte>();
        }

        public Combinaison(List<Carte> cartes)
        {
            lstCartes = new List<Carte>(cartes);
            lstCartesManquantes = new List<Carte>();
        }

        public eTypeCombinaison Type
        {
            get
            {
                return enumTypeCombinaison;
            }
        }

        /// <summary>
        /// Nombre de cartes constituant la combinaison
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int NbCartes
        {
            get
            {
                return lstCartes.Count;
            }
        }

        /// <summary>
        /// Ensemble des cartes constituant la combinaison
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<Carte> CartesCombinaison
        {
            get
            {
                return new List<Carte>(lstCartes);
            }
        }

        /// <summary>
        /// Renvoie la liste des cartes manquantes à la combinaison pour que celle-ci soit viable
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<Carte> CartesManquantes
        {
            get
            {
                return new List<Carte>(lstCartesManquantes);
            }
            set
            {
                lstCartesManquantes = value;
            }
        }

        /// <summary>
        /// Renvoie l'instance de la carte dont les caractéristiques {valeur/couleur} correspondent à ceux
        /// passés en paramètre
        /// </summary>
        /// <param name="valeur">Valeur de la carte recherchée</param>
        /// <param name="couleur">Couleur de la carte recherchée</param>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Carte Carte(eValeur valeur, eCouleur couleur)
        {
            Carte res = null;

            foreach (Carte objCarte in lstCartes)
                if (objCarte.Valeur == valeur && objCarte.Couleur == couleur)
                {
                    res = objCarte;
                    break;
                }

            return res;
        }

        public Carte Carte(Carte carteRecherchee)
        {
            return Carte(carteRecherchee.Valeur, carteRecherchee.Couleur);
        }

        public int Position(Carte carte)
        {
            return lstCartes.IndexOf(carte);
        }

        /// <summary>
        /// Renvoie la somme des valeurs des cartes contenues dans la combinaison
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Un joker non utilisé a une valeur de 20 points</remarks>
        public int PointsRestants
        {
            get
            {
                int intRes = 0;
                foreach (Carte objCarte in lstCartes)
                    intRes += (int)objCarte.Valeur;

                return intRes;
            }
        }

        /// <summary>
        /// Renvoie la somme des valeurs des cartes contenues dans la combinaison
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Un joker non utilisé a une valeur de 20 points</remarks>
        public int NbPoints
        {
            get
            {
                int intRes = 0;
                foreach (Carte objCarte in lstCartes)
                    if (objCarte.Valeur == eValeur.Joker)
                        intRes += (int)objCarte.ValeurRemplacee;
                    else
                        intRes += (int)objCarte.Valeur;

                return intRes;
            }
        }

        /// <summary>
        /// Indique si la combinaison contient au moins une carte provenant de la main de jeu du joueur
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool PossedeCarteJoueur()
        {
            foreach (Carte objCarte in lstCartes)
                if (objCarte.VientDeZoneJoueur)
                    return true;

            return false;
        }

        public int PositionJoker
        {
            get
            {
                bool boolTrouve = false;
                int intRes = 0;

                foreach (Carte objCarte in lstCartes)
                    if (objCarte.Valeur == eValeur.Joker)
                    {
                        boolTrouve = true;
                        break;
                    }
                    else
                        intRes++;

                if (boolTrouve)
                    return intRes;
                else
                    return -1;
            }
        }

        /// <summary>
        /// Indique si la combinaison possède un joker déjà déposé à l'un des tours de jeu précédent
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool PossedeJokerDejaDepose()
        {
            bool boolRes = false;

            foreach (Carte objCarte in lstCartes)
                if (objCarte.Valeur == eValeur.Joker && objCarte.VientDeZoneJeu)
                {
                    boolRes = true;
                    break;
                }
            return boolRes;
        }

        /// <summary>
        /// Permet d'ajouter/insérer une carte à un endroit précis de la combinaison
        /// </summary>
        /// <param name="carte">Carte à ajouter</param>
        /// <param name="position">Position d'insertion/ajout</param>
        /// <returns>Renvoie "Nothing" si tout se passe bien
        /// Renvoie la carte bordée de rouge si une erreur est survenue</returns>
        /// Renvoie la carte précédemment située à la position, en cas de remplacement de carte
        /// <param name="remplacerCarte">Indique si, dans le cas d'une insertion à une place précise, la carte présente
        /// à cet emplacement doit ou non être remplacer par la nouvelle carte</param>
        /// <remarks></remarks>
        public Carte Ajouter(Carte carte, int position = -1, bool remplacerCarte = false)
        {
            Carte objRes = null;

            try
            {
                // Recalibrage de la position d'insertion, en fonction du nombre de cartes déjà présentes dans la combinaison
                if (position > lstCartes.Count)
                    position = lstCartes.Count;

                if (!(carte.Combinaison == null || carte.Combinaison.NbCartes == 0))
                    throw new Exception("La carte ajoutée est toujours rattachée à la combinaison " + carte.Combinaison.ToString() + " !");

                carte.Combinaison = this;

                if (position < 0 || position == lstCartes.Count)
                    lstCartes.Add(carte);
                else if (remplacerCarte && lstCartes[position].PeutRemplacer(carte))
                {
                    objRes = lstCartes[position];
                    objRes.Supprimer();
                    lstCartes.Insert(position, carte);
                    objRes.Combinaison = new Combinaison(new List<Carte>());
                    objRes.Combinaison.Ajouter(objRes);
                    objRes.Surligner();
                }
                else
                    lstCartes.Insert(position, carte);

                // Prévention de qui écoute que la combinaison a été modifiée
                if (CombinaisonChanged != null)
                    CombinaisonChanged();

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                carte.Surligner();
                return carte;
            }
            finally
            {
                boolModifiee = true;
            }
        }

        /// <summary>
        /// Retire une carte de la combinaison
        /// </summary>
        /// <param name="carte">Carte à retirer de la combinaison</param>
        /// <returns>Renvoie la carte retirée
        /// Renvoie "Nothing" si la carte n'est pas trouvée ou en cas d'erreur dans le traitement</returns>
        /// <remarks></remarks>
        public Carte Supprimer(Carte carte)
        {
            Carte objRes = null;

            try
            {
                int intPosition = Position(carte);

                if (intPosition == -1)
                    return objRes;

                objRes = lstCartes[intPosition];
                lstCartes.Remove(carte);

                if (objRes.Combinaison == this)
                    objRes.Combinaison = null;

                // Prévention de qui écoute que la combinaison a été modifiée
                if (CombinaisonChanged != null)
                    CombinaisonChanged();

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
            finally
            {
                boolModifiee = true;
            }
        }

        /// <summary>
        /// Trie la combinaison par ordre croissant de valeurs
        /// </summary>
        /// <remarks></remarks>
        public void Trier()
        {
            try
            {
                lstCartes.Sort(new CComparerCartes());
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        /// <summary>
        /// Assure la division de la combinaison au niveau de la carte
        /// </summary>
        /// <param name="carte">Carte à partir de laquelle diviser la combinaison</param>
        /// <param name="avant">Indique si la division doit être effectuée au niveau de la carte ou après celle-ci</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Combinaison Diviser(Carte carte, bool avant)
        {
            try
            {
                Combinaison objRes = null;

                // Si la combinaison ne contient qu'une seule carte, on considère la division comme
                // déjà effectuée
                if (NbCartes == 1)
                    return this;

                int intPosition = carte.Position;

                /* Afin de s'assurer que le joker continue de remplacer les mêmes cartes
                   après la division de la combinaison, il faut que celui-ci reste sur
                   le Panel d'origine
                   Cela implique donc de déplacer l'autre moitié de la combinaison */
                int intPositionJoker = this.PositionJoker;

                if (avant)
                    if (intPosition < 1)
                        return null;
                    else if (intPosition <= intPositionJoker)
                        intPosition -= 1;
                    else
                        avant = false;
                else if (intPosition > NbCartes - 2)
                    return null;
                else if (intPosition < intPositionJoker)
                    avant = true;
                else
                    intPosition += 1;

                if (avant)
                {
                    // Obtention de la partie gauche de la combinaison
                    while (intPosition >= 0)
                    {
                        Carte objCarte = CartesCombinaison[intPosition];
                        objCarte = objCarte.Supprimer();

                        if (objRes == null)
                            objRes = new Combinaison();

                        objRes.Ajouter(objCarte, 0);

                        intPosition--;
                    }
                }
                else
                {
                    // Obtention de la partie droite de la combinaison
                    while (intPosition < NbCartes)
                    {
                        Carte objCarte = CartesCombinaison[intPosition];
                        objCarte = objCarte.Supprimer();

                        if (objRes == null)
                            objRes = new Combinaison();

                        objRes.Ajouter(objCarte);
                    }
                }

                return objRes;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return null;
            }
        }

        public bool Verifier(bool verifierNbCartes = true)
        {
            try
            {
                if (!boolModifiee)
                    return true;

                // Première chose à vérifier : la combinaison contient au moins 3 cartes
                if (verifierNbCartes && NbCartes < 3)
                {
                    // Marquage des cartes
                    foreach (Carte objCarte in CartesCombinaison)
                        objCarte.Surligner();

                    return false;
                }

                Dictionary<eCouleur, List<eValeur>> dicValeursAttendues = null;
                Dictionary<eCouleur, List<eValeur>> dicValeursRemplacees = null;
                eValeur enumValeurPrec = eValeur.Indetermine;
                eCouleur enumCouleurPrec = eCouleur.Indetermine;
                bool boolJoker = false;

                // Affectation de la valeur de la carte que remplace le joker
                int intPositionJoker = -1;
                int intNoCarte = 0;
                enumTypeCombinaison = eTypeCombinaison.indetermine;

                foreach (Carte objCarte in CartesCombinaison)
                {
                    objCarte.Desurligner();

                    if (dicValeursAttendues == null)
                    {
                        if (objCarte.Valeur != eValeur.Joker)
                        {
                            dicValeursAttendues = new Dictionary<eCouleur, List<eValeur>>();

                            foreach (eCouleur enumCouleur in Enum.GetValues(typeof(eCouleur)))
                                if (enumCouleur == eCouleur.Indetermine)
                                {
                                    /* Cette carte représente un joker "créé" à partir d'une carte présente
                                       dans la main du joueur automatique */
                                    dicValeursAttendues.Add(enumCouleur, new List<eValeur> { eValeur.Joker });
                                }
                                else if (enumCouleur == objCarte.Couleur)
                                    dicValeursAttendues.Add(enumCouleur, new List<eValeur> { objCarte.Valeur + 1, eValeur.Joker });
                                else
                                    dicValeursAttendues.Add(enumCouleur, new List<eValeur> { objCarte.Valeur, eValeur.Joker });

                            // Initialisation de la liste des valeurs remplaçables par un éventuel joker
                            dicValeursRemplacees = new Dictionary<eCouleur, List<eValeur>>();

                            foreach (eCouleur enumCouleur in dicValeursAttendues.Keys)
                                if (enumCouleur != eCouleur.Indetermine)
                                {
                                    dicValeursRemplacees.Add(enumCouleur, new List<eValeur>());
                                    if (enumCouleur == objCarte.Couleur)
                                    {
                                        /* Un joker peut, à priori, remplacer la carte précédent la carte
                                         * courante, sauf si celle-ci est un UN */
                                        if (objCarte.Valeur > eValeur.Un)
                                            dicValeursRemplacees[enumCouleur].Add((eValeur)(objCarte.Valeur - 1));

                                        // Il peut aussi remplacer toutes les cartes suivant la carte courante
                                        for (eValeur enumValeur = eValeur.Treize; enumValeur > objCarte.Valeur; enumValeur--)
                                            dicValeursRemplacees[enumCouleur].Add(enumValeur);
                                    }
                                    else
                                        dicValeursRemplacees[enumCouleur] = new List<eValeur> { objCarte.Valeur };
                                }

                            enumCouleurPrec = objCarte.Couleur;
                            enumValeurPrec = objCarte.Valeur;
                        }
                        else if (boolJoker)
                        {
                            // Il ne peut y avoir plus d'un joker dans la combinaison
                            objCarte.Surligner();
                            return false;
                        }
                        else
                        {
                            boolJoker = true;
                            intPositionJoker = 0;
                        }
                    }
                    else if (dicValeursAttendues.ContainsKey(objCarte.Couleur) &&
                             dicValeursAttendues[objCarte.Couleur].Contains(objCarte.Valeur))
                        if (objCarte.Valeur == eValeur.Joker)
                        {
                            eValeur enumValeurRemplacee = (eValeur)(enumValeurPrec + 1);

                            foreach (eCouleur enumCouleur in dicValeursAttendues.Keys.ToList())
                                if (dicValeursAttendues[enumCouleur].Contains(enumValeurRemplacee))
                                    dicValeursAttendues[enumCouleur] = new List<eValeur> { enumValeurPrec + 2 };
                                else if (dicValeursAttendues[enumCouleur].Count == 1)
                                    dicValeursAttendues.Remove(enumCouleur);
                                else
                                    dicValeursAttendues[enumCouleur] = new List<eValeur> { enumValeurPrec };

                            intPositionJoker = intNoCarte;
                        }
                        else if (objCarte.Couleur == enumCouleurPrec)
                        {
                            // Il s'agit d'une suite de même valeur
                            enumTypeCombinaison = eTypeCombinaison.suitecouleurs;

                            if (objCarte.Valeur == eValeur.Deux &&
                                intPositionJoker == 0)
                            {
                                // Il ne peut y avoir de joker avant un Un !
                                lstCartes.First().Surligner();
                                return false;
                            }

                            // Il ne peut y avoir aucune carte après un 13 dans une suite de couleur
                            if (objCarte.Valeur == eValeur.Treize)
                                dicValeursAttendues = new Dictionary<eCouleur, List<eValeur>>();
                            else
                                foreach (eCouleur enumCouleur in dicValeursAttendues.Keys.ToList())
                                {
                                    if (enumCouleur == enumCouleurPrec)
                                        dicValeursAttendues[enumCouleur] = new List<eValeur> { objCarte.Valeur + 1 };
                                    else
                                        dicValeursAttendues[enumCouleur] = new List<eValeur>();

                                    if (intPositionJoker == -1)
                                        dicValeursAttendues[enumCouleur].Add(eValeur.Joker);
                                }

                            foreach (eCouleur enumCouleur in dicValeursRemplacees.Keys.ToList())
                                if (enumCouleur == enumCouleurPrec)
                                    dicValeursRemplacees[enumCouleur].Remove(objCarte.Valeur);
                                else
                                    dicValeursRemplacees.Remove(enumCouleur);

                            enumValeurPrec = objCarte.Valeur;
                        }
                        else
                        {
                            // Il s'agit d'une suite de même valeur
                            foreach (eCouleur enumCouleur in dicValeursAttendues.Keys.ToList())
                            {
                                if (intPositionJoker != -1)
                                    dicValeursAttendues[enumCouleur].Remove(eValeur.Joker);

                                if (enumCouleur == objCarte.Couleur)
                                    dicValeursAttendues[enumCouleur].Remove(enumValeurPrec);
                                else if (enumCouleur == enumCouleurPrec)
                                {
                                    dicValeursAttendues.Remove(enumCouleur);
                                    if (intPositionJoker == -1)
                                        dicValeursAttendues.Add(enumCouleur, new List<eValeur> { eValeur.Joker });
                                }

                                if (dicValeursAttendues.ContainsKey(enumCouleur) &&
                                    dicValeursAttendues[enumCouleur].Count == 0)
                                    dicValeursAttendues.Remove(enumCouleur);
                            }

                            // Filtre sur les valeurs que le joker peut remplacer
                            dicValeursRemplacees.Remove(enumCouleurPrec);
                            dicValeursRemplacees.Remove(objCarte.Couleur);

                            // Il s'agit d'une suite de même couleur
                            enumTypeCombinaison = eTypeCombinaison.suitevaleurs;
                            enumCouleurPrec = objCarte.Couleur;
                        }
                    else
                    {
                        objCarte.Surligner();
                        return false;
                    }

                    intNoCarte++;
                }

                // S'il y a un joker dans la combinaison, il faut lui attribuer la/les valeur/s qu'il est sensé remplacer
                if (intPositionJoker > -1)
                {
                    Carte objJoker = lstCartes[intPositionJoker];

                    objJoker.ReinitialiserCartesRemplacees();
                    switch (enumTypeCombinaison)
                    {
                        case eTypeCombinaison.suitecouleurs:
                            {
                                eCouleur enumCouleur = dicValeursRemplacees.Keys.First();

                                if (intPositionJoker == 0)
                                    lstCartes[intPositionJoker].AjouterCarteRemplacee(dicValeursRemplacees[enumCouleur].First(), enumCouleur);
                                else
                                    lstCartes[intPositionJoker].AjouterCarteRemplacee(dicValeursRemplacees[enumCouleur].Last(), enumCouleur);

                                break;
                            }
                        case eTypeCombinaison.suitevaleurs:
                            {
                                foreach (eCouleur enumCouleur in dicValeursRemplacees.Keys)
                                    lstCartes[intPositionJoker].AjouterCarteRemplacee(dicValeursRemplacees[enumCouleur].First(), enumCouleur);

                                break;
                            }
                    }
                }

                boolModifiee = !verifierNbCartes;

                return true;
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
                return false;
            }
        }

        public override string ToString()
        {
            string strRes = String.Empty;

            foreach (Carte objCarte in CartesCombinaison)
            {
                if (strRes.Length > 0)
                    strRes += " - ";

                strRes += objCarte.ToString();
            }

            return strRes;
        }

        #region Méthode de clonage de l'objet

        public Combinaison Clone()
        {
            Combinaison objCombinaison = new Combinaison();

            foreach (Carte objCarte in CartesCombinaison)
                objCombinaison.Ajouter(objCarte.Clone());

            objCombinaison.CartesManquantes = new List<Carte>(lstCartesManquantes);
            objCombinaison.Verifier();

            return objCombinaison;
        }

        public bool Equals(Combinaison combinaison)
        {
            if (combinaison.NbCartes != NbCartes)
                return false;

            int intNoCarte = 0;

            foreach (Carte objCarte in combinaison.CartesCombinaison)
            {
                if (!objCarte.Equals(lstCartes[intNoCarte]))
                    return false;

                intNoCarte++;
            }

            return true;
        }

        #endregion

        #region Classe assurant le tri par ordre croissant de valeur puis par ordre de couleur

        private class CComparerCartes : IComparer<Carte>
        {
            // Implémente le tri de la collection par valeur puis par couleur de carte
            public int Compare(Carte c1, Carte c2)
            {
                int intTmp = c1.Valeur - c2.Valeur;

                if (intTmp < 0)
                    return -1;
                else if (intTmp > 0)
                    return 1;
                else
                    return c1.Couleur - c2.Couleur;
            }
        }

        #endregion
    }
}