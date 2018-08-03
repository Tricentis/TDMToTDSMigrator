using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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

        private Dictionary<string, List<TestDataObject>> testData = new Dictionary<string, List<TestDataObject>>();

        private string xmlPath;

        private Boolean migrationInWork = false;


        //Initialization 
        private void TdsMigrator_Load(object sender, EventArgs e) {
            logTextBox.AppendText(
                    "Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
            verifyUrlButton.Select();
        }


        //Migration and API related methods
        private async Task<HttpResponseMessage> LaunchMigration() {
            if (categoriesListBox.SelectedItems.Count < categoriesListBox.Items.Count) {
                Dictionary<string, List<TestDataObject>> filteredTestData = new Dictionary<string, List<TestDataObject>>();
                foreach (string category in categoriesListBox.CheckedItems) {
                    string categoryNameWithoutNumberOfInstances = Regex.Replace(category, "  [(]\\d+[)]", "");
                    filteredTestData.Add(categoryNameWithoutNumberOfInstances, testData[categoryNameWithoutNumberOfInstances]);
                }
                EstimatedWaitTimeMessage(filteredTestData);
                HttpResponseMessage message =
                        await TdsLoader.MigrateXmlDataIntoTds(filteredTestData, repositoriesBox.SelectedItem.ToString(), apiUrlTextBox.Text);
                return message;
            } else {
                EstimatedWaitTimeMessage(testData);
                HttpResponseMessage message = await TdsLoader.MigrateXmlDataIntoTds(testData, repositoriesBox.SelectedItem.ToString(), apiUrlTextBox.Text);
                return message;
            }
        }

        private void ClearRepository(string repositoryName)
        {
            var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                "Clear " + repositoriesBox.SelectedItem + " repository",
                                                MessageBoxButtons.OKCancel,
                                                MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.OK)
            {
                Boolean clearanceSuccessful = HttpRequest.ClearRepository(repositoryName).IsSuccessStatusCode;
                if (clearanceSuccessful)
                {
                    logTextBox.AppendText(ClearedRepositoryMessage(repositoryName));
                }
                else
                {
                    logTextBox.AppendText("Could not clear " + repositoryName + "\n");
                }
            }
        }

        private void DeleteRepository(string repositoryName)
        {
            var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                "Clear " + repositoriesBox.SelectedItem + " repository",
                                                MessageBoxButtons.OKCancel,
                                                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.OK)
            {
                Boolean deletionSuccessful = HttpRequest.ClearRepository(repositoryName).IsSuccessStatusCode
                                             && HttpRequest.DeleteRepository(repositoryName).IsSuccessStatusCode;

                if (deletionSuccessful)
                {
                    logTextBox.AppendText(DeletedRepositoryMessage(repositoryName));
                    repositoriesBox.Items.Remove(repositoryName);
                }
                else
                {
                    logTextBox.AppendText("Could not delete " + repositoryName +"\n");
                }

                if (repositoriesBox.Items.Count > 0)
                {
                    repositoriesBox.SelectedItem = repositoriesBox.Items[0];
                }
            }
        }

        private void CreateRepository(string repositoryName, string repositoryDescription) {
            



            RefreshRepositoriesList();
            TestDataRepository repository = new TestDataRepository()
            {
                    Description = repositoryDescription,
                    Name = repositoryName,
                    Type = DataBaseType.Sqlite,
                    Location = "%PROGRAMDATA%\\Tricentis\\TestDataService\\" + repositoryName + ".db",
                    Link = apiUrlTextBox.Text+""+HttpRequest.Version+"/configuration/repositories/"+repositoryName
            };
            Console.WriteLine(repository.Link);
            Boolean creationSuccessful = HttpRequest.CreateRepository(repository).IsSuccessStatusCode;
            if (creationSuccessful) {
                logTextBox.AppendText(CreatedRepositoryMessage(repositoryName, repositoryDescriptionTextbox.Text));
                repositoriesBox.Items.Add(repositoryName);
                repositoriesBox.SelectedItem = repositoryName;
            } else {
                logTextBox.AppendText("Could not create repository " + repositoryName + "\n");
            }
            
        }


        //Verification and calculus methods
        private string ValidateUrl(string url) {
            if (url.Length > 0 && url[url.Length - 1] != '/') {
                url = url + "/";
            }
            return url;
        }

        private void CheckForAssociations() {
            XmlNode metaInfoAssoc = XmlParser.GetMetaInfoAssociations(xmlPath);
            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(xmlPath);
            RawDataObject obj = new RawDataObject();
            StringBuilder s = new StringBuilder();

            if (metaInfoAssoc.HasChildNodes) {
                s.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (XmlNode node in metaInfoAssoc.ChildNodes) {
                    s.Append(node.Attributes?[1].Value + " and " + obj.FindCategoryName(node.Attributes?[2].Value, metaInfoTypes) + "\n");
                }
                MessageBox.Show(s.ToString(), "Associations not supported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SortDataList(List<TestDataObject> dataList) {
            testData= new Dictionary<string, List<TestDataObject>>();
            foreach (XmlNode metaInfoType in XmlParser.GetMetaInfoTypes(xmlPath))
            {
                testData.Add(metaInfoType.Attributes?[1].Value ?? throw new InvalidOperationException(), new List<TestDataObject>());
            }

            foreach (TestDataObject row in dataList) {
                testData[row.Category].Add(row);
            }
        }

        private int CountNumberOfRows(Dictionary<string, List<TestDataObject>> data)
        {
            int totalNumberOfRows = 0;
            foreach (string category in data.Keys)
            {
                totalNumberOfRows += data[category].Count;
            }
            return totalNumberOfRows;
        }

        private int EstimatedWaitTime(Dictionary<string, List<TestDataObject>> data)
        {
            //in seconds, based on the number of rows
            return (int)(35 * CountNumberOfRows(data) / (float)2713)+1;
        }


        //UI element attributes and logText methods 
        private void LoadCategoriesIntoListBox()
        {
            categoriesListBox.Items.Clear();
            int numberOfEmptyCategories = 0;
            StringBuilder emptyCategoriesStringBuilder = new StringBuilder();
            foreach (string category in testData.Keys)
            {
                if (testData[category].Count == 0)
                {
                    emptyCategoriesStringBuilder.Append(category + " , ");
                    numberOfEmptyCategories++;
                }
                else
                {
                    categoriesListBox.Items.Add(category + "  (" + testData[category].Count + ")", true);
                }
            }
            if (numberOfEmptyCategories > 0)
            {
                emptyCategoriesStringBuilder.Remove(emptyCategoriesStringBuilder.Length - 3, 3);
            }
            emptyCategoriesStringBuilder.Append(".");
            if (numberOfEmptyCategories == 1)
            {
                logTextBox.AppendText("1 category was empty and has been deleted: " + emptyCategoriesStringBuilder);
            }
            else if (numberOfEmptyCategories > 1)
            {
                logTextBox.AppendText(numberOfEmptyCategories + " categories were empty and have been removed from the list: " + emptyCategoriesStringBuilder);
            }
            logTextBox.Refresh();
        }

        private void TddFileProcessingInWork(Boolean processingInWork) {
            GenerateButton.Enabled = !processingInWork;
            categoriesListBox.Enabled = !processingInWork;
            selectAllButton.Enabled = !processingInWork;
            deselectAllButton.Enabled = !processingInWork;
            verifyUrlButton.Enabled = !processingInWork;
            pickFileButton.Enabled = !processingInWork;
            tddFileProcessingProgressBar.Visible = processingInWork;
        }

        private void MigrationInWork(Boolean inWork) {
            migrationInWork = inWork;

            verifyUrlButton.Enabled = !migrationInWork;
            pickFileButton.Enabled = !migrationInWork;
            migrationProgressBar.Visible = migrationInWork;
            deleteRepositoryButton.Enabled = !migrationInWork;
            clearRepositoryButton.Enabled = !migrationInWork;
            GenerateButton.Enabled = !migrationInWork;
            loadRefreshRepositories.Enabled = !migrationInWork;
        }

        private void ApiConnectionOk(Boolean apiConnectionOk, object sender, EventArgs e) {
            Boolean tddFilePicked = TDDPathTextBox.Text != "";

            createRepositoryButton.Enabled = apiConnectionOk;
            deleteRepositoryButton.Enabled = apiConnectionOk;
            clearRepositoryButton.Enabled = apiConnectionOk;
            GenerateButton.Enabled = apiConnectionOk & tddFilePicked & !migrationInWork;
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
        private string CreatedRepositoryMessage(string name, string description) {
            StringBuilder s = new StringBuilder();
            s.Append("Repository Created : " + name);
            if (description != "") {
                s.Append(" , Description : " + description);
            }

            s.Append("\n");
            return s.ToString();
        }

        private string DeletedRepositoryMessage(string name) {
            return "Repository Deleted : " + name + "\n";
        }

        private string ClearedRepositoryMessage(string name) {
            return "Repository Cleared : " + name + "\n";
        }

        private string MigrationFinishedMessage(int numberOfCategories, string repositoryName) {
            return "Successfully migrated " + numberOfCategories + " out of " + categoriesListBox.Items.Count + " available categories into the repository : \""
                   + repositoryName + "\".\n";
        }

        private void EstimatedWaitTimeMessage(Dictionary<string, List<TestDataObject>> data) {
            if (EstimatedWaitTime(data)>5) {
                logTextBox.AppendText("Estimated waiting time : " +EstimatedWaitTime(data)+" seconds\n");
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
                await Task.Delay(10);
                await LaunchMigration();
                logTextBox.AppendText(MigrationFinishedMessage(categoriesListBox.CheckedItems.Count, repositoriesBox.SelectedItem.ToString()));
                logTextBox.Refresh();
                MigrationInWork(false);
            }
        }

        private async void TddPathTextBox_TextChanged(object sender, EventArgs e) {
            logTextBox.Select();
            logTextBox.AppendText("The .tdd file is being processed. Please wait...\n");
            logTextBox.Refresh();
            TddFileProcessingInWork(true);
            await Task.Delay(10);
            xmlPath = TdsLoader.DecompressTddFileIntoXml(new FileInfo(TDDPathTextBox.Text));
            LoadCategoriesIntoListBox();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => { r.Result = XmlParser.CreateDataList(xmlPath); };
            worker.RunWorkerCompleted += (s, r) => {
                                             SortDataList((List<TestDataObject>)r.Result);

                                             TddFileProcessingInWork(false);
                                             tddFileProcessingProgressBar.Visible = false;
                                             CheckForAssociations();
                                             logTextBox.AppendText("\nThe .tdd file was successfully processed. \n"+CountNumberOfRows(testData)+" records among " + testData.Count + " categories were found.");
                                             logTextBox.AppendText("\n");

                                             LoadCategoriesIntoListBox();

                                             logTextBox.AppendText(
                                                     "\n\nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n\n");
                                             logTextBox.Refresh();
                                         };
            worker.RunWorkerAsync();
        }

        private void PickFileButton_Click(object sender, EventArgs e) {
            openFileDialog.ShowDialog();
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e) {
            categoriesListBox.Items.Clear();
            TDDPathTextBox.Text = openFileDialog.FileName;
        }

        private void CreateRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoryNameTextBox.Text != "") {
                
                if (!repositoriesBox.Items.Contains(repositoryNameTextBox.Text)) {
                    CreateRepository(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text);
                } else {
                    logTextBox.AppendText("Repository \"" + repositoryNameTextBox.Text + "\" already exists \n");
                }
            } else {
                logTextBox.AppendText("Please enter a repository name\n");
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
                Boolean connectionSuccessfull = HttpRequest.SetConnection(ValidateUrl(apiUrlTextBox.Text));
                if (connectionSuccessfull) {
                    apiUrlTextBox.BackColor = Color.Lime;
                    ApiConnectionOk(true, sender, e);
                    apiUrlTextBox.Text = ValidateUrl(apiUrlTextBox.Text);
                    verifyUrlButton.Text = "Change URL";
                    logTextBox.AppendText("Valid URL. \n");
                    if (TDDPathTextBox.Text == "") {
                        logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
                    }
                    logTextBox.Refresh();
                } else {
                    apiUrlTextBox.BackColor = Color.PaleVioletRed;
                    ApiConnectionOk(false, sender, e);
                    repositoriesBox.Items.Clear();
                    logTextBox.AppendText("Not a valid URL.\n");
                    logTextBox.Refresh();
                }
            }
        }

        private void LogTextBox_TextChanged(object sender, EventArgs e) {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }
    }


}