using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using NoyauCommun.Enumeres;

namespace NoyauCommun
{
    namespace Cartes
    {
        public class Representation : AbstractRepresentation
        {

            public override Size Dimensions
            {
                get
                {
                    return Configuration.Configuration.Dimensions;
                }
            }

            public override Color CouleurFond
            {
                get
                {
                    return Configuration.Configuration.CouleurFond;
                }
            }

            public override Image Image(eValeur valeur = eValeur.Indetermine,
                                        eCouleur couleur = eCouleur.Indetermine)
            {
                try
                {
                    if (valeur == eValeur.Indetermine || couleur == eCouleur.Indetermine)
                        return (Image)Properties.Resources.dos;
                    else
                        return (Image)(Properties.Resources.ResourceManager.GetObject(valeur.ToString().ToLower() +
                                                                                      couleur.ToString().ToLower()));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}