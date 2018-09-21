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

namespace MigratorUI
{
    public partial class TdsMigratorDialog : Form {

        private bool IsTdsConnected {
            get => isTdsConnected;
            set {
                isTdsConnected = value;
                UpdateUi();
            }
        }

        private bool isTdsConnected;

        private void UpdateUi() {
            apiUrlTextBox.Enabled = isUrlChangeable;
            urlButton.Text = isUrlChangeable ? "Verify URL" : "Change URL";

            createRepositoryButton.Enabled = IsTdsConnected;
            clearRepositoryButton.Enabled = IsTdsConnected;
            deleteRepositoryButton.Enabled = IsTdsConnected;
            loadRefreshRepositoriesButton.Enabled = IsTdsConnected;
            pickFileButton.Enabled = IsTdsConnected;
            pickFileButton.Text = IsTdsConnected ? "Browse..." : "...";
            repositoriesBox.Enabled = IsTdsConnected;
            apiUrlTextBox.BackColor = IsTdsConnected ? Color.Lime : Color.PaleVioletRed;

            categoriesListBox.Enabled = IsReadyForMigration;
            selectAllButton.Enabled = IsReadyForMigration;
            deselectAllButton.Enabled = IsReadyForMigration;
            selectRemainingCategoriesButton.Enabled = IsReadyForMigration && !allDataWasMigrated;
            reverseButton.Enabled = IsReadyForMigration;
            loadIntoRepositoryButton.Enabled = IsReadyForMigration && categoriesListBox.SelectedItems.Count > 0;
        }

        private bool IsValidTddSelected {
            get => isValidTddSelected;
            set {
                isValidTddSelected = value;
                UpdateUi();
            }
        }

        private bool IsProcessing;

        private bool isValidTddSelected;

        private bool isUrlChangeable = true;

        private bool IsReadyForMigration => IsTdsConnected && IsValidTddSelected;

        public string ApiUrl;

        private TdmDataDocument tdmData;

        private Dictionary<TestDataCategory, Boolean> categoryMigrated = new Dictionary<TestDataCategory, Boolean>();

        private bool allDataWasMigrated;

        public TdsMigratorDialog() {
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
        }

        //Migration and API related methods
        private void VerifyUrl() {
            IsTdsConnected = HttpRequest.SetConnection(apiUrlTextBox.Text);
            if (!IsTdsConnected) {
                repositoriesBox.Items.Clear();
                logTextBox.AppendText("Url is not valid\n");
                return;
            }
            RefreshRepositoriesList();
            logTextBox.AppendText("Valid URL. \n");
            if (!IsValidTddSelected) {
                logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
            }

            //switch (urlButton.Text) {
            //    case "Verify URL":
            //        bool connectionSuccessfull = HttpRequest.SetConnection(apiUrl);
            //        if (connectionSuccessfull) {
            //            urlButton.Text = "Change URL";
            //            logTextBox.AppendText("Valid URL. \n");
            //            apiUrlTextBox.BackColor = Color.Lime;
            //            this.ApiUrl = apiUrl;
            //            ApiConnectionOk(true);
            //            if (string.IsNullOrEmpty(tddPathTextBox.Text)) {
            //                logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
            //            }
            //        } else {
            //            repositoriesBox.Items.Clear();
            //            logTextBox.AppendText("Url is not valid\n");
            //            apiUrlTextBox.BackColor = Color.PaleVioletRed;
            //            ApiConnectionOk(false);
            //        }
            //        break;
            //    case "Change URL":
            //        urlButton.Text = "Verify URL";
            //        ApiConnectionOk(false);
            //        ActiveControl = apiUrlTextBox;
            //        break;
            //}
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
            switch (targetRepository.Type) {
                case DataBaseType.InMemory:
                    message = await HttpRequest.MigrateInMemory(filteredTestData, targetRepository, ApiUrl);
                    break;
                default:
                    message = await HttpRequest.Migrate(filteredTestData, targetRepository, ApiUrl);
                    break;
            }
            MigrationInWork(false);
            if (message.IsSuccessStatusCode) {
                PrintMigrationSuccessfullMessage(filteredTestData, targetRepository);
                SortCategoriesCheckBox(filteredTestData);
                if (!categoryMigrated.Values.Contains(false) && !allDataWasMigrated) {
                    allDataWasMigrated = true;
                    PrintAllDataWasMigratedMessage();
                    selectRemainingCategoriesButton.Enabled = false;
                }
            } else {
                logTextBox.AppendText("Migration failed. Reason: " + message.ReasonPhrase
                                                                   + "\nPlease make sure the repository was correctly created (no extra spaces or unallowed characters in name, correct location)\n");
            }
        }

        //UI element attributes methods
        //private void ApiConnectionOk(bool apiConnectionOk) {
        //    bool tddFilePicked = !string.IsNullOrEmpty(tddPathTextBox.Text);

