using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using NoyauCommun.Enumeres;

namespace NoyauCommun
{
    namespace Cartes
    {
        public partial class Carte : UserControl
        {
            private eValeur enumValeur = eValeur.Indetermine;
            private Dictionary<eCouleur, eValeur> dicCartesRemplacees;
            private eCouleur enumCouleur = eCouleur.Indetermine;
            private Carte objSuivant = null;
            private Carte objPrecedent = null;
            private Combinaison objCombinaison = null;
            private AbstractRepresentation objRepresentation = null;

            private eControleOrigine enumParentActuel = eControleOrigine.indetermine;
            private eControleOrigine enumParentOrigine = eControleOrigine.indetermine;

            private bool boolBoutonEnfonce = false;

            #region Evénements lancés par le contrôle "Carte"

            public delegate void CardClickHandler(object sender, System.EventArgs e);
            public delegate void CardMouseHandler(object sender, MouseEventArgs e);
            public delegate void CardDragHandler(object sender, DragEventArgs e);
            public delegate void CardDragLeaveHandler(object sender, System.EventArgs e);

            public event CardClickHandler CardClick;
            public event CardMouseHandler CardMouseDown;
            public event CardMouseHandler CardMouseMove;
            public event CardMouseHandler CardMouseUp;
            public event CardDragHandler CardDragDrop;
            public event CardDragHandler CardDragEnter;
            public event CardDragLeaveHandler CardDragLeave;

            #endregion

            public Carte Suivant
            {
                get
                {
                    return objSuivant;
                }
                set
                {
                    objSuivant = value;
                }
            }

            public Carte Precedent
            {
                get
                {
                    return objPrecedent;
                }
                set
                {
                    objPrecedent = value;
                }
            }

            public Carte()
            {
                InitializeComponent();
            }

            public Carte(eValeur valeur, eCouleur couleur, AbstractRepresentation representation)
            {
                InitializeComponent();

                enumValeur = valeur;
                enumCouleur = couleur;
                this.Size = representation.Dimensions;
                this.BackColor = representation.CouleurFond;
                pbCarte.Size = new Size(Dimensions.Width - 2 * pbCarte.Left, Dimensions.Height - 2 * pbCarte.Top);
                pbCarte.SizeMode = PictureBoxSizeMode.StretchImage;
                pbCarte.AllowDrop = true;
                objRepresentation = representation;
                dicCartesRemplacees = new Dictionary<eCouleur, eValeur>();
                Afficher();
            }

            public override string ToString()
            {
                string strParent = String.Empty;
                if (this.ControleParentActuel != eControleOrigine.indetermine)
                    strParent = this.ControleParentActuel.ToString();

                string strControleOrigine = String.Empty;
                if (enumParentOrigine != eControleOrigine.indetermine)
                    strControleOrigine = enumParentOrigine.ToString();

                return String.Format("{0} {1} ({2} - {3})",
                                     enumValeur.ToString(),
                                     enumCouleur.ToString(),
                                     strParent,
                                     strControleOrigine);
            }

            public void Afficher(bool visible = false)
            {
                if (visible)
                    pbCarte.Image = objRepresentation.Image(enumValeur, enumCouleur);
                else
                    pbCarte.Image = objRepresentation.Image();
            }

            public eValeur Valeur
            {
                get
                {
                    return enumValeur;
                }
            }

            public eCouleur Couleur
            {
                get
                {
                    return enumCouleur;
                }
            }

            public Size Dimensions
            {
                get
                {
                    return this.Size;
                }
            }

            public Combinaison Combinaison
            {
                get
                {
                    return objCombinaison;
                }
                set
                {
                    if (value == null)
                        objCombinaison = value;
                    else if (objCombinaison == null || objCombinaison.NbCartes == 0)
                        objCombinaison = value;
                    else
                        throw new Exception("La carte appartient déjà à une combinaison !");
                }
            }

            public int Position
            {
                get
                {
                    if (objCombinaison == null)
                        return 0;
                    else
                        return objCombinaison.Position(this);
                }
            }

            public Carte Ajouter(int position = -1)
            {
                return objCombinaison.Ajouter(this, position);
            }

            public Carte Supprimer()
            {
                if (objCombinaison == null)
                    return this;
                else
                    return objCombinaison.Supprimer(this);
            }

            public void Surligner()
            {
                if (!pbCarte.Image.Equals(objRepresentation.Image()))
                    this.BackColor = Color.Red;
            }

            public void Surligner(Color couleur)
            {
                if (!pbCarte.Image.Equals(objRepresentation.Image()))
                    this.BackColor = couleur;
            }

            public void Desurligner()
            {
                this.BackColor = objRepresentation.CouleurFond;
            }

            public eControleOrigine ControleParentActuel
            {
                get
                {
                    return enumParentActuel;
                }
                set
                {
                    enumParentActuel = value;
                }
            }

            public eControleOrigine ControleParentOrigine
            {
                get
                {
                    return enumParentOrigine;
                }
                set
                {
                    enumParentOrigine = value;
                }
            }

