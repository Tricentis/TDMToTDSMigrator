using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TDMtoTDSMigrator
{

    public class HttpRequest
    {
    
        public static Uri CreateRepository(string repositoryName, string repositoryDescription, string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsync("configuration/repositories/", new StringContent("{\"description\":\"" + repositoryDescription + "\"," +
                                                                                                                     "\"location\":\"%PROGRAMDATA%\\\\Tricentis\\\\TestDataService\\\\" + repositoryName + ".db\"," +
                                                                                                                     "\"name\":\"" + repositoryName+ "\"," +
                                                                                                                     "\"type\":1," +
                                                                                                                     "\"link\": \""+apiUrl+"configuration/repositories/"+repositoryName+"\"}",
                                                                                                                      Encoding.UTF8, "application/json") ).Result;
                    return response.Headers.Location;
                }
            }
        }
        public static Uri ClearRepository(string repositoryName, string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.DeleteAsync(repositoryName).Result;

                    return response.Headers.Location;
                }
            }
        }
        public static Uri DeleteRepository(string repositoryName, string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.DeleteAsync("configuration/repositories/" + repositoryName).Result;
                    return response.Headers.Location;
                }
            }
        }
        public static string GetRepositories(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    client.BaseAddress = new Uri(apiUrl);
                    return client.GetStringAsync("").Result;
                }
            }
        }
        public static Boolean VerifyApiUrl(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {

                {
                    if (apiUrl.Length == 0)
                    {
                        return false;
                    }
                    try
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.PostAsync("configuration/repositories/", new StringContent("{\"name\":\"testconnectionwithapi2546987\",\"location\":\"testconnectionwithapi2546987\",\"description\":\"testconnectionwithapi2546987\"}", Encoding.UTF8, "application/json")).Result;
                        DeleteRepository("testconnectionwithapi2546987", apiUrl);
                        return (response.IsSuccessStatusCode);
                    }catch(ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                    catch (UriFormatException)
                    {
                        return false;
                    }
                        
                    }
                }
            }

        public static async Task<HttpResponseMessage> PostObject(string jSon, string repositoryName, string apiUrl)
        {
            try
            {

                StringContent json = new StringContent(jSon, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        Task<HttpResponseMessage> task =client.PostAsync(repositoryName, json);
                        HttpResponseMessage response = await task;
                        response.EnsureSuccessStatusCode();
                        return response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return null;

        }


    }
}
