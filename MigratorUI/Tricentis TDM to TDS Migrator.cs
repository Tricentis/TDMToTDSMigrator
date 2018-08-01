using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using TDMtoTDSMigrator;

namespace MigratorUI
{
    public partial class TdsMigrator : Form {
        public TdsMigrator() {
            InitializeComponent();
        }

        private List<DataRow> dataList;

        private string xmlPath;

        private Boolean migrationInWork = false;

        //Initialization 
        private void TdsMigrator_Load(object sender, EventArgs e) {
            logTextBox.AppendText(
                    "Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
            verifyUrlButton.Select();
        }

        //Migration and tdd processing methods
        private async Task<HttpResponseMessage> LaunchMigration(List<string> authorizedTypes,
                                                                Boolean applyFilter,
                                                                List<DataRow> objectList,
                                                                string selectedRepository,
                                                                string apiUrl) {
            if (applyFilter) {
                HttpResponseMessage message = await TdsLoader.MigrateXmlDataIntoTdsWithFilter(xmlPath, objectList, selectedRepository, authorizedTypes, apiUrl);
                return message;
            } else {
                HttpResponseMessage message = await TdsLoader.MigrateXmlDataIntoTdsWithoutFilter(xmlPath, objectList, selectedRepository, apiUrl);
                return message;
            }
        }

        private void LoadCategoriesIntoListBox() {
            foreach (XmlNode metaInfoType in XmlParser.GetMetaInfoTypes(xmlPath)) {
                categoriesListBox.Items.Add(metaInfoType.Attributes?[1].Value ?? throw new InvalidOperationException(), true);
            }
        }



        //Verification methods
        private string ValidateUrl(string url) {
            if (url.Length > 0 && url[url.Length - 1] != '/') {
                url = url + "/";
            }
            return url;
        }

        private void CheckForAssociations() {
            XmlNode metaInfoAssoc = XmlParser.GetMetaInfoAssociations(xmlPath);
            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(xmlPath);
            DataRow obj = new DataRow();
            StringBuilder s = new StringBuilder();

            if (metaInfoAssoc.HasChildNodes) {
                s.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (XmlNode node in metaInfoAssoc.ChildNodes) {
                    s.Append(node.Attributes?[1].Value + " and " + obj.FindCategoryName(node.Attributes?[2].Value, metaInfoTypes) + "\n");
                }
                MessageBox.Show(s.ToString(), "Associations not supported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /*private string CheckForEmptyCategories() {
            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(xmlPath);
            List<string> emptyCategories = new List<string>();

            foreach (XmlNode metaInfoType in metaInfoTypes.ChildNodes) {
                emptyCategories.Add(metaInfoType.Attributes?[1].Value);
            }

            foreach (DataRow row in dataList) {
                foreach (string category in emptyCategories) {
                    if (row.GetCategoryName() == category) {
                        emptyCategories.Remove(category);
                        break;
                    }
                }
            }

            if (emptyCategories.Count == 0) {
                return ".";
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(", including " + emptyCategories.Count + " empty : \n");

            foreach (string emptyCategory in emptyCategories) {
                foreach (string category in categoriesListBox.Items) {
                    if (category == emptyCategory) {
                        builder.Append(category + " , ");
                        categoriesListBox.Items.Remove(category);
                        break;
                    }
                }
            }
            builder = builder.Remove(builder.Length - 3, 2);
            return builder.ToString();
        }*/

        private void CountCategoryMembersAndRemoveEmptyCategories() {
            Dictionary<string,int> categoriesMembersCount = new Dictionary<string, int>();
            foreach (var category in categoriesListBox.Items) {
                categoriesMembersCount.Add(category.ToString(),0);
            }
            foreach (var row in dataList) {
                categoriesMembersCount[row.GetCategoryName()]++;
            }

            int numberOfEmptyCategories = 0;
            StringBuilder emptyCategoriesStringBuilder = new StringBuilder();
            for (int i = 0 ; i < categoriesListBox.Items.Count ; i++) {
                Console.WriteLine(i + "  " + categoriesListBox.Items[i]);
                if (categoriesMembersCount[categoriesListBox.Items[i].ToString()] == 0) {
                    emptyCategoriesStringBuilder.Append(categoriesListBox.Items[i] +" , ");
                    categoriesListBox.Items.RemoveAt(i);
                    if (i != 0) { i--; }
                    numberOfEmptyCategories++;

                } else {
                    categoriesListBox.Items[i] = categoriesListBox.Items[i] + "  (" + categoriesMembersCount[categoriesListBox.Items[i].ToString()] + ")";
                }
                
            }
            if (numberOfEmptyCategories>0) {
                emptyCategoriesStringBuilder.Remove(emptyCategoriesStringBuilder.Length - 3, 3);
            }
            emptyCategoriesStringBuilder.Append(".");
            if (numberOfEmptyCategories == 1 ) {
                logTextBox.AppendText("1 category was empty and has been deleted: " + emptyCategoriesStringBuilder); 
            }else if (numberOfEmptyCategories>1) {
                logTextBox.AppendText(numberOfEmptyCategories + " categories were empty and have been deleted: " + emptyCategoriesStringBuilder);
            }
            logTextBox.Refresh();
        }

        //UI element attributes and logText methods 
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
            string reposJson = HttpRequest.GetRepositories(ValidateUrl(apiUrlTextBox.Text));
            string[] repoList = JsonConverter.ConvertJsonIntoRepositoryList(reposJson);

            foreach (string repository in repoList) {
                repositoriesBox.Items.Add(repository);
            }
            if (repoList.Length > 0) {
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

        //Event methods
        private async void LoadIntoRepositoryButton_Click(object sender, EventArgs e) {
            Boolean applyFilter = false;
            List<string> filteredCategories = new List<string>();

            foreach (var category in categoriesListBox.CheckedItems) {
                filteredCategories.Add(category.ToString());
            }

            applyFilter = filteredCategories.Count < categoriesListBox.Items.Count;

            if (repositoriesBox.SelectedItem == null) {
                logTextBox.AppendText("Please pick a repository, or create one\n");
            } else if (filteredCategories.Count == 0) {
                logTextBox.AppendText("Please pick at least one category\n");
            } else {
                logTextBox.AppendText("Migrating " + filteredCategories.Count + " categories into \"" + repositoriesBox.SelectedItem + "\". Please wait...\n");

                MigrationInWork(true);
                await Task.Delay(10);

                await LaunchMigration(filteredCategories, applyFilter, dataList, repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));

                logTextBox.AppendText(MigrationFinishedMessage(filteredCategories.Count, repositoriesBox.SelectedItem.ToString()));
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
            worker.DoWork += (s, r) => { r.Result = dataList = XmlParser.CreateDataList(xmlPath); };
            worker.RunWorkerCompleted += (s, r) => {
                                             dataList = (List<DataRow>)r.Result;
                                             
                                             TddFileProcessingInWork(false);
                                             tddFileProcessingProgressBar.Visible = false;
                                             CheckForAssociations();
                                             logTextBox.AppendText("\nThe .tdd file was successfully processed. \n" + dataList.Count + " objects among " + categoriesListBox.Items.Count
                                                                   + " categories were found.");
                                             logTextBox.AppendText("\n");
                                             CountCategoryMembersAndRemoveEmptyCategories();
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
                RefreshRepositoriesList();
                if (!repositoriesBox.Items.Contains(repositoryNameTextBox.Text)) {
                    HttpRequest.CreateRepository(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text, ValidateUrl(apiUrlTextBox.Text));
                    logTextBox.AppendText(CreatedRepositoryMessage(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text));
                    repositoriesBox.Items.Add(repositoryNameTextBox.Text);
                    repositoriesBox.SelectedItem = repositoryNameTextBox.Text;
                } else {
                    logTextBox.AppendText("Repository \"" + repositoryNameTextBox.Text + "\" already exists \n");
                }
            } else {
                logTextBox.AppendText("Please enter a repository name\n");
            }
        }

        private void ClearRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem != null) {
                var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                    "Clear " + repositoriesBox.SelectedItem + " repository",
                                                    MessageBoxButtons.OKCancel,
                                                    MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.OK) {
                    HttpRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    logTextBox.AppendText(ClearedRepositoryMessage(repositoriesBox.SelectedItem.ToString()));
                }
            } else {
                logTextBox.AppendText("Please select a repository to clear \n");
            }
        }

        private void DeleteRepositoryButton_Click(object sender, EventArgs e) {
            if (repositoriesBox.SelectedItem != null) {
                var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                                    "Clear " + repositoriesBox.SelectedItem + " repository",
                                                    MessageBoxButtons.OKCancel,
                                                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.OK) {
                    HttpRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    HttpRequest.DeleteRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    logTextBox.AppendText(DeletedRepositoryMessage(repositoriesBox.SelectedItem.ToString()));
                    repositoriesBox.Items.Remove(repositoriesBox.SelectedItem);

                    if (repositoriesBox.Items.Count > 0) {
                        repositoriesBox.SelectedItem = repositoriesBox.Items[0];
                    }
                }
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
                    verifyUrlButton.Text = "Change URL";
                    logTextBox.AppendText("Valid URL. \n");
                    if (TDDPathTextBox.Text == "") {
                        logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
                    }
                    logTextBox.Refresh();
                } else {
                    apiUrlTextBox.BackColor = Color.PaleVioletRed;
                    ApiConnectionOk(false, sender, e);
                    logTextBox.AppendText("Not a valid URL.\n");
                    logTextBox.Refresh();
                }
            }
        }

        private void LogTextBox_TextChanged(object sender, EventArgs e) {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private async void CategoriesListBox_EnabledChanged(object sender, EventArgs e) {

        }
    }
}