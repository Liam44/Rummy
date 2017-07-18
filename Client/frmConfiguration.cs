using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NoyauCommun.Configuration;
using NoyauCommun.Enumeres;

namespace Client
{
    public partial class frmConfiguration : Form
    {
        private bool boolPartieEnCours;
        private bool boolConfigJoueurModifiee = false;
        private bool boolFermetureEnCours = false;
        private List<ConfigJoueur> lstJoueurs = new List<ConfigJoueur>();
        private CultureInfo objCulture;

        protected internal bool ConfigurationJoueursModifiee
        {
            get
            {
                return boolConfigJoueurModifiee;
            }
        }

        protected internal void Initialiser(bool partieEnCours)
        {
            boolPartieEnCours = partieEnCours;
            this.ShowDialog();
        }

        private void AppliquerLangue()
        {
            // Onglet «Joueurs»
            TabJoueurs.Text = Configuration.ObtenirTexte("TabJoueurs");

            // Onglet «Langues»
            TabLangues.Text = Configuration.ObtenirTexte("TabLangues");
            rbAnglais.Text = Configuration.ObtenirTexte("rbAnglais");
            rbFrancais.Text = Configuration.ObtenirTexte("rbFrancais");
            rbSuedois.Text = Configuration.ObtenirTexte("rbSuedois");

            // Onglet «Divers»
            lblCouleurFond.Text = Configuration.ObtenirTexte("ConfigCouleurFondJeu");
            cmdCouleurFond.Left = lblCouleurFond.Left + lblCouleurFond.Width + 6;

            chkTempsLimite.Text = Configuration.ObtenirTexte("ConfigTempsLimite");
            lblTempsLimite.Text = Configuration.ObtenirTexte("ConfigNbSecondes");
            nudTempsLimite.Left = lblTempsLimite.Left + lblTempsLimite.Width + 6;
        }

