using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

using TDMtoTDSMigrator;

using TestDataContract.Configuration;
using TestDataContract.TestData;

namespace MigratorUI {
    public partial class TdsMigrator : Form {
        private TdmDataDocument tdmData;

        private Dictionary<TestDataCategory, Boolean> categoryMigrated = new Dictionary<TestDataCategory, Boolean>();

        private bool allDataWasMigrated;

        private bool migrationInWork;

        public string ApiUrl;

        public TdsMigrator() {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == NativeMethods.WM_SHOWME) {
                ShowApp();
            }
            base.WndProc(ref m);
        }

        private void ShowApp() {
            if (WindowState == FormWindowState.Minimized) {
                WindowState = FormWindowState.Normal;
            }
        }

        //Initialization 
        private void TdsMigrator_Load(object sender, EventArgs e) {
            PrintWelcomeMessage();
            verifyUrlButton.Select();
        }

        //Migration and API related methods
        private void VerifyUrl(string apiUrl) {
            if (verifyUrlButton.Text == "Verify URL") {
                bool connectionSuccessfull = HttpRequest.SetConnection(apiUrl);
                if (connectionSuccessfull) {
                    this.ApiUrl = apiUrl;
                    apiUrlTextBox.BackColor = Color.Lime;
                    ApiConnectionOk(true);
                    verifyUrlButton.Text = "Change URL";
                    logTextBox.AppendText("Valid URL. \n");
                    if (string.IsNullOrEmpty(tddPathTextBox.Text)) {
                        logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
                    }
                } else {
                    apiUrlTextBox.BackColor = Color.PaleVioletRed;
                    ApiConnectionOk(false);
                    repositoriesBox.Items.Clear();
                    logTextBox.AppendText("Not a valid URL.\n");
                }
            } else {
                verifyUrlButton.Text = "Verify URL";
                ApiConnectionOk(false);
            }
        }

        private void ProcessTddFile() {
            TddFileProcessingLaunched();
            tdmData = new TdmDataDocument(tddPathTextBox.Text);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => { tdmData.CreateDataDictionary(); };
            worker.RunWorkerCompleted += (s, r) => { TddFileProcessingFinished(); };
            worker.RunWorkerAsync();
        }

        private void CheckForAssociations() {
            if (tdmData.MetaInfoAssociations.Count != 0) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (MetaInfoAssociation metaInfoAssociation in tdmData.MetaInfoAssociations) {
                    stringBuilder.Append(metaInfoAssociation.CategoryName + " and " + tdmData.FindCategoryName(metaInfoAssociation.PartnerId) + "\n");
                }
                MessageBox.Show(stringBuilder.ToString(), "Associations not supported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearRepository(TestDataRepository repository) {
            DialogResult confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                         "Clear " + repository.Name + " repository",
                                                         MessageBoxButtons.OKCancel,
                                                         MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.OK) {
                bool clearanceSuccessful = HttpRequest.ClearRepository(repository).IsSuccessStatusCode;
                if (clearanceSuccessful) {
                    PrintClearedRepositoryMessage(repository);
                } else {
                    logTextBox.AppendText("Could not clear " + repository.Name + "\n");
                }
            }
        }

