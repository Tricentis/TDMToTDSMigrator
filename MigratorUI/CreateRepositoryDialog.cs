using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using TDMtoTDSMigrator;

using TestDataContract.Configuration;

namespace MigratorUI {
    public partial class CreateRepositoryDialog : Form {
        private readonly TdsMigrator migrator;

        private bool automaticLocationWritingEnabled;

        readonly char[] unallowedRepositoryCharacters = { '/', '|', '\\', '<', '>', '#', '*', '+', ':', ';', '"', '.', ',', '?' };

        public CreateRepositoryDialog(TdsMigrator migrator) {
            InitializeComponent();
            this.migrator = migrator;
            automaticLocationWritingEnabled = true;
        }

        private void CreateRepositoryDialog_Load(object sender, EventArgs e) {
            foreach (DataBaseType dataBaseType in Enum.GetValues(typeof(DataBaseType))) {
                repositoryTypeComboBox.Items.Add(dataBaseType);
                repositoryTypeComboBox.SelectedItem = dataBaseType;
            }
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
            if (string.IsNullOrEmpty(repository.Location)) {
                migrator.logTextBox.AppendText("Please enter a repository location\n");
                repositoryLocationTextBox.BackColor = Color.PaleVioletRed;
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
            foreach (char unallowedCharacter in unallowedRepositoryCharacters) {
                if (repository.Name.Contains(unallowedCharacter.ToString())) {
                    migrator.logTextBox.AppendText("A repository name cannot contain the following characters :\n");
                    foreach (char character in unallowedRepositoryCharacters) {
                        migrator.logTextBox.AppendText(character + " ");
                    }
                    migrator.logTextBox.AppendText("\n");
                    repositoryNameTextBox.BackColor = Color.PaleVioletRed;
                    return false;
                }
            }
            return true;
        }

        private void PrintCreatedRepositoryMessage(TestDataRepository repository) {
            StringBuilder s = new StringBuilder();
            s.Append("Repository Created : " + repository.Name);
            if (repository.Description != "") {
                s.Append(" , Description : " + repository.Description + ".");
            }
            s.Append("\n");
            migrator.logTextBox.AppendText(s.ToString());
        }

        private void CreateRepositoryButton_Click(object sender, EventArgs e) {
            TestDataRepository repository = new TestDataRepository {
                    Description = repositoryDescriptionTextbox.Text,
                    Name = repositoryNameTextBox.Text,
                    Type = (DataBaseType)repositoryTypeComboBox.SelectedItem,
                    Location = repositoryLocationTextBox.Text,
            };
            if (IsValidRepository(repository)) {
                CreateRepository(repository);
            }
        }

        private void RepositoryNameTextBox_TextChanged(object sender, EventArgs e) {
            if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite) {
                repositoryLocationTextBox.Text = @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text + ".db";
            }else if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.InMemory) {
                repositoryLocationTextBox.Text = repositoryNameTextBox.Text;
            }
            repositoryNameTextBox.BackColor = Color.White;
        }

        private void RepositoryTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if ((DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.InMemory) {
                repositoryLocationTextBox.Text = repositoryNameTextBox.Text;
            } else if ((DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite) {
                repositoryLocationTextBox.Text = @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text + ".db";
            }
            automaticLocationWritingEnabled = true;
        }

        private void RepositoryLocationTextBox_TextChanged(object sender, EventArgs e) {
            repositoryLocationTextBox.BackColor = Color.White;
            if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite
                                         && !repositoryLocationTextBox.Text.Contains(repositoryNameTextBox.Text + ".db")) {
                automaticLocationWritingEnabled = false;
            }
            else if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.InMemory
                                         && repositoryLocationTextBox.Text != repositoryNameTextBox.Text)
            {
                automaticLocationWritingEnabled = false;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            Dispose();
        }
    }
}