        private bool AppliquerModifications()
        {
            //      _______     //
            //      Joueurs     //
            //      ¯¯¯¯¯¯¯     //
            // Vérification qu'au moins un des joueurs est humains
            bool boolHumain = false;
            bool boolNomsOk = true;
            foreach (ConfigJoueur objJoueur in lstJoueurs)
                if (objJoueur.NiveauDifficulte != eNiveauDifficulte.Indetermine && objJoueur.Nom.Length == 0)
                {
                    boolNomsOk = false;
                    break;
                }
                else if (objJoueur.Humain)
                    boolHumain = true;

            if (!boolNomsOk)
            {
                MessageBox.Show("Veuillez vérifier que le nom de tous les joueurs est bien renseigné.",
                                "Erreur de configuration",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                return false;
            }

            if (!boolHumain)
            {
                MessageBox.Show("Au moins un des joueurs doit être humain !",
                                "Erreur de configuration",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                return false;
            }

            boolConfigJoueurModifiee = false;

            if (Configuration.Joueurs.Count != nupNbJoueurs.Value)
                if (MessageBox.Show("La modification de la configuration des joueurs annulera la partie en cours." + Environment.NewLine +
                                    "Voulez-vous continuer ?",
                                    "Modification de la configuration de jeu",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information) == DialogResult.No)
                    return false;
                else
                    boolConfigJoueurModifiee = true;

            for (int intNoJoueur = 0; intNoJoueur < nupNbJoueurs.Value; intNoJoueur++)
            {
                ConfigJoueur objJoueur = lstJoueurs[intNoJoueur];

                if (intNoJoueur == Configuration.Joueurs.Count)
                    Configuration.Joueurs.Add(objJoueur);
                else
                {
                    ConfigJoueur objJoueurConfig = Configuration.Joueurs[intNoJoueur];

                    if ((objJoueur.Humain != objJoueurConfig.Humain ||
                         objJoueur.NiveauDifficulte != objJoueurConfig.NiveauDifficulte) &&
                        !boolConfigJoueurModifiee)
                        if (MessageBox.Show("La modification de la configuration des joueurs annulera la partie en cours." + Environment.NewLine +
                                            "Voulez-vous continuer ?",
                                            "Modification de la configuration de jeu",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Information) == DialogResult.No)
                            return false;
                        else
                            boolConfigJoueurModifiee = true;

                    Configuration.Joueurs[intNoJoueur] = objJoueur;
                }
            }

            while (Configuration.Joueurs.Count > nupNbJoueurs.Value)
            {
                Configuration.Joueurs.RemoveAt(Configuration.Joueurs.Count - 1);
            }

            //      _______     //
            //      Langues     //
            //      ¯¯¯¯¯¯¯     //
            Configuration.Culture = objCulture;

            //      ______     //
            //      Divers     //
            //      ¯¯¯¯¯¯     //
            Configuration.CouleurFond = cmdCouleurFond.BackColor;

            Configuration.TempsJeuLimite = chkTempsLimite.Checked;
            Configuration.NbSecondesReflexion = (int)nudTempsLimite.Value;

            Configuration.Enregistrer();

            return true;
        }

        private void Fermer()
        {
            boolFermetureEnCours = true;
            this.Hide();
        }

        private void frmConfiguration_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (!boolFermetureEnCours)
                Fermer();
        }

        private void frmConfiguration_Load(object sender, System.EventArgs e)
        {
            Configuration.LangueChangee += AppliquerLangue;

            AppliquerLangue();

            cmdCouleurFond.BackColor = Configuration.CouleurFond;

            lstJoueurs = new List<ConfigJoueur>{new ConfigJoueur(), 
                                                new ConfigJoueur(), 
                                                new ConfigJoueur(), 
                                                new ConfigJoueur()};

            //nupNbJoueurs_ValueChanged(new object(), EventArgs.Empty);

            int intNoJoueur = 0;
            foreach (ConfigJoueur objJoueur in Configuration.Joueurs)
            {
                lstJoueurs[intNoJoueur] = new ConfigJoueur(objJoueur.Nom, objJoueur.Humain, objJoueur.NiveauDifficulte);
                intNoJoueur++;

                Control ctrl = TabJoueurs.Controls["gbJoueur" + intNoJoueur];
                ctrl.Enabled = true;
                RadioButton rb = (RadioButton)ctrl.Controls["rbHumain" + intNoJoueur];
                rb.Checked = objJoueur.Humain;
                rb = (RadioButton)ctrl.Controls["rbOrdinateur" + intNoJoueur];
                rb.Checked = !objJoueur.Humain;
                TextBox tb = (TextBox)ctrl.Controls["txtNom" + intNoJoueur];
                tb.Text = objJoueur.Nom;
                ComboBox cb = (ComboBox)ctrl.Controls["cboNiveau" + intNoJoueur];

                cb.SelectedIndex = (int)objJoueur.NiveauDifficulte;
            }

            nupNbJoueurs.Value = intNoJoueur;

            switch (Configuration.Culture.ToString())
            {
                case "fr-FR":
                    {
                        rbFrancais.Checked = true;

                        break;
                    }
                case "en-US":
                    {
                        rbAnglais.Checked = true;

                        break;
                    }
                case "sv-SV":
                    {
                        rbSuedois.Checked = true;

                        break;
                    }
            }

            chkTempsLimite.Checked = Configuration.TempsJeuLimite;
            nudTempsLimite.Value = Configuration.NbSecondesReflexion;
        }

        private void frmConfiguration_Resize(object sender, System.EventArgs e)
        {
        }

        private void cboNiveau_EnabledChanged(object sender, System.EventArgs e)
        {
            // Initialisation de la ComboBox au moment de son activation
            ComboBox cb = (ComboBox)sender;
            if (cb.Enabled && cb.Items.Count == 0)
                foreach (eNiveauDifficulte enumNiveau in Enum.GetValues(typeof(eNiveauDifficulte)))
                    if (enumNiveau != eNiveauDifficulte.Indetermine)
                        cb.Items.Add(enumNiveau.ToString());
        }

        private void cboNiveau_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            int intNoJoueur = int.Parse(cb.Name.Substring(cb.Name.Length - 1));

            lstJoueurs[intNoJoueur - 1].NiveauDifficulte = (eNiveauDifficulte)cb.SelectedIndex;
        }