        private void DeleteRepository(TestDataRepository repository) {
            DialogResult confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                         "Clear " + repository.Name + " repository",
                                                         MessageBoxButtons.OKCancel,
                                                         MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.OK) {
                bool deletionSuccessful = HttpRequest.ClearRepository(repository).IsSuccessStatusCode && HttpRequest.DeleteRepository(repository).IsSuccessStatusCode;

                if (deletionSuccessful) {
                    PrintDeletedRepositoryMessage(repository);
                    RefreshRepositoriesList();
                } else {
                    logTextBox.AppendText("Could not delete " + repository.Name + "\n");
                }
            }
        }

        private async void LaunchMigration() {
            Dictionary<string, TestDataCategory> filteredTestData = FilterTestData();
            TestDataRepository targetRepository = (TestDataRepository)repositoriesBox.SelectedItem;
            PrintMigrationLaunchedMessage(filteredTestData, targetRepository);
            PrintEstimatedWaitTimeMessage(filteredTestData, targetRepository);
            MigrationInWork(true);
            HttpResponseMessage message;
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type == DataBaseType.InMemory) {
                //DELETE AFTER INMEMORY API BUGFIX
                message = await HttpRequest.MigrateInMemory(filteredTestData, targetRepository, ApiUrl);
            } else {
                message = await HttpRequest.Migrate(filteredTestData, targetRepository, ApiUrl);
            }
            MigrationInWork(false);
            if (message.IsSuccessStatusCode) {
                PrintMigrationSuccessfullMessage(filteredTestData, targetRepository);
                SortCategoriesCheckBox(filteredTestData);
                if (categoryMigrated.Values.Contains(false) || allDataWasMigrated) {
                    return;
                }
                allDataWasMigrated = true;
                PrintAllDataWasMigratedMessage();
                selectRemainingCategoriesButton.Enabled = false;
                categoriesListBox.Sorted = true;
            } else {
                logTextBox.AppendText("Migration failed. Reason: " + message.ReasonPhrase
                                                                   + "\nPlease make sure the repository was correctly created (no extra spaces or unallowed characters in name, correct location)\n");
            }
        }

        //UI element attributes methods
        private void ApiConnectionOk(bool apiConnectionOk) {
            bool tddFilePicked = string.IsNullOrEmpty(tddPathTextBox.Text);

            createRepositoryButton.Enabled = apiConnectionOk;
            deleteRepositoryButton.Enabled = apiConnectionOk;
            clearRepositoryButton.Enabled = apiConnectionOk;
            loadIntoRepositoryButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            loadRefreshRepositoriesButton.Enabled = apiConnectionOk;
            repositoriesBox.Enabled = apiConnectionOk;
            categoriesListBox.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            selectAllButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            deselectAllButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            reverseButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            selectRemainingCategoriesButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork & !allDataWasMigrated;
            apiUrlTextBox.Enabled = !apiConnectionOk & !migrationInWork;
            pickFileButton.Enabled = apiConnectionOk & !migrationInWork;

            if (apiConnectionOk) {
                pickFileButton.Text = "Browse...";
                RefreshRepositoriesList();
                pickFileButton.Select();
            } else {
                pickFileButton.Text = "...";
            }
        }

        private void TddFileProcessingLaunched() {
            PrintTddProcessingLaunchedMessage();
            allDataWasMigrated = false;
            loadIntoRepositoryButton.Enabled = false;
            categoriesListBox.Enabled = false;
            selectAllButton.Enabled = false;
            deselectAllButton.Enabled = false;
            reverseButton.Enabled = false;
            selectRemainingCategoriesButton.Enabled = false;
            verifyUrlButton.Enabled = false;
            pickFileButton.Enabled = false;
            tddFileProcessingProgressBar.Visible = true;
        }

        private void TddFileProcessingFinished() {
            PrintNumberOfRecordsAndCategoriesFoundMessage();
            LoadCategoriesIntoListBox();
            tddFileProcessingProgressBar.Visible = false;
            CheckForAssociations();
            PrintTddProcessingFinishedMessage();
            loadIntoRepositoryButton.Enabled = true;
            categoriesListBox.Enabled = true;
            selectAllButton.Enabled = true;
            deselectAllButton.Enabled = true;
            reverseButton.Enabled = true;
            selectRemainingCategoriesButton.Enabled = true;
            verifyUrlButton.Enabled = true;
            pickFileButton.Enabled = true;
        }

        private void LoadCategoriesIntoListBox() {
            categoriesListBox.Items.Clear();
            categoryMigrated = new Dictionary<TestDataCategory, bool>();
            categoriesListBox.Sorted = true;
            foreach (TestDataCategory category in tdmData.TestData.Values) {
                if (category.ElementCount != 0) {
                    categoryMigrated.Add(category, false);
                    categoriesListBox.Items.Add(category, true);
                }
            }
            PrintEmptyCategoriesWarningMessage();
            categoriesListBox.Sorted = false;
        }

        private void SelectAllCategories() {
            for (int i = 0; i < categoriesListBox.Items.Count; i++) {
                categoriesListBox.SetItemChecked(i, true);
            }
        }

        private void DeselectAllCategories() {
            for (int i = 0; i < categoriesListBox.Items.Count; i++) {
                categoriesListBox.SetItemChecked(i, false);
            }
        }

        private void ReverseSelectedCategories() {
            for (int i = 0; i < categoriesListBox.Items.Count; i++) {
                categoriesListBox.SetItemChecked(i, !categoriesListBox.GetItemChecked(i));
            }
        }

        private void SelectRemainingCategories() {
            for (int i = 0; i < categoriesListBox.Items.Count; i++) {
                if (!categoriesListBox.GetItemChecked(i)) {
                    categoriesListBox.SetItemChecked(i, !categoryMigrated[(TestDataCategory)categoriesListBox.Items[i]]);
                }
            }
        }

        public void RefreshRepositoriesList() {
            int previouslySelectedIndex = 0;
            if (repositoriesBox.SelectedItem != null) {
                previouslySelectedIndex = repositoriesBox.SelectedIndex;
            }
            repositoriesBox.Items.Clear();
            foreach (TestDataRepository repository in HttpRequest.GetRepositories()) {
                repositoriesBox.Items.Add(repository);
            }
            repositoriesBox.SelectedItem = previouslySelectedIndex > repositoriesBox.Items.Count - 1
                                                   ? repositoriesBox.Items[repositoriesBox.Items.Count - 1]
                                                   : repositoriesBox.Items[previouslySelectedIndex];
        }

        private void MigrationInWork(bool inWork) {
            migrationInWork = inWork;

            verifyUrlButton.Enabled = !migrationInWork;
            pickFileButton.Enabled = !migrationInWork;
            migrationProgressBar.Visible = migrationInWork;
            deleteRepositoryButton.Enabled = !migrationInWork;
            clearRepositoryButton.Enabled = !migrationInWork;
            createRepositoryButton.Enabled = !migrationInWork;
            loadIntoRepositoryButton.Enabled = !migrationInWork;
            loadRefreshRepositoriesButton.Enabled = !migrationInWork;
        }

        private Dictionary<string, TestDataCategory> FilterTestData() {
            Dictionary<string, TestDataCategory> filteredTestData = new Dictionary<string, TestDataCategory>();
            foreach (TestDataCategory category in categoriesListBox.CheckedItems.OfType<TestDataCategory>().ToList()) {
                filteredTestData.Add(category.Name, category);
            }
            return filteredTestData;
        }

        private void SortCategoriesCheckBox(Dictionary<string, TestDataCategory> filteredTestData) {
            foreach (TestDataCategory category in filteredTestData.Values) {
                categoryMigrated[category] = true;
                SendCategoryToEndOfCheckBox(category);
            }
        }

        private void SendCategoryToEndOfCheckBox(TestDataCategory category) {
            categoriesListBox.Items.Remove(category);
            categoriesListBox.Items.Add(category, false);
        }

        //logText message generation methoids
        private void PrintWelcomeMessage() {
            logTextBox.AppendText(
                    "Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
        }

        private void PrintTddProcessingLaunchedMessage() {
            logTextBox.AppendText("The .tdd file is being processed. Please wait...\n");
        }

        private void PrintTddProcessingFinishedMessage() {
            logTextBox.AppendText(
                    "\n\nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n");
        }

        private void PrintNumberOfRecordsAndCategoriesFoundMessage() {
            logTextBox.AppendText(tdmData.CountNumberOfObjects() + " records among " + tdmData.CountNumberOfCategories() + " categories were found.");
        }

        private void PrintEmptyCategoriesWarningMessage() {
            bool oneCategoryIsEmpty = false;
            StringBuilder emptyCategoriesStringBuilder = new StringBuilder();
            emptyCategoriesStringBuilder.Append("\nThe following categories were empty and have been removed from the list :\n");
            foreach (TestDataCategory category in tdmData.TestData.Values) {
                if (category.ElementCount == 0) {
                    oneCategoryIsEmpty = true;
                    emptyCategoriesStringBuilder.Append(category.Name + ", ");
                }
            }
            if (oneCategoryIsEmpty) {
                logTextBox.AppendText(emptyCategoriesStringBuilder.Remove(emptyCategoriesStringBuilder.ToString().LastIndexOf(", ", StringComparison.Ordinal), 2).ToString());
            }
        }

        private void PrintDeletedRepositoryMessage(TestDataRepository repository) {
            logTextBox.AppendText("Repository Deleted : " + repository.Name + "\n");
        }

        private void PrintClearedRepositoryMessage(TestDataRepository repository) {
            logTextBox.AppendText("Repository Cleared : " + repository.Name + "\n");
        }

        private void PrintMigrationLaunchedMessage(Dictionary<string, TestDataCategory> filteredTestData, TestDataRepository repository) {
            logTextBox.AppendText("Migrating " + filteredTestData.Keys.Count + " categories into \"" + repository.Name + "\". Please wait...\n");
            //
            //
            //DELETE AFTER INMEMORY API BUGFIX
            //
            //
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type == DataBaseType.InMemory) {
                logTextBox.AppendText("The process of migrating into an InMemory database is currently slow. \n");
            }
        }

        private void PrintEstimatedWaitTimeMessage(Dictionary<string, TestDataCategory> filteredTestData, TestDataRepository repository) {
            if (HttpRequest.EstimatedMigrationWaitTime(filteredTestData, repository) > 5) {
                logTextBox.AppendText("Estimated waiting time : " + HttpRequest.EstimatedMigrationWaitTime(filteredTestData, repository) + " seconds\n");
            }
        }

        private void PrintMigrationSuccessfullMessage(Dictionary<string, TestDataCategory> filteredTestData, TestDataRepository repository) {
            logTextBox.AppendText("Successfully migrated " + filteredTestData.Keys.Count + " out of " + categoriesListBox.Items.Count
                                  + " available categories into the repository : \"" + repository.Name + "\".\n");
        }

        private void PrintAllDataWasMigratedMessage() {
            logTextBox.AppendText("\nAll your TDM data has been migrated to Tricentis TDS.\nYou can now exit the application.\n\n");
        }

        //OnEvent methods
        private void VerifyUrlButton_Click(object sender, EventArgs e) {
            VerifyUrl(apiUrlTextBox.Text);
        }

        private void PickFileButton_Click(object sender, EventArgs e) {
            openFileDialog.ShowDialog();
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e) {
            tddPathTextBox.Text = openFileDialog.FileName;
        }

        private void TddPathTextBox_TextChanged(object sender, EventArgs e) {
            categoriesListBox.Items.Clear();
            ProcessTddFile();
        }

        private void CreateRepositoryButton_Click(object sender, EventArgs e) {
            new NewRepositoryDialog(this).ShowDialog();
        }

        private void ClearRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem != null) {
                ClearRepository((TestDataRepository)repositoriesBox.SelectedItem);
            } else {
                logTextBox.AppendText("Please select a repository to clear \n");
            }
        }

        private void DeleteRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem != null) {
                DeleteRepository((TestDataRepository)repositoriesBox.SelectedItem);
            } else {
                logTextBox.AppendText("Please select a repository to delete \n");
            }
        }

        private void RepositoriesBox_SelectedValueChanged(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem != null) {
                loadIntoRepositoryButton.Text = "Load categories into repository : \" " + ((TestDataRepository)repositoriesBox.SelectedItem).Name + " \"";
            }
        }

        private void LoadRefreshRepositoriesButton_Click(object sender, EventArgs e) {
            RefreshRepositoriesList();
        }

        private void SelectAllButton_Click(object sender, EventArgs e) {
            SelectAllCategories();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e) {
            DeselectAllCategories();
        }

        private void ReverseSelectedCategoriesButton_Click(object sender, EventArgs e) {
            ReverseSelectedCategories();
        }

        private void SelectRemainingCategoriesButton_Click(object sender, EventArgs e) {
            SelectRemainingCategories();
        }

        private void LoadIntoRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem == null) {
                logTextBox.AppendText("Please pick a repository, or create one\n");
            } else if (categoriesListBox.CheckedItems.Count == 0) {
                logTextBox.AppendText("Please pick at least one category\n");
            } else {
                LaunchMigration();
            }
        }

        private void LogTextBox_TextChanged(object sender, EventArgs e) {
            logTextBox.ScrollToCaret();
        }

        private void CategoriesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            categoriesListBox.ClearSelected();
        }

        private void RepositoriesBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((TestDataRepository)e.ListItem).Name;
            if (!string.IsNullOrEmpty(((TestDataRepository)e.ListItem).Description)) {
                e.Value += " (" + ((TestDataRepository)e.ListItem).Description + ")";
            }
        }

        private void CategoriesListBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = "";
            if (categoryMigrated[(TestDataCategory)e.ListItem]) {
                e.Value += "✓ ";
            }
            e.Value += ((TestDataCategory)e.ListItem).Name + " (" + ((TestDataCategory)e.ListItem).ElementCount + ")";
        }
    }
}