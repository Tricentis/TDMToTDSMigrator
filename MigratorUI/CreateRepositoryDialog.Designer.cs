﻿namespace MigratorUI
{
    partial class CreateRepositoryDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateRepositoryDialog));
            this.repositoryLocationTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.repositoryTypeComboBox = new System.Windows.Forms.ComboBox();
            this.repositoryDescriptionText = new System.Windows.Forms.Label();
            this.repositoryDescriptionTextbox = new System.Windows.Forms.TextBox();
            this.createRepositoryButton = new System.Windows.Forms.Button();
            this.repositoryNameText = new System.Windows.Forms.Label();
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(42, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 17);
            this.label4.TabIndex = 40;
            this.label4.Text = "Location";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(64, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 17);
            this.label2.TabIndex = 39;
            this.label2.Text = "Type";
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
            this.repositoryTypeComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.RepositoryTypeComboBox_Format);
            // 
            // repositoryDescriptionText
            // 
            this.repositoryDescriptionText.AutoSize = true;
            this.repositoryDescriptionText.BackColor = System.Drawing.Color.Transparent;
            this.repositoryDescriptionText.Location = new System.Drawing.Point(25, 51);
            this.repositoryDescriptionText.Name = "repositoryDescriptionText";
            this.repositoryDescriptionText.Size = new System.Drawing.Size(79, 17);
            this.repositoryDescriptionText.TabIndex = 37;
            this.repositoryDescriptionText.Text = "Description";
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
            // repositoryNameText
            // 
            this.repositoryNameText.AutoSize = true;
            this.repositoryNameText.BackColor = System.Drawing.Color.Transparent;
            this.repositoryNameText.Location = new System.Drawing.Point(59, 22);
            this.repositoryNameText.Name = "repositoryNameText";
            this.repositoryNameText.Size = new System.Drawing.Size(45, 17);
            this.repositoryNameText.TabIndex = 33;
            this.repositoryNameText.Text = "Name";
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
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.repositoryTypeComboBox);
            this.Controls.Add(this.repositoryDescriptionText);
            this.Controls.Add(this.repositoryDescriptionTextbox);
            this.Controls.Add(this.createRepositoryButton);
            this.Controls.Add(this.repositoryNameText);
            this.Controls.Add(this.repositoryNameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CreateRepositoryDialog";
            this.Text = "Create new TDS Repository";
            this.Load += new System.EventHandler(this.CreateRepositoryDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox repositoryLocationTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox repositoryTypeComboBox;
        private System.Windows.Forms.Label repositoryDescriptionText;
        private System.Windows.Forms.TextBox repositoryDescriptionTextbox;
        private System.Windows.Forms.Button createRepositoryButton;
        private System.Windows.Forms.Label repositoryNameText;
        private System.Windows.Forms.TextBox repositoryNameTextBox;
        private System.Windows.Forms.Button cancelButton;
    }
}