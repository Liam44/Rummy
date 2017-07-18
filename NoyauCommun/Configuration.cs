using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using System.Xml.Serialization;

using NoyauCommun.Enumeres;

namespace NoyauCommun
{
    namespace Configuration
    {
        public class Configuration
        {

            private static ConfigurationSerialisable objConfiguration = null;
            private static CultureInfo objCulture;

            public delegate void LangueChangeeEvtHandler();
            public delegate void CouleurFondChangeeEvtHandler();

            public static event LangueChangeeEvtHandler LangueChangee;
            public static event CouleurFondChangeeEvtHandler CouleurFondChangee;

            private const string NOM_FICHIER_PARAMETRE = "parametres.xml";

            public static Color CouleurFond
            {
                get
                {
                    return Color.FromArgb(objConfiguration.ARGB);
                }
                set
                {
                    bool boolCouleurFondChangee = Color.FromArgb(objConfiguration.ARGB) != value;

                    objConfiguration.ARGB = value.ToArgb();

                    if (boolCouleurFondChangee)
                        CouleurFondChangee();
                }
            }

            public static bool TempsJeuLimite
            {
                get
                {
                    return objConfiguration.TempsLimite;
                }
                set
                {
                    objConfiguration.TempsLimite = value;
                }
            }

            public static int NbSecondesReflexion
            {
                get
                {
                    return objConfiguration.NbSecondesReflexion;
                }
                set
                {
                    objConfiguration.NbSecondesReflexion = value;
                }
            }

            public static Size Dimensions
            {
                get
                {
                    return objConfiguration.DimensionsCarte;
                }
                set
                {
                    objConfiguration.DimensionsCarte = value;
                }
            }

            public static List<ConfigJoueur> Joueurs
            {
                get
                {
                    return objConfiguration.Joueurs;
                }
            }

            public static FormatInterface ParametresInterface
            {
                get
                {
                    return objConfiguration.FormatInterface;
                }
            }

            public static CultureInfo Culture
            {
                get
                {
                    return objCulture;
                }
                set
                {
                    bool boolLangueChangee = value != objCulture;
                    objCulture = value;

                    if (boolLangueChangee)
                        LangueChangee();
                }
            }

            private static ResourceManager ResourceManager
            {
                get
                {
                    return new ResourceManager("NoyauCommun.Langues.Resource1", typeof(Configuration).Assembly);
                }
            }

            public static void Lire()
            {
                if (File.Exists(NOM_FICHIER_PARAMETRE))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ConfigurationSerialisable));
                    StreamReader rd = new StreamReader(NOM_FICHIER_PARAMETRE);
                    objConfiguration = (ConfigurationSerialisable)xs.Deserialize(rd);

                    objCulture = CultureInfo.CreateSpecificCulture(objConfiguration.Culture);

