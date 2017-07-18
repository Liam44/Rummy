using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
using System.Reflection;
using System.Threading;

using System.Windows.Forms;

using NoyauCommun.Erreurs;

namespace Client.MessageAttente
{
    public partial class MessageAttente : Form
    {

        private Thread thrMessage;
        private F_Message frmMessage;

        internal void Fermer()
        {
            try
            {
                if (thrMessage != null)
                    switch (thrMessage.ThreadState)
                    {
                        case ThreadState.Running:
                        case ThreadState.WaitSleepJoin:
                            {
                                if (!thrMessage.Join(100))
                                {
                                    frmMessage.Cancel();
                                    thrMessage.Abort();
                                }

                                break;
                            }
                        default:
                            {
                                if (!frmMessage.Visible)
                                {
                                    frmMessage.Cancel();
                                    thrMessage.Abort();
                                }

                                break;
                            }
                    }
            }
            catch (ThreadAbortException ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }

        }

        internal IController Controller
        {
            get
            {
                return frmMessage.Controller;
            }
        }

        internal void SetMessage(string texte)
        {
            frmMessage.Controller.SetText(texte);
        }

        internal void SetVisible(bool visible)
        {
            frmMessage.Controller.SetVisible(visible);
        }

        public override string ToString()
        {
            return frmMessage.Text + " => " + frmMessage.MessageAffiche;
        }

        internal MessageAttente(string message,
                                string titre,
                                bool boutonAnnuler = false)
        {
            try
            {
                frmMessage = new F_Message(message, titre, boutonAnnuler);
                thrMessage = new Thread(frmMessage.ShowMe);
                thrMessage.Start();
            }
            catch (ThreadAbortException ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
            catch (Exception ex)
            {
                GestionErreurs.EnregistrerErreur(ex);
            }
        }

        private class F_Message : Form, IClient
        {
            private Label lbl;
            private ActivityBar ab;
            private Button cmdAnnuler;

            private Controller mController;

            private const int MARGE_HAUT = 12;
            private const int MARGE_BAS = 12;
            private const int MARGE_GAUCHE = 6;
            private const int MARGE_DROITE = 6;
            private const int MARGE_INTERMEDIAIRE = 6;

            #region IClient

            public void Start(IController controller)
            {
                ab.Start();
            }

            public void Display()
            {
            }

            public void SetText(string texte)
            {
                lbl.Text = texte;
            }

            public void Failed(Exception e)
            {
                ab.Stop();
                MessageBox.Show(e.ToString());
            }

            public void Completed(bool cancelled)
            {
                try
                {
                    ab.Stop();
                }
                catch (Exception ex)
                {
                    GestionErreurs.EnregistrerErreur(ex);
                }
            }

            public void SetVisible(bool visible)
            {
                this.Visible = visible;
            }

            #endregion

            #region Gestion du Controller"

            protected internal IController Controller
            {
                get
                {
                    return mController;
                }
            }

            protected internal void Cancel()
            {
                mController.Cancel();
                Cursor.Current = Cursors.Default;
            }

            #endregion

            protected internal string MessageAffiche
            {
                get
                {
                    return lbl.Text;
                }
            }

            protected internal F_Message(string message,
                                         string titre,
                                         bool boutonAnnuler)
            {
                mController = new Controller(this);

                lbl = new Label();
                lbl.Top = MARGE_HAUT;
                lbl.Left = MARGE_GAUCHE;
                lbl.Text = message;
                lbl.AutoSize = true;
                lbl.Name = "lbl";
                this.Controls.Add(lbl);

                int Top = lbl.Top + lbl.Height;

                ab = new ActivityBar(20);
                ab.Top = Top + MARGE_INTERMEDIAIRE;
                Top += ab.Height;
                ab.Left = MARGE_GAUCHE;
                ab.Width = lbl.Width;
                this.Controls.Add(ab);

                if (boutonAnnuler)
                {
                    cmdAnnuler = new Button();
                    cmdAnnuler.Text = "&Annuler";
                    cmdAnnuler.Top = Top + MARGE_INTERMEDIAIRE;
                    Top += cmdAnnuler.Height;
                    cmdAnnuler.Click += BoutonAnnuler_Click;
                    this.Controls.Add(cmdAnnuler);
                    this.CancelButton = cmdAnnuler;
                }

                this.ClientSize = new System.Drawing.Size(MARGE_GAUCHE + lbl.Width + MARGE_DROITE, Top + MARGE_BAS);

                if (boutonAnnuler)
                    cmdAnnuler.Left = this.ClientSize.Width - cmdAnnuler.Width - MARGE_DROITE;

                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MinimizeBox = false;
                this.MaximizeBox = false;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.ShowInTaskbar = false;
                this.ShowIcon = true;
                this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                this.Text = titre;

                // Affectation de la gestion des événements
                this.Activated += new EventHandler(F_Message_Activated);
                this.GotFocus += new EventHandler(F_Message_GotFocus);
                this.Shown += new EventHandler(F_Message_Shown);
            }

            protected internal void ShowMe()
            {
                try
                {
                    this.ShowDialog();
                }
                catch (InvalidOperationException)
                {
                }
            }

            private const int CS_NOCLOSE = 0x200;

            /// <summary>
            /// Configure la barre de titre de la fenêtre pour masquer la croix de fermeture
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ClassStyle = cp.ClassStyle | CS_NOCLOSE;
                    return cp;
                }
            }

            private void F_Message_Activated(object sender, System.EventArgs e)
            {
                try
                {
                    this.Refresh();
                }
                catch (InvalidOperationException)
                {
                    this.Close();
                }
            }

            private void F_Message_GotFocus(object sender, System.EventArgs e)
            {
                this.Refresh();
            }

            private void F_Message_Shown(object sender, System.EventArgs e)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    mController.Start(new TacheDeportee());
                }
                catch (ThreadAbortException)
                {
                    // Aucun traitement : cette exception est levée au moment où le Thread est brutalement arrêté
                }
                catch (Exception ex)
                {
                    GestionErreurs.EnregistrerErreur(ex);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }

            private void BoutonAnnuler_Click(object sender, System.EventArgs e)
            {
                this.Cancel();
            }
        }

        private class TacheDeportee : ITacheDeportee
        {
            private IController mController;

            public void Initialize(IController controller)
            {
                mController = controller;
            }

            public void Start()
            {
                try
                {
                    while (mController.Running)
                    {
                        mController.Display();
                        System.Threading.Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    GestionErreurs.EnregistrerErreur(ex);
                }
            }
        }
    }
}
