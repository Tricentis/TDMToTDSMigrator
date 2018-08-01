using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TDMtoTDSMigrator {
    public class HttpRequest {

        private static HttpClient client;

        private static readonly string version = "v1.1";

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

        public static Boolean SetConnection(string apiUrl) {

            if (apiUrl.Length == 0) {
                return false;
            }
            try {
                Client.BaseAddress = new Uri(apiUrl + "/" + version);
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return Client.GetAsync("").Result.IsSuccessStatusCode;
            } catch (ArgumentException) {
                return false;
            } catch (UriFormatException) {
                return false;
            }
        }

        public static void CreateRepository(string repositoryName, string repositoryDescription, string apiUrl) {
            Client.PostAsync("configuration/repositories/",
                             new StringContent("{\"description\":\"" + repositoryDescription + "\"," + "\"location\":\"%PROGRAMDATA%\\\\Tricentis\\\\TestDataService\\\\"
                                               + repositoryName + ".db\"," + "\"name\":\"" + repositoryName + "\"," + "\"type\":1," + "\"link\": \"" + apiUrl
                                               + "configuration/repositories/" + repositoryName + "\"}",
                                               Encoding.UTF8,
                                               "application/json"));
        }

        public static void ClearRepository(string repositoryName, string apiUrl) {
            Client.DeleteAsync(repositoryName);
        }

        public static void DeleteRepository(string repositoryName, string apiUrl) {
            Client.DeleteAsync("configuration/repositories/" + repositoryName);
        }

        public static string GetRepositories(string apiUrl) {
            return Client.GetStringAsync("").Result;
        }

        public static async Task<HttpResponseMessage> PostObject(string jSon, string repositoryName, string apiUrl) {
            return await Client.PostAsync(repositoryName, new StringContent(jSon, Encoding.UTF8, "application/json"));
        }
    }
}