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
        public TdsMigrator() {
            InitializeComponent();
        }


        private Dictionary<string, TestDataCategory> testData;

        private TdmDataDocument tdmDataSheet;

        private bool migrationInWork;

        public string ApiUrl { get; set; }

        //Initialization 
        private void TdsMigrator_Load(object sender, EventArgs e) {
            logTextBox.AppendText(
                    "Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
            verifyUrlButton.Select();
        }

        //Migration and API related methods
        private async Task<HttpResponseMessage> LaunchMigration() {
            Dictionary<string, TestDataCategory> filteredTestData = new Dictionary<string, TestDataCategory>();

            if (ApplyFilter()) {
                foreach (TestDataCategory category in categoriesListBox.CheckedItems) {
                    filteredTestData.Add(category.Name,category);
                }
            } else {
                filteredTestData = testData;
            }
            PrintEstimatedWaitTimeMessage(filteredTestData);
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type == DataBaseType.InMemory) {
                return await HttpRequest.MigrateInMemory(filteredTestData, (TestDataRepository)repositoriesBox.SelectedItem, ApiUrl);  
            }
            return await HttpRequest.Migrate(filteredTestData, (TestDataRepository)repositoriesBox.SelectedItem, ApiUrl);
        }

        private void ClearRepository(TestDataRepository repository) {
            DialogResult confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                         "Clear " + repositoriesBox.SelectedItem + " repository",
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
                                                         "Clear " + repositoriesBox.SelectedItem + " repository",
                                                         MessageBoxButtons.OKCancel,
                                                         MessageBoxIcon.Warning);
        
            if (confirmResult == DialogResult.OK) {
                bool deletionSuccessful = HttpRequest.ClearRepository(repository).IsSuccessStatusCode
                                             && HttpRequest.DeleteRepository(repository).IsSuccessStatusCode;

                if (deletionSuccessful) {
                    PrintDeletedRepositoryMessage(repository);
                    repositoriesBox.Items.Remove(repository);
                    if (repositoriesBox.Items.Count > 0)
                    {
                        repositoriesBox.SelectedItem = repositoriesBox.Items[0];
                    }
                } else {
                    logTextBox.AppendText("Could not delete " + repository.Name + "\n");
                }
            }
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

        private async void ProcessTddFile() {
            TddFileProcessingInWork();
            await Task.Delay(10);
            tdmDataSheet = new TdmDataDocument(TDDPathTextBox.Text);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => { r.Result = tdmDataSheet.CreateDataList(); };
            worker.RunWorkerCompleted += (s, r) => {
                                             testData = (Dictionary<string, TestDataCategory>)r.Result;
                                             TddFileProcessingFinished();
                                         };
            worker.RunWorkerAsync();
        }

        private int CountNumberOfObjects(Dictionary<string, TestDataCategory> data) {
            int numberOfObjects = 0;
            foreach (string category in data.Keys) {
                numberOfObjects += data[category].ElementCount;
            }
            return numberOfObjects;
        }

        private int EstimatedWaitTime(Dictionary<string, TestDataCategory> data) {
            //DELETE AFTER API INMEMORY BUGFIX
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type== DataBaseType.InMemory) {
                return (int)(230 * CountNumberOfObjects(data) / (float)618);
            }
            //in seconds, based on the number of objects
            return (int)(35 * CountNumberOfObjects(data) / (float)2713) + 1;
        }

        private bool ApplyFilter() {
            return categoriesListBox.SelectedItems.Count < categoriesListBox.Items.Count;
        }

        //UI element attributes and logText methods 
        private void LoadCategoriesIntoListBox() {
            categoriesListBox.Items.Clear();
            categoriesListBox.ValueMember = "Name";
            bool oneCategoryIsEmpty=false;
            StringBuilder emptyCategoriesStringBuilder = new StringBuilder();
            emptyCategoriesStringBuilder.Append("\nThe following categories were empty and have been removed from the list :\n");
            foreach (string category in testData.Keys) {
                if (testData[category].ElementCount!=0) {
                    categoriesListBox.Items.Add(testData[category],true);
                } else {
                    oneCategoryIsEmpty = true;
                    emptyCategoriesStringBuilder.Append(category + ", ");
                }
            }
            if (oneCategoryIsEmpty) {
                logTextBox.AppendText(emptyCategoriesStringBuilder.Remove(emptyCategoriesStringBuilder.ToString().LastIndexOf(", ", StringComparison.Ordinal),2).ToString());
            }
        }

        private void TddFileProcessingInWork() {
            logTextBox.AppendText("The .tdd file is being processed. Please wait...\n");
            loadIntoRepositoryButton.Enabled = false;
            categoriesListBox.Enabled = false;
            selectAllButton.Enabled = false;
            deselectAllButton.Enabled = false;
            verifyUrlButton.Enabled = false;
            pickFileButton.Enabled = false;
            tddFileProcessingProgressBar.Visible = true;
        }

        private void TddFileProcessingFinished() {
            logTextBox.AppendText(CountNumberOfObjects(testData) + " records among " + testData.Count + " categories were found.");
            LoadCategoriesIntoListBox();
            CheckForAssociations();
            logTextBox.AppendText(
                    "\n\nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n");
            loadIntoRepositoryButton.Enabled = true;
            categoriesListBox.Enabled = true;
            selectAllButton.Enabled = true;
            deselectAllButton.Enabled = true;
            verifyUrlButton.Enabled = true;
            pickFileButton.Enabled = true;
            tddFileProcessingProgressBar.Visible = false;
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
            loadRefreshRepositories.Enabled = !migrationInWork;
        }

        private void ApiConnectionOk(bool apiConnectionOk, object sender, EventArgs e) {
            bool tddFilePicked = TDDPathTextBox.Text != "";

            createRepositoryButton.Enabled = apiConnectionOk;
            deleteRepositoryButton.Enabled = apiConnectionOk;
            clearRepositoryButton.Enabled = apiConnectionOk;
            loadIntoRepositoryButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            loadRefreshRepositories.Enabled = apiConnectionOk;
            repositoriesBox.Enabled = apiConnectionOk;
            categoriesListBox.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            selectAllButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            deselectAllButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            apiUrlTextBox.Enabled = !apiConnectionOk & !migrationInWork;
            pickFileButton.Enabled = apiConnectionOk & !migrationInWork;

            if (apiConnectionOk) {
                pickFileButton.Text = "Browse...";
                LoadRefreshRepositories_Click(sender, e);
                pickFileButton.Select();
            } else {
                pickFileButton.Text = "...";
            }
        }

        public void RefreshRepositoriesList() {
            TestDataRepository previouslySelectedRepository = null;
            if (repositoriesBox.SelectedItem!=null) {
                previouslySelectedRepository = (TestDataRepository)repositoriesBox.SelectedItem;
            }
            repositoriesBox.Items.Clear();
            List<TestDataRepository> repositories = HttpRequest.GetRepositories();
            foreach (TestDataRepository repository in repositories) {
                repositoriesBox.Items.Add(repository);
            }
            if (previouslySelectedRepository!=null && repositoriesBox.Items.Contains(previouslySelectedRepository)) {
                repositoriesBox.SelectedItem = previouslySelectedRepository;
            } else if (repositoriesBox.Items.Count!=0) {
                repositoriesBox.SelectedItem = repositoriesBox.Items[0];
            }
        }

        //logText message generation methoids

        private void PrintDeletedRepositoryMessage(TestDataRepository repository) {
            logTextBox.AppendText("Repository Deleted : " + repository.Name + "\n");
        }

        private void PrintClearedRepositoryMessage(TestDataRepository repository) {
            logTextBox.AppendText("Repository Cleared : " + repository.Name + "\n");
        }
        private void PrintMigrationLaunchedMessage(int numberOfCategories, TestDataRepository repository)
        {
            logTextBox.AppendText("Migrating " + numberOfCategories + " categories into \"" + repository.Name + "\". Please wait...\n");

            //DELETE AFTER API INMEMORY BUGFIX
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type == DataBaseType.InMemory) {
                logTextBox.AppendText("The process of migrating into an InMemory database is currently slow. \n");
            }
        }
        private void PrintMigrationFinishedMessage(int numberOfCategories, TestDataRepository repository) {
            logTextBox.AppendText("Successfully migrated " + numberOfCategories + " out of " + categoriesListBox.Items.Count + " available categories into the repository : \""
                                  + repository.Name + "\".\n");
        }
        private void PrintEstimatedWaitTimeMessage(Dictionary<string, TestDataCategory> data) {
            if (EstimatedWaitTime(data) > 5) {
                logTextBox.AppendText("Estimated waiting time : " + EstimatedWaitTime(data) + " seconds\n");
            }
        }

        //Event methods
        private async void LoadIntoRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem == null) {
                logTextBox.AppendText("Please pick a repository, or create one\n");
            } else if (categoriesListBox.CheckedItems.Count == 0) {
                logTextBox.AppendText("Please pick at least one category\n");
            } else {
                int numberOfCategories = categoriesListBox.CheckedItems.Count;
                PrintMigrationLaunchedMessage(numberOfCategories, (TestDataRepository)repositoriesBox.SelectedItem);
                MigrationInWork(true);
                await LaunchMigration();
                MigrationInWork(false);
                PrintMigrationFinishedMessage(numberOfCategories, (TestDataRepository)repositoriesBox.SelectedItem);
            }
        }

        private void TddPathTextBox_TextChanged(object sender, EventArgs e) {
            ProcessTddFile();
        }

        private void PickFileButton_Click(object sender, EventArgs e) {
            openFileDialog.ShowDialog();
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e) {
            categoriesListBox.Items.Clear();
            TDDPathTextBox.Text = openFileDialog.FileName;
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

        private void LoadRefreshRepositories_Click(object sender, EventArgs e) {
            RefreshRepositoriesList();
        }

        private void VerifyUrlButton_Click_1(object sender, EventArgs e) {
            if (verifyUrlButton.Text == "Change URL") {
                verifyUrlButton.Text = "Verify URL";
                ApiConnectionOk(false, sender, e);
            } else {
                bool connectionSuccessfull = HttpRequest.SetConnection(apiUrlTextBox.Text);
                if (connectionSuccessfull) {
                    ApiUrl = apiUrlTextBox.Text;
                    apiUrlTextBox.BackColor = Color.Lime;
                    ApiConnectionOk(true, sender, e);
                    verifyUrlButton.Text = "Change URL";
                    logTextBox.AppendText("Valid URL. \n");
                    if (string.IsNullOrEmpty(TDDPathTextBox.Text)) {
                        logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
                    }
                                //for testing purposes, delete for release
            repositoriesBox.SelectedItem = "data";
                } else {
                    apiUrlTextBox.BackColor = Color.PaleVioletRed;
                    ApiConnectionOk(false, sender, e);
                    repositoriesBox.Items.Clear();
                    logTextBox.AppendText("Not a valid URL.\n");
                }
            }
        }

        private void LogTextBox_TextChanged(object sender, EventArgs e) {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void RepositoriesBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (repositoriesBox.SelectedItem!=null) {
                loadIntoRepositoryButton.Text = "Load categories into repository : \" " + ((TestDataRepository)repositoriesBox.SelectedItem).Name + " \"";
            }
        }

        private void CategoriesListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((TestDataCategory)e.ListItem).Name + " (" + ((TestDataCategory)e.ListItem).ElementCount+")";
        }

        private void CategoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            categoriesListBox.ClearSelected();
        }

        private void RepositoriesBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (!string.IsNullOrEmpty(((TestDataRepository)e.ListItem).Description)) {
                e.Value = ((TestDataRepository)e.ListItem).Name + " (" + ((TestDataRepository)e.ListItem).Description + ")";
            } else {
                e.Value = ((TestDataRepository)e.ListItem).Name;
            }
        }
    }
}