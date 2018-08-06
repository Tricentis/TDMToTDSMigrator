﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using Newtonsoft.Json;

using TDMtoTDSMigrator;

using TestDataContract.Configuration;
using TestDataContract.TestData;

namespace MigratorUI {
    public partial class TdsMigrator : Form {
        public TdsMigrator() {
            InitializeComponent();
        }

        private Dictionary<string, List<TestDataObject>> testData;

        private TdmDataDocument tdmDataSheet;

        private string apiUrl;

        private Boolean migrationInWork;


        //Initialization 
        private void TdsMigrator_Load(object sender, EventArgs e) {
            logTextBox.AppendText(
                    "Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
            verifyUrlButton.Select();


        }

        //Migration and API related methods
        private async Task<HttpResponseMessage> LaunchMigration() {
            Dictionary<string, List<TestDataObject>> filteredTestData = new Dictionary<string, List<TestDataObject>>();

            if (ApplyFilter()) {
                foreach (string category in categoriesListBox.CheckedItems) {
                    filteredTestData.Add(RemoveCategorySizeFromString(category), testData[RemoveCategorySizeFromString(category)]);
                }
            } else {
                filteredTestData = testData;
            }
            PrintEstimatedWaitTimeMessage(filteredTestData);
            return await HttpRequest.Migrate(filteredTestData, repositoriesBox.SelectedItem.ToString(), apiUrl);
        }

        private void ClearRepository(string repositoryName) {
            DialogResult confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                         "Clear " + repositoriesBox.SelectedItem + " repository",
                                                         MessageBoxButtons.OKCancel,
                                                         MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.OK) {
                Boolean clearanceSuccessful = HttpRequest.ClearRepository(repositoryName).IsSuccessStatusCode;
                if (clearanceSuccessful) {
                    PrintClearedRepositoryMessage(repositoryName);
                } else {
                    logTextBox.AppendText("Could not clear " + repositoryName + "\n");
                }
            }
        }

        private void DeleteRepository(string repositoryName) {
            DialogResult confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                         "Clear " + repositoriesBox.SelectedItem + " repository",
                                                         MessageBoxButtons.OKCancel,
                                                         MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.OK) {
                Boolean deletionSuccessful = HttpRequest.ClearRepository(repositoryName).IsSuccessStatusCode
                                             && HttpRequest.DeleteRepository(repositoryName).IsSuccessStatusCode;

                if (deletionSuccessful) {
                    PrintDeletedRepositoryMessage(repositoryName);
                    repositoriesBox.Items.Remove(repositoryName);
                } else {
                    logTextBox.AppendText("Could not delete " + repositoryName + "\n");
                }

                if (repositoriesBox.Items.Count > 0) {
                    repositoriesBox.SelectedItem = repositoriesBox.Items[0];
                }
            }
        }

        private void CreateRepository(string repositoryName, string repositoryDescription) {
            Boolean creationSuccessful = HttpRequest.CreateRepository(repositoryName, repositoryDescription, apiUrl).IsSuccessStatusCode;
            if (creationSuccessful) {
                logTextBox.AppendText(PrintCreatedRepositoryMessage(repositoryName, repositoryDescriptionTextbox.Text));
                repositoriesBox.Items.Add(repositoryName);
                repositoriesBox.SelectedItem = repositoryName;
            } else {
                logTextBox.AppendText("Could not create repository " + repositoryName + "\n");
            }
        }

