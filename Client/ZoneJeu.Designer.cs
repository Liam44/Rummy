namespace Client
{
    partial class ZoneJeu
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MenuContextuel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MCDiviserCombinaison = new System.Windows.Forms.ToolStripMenuItem();
            this.MCD_Avant = new System.Windows.Forms.ToolStripMenuItem();
            this.MCD_Apres = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuContextuel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuContextuel
            // 
            this.MenuContextuel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MCDiviserCombinaison});
            this.MenuContextuel.Name = "MenuContextuel";
            this.MenuContextuel.Size = new System.Drawing.Size(206, 26);
            // 
            // MCDiviserCombinaison
            // 
            this.MCDiviserCombinaison.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MCD_Avant,
            this.MCD_Apres});
            this.MCDiviserCombinaison.Name = "MCDiviserCombinaison";
            this.MCDiviserCombinaison.Size = new System.Drawing.Size(205, 22);
            this.MCDiviserCombinaison.Text = "&Diviser la combinaison ...";
            this.MCDiviserCombinaison.MouseHover += new System.EventHandler(this.MCDiviserCombinaison_MouseHover);
            // 
            // MCD_Avant
            // 
            this.MCD_Avant.Name = "MCD_Avant";
            this.MCD_Avant.Size = new System.Drawing.Size(105, 22);
            this.MCD_Avant.Text = "A&vant";
            this.MCD_Avant.Click += new System.EventHandler(this.MCD_Avant_Click);
            // 
            // MCD_Apres
            // 
            this.MCD_Apres.Name = "MCD_Apres";
            this.MCD_Apres.Size = new System.Drawing.Size(105, 22);
            this.MCD_Apres.Text = "A&près";
            this.MCD_Apres.Click += new System.EventHandler(this.MCD_Apres_Click);
            // 
            // ZoneJeu
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Name = "ZoneJeu";
            this.Size = new System.Drawing.Size(602, 236);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ZoneJeu_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ZoneJeu_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ZoneJeu_DragOver);
            this.DragLeave += new System.EventHandler(this.ZoneJeu_DragLeave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ZoneJeu_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ZoneJeu_MouseUp);
            this.Resize += new System.EventHandler(this.ZoneJeu_Resize);
            this.MenuContextuel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ContextMenuStrip MenuContextuel;
        internal System.Windows.Forms.ToolStripMenuItem MCDiviserCombinaison;
        internal System.Windows.Forms.ToolStripMenuItem MCD_Avant;
        internal System.Windows.Forms.ToolStripMenuItem MCD_Apres;
    }
}
