using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TDMtoTDSMigrator
{
    public class Repository
    {
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string link { get; set; }
        

    }
    public class JSONConverter
    { 
        public static string ConvertObjectIntoJsonString(TableObject obj)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("{ ");

            //Unwraps the attribute "Attributes" (list<string[]>) of the TableObject for valid Json creation  
            for(int i = 0;i<obj.GetAttributes().Count-1; i++)
            {
                builder.Append("\"" + obj.GetAttributes()[i][0] + "\":\"" + obj.GetAttributes()[i][1] + "\" , ");
            }
            builder.Append("\""+ obj.GetAttributes()[obj.GetAttributes().Count - 1][0] + "\":\"" + obj.GetAttributes()[obj.GetAttributes().Count - 1][1] +"\"");
            builder.Append("}\n");
            
            return builder.ToString();

        }
        public static string ConvertObjectIntoJsonPostRequest(TableObject obj, XmlNode metaInfoAttributes) => "{\"category\" : \"" +obj.GetCategoryName()+ "\" , \"consumed\" : false, \"data\" : " + ConvertObjectIntoJsonString(obj) + "}";
        public static string[] ParseJsonIntoRepositoryList(string json)
        {
            json = json.Remove(0,1);
            
            json = json.Remove(json.Length - 1, 1);
            string [] splitter=new string [1];
            splitter[0] = "},";
            string [] list = json.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i <list.Length-1; i++)
            {
                list[i] = list[i] + "}";
            }

            Repository repository = new Repository();
            for (int i = 0; i < list.Length; i++)
            {
                repository = JsonConvert.DeserializeObject<Repository>(list[i]);
                list[i] = repository.name;

            }
            
            return list;
        }
    }

    


     
}