            public bool VientDeZoneJoueur
            {
                get
                {
                    return (enumParentOrigine == eControleOrigine.zoneJoueur);
                }
            }

            public bool VientDeZoneJeu
            {
                get
                {
                    return (enumParentOrigine == eControleOrigine.zoneJeu);
                }
            }

            #region Gestion des événements liés au contrôle graphique

            protected virtual void Carte_Click(object sender, System.EventArgs e)
            {
                if (CardClick != null)
                    CardClick(this, e);
            }

            protected virtual void Carte_DragDrop(object sender, DragEventArgs e)
            {
                if (CardDragDrop != null)
                    CardDragDrop(this, e);
            }

            protected virtual void Carte_DragEnter(object sender, DragEventArgs e)
            {
                if (CardDragEnter != null)
                    CardDragEnter(this, e);
            }

            protected virtual void Carte_DragLeave(object sender, System.EventArgs e)
            {
                if (CardDragLeave != null)
                    CardDragLeave(this, e);
            }

            protected virtual void Carte_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                    boolBoutonEnfonce = true;

                if (CardMouseDown != null)
                    CardMouseDown(this, e);
            }

            protected virtual void Carte_MouseMove(object sender, MouseEventArgs e)
            {
                if (boolBoutonEnfonce && CardMouseMove != null)
                    CardMouseMove(this, e);

                boolBoutonEnfonce = false;
            }

            protected virtual void Carte_MouseUp(object sender, MouseEventArgs e)
            {
                boolBoutonEnfonce = false;

                if (CardMouseUp != null)
                    CardMouseUp(this, e);
            }

            #endregion

            #region Gestion des cartes pouvant être remplacées par le joker

            public void ReinitialiserCartesRemplacees()
            {
                dicCartesRemplacees.Clear();
            }

            public void AjouterCarteRemplacee(eValeur valeur, eCouleur couleur)
            {
                dicCartesRemplacees.Add(couleur, valeur);
            }

            public void AjouterCartesRemplacees(List<Carte> cartes)
            {
                foreach (Carte objCarte in cartes)
                    AjouterCarteRemplacee(objCarte.Valeur, objCarte.Couleur);
            }

            /// <summary>
            /// Indique si la carte dont les caractéristiques sont passées en paramètre peut ou non être remplacée par le joker
            /// </summary>
            /// <param name="valeur">Valeur de la carte à tester</param>
            /// <param name="couleur">Couleur de la carte à tester</param>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public bool PeutRemplacer(eValeur valeur, eCouleur couleur)
            {
                return (dicCartesRemplacees.ContainsKey(couleur) && dicCartesRemplacees[couleur] == valeur);
            }

            /// <summary>
            /// Indique si la carte dont les caractéristiques sont passées en paramètre peut ou non être remplacée par le joker
            /// </summary>
            /// <param name="carte">Carte à tester</param>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public bool PeutRemplacer(Carte carte)
            {
                return PeutRemplacer(carte.Valeur, carte.Couleur);
            }

            public eValeur ValeurRemplacee
            {
                get
                {
                    if (dicCartesRemplacees.Count == 0)
                        return eValeur.Indetermine;
                    else
                        return dicCartesRemplacees.First().Value;
                }
            }

            public void SupprimerCarteRemplacee(Carte carte)
            {
                if (dicCartesRemplacees.ContainsKey(carte.Couleur) && dicCartesRemplacees[carte.Couleur] == carte.Valeur)
                    dicCartesRemplacees.Remove(carte.Couleur);
            }

            public List<Carte> ValeursRemplacees
            {
                get
                {
                    if (dicCartesRemplacees.Count == 0)
                        return new List<Carte>();
                    else
                    {
                        List<Carte> lstValeursRemplacees = new List<Carte>();
                        foreach (eCouleur enumCouleur in dicCartesRemplacees.Keys)
                            lstValeursRemplacees.Add(new Carte(dicCartesRemplacees[enumCouleur], enumCouleur, objRepresentation));

                        return lstValeursRemplacees;
                    }
                }
            }

            #endregion

            #region Méthode de clonage

            public Carte Clone()
            {
                Carte objClone = new Carte(this.Valeur, this.Couleur, objRepresentation);

                objClone.ControleParentActuel = this.ControleParentActuel;
                objClone.ControleParentOrigine = this.ControleParentOrigine;

                if (this.Valeur == eValeur.Joker)
                    foreach (Carte objCarteRemplacee in this.ValeursRemplacees)
                        objClone.AjouterCarteRemplacee(objCarteRemplacee.Valeur, objCarteRemplacee.Couleur);

                return objClone;
            }

            public bool Equals(Carte carte)
            {
                if (carte.Couleur != Couleur)
                    return false;

                if (carte.Valeur != Valeur)
                    return false;

                foreach (eCouleur enumCouleur in dicCartesRemplacees.Keys)
                    if (!carte.PeutRemplacer(dicCartesRemplacees[enumCouleur], enumCouleur))
                        return false;

                // Test sur le parent actuel et le contrôle d'origine ?

                return true;
            }

            #endregion

        }
    }
}