        private void CheckForAssociations() {
            RawDataObject obj = new RawDataObject();
            StringBuilder stringBuilder = new StringBuilder();
            if (tdmDataSheet.MetaInfoAssociations.Count != 0) {
                stringBuilder.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (MetaInfoAssociation metaInfoAssociation in tdmDataSheet.MetaInfoAssociations) {
                    obj.SetCategoryId(metaInfoAssociation.AssociatedCategoryId);
                    stringBuilder.Append(metaInfoAssociation.CategoryName + " and " + obj.FindCategoryName(tdmDataSheet) + "\n");
                }
                MessageBox.Show(stringBuilder.ToString(), "Associations not supported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void ProcessTddFile() {

            TddFileProcessingInWork();
            await Task.Delay(10);
            tdmDataSheet = new TdmDataDocument(XmlParser.DecompressTddFileIntoXml(new FileInfo(TDDPathTextBox.Text)));
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => { r.Result = XmlParser.CreateDataList(tdmDataSheet); };
            worker.RunWorkerCompleted += (s, r) => {
                                             testData = (Dictionary<string, List<TestDataObject>>)r.Result;
                                             TddFileProcessingFinished();

                                         };
            worker.RunWorkerAsync();
        }

        private int CountNumberOfObjects(Dictionary<string, List<TestDataObject>> data) {
            int totalNumberOfRows = 0;
            foreach (string category in data.Keys) {
                totalNumberOfRows += data[category].Count;
            }
            return totalNumberOfRows;
        }

        private int EstimatedWaitTime(Dictionary<string, List<TestDataObject>> data) {
            //in seconds, based on the number of objects
            return (int)(35 * CountNumberOfObjects(data) / (float)2713) + 1;
        }

        private string RemoveCategorySizeFromString(string category) {
            //ex : "People (5)" ---> "People"
            return category.Remove(category.LastIndexOf(" ", StringComparison.Ordinal));
        }

        private Boolean IsValidRepositoryName(string repositoryName) {
            if (repositoryName == "") {
                logTextBox.AppendText("Please enter a repository name");
                return false;
            }
            if (repositoriesBox.Items.Contains(repositoryNameTextBox.Text))
            {
                logTextBox.AppendText("Repository \"" + repositoryNameTextBox.Text + "\" already exists \n");
                return false;
            }
            char[] unallowedRepositoryCharacters = { '/', '|', '\\', '<', '>', '#', '*', '+', ':', ';', '"', '.', ',', '?' };
            foreach (char unallowedCharacter in unallowedRepositoryCharacters) {
                if (repositoryName.Contains(unallowedCharacter.ToString())) {
                    logTextBox.AppendText("A repository name cannot contain the following characters :\n");
                    foreach (char character in unallowedRepositoryCharacters) {
                           logTextBox.AppendText(character+ " ");
                    }
                    return false;
                }
            }
            return true;
        }

        private Boolean ApplyFilter() {
            return categoriesListBox.SelectedItems.Count < categoriesListBox.Items.Count;
        }

        //UI element attributes and logText methods 
        private void LoadCategoriesIntoListBox() {
            categoriesListBox.Items.Clear();
            foreach (string category in testData.Keys) {
                categoriesListBox.Items.Add(category + " (" + testData[category].Count + ")", true);
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
                    "\n\nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n\n");
            loadIntoRepositoryButton.Enabled = true;
            categoriesListBox.Enabled = true;
            selectAllButton.Enabled = true;
            deselectAllButton.Enabled = true;
            verifyUrlButton.Enabled = true;
            pickFileButton.Enabled = true;
            tddFileProcessingProgressBar.Visible = false;
        }

        private void MigrationInWork(Boolean inWork) {
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

        private void ApiConnectionOk(Boolean apiConnectionOk, object sender, EventArgs e) {
            Boolean tddFilePicked = TDDPathTextBox.Text != "";

            createRepositoryButton.Enabled = apiConnectionOk;
            deleteRepositoryButton.Enabled = apiConnectionOk;
            clearRepositoryButton.Enabled = apiConnectionOk;
            loadIntoRepositoryButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
            loadRefreshRepositories.Enabled = apiConnectionOk;
            repositoriesBox.Enabled = apiConnectionOk;
            repositoryDescriptionTextbox.Enabled = apiConnectionOk;
            repositoryNameTextBox.Enabled = apiConnectionOk;
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

        private void RefreshRepositoriesList() {
            repositoriesBox.Items.Clear();
            List<TestDataRepository> repositories = JsonConvert.DeserializeObject<List<TestDataRepository>>(HttpRequest.GetRepositories());

            foreach (TestDataRepository repository in repositories) {
                repositoriesBox.Items.Add(repository.Name);
            }
            if (repositoriesBox.Items.Count > 0) {
                repositoriesBox.SelectedItem = repositoriesBox.Items[0];
            }
        }

        //logText message generation methoids
        private string PrintCreatedRepositoryMessage(string name, string description) {
            StringBuilder s = new StringBuilder();
            s.Append("Repository Created : " + name);
            if (description != "") {
                s.Append(" , Description : " + description);
            }

            s.Append("\n");
            return s.ToString();
        }

        private void PrintDeletedRepositoryMessage(string name) {
            logTextBox.AppendText("Repository Deleted : " + name + "\n");
        }

        private void PrintClearedRepositoryMessage(string name) {
            logTextBox.AppendText("Repository Cleared : " + name + "\n");
        }

        private void PrintMigrationFinishedMessage(int numberOfCategories, string repositoryName) {
            logTextBox.AppendText("Successfully migrated " + numberOfCategories + " out of " + categoriesListBox.Items.Count + " available categories into the repository : \""
                                  + repositoryName + "\".\n");
        }

        private void PrintEstimatedWaitTimeMessage(Dictionary<string, List<TestDataObject>> data) {
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
                logTextBox.AppendText("Migrating " + categoriesListBox.CheckedItems.Count + " categories into \"" + repositoriesBox.SelectedItem + "\". Please wait...\n");
                MigrationInWork(true);

                await LaunchMigration();

                PrintMigrationFinishedMessage(categoriesListBox.CheckedItems.Count, repositoriesBox.SelectedItem.ToString());
                MigrationInWork(false);
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
            RefreshRepositoriesList();
            if (IsValidRepositoryName(repositoryNameTextBox.Text)) {
                CreateRepository(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text);
            } 
        }

        private void ClearRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem != null) {
                ClearRepository(repositoriesBox.SelectedItem.ToString());
            } else {
                logTextBox.AppendText("Please select a repository to clear \n");
            }
        }

        private void DeleteRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem != null) {
                DeleteRepository(repositoriesBox.SelectedItem.ToString());
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
                Boolean connectionSuccessfull = HttpRequest.SetConnection(apiUrlTextBox.Text);
                if (connectionSuccessfull) {
                    apiUrl = apiUrlTextBox.Text;
                    apiUrlTextBox.BackColor = Color.Lime;
                    ApiConnectionOk(true, sender, e);
                    verifyUrlButton.Text = "Change URL";
                    logTextBox.AppendText("Valid URL. \n");
                    if (TDDPathTextBox.Text == "") {
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
            loadIntoRepositoryButton.Text = "Load categories into repository : \" " + repositoriesBox.SelectedItem + " \"";
        }
    }
}