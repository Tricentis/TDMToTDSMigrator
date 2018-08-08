﻿using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using TDMtoTDSMigrator;

using TestDataContract.Configuration;

namespace MigratorUI {
    public partial class CreateRepositoryDialog : Form {
        private readonly TdsMigrator migrator;

        private bool automaticLocationWriting ;
        public CreateRepositoryDialog(TdsMigrator migrator) {
            InitializeComponent();
            this.migrator = migrator;
            automaticLocationWriting = true;
        }

        
        private void CreateRepositoryDialog_Load(object sender, EventArgs e)
        {
            repositoryTypeComboBox.Items.Add(DataBaseType.Sqlite);
            repositoryTypeComboBox.Items.Add(DataBaseType.InMemory);
            repositoryTypeComboBox.SelectedItem = DataBaseType.Sqlite;
        }

        public void CreateRepository(TestDataRepository repository) {
            bool creationSuccessful = HttpRequest.CreateRepository(repository).IsSuccessStatusCode;
            if (creationSuccessful) {
                PrintCreatedRepositoryMessage(repository);
                migrator.repositoriesBox.Items.Add(repository);
                migrator.repositoriesBox.SelectedItem = repository;
                Dispose();
            } else {
                migrator.logTextBox.AppendText("Could not create repository " + repository.Name + "\n");
            }
        }

        private bool IsValidRepository(TestDataRepository repository) {
            if (string.IsNullOrEmpty(repository.Name)) {
                migrator.logTextBox.AppendText("Please enter a repository name\n");
                repositoryNameTextBox.BackColor = Color.PaleVioletRed;
                return false;
            }
            foreach (TestDataRepository existingRepository in HttpRequest.GetRepositories()) {
                if (existingRepository.Name == repository.Name) {
                    migrator.logTextBox.AppendText("Repository \"" + repository.Name + "\" already exists \n");
                    repositoryNameTextBox.BackColor = Color.PaleVioletRed;
                    return false;
                }
                if (existingRepository.Location == repository.Location) {
                    migrator.logTextBox.AppendText("Repository \"" + existingRepository.Name + "\" already occupies the location " + repository.Location + "\n");
                    repositoryLocationTextBox.BackColor = Color.PaleVioletRed;
                    return false;
                }
            }
            char[] unallowedRepositoryCharacters = { '/', '|', '\\', '<', '>', '#', '*', '+', ':', ';', '"', '.', ',', '?' };
            foreach (char unallowedCharacter in unallowedRepositoryCharacters) {
                if (repository.Name.Contains(unallowedCharacter.ToString())) {
                    migrator.logTextBox.AppendText("A repository name cannot contain the following characters :\n");
                    foreach (char character in unallowedRepositoryCharacters) {
                        migrator.logTextBox.AppendText(character + " ");
                    }
                    migrator.logTextBox.AppendText("\n");
                    return false;
                }
            }
            return true;
        }

        private void PrintCreatedRepositoryMessage(TestDataRepository repository) {
            StringBuilder s = new StringBuilder();
            s.Append("Repository Created : " + repository.Name);
            if (repository.Description != "") {
                s.Append(" , Description : " + repository.Description+".");
            }
            s.Append("\n");
            migrator.logTextBox.AppendText(s.ToString());
        }

        private void CreateRepositoryButton_Click(object sender, EventArgs e)
        {
            TestDataRepository repository = new TestDataRepository()
            {
                    Description = repositoryDescriptionTextbox.Text,
                    Name = repositoryNameTextBox.Text,
                    Type = (DataBaseType)repositoryTypeComboBox.SelectedItem,
                    Location = repositoryLocationTextBox.Text,
            };
            if (IsValidRepository(repository))
            {
                CreateRepository(repository);
            }
        }

        private void RepositoryNameTextBox_TextChanged(object sender, EventArgs e) {
            if (automaticLocationWriting) {
                repositoryLocationTextBox.Text = @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text + ".db";
            }
            repositoryNameTextBox.BackColor = Color.White;
        }

        private void RepositoryTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.InMemory) {
                repositoryLocationTextBox.Clear();
            }else if ((DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite) {
                repositoryLocationTextBox.Text = @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text + ".db";
            }
        }

        private void RepositoryTypeComboBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((DataBaseType)e.ListItem).ToString();
        }

        private void RepositoryLocationTextBox_TextChanged(object sender, EventArgs e)
        {
            repositoryLocationTextBox.BackColor = Color.White;
            if (automaticLocationWriting && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite && !repositoryLocationTextBox.Text.Contains(repositoryNameTextBox.Text+".db")) {
                automaticLocationWriting = false;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }


    }
}