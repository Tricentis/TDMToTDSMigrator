﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;


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
            Task<HttpResponseMessage> message = null;
            foreach (var obj in dataList)
            {
                message = HttpRequest.PostObject(JsonConverter.ConvertObjectIntoJsonPostRequest(obj), repositoryName, apiUrl);
            }
            return message;
        }



        public static Task<HttpResponseMessage> MigrateXmlDataIntoTdsWithFilter(string xmlPath, List<TableObject> dataList, string repositoryName, List<string> filteredCategories, string apiUrl)
        {
            Task<HttpResponseMessage> message = null;

            foreach (var obj in dataList)
            {
                if (filteredCategories.Contains(obj.GetCategoryName()))
                {
                    message = HttpRequest.PostObject(JsonConverter.ConvertObjectIntoJsonPostRequest(obj), repositoryName, apiUrl);
                }
            }
            return message;
        }
    }
}
