using System;
using System.Drawing;

using NoyauCommun.Enumeres;

namespace NoyauCommun
{
    namespace Cartes
    {
        public abstract class AbstractRepresentation
        {
            public abstract Image Image(eValeur valeur = eValeur.Indetermine,
                                        eCouleur couleur = eCouleur.Indetermine);
            public abstract Size Dimensions { get;  }
            public abstract Color CouleurFond { get; }
        }
    }
}