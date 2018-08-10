using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using TDMtoTDSMigrator;

using TestDataContract.Configuration;
using TestDataContract.TestData;

namespace MigratorUI {
    public partial class TdsMigrator : Form {
        private Dictionary<string, TestDataCategory> testData;

        private TdmDataDocument tdmDataSheet;

        private bool migrationInWork;

        public string ApiUrl { get; set; }

        public TdsMigrator() {
            InitializeComponent();
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                ShowApp();
            }
            base.WndProc(ref m);
        }
        private void ShowApp()
        {
            if (WindowState == FormWindowState.Minimized)
            {
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
                }
                else {
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

        private async void ProcessTddFile() {
            TddFileProcessingLaunched();
            await Task.Delay(10);
            tdmDataSheet = new TdmDataDocument(tddPathTextBox.Text);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => { r.Result = tdmDataSheet.CreateDataList(); };
            worker.RunWorkerCompleted += (s, r) => {
                                             testData = (Dictionary<string, TestDataCategory>)r.Result;
                                             TddFileProcessingFinished();
                                         };
            worker.RunWorkerAsync();
        }

        private void CheckForAssociations() {
            StringBuilder stringBuilder = new StringBuilder();
            if (tdmDataSheet.MetaInfoAssociations.Count != 0) {
                stringBuilder.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (MetaInfoAssociation metaInfoAssociation in tdmDataSheet.MetaInfoAssociations) {
                    stringBuilder.Append(metaInfoAssociation.CategoryName + " and " + tdmDataSheet.FindCategoryName(metaInfoAssociation.PartnerId) + "\n");
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
                    repositoriesBox.Items.Remove(repository);
                    if (repositoriesBox.Items.Count > 0) {
                        repositoriesBox.SelectedItem = repositoriesBox.Items[0];
                    }
                } else {
                    logTextBox.AppendText("Could not delete " + repository.Name + "\n");
                }
            }
        }

        private async Task<HttpResponseMessage> LaunchMigration() {
            Dictionary<string, TestDataCategory> filteredTestData = FilterTestData();
            PrintEstimatedWaitTimeMessage(filteredTestData, (TestDataRepository)repositoriesBox.SelectedItem);
            //
            //
            //DELETE AFTER INMEMORY API BUGFIX
            //
            //
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type == DataBaseType.InMemory) {
                return await HttpRequest.MigrateInMemory(filteredTestData, (TestDataRepository)repositoriesBox.SelectedItem, ApiUrl);
            }
            return await HttpRequest.Migrate(filteredTestData, (TestDataRepository)repositoriesBox.SelectedItem, ApiUrl);
        }

        private Dictionary<string, TestDataCategory> FilterTestData() {
            Dictionary<string, TestDataCategory> filteredTestData = new Dictionary<string, TestDataCategory>();
            foreach (TestDataCategory category in categoriesListBox.CheckedItems)
            {
                filteredTestData.Add(category.Name, category);
            }
            return filteredTestData;
        }

        //UI element attributes methods
        private void ApiConnectionOk(bool apiConnectionOk) {
            bool tddFilePicked = tddPathTextBox.Text != "";

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
            loadIntoRepositoryButton.Enabled = false;
            categoriesListBox.Enabled = false;
            selectAllButton.Enabled = false;
            deselectAllButton.Enabled = false;
            reverseButton.Enabled = false;
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
            verifyUrlButton.Enabled = true;
            pickFileButton.Enabled = true;
        }

        private void LoadCategoriesIntoListBox() {
            categoriesListBox.Items.Clear();
            foreach (TestDataCategory category in testData.Values) {
                if (category.ElementCount != 0) {
                    categoriesListBox.Items.Add(category, true);
                }
            }
            PrintEmptyCategoriesWarningMessage();
        }

        public void RefreshRepositoriesList() {
            TestDataRepository previouslySelectedRepository = null;
            if (repositoriesBox.SelectedItem != null) {
                previouslySelectedRepository = (TestDataRepository)repositoriesBox.SelectedItem;
            }
            repositoriesBox.Items.Clear();
            List<TestDataRepository> repositories = HttpRequest.GetRepositories();
            foreach (TestDataRepository repository in repositories) {
                repositoriesBox.Items.Add(repository);
            }
            if (previouslySelectedRepository != null && repositoriesBox.Items.Contains(previouslySelectedRepository)) {
                repositoriesBox.SelectedItem = previouslySelectedRepository;
            } else if (repositoriesBox.Items.Count != 0) {
                repositoriesBox.SelectedItem = repositoriesBox.Items[0];
            }
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
            logTextBox.AppendText(tdmDataSheet.CountNumberOfObjects() + " records among " + tdmDataSheet.CountNumberOfCategories() + " categories were found.");
        }

        private void PrintEmptyCategoriesWarningMessage() {
            bool oneCategoryIsEmpty = false;
            StringBuilder emptyCategoriesStringBuilder = new StringBuilder();
            emptyCategoriesStringBuilder.Append("\nThe following categories were empty and have been removed from the list :\n");
            foreach (TestDataCategory category in testData.Values) {
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

        private void PrintMigrationLaunchedMessage(int numberOfCategories, TestDataRepository repository) {
            logTextBox.AppendText("Migrating " + numberOfCategories + " categories into \"" + repository.Name + "\". Please wait...\n");
            //
            //
            //DELETE AFTER INMEMORY API BUGFIX
            //
            //
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type == DataBaseType.InMemory) {
                logTextBox.AppendText("The process of migrating into an InMemory database is currently slow. \n");
            }
        }

        private void PrintEstimatedWaitTimeMessage(Dictionary<string, TestDataCategory> data, TestDataRepository repository) {
            if (HttpRequest.EstimatedMigrationWaitTime(data, repository) > 5) {
                logTextBox.AppendText("Estimated waiting time : " + HttpRequest.EstimatedMigrationWaitTime(data, repository) + " seconds\n");
            }
        }

        private void PrintMigrationFinishedMessage(int numberOfCategories, TestDataRepository repository) {
            logTextBox.AppendText("Successfully migrated " + numberOfCategories + " out of " + categoriesListBox.Items.Count + " available categories into the repository : \""
                                  + repository.Name + "\".\n");
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
            new CreateRepositoryDialog(this).ShowDialog();
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
            for (int i = 0; i < categoriesListBox.Items.Count; i++) {
                categoriesListBox.SetItemChecked(i, true);
            }
        }

        private void DeselectAllButton_Click(object sender, EventArgs e) {
            for (int i = 0; i < categoriesListBox.Items.Count; i++) {
                categoriesListBox.SetItemChecked(i, false);
            }
        }

        private void ReverseButton_Click(object sender, EventArgs e) {
            for (int i = 0; i < categoriesListBox.Items.Count; i++){
                categoriesListBox.SetItemChecked(i, !categoriesListBox.GetItemChecked(i));
            }
        }

        private async void LoadIntoRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem == null) {
                logTextBox.AppendText("Please pick a repository, or create one\n");
            } else if (categoriesListBox.CheckedItems.Count == 0) {
                logTextBox.AppendText("Please pick at least one category\n");
            } else {
                int numberOfCategoriesToMigrate = categoriesListBox.CheckedItems.Count;
                PrintMigrationLaunchedMessage(numberOfCategoriesToMigrate, (TestDataRepository)repositoriesBox.SelectedItem);
                MigrationInWork(true);
                await LaunchMigration();
                MigrationInWork(false);
                PrintMigrationFinishedMessage(numberOfCategoriesToMigrate, (TestDataRepository)repositoriesBox.SelectedItem);
            }
        }

        private void LogTextBox_TextChanged(object sender, EventArgs e) {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void CategoriesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            categoriesListBox.ClearSelected();
        }

        private void RepositoriesBox_Format(object sender, ListControlConvertEventArgs e) {
            if (!string.IsNullOrEmpty(((TestDataRepository)e.ListItem).Description)) {
                e.Value = ((TestDataRepository)e.ListItem).Name + " (" + ((TestDataRepository)e.ListItem).Description + ")";
            } else {
                e.Value = ((TestDataRepository)e.ListItem).Name;
            }
        }

        private void CategoriesListBox_Format(object sender, ListControlConvertEventArgs e) {
            e.Value = ((TestDataCategory)e.ListItem).Name + " (" + ((TestDataCategory)e.ListItem).ElementCount + ")";
        }
    }
}