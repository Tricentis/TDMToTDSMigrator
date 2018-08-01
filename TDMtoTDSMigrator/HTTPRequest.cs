using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TDMtoTDSMigrator
{

    public class HttpRequest
    {

        private static HttpClient client;
        private static string _version = "v1.1";

        public static void InitializeHttpClient()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                MaxRequestContentBufferSize = 2147483647
            };

            client = new HttpClient(clientHandler);
        }

        public static Boolean SetAndVerifyConnection(string apiUrl)
        {
            InitializeHttpClient();
            

            if (apiUrl.Length == 0)
            {
                return false;
            }
            try
            {
                client.BaseAddress = new Uri(apiUrl + "/" + _version);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return client.GetAsync("").Result.IsSuccessStatusCode;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        public static void CreateRepository(string repositoryName, string repositoryDescription, string apiUrl)
        {
            client.PostAsync("configuration/repositories/", new StringContent(
                "{\"description\":\"" + repositoryDescription + "\"," +
                "\"location\":\"%PROGRAMDATA%\\\\Tricentis\\\\TestDataService\\\\" + repositoryName + ".db\"," +
                "\"name\":\"" + repositoryName + "\"," +
                "\"type\":1," +
                "\"link\": \"" + apiUrl + "configuration/repositories/" + repositoryName + "\"}",
                Encoding.UTF8, "application/json"));            
        }

        public static void ClearRepository(string repositoryName, string apiUrl)
        {
            client.DeleteAsync(repositoryName);
        }

        public static void DeleteRepository(string repositoryName, string apiUrl)
        {
            client.DeleteAsync("configuration/repositories/" + repositoryName);
        }

        public static string GetRepositories(string apiUrl)
        {
            return client.GetStringAsync("").Result;
        }

        public static async Task<HttpResponseMessage> PostObject(string jSon, string repositoryName, string apiUrl)
        {
            return await client.PostAsync(repositoryName, new StringContent(jSon, Encoding.UTF8, "application/json"));
        }
    }



}
