using System;
using System.Text;
using System.Windows.Forms;

using TDMtoTDSMigrator;

using TestDataContract.Configuration;

namespace MigratorUI {
    public partial class CreateRepositoryDialog : Form {
        private readonly TdsMigrator migrator;

        public CreateRepositoryDialog(TdsMigrator migrator) {
            InitializeComponent();
            this.migrator = migrator;
            repositoryTypeComboBox.SelectedItem = "SQLite";
        }

        private void CreateRepositoryButton_Click(object sender, EventArgs e) {
            DataBaseType type;
            if (repositoryTypeComboBox.Text == "InMemory") {
                type = DataBaseType.InMemory;
            } else {
                type = DataBaseType.Sqlite;
            }
            TestDataRepository repository = new TestDataRepository() {
                    Description = repositoryDescriptionTextbox.Text,
                    Name = repositoryNameTextBox.Text,
                    Type = type,
                    Location = repositoryLocationTextBox.Text,
            };
            if (IsValidRepository(repository)) {
                CreateRepository(repository);
            }
        }

        public void CreateRepository(TestDataRepository repository) {
            Boolean creationSuccessful = HttpRequest.CreateRepository(repository).IsSuccessStatusCode;
            if (creationSuccessful) {
                PrintCreatedRepositoryMessage(repository);
                migrator.repositoriesBox.Items.Add(repository);
                migrator.repositoriesBox.SelectedItem = repository;
                Dispose();
            } else {
                migrator.logTextBox.AppendText("Could not create repository " + repository.Name + "\n");
            }
        }

        private Boolean IsValidRepository(TestDataRepository repository) {
            if (repository.Name == "") {
                migrator.logTextBox.AppendText("Please enter a repository name\n");
                return false;
            }
            foreach (TestDataRepository existingRepository in HttpRequest.GetRepositories()) {
                if (existingRepository.Name == repository.Name) {
                    migrator.logTextBox.AppendText("Repository \"" + repository.Name + "\" already exists \n");
                    return false;
                }
                if (existingRepository.Location == repository.Location) {
                    migrator.logTextBox.AppendText("Repository \"" + existingRepository.Name + "\" already occupies the location " + repository.Location + "\n");
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
                s.Append(" , Description : " + repository.Description);
            }
            s.Append("\n");
            migrator.logTextBox.AppendText(s.ToString());
        }

        private void RepositoryNameTextBox_TextChanged(object sender, EventArgs e) {
            repositoryLocationTextBox.Text = @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text + ".db";
        }
    }
}