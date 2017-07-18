namespace NoyauCommun
{
    namespace Cartes
    {
        partial class Carte
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
                this.pbCarte = new System.Windows.Forms.PictureBox();
                ((System.ComponentModel.ISupportInitialize)(this.pbCarte)).BeginInit();
                this.SuspendLayout();
                // 
                // pbCarte
                // 
                this.pbCarte.Location = new System.Drawing.Point(3, 3);
                this.pbCarte.Name = "pbCarte";
                this.pbCarte.Size = new System.Drawing.Size(144, 144);
                this.pbCarte.TabIndex = 0;
                this.pbCarte.TabStop = false;
                this.pbCarte.Click += new System.EventHandler(this.Carte_Click);
                this.pbCarte.DragDrop += new System.Windows.Forms.DragEventHandler(this.Carte_DragDrop);
                this.pbCarte.DragEnter += new System.Windows.Forms.DragEventHandler(this.Carte_DragEnter);
                this.pbCarte.DragLeave += new System.EventHandler(this.Carte_DragLeave);
                this.pbCarte.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Carte_MouseDown);
                this.pbCarte.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Carte_MouseMove);
                this.pbCarte.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Carte_MouseUp);
                // 
                // Carte
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Controls.Add(this.pbCarte);
                this.Name = "Carte";
                this.Click += new System.EventHandler(this.Carte_Click);
                this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Carte_DragDrop);
                this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Carte_DragEnter);
                this.DragLeave += new System.EventHandler(this.Carte_DragLeave);
                this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Carte_MouseDown);
                this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Carte_MouseMove);
                this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Carte_MouseUp);
                ((System.ComponentModel.ISupportInitialize)(this.pbCarte)).EndInit();
                this.ResumeLayout(false);

            }

            #endregion

            private System.Windows.Forms.PictureBox pbCarte;
        }
    }
}