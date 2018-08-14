namespace MigratorUI
{
    partial class NewRepositoryDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewRepositoryDialog));
            this.repositoryLocationTextBox = new System.Windows.Forms.TextBox();
            this.locationLabel = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.repositoryTypeComboBox = new System.Windows.Forms.ComboBox();
            this.repositoryDescriptionLabel = new System.Windows.Forms.Label();
            this.repositoryDescriptionTextbox = new System.Windows.Forms.TextBox();
            this.createRepositoryButton = new System.Windows.Forms.Button();
            this.repositoryNameLabel = new System.Windows.Forms.Label();
            this.repositoryNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // repositoryLocationTextBox
            // 
            this.repositoryLocationTextBox.Location = new System.Drawing.Point(115, 107);
            this.repositoryLocationTextBox.Name = "repositoryLocationTextBox";
            this.repositoryLocationTextBox.Size = new System.Drawing.Size(406, 22);
            this.repositoryLocationTextBox.TabIndex = 5;
            this.repositoryLocationTextBox.Text = "%PROGRAMDATA%\\Tricentis\\TestDataService\\";
            this.repositoryLocationTextBox.TextChanged += new System.EventHandler(this.RepositoryLocationTextBox_TextChanged);
            // 
            // locationLabel
            // 
            this.locationLabel.AutoSize = true;
            this.locationLabel.BackColor = System.Drawing.Color.Transparent;
            this.locationLabel.Location = new System.Drawing.Point(42, 110);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.Size = new System.Drawing.Size(62, 17);
            this.locationLabel.TabIndex = 40;
            this.locationLabel.Text = "Location";
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.BackColor = System.Drawing.Color.Transparent;
            this.typeLabel.Location = new System.Drawing.Point(64, 80);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(40, 17);
            this.typeLabel.TabIndex = 39;
            this.typeLabel.Text = "Type";
            // 
            // repositoryTypeComboBox
            // 
            this.repositoryTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.repositoryTypeComboBox.FormattingEnabled = true;
            this.repositoryTypeComboBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.repositoryTypeComboBox.Location = new System.Drawing.Point(115, 77);
            this.repositoryTypeComboBox.Name = "repositoryTypeComboBox";
            this.repositoryTypeComboBox.Size = new System.Drawing.Size(406, 24);
            this.repositoryTypeComboBox.TabIndex = 4;
            this.repositoryTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.RepositoryTypeComboBox_SelectedIndexChanged);
            // 
            // repositoryDescriptionLabel
            // 
            this.repositoryDescriptionLabel.AutoSize = true;
            this.repositoryDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.repositoryDescriptionLabel.Location = new System.Drawing.Point(25, 51);
            this.repositoryDescriptionLabel.Name = "repositoryDescriptionLabel";
            this.repositoryDescriptionLabel.Size = new System.Drawing.Size(79, 17);
            this.repositoryDescriptionLabel.TabIndex = 37;
            this.repositoryDescriptionLabel.Text = "Description";
            // 
            // repositoryDescriptionTextbox
            // 
            this.repositoryDescriptionTextbox.Location = new System.Drawing.Point(115, 48);
            this.repositoryDescriptionTextbox.Name = "repositoryDescriptionTextbox";
            this.repositoryDescriptionTextbox.Size = new System.Drawing.Size(406, 22);
            this.repositoryDescriptionTextbox.TabIndex = 2;
            // 
            // createRepositoryButton
            // 
            this.createRepositoryButton.Location = new System.Drawing.Point(115, 147);
            this.createRepositoryButton.Name = "createRepositoryButton";
            this.createRepositoryButton.Size = new System.Drawing.Size(406, 32);
            this.createRepositoryButton.TabIndex = 3;
            this.createRepositoryButton.Text = "Create repository";
            this.createRepositoryButton.UseVisualStyleBackColor = true;
            this.createRepositoryButton.Click += new System.EventHandler(this.CreateRepositoryButton_Click);
            // 
            // repositoryNameLabel
            // 
            this.repositoryNameLabel.AutoSize = true;
            this.repositoryNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.repositoryNameLabel.Location = new System.Drawing.Point(59, 22);
            this.repositoryNameLabel.Name = "repositoryNameLabel";
            this.repositoryNameLabel.Size = new System.Drawing.Size(45, 17);
            this.repositoryNameLabel.TabIndex = 33;
            this.repositoryNameLabel.Text = "Name";
            // 
            // repositoryNameTextBox
            // 
            this.repositoryNameTextBox.Location = new System.Drawing.Point(115, 19);
            this.repositoryNameTextBox.Name = "repositoryNameTextBox";
            this.repositoryNameTextBox.Size = new System.Drawing.Size(406, 22);
            this.repositoryNameTextBox.TabIndex = 1;
            this.repositoryNameTextBox.TextChanged += new System.EventHandler(this.RepositoryNameTextBox_TextChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(28, 147);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(76, 32);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // CreateRepositoryDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MigratorUI.Properties.Resources.fonmdappd;
            this.ClientSize = new System.Drawing.Size(553, 199);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.repositoryLocationTextBox);
            this.Controls.Add(this.locationLabel);
            this.Controls.Add(this.typeLabel);
            this.Controls.Add(this.repositoryTypeComboBox);
            this.Controls.Add(this.repositoryDescriptionLabel);
            this.Controls.Add(this.repositoryDescriptionTextbox);
            this.Controls.Add(this.createRepositoryButton);
            this.Controls.Add(this.repositoryNameLabel);
            this.Controls.Add(this.repositoryNameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NewRepositoryDialog";
            this.Text = "Create new TDS Repository";
            this.Load += new System.EventHandler(this.CreateRepositoryDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox repositoryLocationTextBox;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.ComboBox repositoryTypeComboBox;
        private System.Windows.Forms.Label repositoryDescriptionLabel;
        private System.Windows.Forms.TextBox repositoryDescriptionTextbox;
        private System.Windows.Forms.Button createRepositoryButton;
        private System.Windows.Forms.Label repositoryNameLabel;
        private System.Windows.Forms.TextBox repositoryNameTextBox;
        private System.Windows.Forms.Button cancelButton;
    }
}