using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoyauCommun
{
    namespace Enumeres
    {
        #region Enumérés relatifs aux cartes

        public enum eValeur
        {
            Indetermine = 0,
            Un,
            Deux,
            Trois,
            Quatre,
            Cinq,
            Six,
            Sept,
            Huit,
            Neuf,
            Dix,
            Onze,
            Douze,
            Treize,
            Joker = 20
        }

        public enum eCouleur
        {
            Indetermine,
            Pique,
            Coeur,
            Carreau,
            Trefle
        }

        public enum eControleOrigine
        {
            indetermine,
            zoneJeu,
            zoneJoueur
        }

        #endregion

        #region Enumérés relatifs au joueur automatique

        public enum eNiveauDifficulte
        {
            Indetermine = -1,
            Debutant = 0,
            Facile = 1,
            Moyen = 2,
            Difficile = 3
        }

        #endregion

        #region Enumérés relatifs à l'interface graphique

        public enum ePosition
        {
            Bas,
            Gauche,
            Haut,
            Droite
        }

        #endregion
    }

}