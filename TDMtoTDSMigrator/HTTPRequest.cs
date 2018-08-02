﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TestDataContract.Configuration;

namespace TDMtoTDSMigrator {
    public class HttpRequest {

        private static HttpClient client;

        public static readonly string version = "v1.1";

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
            InitializeHttpClient();
            if (apiUrl.Length == 0) {
                return false;
            }
            try {
                Client.BaseAddress = new Uri(apiUrl + "/" + version);
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return Client.GetAsync("").Result.IsSuccessStatusCode;
            } catch (Exception) {
                return false;
            }
        }

        public static HttpResponseMessage CreateRepository(TestDataRepository repository) {
            return Client.PostAsync("configuration/repositories/",
                                    new StringContent(JsonConvert.SerializeObject(repository),Encoding.UTF8,"application/json")).Result;
        }

        public static HttpResponseMessage ClearRepository(string repositoryName) {
            return Client.DeleteAsync(repositoryName).Result;
        }

        public static HttpResponseMessage DeleteRepository(string repositoryName) {
            return Client.DeleteAsync("configuration/repositories/" + repositoryName).Result;
        }

        public static string GetRepositories() {
            return Client.GetStringAsync("").Result;
        }

        public static async Task<HttpResponseMessage> PostObject(string jSon, string repositoryName, string apiUrl) {
            return await Client.PostAsync(repositoryName, new StringContent(jSon, Encoding.UTF8, "application/json"));
        }
    }
}