                    rd.Close();
                }
                else
                {
                    objCulture = CultureInfo.CurrentCulture;
                    objConfiguration = new ConfigurationSerialisable(new Object());
                    Enregistrer();
                }
            }

            public static void Enregistrer()
            {
                objConfiguration.Culture = objCulture.ToString();

                XmlSerializer xs = new XmlSerializer(typeof(ConfigurationSerialisable));
                StreamWriter wr = new StreamWriter(NOM_FICHIER_PARAMETRE);

                xs.Serialize(wr, objConfiguration);
                wr.Close();
            }

            #region Gestion de la langue de l'application

            public static string ObtenirTexte(string intitule)
            {
                ResourceManager rm = ResourceManager;
                return rm.GetString(intitule, objCulture);
            }

            #endregion
        }

        [Serializable]
        public class ConfigurationSerialisable
        {
            private Size szDimensionsCarte = new Size(56, 80);
            private int intARGB;
            private List<ConfigJoueur> lstJoueurs = new List<ConfigJoueur>();
            private FormatInterface objParametresInterface = new FormatInterface();
            private string strCulture;
            private bool boolTempsLimite = false;
            private int intNbSecondesReflexion = 10;    // Valeur minimale

            public int ARGB
            {
                get
                {
                    return intARGB;
                }
                set
                {
                    intARGB = value;
                }
            }

            public Size DimensionsCarte
            {
                get
                {
                    return szDimensionsCarte;
                }
                set
                {
                    szDimensionsCarte = value;
                }
            }

            public List<ConfigJoueur> Joueurs
            {
                get
                {
                    return lstJoueurs;
                }
                set
                {
                    lstJoueurs = value;
                }
            }

            public FormatInterface FormatInterface
            {
                get
                {
                    return objParametresInterface;
                }
                set
                {
                    objParametresInterface = value;
                }
            }

            public string Culture
            {
                get
                {
                    return strCulture;
                }
                set
                {
                    strCulture = value;
                }
            }

            public bool TempsLimite
            {
                get
                {
                    return boolTempsLimite;
                }
                set
                {
                    boolTempsLimite = value;
                }
            }

            public int NbSecondesReflexion
            {
                get
                {
                    return intNbSecondesReflexion;
                }
                set
                {
                    intNbSecondesReflexion = value;
                }
            }

            /// <summary>
            /// Méthode d'initialisation appelée par la classe "Configuration" si le fichier de paramètres
            /// n'existe pas encore
            /// </summary>
            /// <param name="paramBidon">Sert uniquement à différencier les deux méthodes d'initialisation</param>
            /// <remarks></remarks>
            protected internal ConfigurationSerialisable(object paramBidon)
            {
                szDimensionsCarte = new Size(56, 80);
                intARGB = Color.MediumSeaGreen.ToArgb();
                lstJoueurs = new List<ConfigJoueur> { new ConfigJoueur("Joueur1", true), 
                                                      new ConfigJoueur("Joueur2", false, Enumeres.eNiveauDifficulte.Debutant) };
                objParametresInterface = new FormatInterface();
                boolTempsLimite = true;
                intNbSecondesReflexion = 120;    // 2 minutes
            }

            /// <summary>
            /// Méthode d'initialisation uniquement appelée lors de la désérialisation du fichier de paramètres
            /// </summary>
            /// <remarks></remarks>
            public ConfigurationSerialisable()
            {
            }
        }

        [Serializable]
        public class ConfigJoueur
        {
            private bool boolHumain;
            private eNiveauDifficulte enumNiveau;
            private string strNom;

            public bool Humain
            {
                get
                {
                    return boolHumain;
                }
                set
                {
                    boolHumain = value;
                }
            }

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

            public eNiveauDifficulte NiveauDifficulte
            {
                get
                {
                    return enumNiveau;
                }
                set
                {
                    enumNiveau = value;
                }
            }

            public ConfigJoueur()
            {
                boolHumain = false;
                strNom = String.Empty;
                enumNiveau = eNiveauDifficulte.Indetermine;
            }

            public ConfigJoueur(string nom, bool humain, eNiveauDifficulte niveau = eNiveauDifficulte.Indetermine)
            {
                boolHumain = humain;
                strNom = nom;
                enumNiveau = niveau;
            }
        }

        [Serializable]
        public class FormatInterface
        {
            private Size szDimensions;
            private Point ptLocation;
            private FormWindowState enumState;

            public Size Dimensions
            {
                get
                {
                    return szDimensions;
                }
                set
                {
                    szDimensions = value;
                }
            }

            public Point Location
            {
                get
                {
                    return ptLocation;
                }
                set
                {
                    ptLocation = value;
                }
            }

            public FormWindowState Etat
            {
                get
                {
                    return enumState;
                }
                set
                {
                    enumState = value;
                }
            }

            public FormatInterface()
            {
                szDimensions = new Size();
                ptLocation = new Point();
                enumState = FormWindowState.Normal;
            }

            protected internal FormatInterface(Size dimensions, Point location, FormWindowState etat)
            {
                szDimensions = dimensions;
                ptLocation = location;
                enumState = etat;
            }
        }
    }
}