        //    createRepositoryButton.Enabled = apiConnectionOk;
        //    deleteRepositoryButton.Enabled = apiConnectionOk;
        //    clearRepositoryButton.Enabled = apiConnectionOk;
        //    loadIntoRepositoryButton.Enabled = apiConnectionOk & tddFilePicked;
        //    loadRefreshRepositoriesButton.Enabled = apiConnectionOk;
        //    repositoriesBox.Enabled = apiConnectionOk;
        //    categoriesListBox.Enabled = apiConnectionOk & tddFilePicked;
        //    selectAllButton.Enabled = apiConnectionOk & tddFilePicked;
        //    deselectAllButton.Enabled = apiConnectionOk & tddFilePicked;
        //    reverseButton.Enabled = apiConnectionOk & tddFilePicked;
        //    selectRemainingCategoriesButton.Enabled = apiConnectionOk & tddFilePicked & !allDataWasMigrated;
        //    apiUrlTextBox.Enabled = !apiConnectionOk;
        //    pickFileButton.Enabled = apiConnectionOk;

        //    if (apiConnectionOk) {
        //        pickFileButton.Text = "Browse...";
        //        RefreshRepositoriesList();
        //        pickFileButton.Select();
        //    } else {
        //        pickFileButton.Text = "...";
        //    }
        //}

        private void TddFileProcessingLaunched() {
            PrintTddProcessingLaunchedMessage();
            allDataWasMigrated = false;
            loadIntoRepositoryButton.Enabled = false;
            categoriesListBox.Enabled = false;
            selectAllButton.Enabled = false;
            deselectAllButton.Enabled = false;
            reverseButton.Enabled = false;
            selectRemainingCategoriesButton.Enabled = false;
            urlButton.Enabled = false;
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
            urlButton.Enabled = true;
            pickFileButton.Enabled = true;
            categoryMigrated = new Dictionary<TestDataCategory, bool>();
            foreach (TestDataCategory category in tdmData.TestData.Values) {
                if (category.ElementCount != 0) {
                    categoryMigrated.Add(category, false);
                }
            }
        }

        private void LoadCategoriesIntoListBox() {
            categoriesListBox.Items.Clear();
            categoriesListBox.Sorted = true;
            foreach (TestDataCategory category in tdmData.TestData.Values) {
                if (category.ElementCount != 0) {
                    categoriesListBox.Items.Add(category, true);
                }
            }
            PrintEmptyCategoriesWarningMessage();
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
                categoriesListBox.SetItemChecked(i, !categoryMigrated[(TestDataCategory)categoriesListBox.Items[i]]);
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

        private void MigrationInWork(bool migrationInWork) {
            migrationProgressBar.Visible = migrationInWork;
            urlButton.Enabled = !migrationInWork;
            pickFileButton.Enabled = !migrationInWork;
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

        //logText message generation methods
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
            foreach(TestDataCategory category in tdmData.TestData.Values.Where(category => category.ElementCount == 0).ToList()){
                emptyCategoriesStringBuilder.Append(category.Name + ", ");
                oneCategoryIsEmpty = true;
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
            if (((TestDataRepository)repositoriesBox.SelectedItem).Type == DataBaseType.InMemory) {
                logTextBox.AppendText("The process of migrating into an InMemory database is currently slower. \n");
            }
        }

        private void PrintEstimatedWaitTimeMessage(Dictionary<string, TestDataCategory> filteredTestData, TestDataRepository repository) {
            if (HttpRequest.EstimatedMigrationWaitTime(filteredTestData, repository) > 5) {
                logTextBox.AppendText("Estimated waiting time : " + HttpRequest.EstimatedMigrationWaitTime(filteredTestData, repository) + " seconds\n");
            }
        }

        private void PrintMigrationSuccessfullMessage(Dictionary<string, TestDataCategory> filteredTestData, TestDataRepository repository) {
            logTextBox.AppendText("Successfully migrated " + filteredTestData.Keys.Count + " out of " + categoriesListBox.Items.Count
                                  + " available categories into \"" + repository.Name + "\".\n");
        }

        private void PrintAllDataWasMigratedMessage() {
            logTextBox.AppendText("\nAll TDM data has been migrated to Tricentis TDS.\nYou can now exit the application.\n\n");
        }

        //OnEvent methods
        private void VerifyUrlButton_Click(object sender, EventArgs e) {
            if (isUrlChangeable) {
                VerifyUrl();
            }
            isUrlChangeable = !isUrlChangeable || !IsTdsConnected;
            UpdateUi();
        }

        private void PickFileButton_Click(object sender, EventArgs e) {
            openFileDialog.ShowDialog();
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e) {
            tddPathTextBox.Text = (sender as OpenFileDialog)?.FileName;
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
            try {
                if (categoryMigrated[(TestDataCategory)e.ListItem]) {
                    e.Value += " ✓ ";
                }
            } catch (KeyNotFoundException) {
                e.Value += ((TestDataCategory)e.ListItem).Name + " (" + ((TestDataCategory)e.ListItem).ElementCount + ")";
                return;
            }
            e.Value += ((TestDataCategory)e.ListItem).Name + " (" + ((TestDataCategory)e.ListItem).ElementCount + ")";
        }
    }
}