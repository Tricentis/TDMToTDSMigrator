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
        public static string JSONifyObject(TableObject obj)
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
        public static string JSONifyObjectForAPI(TableObject obj, XmlNode metaInfoAttributes) => "{\"category\" : \"" +obj.GetTypeName()+ "\" , \"consumed\" : false, \"data\" : " + JSONifyObject(obj) + "}";
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

    /*
     Unused Method : TransformObjectListIntoJSonEquivalent

    //Transforms the whole TableObject list into a valid Json file. Useless because API input is single object

     public static string TransformObjectListIntoJSonEquivalent(List<TableObject> objectList, XmlDocument doc)
        {
            //Creates a list of stringbuilders, each will build the Json part corresponding to a type of object (ex:Person, City)
            //The stringbuilders will then be appened to form a valid Json string

            JSONconverter Jsonconverter = new JSONconverter();
            XMLParser parser = new XMLParser();
            List<StringBuilder> builders = new List<StringBuilder>();

            //Determine the number of types
            int numberOfTypes = XMLParser.getMetaInfoTypes(XMLParser.getParentNodeOfData(doc)).ChildNodes.Count;


            for (int i = 0; i < numberOfTypes; i++)
            {
                builders.Add(new StringBuilder());
            }

            for (int i = 0; i < objectList.Count; i++)
            {
                builders[Int32.Parse(objectList[i].GetTypeId()) - 1].Append(JsonifyObject(objectList[i], false, XMLParser.getMetaInfoAttributes(XMLParser.getParentNodeOfData(doc))));
            }

            StringBuilder finaljson = new StringBuilder();
            finaljson.Append("{");
            for (int i = 0; i < numberOfTypes; i++)
            {
                finaljson.Append("\"" + XMLParser.getMetaInfoTypes(XMLParser.getParentNodeOfData(doc)).ChildNodes[i].Attributes[1].Value + "\" : [");
                finaljson.Append(builders[i].ToString());
                finaljson.Remove(finaljson.Length - 2, 1);
                finaljson.Append("],");
            }
            finaljson.Remove(finaljson.Length - 1, 1);
            finaljson.Append("}");
            return finaljson.ToString();
        }



     
     */
}
