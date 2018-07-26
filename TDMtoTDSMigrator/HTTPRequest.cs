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

    public class HTTPRequest
    {
        public static Uri CreateRepository(string repositoryName, string repositoryDescription, string apiURL)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsync("configuration/repositories/", new StringContent("{\"description\":\"" + repositoryDescription + "\",\"location\":\"%PROGRAMDATA%\\\\Tricentis\\\\TestDataService\\\\" + repositoryName + ".db\",\"name\":\"" + repositoryName+ "\",\"type\":1,\"link\": \""+apiURL+"configuration/repositories/"+repositoryName+"\"}", Encoding.UTF8, "application/json") ).Result;
                  
                    //return URI of the created resource.
                    return response.Headers.Location;
                }
            }
        }
        public static Uri CreateRepository(string repositoryName, string apiURL)
        {
            return CreateRepository(repositoryName,"",apiURL);
        }
        public static Uri ClearRepository(string repositoryName, string apiURL)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    // Update port # in the following line.
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.DeleteAsync(repositoryName).Result;
                    //response.EnsureSuccessStatusCode();
                    if (response.StatusCode.ToString() == "400")
                    {
                        //repo created message
                    }
                    else
                    {
                        //failed to create message
                    }

                    // return URI of the created resource.
                    return response.Headers.Location;
                }
            }
        }
        public static Uri DeleteRepository(string repositoryName, string apiURL)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    // Update port # in the following line.
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.DeleteAsync("configuration/repositories/" + repositoryName).Result;
                    //response.EnsureSuccessStatusCode();
                    if (response.StatusCode.ToString() == "400")
                    {
                        //repo deleted message
                    }
                    
                    else
                    {
                        //failed to delete message
                    }

                    // return URI of the created resource.
                    return response.Headers.Location;
                }
            }
        }
        public static string GetRepositories(string apiURL)
        {
            using (HttpClient client = new HttpClient())
            {
                {
                    string json = "";

                    // Update port # in the following line.
                    apiURL.Remove(apiURL.Length - 1, 1);
                    client.BaseAddress = new Uri(apiURL);

                    client.DefaultRequestHeaders.Accept.Clear();
                    json = client.GetStringAsync("").Result;
                    JSONConverter.ParseJsonIntoRepositoryList(json);
                    return json;
                }
            }
        }
        public static Boolean VerifyApiURI(string apiURL)
        {
            using (HttpClient client = new HttpClient())
            {

                {
                    if (apiURL.Length == 0)
                    {
                        return false;
                    }
                    try
                    {
                        client.BaseAddress = new Uri(apiURL);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.PostAsync("configuration/repositories/", new StringContent("{\"name\":\"testconnectionwithapi2546987\",\"location\":\"testconnectionwithapi2546987\",\"description\":\"testconnectionwithapi2546987\"}", Encoding.UTF8, "application/json")).Result;
                        DeleteRepository("testconnectionwithapi2546987", apiURL);
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

        public static async Task<HttpResponseMessage> PostObject(string jSon, string repositoryName, string apiURL)
        {
            try
            {
                // Create a new row

                StringContent json = new StringContent(jSon, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    {
                        client.BaseAddress = new Uri(apiURL);
                        client.DefaultRequestHeaders.Accept.Clear();
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