        private void cmdAnnuler_Click(object sender, System.EventArgs e)
        {
            Fermer();
        }

        private void cmdAppliquer_Click(object sender, System.EventArgs e)
        {
            AppliquerModifications();
        }

        private void cmdCouleurFond_Click(object sender, System.EventArgs e)
        {
            if (ColorDialog1.ShowDialog() == DialogResult.OK)
                cmdCouleurFond.BackColor = ColorDialog1.Color;
        }

        private void cmdOK_Click(object sender, System.EventArgs e)
        {
            if (AppliquerModifications())
                Fermer();
        }

        private void nupNbJoueurs_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                foreach (GroupBox gb in TabJoueurs.Controls.OfType<GroupBox>())
                    gb.Enabled = false;

                for (int intNoJoueur = 1; intNoJoueur <= nupNbJoueurs.Value; intNoJoueur++)
                    TabJoueurs.Controls["gbJoueur" + intNoJoueur].Enabled = true;
            }
            catch (Exception)
            {
            }
        }

        private void rbAnglais_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbAnglais.Checked)
                objCulture = CultureInfo.CreateSpecificCulture("en-US");

        }

        private void rbFrancais_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbFrancais.Checked)
                objCulture = CultureInfo.CreateSpecificCulture("fr-FR");
        }

        private void rbHumain_CheckedChanged(object sender, System.EventArgs e)
        {
            RadioButton rbHumain = (RadioButton)sender;

            /* Il faut activer/désactiver la notion de niveau de jeu
             * en fonction du type de joueur : "Humain" ou "Ordinateur" */
            ComboBox cb = rbHumain.Parent.Controls.OfType<ComboBox>().First();
            cb.SelectedIndex = -1;
            cb.Enabled = !rbHumain.Checked;
            RadioButton rb = rbHumain.Parent.Controls.OfType<RadioButton>().First();
            rb.Checked = !rbHumain.Checked;

            int intNoJoueur = int.Parse(rbHumain.Name.Substring(rbHumain.Name.Length - 1));

            lstJoueurs[intNoJoueur - 1].Humain = rbHumain.Checked;
        }

        private void rbOrdinateur_CheckedChanged(object sender, System.EventArgs e)
        {
            RadioButton rbOrdinateur = (RadioButton)sender;

            /* Il faut activer/désactiver la notion de niveau de jeu
             * en fonction du type de joueur : "Humain" ou "Ordinateur" */
            ComboBox cb = rbOrdinateur.Parent.Controls.OfType<ComboBox>().First();
            cb.Enabled = rbOrdinateur.Checked;
            RadioButton rb = rbOrdinateur.Parent.Controls.OfType<RadioButton>().Last();
            rb.Checked = !rbOrdinateur.Checked;

            int intNoJoueur = int.Parse(rbOrdinateur.Name.Substring(rbOrdinateur.Name.Length - 1));

            lstJoueurs[intNoJoueur - 1].Humain = !rbOrdinateur.Checked;
        }

        private void rbSuedois_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbSuedois.Checked)
                objCulture = CultureInfo.CreateSpecificCulture("sv-SE");
        }

        private void txtNom_TextChanged(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            int intNoJoueur = int.Parse(tb.Name.Substring(tb.Name.Length - 1));

            lstJoueurs[intNoJoueur - 1].Nom = tb.Text;
        }

        #region Onglet "Divers"

        private void chkTempsLimite_CheckedChanged(object sender, System.EventArgs e)
        {
            lblTempsLimite.Enabled = chkTempsLimite.Checked;
            nudTempsLimite.Enabled = chkTempsLimite.Checked;
        }

        #endregion

        public frmConfiguration()
        {
            InitializeComponent();
        }
    }
}
