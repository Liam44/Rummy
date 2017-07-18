namespace Client.MessageAttente
{
    partial class ActivityBar
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
            this.components = new System.ComponentModel.Container();
            this.tmAnim = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmAnim
            // 
            this.tmAnim.Interval = 300;
            this.tmAnim.Tick += new System.EventHandler(this.tmAnim_Tick);
            // 
            // ActivityBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ActivityBar";
            this.Size = new System.Drawing.Size(110, 20);
            this.Load += new System.EventHandler(this.ActivityBar_Load);
            this.Resize += new System.EventHandler(this.ActivityBar_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Timer tmAnim;
    }
}
