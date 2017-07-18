namespace Client
{
    partial class Rummy
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Rummy));
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.MA_Refaire = new System.Windows.Forms.ToolStripMenuItem();
            this.MA_RefaireTout = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.MO_Scores = new System.Windows.Forms.ToolStripMenuItem();
            this.MO_Configuration = new System.Windows.Forms.ToolStripMenuItem();
            this.MA_AnnulerTout = new System.Windows.Forms.ToolStripMenuItem();
            this.MA_Annuler = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuJeu = new System.Windows.Forms.ToolStripMenuItem();
            this.MJ_Nouveau = new System.Windows.Forms.ToolStripMenuItem();
            this.MJ_Rafraichir = new System.Windows.Forms.ToolStripMenuItem();
            this.MJ_Quitter = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuActions = new System.Windows.Forms.ToolStripMenuItem();
            this.timTempsRestant = new System.Windows.Forms.Timer(this.components);
            this.MenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.StatusStrip1.Location = new System.Drawing.Point(0, 714);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.StatusStrip1.Size = new System.Drawing.Size(1219, 22);
            this.StatusStrip1.TabIndex = 2;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // MA_Refaire
            // 
            this.MA_Refaire.Name = "MA_Refaire";
            this.MA_Refaire.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.MA_Refaire.Size = new System.Drawing.Size(257, 26);
            this.MA_Refaire.Text = "&Refaire";
            this.MA_Refaire.Click += new System.EventHandler(this.MA_Refaire_Click);
            // 
            // MA_RefaireTout
            // 
            this.MA_RefaireTout.Name = "MA_RefaireTout";
            this.MA_RefaireTout.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Y)));
            this.MA_RefaireTout.Size = new System.Drawing.Size(257, 26);
            this.MA_RefaireTout.Text = "R&efaire tout";
            this.MA_RefaireTout.Click += new System.EventHandler(this.MA_RefaireTout_Click);
            // 
            // MenuOptions
            // 
            this.MenuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MO_Scores,
            this.MO_Configuration});
            this.MenuOptions.Name = "MenuOptions";
            this.MenuOptions.Size = new System.Drawing.Size(73, 24);
            this.MenuOptions.Text = "&Options";
            // 
            // MO_Scores
            // 
            this.MO_Scores.Name = "MO_Scores";
            this.MO_Scores.Size = new System.Drawing.Size(175, 26);
            this.MO_Scores.Text = "&Scores";
            this.MO_Scores.Click += new System.EventHandler(this.MO_Scores_Click);
            // 
            // MO_Configuration
            // 
            this.MO_Configuration.Name = "MO_Configuration";
            this.MO_Configuration.Size = new System.Drawing.Size(175, 26);
            this.MO_Configuration.Text = "&Configuration";
            this.MO_Configuration.Click += new System.EventHandler(this.MO_Configuration_Click);
            // 
            // MA_AnnulerTout
            // 
            this.MA_AnnulerTout.Name = "MA_AnnulerTout";
            this.MA_AnnulerTout.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.MA_AnnulerTout.Size = new System.Drawing.Size(257, 26);
            this.MA_AnnulerTout.Text = "A&nnuler tout";
            this.MA_AnnulerTout.Click += new System.EventHandler(this.MA_AnnulerTout_Click);
            // 
            // MA_Annuler
            // 
            this.MA_Annuler.Name = "MA_Annuler";
            this.MA_Annuler.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.MA_Annuler.Size = new System.Drawing.Size(257, 26);
            this.MA_Annuler.Text = "&Annuler";
            this.MA_Annuler.Click += new System.EventHandler(this.MA_Annuler_Click);
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuJeu,
            this.MenuActions,
            this.MenuOptions});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.MenuStrip1.Size = new System.Drawing.Size(1219, 28);
            this.MenuStrip1.TabIndex = 3;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // MenuJeu
            // 
            this.MenuJeu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MJ_Nouveau,
            this.MJ_Rafraichir,
            this.MJ_Quitter});
            this.MenuJeu.Name = "MenuJeu";
            this.MenuJeu.Size = new System.Drawing.Size(42, 24);
            this.MenuJeu.Text = "&Jeu";
            // 
            // MJ_Nouveau
            // 
            this.MJ_Nouveau.Name = "MJ_Nouveau";
            this.MJ_Nouveau.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.MJ_Nouveau.Size = new System.Drawing.Size(210, 26);
            this.MJ_Nouveau.Text = "&Nouvelle partie";
            this.MJ_Nouveau.Click += new System.EventHandler(this.MJ_Nouveau_Click);
            // 
            // MJ_Rafraichir
            // 
            this.MJ_Rafraichir.Name = "MJ_Rafraichir";
            this.MJ_Rafraichir.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.MJ_Rafraichir.Size = new System.Drawing.Size(210, 26);
            this.MJ_Rafraichir.Text = "&Rafraîchir";
            this.MJ_Rafraichir.Click += new System.EventHandler(this.MJ_Rafraichir_Click);
            // 
            // MJ_Quitter
            // 
            this.MJ_Quitter.Name = "MJ_Quitter";
            this.MJ_Quitter.Size = new System.Drawing.Size(210, 26);
            this.MJ_Quitter.Text = "&Quitter";
            this.MJ_Quitter.Click += new System.EventHandler(this.MJ_Quitter_Click);
            // 
            // MenuActions
            // 
            this.MenuActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MA_Annuler,
            this.MA_AnnulerTout,
            this.MA_Refaire,
            this.MA_RefaireTout});
            this.MenuActions.Name = "MenuActions";
            this.MenuActions.Size = new System.Drawing.Size(70, 24);
            this.MenuActions.Text = "&Actions";
            // 
            // Rummy
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1219, 736);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.MenuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Rummy";
            this.Text = "Rummy";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Rummy_FormClosing);
            this.Load += new System.EventHandler(this.Rummy_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Rummy_KeyDown);
            this.Resize += new System.EventHandler(this.Rummy_Resize);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.StatusStrip StatusStrip1;
        internal System.Windows.Forms.ToolStripMenuItem MA_Refaire;
        internal System.Windows.Forms.ToolStripMenuItem MA_RefaireTout;
        internal System.Windows.Forms.ToolStripMenuItem MenuOptions;
        internal System.Windows.Forms.ToolStripMenuItem MO_Scores;
        internal System.Windows.Forms.ToolStripMenuItem MO_Configuration;
        internal System.Windows.Forms.ToolStripMenuItem MA_AnnulerTout;
        internal System.Windows.Forms.ToolStripMenuItem MA_Annuler;
        internal System.Windows.Forms.MenuStrip MenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem MenuJeu;
        internal System.Windows.Forms.ToolStripMenuItem MJ_Nouveau;
        internal System.Windows.Forms.ToolStripMenuItem MJ_Quitter;
        internal System.Windows.Forms.ToolStripMenuItem MenuActions;
        internal System.Windows.Forms.Timer timTempsRestant;
        private System.Windows.Forms.ToolStripMenuItem MJ_Rafraichir;
    }
}

