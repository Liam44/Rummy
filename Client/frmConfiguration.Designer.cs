namespace Client
{
    partial class frmConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfiguration));
            this.txtNom1 = new System.Windows.Forms.TextBox();
            this.Onglets = new System.Windows.Forms.TabControl();
            this.TabJoueurs = new System.Windows.Forms.TabPage();
            this.gbJoueur4 = new System.Windows.Forms.GroupBox();
            this.lblNiveau4 = new System.Windows.Forms.Label();
            this.txtNom4 = new System.Windows.Forms.TextBox();
            this.lblNom4 = new System.Windows.Forms.Label();
            this.cboNiveau4 = new System.Windows.Forms.ComboBox();
            this.rbOrdinateur4 = new System.Windows.Forms.RadioButton();
            this.rbHumain4 = new System.Windows.Forms.RadioButton();
            this.gbJoueur2 = new System.Windows.Forms.GroupBox();
            this.lblNiveau2 = new System.Windows.Forms.Label();
            this.cboNiveau2 = new System.Windows.Forms.ComboBox();
            this.txtNom2 = new System.Windows.Forms.TextBox();
            this.lblNom2 = new System.Windows.Forms.Label();
            this.rbOrdinateur2 = new System.Windows.Forms.RadioButton();
            this.rbHumain2 = new System.Windows.Forms.RadioButton();
            this.gbJoueur3 = new System.Windows.Forms.GroupBox();
            this.lblNiveau3 = new System.Windows.Forms.Label();
            this.txtNom3 = new System.Windows.Forms.TextBox();
            this.cboNiveau3 = new System.Windows.Forms.ComboBox();
            this.lblNom3 = new System.Windows.Forms.Label();
            this.rbOrdinateur3 = new System.Windows.Forms.RadioButton();
            this.rbHumain3 = new System.Windows.Forms.RadioButton();
            this.gbJoueur1 = new System.Windows.Forms.GroupBox();
            this.lblNiveau1 = new System.Windows.Forms.Label();
            this.cboNiveau1 = new System.Windows.Forms.ComboBox();
            this.lblNom1 = new System.Windows.Forms.Label();
            this.rbOrdinateur1 = new System.Windows.Forms.RadioButton();
            this.rbHumain1 = new System.Windows.Forms.RadioButton();
            this.nupNbJoueurs = new System.Windows.Forms.NumericUpDown();
            this.lblNbJoueurs = new System.Windows.Forms.Label();
            this.TabLangues = new System.Windows.Forms.TabPage();
            this.rbSuedois = new System.Windows.Forms.RadioButton();
            this.rbAnglais = new System.Windows.Forms.RadioButton();
            this.rbFrancais = new System.Windows.Forms.RadioButton();
            this.TabDivers = new System.Windows.Forms.TabPage();
            this.nudTempsLimite = new System.Windows.Forms.NumericUpDown();
            this.lblTempsLimite = new System.Windows.Forms.Label();
            this.chkTempsLimite = new System.Windows.Forms.CheckBox();
            this.lblCouleurFond = new System.Windows.Forms.Label();
            this.cmdCouleurFond = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.cmdAnnuler = new System.Windows.Forms.Button();
            this.cmdAppliquer = new System.Windows.Forms.Button();
            this.Onglets.SuspendLayout();
            this.TabJoueurs.SuspendLayout();
            this.gbJoueur4.SuspendLayout();
            this.gbJoueur2.SuspendLayout();
            this.gbJoueur3.SuspendLayout();
            this.gbJoueur1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupNbJoueurs)).BeginInit();
            this.TabLangues.SuspendLayout();
            this.TabDivers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTempsLimite)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNom1
            // 
            this.txtNom1.Location = new System.Drawing.Point(6, 78);
            this.txtNom1.Name = "txtNom1";
            this.txtNom1.Size = new System.Drawing.Size(238, 20);
            this.txtNom1.TabIndex = 2;
            this.txtNom1.TextChanged += new System.EventHandler(this.txtNom_TextChanged);
            // 
            // Onglets
            // 
            this.Onglets.Controls.Add(this.TabJoueurs);
            this.Onglets.Controls.Add(this.TabLangues);
            this.Onglets.Controls.Add(this.TabDivers);
            this.Onglets.Location = new System.Drawing.Point(12, 12);
            this.Onglets.Name = "Onglets";
            this.Onglets.SelectedIndex = 0;
            this.Onglets.Size = new System.Drawing.Size(896, 530);
            this.Onglets.TabIndex = 4;
            // 
            // TabJoueurs
            // 
            this.TabJoueurs.Controls.Add(this.gbJoueur4);
            this.TabJoueurs.Controls.Add(this.gbJoueur2);
            this.TabJoueurs.Controls.Add(this.gbJoueur3);
            this.TabJoueurs.Controls.Add(this.gbJoueur1);
            this.TabJoueurs.Controls.Add(this.nupNbJoueurs);
            this.TabJoueurs.Controls.Add(this.lblNbJoueurs);
            this.TabJoueurs.Location = new System.Drawing.Point(4, 22);
            this.TabJoueurs.Name = "TabJoueurs";
            this.TabJoueurs.Padding = new System.Windows.Forms.Padding(3);
            this.TabJoueurs.Size = new System.Drawing.Size(888, 504);
            this.TabJoueurs.TabIndex = 0;
            this.TabJoueurs.Text = "Joueurs";
            this.TabJoueurs.UseVisualStyleBackColor = true;
            // 
            // gbJoueur4
            // 
            this.gbJoueur4.Controls.Add(this.lblNiveau4);
            this.gbJoueur4.Controls.Add(this.txtNom4);
            this.gbJoueur4.Controls.Add(this.lblNom4);
            this.gbJoueur4.Controls.Add(this.cboNiveau4);
            this.gbJoueur4.Controls.Add(this.rbOrdinateur4);
            this.gbJoueur4.Controls.Add(this.rbHumain4);
            this.gbJoueur4.Enabled = false;
            this.gbJoueur4.Location = new System.Drawing.Point(262, 169);
            this.gbJoueur4.Name = "gbJoueur4";
            this.gbJoueur4.Size = new System.Drawing.Size(250, 131);
            this.gbJoueur4.TabIndex = 4;
            this.gbJoueur4.TabStop = false;
            this.gbJoueur4.Text = "Joueur 4";
            // 
            // lblNiveau4
            // 
            this.lblNiveau4.AutoSize = true;
            this.lblNiveau4.Location = new System.Drawing.Point(6, 107);
            this.lblNiveau4.Name = "lblNiveau4";
            this.lblNiveau4.Size = new System.Drawing.Size(47, 13);
            this.lblNiveau4.TabIndex = 5;
            this.lblNiveau4.Text = "Niveau :";
            // 
            // txtNom4
            // 
            this.txtNom4.Location = new System.Drawing.Point(6, 78);
            this.txtNom4.Name = "txtNom4";
            this.txtNom4.Size = new System.Drawing.Size(238, 20);
            this.txtNom4.TabIndex = 2;
            this.txtNom4.TextChanged += new System.EventHandler(this.txtNom_TextChanged);
            // 
            // lblNom4
            // 
            this.lblNom4.AutoSize = true;
            this.lblNom4.Location = new System.Drawing.Point(6, 62);
            this.lblNom4.Name = "lblNom4";
            this.lblNom4.Size = new System.Drawing.Size(35, 13);
            this.lblNom4.TabIndex = 3;
            this.lblNom4.Text = "Nom :";
            // 
            // cboNiveau4
            // 
            this.cboNiveau4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNiveau4.FormattingEnabled = true;
            this.cboNiveau4.Location = new System.Drawing.Point(59, 104);
            this.cboNiveau4.Name = "cboNiveau4";
            this.cboNiveau4.Size = new System.Drawing.Size(185, 21);
            this.cboNiveau4.TabIndex = 4;
            this.cboNiveau4.SelectedIndexChanged += new System.EventHandler(this.cboNiveau_SelectedIndexChanged);
            this.cboNiveau4.EnabledChanged += new System.EventHandler(this.cboNiveau_EnabledChanged);
            // 
            // rbOrdinateur4
            // 
            this.rbOrdinateur4.AutoSize = true;
            this.rbOrdinateur4.Location = new System.Drawing.Point(6, 42);
            this.rbOrdinateur4.Name = "rbOrdinateur4";
            this.rbOrdinateur4.Size = new System.Drawing.Size(74, 17);
            this.rbOrdinateur4.TabIndex = 1;
            this.rbOrdinateur4.TabStop = true;
            this.rbOrdinateur4.Text = "Ordinateur";
            this.rbOrdinateur4.UseVisualStyleBackColor = true;
            this.rbOrdinateur4.CheckedChanged += new System.EventHandler(this.rbOrdinateur_CheckedChanged);
            // 
            // rbHumain4
            // 
            this.rbHumain4.AutoSize = true;
            this.rbHumain4.Location = new System.Drawing.Point(6, 19);
            this.rbHumain4.Name = "rbHumain4";
            this.rbHumain4.Size = new System.Drawing.Size(61, 17);
            this.rbHumain4.TabIndex = 0;
            this.rbHumain4.TabStop = true;
            this.rbHumain4.Text = "Humain";
            this.rbHumain4.UseVisualStyleBackColor = true;
            this.rbHumain4.CheckedChanged += new System.EventHandler(this.rbHumain_CheckedChanged);
            // 
            // gbJoueur2
            // 
            this.gbJoueur2.Controls.Add(this.lblNiveau2);
            this.gbJoueur2.Controls.Add(this.cboNiveau2);
            this.gbJoueur2.Controls.Add(this.txtNom2);
            this.gbJoueur2.Controls.Add(this.lblNom2);
            this.gbJoueur2.Controls.Add(this.rbOrdinateur2);
            this.gbJoueur2.Controls.Add(this.rbHumain2);
            this.gbJoueur2.Enabled = false;
            this.gbJoueur2.Location = new System.Drawing.Point(262, 32);
            this.gbJoueur2.Name = "gbJoueur2";
            this.gbJoueur2.Size = new System.Drawing.Size(250, 131);
            this.gbJoueur2.TabIndex = 2;
            this.gbJoueur2.TabStop = false;
            this.gbJoueur2.Text = "Joueur 2";
            // 
            // lblNiveau2
            // 
            this.lblNiveau2.AutoSize = true;
            this.lblNiveau2.Location = new System.Drawing.Point(6, 107);
            this.lblNiveau2.Name = "lblNiveau2";
            this.lblNiveau2.Size = new System.Drawing.Size(47, 13);
            this.lblNiveau2.TabIndex = 5;
            this.lblNiveau2.Text = "Niveau :";
            // 
            // cboNiveau2
            // 
            this.cboNiveau2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNiveau2.FormattingEnabled = true;
            this.cboNiveau2.Location = new System.Drawing.Point(59, 104);
            this.cboNiveau2.Name = "cboNiveau2";
            this.cboNiveau2.Size = new System.Drawing.Size(185, 21);
            this.cboNiveau2.TabIndex = 4;
            this.cboNiveau2.SelectedIndexChanged += new System.EventHandler(this.cboNiveau_SelectedIndexChanged);
            this.cboNiveau2.EnabledChanged += new System.EventHandler(this.cboNiveau_EnabledChanged);
            // 
            // txtNom2
            // 
            this.txtNom2.Location = new System.Drawing.Point(6, 78);
            this.txtNom2.Name = "txtNom2";
            this.txtNom2.Size = new System.Drawing.Size(238, 20);
            this.txtNom2.TabIndex = 2;
            this.txtNom2.TextChanged += new System.EventHandler(this.txtNom_TextChanged);
            // 
            // lblNom2
            // 
            this.lblNom2.AutoSize = true;
            this.lblNom2.Location = new System.Drawing.Point(6, 62);
            this.lblNom2.Name = "lblNom2";
            this.lblNom2.Size = new System.Drawing.Size(35, 13);
            this.lblNom2.TabIndex = 3;
            this.lblNom2.Text = "Nom :";
            // 
            // rbOrdinateur2
            // 
            this.rbOrdinateur2.AutoSize = true;
            this.rbOrdinateur2.Location = new System.Drawing.Point(6, 42);
            this.rbOrdinateur2.Name = "rbOrdinateur2";
            this.rbOrdinateur2.Size = new System.Drawing.Size(74, 17);
            this.rbOrdinateur2.TabIndex = 1;
            this.rbOrdinateur2.TabStop = true;
            this.rbOrdinateur2.Text = "Ordinateur";
            this.rbOrdinateur2.UseVisualStyleBackColor = true;
            this.rbOrdinateur2.CheckedChanged += new System.EventHandler(this.rbOrdinateur_CheckedChanged);
            // 
            // rbHumain2
            // 
            this.rbHumain2.AutoSize = true;
            this.rbHumain2.Location = new System.Drawing.Point(6, 19);
            this.rbHumain2.Name = "rbHumain2";
            this.rbHumain2.Size = new System.Drawing.Size(61, 17);
            this.rbHumain2.TabIndex = 0;
            this.rbHumain2.TabStop = true;
            this.rbHumain2.Text = "Humain";
            this.rbHumain2.UseVisualStyleBackColor = true;
            this.rbHumain2.CheckedChanged += new System.EventHandler(this.rbHumain_CheckedChanged);
            // 
            // gbJoueur3
            // 
            this.gbJoueur3.Controls.Add(this.lblNiveau3);
            this.gbJoueur3.Controls.Add(this.txtNom3);
            this.gbJoueur3.Controls.Add(this.cboNiveau3);
            this.gbJoueur3.Controls.Add(this.lblNom3);
            this.gbJoueur3.Controls.Add(this.rbOrdinateur3);
            this.gbJoueur3.Controls.Add(this.rbHumain3);
            this.gbJoueur3.Enabled = false;
            this.gbJoueur3.Location = new System.Drawing.Point(6, 169);
            this.gbJoueur3.Name = "gbJoueur3";
            this.gbJoueur3.Size = new System.Drawing.Size(250, 131);
            this.gbJoueur3.TabIndex = 3;
            this.gbJoueur3.TabStop = false;
            this.gbJoueur3.Text = "Joueur 3";
            // 
            // lblNiveau3
            // 
            this.lblNiveau3.AutoSize = true;
            this.lblNiveau3.Location = new System.Drawing.Point(6, 107);
            this.lblNiveau3.Name = "lblNiveau3";
            this.lblNiveau3.Size = new System.Drawing.Size(47, 13);
            this.lblNiveau3.TabIndex = 5;
            this.lblNiveau3.Text = "Niveau :";
            // 
            // txtNom3
            // 
            this.txtNom3.Location = new System.Drawing.Point(6, 78);
            this.txtNom3.Name = "txtNom3";
            this.txtNom3.Size = new System.Drawing.Size(238, 20);
            this.txtNom3.TabIndex = 2;
            this.txtNom3.TextChanged += new System.EventHandler(this.txtNom_TextChanged);
            // 
            // cboNiveau3
            // 
            this.cboNiveau3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNiveau3.FormattingEnabled = true;
            this.cboNiveau3.Location = new System.Drawing.Point(59, 104);
            this.cboNiveau3.Name = "cboNiveau3";
            this.cboNiveau3.Size = new System.Drawing.Size(185, 21);
            this.cboNiveau3.TabIndex = 4;
            this.cboNiveau3.SelectedIndexChanged += new System.EventHandler(this.cboNiveau_SelectedIndexChanged);
            this.cboNiveau3.EnabledChanged += new System.EventHandler(this.cboNiveau_EnabledChanged);
            // 
            // lblNom3
            // 
            this.lblNom3.AutoSize = true;
            this.lblNom3.Location = new System.Drawing.Point(6, 62);
            this.lblNom3.Name = "lblNom3";
            this.lblNom3.Size = new System.Drawing.Size(35, 13);
            this.lblNom3.TabIndex = 3;
            this.lblNom3.Text = "Nom :";
            // 
            // rbOrdinateur3
            // 
            this.rbOrdinateur3.AutoSize = true;
            this.rbOrdinateur3.Location = new System.Drawing.Point(6, 42);
            this.rbOrdinateur3.Name = "rbOrdinateur3";
            this.rbOrdinateur3.Size = new System.Drawing.Size(74, 17);
            this.rbOrdinateur3.TabIndex = 1;
            this.rbOrdinateur3.TabStop = true;
            this.rbOrdinateur3.Text = "Ordinateur";
            this.rbOrdinateur3.UseVisualStyleBackColor = true;
            this.rbOrdinateur3.CheckedChanged += new System.EventHandler(this.rbOrdinateur_CheckedChanged);
            // 
            // rbHumain3
            // 
            this.rbHumain3.AutoSize = true;
            this.rbHumain3.Location = new System.Drawing.Point(6, 19);
            this.rbHumain3.Name = "rbHumain3";
            this.rbHumain3.Size = new System.Drawing.Size(61, 17);
            this.rbHumain3.TabIndex = 0;
            this.rbHumain3.TabStop = true;
            this.rbHumain3.Text = "Humain";
            this.rbHumain3.UseVisualStyleBackColor = true;
            this.rbHumain3.CheckedChanged += new System.EventHandler(this.rbHumain_CheckedChanged);
            // 
            // gbJoueur1
            // 
            this.gbJoueur1.Controls.Add(this.lblNiveau1);
            this.gbJoueur1.Controls.Add(this.cboNiveau1);
            this.gbJoueur1.Controls.Add(this.txtNom1);
            this.gbJoueur1.Controls.Add(this.lblNom1);
            this.gbJoueur1.Controls.Add(this.rbOrdinateur1);
            this.gbJoueur1.Controls.Add(this.rbHumain1);
            this.gbJoueur1.Enabled = false;
            this.gbJoueur1.Location = new System.Drawing.Point(6, 32);
            this.gbJoueur1.Name = "gbJoueur1";
            this.gbJoueur1.Size = new System.Drawing.Size(250, 131);
            this.gbJoueur1.TabIndex = 1;
            this.gbJoueur1.TabStop = false;
            this.gbJoueur1.Text = "Joueur 1";
            // 
            // lblNiveau1
            // 
            this.lblNiveau1.AutoSize = true;
            this.lblNiveau1.Location = new System.Drawing.Point(6, 107);
            this.lblNiveau1.Name = "lblNiveau1";
            this.lblNiveau1.Size = new System.Drawing.Size(47, 13);
            this.lblNiveau1.TabIndex = 5;
            this.lblNiveau1.Text = "Niveau :";
            // 
            // cboNiveau1
            // 
            this.cboNiveau1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNiveau1.FormattingEnabled = true;
            this.cboNiveau1.Location = new System.Drawing.Point(59, 104);
            this.cboNiveau1.Name = "cboNiveau1";
            this.cboNiveau1.Size = new System.Drawing.Size(185, 21);
            this.cboNiveau1.TabIndex = 4;
            this.cboNiveau1.SelectedIndexChanged += new System.EventHandler(this.cboNiveau_SelectedIndexChanged);
            this.cboNiveau1.EnabledChanged += new System.EventHandler(this.cboNiveau_EnabledChanged);
            // 
            // lblNom1
            // 
            this.lblNom1.AutoSize = true;
            this.lblNom1.Location = new System.Drawing.Point(6, 62);
            this.lblNom1.Name = "lblNom1";
            this.lblNom1.Size = new System.Drawing.Size(35, 13);
            this.lblNom1.TabIndex = 3;
            this.lblNom1.Text = "Nom :";
            // 
            // rbOrdinateur1
            // 
            this.rbOrdinateur1.AutoSize = true;
            this.rbOrdinateur1.Location = new System.Drawing.Point(6, 42);
            this.rbOrdinateur1.Name = "rbOrdinateur1";
            this.rbOrdinateur1.Size = new System.Drawing.Size(74, 17);
            this.rbOrdinateur1.TabIndex = 1;
            this.rbOrdinateur1.TabStop = true;
            this.rbOrdinateur1.Text = "Ordinateur";
            this.rbOrdinateur1.UseVisualStyleBackColor = true;
            this.rbOrdinateur1.CheckedChanged += new System.EventHandler(this.rbOrdinateur_CheckedChanged);
            // 
            // rbHumain1
            // 
            this.rbHumain1.AutoSize = true;
            this.rbHumain1.Location = new System.Drawing.Point(6, 19);
            this.rbHumain1.Name = "rbHumain1";
            this.rbHumain1.Size = new System.Drawing.Size(61, 17);
            this.rbHumain1.TabIndex = 0;
            this.rbHumain1.TabStop = true;
            this.rbHumain1.Text = "Humain";
            this.rbHumain1.UseVisualStyleBackColor = true;
            this.rbHumain1.CheckedChanged += new System.EventHandler(this.rbHumain_CheckedChanged);
            // 
            // nupNbJoueurs
            // 
            this.nupNbJoueurs.Location = new System.Drawing.Point(114, 6);
            this.nupNbJoueurs.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nupNbJoueurs.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nupNbJoueurs.Name = "nupNbJoueurs";
            this.nupNbJoueurs.Size = new System.Drawing.Size(38, 20);
            this.nupNbJoueurs.TabIndex = 0;
            this.nupNbJoueurs.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nupNbJoueurs.ValueChanged += new System.EventHandler(this.nupNbJoueurs_ValueChanged);
            // 
            // lblNbJoueurs
            // 
            this.lblNbJoueurs.AutoSize = true;
            this.lblNbJoueurs.Location = new System.Drawing.Point(6, 8);
            this.lblNbJoueurs.Name = "lblNbJoueurs";
            this.lblNbJoueurs.Size = new System.Drawing.Size(102, 13);
            this.lblNbJoueurs.TabIndex = 3;
            this.lblNbJoueurs.Text = "Nombre de joueurs :";
            // 
            // TabLangues
            // 
            this.TabLangues.Controls.Add(this.rbSuedois);
            this.TabLangues.Controls.Add(this.rbAnglais);
            this.TabLangues.Controls.Add(this.rbFrancais);
            this.TabLangues.Location = new System.Drawing.Point(4, 22);
            this.TabLangues.Name = "TabLangues";
            this.TabLangues.Padding = new System.Windows.Forms.Padding(3);
            this.TabLangues.Size = new System.Drawing.Size(888, 504);
            this.TabLangues.TabIndex = 1;
            this.TabLangues.Text = "Langues";
            this.TabLangues.UseVisualStyleBackColor = true;
            // 
            // rbSuedois
            // 
            this.rbSuedois.AutoSize = true;
            this.rbSuedois.Location = new System.Drawing.Point(6, 52);
            this.rbSuedois.Name = "rbSuedois";
            this.rbSuedois.Size = new System.Drawing.Size(63, 17);
            this.rbSuedois.TabIndex = 2;
            this.rbSuedois.TabStop = true;
            this.rbSuedois.Text = "&Suédois";
            this.rbSuedois.UseVisualStyleBackColor = true;
            this.rbSuedois.CheckedChanged += new System.EventHandler(this.rbSuedois_CheckedChanged);
            // 
            // rbAnglais
            // 
            this.rbAnglais.AutoSize = true;
            this.rbAnglais.Location = new System.Drawing.Point(6, 29);
            this.rbAnglais.Name = "rbAnglais";
            this.rbAnglais.Size = new System.Drawing.Size(59, 17);
            this.rbAnglais.TabIndex = 1;
            this.rbAnglais.TabStop = true;
            this.rbAnglais.Text = "&Anglais";
            this.rbAnglais.UseVisualStyleBackColor = true;
            this.rbAnglais.CheckedChanged += new System.EventHandler(this.rbAnglais_CheckedChanged);
            // 
            // rbFrancais
            // 
            this.rbFrancais.AutoSize = true;
            this.rbFrancais.Location = new System.Drawing.Point(6, 6);
            this.rbFrancais.Name = "rbFrancais";
            this.rbFrancais.Size = new System.Drawing.Size(65, 17);
            this.rbFrancais.TabIndex = 0;
            this.rbFrancais.TabStop = true;
            this.rbFrancais.Text = "&Français";
            this.rbFrancais.UseVisualStyleBackColor = true;
            this.rbFrancais.CheckedChanged += new System.EventHandler(this.rbFrancais_CheckedChanged);
            // 
            // TabDivers
            // 
            this.TabDivers.Controls.Add(this.nudTempsLimite);
            this.TabDivers.Controls.Add(this.lblTempsLimite);
            this.TabDivers.Controls.Add(this.chkTempsLimite);
            this.TabDivers.Controls.Add(this.lblCouleurFond);
            this.TabDivers.Controls.Add(this.cmdCouleurFond);
            this.TabDivers.Location = new System.Drawing.Point(4, 22);
            this.TabDivers.Name = "TabDivers";
            this.TabDivers.Size = new System.Drawing.Size(888, 504);
            this.TabDivers.TabIndex = 2;
            this.TabDivers.Text = "Divers";
            this.TabDivers.UseVisualStyleBackColor = true;
            // 
            // nudTempsLimite
            // 
            this.nudTempsLimite.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTempsLimite.Location = new System.Drawing.Point(198, 55);
            this.nudTempsLimite.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.nudTempsLimite.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTempsLimite.Name = "nudTempsLimite";
            this.nudTempsLimite.Size = new System.Drawing.Size(64, 20);
            this.nudTempsLimite.TabIndex = 4;
            this.nudTempsLimite.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblTempsLimite
            // 
            this.lblTempsLimite.AutoSize = true;
            this.lblTempsLimite.Location = new System.Drawing.Point(20, 57);
            this.lblTempsLimite.Name = "lblTempsLimite";
            this.lblTempsLimite.Size = new System.Drawing.Size(172, 13);
            this.lblTempsLimite.TabIndex = 3;
            this.lblTempsLimite.Text = "Temps de réflexion (en secondes) :";
            // 
            // chkTempsLimite
            // 
            this.chkTempsLimite.AutoSize = true;
            this.chkTempsLimite.Location = new System.Drawing.Point(3, 32);
            this.chkTempsLimite.Name = "chkTempsLimite";
            this.chkTempsLimite.Size = new System.Drawing.Size(116, 17);
            this.chkTempsLimite.TabIndex = 2;
            this.chkTempsLimite.Text = "Temps de jeu limité";
            this.chkTempsLimite.UseVisualStyleBackColor = true;
            this.chkTempsLimite.CheckedChanged += new System.EventHandler(this.chkTempsLimite_CheckedChanged);
            // 
            // lblCouleurFond
            // 
            this.lblCouleurFond.AutoSize = true;
            this.lblCouleurFond.Location = new System.Drawing.Point(3, 8);
            this.lblCouleurFond.Name = "lblCouleurFond";
            this.lblCouleurFond.Size = new System.Drawing.Size(120, 13);
            this.lblCouleurFond.TabIndex = 0;
            this.lblCouleurFond.Text = "Couleur du fond de jeu :";
            // 
            // cmdCouleurFond
            // 
            this.cmdCouleurFond.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cmdCouleurFond.Location = new System.Drawing.Point(129, 3);
            this.cmdCouleurFond.Name = "cmdCouleurFond";
            this.cmdCouleurFond.Size = new System.Drawing.Size(23, 23);
            this.cmdCouleurFond.TabIndex = 1;
            this.cmdCouleurFond.UseVisualStyleBackColor = true;
            this.cmdCouleurFond.Click += new System.EventHandler(this.cmdCouleurFond_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(671, 633);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 5;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdAnnuler
            // 
            this.cmdAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdAnnuler.Location = new System.Drawing.Point(833, 633);
            this.cmdAnnuler.Name = "cmdAnnuler";
            this.cmdAnnuler.Size = new System.Drawing.Size(75, 23);
            this.cmdAnnuler.TabIndex = 7;
            this.cmdAnnuler.Text = "&Annuler";
            this.cmdAnnuler.UseVisualStyleBackColor = true;
            this.cmdAnnuler.Click += new System.EventHandler(this.cmdAnnuler_Click);
            // 
            // cmdAppliquer
            // 
            this.cmdAppliquer.Location = new System.Drawing.Point(752, 633);
            this.cmdAppliquer.Name = "cmdAppliquer";
            this.cmdAppliquer.Size = new System.Drawing.Size(75, 23);
            this.cmdAppliquer.TabIndex = 6;
            this.cmdAppliquer.Text = "&Appliquer";
            this.cmdAppliquer.UseVisualStyleBackColor = true;
            this.cmdAppliquer.Click += new System.EventHandler(this.cmdAppliquer_Click);
            // 
            // frmConfiguration
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdAnnuler;
            this.ClientSize = new System.Drawing.Size(920, 668);
            this.Controls.Add(this.Onglets);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdAnnuler);
            this.Controls.Add(this.cmdAppliquer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmConfiguration";
            this.Text = "Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfiguration_FormClosing);
            this.Load += new System.EventHandler(this.frmConfiguration_Load);
            this.Resize += new System.EventHandler(this.frmConfiguration_Resize);
            this.Onglets.ResumeLayout(false);
            this.TabJoueurs.ResumeLayout(false);
            this.TabJoueurs.PerformLayout();
            this.gbJoueur4.ResumeLayout(false);
            this.gbJoueur4.PerformLayout();
            this.gbJoueur2.ResumeLayout(false);
            this.gbJoueur2.PerformLayout();
            this.gbJoueur3.ResumeLayout(false);
            this.gbJoueur3.PerformLayout();
            this.gbJoueur1.ResumeLayout(false);
            this.gbJoueur1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupNbJoueurs)).EndInit();
            this.TabLangues.ResumeLayout(false);
            this.TabLangues.PerformLayout();
            this.TabDivers.ResumeLayout(false);
            this.TabDivers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTempsLimite)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TextBox txtNom1;
        internal System.Windows.Forms.TabControl Onglets;
        internal System.Windows.Forms.TabPage TabJoueurs;
        internal System.Windows.Forms.GroupBox gbJoueur4;
        internal System.Windows.Forms.Label lblNiveau4;
        internal System.Windows.Forms.TextBox txtNom4;
        internal System.Windows.Forms.Label lblNom4;
        internal System.Windows.Forms.ComboBox cboNiveau4;
        internal System.Windows.Forms.RadioButton rbOrdinateur4;
        internal System.Windows.Forms.RadioButton rbHumain4;
        internal System.Windows.Forms.GroupBox gbJoueur2;
        internal System.Windows.Forms.Label lblNiveau2;
        internal System.Windows.Forms.ComboBox cboNiveau2;
        internal System.Windows.Forms.TextBox txtNom2;
        internal System.Windows.Forms.Label lblNom2;
        internal System.Windows.Forms.RadioButton rbOrdinateur2;
        internal System.Windows.Forms.RadioButton rbHumain2;
        internal System.Windows.Forms.GroupBox gbJoueur3;
        internal System.Windows.Forms.Label lblNiveau3;
        internal System.Windows.Forms.TextBox txtNom3;
        internal System.Windows.Forms.ComboBox cboNiveau3;
        internal System.Windows.Forms.Label lblNom3;
        internal System.Windows.Forms.RadioButton rbOrdinateur3;
        internal System.Windows.Forms.RadioButton rbHumain3;
        internal System.Windows.Forms.GroupBox gbJoueur1;
        internal System.Windows.Forms.Label lblNiveau1;
        internal System.Windows.Forms.ComboBox cboNiveau1;
        internal System.Windows.Forms.Label lblNom1;
        internal System.Windows.Forms.RadioButton rbOrdinateur1;
        internal System.Windows.Forms.RadioButton rbHumain1;
        internal System.Windows.Forms.NumericUpDown nupNbJoueurs;
        internal System.Windows.Forms.Label lblNbJoueurs;
        internal System.Windows.Forms.TabPage TabLangues;
        internal System.Windows.Forms.RadioButton rbSuedois;
        internal System.Windows.Forms.RadioButton rbAnglais;
        internal System.Windows.Forms.RadioButton rbFrancais;
        internal System.Windows.Forms.TabPage TabDivers;
        internal System.Windows.Forms.Label lblCouleurFond;
        internal System.Windows.Forms.Button cmdCouleurFond;
        internal System.Windows.Forms.Button cmdOK;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.Button cmdAnnuler;
        internal System.Windows.Forms.Button cmdAppliquer;
        internal System.Windows.Forms.NumericUpDown nudTempsLimite;
        internal System.Windows.Forms.Label lblTempsLimite;
        internal System.Windows.Forms.CheckBox chkTempsLimite;
    }
}