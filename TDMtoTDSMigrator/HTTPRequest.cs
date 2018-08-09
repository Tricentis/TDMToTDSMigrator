using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TestDataContract.Configuration;
using TestDataContract.TestData;

namespace TDMtoTDSMigrator {
    public class HttpRequest {
        private static HttpClient client;

        public static readonly string Version = "v1.1";

        private static HttpClient Client {
            get {
                if (client == null) {
                    InitializeHttpClient();
                }
                return client;
            }
        }

        public static void InitializeHttpClient() {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            client = new HttpClient();
        }

        public static bool SetConnection(string apiUrl) {
            InitializeHttpClient();
            if (apiUrl.Length == 0) {
                return false;
            }
            try {
                Client.BaseAddress = new Uri(apiUrl + "/" + Version);
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return Client.GetAsync("").Result.IsSuccessStatusCode;
            } catch (Exception) {
                return false;
            }
        }

        public static HttpResponseMessage CreateRepository(TestDataRepository repository) {
            return Client.PostAsync("configuration/repositories/", new StringContent(JsonConvert.SerializeObject(repository), Encoding.UTF8, "application/json")).Result;
        }

        public static HttpResponseMessage ClearRepository(TestDataRepository repository) {
            return Client.DeleteAsync(repository.Name).Result;
        }

        public static HttpResponseMessage DeleteRepository(TestDataRepository repository) {
            return Client.DeleteAsync("configuration/repositories/" + repository.Name).Result;
        }

        public static List<TestDataRepository> GetRepositories() {
            return JsonConvert.DeserializeObject<List<TestDataRepository>>(Client.GetStringAsync("configuration/repositories").Result);
        }

        public static async Task<HttpResponseMessage> PostObject(string jSon, TestDataRepository repository, string apiUrl) {
            return await Client.PostAsync(repository.Name, new StringContent(jSon, Encoding.UTF8, "application/json"));
        }

        public static Task<HttpResponseMessage> Migrate(Dictionary<string, TestDataCategory> testData, TestDataRepository repository, string apiUrl) {
            Task<HttpResponseMessage> message = null;
            foreach (string category in testData.Keys) {
                foreach (TestDataObject obj in testData[category].Elements) {
                    message = PostObject(JsonConvert.SerializeObject(obj), repository, apiUrl);
                }
            }
            return message;
        }

        //
        //
        //DELETE AFTER INMEMORY API BUGFIX
        //
        //
        public static async Task<HttpResponseMessage> MigrateInMemory(Dictionary<string, TestDataCategory> testData, TestDataRepository repository, string apiUrl) {
            foreach (string category in testData.Keys) {
                foreach (TestDataObject obj in testData[category].Elements) {
                    await PostObject(JsonConvert.SerializeObject(obj), repository, apiUrl);
                }
            }
            return new HttpResponseMessage();
        }

        public static int EstimatedMigrationWaitTime(Dictionary<string, TestDataCategory> data, TestDataRepository repository) {
            int numberOfRecords = 0;
            foreach (TestDataCategory category in data.Values) {
                numberOfRecords += category.ElementCount;
            }
            //
            //
            //DELETE AFTER INMEMORY API BUGFIX
            //
            //
            if (repository.Type == DataBaseType.InMemory) {
                return (int)(230 * numberOfRecords / (float)618);
            }
            //in seconds, based on the number of objects
            return (int)(35 * numberOfRecords / (float)2713) + 1;
        }
    }
}