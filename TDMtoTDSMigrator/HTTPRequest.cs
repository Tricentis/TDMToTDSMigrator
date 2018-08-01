using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TDMtoTDSMigrator
{

    public class HttpRequest
    {

        private static HttpClient _client;
        private static string _version = "v1.1";

        public static void InitializeHttpClient()
        {
            ServicePointManager.DefaultConnectionLimit = 1000000;

            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                MaxRequestContentBufferSize = 1000000000
            };

            _client = new HttpClient(clientHandler);
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
                _client.BaseAddress = new Uri(apiUrl + "/" + _version);
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return _client.GetAsync("").Result.IsSuccessStatusCode;
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
            _client.PostAsync("configuration/repositories/", new StringContent(
                "{\"description\":\"" + repositoryDescription + "\"," +
                "\"location\":\"%PROGRAMDATA%\\\\Tricentis\\\\TestDataService\\\\" + repositoryName + ".db\"," +
                "\"name\":\"" + repositoryName + "\"," +
                "\"type\":1," +
                "\"link\": \"" + apiUrl + "configuration/repositories/" + repositoryName + "\"}",
                Encoding.UTF8, "application/json"));            
        }

        public static void ClearRepository(string repositoryName, string apiUrl)
        {
            _client.DeleteAsync(repositoryName);
        }

        public static void DeleteRepository(string repositoryName, string apiUrl)
        {
            _client.DeleteAsync("configuration/repositories/" + repositoryName);
        }

        public static string GetRepositories(string apiUrl)
        {
            return _client.GetStringAsync("").Result;
        }

        public static async Task<HttpResponseMessage> PostObject(string jSon, string repositoryName, string apiUrl)
        {
            return await _client.PostAsync(repositoryName, new StringContent(jSon, Encoding.UTF8, "application/json"));
        }
    }



}
