using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using TDMtoTDSMigrator;
using System.Diagnostics;

namespace TDMtoTDSMigrator
{
    
    public class TdsLoader
    {

        public static string DecompressTddFileIntoXml(FileInfo fi)
        {
            using (FileStream inFile = fi.OpenRead())
            {
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length - fi.Extension.Length);
 
                string pathOfOutput = origName + ".xml";
                using (FileStream outFile = File.Create(pathOfOutput))
                {
                    using (GZipStream decompress = new GZipStream(inFile,
                            CompressionMode.Decompress))
                    {
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = decompress.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFile.Write(buffer, 0, numRead);
                        }

                    }
                }
                return pathOfOutput;
            }

        }



        public static Task<HttpResponseMessage> MigrateXmlDataIntoTdsWithoutFilter(string xmlPath, List<TableObject> dataList, string repositoryName, string apiUrl)
        {

            XmlNode metaInfoAttributes = XmlParser.GetMetaInfoAttributes(xmlPath);
            XmlNode metaInfoType = XmlParser.GetMetaInfoTypes(xmlPath);

            Task<HttpResponseMessage> message = null;

            for (int i = 0; i < dataList.Count; i++)
            {
                message = HttpRequest.PostObject(JsonConverter.ConvertObjectIntoJsonPostRequest(dataList[i], metaInfoAttributes),repositoryName, apiUrl);
            }

            // returns the response of the last request
            return message;
        }
        public static Task<HttpResponseMessage> MigrateXmlDataIntoTdsWithFilter(string xmlPath, List<TableObject> dataList, string repositoryName, List<string> filteredCategories, string apiUrl)
        {
            XmlNode metaInfoAttributes = XmlParser.GetMetaInfoAttributes(xmlPath);
            //XmlNode metaInfoType = XmlParser.GetMetaInfoTypes(xmlPath);

            Task<HttpResponseMessage> message = null;

            for (int i = 0; i < dataList.Count; i++)
            {
                if (filteredCategories.Contains(dataList[i].GetCategoryName()))
                {
                    message = HttpRequest.PostObject(JsonConverter.ConvertObjectIntoJsonPostRequest(dataList[i], metaInfoAttributes), repositoryName, apiUrl);  
                }
            }

            // returns the response of the last request
            return message;
        }
    }
}
