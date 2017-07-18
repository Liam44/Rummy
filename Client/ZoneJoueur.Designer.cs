namespace Client
{
    partial class ZoneJoueur
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ZoneJoueur
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Name = "ZoneJoueur";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ZoneJoueur_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ZoneJoueur_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ZoneJoueur_DragOver);
            this.DragLeave += new System.EventHandler(this.ZoneJoueur_DragLeave);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ZoneJoueur_Paint);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ZoneJoueur_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
