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
    public partial class TdsMigrator : Form
    {
        public TdsMigrator()
        {
            InitializeComponent();
        }


        private List<DataRow> dataList; // List of TableObjects each corresponding to one row of data 
        private string xmlPath; 
        private Boolean migrationInWork=false;

        //Initialization 
        private void TdsMigrator_Load(object sender, EventArgs e)
        {
            logTextBox.AppendText("Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
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
        private void LoadCategoriesIntoListBox()
        {

            XmlNode metaInfoType = XmlParser.GetMetaInfoTypes(xmlPath);
            for (int i = 0; i < metaInfoType.ChildNodes.Count; i++)
            {
                categoriesListBox.Items.Add(metaInfoType.ChildNodes[i].Attributes?[1].Value ?? throw new InvalidOperationException(), true);
            }
        }


        //Verification methods
        private string ValidateUrl(string url)
        { 
            if (url.Length > 0 && url[url.Length - 1] != '/')
            {
                url = url + "/";
            }
            return url;
        }
        private void CheckForAssociations()
        {

            XmlNode metaInfoAssoc = XmlParser.GetMetaInfoAssociations(xmlPath);
            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(xmlPath);
            DataRow obj = new DataRow();
            StringBuilder s = new StringBuilder();

            if (metaInfoAssoc.HasChildNodes)
            {
                
                s.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (XmlNode node in metaInfoAssoc.ChildNodes)
                {
                    
                    s.Append(node.Attributes?[1].Value + " and " + obj.FindCategoryName(node.Attributes?[2].Value,metaInfoTypes) + "\n");
                }
                MessageBox.Show(s.ToString(),
                                    "Associations not supported",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private string CheckForEmptyCategories()
        {

            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(xmlPath);

            List<string> emptyCategories = new List<string>();

            foreach (XmlNode metaInfoType in metaInfoTypes.ChildNodes)
            {
                emptyCategories.Add(metaInfoType.Attributes?[1].Value);
            }

            foreach (DataRow row in dataList) {
                for (int j = 0; j < emptyCategories.Count; j++)
                {
                    if (row.GetCategoryName() == emptyCategories[j])
                    {
                        emptyCategories.RemoveAt(j);
                    }
                }
            }

            
            StringBuilder sb = new StringBuilder();

            if (emptyCategories.Count > 0)
            {
                sb.Append(", including " + emptyCategories.Count + " empty : \n");
            }
            
            foreach (string s in emptyCategories)
            {
                for (int i = 0; i < categoriesListBox.Items.Count; i++)
                {
                    if (categoriesListBox.Items[i].ToString() == s) {
                        sb.Append(categoriesListBox.Items[i] + " ");
                        categoriesListBox.Items.RemoveAt(i);
                    }
                }
            }

            return sb.ToString();
        }


        //UI element attributes and logText methods 
        private void TddFileProcessingInWork(Boolean processingInWork)
        {

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
        private void ApiConnectionOk(Boolean apiConnectionOk, object sender, EventArgs e)
        {
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

            if (apiConnectionOk)
            {
                pickFileButton.Text = "Browse...";
                LoadRefreshRepositories_Click(sender, e);
                pickFileButton.Select();
            }
            else
            {
                pickFileButton.Text = "...";
            }

            


        }
        private int EstimatedWaitTime()
        {            
            FileInfo tdd = new FileInfo(TDDPathTextBox.Text);
            float scaleLength = 611797; // length of the tdd file that is the scale for processing time estimation (~30 seconds for this file)
            return (int)(tdd.Length / scaleLength * 30);
        }
        private void RefreshRepositoriesList()
        {
            repositoriesBox.Items.Clear();
            string reposJson = HttpRequest.GetRepositories(ValidateUrl(apiUrlTextBox.Text));
            string[] repoList = JsonConverter.ConvertJsonIntoRepositoryList(reposJson);

            foreach (string repository in repoList) {
                repositoriesBox.Items.Add(repository);
            }
            if (repoList.Length > 0)
            {
                repositoriesBox.SelectedItem = repositoriesBox.Items[0];
            }
        }


        //logText message generation methoids
        private string CreatedRepositoryMessage(string name, string description)
        {
            StringBuilder s = new StringBuilder();
            s.Append("Repository Created : " + name);
            if (description != "")
            {
                s.Append(" , Description : " + description);
            }

            s.Append("\n");
            return  s.ToString();
        }
        private string DeletedRepositoryMessage(string name)
        {
            return "Repository Deleted : " + name+"\n";
        }
        private string ClearedRepositoryMessage(string name)
        {
            return "Repository Cleared : " + name+"\n";
        }
        private string MigrationFinishedMessage(int numberOfCategories, string repositoryName)
        {
            return "Successfully migrated "+numberOfCategories+" out of "+categoriesListBox.Items.Count+" available categories into the repository : \""+repositoryName+"\".\n";
        }


        //Event methods
        private async void LoadIntoRepositoryButton_Click(object sender, EventArgs e)
        {   
            Boolean applyFilter = false;
            List<string> filteredCategories = new List<string>();

            foreach (var category in categoriesListBox.CheckedItems) {
                filteredCategories.Add(category.ToString());
            }

            applyFilter = filteredCategories.Count < categoriesListBox.Items.Count;
                

            if (repositoriesBox.SelectedItem == null)
            {
                logTextBox.AppendText("Please pick a repository, or create one\n");

            }
            else if (filteredCategories.Count == 0)
                {
                logTextBox.AppendText("Please pick at least one category\n");
            }
            else
            {
                logTextBox.AppendText("Migrating " + filteredCategories.Count + " categories into \"" + repositoriesBox.SelectedItem + "\". Please wait...\n");

                MigrationInWork(true);
                await Task.Delay(10);

                await LaunchMigration(filteredCategories, applyFilter, dataList, repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));

                logTextBox.AppendText(MigrationFinishedMessage(filteredCategories.Count, repositoriesBox.SelectedItem.ToString()));
                logTextBox.Refresh();

                MigrationInWork(false);
            }

        }
        private async void TddPathTextBox_TextChanged(object sender, EventArgs e)
        {
            int estimatedWait = EstimatedWaitTime();
            logTextBox.Select();

            logTextBox.AppendText("The .tdd file is being processed.\nPlease wait...\n");
            if (estimatedWait > 5)
            {
                logTextBox.AppendText("Estimated waiting time : " + estimatedWait + " seconds \n");
            }            
            logTextBox.Refresh();

            TddFileProcessingInWork(true);
            await Task.Delay(10);          

            xmlPath = TdsLoader.DecompressTddFileIntoXml(new FileInfo(TDDPathTextBox.Text));

            LoadCategoriesIntoListBox();
            
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => {
                r.Result = dataList = XmlParser.CreateDataList(xmlPath);
            };
            worker.RunWorkerCompleted += (s, r) => {
                dataList =(List<DataRow>)r.Result;

                logTextBox.AppendText("\nThe .tdd file was successfully processed. \n" + categoriesListBox.Items.Count + " categories were found");
                logTextBox.AppendText(CheckForEmptyCategories());
                logTextBox.AppendText("\n");                             
                logTextBox.AppendText("\nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n\n");
                logTextBox.Refresh();
   
                TddFileProcessingInWork(false);
                tddFileProcessingProgressBar.Visible = false;
                CheckForAssociations();
            };
            worker.RunWorkerAsync();


        }
        private void PickFileButton_Click(object sender, EventArgs e)
        {

            openFileDialog.ShowDialog();

        }
        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            categoriesListBox.Items.Clear();
            TDDPathTextBox.Text = openFileDialog.FileName;
            
        }
        private void CreateRepositoryButton_Click(object sender, EventArgs e)
        {
            if (repositoryNameTextBox.Text != "")
            {
                RefreshRepositoriesList();

                if (!repositoriesBox.Items.Contains(repositoryNameTextBox.Text))
                {  
                HttpRequest.CreateRepository(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text, ValidateUrl(apiUrlTextBox.Text));
                logTextBox.AppendText(CreatedRepositoryMessage(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text));
                repositoriesBox.Items.Add(repositoryNameTextBox.Text);
                repositoriesBox.SelectedItem = repositoryNameTextBox.Text;
                }
                else
                {
                    logTextBox.AppendText("Repository \"" + repositoryNameTextBox.Text + "\" already exists \n");
                }
            }
            else
            {
                logTextBox.AppendText("Please enter a repository name\n");
            }

            
        }
        private void ClearRepositoryButton_Click(object sender, EventArgs e)
        {
            if (repositoriesBox.SelectedItem != null)
            {
                var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                     "Clear " + repositoriesBox.SelectedItem + " repository",
                                     MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.OK)
                {
                    HttpRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    logTextBox.AppendText(ClearedRepositoryMessage(repositoriesBox.SelectedItem.ToString()));
                }
                
            }
            else
            {
                logTextBox.AppendText("Please select a repository to clear \n");

            }
        }
        private void DeleteRepositoryButton_Click(object sender, EventArgs e) 
        {
            
            if (repositoriesBox.SelectedItem != null)
            {
                var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                     "Clear " + repositoriesBox.SelectedItem + " repository",
                                     MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.OK)
                {

                    HttpRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    HttpRequest.DeleteRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    logTextBox.AppendText(DeletedRepositoryMessage(repositoriesBox.SelectedItem.ToString()));
                    repositoriesBox.Items.Remove(repositoriesBox.SelectedItem);

                    if (repositoriesBox.Items.Count > 0)
                    {
                        repositoriesBox.SelectedItem = repositoriesBox.Items[0];
                    }
                }

                
            }
            else
            {
                logTextBox.AppendText("Please select a repository to delete \n");
            }




        }
        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            for(int  i = 0; i<categoriesListBox.Items.Count; i++)
            {
                categoriesListBox.SetItemChecked(i, true);
            }
            
        }
        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < categoriesListBox.Items.Count; i++)
            {
                categoriesListBox.SetItemChecked(i, false);
            }
        }
        private void LoadRefreshRepositories_Click(object sender, EventArgs e)
        {
            RefreshRepositoriesList();
        }
        private void VerifyUrlButton_Click_1(object sender, EventArgs e)
        {
            if(verifyUrlButton.Text == "Change URL")
            {
                verifyUrlButton.Text = "Verify URL";
                ApiConnectionOk(false,sender,e);

            }
            else
            {

                if (HttpRequest.SetAndVerifyConnection(ValidateUrl(apiUrlTextBox.Text)))
                {
                    apiUrlTextBox.BackColor = Color.Lime;
                    ApiConnectionOk(true, sender, e);
                    verifyUrlButton.Text = "Change URL";
                    logTextBox.AppendText("Valid URL. \n");
                    if (TDDPathTextBox.Text == "")
                    {
                        logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
                    }
                    logTextBox.Refresh();
                    


                }
                else
                {
                    apiUrlTextBox.BackColor = Color.PaleVioletRed;
                    ApiConnectionOk(false, sender, e);
                    logTextBox.AppendText("Not a valid URL.\n");
                    logTextBox.Refresh();
                }
            }
           


        }
        private void LogTextBox_TextChanged(object sender, EventArgs e)
        {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }
    }


}

