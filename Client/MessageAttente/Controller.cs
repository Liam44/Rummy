using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client.MessageAttente
{
    class Controller : IController
    {
        private ITacheDeportee mWorker = null;
        private Form frmClient;
        private bool boolRunning;
        private bool boolCanceled;

        /* cette signature de délégué correspond à celle de 
         * IClient.Completed et permet d'appeler en toute
         * sécurité cette méthode sur la thread UI */
        private delegate void CompletedDelegate(bool cancelled);

        /* cette signature de délégué correspond à celle de 
         * IClient.Display et permet d'appeler en toute
         * sécurité cette méthode sur la thread UI */
        private delegate void DisplayDelegate();

        /* cette signature de délégué correspond à celle de 
         * IClient.SetText et permet d'appeler en toute
         * sécurité cette méthode sur la thread UI */
        private delegate void SetTextDelegate(string text);

        /* cette signature de délégué correspond à celle de 
         * IClient.Failed et permet d'appeler en toute
         * sécurité cette méthode sur la thread UI */
        private delegate void FailedDelegate(Exception e);

        /* cette signature de délégué correspond à celle de 
         * IClient.SetVisible et permet d'appeler en toute
         * sécurité cette méthode sur la thread UI */
        private delegate void SetVisibleDelegate(bool visible);

        #region Code called from UI thread

        // Initialise le contrôleur avec un client
        public Controller(IClient client)
        {
            frmClient = (Form)client;
            boolCanceled = false;
        }

        /* Cette méthode est appelée par l'UI et
         * s'exécute sur la thread UI. C'est là que nous allons 
         * démarrer la thread de travail */
        public void Start(ITacheDeportee worker = null)
        {
            // si elle s'exécute déjà, générer une erreur 
            if (boolRunning)
                throw new Exception("Background process already running");

            boolRunning = true;

            if (worker != null)
            {
                /* stocke une référence à l'objet worker
                 * et initialise l'objet worker afin qu'il ait
                 * une référence à l'objet Controller */
                mWorker = worker;
                mWorker.Initialize(this);

                /* crée la thread d'arrière-plan pour
                 * le traitement d'arrière-plan */
                Thread backThread = new Thread(mWorker.Start);

                // démarre le travail d'arrière-plan
                backThread.Start();
            }

            // indique au client que le travail d'arrière-plan a démarré
            ((IClient)frmClient).Start(this);
        }

        /* ce code est appelé par l'UI et 
         * s'exécute donc sur la thread UI. Il définit simplement
         * un indicateur pour demander une annulation */
        public void Cancel()
        {
            boolRunning = false;
            boolCanceled = true;
        }

        #endregion

        #region Code called from the worker thread

        /* code appelé à partir de la thread de travail pour mettre à jour l'affichage
         * ceci déclenche un appel de méthode passé à l'UI avec le 
         * texte d'état - et cet appel est fait sur la
         * thread UI */
        public void Display()
        {
            try
            {
                DisplayDelegate disp = new DisplayDelegate(((IClient)frmClient).Display);

                /* appelle le formulaire client sur la thread UI
                 * pour mettre à jour l'affichage */
                frmClient.BeginInvoke(disp);
            }
            catch (Exception)
            {
            }
        }

        /* code appelé à partir de la thread de travail pour mettre à jour l'affichage
         * ceci déclenche un appel de méthode passé à l'UI avec le 
         * texte d'état - et cet appel est fait sur la
         * thread UI */
        public void SetText(string text)
        {
            try
            {
                SetTextDelegate disp = new SetTextDelegate(((IClient)frmClient).SetText);
                object[] ar = { text };

                /* appelle le formulaire client sur la thread UI
                 * pour mettre à jour l'affichage */
                frmClient.BeginInvoke(disp, ar);
            }
            catch
            {
                throw;
            }
        }

        /* code appelé à partir de la thread de travail pour indiquer l'échec
         * ceci déclenche un appel de méthode à l'UI avec 
         * l'objet exception - et cet appel est effectué sur 
         * la thread UI */
        public void Failed(Exception e)
        {
            boolRunning = false;
            FailedDelegate disp = new FailedDelegate(((IClient)frmClient).Failed);
            object[] ar = { e };

            /* appelle le formulaire client sur la thread UI
             * pour signaler l'échec /*/
            frmClient.Invoke(disp, ar);
        }

        /* code appelé à partir de la thread de travail pour mettre à jour l'affichage
         * ceci déclenche un appel de méthode passé à l'UI avec le 
         * texte d'état - et cet appel est fait sur la
         * thread UI */
        public void SetVisible(bool visible)
        {
            try
            {
                SetVisibleDelegate disp = new SetVisibleDelegate(((IClient)frmClient).SetVisible);
                object[] ar = { visible };

                /* appelle le formulaire client sur la thread UI
                 * pour mettre à jour l'affichage */
                frmClient.BeginInvoke(disp, ar);
            }
            catch
            {
            }
        }

        /* code appelé à partir de la thread de travail pour indiquer l'achèvement
         * nous passons aussi un paramètre pour indiquer si nous
         * avons réellement terminé ou si nous avons annulé l'opération
         * l'appel à l'UI est fait sur la thread UI */
        public void Completed(bool cancelled)
        {
            try
            {
                boolRunning = false;
                boolCanceled = cancelled;
                CompletedDelegate comp = new CompletedDelegate(((IClient)frmClient).Completed);
                object[] ar = { cancelled };

                /* appelle le formulaire client sur la thread UI
                 * pour indiquer l'achèvement */
                frmClient.Invoke(comp, ar);
            }
            catch (Exception)
            {
            }
        }

        /* indique si l'exécution se poursuit ou 
         * si une annulation a été demandée
         * ce code est appelé sur la thread de travail afin que 
         * le code de travail sache s'il doit s'arrêter 
         * de façon ordonnée */
        public bool Running
        {
            get
            {
                return boolRunning;
            }
        }

        /* indique si l'exécution se poursuit ou 
         * si une annulation a été demandée
         * ce code est appelé sur la thread de travail afin que 
         * le code de travail sache s'il doit s'arrêter 
         * de façon ordonnée */
        public bool Canceled
        {
            get
            {
                return boolCanceled;
            }
        }

        #endregion
    }
}
