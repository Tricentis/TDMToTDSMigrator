using System;
using System.Text;
using Newtonsoft.Json;

namespace TDMtoTDSMigrator
{
    public class Repository
    {
        public string name { get; set; }
       

    }

    public class JsonConverter {
        public static string ConvertObjectIntoJsonString(DataRow obj) {
            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");

            for (int i = 0; i < obj.GetAttributes().Count - 1; i++) {
                builder.Append("\"" + obj.GetAttributes()[i][0] + "\":\"" + obj.GetAttributes()[i][1] + "\" , ");
            }
            builder.Append("\"" + obj.GetAttributes()[obj.GetAttributes().Count - 1][0] + "\":\"" + obj.GetAttributes()[obj.GetAttributes().Count - 1][1] + "\"");
            builder.Append("}\n");

            return builder.ToString();
        }

        public static string ConvertObjectIntoJsonPostRequest(DataRow obj) {
            return "{\"category\" : \"" + obj.GetCategoryName() + "\" , \"consumed\" : false, \"data\" : " + ConvertObjectIntoJsonString(obj) + "}";
        }

        public static string[] ConvertJsonIntoRepositoryList(string json) {
            json = json.Remove(0, 1);
            json = json.Remove(json.Length - 1, 1);

            string[] splitter = new string [1];
            splitter[0] = "},";
            string[] repositoryList = json.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < repositoryList.Length - 1; i++) {
                repositoryList[i] = repositoryList[i] + "}";
            }

            for (int i = 0; i < repositoryList.Length; i++) {
                var repository = JsonConvert.DeserializeObject<Repository>(repositoryList[i]);
                repositoryList[i] = repository.name;
            }

            return repositoryList;
        }
    }

    


     
}
