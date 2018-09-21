using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using TDMtoTDSMigrator;

using TestDataContract.Configuration;

namespace MigratorUI {
    public partial class NewRepositoryDialog : Form {
        private readonly TdsMigratorDialog migrator;

        private bool automaticLocationWritingEnabled;

        readonly char[] unallowedRepositoryCharacters = { '/', '|', '\\', '<', '>', '#', '*', '+', ':', ';', '"', '.', ',', '?' };

        public NewRepositoryDialog(TdsMigratorDialog migrator) {
            InitializeComponent();
            this.migrator = migrator;
            automaticLocationWritingEnabled = true;
        }

        private void CreateRepositoryDialog_Load(object sender, EventArgs e) {
            foreach (DataBaseType dataBaseType in Enum.GetValues(typeof(DataBaseType))) {
                repositoryTypeComboBox.Items.Add(dataBaseType);
            }
            try {
                repositoryTypeComboBox.SelectedItem = DataBaseType.Sqlite;
            }
            catch { 
                try {
                    repositoryTypeComboBox.SelectedItem = repositoryTypeComboBox.Items[0];
                }
                catch
                {
                    //Do nothing
                }
            }
        }

        public void CreateRepository(TestDataRepository newRepository) {
            bool creationSuccessful = HttpRequest.CreateRepository(newRepository).IsSuccessStatusCode;
            if (creationSuccessful) {
                PrintCreatedRepositoryMessage(newRepository);
                migrator.repositoriesBox.Items.Add(newRepository);
                migrator.repositoriesBox.SelectedItem = newRepository;
                Dispose();
            } else {
                migrator.logTextBox.AppendText("Could not create repository " + newRepository.Name + "\n");
            }
        }

        private bool IsValidRepository(TestDataRepository newRepository) {
            if (string.IsNullOrEmpty(newRepository.Name)) {
                migrator.logTextBox.AppendText("Please enter a repository name\n");
                repositoryNameTextBox.BackColor = Color.PaleVioletRed;
                return false;
            }
            if (string.IsNullOrEmpty(newRepository.Location)) {
                migrator.logTextBox.AppendText("Please enter a repository location\n");
                repositoryLocationTextBox.BackColor = Color.PaleVioletRed;
                return false;
            }
            foreach (TestDataRepository existingRepository in HttpRequest.GetRepositories()) {
                if (existingRepository.Name == newRepository.Name) {
                    migrator.logTextBox.AppendText("Repository \"" + newRepository.Name + "\" already exists \n");
                    repositoryNameTextBox.BackColor = Color.PaleVioletRed;
                    return false;
                }
                if (existingRepository.Location == newRepository.Location) {
                    migrator.logTextBox.AppendText("Repository \"" + existingRepository.Name + "\" already occupies the location " + newRepository.Location + "\n");
                    repositoryLocationTextBox.BackColor = Color.PaleVioletRed;
                    return false;
                }
            }
            foreach (char unallowedCharacter in unallowedRepositoryCharacters) {
                if (newRepository.Name.Contains(unallowedCharacter.ToString())) {
                    migrator.logTextBox.AppendText("A repository name cannot contain the following characters :\n"+new string(unallowedRepositoryCharacters)+"\n");
                    migrator.logTextBox.AppendText("\n");
                    repositoryNameTextBox.BackColor = Color.PaleVioletRed;
                    return false;
                }
            }
            return true;
        }

        private void PrintCreatedRepositoryMessage(TestDataRepository newRepository) {
            StringBuilder s = new StringBuilder();
            s.Append("Repository Created : " + newRepository.Name);
            if (newRepository.Description != "") {
                s.Append(" , Description : " + newRepository.Description + ".");
            }
            s.Append("\n");
            migrator.logTextBox.AppendText(s.ToString());
        }

        private void CreateRepositoryButton_Click(object sender, EventArgs e) {
            TestDataRepository newRepository = new TestDataRepository {
                    Description = repositoryDescriptionTextbox.Text.Trim(),
                    Name = repositoryNameTextBox.Text.Trim(),
                    Type = (DataBaseType)repositoryTypeComboBox.SelectedItem,
                    Location = repositoryLocationTextBox.Text.Trim(),
            };
            if (IsValidRepository(newRepository)) {
                CreateRepository(newRepository);
            }
        }

        private void RepositoryNameTextBox_TextChanged(object sender, EventArgs e) {
            if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite) {
                repositoryLocationTextBox.Text = @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text.Trim() + ".db";
            }else if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.InMemory) {
                repositoryLocationTextBox.Text = repositoryNameTextBox.Text.Trim();
            }
            repositoryNameTextBox.BackColor = Color.White;
        }

        private void RepositoryTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if ((DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.InMemory) {
                repositoryLocationTextBox.Text = repositoryNameTextBox.Text.Trim();
            } else if ((DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite) {
                repositoryLocationTextBox.Text = @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text.Trim() + ".db";
            }
            automaticLocationWritingEnabled = true;
        }

        private void RepositoryLocationTextBox_TextChanged(object sender, EventArgs e) {
            repositoryLocationTextBox.BackColor = Color.White;
            if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.Sqlite
                                                && repositoryLocationTextBox.Text != @"%PROGRAMDATA%\Tricentis\TestDataService\" + repositoryNameTextBox.Text.Trim() + ".db") {
                automaticLocationWritingEnabled = false;
            } else if (automaticLocationWritingEnabled && (DataBaseType)repositoryTypeComboBox.SelectedItem == DataBaseType.InMemory
                                                       && repositoryLocationTextBox.Text != repositoryNameTextBox.Text.Trim()) {
                automaticLocationWritingEnabled = false;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            Dispose();
        }
    }
}